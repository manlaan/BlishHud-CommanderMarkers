using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Controls.Extern;
using Blish_HUD.Input;
using Blish_HUD.Settings;
using Manlaan.CommanderMarkers.Settings.Services;
using Manlaan.CommanderMarkers.Utils;
using Manlaan.CommanderMarkers.Library.Services;
using Manlaan.CommanderMarkers.Library.Models;
using Manlaan.CommanderMarkers.Library.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;

namespace Manlaan.CommanderMarkers.Markers;


public class MarkersPanel : FlowPanel, IDisposable
{
    private bool _draggable = false;
    private bool _isDraggedByMouse;

    private bool _mouseEventsEnabled;

    private bool _mouseIsInsidePanel = false;

    private bool _panelEnabled = true;

    private Point _dragStart = Point.Zero;


    private KeyBinding? _tmpBinding;
    private Image? _tmpButton;
    protected SettingService _settings;
    protected TextureService _textures;

    private static readonly BitmapFont _dragFont = GameService.Content.DefaultFont16;

    public MarkersPanel(SettingService settings, TextureService textures, bool mouseEventsEnabled=true)
    {
        _mouseEventsEnabled = mouseEventsEnabled;
        _settings = settings;
        this._textures = textures;


        Parent = Blish_HUD.GameService.Graphics.SpriteScreen;
        Location = settings._settingLoc.Value;
        
        FlowDirection = ControlFlowDirection.SingleTopToBottom;
        WidthSizingMode = SizingMode.AutoSize;
        HeightSizingMode = SizingMode.AutoSize;
        
        (this as FlowPanel).LayoutChange(_settings._settingOrientation);
        (this as FlowPanel).OpacityChange(_settings._settingOpacity);

        var size = settings._settingImgWidth.Value;
        var opacity = settings._settingOpacity.Value;

        var groundIcons = CreateGroupingFlowPanel();
        groundIcons.VisiblityChanged(_settings._settingGroundMarkersEnabled);
        var objectIcons = CreateGroupingFlowPanel();
        objectIcons.VisiblityChanged(_settings._settingTargetMarkersEnabled);

        // Create marker buttons using the new definition system
        CreateMarkerButtons(groundIcons, objectIcons, size, opacity);

        if(_mouseEventsEnabled)
            AddDragDelegates();

        _settings._settingDrag.SettingChanged += (s, e) => _draggable = e.NewValue;
        _draggable = _settings._settingDrag.Value;

        _panelEnabled = _settings._settingShowMarkersPanel.Value;
        _settings._settingShowMarkersPanel.SettingChanged += (s, e) => { _panelEnabled = e.NewValue; };


        if (_mouseEventsEnabled)
        {
            GameService.Input.Mouse.LeftMouseButtonPressed += OnMouseClick;
        }
    }

    /// <summary>
    /// Creates marker buttons for all defined markers using the centralized definition system
    /// </summary>
    private void CreateMarkerButtons(FlowPanel groundIcons, FlowPanel objectIcons, int size, float opacity)
    {
        foreach (var markerDef in MarkerDefinitionService.AllMarkers)
        {
            // Create ground marker button if supported
            if (markerDef.SupportsGroundTarget)
            {
                var groundBinding = GetKeyBindingForMarker(markerDef, true);
                CreateIconButton(
                    groundIcons, 
                    GetTextureForMarker(markerDef.MarkerType), 
                    size, 
                    opacity, 
                    markerDef.GetGroundTooltip(), 
                    groundBinding, 
                    true
                );
            }

            // Create object marker button if supported
            if (markerDef.SupportsObjectTarget)
            {
                var objectBinding = GetKeyBindingForMarker(markerDef, false);
                CreateIconButton(
                    objectIcons, 
                    GetTextureForMarker(markerDef.MarkerType), 
                    size, 
                    opacity, 
                    markerDef.GetObjectTooltip(), 
                    objectBinding, 
                    false
                );
            }
        }
    }

