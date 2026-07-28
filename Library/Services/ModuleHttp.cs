using System;
using System.Net;
using System.Reflection;

namespace Manlaan.CommanderMarkers.Library.Services;

/// <summary>
/// Shared HTTP helpers for community API traffic. Sets an identifiable User-Agent
/// so the server can classify Blish Commander Markers clients.
/// </summary>
public static class ModuleHttp
{
    public const string UserAgentPrefix = "COMM-MARKERS-Blish";
    private static string? _cachedVersion;

    public static string UserAgent => $"{UserAgentPrefix}/{ResolveVersion()}";

    public static WebClient CreateClient()
    {
        var client = new WebClient();
        client.Headers[HttpRequestHeader.UserAgent] = UserAgent;
        return client;
    }

    private static string ResolveVersion()
    {
        if (!string.IsNullOrEmpty(_cachedVersion))
        {
            return _cachedVersion!;
        }

        try
        {
            var module = Service.ModuleInstance;
            if (module != null)
            {
                // Blish Module.Version is SemVer.Version — avoid a hard SemVer package reference.
                var versionProp = module.GetType().GetProperty("Version", BindingFlags.Instance | BindingFlags.Public);
                var value = versionProp?.GetValue(module)?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _cachedVersion = value!;
                    return _cachedVersion;
                }
            }
        }
        catch (Exception)
        {
            // Module may not be ready during very early init.
        }

        _cachedVersion = "0.0.0";
        return _cachedVersion;
    }
}
