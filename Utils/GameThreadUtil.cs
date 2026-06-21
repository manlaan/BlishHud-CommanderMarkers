using Blish_HUD;
using System;

namespace Manlaan.CommanderMarkers.Utils;

public static class GameThreadUtil
{
    public static void Enqueue(Action action)
    {
        GameService.Overlay.QueueMainThreadUpdate(_ => action());
    }
}
