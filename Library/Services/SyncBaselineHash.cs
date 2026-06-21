using Manlaan.CommanderMarkers.Presets.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Manlaan.CommanderMarkers.Library.Services;

public static class SyncBaselineHash
{
    public static string Compute(MarkerSet markerSet)
    {
        var markers = markerSet.marks
            .OrderBy(m => m.icon)
            .Select(m =>
            {
                var row = new JObject
                {
                    ["i"] = m.icon,
                    ["x"] = m.x,
                    ["y"] = m.y,
                    ["z"] = m.z
                };
                if (!string.IsNullOrWhiteSpace(m.name))
                {
                    row["d"] = m.name;
                }
                return row;
            })
            .ToList();

        var payload = new JObject
        {
            ["name"] = markerSet.name ?? "",
            ["description"] = markerSet.description ?? "",
            ["mapId"] = markerSet.mapId ?? 0,
            ["trigger"] = JObject.FromObject(markerSet.Trigger),
            ["markers"] = new JArray(markers)
        };

        var json = payload.ToString(Formatting.None);
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
