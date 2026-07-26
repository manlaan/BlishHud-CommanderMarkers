using Manlaan.CommanderMarkers.Library.Models;
using Manlaan.CommanderMarkers.Presets.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Library.Services;

public class CommunityCatalogService
{
    public const string IndexFileName = "community_index.json";
    private const int MaxDetailCacheEntries = 100;

    private readonly CommanderMarkersManifestService _manifestService;
    private readonly string _moduleDirectory;
    private readonly List<CommunitySetSummary> _sets = new();
    private readonly List<CommunityCategoryEntry> _categories = new();
    private readonly ConcurrentDictionary<string, (MarkerSet Set, long Version)> _detailCache = new();
    private readonly ConcurrentDictionary<string, Task<MarkerSet?>> _detailInflight = new();
    private readonly ConcurrentQueue<(string SetId, long Version)> _detailCacheOrder = new();
    private long _detailCacheVersion;
    private string _lastEdit = "";

    public event EventHandler? CatalogUpdated;

    public CommunityCatalogService(CommanderMarkersManifestService manifestService, string moduleDirectory)
    {
        _manifestService = manifestService;
        _moduleDirectory = moduleDirectory;
    }

    public IReadOnlyList<CommunitySetSummary> Sets => _sets;
    public IReadOnlyList<CommunityCategoryEntry> Categories => _categories;

