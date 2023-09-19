using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Library.Models;

public  class CommunityCategory
{
    [JsonProperty("categoryName")]
    public string CategoryName { get; set; } = "Community Created";

    [JsonProperty("markerSets")]
    public List<CommunityMarkerSet> MarkerSets { get; set; } = new();

}
