using Blish_HUD.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Manlaan.CommanderMarkers.Library.Controls;

public class EnabledIconButton : GlowButton, IDisposable
{
    private Texture2D _enabledTexture = Service.Textures!._imgCheck;
    private Texture2D _disabledTexture = Service.Textures!._imgClear;
    private bool _watchValue = true;

    public bool WatchValue { get => _watchValue; set { _watchValue = value; SetTexture(); } }

    protected override CaptureType CapturesInput()
    {
        return CaptureType.Mouse;
    }
    public EnabledIconButton(bool watchValue)
    {
        _watchValue = watchValue;
        Click += EnabledIconButton_Click;
        SetTexture();
    }

    public EnabledIconButton(bool watchValue, Texture2D enabledTexture, Texture2D disabledTexture)
    {
        _watchValue = watchValue;
        _enabledTexture = enabledTexture;
        _disabledTexture = disabledTexture;
        Click += EnabledIconButton_Click;
        SetTexture();
    }

    protected void SetTexture()
    {
        if (_watchValue)
        {
            Icon = _enabledTexture;
            BasicTooltipText = "Click to disable";
        }
        else
        {
            Icon = _disabledTexture;
            BasicTooltipText = "Click to enable";
        }
        Invalidate();
    }

    private void EnabledIconButton_Click(object sender, Blish_HUD.Input.MouseEventArgs e)
    {
        _watchValue = !_watchValue;
        SetTexture();
    }

    protected override void DisposeControl()
    {
        Click -= EnabledIconButton_Click;
    }

}
