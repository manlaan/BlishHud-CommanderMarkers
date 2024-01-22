using Blish_HUD;
using Gw2Sharp.WebApi.V2.Models;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Newtonsoft.Json;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading;
using System.Threading.Tasks;
using static Blish_HUD.GameService;
using static Manlaan.CommanderMarkers.Presets.ScreenMap;
using File = System.IO.File;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Manlaan.CommanderMarkers.Presets;

public class MapData : IDisposable
{
    public static string MAPDATA_CACHE_FILENAME = "map_data_cache.json";
    private static readonly Logger Logger = Logger.GetLogger<MapData>();
    private const int Retries = 3;

    public Map? Current { get; private set; }

    private readonly Dictionary<int, Map> _maps = new();
    private readonly CancellationTokenSource _cts = new();

    private class CacheFile
    {
        public int BuildId { get; set; }
        public Dictionary<int, Map> Maps { get; set; } = new();
    }



    public MapData(string cacheFilePath)
    {
        CacheFile? cache = null;
        if (File.Exists(cacheFilePath))
        {
            Logger.Info("Found cache file, loading.");
            try
            {
                cache = JsonSerializer.Deserialize<CacheFile>(File.ReadAllText(cacheFilePath));
                if (cache == null)
                    Logger.Warn("Cache load resulted in null.");
            }
            catch (Exception e)
            {
                Logger.Warn(e, "Exception when loading cache.");
            }
        }

        _maps = cache?.Maps ?? new Dictionary<int, Map>();

        _ = LoadMapData(cache?.BuildId ?? 0, cacheFilePath, _cts.Token);
        Gw2Mumble.CurrentMap.MapChanged += CurrentMapChanged;
    }

    public string Describe(int mapId)
        => GetMap(mapId)?.Name ?? $"({mapId})";

    public Map? GetMap(int id)
    {
        lock (_maps)
        {
            return _maps.TryGetValue(id, out var map) ? map : null;
        }
    }

    public Vector2 WorldToScreenMap(Vector3 worldMeters)
        => WorldToScreenMap(Gw2Mumble.CurrentMap.Id, worldMeters);

    public Vector2 WorldToScreenMap(int mapId, Vector3 worldMeters)
        => WorldToScreenMap(mapId, worldMeters, ScreenMap.Data.MapCenter, ScreenMap.Data.Scale, ScreenMap.Data.MapRotation, ScreenMap.Data.BoundsCenter);

    public Vector2 WorldToScreenMap(int mapId, Vector3 worldMeters, Vector2 mapCenter, float scale, Matrix rotation, Vector2 boundsCenter)
        => GetMap(mapId) is Map map
            ? MapToScreenMap(map.WorldMetersToMap(worldMeters), mapCenter, scale, rotation, boundsCenter)
            : Vector2.Zero;

    public static Vector2 MapToScreenMap(Vector2 mapCoords)
        => MapToScreenMap(mapCoords, ScreenMap.Data.MapCenter, ScreenMap.Data.Scale, ScreenMap.Data.MapRotation, ScreenMap.Data.BoundsCenter);

    public static Vector2 MapToScreenMap(Vector2 mapCoords, Vector2 mapCenter, float scale, Matrix rotation, Vector2 boundsCenter)
        => Vector2.Transform((mapCoords - mapCenter) * scale, rotation) + boundsCenter;

    public Vector2 ScreenMapToMap(Vector2 screenMapCoord)
    {

        Vector2 boundsCenter = ScreenMap.Data.BoundsCenter;
        Matrix rotation = ScreenMap.Data.MapRotation;
        float scale = ScreenMap.Data.Scale;
        Vector2 mapCenter = ScreenMap.Data.MapCenter;

        ;
        return Vector2.Transform((screenMapCoord - boundsCenter) / scale , - rotation) + mapCenter;

    }
    public Vector3 MapToWorld(Vector2 mapCoords)
    {
        int mapId = Gw2Mumble.CurrentMap.Id;
        return GetMap(mapId) is Map map ? map.MapToWorldMeters(mapCoords) : Vector3.Zero;

    }

    private async Task LoadMapData(int cachedVersion, string cacheFilePath, CancellationToken ct)
    {
        for (int i = 0; i < 30; i++)
        {
            if (Gw2Mumble.Info.BuildId != 0)
                break;

            Logger.Warn("Waiting for mumble to update map data...");
            await Task.Delay(1000);
        }

        if (Gw2Mumble.Info.BuildId == cachedVersion)
        {
            UpdateCurrent();
            return;
        }

        IEnumerable<Map>? maps = null;

        for (int i = 0; i < Retries; i++)
        {
            try
            {
                var maps2 = await Gw2WebApi.AnonymousConnection.Client.V2.Maps.AllAsync(ct);
                maps = maps2;
                break;
            }
            catch (Exception e)
            {
                if (i < Retries)
                {
                    Logger.Warn(e, "Failed to pull map data from the Gw2 API. Trying again in 30 seconds.");
                    await Task.Delay(30000);
                }
            }
        }

        if (maps == null)
        {
            Logger.Warn("Max retries exeeded. Skipping map data update.");
            return;  // We failed to load any map data.
        }

        lock (_maps)
        {
            foreach (var map in maps)
                _maps[map.Id] = map;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(cacheFilePath));
        File.WriteAllText(cacheFilePath, JsonConvert.SerializeObject(new CacheFile()
        {
            BuildId = Gw2Mumble.Info.BuildId != 0 ? Gw2Mumble.Info.BuildId : cachedVersion,
            Maps = _maps,
        }));

        UpdateCurrent();
    }

    private void UpdateCurrent()
    {
        lock (_maps)
        {
            Current = _maps.TryGetValue(Gw2Mumble.CurrentMap.Id, out var map) ? map : null;
        }
    }

    private void CurrentMapChanged(object sender, ValueEventArgs<int> e) => UpdateCurrent();

    public void Dispose()
    {
        Gw2Mumble.CurrentMap.MapChanged -= CurrentMapChanged;
        _cts?.Cancel();
    }
}