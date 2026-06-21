using Manlaan.CommanderMarkers.Presets.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Utils;

public sealed class CommunityShareResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = "";
}

public static class CommunityShareHelper
{
    public static List<string> CategoryNames()
    {
        var names = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var category in Service.CommunityCatalog.Categories)
        {
            if (!string.IsNullOrWhiteSpace(category.Name) && seen.Add(category.Name))
            {
                names.Add(category.Name);
            }
        }

        if (names.Count == 0)
        {
            foreach (var summary in Service.CommunityCatalog.Sets)
            {
                if (!string.IsNullOrWhiteSpace(summary.CategoryName) && seen.Add(summary.CategoryName))
                {
                    names.Add(summary.CategoryName);
                }
            }
        }

        return names;
    }

    public static string ResolveCategory(int categoryIndex, string customCategory, IReadOnlyList<string> categoryNames)
    {
        var customIndex = categoryNames.Count;
        if (categoryNames.Count == 0 || categoryIndex == customIndex)
        {
            return customCategory.Trim();
        }

        if (categoryIndex >= 0 && categoryIndex < categoryNames.Count)
        {
            return categoryNames[categoryIndex];
        }

        return customCategory.Trim();
    }

    public static async Task<CommunityShareResult> SubmitAsync(MarkerSet markerSet, string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return new CommunityShareResult { Success = false, Message = "Enter a category name." };
        }

        try
        {
            var subtoken = await Service.SubtokenService.GetValidSubtokenAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(subtoken))
            {
                return new CommunityShareResult
                {
                    Success = false,
                    Message = "Account API permission required."
                };
            }

            var payload = MarkerSetSubmission.ToSubmissionPayload(markerSet, category);
            using var client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Bearer " + subtoken;
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            var url = Service.ManifestService.Manifest.Absolute(Service.ManifestService.Manifest.SubmissionsUrl);
            client.UploadString(url, payload.ToString(Formatting.None));

            return new CommunityShareResult
            {
                Success = true,
                Message = "Sent for moderator review."
            };
        }
        catch (WebException ex) when (ex.Response is HttpWebResponse response)
        {
            return new CommunityShareResult
            {
                Success = false,
                Message = $"Share failed ({(int)response.StatusCode})."
            };
        }
        catch (Exception)
        {
            return new CommunityShareResult { Success = false, Message = "Share failed." };
        }
    }
}
