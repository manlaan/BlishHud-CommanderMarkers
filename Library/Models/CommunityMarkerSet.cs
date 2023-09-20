
using Manlaan.CommanderMarkers.Presets.Model;
using Newtonsoft.Json;
using System;

namespace Manlaan.CommanderMarkers.Library.Models;


[Serializable]
public class CommunityMarkerSet: MarkerSet
{
    [JsonProperty("author")]
    public string Author { get; set; } = "BlishHud Community";


}
