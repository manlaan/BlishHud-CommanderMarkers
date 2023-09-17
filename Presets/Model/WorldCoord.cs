
using Blish_HUD;
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

    public WorldCoord FromWorldCoord(WorldCoord coord)
    {
        return FromVector3(coord.ToVector3());
    }
    public WorldCoord FromVector3(Vector3 position)
    {
        x = position.X;
        y = position.Y;
        z = position.Z;
        return this;
    }
    public WorldCoord SetFromMumbleLocation()
    {
        return FromVector3(Gw2MumbleService.Gw2Mumble.PlayerCharacter.Position);
    }
}
