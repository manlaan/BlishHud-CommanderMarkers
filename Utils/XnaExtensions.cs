
using Gw2Sharp.Models;
using Microsoft.Xna.Framework;

namespace Manlaan.CommanderMarkers;

public static class XnaExtensions
{
    public static Vector2 ToXnaVector2(this Coordinates2 coords)
        => new Vector2((float)coords.X, (float)coords.Y);

    public static Vector2 ToVector2(this Vector3 vector)
        => new Vector2(vector.X, vector.Y);
}