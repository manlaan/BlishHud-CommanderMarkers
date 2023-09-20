using Newtonsoft.Json;
using System.Collections.Generic;

namespace Manlaan.CommanderMarkers.Library.Models;

public  class CommunityCategory
{
    [JsonProperty("categoryName")]
    public string CategoryName { get; set; } = "Community Created";

    [JsonProperty("markerSets")]
    public List<CommunityMarkerSet> MarkerSets { get; set; } = new();

}
