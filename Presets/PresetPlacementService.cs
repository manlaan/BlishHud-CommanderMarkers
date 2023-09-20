using Microsoft.Xna.Framework;
using Blish_HUD;

namespace Manlaan.CommanderMarkers.Presets;

public static class PresetPlacementService
{

    public static void PlaceMarkers()
    {
        //var continentCoords;
    }

    public static Vector2 ContinentToMapScreen(Vector2 continentCoords)
    {
        var mapCenter = GameService.Gw2Mumble.UI.MapCenter.ToXnaVector2();
        var mapRotation = Matrix.CreateRotationZ(
            GameService.Gw2Mumble.UI.IsCompassRotationEnabled && !GameService.Gw2Mumble.UI.IsMapOpen
                ? (float)GameService.Gw2Mumble.UI.CompassRotation
                : 0);

        var screenBounds = MumbleUtils.GetMapBounds();  // <-- The static method I linked above
        var scale = (float)(GameService.Gw2Mumble.UI.MapScale * 0.897);  // Blish HUD scale workaround
        var boundsCenter = screenBounds.Location.ToVector2() + screenBounds.Size.ToVector2() / 2f;

        return Vector2.Transform((continentCoords - mapCenter) / scale, mapRotation) + boundsCenter;
    }
}
