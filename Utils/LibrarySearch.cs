using Manlaan.CommanderMarkers.Library.Models;
using Manlaan.CommanderMarkers.Presets.Model;
using System;

namespace Manlaan.CommanderMarkers.Utils;

public static class LibrarySearch
{
    public const int SearchFieldWidth = 200;

    public static string ToLowerCopy(string value) => value.ToLowerInvariant();

    public static bool MatchesLocal(MarkerSet markerSet, string mapName, string queryLower)
    {
        if (string.IsNullOrEmpty(queryLower))
        {
            return true;
        }

        return Contains(queryLower, markerSet.name) ||
               Contains(queryLower, markerSet.description) ||
               Contains(queryLower, mapName);
    }

    public static bool MatchesCommunity(CommunitySetSummary summary, string mapName, string queryLower)
    {
        if (string.IsNullOrEmpty(queryLower))
        {
            return true;
        }

        return Contains(queryLower, summary.Name) ||
               Contains(queryLower, summary.Description) ||
               Contains(queryLower, summary.Author) ||
               Contains(queryLower, mapName);
    }

    private static bool Contains(string queryLower, string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        return ToLowerCopy(text!).Contains(queryLower);
    }
}
