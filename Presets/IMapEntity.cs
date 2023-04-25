using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Manlaan.CommanderMarkers.Presets;

public interface IMapEntity
{
    void DrawToMap(SpriteBatch spriteBatch, IMapBounds map, Control control);

    float DistanceFrom(Vector3 playerPosition);

    string GetMarkerText();

}