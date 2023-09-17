using Microsoft.Xna.Framework;

namespace Manlaan.CommanderMarkers.Utils;

public static class PointExtensions
{
    public static Point Half(this Point point) => new(point.X / 2, point.Y / 2);
    public static Point Scale(this Point point, float scale) => new((int) (point.X * scale), (int) (point.Y * scale));
}