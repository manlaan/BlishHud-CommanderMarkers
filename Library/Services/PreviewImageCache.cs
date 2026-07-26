using Blish_HUD;
using Blish_HUD.Content;
using Manlaan.CommanderMarkers.Library.Models;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Library.Services;

public class PreviewImageCache : IDisposable
{
    private readonly Dictionary<string, Texture2D> _textures = new();
    private readonly HashSet<string> _thumbDownloadsInFlight = new();
    private readonly HashSet<string> _previewDownloadsInFlight = new();
    private readonly object _downloadLock = new();
    private string _moduleDirectory = "";
    private string _serverUrl = "";

    public void SetModuleDirectory(string moduleDirectory) => _moduleDirectory = moduleDirectory;
    public void SetServerUrl(string serverUrl) => _serverUrl = serverUrl;

    public string? ThumbPathForSet(string communitySetId)
    {
        if (string.IsNullOrEmpty(communitySetId))
        {
            return null;
        }

        var path = GetThumbFilePath(communitySetId);
        return File.Exists(path) ? path : null;
    }

    public Texture2D? GetThumbTexture(string communitySetId, Texture2D fallback)
    {
        var path = ThumbPathForSet(communitySetId);
        if (path == null)
        {
            return null;
        }

        return LoadTexture(communitySetId, path);
    }

    public string? PreviewPathForSet(string communitySetId)
    {
        if (string.IsNullOrEmpty(communitySetId))
        {
            return null;
        }

        var path = GetPreviewFilePath(communitySetId);
        return File.Exists(path) ? path : null;
    }

    public Texture2D? GetPreviewTexture(string communitySetId)
    {
        var path = PreviewPathForSet(communitySetId);
        if (path == null)
        {
            return null;
        }

        return LoadTexture("preview:" + communitySetId, path);
    }

    private Texture2D? LoadTexture(string cacheKey, string path)
    {
        if (_textures.TryGetValue(cacheKey, out var cached))
        {
            return cached;
        }

        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var texture = TextureUtil.FromStreamPremultiplied(stream);
            _textures[cacheKey] = texture;
            return texture;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public void RequestPreview(string communitySetId, string previewLargeUrl, Action<string>? onReady = null)
    {
        if (string.IsNullOrEmpty(communitySetId))
        {
            return;
        }

        var existing = PreviewPathForSet(communitySetId);
        if (existing != null)
        {
            NotifyReady(onReady, existing);
            return;
        }

        lock (_downloadLock)
        {
            if (!_previewDownloadsInFlight.Add(communitySetId))
            {
                return;
            }
        }

        var url = ResolvePreviewDownloadUrl(communitySetId, previewLargeUrl);
        Task.Run(() =>
        {
            try
            {
                if (DownloadPreview(communitySetId, url))
                {
                    var path = PreviewPathForSet(communitySetId);
                    if (path != null)
                    {
                        NotifyReady(onReady, path);
                    }
                }
            }
            finally
            {
                lock (_downloadLock)
                {
                    _previewDownloadsInFlight.Remove(communitySetId);
                }
            }
        });
    }

    public void RequestThumb(string communitySetId, string previewThumbUrl, Action<string>? onReady = null)
    {
        if (string.IsNullOrEmpty(communitySetId))
        {
            return;
        }

        var existing = ThumbPathForSet(communitySetId);
        if (existing != null)
        {
            NotifyReady(onReady, existing);
            return;
        }

        lock (_downloadLock)
        {
            if (!_thumbDownloadsInFlight.Add(communitySetId))
            {
                return;
            }
        }

        var url = ResolveDownloadUrl(communitySetId, previewThumbUrl);
        Task.Run(() =>
        {
            try
            {
                if (DownloadThumb(communitySetId, url))
                {
                    var path = ThumbPathForSet(communitySetId);
                    if (path != null)
                    {
                        NotifyReady(onReady, path);
                    }
                }
            }
            finally
            {
                lock (_downloadLock)
                {
                    _thumbDownloadsInFlight.Remove(communitySetId);
                }
            }
        });
    }

    private static void NotifyReady(Action<string>? onReady, string path)
    {
        if (onReady == null)
        {
            return;
        }

        GameThreadUtil.Enqueue(() => onReady(path));
    }

    private string GetThumbFilePath(string communitySetId) =>
        Path.Combine(_moduleDirectory, "thumbs", communitySetId + ".png");

    private string GetPreviewFilePath(string communitySetId) =>
        Path.Combine(_moduleDirectory, "previews", communitySetId + ".png");

    private string ResolveDownloadUrl(string communitySetId, string previewThumbUrl)
    {
        if (!string.IsNullOrEmpty(previewThumbUrl))
        {
            if (previewThumbUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                previewThumbUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return previewThumbUrl;
            }

            var baseUrl = _serverUrl.TrimEnd('/');
            return previewThumbUrl.StartsWith("/") ? baseUrl + previewThumbUrl : baseUrl + "/" + previewThumbUrl;
        }

        var server = _serverUrl.TrimEnd('/');
        return server + "/commander-markers/v1/sets/" + communitySetId + "/thumb.png";
    }

    private string ResolvePreviewDownloadUrl(string communitySetId, string previewLargeUrl)
    {
        if (!string.IsNullOrEmpty(previewLargeUrl))
        {
            if (previewLargeUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                previewLargeUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return previewLargeUrl;
            }

            var baseUrl = _serverUrl.TrimEnd('/');
            return previewLargeUrl.StartsWith("/") ? baseUrl + previewLargeUrl : baseUrl + "/" + previewLargeUrl;
        }

        var server = _serverUrl.TrimEnd('/');
        return server + "/commander-markers/v1/sets/" + communitySetId + "/preview.png";
    }

    private bool DownloadThumb(string communitySetId, string url)
    {
        try
        {
            using var client = ModuleHttp.CreateClient();
            var bytes = client.DownloadData(url);
            if (bytes.Length == 0)
            {
                return false;
            }

            var dir = Path.Combine(_moduleDirectory, "thumbs");
            Directory.CreateDirectory(dir);
            File.WriteAllBytes(GetThumbFilePath(communitySetId), bytes);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool DownloadPreview(string communitySetId, string url)
    {
        try
        {
            using var client = ModuleHttp.CreateClient();
            var bytes = client.DownloadData(url);
            if (bytes.Length == 0)
            {
                return false;
            }

            var dir = Path.Combine(_moduleDirectory, "previews");
            Directory.CreateDirectory(dir);
            File.WriteAllBytes(GetPreviewFilePath(communitySetId), bytes);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void Dispose()
    {
        foreach (var texture in _textures.Values)
        {
            texture?.Dispose();
        }
        _textures.Clear();
    }
}
