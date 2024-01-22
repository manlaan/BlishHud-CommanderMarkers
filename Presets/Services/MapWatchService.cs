using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Manlaan.CommanderMarkers.Library.Enums;
using Manlaan.CommanderMarkers.Presets.Model;
using Manlaan.CommanderMarkers.Settings.Services;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Presets.Services;

public class MapWatchService : IDisposable
{
    private MapData _map;
    private SettingService _setting;
    private int _currentmap = 0;
    private List<MarkerSet> _markers = new List<MarkerSet>();

    private ScreenMap _screenMap;
    private List<BasicMarker> _triggerMarker = new();

    private MarkerPreview? _previewMarkerSet;

    public MapWatchService(MapData map, SettingService settings) {

        _screenMap = new ScreenMap(map) {
            Parent = GameService.Graphics.SpriteScreen
        };
        _map = map;
        _setting = settings;
        GameService.Gw2Mumble.CurrentMap.MapChanged += CurrentMap_MapChanged;
        Service.MarkersListing.MarkersChanged += MarkersListing_MarkersChanged;

        _setting._settingInteractKeyBinding.Value.Enabled = true;
        _setting._settingInteractKeyBinding.Value.BlockSequenceFromGw2 = false;
        _setting._settingInteractKeyBinding.Value.Activated += _interactKeybind_Activated;

        CurrentMap_MapChanged(this, new ValueEventArgs<int>(GameService.Gw2Mumble.CurrentMap.Id));
        _setting.AutoMarker_FeatureEnabled.SettingChanged += AutoMarkerBooleanSettingChanged;
        _setting.AutoMarker_ShowTrigger.SettingChanged += AutoMarkerBooleanSettingChanged;
        Service.LtMode.SettingChanged += AutoMarkerBooleanSettingChanged;

    }

    public Vector3 GetWorldCoordsFromMouseScreenMap()
    {
        try
        {
            var originalMousePos = Mouse.GetState().Position;
            //The math needs to be mirrored over the origin to align with the mouse.
            //I still do not understand why
            originalMousePos = originalMousePos.MirrorOverOrigin(ScreenMap.Data.ScreenBounds.Center);
            Vector2 mapCoords = _map.ScreenMapToMap(originalMousePos.ToVector2());
            Vector3 worldCoords = _map.MapToWorld(mapCoords);

            return worldCoords;
        }catch(Exception)
        {
            return Vector3.Zero;
        }
    }

    private void AutoMarkerBooleanSettingChanged(object sender, ValueChangedEventArgs<bool> e)
    {
        CurrentMap_MapChanged(this, new ValueEventArgs<int>(GameService.Gw2Mumble.CurrentMap.Id));
    }

    private void MarkersListing_MarkersChanged(object sender, EventArgs e)
    {
        CurrentMap_MapChanged(this, new ValueEventArgs<int>(GameService.Gw2Mumble.CurrentMap.Id));
    }

    private void _interactKeybind_Activated(object sender, EventArgs e)
    {
        if(_markers.Count <=0) return;
        if(GameService.Gw2Mumble.UI.IsMapOpen ==false) return;

        if (!ShouldAttemptPlacement()) return;

        var playerPosition = GameService.Gw2Mumble.PlayerCharacter.Position;
        foreach(MarkerSet marker in _markers) {
            var d = (playerPosition - marker.trigger?.ToVector3())?.Length() ?? 1000f;
            if(d < 15f)
            {
                PlaceMarkers(marker, _map);
                return;
            }
        }
    }

    public void Update(GameTime gameTime)
    {
        _screenMap.Update(gameTime);
    }

    public Task PlaceMarkers(MarkerSet marders)
    {
        return PlaceMarkers(marders, _map);
    }

