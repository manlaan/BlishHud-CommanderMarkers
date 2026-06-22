using Blish_HUD.Controls;
using System;
using Microsoft.Xna.Framework.Graphics;
using Blish_HUD;

namespace Manlaan.CommanderMarkers.Library.Controls;

public class EnabledIconButton : IconButton, IDisposable
{
    private Texture2D _enabledTexture = Service.Textures!._imgCheck;
    private Texture2D _disabledTexture = Service.Textures!._imgClear;
    private bool _watchValue = true;

    public event EventHandler<bool>? ValueChanged;

    public bool WatchValue
    {
        get => _watchValue;
        set
        {
            if (_watchValue == value)
            {
                return;
            }

            _watchValue = value;
            SetTexture();
        }
    }

    protected override CaptureType CapturesInput() => CaptureType.Mouse;

    public EnabledIconButton(bool watchValue, Texture2D? enabledTexture = null, Texture2D? disabledTexture = null)
    {
        _watchValue = watchValue;
        if (enabledTexture != null)
        {
            _enabledTexture = enabledTexture;
        }

        if (disabledTexture != null)
        {
            _disabledTexture = disabledTexture;
        }

        Click += EnabledIconButton_Click;
        SetTexture();
    }

    protected void SetTexture()
    {
        if (_watchValue)
        {
            Icon = _enabledTexture;
            BasicTooltipText = "Click to disable this marker set";
        }
        else
        {
            Icon = _disabledTexture;
            BasicTooltipText = "Click to enable this marker set";
        }

        Invalidate();
    }

    private void EnabledIconButton_Click(object sender, Blish_HUD.Input.MouseEventArgs e)
    {
        WatchValue = !WatchValue;
        ValueChanged?.Invoke(this, WatchValue);
    }

    protected override void DisposeControl()
    {
        Click -= EnabledIconButton_Click;
    }
}