    /// <summary>
    /// Gets the keybinding setting for a specific marker and target type
    /// </summary>
    private SettingEntry<KeyBinding> GetKeyBindingForMarker(MarkerDefinition markerDef, bool isGroundTarget)
    {
        return markerDef.MarkerType switch
        {
            SquadMarker.Arrow => isGroundTarget ? _settings._settingArrowGndBinding : _settings._settingArrowObjBinding,
            SquadMarker.Circle => isGroundTarget ? _settings._settingCircleGndBinding : _settings._settingCircleObjBinding,
            SquadMarker.Heart => isGroundTarget ? _settings._settingHeartGndBinding : _settings._settingHeartObjBinding,
            SquadMarker.Square => isGroundTarget ? _settings._settingSquareGndBinding : _settings._settingSquareObjBinding,
            SquadMarker.Star => isGroundTarget ? _settings._settingStarGndBinding : _settings._settingStarObjBinding,
            SquadMarker.Spiral => isGroundTarget ? _settings._settingSpiralGndBinding : _settings._settingSpiralObjBinding,
            SquadMarker.Triangle => isGroundTarget ? _settings._settingTriangleGndBinding : _settings._settingTriangleObjBinding,
            SquadMarker.Cross => isGroundTarget ? _settings._settingXGndBinding : _settings._settingXObjBinding,
            SquadMarker.Clear => isGroundTarget ? _settings._settingClearGndBinding : _settings._settingClearObjBinding,
            _ => throw new ArgumentException($"Unknown marker type: {markerDef.MarkerType}")
        };
    }

    /// <summary>
    /// Gets the texture for a specific marker type
    /// </summary>
    private AsyncTexture2D GetTextureForMarker(SquadMarker markerType)
    {
        return markerType switch
        {
            SquadMarker.Arrow => _textures._imgArrow,
            SquadMarker.Circle => _textures._imgCircle,
            SquadMarker.Heart => _textures._imgHeart,
            SquadMarker.Square => _textures._imgSquare,
            SquadMarker.Star => _textures._imgStar,
            SquadMarker.Spiral => _textures._imgSpiral,
            SquadMarker.Triangle => _textures._imgTriangle,
            SquadMarker.Cross => _textures._imgX,
            SquadMarker.Clear => _textures._imgClear,
            _ => _textures._imgArrow // fallback
        };
    }