    private bool ShouldAttemptPlacement()
    {
        var shouldDoIt =
          Service.Settings.AutoMarker_FeatureEnabled.Value &&
          GameService.GameIntegration.Gw2Instance.Gw2IsRunning &&
          GameService.GameIntegration.Gw2Instance.IsInGame &&
          GameService.Gw2Mumble.IsAvailable;

        if (Service.Settings._settingOnlyWhenCommander.Value || Service.LtMode.Value)
        {
            shouldDoIt &= (GameService.Gw2Mumble.PlayerCharacter.IsCommander || Service.LtMode.Value);
        }
        return shouldDoIt;
            
    }
    private Task PlaceMarkers(MarkerSet markers, MapData mapData)
    {
        if (markers.marks == null) return Task.CompletedTask;

        var scale = GameService.Graphics.UIScaleMultiplier;
        var keys = new List<KeyBinding>
        {
            _setting._settingClearGndBinding.Value,
            _setting._settingArrowGndBinding.Value,
            _setting._settingCircleGndBinding.Value,
            _setting._settingHeartGndBinding.Value,
            _setting._settingSquareGndBinding.Value,
            _setting._settingStarGndBinding.Value,
            _setting._settingSpiralGndBinding.Value,
            _setting._settingTriangleGndBinding.Value,
            _setting._settingXGndBinding.Value,
            _setting._settingClearGndBinding.Value,
        };
        var delay = _setting.AutoMarker_PlacementDelay.Value;

        var originalMousePos = Mouse.GetState().Position;

        var screenBounds = ScreenMap.Data.ScreenBounds;
        InputHelper.DoHotKey(keys[0]);
        Thread.Sleep((int) delay/2);
        var errors = new List<string>();
        for (var i = 0; i < markers.marks.Count; i++)
        {
            var marker = markers.marks[i];

            if (marker.icon > 9 || marker.icon < 0) continue;

            var blishCoord = mapData.WorldToScreenMap(marker.ToVector3());
            var d = blishCoord * scale;
            if (screenBounds.Contains(blishCoord))
            {
                Mouse.SetPosition((int)d.X, (int)d.Y);
                Thread.Sleep((int) delay/2);
                InputHelper.DoHotKey(keys[marker.icon]);
                Thread.Sleep(delay);

            }
            else
            {
                errors.Add($"{((SquadMarker)marker.icon).EnumValue()} {marker.name}");
            }

        }
        if(errors.Count > 0)
        {

            ScreenNotification.ShowNotification(
                $"Unable to place {errors.Count} marker(s)\nTry moving your map to the marker trigger",
                ScreenNotification.NotificationType.Warning, null, 6
            );
        }

        Mouse.SetPosition(originalMousePos.X, originalMousePos.Y);

        return Task.CompletedTask;
    
    }

    private void CurrentMap_MapChanged(object sender, ValueEventArgs<int> e)
    {
        _screenMap.ClearEntities();
        if (!_setting.AutoMarker_ShowTrigger.Value) return;
        _currentmap= e.Value;
        _markers = Service.MarkersListing.GetMarkersForMap(e.Value).Where(m => m.enabled).ToList();
        foreach(var marker in _markers)
        {
            _screenMap.AddEntity(new BasicMarker(_map, marker.trigger!.ToVector3(), marker.name, marker.description));
            //PreviewMarkerSet(marker);

        }
    }


    public void PreviewMarkerSet(MarkerSet preview)
    {
        RemovePreviewMarkerSet();
        if (Service.Settings.AutoMarker_ShowPreview.Value)
        {
            _previewMarkerSet = new MarkerPreview(_map, preview);
            _screenMap.AddEntity(_previewMarkerSet);
        }

    }
    public void PreviewClosestMarkerSet()
    {
        var playerPosition = GameService.Gw2Mumble.PlayerCharacter.Position;
        foreach (MarkerSet marker in _markers)
        {
            var d = (playerPosition - marker.trigger?.ToVector3())?.Length() ?? 1000f;
            if (d < 15f)
            {
                PreviewMarkerSet(marker);
                return;
            }
        }
    }
    public void RemovePreviewMarkerSet()
    {
        if (_previewMarkerSet != null)
        {
            _screenMap.RemoveEntity(_previewMarkerSet);
            _previewMarkerSet = null;
        }
    }

    public void Dispose()
    {
        _screenMap.Dispose();

        _setting.AutoMarker_ShowTrigger.SettingChanged -= AutoMarkerBooleanSettingChanged;
        _setting.AutoMarker_FeatureEnabled.SettingChanged -= AutoMarkerBooleanSettingChanged;
        Service.LtMode.SettingChanged -= AutoMarkerBooleanSettingChanged;
        Service.MarkersListing.MarkersChanged -= MarkersListing_MarkersChanged;
        GameService.Gw2Mumble.CurrentMap.MapChanged -= CurrentMap_MapChanged;
        _setting._settingInteractKeyBinding.Value.Enabled= false;
        _setting._settingInteractKeyBinding.Value.Activated -= _interactKeybind_Activated;
    }
}
