using Manlaan.CommanderMarkers.Presets.Model;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Manlaan.CommanderMarkers.Utils;

public static class MarkerSetSubmission
{
    public static JObject ToSubmissionPayload(MarkerSet markerSet, string suggestedCategory)
    {
        var markers = new JArray();
        foreach (var mark in markerSet.marks)
        {
            var row = new JObject
            {
                ["i"] = mark.icon,
                ["x"] = mark.x,
                ["y"] = mark.y,
                ["z"] = mark.z
            };
            if (!string.IsNullOrWhiteSpace(mark.name))
            {
                row["d"] = mark.name;
            }
            markers.Add(row);
        }

        return new JObject
        {
            ["name"] = markerSet.name ?? "",
            ["description"] = markerSet.description ?? "",
            ["mapId"] = markerSet.mapId ?? 0,
            ["enabled"] = markerSet.enabled,
            ["trigger"] = markerSet.trigger != null
                ? JObject.FromObject(markerSet.trigger)
                : new JObject { ["x"] = 0, ["y"] = 0, ["z"] = 0 },
            ["markers"] = markers,
            ["suggestedCategory"] = suggestedCategory
        };
    }
}
