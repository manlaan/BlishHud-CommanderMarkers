using Blish_HUD;
using Blish_HUD.Input;
using Blish_HUD.Settings;
using Manlaan.CommanderMarkers.Settings.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Manlaan.CommanderMarkers.Settings.Services;

public class SettingService: IDisposable // singular because Setting"s"Service already exists in Blish
{
    public SettingEntry<KeyBinding> _settingArrowGndBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingCircleGndBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingHeartGndBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingSpiralGndBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingSquareGndBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingStarGndBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingTriangleGndBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingXGndBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingClearGndBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingArrowObjBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingCircleObjBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingHeartObjBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingSpiralObjBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingSquareObjBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingStarObjBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingTriangleObjBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingXObjBinding { get; private set; }
    public SettingEntry<KeyBinding> _settingClearObjBinding { get; private set; }

    public SettingEntry<string> _settingOrientation { get; private set; }
    public SettingEntry<Point> _settingLoc { get; private set; }
    public SettingEntry<int> _settingImgWidth { get; private set; }
    public SettingEntry<float> _settingOpacity { get; private set; }
    public SettingEntry<bool> _settingDrag { get; private set; }
    public SettingEntry<bool> _settingOnlyWhenCommander { get; private set; }
    //public SettingEntry<VisibleOnMap> _settingMapVisible { get; private set; }


