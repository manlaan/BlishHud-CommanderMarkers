using Blish_HUD;
using Blish_HUD.Common.Gw2;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Manlaan.CommanderMarkers.Library.Enums;
using Manlaan.CommanderMarkers.Presets.Model;
using Manlaan.CommanderMarkers.Settings.Services;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    private List<MarkerSet> _markers= new List<MarkerSet>();

    private ScreenMap _screenMap;
    private List<BasicMarker> _triggerMarker = new();
    public MapWatchService(MapData map, SettingService settings) {

        _screenMap = new ScreenMap(map) {
            Parent = GameService.Graphics.SpriteScreen
        };
        _map = map;
        _setting = settings;
        GameService.Gw2Mumble.CurrentMap.MapChanged += CurrentMap_MapChanged;
        Service.MarkersListing.MarkersChanged += MarkersListing_MarkersChanged;

        _setting._settingInteractKeyBinding.Value.Enabled= true;
        _setting._settingInteractKeyBinding.Value.BlockSequenceFromGw2 = false;
        _setting._settingInteractKeyBinding.Value.Activated += _interactKeybind_Activated;

        CurrentMap_MapChanged(this, new ValueEventArgs<int>(GameService.Gw2Mumble.CurrentMap.Id));
        _setting.AutoMarker_FeatureEnabled.SettingChanged += AutoMarker_FeatureEnabled_SettingChanged;
        Service.LtMode.SettingChanged += AutoMarker_FeatureEnabled_SettingChanged;
    }

    private void AutoMarker_FeatureEnabled_SettingChanged(object sender, ValueChangedEventArgs<bool> e)
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

            var d = mapData.WorldToScreenMap(marker.ToVector3()) * scale;
            if (screenBounds.Contains(d))
            {
                Mouse.SetPosition((int)d.X, (int)d.Y);
                Thread.Sleep((int) delay/2);
                InputHelper.DoHotKey(keys[marker.icon]);
                Thread.Sleep(delay);

            }
            else
            {
                Debug.WriteLine($"{marker.icon} {d} is not in mapbounds {screenBounds}");
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
        _currentmap= e.Value;
        _markers = Service.MarkersListing.GetMarkersForMap(e.Value).Where(m => m.enabled).ToList();
        Debug.WriteLine($"Found {_markers.Count()} marker sets for this map {e.Value}");
        _screenMap.ClearEntities();
        foreach(var marker in _markers)
        {
            _screenMap.AddEntity(new BasicMarker(_map, marker.trigger!.ToVector3(), marker.name, marker.description));

        }
    }

    public void Dispose()
    {
        _screenMap.Dispose();

        _setting.AutoMarker_FeatureEnabled.SettingChanged -= AutoMarker_FeatureEnabled_SettingChanged;
        Service.MarkersListing.MarkersChanged -= MarkersListing_MarkersChanged;
        GameService.Gw2Mumble.CurrentMap.MapChanged -= CurrentMap_MapChanged;
        _setting._settingInteractKeyBinding.Value.Enabled= false;
        _setting._settingInteractKeyBinding.Value.Activated -= _interactKeybind_Activated;
    }
}
