using Blish_HUD.Content;
using Manlaan.CommanderMarkers.Library.Models;
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

        if (_textures.TryGetValue(communitySetId, out var cached))
        {
            return cached;
        }

        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var texture = TextureUtil.FromStreamPremultiplied(stream);
            _textures[communitySetId] = texture;
            return texture;
        }
        catch (Exception)
        {
            return null;
        }
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
            onReady?.Invoke(existing);
            return;
        }

        var url = ResolveDownloadUrl(communitySetId, previewThumbUrl);
        Task.Run(() =>
        {
            if (DownloadThumb(communitySetId, url))
            {
                var path = ThumbPathForSet(communitySetId);
                if (path != null)
                {
                    onReady?.Invoke(path);
                }
            }
        });
    }

    private string GetThumbFilePath(string communitySetId) =>
        Path.Combine(_moduleDirectory, "thumbs", communitySetId + ".png");

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

    private bool DownloadThumb(string communitySetId, string url)
    {
        try
        {
            using var client = new WebClient();
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

    public void Dispose()
    {
        foreach (var texture in _textures.Values)
        {
            texture?.Dispose();
        }
        _textures.Clear();
    }
}
