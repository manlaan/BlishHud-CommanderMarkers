using Blish_HUD.Input;
using Blish_HUD.Settings;
using Manlaan.CommanderMarkers.Settings.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Manlaan.CommanderMarkers.Settings.Services;

public class SettingService: IDisposable // singular because Setting"s"Service already exists in Blish
{
    public SettingEntry<bool> _settingGroundMarkersEnabled { get; private set; }
    public SettingEntry<bool> _settingTargetMarkersEnabled { get; private set; }
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
    public SettingEntry<KeyBinding> _settingInteractKeyBinding { get; private set; }

    public SettingEntry<Point> _settingLoc { get; private set; }
    public SettingEntry<Layout> _settingOrientation { get; private set; }
    public SettingEntry<int> _settingImgWidth { get; private set; }
    public SettingEntry<float> _settingOpacity { get; private set; }
    public SettingEntry<bool> _settingDrag { get; private set; }
    public SettingEntry<bool> _settingShowMarkersPanel{ get; private set; }
    public SettingEntry<bool> _settingOnlyWhenCommander { get; private set; }

    //public SettingEntry<VisibleOnMap> _settingMapVisible { get; private set; }
    public SettingEntry<int> AutoMarker_PlacementDelay { get; private set; }
    public SettingEntry<bool>AutoMarker_OnlyWhenCommander { get; private set; }
    public SettingEntry<bool> AutoMarker_FeatureEnabled { get; private set; }
    public SettingEntry<bool> AutoMarker_LibraryFilterToCurrent { get; private set; }
    public SettingEntry<bool> AutoMarker_ShowPreview { get; private set; }
    public SettingEntry<bool> CornerIconEnabled { get; private set; }
    public SettingEntry<CornerIconActions> CornerIconLeftClickAction { get; private set; }


