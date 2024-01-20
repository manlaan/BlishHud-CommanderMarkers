using Blish_HUD.Entities;
using Gw2Sharp.WebApi.V2.Models;
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


    public static Vector2 MapToWorldInches(this Map map, Vector2 world)
        => new Vector2(
            (float)(map.MapRect.TopLeft.X + (world.X - map.ContinentRect.TopLeft.X) / map.ContinentRect.Width * map.MapRect.Width),
            (float)(map.MapRect.TopLeft.Y - (world.Y - map.ContinentRect.TopLeft.Y) / map.ContinentRect.Width * map.MapRect.Width)
            );

    public static Vector2 MapToWorldMeters(this Map map, Vector2 screenMap)
        => map.MapToWorldInches(screenMap)/MathUtils.MetersToInches;


}

