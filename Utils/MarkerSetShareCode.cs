using Manlaan.CommanderMarkers.Presets.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Manlaan.CommanderMarkers.Utils;

public static class MarkerSetShareCode
{
    public static string Export(MarkerSet markerSet)
    {
        if (!string.IsNullOrEmpty(markerSet.communitySetId) && !markerSet.syncDetached)
        {
            var payload = new JObject
            {
                ["shareType"] = "community",
                ["communitySetId"] = markerSet.communitySetId,
                ["name"] = markerSet.name ?? ""
            };
            return Encode(payload.ToString(Formatting.None));
        }

        return Encode(JsonConvert.SerializeObject(markerSet));
    }

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
            var shareType = j.Value<string>("shareType");
            var hasCommunityId = !string.IsNullOrWhiteSpace(j.Value<string>("communitySetId"));
            var hasMap = j["mapId"] != null;
            var isCommunityRef = shareType == "community" || (hasCommunityId && !hasMap);
            if (isCommunityRef)
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

    private static string Encode(string json) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

    private static string Decode(string text) =>
        Encoding.UTF8.GetString(Convert.FromBase64String(text));
}
