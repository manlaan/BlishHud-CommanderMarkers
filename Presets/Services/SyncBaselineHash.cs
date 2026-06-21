using Manlaan.CommanderMarkers.Presets.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Manlaan.CommanderMarkers.Presets.Services;

public static class SyncBaselineHash
{
    public static string Compute(MarkerSet markerSet)
    {
        var payload = CanonicalPayloadJson(markerSet);
        var json = JsonConvert.SerializeObject(payload);
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    private static JObject CanonicalPayloadJson(MarkerSet markerSet)
    {
        var sorted = markerSet.marks.OrderBy(m => m.icon).ToList();
        var markers = new JArray();
        foreach (var marker in sorted)
        {
            var row = new JObject
            {
                ["i"] = marker.icon,
                ["x"] = marker.x,
                ["y"] = marker.y,
                ["z"] = marker.z
            };
            if (!string.IsNullOrEmpty(marker.name))
            {
                row["d"] = marker.name;
            }
            markers.Add(row);
        }

        var trigger = markerSet.trigger ?? new WorldCoord();
        return new JObject
        {
            ["name"] = markerSet.name,
            ["description"] = markerSet.description,
            ["mapId"] = markerSet.mapId,
            ["trigger"] = new JObject
            {
                ["x"] = trigger.x,
                ["y"] = trigger.y,
                ["z"] = trigger.z
            },
            ["markers"] = markers
        };
    }
}
