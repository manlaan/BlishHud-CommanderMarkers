using Manlaan.CommanderMarkers;
using Microsoft.Xna.Framework;

namespace Manlaan.CommanderMarkers.Presets;

public interface IMapBounds
{
    Rectangle Rectangle { get; }

    Vector2 FromWorld(int mapId, Vector3 worldMeters);
    Vector2 FromMap(Vector2 mapCoords);
    Rectangle FromMap(Gw2Sharp.WebApi.V2.Models.Rectangle rect);
    bool Contains(Vector2 point);
}

public abstract class MapBounds : IMapBounds
{
    public Rectangle Rectangle { get; set; }
    public Point Location => Rectangle.Location;
    public Point Size => Rectangle.Size;
    public Point Center => Rectangle.Center;

    public abstract Vector2 FromWorld(int mapId, Vector3 worldMeters);

    public abstract Vector2 FromMap(Vector2 mapCoords);

    public Rectangle FromMap(Gw2Sharp.WebApi.V2.Models.Rectangle rect)
    {
        var topLeft = FromMap(rect.TopLeft.ToXnaVector2());
        var bottomRight = FromMap(rect.BottomRight.ToXnaVector2());
        return new Rectangle(topLeft.ToPoint(), (bottomRight - topLeft).ToPoint());
    }

    public bool Contains(Vector2 point)
        => Rectangle.Contains(point);
}