    public override void PaintAfterChildren(SpriteBatch spriteBatch, Rectangle bounds)
    {
        if (_draggable && _mouseEventsEnabled)
        {
            spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(0,0,this.Width, this.Height), new Color(96,96,96,192));
            spriteBatch.DrawStringOnCtrl(this, "Drag", _dragFont, new Rectangle(0, 0, this.Width, this.Height), Color.Black, horizontalAlignment: Blish_HUD.Controls.HorizontalAlignment.Center, verticalAlignment: VerticalAlignment.Middle);

        }
    }
    protected override void DisposeControl()
    {
        if(_mouseEventsEnabled)
            GameService.Input.Mouse.LeftMouseButtonPressed -= OnMouseClick;
    }

    public new void Update(GameTime gt)
    {
        base.Update(gt);
        var shouldBeVisible =
          _panelEnabled &&
          GameService.GameIntegration.Gw2Instance.Gw2IsRunning &&
          GameService.GameIntegration.Gw2Instance.IsInGame &&
          GameService.Gw2Mumble.IsAvailable;


        if (GameService.Gw2Mumble.UI.IsMapOpen)
        {
            //shouldBeVisible = shouldBeVisible && (_settings._settingMapVisible.Value == Settings.Enums.VisibleOnMap.ShowOnMap);
            shouldBeVisible = false;
        }
        
        if (_settings._settingOnlyWhenCommander.Value || Service.LtMode.Value)
        {
            shouldBeVisible = shouldBeVisible && CommanderPermissionHelper.PassesCommanderGate();
        }
        

        if (!Visible && shouldBeVisible)
            Show();
        else if (Visible && !shouldBeVisible)
            Hide();

        if (Visible && _draggable && _isDraggedByMouse)
        {
            var nOffset = GameService.Input.Mouse.Position - _dragStart;
            Location += nOffset;

            _dragStart = GameService.Input.Mouse.Position;

        }
    }

    private void AddDragDelegates()
    {
        LeftMouseButtonPressed += delegate
        {
            if (_draggable)
            {
                _isDraggedByMouse = true;
                _dragStart = GameService.Input.Mouse.Position;
            }
        };
        LeftMouseButtonReleased += delegate
        {
            if (_draggable)
            {
                _isDraggedByMouse = false;
                _settings._settingLoc.Value = Location;
            }
        };
    }
    protected FlowPanel CreateGroupingFlowPanel()
    {
        var panel =  new FlowPanel()
        {
            Parent = this,
            FlowDirection = ControlFlowDirection.SingleLeftToRight,
            WidthSizingMode = SizingMode.AutoSize,
            HeightSizingMode = SizingMode.AutoSize,
        };
        panel.LayoutChange(_settings._settingOrientation, 1);
        panel.SizeChange(_settings._settingImgWidth);
        return panel;
    }
    protected void CreateIconButton(Container parent, AsyncTexture2D texture, int size, float opacity, String tooltip, SettingEntry<KeyBinding> keybind, bool groundTarget = true)
    {
        Image button = new Image
        {
            Parent = parent,
            Texture = texture,
            Size = new Point(size,size),
            Opacity = opacity,
            BasicTooltipText =  tooltip
        };
        if (!_mouseEventsEnabled) return;
        if (groundTarget)
        {
            button.LeftMouseButtonPressed += delegate { AddGround(button, keybind.Value); };
            button.RightMouseButtonPressed += delegate { RemoveGround(keybind.Value); };
        } else
        {
            button.LeftMouseButtonPressed += delegate { DoHotKey(keybind.Value); };
            button.RightMouseButtonPressed += delegate { DoHotKey(keybind.Value); };
        }
        
    }

    private void OnMouseClick(object o, MouseEventArgs e)
    {
        if (_draggable) return;
        if (_tmpBinding == null) return;
        if (_mouseIsInsidePanel) return;
        DoHotKey(_tmpBinding);
        ResetGroundIcon();
    }

    protected override void OnMouseEntered(MouseEventArgs e)
    {
        base.OnMouseEntered(e);
        _mouseIsInsidePanel = true;
    }
    protected override void OnMouseLeft(MouseEventArgs e)
    {
        base.OnMouseLeft(e);
        _mouseIsInsidePanel = false;
    }


    protected void ResetGroundIcon()
    {
        if (_tmpButton != null)
        {
            _tmpButton.BackgroundColor = Color.Transparent;
            _tmpButton = null;
        }
        _tmpBinding = null;
    }

    protected void AddGround(Image btn, KeyBinding key)
    {
        if (_draggable) return;
        if(_tmpBinding == key)
        {
            ResetGroundIcon();
            return;
        }
        if (_tmpBinding != null)
        {
            ResetGroundIcon();
        }
        _tmpBinding = key;
        _tmpButton = btn;
        btn.BackgroundColor = Color.Yellow;
    }
    protected void RemoveGround(KeyBinding key)
    {
        if (_draggable) return;
        DoHotKey(key);
        System.Threading.Thread.Sleep(Constants.Delays.HOTKEY_DELAY_MS);
        DoHotKey(key);
    }
    protected void DoHotKey(KeyBinding key)
    {
        if (_draggable) return;
        if (key == null) return;
        if (key.ModifierKeys != ModifierKeys.None)
        {
            if (key.ModifierKeys.HasFlag(ModifierKeys.Alt))
                Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.MENU, true);
            if (key.ModifierKeys.HasFlag(ModifierKeys.Ctrl))
                Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.CONTROL, true);
            if (key.ModifierKeys.HasFlag(ModifierKeys.Shift))
                Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.SHIFT, true);
        }
        Blish_HUD.Controls.Intern.Keyboard.Press(ToVirtualKey(key.PrimaryKey), true);
        System.Threading.Thread.Sleep(Constants.Delays.HOTKEY_DELAY_MS);
        Blish_HUD.Controls.Intern.Keyboard.Release(ToVirtualKey(key.PrimaryKey), true);
        if (key.ModifierKeys != ModifierKeys.None)
        {
            if (key.ModifierKeys.HasFlag(ModifierKeys.Shift))
                Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.SHIFT, true);
            if (key.ModifierKeys.HasFlag(ModifierKeys.Ctrl))
                Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.CONTROL, true);
            if (key.ModifierKeys.HasFlag(ModifierKeys.Alt))
                Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.MENU, true);
        }
    }
    private VirtualKeyShort ToVirtualKey(Keys key)
    {
        try
        {
            return (VirtualKeyShort)key;
        }
        catch
        {
            return new VirtualKeyShort();
        }
    }

}
