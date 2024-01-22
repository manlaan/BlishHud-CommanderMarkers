using Blish_HUD.Entities;
using Gw2Sharp.WebApi.V2.Models;
using Manlaan.CommanderMarkers.Presets;
using Microsoft.Xna.Framework;

namespace Manlaan.CommanderMarkers.Utils;

public static class MapExtensions
{
    public static Vector2 WorldInchesToMap(this Map map, Vector3 world)
        => new Vector2(
            (float)(map.ContinentRect.TopLeft.X + (world.X - map.MapRect.TopLeft.X) / map.MapRect.Width * map.ContinentRect.Width),
            (float)(map.ContinentRect.TopLeft.Y - (world.Y - map.MapRect.TopLeft.Y) / map.MapRect.Height * map.ContinentRect.Height));

    public static Vector2 WorldMetersToMap(this Map map, Vector3 world)
        => map.WorldInchesToMap(world * MathUtils.MetersToInches);

    public static Vector3 MapToWorldInches(this Map map, Vector2 coords)
        => new Vector3(
            (float)(map.MapRect.TopLeft.X + (coords.X - map.ContinentRect.TopLeft.X) / map.ContinentRect.Width * map.MapRect.Width),
            (float)(map.MapRect.TopLeft.Y - (coords.Y - map.ContinentRect.TopLeft.Y) / map.ContinentRect.Height * map.MapRect.Height),
            0);

    public static Vector3 MapToWorldMeters(this Map map, Vector2 coords)
        => map.MapToWorldInches(coords) * MathUtils.InchesToMeters;
  

    public static Point MirrorOverOrigin(this Point target, Point origin)
    {
        target -= origin;
        return new Point(-target.X, -target.Y) + origin;
    }

}

