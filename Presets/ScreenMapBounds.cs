using Manlaan.CommanderMarkers.Presets;
using Microsoft.Xna.Framework;

namespace Manlaan.CommanderMarkers.Presets;

public class ScreenMapBounds : MapBounds
{
    private readonly MapData _mapData;

    public ScreenMapBounds(MapData mapData)
    {
        _mapData = mapData;
    }

    public override Vector2 FromWorld(int mapId, Vector3 worldMeters)
        => _mapData.WorldToScreenMap(mapId, worldMeters);

    public override Vector2 FromMap(Vector2 mapCoords)
        => MapData.MapToScreenMap(mapCoords);
}