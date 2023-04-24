using Microsoft.Xna.Framework.Graphics;

namespace Manlaan.CommanderMarkers.Presets;

public interface IMapEntity
{
    void DrawToMap(SpriteBatch spriteBatch, IMapBounds map);
}