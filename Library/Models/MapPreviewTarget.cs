using Manlaan.CommanderMarkers.Library.Models;
using Manlaan.CommanderMarkers.Presets.Model;
using System.Collections.Generic;
using System.Linq;

namespace Manlaan.CommanderMarkers.Library.Models;

public sealed class MapPreviewTarget
{
    public string CommunitySetId { get; init; } = "";
    public string Label { get; init; } = "";
    public string Description { get; init; } = "";
    public string PreviewThumbUrl { get; init; } = "";
    public string PreviewLargeUrl { get; init; } = "";
    public IReadOnlyList<MarkerCoord> Markers { get; init; } = new List<MarkerCoord>();

    public static MapPreviewTarget FromLocalMarker(MarkerSet markerSet)
    {
        return new MapPreviewTarget
        {
            CommunitySetId = markerSet.communitySetId ?? "",
            Label = markerSet.name ?? "",
            Description = markerSet.description ?? "",
            Markers = markerSet.marks.ToList()
        };
    }

    public static MapPreviewTarget FromCommunitySummary(CommunitySetSummary summary)
    {
        return new MapPreviewTarget
        {
            CommunitySetId = summary.Id,
            Label = summary.Name,
            Description = summary.Description,
            PreviewThumbUrl = summary.PreviewThumbUrl,
            PreviewLargeUrl = summary.PreviewLargeUrl
        };
    }
}
