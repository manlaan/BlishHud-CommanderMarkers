using Blish_HUD.Controls;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Manlaan.CommanderMarkers.Library.Controls;

public class IconPicker : FlowPanel
{
    public event EventHandler<int>? IconSelectionChanged;

    protected List<(int, Texture2D, IconButton)> state = new();

    protected int _selectedItem = -1;
    public IconPicker()
    {

    }

    public void SelectItem(int select)
    {
        _selectedItem = select;
        foreach(var e in state)
        {
            if (e.Item1 == select)
            {
                e.Item3.Checked = true;
                e.Item3.Opacity = 1.0f;
                e.Item3.Size = new Microsoft.Xna.Framework.Point(32, 33);
            }
            else
            {
                e.Item3.Checked = false;
                e.Item3.Opacity = 0.3f;
                e.Item3.Size = new Microsoft.Xna.Framework.Point(28, 28);
            }
        }
        IconSelectionChanged?.Invoke(this,select);

    }

    public void LoadList(List<(int, Texture2D)> textureList)
    {
        state.Clear();
        Children.Clear();
        foreach(var texture in textureList)
        {
            var btn = new IconButton()
            {
                Parent = this,
                Icon = texture.Item2,
                //ToggleGlow = true,
                Checked = false,
                Opacity = 0.3f,
                Size = new Microsoft.Xna.Framework.Point(28,28)

            };
            btn.Click += (s, e) => SelectItem(texture.Item1);
            state.Add((texture.Item1, texture.Item2, btn));

        }
    }
}
