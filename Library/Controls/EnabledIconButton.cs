using Blish_HUD.Controls;
using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Blish_HUD;

namespace Manlaan.CommanderMarkers.Library.Controls;

public class EnabledIconButton : IconButton, IDisposable
{
    private Texture2D _enabledTexture = Service.Textures!._imgCheck;
    private Texture2D _disabledTexture = Service.Textures!._imgClear;
    private static readonly BitmapFont _font = GameService.Content.DefaultFont16;
    private bool _watchValue = true;

    public bool WatchValue { get => _watchValue; set { _watchValue = value; SetTexture(); } }

    protected override CaptureType CapturesInput()
    {
        return CaptureType.Mouse;
    }
    public EnabledIconButton(bool watchValue, Texture2D? enabledTexture=null, Texture2D? disabledTexture=null)
    {
        _watchValue = watchValue;
        if(enabledTexture !=null) _enabledTexture = enabledTexture;
        if(disabledTexture!=null) _disabledTexture = disabledTexture;
        Click += EnabledIconButton_Click;
        SetTexture();
    }

    protected void SetTexture()
    {
        if (_watchValue)
        {
            Icon = _disabledTexture;
            BasicTooltipText = "disable";
        }
        else
        {
            Icon = _enabledTexture;
            BasicTooltipText = "enable";
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