    public SettingService(SettingCollection settings)
    {
        _settingGroundMarkersEnabled = settings.DefineSetting("CmdMrkGnnEnabled", true, () => "Show icons for placing Ground Markers", () => "");
        _settingTargetMarkersEnabled = settings.DefineSetting("CmdMrkTgtEnabled", true, () => "Show icons for placing Target/Object Markers", () => "");
        _settingArrowGndBinding = settings.DefineSetting("CmdMrkArrowGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D1), () => "Arrow Ground Binding", () => "");
        _settingArrowGndBinding = settings.DefineSetting("CmdMrkArrowGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D1), () => "Arrow Ground Binding", () => "");
        _settingCircleGndBinding = settings.DefineSetting("CmdMrkCircleGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D2), () => "Circle Ground Binding", () => "");
        _settingHeartGndBinding = settings.DefineSetting("CmdMrkHeartGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D3), () => "Heart Ground Binding", () => "");
        _settingSquareGndBinding = settings.DefineSetting("CmdMrkSquareGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D4), () => "Square Ground Binding", () => "");
        _settingStarGndBinding = settings.DefineSetting("CmdMrkStarGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D5), () => "Star Ground Binding", () => "");
        _settingSpiralGndBinding = settings.DefineSetting("CmdMrkSpiralGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D6), () => "Spiral Ground Binding", () => "");
        _settingTriangleGndBinding = settings.DefineSetting("CmdMrkTriangleGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D7), () => "Triangle Ground Binding", () => "");
        _settingXGndBinding = settings.DefineSetting("CmdMrkXGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D8), () => "X Ground Binding", () => "");
        _settingClearGndBinding = settings.DefineSetting("CmdMrkClearGndBinding", new KeyBinding(ModifierKeys.Alt, Keys.D9), () => "Clear Ground Binding", () => "");

        _settingArrowObjBinding = settings.DefineSetting("CmdMrkArrowObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D1), () => "Arrow Object Binding", () => "");
        _settingCircleObjBinding = settings.DefineSetting("CmdMrkCircleObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D2), () => "Circle Object Binding", () => "");
        _settingHeartObjBinding = settings.DefineSetting("CmdMrkHeartObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D3), () => "Heart Object Binding", () => "");
        _settingSquareObjBinding = settings.DefineSetting("CmdMrkSquareObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D4), () => "Square Object Binding", () => "");
        _settingStarObjBinding = settings.DefineSetting("CmdMrkStarObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D5), () => "Star Object Binding", () => "");
        _settingSpiralObjBinding = settings.DefineSetting("CmdMrkSpiralObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D6), () => "Spiral Object Binding", () => "");
        _settingTriangleObjBinding = settings.DefineSetting("CmdMrkTriangleObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D7), () => "Triangle Object Binding", () => "");
        _settingXObjBinding = settings.DefineSetting("CmdMrkXObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D8), () => "X Object Binding", () => "");
        _settingClearObjBinding = settings.DefineSetting("CmdMrkClearObjBinding", new KeyBinding(ModifierKeys.Alt | ModifierKeys.Shift, Keys.D9), () => "Clear Object Binding", () => "");

        _settingInteractKeyBinding = settings.DefineSetting("CmdMrkInteractBinding", new KeyBinding(Keys.F), () => "Interact Key", () => "");


        _settingLoc = settings.DefineSetting("CmdMrkLoc", new Point(100, 100), () => "Location", () => "");

        _settingOrientation = settings.DefineSetting("CmdMrkOrientation2", Layout.Horizontal, () => "Orientation", () => "");
        _settingImgWidth = settings.DefineSetting("CmdMrkImgWidth", 30, () => "Icon Size", () => "Set the size of the on screen marker icons");
        _settingOpacity = settings.DefineSetting("CmdMrkOpacity", 1.0f, () => "Opacity", () => "Set the panel's transparency\nHidden<---->Visible");
        _settingDrag = settings.DefineSetting("CmdMrkDrag", false, () => "Enable Dragging", () => "Allow the clickable markers to be repositioned");
        _settingShowMarkersPanel = settings.DefineSetting("CmdMrkShowMarkerPanelr", true, () => "Show clickable markers on screen", () => "Hide/show the clickable markers panel");
        _settingOnlyWhenCommander = settings.DefineSetting(
            "CmdMrkOnlyCommander",
            false,
            () => "Only show when I am the Commander",
            () => "Hides the clickable markers when you are not the Commander");
        AutoMarker_PlacementDelay = settings.DefineSetting(
            "CmdMrkPlacementDelay",
            100,
            () => "Placement Delay",
            () => "Delay in milliseconds to wait between marker placement\nFaster <-----> Slower"
            );
        //_settingMapVisible = settings.DefineSetting("CmdMrkShow", VisibleOnMap.HideOnMap, ()=>"Show on map", () => "");

        AutoMarker_PlacementDelay.SetRange(50, 300);
        _settingImgWidth.SetRange(16, 200);
        _settingOpacity.SetRange(0.1f, 1f);


        AutoMarker_OnlyWhenCommander = settings.DefineSetting(
            "CmdMrkAMOnlyCommander",
            true,
            () => "Only show when I am the Commander",
            () => "Only show the AutoMarker activation zones on the map when you are the Commander"
        );
        AutoMarker_FeatureEnabled = settings.DefineSetting(
            "CmdMrkAMEnabled",
            true,
            () => "Enable",
            () => ""
        );
        AutoMarker_LibraryFilterToCurrent = settings.DefineSetting(
            "CmdMrkAMLibraryFilter",
            false,
            () => "Filter to current map",
            () => "Filter the library list to only show marker sets for your current map"
        );
        AutoMarker_ShowPreview = settings.DefineSetting(
            "CmdMrkAMShowPreview",
            true,
            () => "Show preview of markers",
            () => "Draw a preview of the markers when placing from the menu bar icon"
        );

        CornerIconEnabled = settings.DefineSetting(
            "CmdMrkCornerIconEnabled",
            true,
            () => "Show an icon in the top-left manu bar",
            () => "Adds a shortcut icon in the top-left menu bar"
        );

        CornerIconLeftClickAction = settings.DefineSetting(
            "CmdMrkAMCornerIconAction",
            CornerIconActions.SHOW_ICON_MENU,
            () => "Icon left-click action",
            () => "Select an action for menu bar icon left-click\nRight click will always open a small menu"
        );
    }

    public void Dispose()
    {

    }
}