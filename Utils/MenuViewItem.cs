using System;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;

namespace Manlaan.CommanderMarkers.Utils;

public record MenuViewItem(MenuItem MenuItem, Func<MenuItem, IView> ViewFunction)
{
    public Func<MenuItem, IView> ViewFunction { get; } = ViewFunction;
    public MenuItem MenuItem { get; } = MenuItem;
}
