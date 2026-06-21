using Manlaan.CommanderMarkers.Library.Models;
using Manlaan.CommanderMarkers.Presets.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Manlaan.CommanderMarkers.Library.Services;

public class CommunityCatalogService
{
    public const string IndexFileName = "community_index.json";

    private readonly CommanderMarkersManifestService _manifestService;
    private readonly string _moduleDirectory;
    private readonly List<CommunitySetSummary> _sets = new();
    private readonly List<CommunityCategoryEntry> _categories = new();
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
            using var client = new WebClient();
            var checkUrl = manifest.Absolute(manifest.CommunityCheckUrl);
            var checkJson = JObject.Parse(client.DownloadString(checkUrl));
            var remoteLastEdit = checkJson.Value<string>("lastEdit") ?? "";
            if (!string.IsNullOrEmpty(remoteLastEdit) && remoteLastEdit == _lastEdit && _sets.Count > 0)
            {
                return false;
            }

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
            SaveIndex();
            CatalogUpdated?.Invoke(this, EventArgs.Empty);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public MarkerSet? FetchSetDetail(string setId)
    {
        if (string.IsNullOrWhiteSpace(setId))
        {
            return null;
        }

        try
        {
            using var client = new WebClient();
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
            return markerSet;
        }
        catch (Exception)
        {
            return null;
        }
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
