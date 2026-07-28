using Manlaan.CommanderMarkers.Library;
using Manlaan.CommanderMarkers.Library.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Manlaan.CommanderMarkers.Library.Services;

public class CommanderMarkersManifestService
{
    public static string ManifestUrl => DevEndpoints.ManifestUrl;

    private CommanderMarkersManifest _manifest = new();
    private bool _loaded;

    public CommanderMarkersManifest Manifest => _manifest;
    public bool IsLoaded => _loaded;

    public void LoadOrFetch()
    {
        try
        {
            using var client = ModuleHttp.CreateClient();
            var json = client.DownloadString(ManifestUrl);
            var j = JObject.Parse(json);
#if DEBUG
            _manifest.ServerUrl = DevEndpoints.ApiBaseUrl;
#else
            _manifest.ServerUrl = j.Value<string>("server_url") ?? _manifest.ServerUrl;
#endif
            _manifest.CommunityCheckUrl = j.Value<string>("community_check_url") ?? _manifest.CommunityCheckUrl;
            _manifest.CommunityMarkersUrl = j.Value<string>("community_markers_url") ?? _manifest.CommunityMarkersUrl;
            _manifest.SetsUrl = j.Value<string>("sets_url") ?? _manifest.SetsUrl;
            _manifest.SetDetailUrl = j.Value<string>("set_detail_url") ?? _manifest.SetDetailUrl;
            _manifest.ThumbUrl = j.Value<string>("thumb_url") ?? _manifest.ThumbUrl;
            _manifest.CategoriesUrl = j.Value<string>("categories_url") ?? _manifest.CategoriesUrl;
            _manifest.SubmissionsUrl = j.Value<string>("submissions_url") ?? _manifest.SubmissionsUrl;
            _manifest.SubmissionsMineUrl = j.Value<string>("submissions_mine_url") ?? _manifest.SubmissionsMineUrl;
            _manifest.SubtokenUrl = j.Value<string>("subtoken_url") ?? _manifest.SubtokenUrl;
            _manifest.LibraryUrl = j.Value<string>("library_url") ?? _manifest.LibraryUrl;
        }
        catch (Exception)
        {
            // Keep defaults when manifest fetch fails.
        }

        _loaded = true;
    }
}
