
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Manlaan.CommanderMarkers.Presets.Model;


[Serializable]
public class MarkerSet
{
    [JsonProperty("id")]
    public string? id { get; set; }

    [JsonProperty("communitySetId")]
    public string? communitySetId { get; set; }

    [JsonProperty("author")]
    public string? author { get; set; }

    [JsonProperty("source")]
    public string? source { get; set; }

    [JsonProperty("communityUpdatedAt")]
    public string? communityUpdatedAt { get; set; }

    [JsonProperty("localModifiedAt")]
    public string? localModifiedAt { get; set; }

    [JsonProperty("syncBaselineHash")]
    public string? syncBaselineHash { get; set; }

    [JsonProperty("syncDetached")]
    public bool syncDetached { get; set; }

    [JsonProperty("name")]
    public string? name { get; set; }

    [JsonProperty("description")]
    public string? description { get; set; }

    [JsonProperty("mapId")]
    public int? mapId { get; set; }

    [JsonProperty("trigger")]
    public WorldCoord? trigger { get; set; }

    [JsonProperty("markers")]
    public List<MarkerCoord> marks { get; set; } = new() {};

    [JsonProperty("enabled")]
    public bool enabled = true;

    [JsonIgnore()]
    public WorldCoord Trigger { get => trigger ?? new WorldCoord(); }

    [JsonIgnore()]
    public int MapId { get => (int)(mapId ?? 0); }

    [JsonIgnore()]
    public string MapName { get => Service.MapDataCache.Describe((int)MapId!); }

    public string DescribeMarkers()
    {
        return string.Join("\n", marks);   
    }


    public void CloneFromMarkerSet(MarkerSet otherSet)
    {
        id = otherSet.id;
        communitySetId = otherSet.communitySetId;
        author = otherSet.author;
        source = otherSet.source;
        communityUpdatedAt = otherSet.communityUpdatedAt;
        localModifiedAt = otherSet.localModifiedAt;
        syncBaselineHash = otherSet.syncBaselineHash;
        syncDetached = otherSet.syncDetached;
        name = otherSet.name;
        description = otherSet.description;
        mapId = otherSet.mapId;
        trigger = otherSet.trigger;
        marks = new List<MarkerCoord>();
        enabled = otherSet.enabled;

        for(var i = 0; i < otherSet.marks.Count && i<8; i++)
        {
            marks.Add(otherSet.marks[i]);
        }
    }

}
