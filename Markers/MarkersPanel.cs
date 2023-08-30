using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Controls.Extern;
using Blish_HUD.Input;
using Blish_HUD.Settings;
using Manlaan.CommanderMarkers.Localization;
using Manlaan.CommanderMarkers.Settings.Models;
using Manlaan.CommanderMarkers.Settings.Services;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Runtime;

namespace Manlaan.CommanderMarkers.Markers;


public class MarkersPanel : FlowPanel, IDisposable
{
    private bool _draggable = false;
    private bool _isDraggedByMouse;

    private bool _mouseEventsEnabled;

    private bool _panelEnabled = true;

    private Point _dragStart = Point.Zero;


    private KeyBinding _tmpBinding;
    private Image _tmpButton;
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
        var objectIcons = CreateGroupingFlowPanel(); 
        //arrow
        CreateIconButton(groundIcons, _textures._imgArrow, size, opacity, "Arrow Ground", _settings._settingArrowGndBinding, true);
        CreateIconButton(objectIcons, _textures._imgArrow, size, opacity, "Arrow Object", _settings._settingArrowObjBinding, false);
        //circle
        CreateIconButton(groundIcons, _textures._imgCircle, size, opacity, "Circle Ground", _settings._settingCircleGndBinding, true);
        CreateIconButton(objectIcons, _textures._imgCircle, size, opacity, "Circle Object", _settings._settingCircleObjBinding, false);
        //heart
        CreateIconButton(groundIcons, _textures._imgHeart, size, opacity, "Heart Ground", _settings._settingHeartGndBinding, true);
        CreateIconButton(objectIcons, _textures._imgHeart, size, opacity, "Heart Object", _settings._settingHeartObjBinding, false);
        //square
        CreateIconButton(groundIcons, _textures._imgSquare, size, opacity, "Square Ground", _settings._settingSquareGndBinding, true);
        CreateIconButton(objectIcons, _textures._imgSquare, size, opacity, "Square Object", _settings._settingSquareObjBinding, false);
        //star
        CreateIconButton(groundIcons, _textures._imgStar, size, opacity, "Star Ground", _settings._settingStarGndBinding, true);
        CreateIconButton(objectIcons, _textures._imgStar, size, opacity, "Star Object", _settings._settingStarObjBinding, false);
        //spiral
        CreateIconButton(groundIcons, _textures._imgSpiral, size, opacity, "Spiral Ground", _settings._settingSpiralGndBinding, true);
        CreateIconButton(objectIcons, _textures._imgSpiral, size, opacity, "Spiral Object", _settings._settingSpiralObjBinding, false);
        //triangle
        CreateIconButton(groundIcons, _textures._imgTriangle, size, opacity, "Triangle Ground", _settings._settingTriangleGndBinding, true);
        CreateIconButton(objectIcons, _textures._imgTriangle, size, opacity, "Triangle Object", _settings._settingTriangleObjBinding, false);
        //x
        CreateIconButton(groundIcons, _textures._imgX, size, opacity, "X Ground", _settings._settingXGndBinding, true);
        CreateIconButton(objectIcons, _textures._imgX, size, opacity, "X Object", _settings._settingXObjBinding, false);
        //clear
        CreateIconButton(groundIcons, _textures._imgClear, size, opacity, "Clear Ground", _settings._settingClearGndBinding, false);
        CreateIconButton(objectIcons, _textures._imgClear, size, opacity, "Clear Object", _settings._settingClearObjBinding, false);

        if(_mouseEventsEnabled)
            AddDragDelegates();

        _settings._settingDrag.SettingChanged += (s, e) => _draggable = e.NewValue;
        _draggable = _settings._settingDrag.Value;

        _panelEnabled = _settings._settingShowMarkersPanel.Value;
        _settings._settingShowMarkersPanel.SettingChanged += (s, e) => { _panelEnabled = e.NewValue; };


        if(_mouseEventsEnabled)
            GameService.Input.Mouse.LeftMouseButtonPressed += OnMouseClick;
    }

    public override void PaintAfterChildren(SpriteBatch spriteBatch, Rectangle bounds)
    {
        if (_draggable && _mouseEventsEnabled)
        {
            spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(0,0,this.Width, this.Height), new Color(96,96,96,192));
            spriteBatch.DrawStringOnCtrl(this, "Drag", _dragFont, new Rectangle(0, 0, this.Width, this.Height), Color.Black, horizontalAlignment: Blish_HUD.Controls.HorizontalAlignment.Center, verticalAlignment: VerticalAlignment.Middle);

        }
    }
    public void Dispose()
    {
        if(_mouseEventsEnabled)
            GameService.Input.Mouse.LeftMouseButtonPressed -= OnMouseClick;
        base.Dispose();
    }

    public void Update(GameTime gt)
    {

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
        
        if (_settings._settingOnlyWhenCommander.Value)
        {
            shouldBeVisible = shouldBeVisible && GameService.Gw2Mumble.PlayerCharacter.IsCommander;
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
        DoHotKey(_tmpBinding);
        ResetGroundIcon();
    }
    protected void ResetGroundIcon()
    {
        _tmpButton.BackgroundColor = Color.Transparent;
        _tmpBinding = null;
        _tmpButton = null;
    }

    protected void AddGround(Image btn, KeyBinding key)
    {
        if (_draggable) return;
        if (_tmpBinding == key)
        {
            ResetGroundIcon();
            return;
        }
        _tmpBinding = key;
        _tmpButton = btn;
        btn.BackgroundColor = Color.Yellow;
    }
    protected void RemoveGround(KeyBinding key)
    {
        if (_draggable) return;
        DoHotKey(key);
        System.Threading.Thread.Sleep(50);
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
        System.Threading.Thread.Sleep(50);
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
