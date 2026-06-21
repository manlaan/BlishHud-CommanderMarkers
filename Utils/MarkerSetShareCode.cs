using Manlaan.CommanderMarkers.Presets.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Manlaan.CommanderMarkers.Utils;

public static class MarkerSetShareCode
{
    public static string Export(MarkerSet markerSet) =>
        Encode(ToPortablePayload(markerSet).ToString(Formatting.None));

    public static MarkerSet? Import(string text, Func<string, string, MarkerSet?>? resolveCommunity = null)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        string json;
        try
        {
            json = Decode(text.Trim());
        }
        catch (Exception)
        {
            json = text.Trim();
        }

        try
        {
            var j = JObject.Parse(json);
            if (IsCommunityShareRef(j))
            {
                var communitySetId = j.Value<string>("communitySetId") ?? "";
                var name = j.Value<string>("name") ?? "";
                if (string.IsNullOrEmpty(communitySetId) || resolveCommunity == null)
                {
                    return null;
                }
                return resolveCommunity(communitySetId, name);
            }

            return j.ToObject<MarkerSet>();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static JObject ToPortablePayload(MarkerSet markerSet)
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
            ["mapId"] = markerSet.MapId,
            ["enabled"] = markerSet.enabled,
            ["trigger"] = markerSet.trigger != null
                ? JObject.FromObject(markerSet.trigger)
                : new JObject { ["x"] = 0, ["y"] = 0, ["z"] = 0 },
            ["markers"] = markers
        };
    }

    private static bool IsCommunityShareRef(JObject j)
    {
        if (string.Equals(j.Value<string>("shareType"), "community", StringComparison.Ordinal))
        {
            return true;
        }

        var communitySetId = j.Value<string>("communitySetId");
        if (string.IsNullOrWhiteSpace(communitySetId))
        {
            return false;
        }

        var hasMapPayload = j["mapId"] != null && j["mapId"].Type != JTokenType.Null;
        if (!hasMapPayload && j["markers"] is JArray markers && markers.Count > 0)
        {
            hasMapPayload = true;
        }
        if (!hasMapPayload && j["marks"] is JArray marks && marks.Count > 0)
        {
            hasMapPayload = true;
        }

        return !hasMapPayload;
    }

    private static string Encode(string json) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

    private static string Decode(string text) =>
        Encoding.UTF8.GetString(Convert.FromBase64String(text));
}
