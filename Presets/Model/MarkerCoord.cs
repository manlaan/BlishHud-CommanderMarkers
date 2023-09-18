
using Manlaan.CommanderMarkers.Library.Enums;
using Newtonsoft.Json;
using System;

namespace Manlaan.CommanderMarkers.Presets.Model;


[Serializable]
public class MarkerCoord : WorldCoord
{
    [JsonProperty("i")]
    public int icon { get; set; }

    [JsonProperty("d")]
    public string name { get; set; }

    public SquadMarker ToSquadMarkerEnum()
    {
        if (icon < 0 || icon > 8) return SquadMarker.None;

        return (SquadMarker)icon;
    }

    public override string? ToString()
    {
        return $"{((SquadMarker)icon).EnumValue()} - {name}";
    }

}
