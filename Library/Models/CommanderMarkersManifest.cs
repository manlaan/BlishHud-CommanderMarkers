namespace Manlaan.CommanderMarkers.Library.Models;

public class CommanderMarkersManifest
{
    public string ServerUrl { get; set; } = "https://gw2geoguesser.fly.dev";
    public string CommunityCheckUrl { get; set; } = "/commander-markers/v1/community/check";
    public string CommunityMarkersUrl { get; set; } = "/commander-markers/v1/community/markers.json";
    public string SetsUrl { get; set; } = "/commander-markers/v1/sets";
    public string SetDetailUrl { get; set; } = "/commander-markers/v1/sets/{id}";
    public string ThumbUrl { get; set; } = "/commander-markers/v1/sets/{id}/thumb.png";
    public string CategoriesUrl { get; set; } = "/commander-markers/v1/categories";
    public string SubmissionsUrl { get; set; } = "/commander-markers/v1/submissions";
    public string SubmissionsMineUrl { get; set; } = "/commander-markers/v1/submissions/mine";
    public string SubtokenUrl { get; set; } = "/v4/auth/subtoken";
    public string LibraryUrl { get; set; } = "/markers";

    public string Resolve(string pathTemplate, string id = "")
    {
        var path = pathTemplate;
        var pos = path.IndexOf("{id}", System.StringComparison.Ordinal);
        if (pos >= 0)
        {
            path = path.Remove(pos, 4).Insert(pos, id);
        }
        return Absolute(path);
    }

    public string Absolute(string path)
    {
        if (path.StartsWith("http://", System.StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("https://", System.StringComparison.OrdinalIgnoreCase))
        {
            return path;
        }

        var baseUrl = ServerUrl.TrimEnd('/');
        if (string.IsNullOrEmpty(path))
        {
            return baseUrl;
        }

        return path.StartsWith("/") ? baseUrl + path : baseUrl + "/" + path;
    }
}
