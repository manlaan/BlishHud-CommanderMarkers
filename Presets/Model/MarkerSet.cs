
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Manlaan.CommanderMarkers.Presets.Model;


[Serializable]
public class MarkerSet
{
    [JsonProperty("name")]
    
    public string? name { get; set; }

    [JsonProperty("description")]
    public string? description { get; set; }

    [JsonProperty("mapId")]
    public int? mapId { get; set; }

    [JsonProperty("trigger")]
    public WorldCoord? trigger { get; set; }

    [JsonProperty("markers")]
    public List<MarkerCoord> marks { get; set; } = new();

    [JsonProperty("enabled")]
    public bool enabled = true;

    [JsonIgnore()]
    public WorldCoord Trigger { get => (trigger == null) ? new WorldCoord() : trigger; }

    [JsonIgnore()]
    public int MapId { get => (mapId == null) ? 0 : (int)mapId; }


    public void CloneFromMarkerSet(MarkerSet otherSet)
    {
        name = otherSet.name;
        description = otherSet.description;
        mapId = otherSet.mapId;
        trigger = otherSet.trigger;
        marks = otherSet.marks;
        enabled = otherSet.enabled;
    }

}