    public void LoadCached()
    {
        var path = Path.Combine(_moduleDirectory, IndexFileName);
        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            var j = JObject.Parse(File.ReadAllText(path));
            _lastEdit = j.Value<string>("lastEdit") ?? "";
            _sets.Clear();
            if (j["sets"] is JArray setsArray)
            {
                foreach (var row in setsArray)
                {
                    var summary = row.ToObject<CommunitySetSummary>();
                    if (summary != null)
                    {
                        _sets.Add(summary);
                    }
                }
            }
        }
        catch (Exception)
        {
            // Ignore corrupt cache.
        }
    }

    public bool SyncCatalog()
    {
        var manifest = _manifestService.Manifest;
        try
        {
            using var client = ModuleHttp.CreateClient();
            var checkUrl = manifest.Absolute(manifest.CommunityCheckUrl);
            var checkJson = JObject.Parse(client.DownloadString(checkUrl));
            var remoteLastEdit = checkJson.Value<string>("lastEdit") ?? "";
            if (!string.IsNullOrEmpty(remoteLastEdit) && remoteLastEdit == _lastEdit && _sets.Count > 0)
            {
                return false;
            }

            var previousLastEdit = _lastEdit;
            var fetched = new List<CommunitySetSummary>();
            var offset = 0;
            const int limit = 200;
            var total = -1;
            while (total < 0 || offset < total)
            {
                var pageUrl = $"{manifest.Absolute(manifest.SetsUrl)}?limit={limit}&offset={offset}";
                var page = JsonConvert.DeserializeObject<CommunitySetsPage>(client.DownloadString(pageUrl));
                if (page == null)
                {
                    break;
                }

                total = page.Total;
                fetched.AddRange(page.Sets);
                offset += limit;
                if (page.Sets.Count == 0)
                {
                    break;
                }
            }

            try
            {
                var categoriesJson = client.DownloadString(manifest.Absolute(manifest.CategoriesUrl));
                var categories = JsonConvert.DeserializeObject<List<CommunityCategoryEntry>>(categoriesJson) ??
                                 new List<CommunityCategoryEntry>();
                _categories.Clear();
                _categories.AddRange(categories);
            }
            catch (Exception)
            {
                _categories.Clear();
            }

            _sets.Clear();
            _sets.AddRange(fetched);
            _lastEdit = remoteLastEdit;
            if (previousLastEdit != remoteLastEdit)
            {
                ClearDetailCache();
            }

            SaveIndex();
            CatalogUpdated?.Invoke(this, EventArgs.Empty);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Returns a cloned marker set for the given community id. Uses an in-memory cache
    /// and coalesces concurrent fetches for the same id.
    /// </summary>
    public MarkerSet? FetchSetDetail(string setId)
    {
        if (string.IsNullOrWhiteSpace(setId))
        {
            return null;
        }

        if (_detailCache.TryGetValue(setId, out var cached))
        {
            TouchDetailCache(setId, cached);
            return CloneMarkerSet(cached.Set);
        }

        var task = _detailInflight.GetOrAdd(setId, id => Task.Run(() => DownloadSetDetail(id)));
        try
        {
            var result = task.GetAwaiter().GetResult();
            return result == null ? null : CloneMarkerSet(result);
        }
        finally
        {
            _detailInflight.TryRemove(setId, out _);
        }
    }

    private MarkerSet? DownloadSetDetail(string setId)
    {
        if (_detailCache.TryGetValue(setId, out var cached))
        {
            TouchDetailCache(setId, cached);
            return cached.Set;
        }

        try
        {
            using var client = ModuleHttp.CreateClient();
            var url = _manifestService.Manifest.Resolve(_manifestService.Manifest.SetDetailUrl, setId);
            var json = client.DownloadString(url);
            var summary = _sets.FirstOrDefault(s => s.Id == setId);
            var markerSet = JsonConvert.DeserializeObject<MarkerSet>(json);
            if (markerSet == null)
            {
                return null;
            }

            markerSet.id = Guid.NewGuid().ToString();
            markerSet.communitySetId = setId;
            markerSet.author = summary?.Author;
            markerSet.communityUpdatedAt = summary?.UpdatedAt;
            markerSet.source = "community";
            markerSet.syncDetached = false;
            markerSet.localModifiedAt = null;
            markerSet.syncBaselineHash = SyncBaselineHash.Compute(markerSet);

            StoreDetailCache(setId, markerSet);
            return markerSet;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private void StoreDetailCache(string setId, MarkerSet markerSet)
    {
        // Store a stable clone so callers cannot mutate the cached instance.
        var stored = CloneMarkerSet(markerSet);
        var version = Interlocked.Increment(ref _detailCacheVersion);
        _detailCache[setId] = (stored, version);
        _detailCacheOrder.Enqueue((setId, version));
        TrimDetailCache();
    }

    /// <summary>
    /// Marks a cache hit as most-recently used. ConcurrentQueue cannot move entries, so we
    /// enqueue a new version and ignore stale versions during eviction.
    /// </summary>
    private void TouchDetailCache(string setId, (MarkerSet Set, long Version) current)
    {
        var version = Interlocked.Increment(ref _detailCacheVersion);
        if (_detailCache.TryUpdate(setId, (current.Set, version), current))
        {
            _detailCacheOrder.Enqueue((setId, version));
        }
    }

    private void TrimDetailCache()
    {
        while (_detailCache.Count > MaxDetailCacheEntries &&
               _detailCacheOrder.TryDequeue(out var oldest))
        {
            // Only evict when this queue entry is still the live version; otherwise it is a
            // stale duplicate left behind by an update or cache hit.
            if (_detailCache.TryGetValue(oldest.SetId, out var current) &&
                current.Version == oldest.Version)
            {
                _detailCache.TryRemove(
                    new KeyValuePair<string, (MarkerSet Set, long Version)>(oldest.SetId, current));
            }
        }
    }

    private void ClearDetailCache()
    {
        _detailCache.Clear();
        while (_detailCacheOrder.TryDequeue(out _))
        {
        }

        // In-flight tasks may still complete and re-populate; that is fine after a sync.
    }

    private static MarkerSet CloneMarkerSet(MarkerSet source)
    {
        // Round-trip JSON for a deep clone; callers mutate id / placement fields.
        return JsonConvert.DeserializeObject<MarkerSet>(JsonConvert.SerializeObject(source))
               ?? new MarkerSet();
    }

    private void SaveIndex()
    {
        Directory.CreateDirectory(_moduleDirectory);
        var payload = new JObject
        {
            ["lastEdit"] = _lastEdit,
            ["fetchedAt"] = DateTime.UtcNow.ToString("o"),
            ["sets"] = JArray.FromObject(_sets)
        };
        File.WriteAllText(Path.Combine(_moduleDirectory, IndexFileName), payload.ToString(Formatting.Indented));
    }

    private sealed class CommunitySetsPage
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("sets")]
        public List<CommunitySetSummary> Sets { get; set; } = new();
    }
}
