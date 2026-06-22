namespace Manlaan.CommanderMarkers.Library;

internal static class DevEndpoints
{
#if DEBUG
    public const string ApiBaseUrl = "http://localhost:3000";
    public const string ManifestUrl = ApiBaseUrl + "/commander_markers_v1.json";
    public const string LegacyCommunityMarkersUrl = ApiBaseUrl + "/commander-markers/v1/community/markers.json";
#else
    public const string ApiBaseUrl = "https://gw2geoguesser.fly.dev";
    public const string ManifestUrl = ApiBaseUrl + "/commander_markers_v1.json";
    public const string LegacyCommunityMarkersUrl =
        "https://bhm.blishhud.com/Manlaan.CommanderMarkers/Community/Markers.json";
#endif
}