    public SettingService(SettingCollection settings)
    {
        _settingArrowGndBinding = settings.DefineSetting("CmdMrkArrowGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D1), ()=>"Arrow Ground Binding", ()=>"");
        _settingCircleGndBinding = settings.DefineSetting("CmdMrkCircleGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D2), ()=>"Circle Ground Binding", ()=>"");
        _settingHeartGndBinding = settings.DefineSetting("CmdMrkHeartGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D3), ()=>"Heart Ground Binding", ()=>"");
        _settingSquareGndBinding = settings.DefineSetting("CmdMrkSquareGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D4), ()=>"Square Ground Binding", ()=>"");
        _settingStarGndBinding = settings.DefineSetting("CmdMrkStarGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D5), ()=>"Star Ground Binding", ()=>"");
        _settingSpiralGndBinding = settings.DefineSetting("CmdMrkSpiralGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D6), ()=>"Spiral Ground Binding", ()=>"");
        _settingTriangleGndBinding = settings.DefineSetting("CmdMrkTriangleGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D7), ()=>"Triangle Ground Binding", ()=>"");
        _settingXGndBinding = settings.DefineSetting("CmdMrkXGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D8), ()=>"X Ground Binding", ()=>"");
        _settingClearGndBinding = settings.DefineSetting("CmdMrkClearGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D9), ()=>"Clear Ground Binding", ()=>"");

        _settingArrowObjBinding = settings.DefineSetting("CmdMrkArrowObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D1), ()=>"Arrow Object Binding", ()=>"");
        _settingCircleObjBinding = settings.DefineSetting("CmdMrkCircleObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D2), ()=>"Circle Object Binding", ()=>"");
        _settingHeartObjBinding = settings.DefineSetting("CmdMrkHeartObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D3), ()=>"Heart Object Binding", ()=>"");
        _settingSquareObjBinding = settings.DefineSetting("CmdMrkSquareObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D4), ()=>"Square Object Binding", ()=>"");
        _settingStarObjBinding = settings.DefineSetting("CmdMrkStarObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D5), ()=>"Star Object Binding", ()=>"");
        _settingSpiralObjBinding = settings.DefineSetting("CmdMrkSpiralObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D6), ()=>"Spiral Object Binding", ()=>"");
        _settingTriangleObjBinding = settings.DefineSetting("CmdMrkTriangleObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D7), ()=>"Triangle Object Binding", ()=>"");
        _settingXObjBinding = settings.DefineSetting("CmdMrkXObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D8), ()=>"X Object Binding", ()=>"");
        _settingClearObjBinding = settings.DefineSetting("CmdMrkClearObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D9), ()=>"Clear Object Binding", ()=>"");

        _settingOrientation = settings.DefineSetting("CmdMrkOrientation2", "Horizontal", () => "Orientation", ()=>"");
        _settingLoc = settings.DefineSetting("CmdMrkLoc", new Point(100, 100), ()=>"Location", ()=>"");
        _settingImgWidth = settings.DefineSetting("CmdMrkImgWidth", 30, ()=> "Width", ()=>"");
        _settingOpacity = settings.DefineSetting("CmdMrkOpacity", 1.0f, () => "Opacity", ()=>"");
        _settingDrag = settings.DefineSetting("CmdMrkDrag", false, ()=>"Enable Dragging", () => "Allow the markers to be repositioned");
        _settingOnlyWhenCommander = settings.DefineSetting("CmdMrkOnlyCommander", true, ()=>"Only show when I am the Commander", () => "Hides the markers when you are not a Commander");
        //_settingMapVisible = settings.DefineSetting("CmdMrkShow", VisibleOnMap.HideOnMap, ()=>"Show on map", () => "");
        
        _settingImgWidth.SetRange(16, 200);
        _settingOpacity.SetRange(0.1f, 1f);

       /* _settingArrowGndBinding.SettingChanged += UpdateSettings;
        _settingCircleGndBinding.SettingChanged += UpdateSettings;
        _settingHeartGndBinding.SettingChanged += UpdateSettings;
        _settingSpiralGndBinding.SettingChanged += UpdateSettings;
        _settingSquareGndBinding.SettingChanged += UpdateSettings;
        _settingStarGndBinding.SettingChanged += UpdateSettings;
        _settingTriangleGndBinding.SettingChanged += UpdateSettings;
        _settingXGndBinding.SettingChanged += UpdateSettings;
        _settingClearGndBinding.SettingChanged += UpdateSettings;

        _settingArrowObjBinding.SettingChanged += UpdateSettings;
        _settingCircleObjBinding.SettingChanged += UpdateSettings;
        _settingHeartObjBinding.SettingChanged += UpdateSettings;
        _settingSpiralObjBinding.SettingChanged += UpdateSettings;
        _settingSquareObjBinding.SettingChanged += UpdateSettings;
        _settingStarObjBinding.SettingChanged += UpdateSettings;
        _settingTriangleObjBinding.SettingChanged += UpdateSettings;
        _settingXObjBinding.SettingChanged += UpdateSettings;
        _settingClearObjBinding.SettingChanged += UpdateSettings;

        _settingOrientation.SettingChanged += UpdateSettings;
        _settingLoc.SettingChanged += UpdateSettings;
        _settingImgWidth.SettingChanged += UpdateSettings;
        _settingOpacity.SettingChanged += UpdateSettings;
        _settingDrag.SettingChanged += UpdateSettings;*/
    }

    public void Dispose()
    {
       /* _settingArrowGndBinding!.SettingChanged -= UpdateSettings;
        _settingCircleGndBinding!.SettingChanged -= UpdateSettings;
        _settingHeartGndBinding!.SettingChanged -= UpdateSettings;
        _settingSpiralGndBinding!.SettingChanged -= UpdateSettings;
        _settingSquareGndBinding!.SettingChanged -= UpdateSettings;
        _settingStarGndBinding!.SettingChanged -= UpdateSettings;
        _settingTriangleGndBinding!.SettingChanged -= UpdateSettings;
        _settingXGndBinding!.SettingChanged -= UpdateSettings;
        _settingClearGndBinding!.SettingChanged -= UpdateSettings;

        _settingArrowObjBinding!.SettingChanged -= UpdateSettings;
        _settingCircleObjBinding!.SettingChanged -= UpdateSettings;
        _settingHeartObjBinding!.SettingChanged -= UpdateSettings;
        _settingSpiralObjBinding!.SettingChanged -= UpdateSettings;
        _settingSquareObjBinding!.SettingChanged -= UpdateSettings;
        _settingStarObjBinding!.SettingChanged -= UpdateSettings;
        _settingTriangleObjBinding!.SettingChanged -= UpdateSettings;
        _settingXObjBinding!.SettingChanged -= UpdateSettings;
        _settingClearObjBinding!.SettingChanged -= UpdateSettings;

        _settingOrientation!.SettingChanged -= UpdateSettings;
        _settingLoc!.SettingChanged -= UpdateSettings;
        _settingImgWidth!.SettingChanged -= UpdateSettings;
        _settingOpacity!.SettingChanged -= UpdateSettings;
        _settingDrag!.SettingChanged -= UpdateSettings;*/
    }

/*    private void UpdateSettings(object sender = null, ValueChangedEventArgs<KeyBinding> e = null)
    {
        DrawIcons();
    }
    private void UpdateSettings(object sender = null, ValueChangedEventArgs<Point> e = null)
    {
        DrawIcons();
    }
    private void UpdateSettings(object sender = null, ValueChangedEventArgs<float> e = null)
    {
        DrawIcons();
    }
    private void UpdateSettings(object sender = null, ValueChangedEventArgs<string> e = null)
    {
        DrawIcons();
    }
    private void UpdateSettings(object sender = null, ValueChangedEventArgs<int> e = null)
    {
        DrawIcons();
    }
    private void UpdateSettings(object sender = null, ValueChangedEventArgs<bool> e = null)
    {
        DrawIcons();
    }

    protected void DrawIcons()
    {

    }*/

}