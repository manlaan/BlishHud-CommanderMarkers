using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.CornerIcon;



public class CornerIconToggleMenuItem : ContextMenuStripItem
{
    public CornerIconToggleMenuItem(SettingEntry<bool> setting, string displayLabel) : base(displayLabel)
    {
        var baseText = displayLabel;
        Text = (setting.Value ? "Hide" : "Show") + " " + baseText;

        Click += delegate { setting.Value = !setting.Value; };
        setting.SettingChanged += delegate
        {
            Text = (setting.Value ? "Hide" : "Show") + " " + baseText;
        };
    }

    public CornerIconToggleMenuItem(Control control, string displayLabel) : base(displayLabel)
    {
        Click += delegate { control.Show(); };
    }
}
public class CornerIconService : IDisposable
{
    public event EventHandler<bool>? IconLeftClicked;

    private readonly IEnumerable<ContextMenuStripItem> _contextMenuItems;
    private readonly Texture2D _cornerIconTexture;
    private readonly Texture2D _cornerIconHoverTexture;
    private readonly SettingEntry<bool> _cornerIconIsVisibleSetting;
    private readonly string _tooltip;
    private Blish_HUD.Controls.CornerIcon? _cornerIcon;

    public CornerIconService(
        SettingEntry<bool> cornerIconIsVisibleSetting,
        string tooltip,
        Texture2D defaultTexture,
        Texture2D hoverTexture,
        IEnumerable<ContextMenuStripItem> contextMenuItems
    ) 
    {
        _tooltip = tooltip;
        _cornerIconIsVisibleSetting = cornerIconIsVisibleSetting;
        _cornerIconTexture = defaultTexture;
        _cornerIconHoverTexture = hoverTexture;
        _contextMenuItems = contextMenuItems;
        _cornerIconIsVisibleSetting.SettingChanged += OnCornerIconIsVisibleSettingChanged;

        if (cornerIconIsVisibleSetting.Value)
            CreateCornerIcon();
    }

    public void OpenContextMenu()
    {
        _cornerIcon?.Menu?.Show(GameService.Input.Mouse.Position);
    }

    public void Dispose()
    {
        _cornerIconIsVisibleSetting.SettingChanged -= OnCornerIconIsVisibleSettingChanged;

        RemoveCornerIcon();
    }

    private void CreateCornerIcon()
    {
        RemoveCornerIcon();

        _cornerIcon = new Blish_HUD.Controls.CornerIcon()
        {
            Icon = _cornerIconTexture,
            HoverIcon = _cornerIconHoverTexture,
            BasicTooltipText = _tooltip,
            Parent = GameService.Graphics.SpriteScreen,
            Priority = 1122334455
        };

        _cornerIcon.Click += OnCornerIconClicked;
        _cornerIcon.Menu = new CornerIconContextMenu(() => _contextMenuItems);
    }

    private void RemoveCornerIcon()
    {
        if (_cornerIcon is not null)
        {
            _cornerIcon.Click -= OnCornerIconClicked;
            _cornerIcon?.Menu?.Dispose();
            _cornerIcon?.Dispose();
        }
    }

    private void OnCornerIconIsVisibleSettingChanged(object sender, ValueChangedEventArgs<bool> e)
    {
        if (e.NewValue)
            CreateCornerIcon();
        else
            RemoveCornerIcon();
    }
    private void OnCornerIconClicked(object sender, MouseEventArgs e)
    {
        IconLeftClicked?.Invoke(this, true);
    }


}

public class ContextMenuStripItemSeparator : ContextMenuStripItem
{
    public ContextMenuStripItemSeparator() : base()
    {
        Enabled = false;
        base.EffectBehind = null;

    }
    protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
    {
        spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(0, bounds.Height / 2, bounds.Width, 1), Color.White * 0.8f);
    }
}