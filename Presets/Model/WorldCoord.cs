
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;

namespace Manlaan.CommanderMarkers.Presets.Model;

[Serializable]
public  class WorldCoord
{
    [JsonProperty("x")]
    public float x { get; set; }

    [JsonProperty("y")] 
    public float y { get; set; }

    [JsonProperty("z")] 
    public float z { get; set; }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
