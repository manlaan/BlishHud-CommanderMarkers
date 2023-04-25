
using Microsoft.Xna.Framework;
using System;
using static Blish_HUD.GameService;

namespace Manlaan.CommanderMarkers;

public static class MumbleUtils
{
    private const int MinCompassWidth = 170;
    private const int MaxCompassWidth = 362;
    private const int MinCompassHeight = 170;
    private const int MaxCompassHeight = 338;
    private const int MinCompassOffset = 19;
    private const int MaxCompassOffset = 40;
    private const int CompassSeparation = 40;

    private static int GetCompassOffset(float curr, float min, float max)
        => (int)Math.Round(MathUtils.Scale(curr, min, max, MinCompassOffset, MaxCompassOffset));

    public static Rectangle GetMapBounds()
    {
        if (Gw2Mumble.UI.CompassSize.Width < 1 || Gw2Mumble.UI.CompassSize.Height < 1)
            return default;

        if (Gw2Mumble.UI.IsMapOpen)
            return new Rectangle(Point.Zero, Graphics.SpriteScreen.Size);

        int offsetWidth = GetCompassOffset(Gw2Mumble.UI.CompassSize.Width, MinCompassWidth, MaxCompassWidth);
        int offsetHeight = GetCompassOffset(Gw2Mumble.UI.CompassSize.Height, MinCompassHeight, MaxCompassHeight);

        return new Rectangle(
            Graphics.SpriteScreen.Width - Gw2Mumble.UI.CompassSize.Width - offsetWidth,
            Gw2Mumble.UI.IsCompassTopRight
                ? 0
                : Graphics.SpriteScreen.Height - Gw2Mumble.UI.CompassSize.Height - offsetHeight - CompassSeparation,
            Gw2Mumble.UI.CompassSize.Width + offsetWidth,
            Gw2Mumble.UI.CompassSize.Height + offsetHeight);
    }
}