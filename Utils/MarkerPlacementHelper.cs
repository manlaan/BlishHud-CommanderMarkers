using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Manlaan.CommanderMarkers.Utils;

/// <summary>
/// Converts map/screen coordinates to the actual mouse position for marker placement.
/// When billboards and the cursor appear correct but placed markers are offset, the overlay
/// (Blish) and the game window are using different coordinate spaces: we draw and move the
/// cursor in overlay space, but GW2 places markers using the game window's mouse position.
/// We fix this by moving the real cursor to screen position (game client origin + coords)
/// so the game receives the same position we intended.
/// </summary>
public static class MarkerPlacementHelper
{
    #region P/Invoke for game window position (windowed mode)

    [DllImport("user32.dll")]
    private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    /// <summary>
    /// Tries to get the GW2 game window's client area top-left in screen coordinates.
    /// Returns true and sets left, top if successful.
    /// </summary>
    public static bool TryGetGameClientOrigin(out int left, out int top)
    {
        left = 0;
        top = 0;
        try
        {
            var processes = Process.GetProcessesByName("Gw2-64");
            if (processes.Length == 0)
                processes = Process.GetProcessesByName("Gw2");
            foreach (var process in processes)
            {
                try
                {
                    IntPtr hwnd = process.MainWindowHandle;
                    if (hwnd == IntPtr.Zero) continue;
                    if (!GetClientRect(hwnd, out RECT rect)) continue;
                    POINT pt = new POINT { X = rect.Left, Y = rect.Top };
                    if (!ClientToScreen(hwnd, ref pt)) continue;
                    left = pt.X;
                    top = pt.Y;
                    return true;
                }
                finally
                {
                    process.Dispose();
                }
            }
        }
        catch
        {
            // ignore
        }
        return false;
    }

    #endregion

    /// <summary>
    /// Converts a position in Blish map/screen space (e.g. from WorldToScreenMap) to the
    /// coordinates that should be used when setting the mouse for marker placement.
    /// Uses game window client origin in windowed mode so placement is correct regardless
    /// of overlay vs game window alignment.
    /// </summary>
    /// <param name="blishCoord">Position in Blish/SpriteScreen space (from MapData.WorldToScreenMap).</param>
    /// <param name="uiScaleMultiplier">Usually GameService.Graphics.UIScaleMultiplier.</param>
    /// <returns>Point to use with SetPlacementMousePosition.</returns>
    public static Point BlishToPlacementPosition(Vector2 blishCoord, float uiScaleMultiplier)
    {
        float scale = uiScaleMultiplier;
        float x = blishCoord.X * scale;
        float y = blishCoord.Y * scale;

        if (TryGetGameClientOrigin(out int gameLeft, out int gameTop))
        {
            // We want the cursor to be at (gameLeft + x, gameTop + y) in screen space so the game
            // receives (x, y) in its client area. SetCursorPos uses screen coordinates.
            return new Point(gameLeft + (int)x, gameTop + (int)y);
        }

        // Fallback: assume overlay and game are aligned (e.g. fullscreen or matching window).
        return new Point((int)x, (int)y);
    }

    /// <summary>
    /// Sets the mouse position for marker placement. Uses screen coordinates when the game
    /// window origin is known (windowed mode), otherwise uses window-relative coordinates.
    /// </summary>
    public static void SetPlacementMousePosition(Point placementPosition, bool useScreenCoordinates)
    {
        if (useScreenCoordinates)
            SetCursorPos(placementPosition.X, placementPosition.Y);
        else
            Mouse.SetPosition(placementPosition.X, placementPosition.Y);
    }

    /// <summary>
    /// Gets the current cursor position in the same coordinate space used for placement.
    /// When UseScreenCoordinatesForPlacement is true, returns screen coords; otherwise window-relative.
    /// Use this to save position before placement and restore after with SetPlacementMousePosition.
    /// </summary>
    public static Point GetPlacementCursorPosition(bool useScreenCoordinates)
    {
        if (useScreenCoordinates && GetCursorPos(out POINT pt))
            return new Point(pt.X, pt.Y);
        var state = Mouse.GetState();
        return new Point(state.X, state.Y);
    }

    /// <summary>
    /// Gets whether we were able to resolve the game window (so placement will use screen coords).
    /// </summary>
    public static bool UseScreenCoordinatesForPlacement()
        => TryGetGameClientOrigin(out _, out _);
}
