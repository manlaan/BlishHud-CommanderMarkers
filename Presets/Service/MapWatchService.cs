using Blish_HUD;
using Blish_HUD.Common.Gw2;
using Blish_HUD.Input;
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

namespace Manlaan.CommanderMarkers.Presets.Service;

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

        _setting._settingInteractKeyBinding.Value.Enabled= true;
        _setting._settingInteractKeyBinding.Value.BlockSequenceFromGw2 = false;
        _setting._settingInteractKeyBinding.Value.Activated += _interactKeybind_Activated;

        CurrentMap_MapChanged(this, new ValueEventArgs<int>(GameService.Gw2Mumble.CurrentMap.Id));

    }

    private void _interactKeybind_Activated(object sender, EventArgs e)
    {
        if(_markers.Count <=0) return;
        if(GameService.Gw2Mumble.UI.IsMapOpen ==false) return;

        var playerPosition = GameService.Gw2Mumble.PlayerCharacter.Position;
        foreach(MarkerSet marker in _markers) {
            var d = (playerPosition - marker.trigger.ToVector3()).Length();
            if(d < 15f)
            {
                Debug.WriteLine($"Found a marker to activate (d={d})");
                PlaceMarkers(marker, _map);
            }
            else
            {
                Debug.WriteLine($"Distance to {marker.name} is {d.ToString()}");
            }
        }
    }

    public void Update(GameTime gameTime)
    {
        _screenMap.Update(gameTime);
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

        var originalMousePos = Mouse.GetState().Position;
        InputHelper.DoHotKey(keys[0]);
        Thread.Sleep(40);

        for (var i = 0; i < markers.marks.Count; i++)
        {
            var marker = markers.marks[i];

            if (marker.icon > 9 || marker.icon < 0) continue;

            var d = mapData.WorldToScreenMap(marker.ToVector3()) * scale;
            Mouse.SetPosition((int)d.X, (int)d.Y);
            Thread.Sleep(20);
            InputHelper.DoHotKey(keys[marker.icon]);
            Thread.Sleep(60);
        }

        Mouse.SetPosition(originalMousePos.X, originalMousePos.Y);

        return Task.CompletedTask;
    
    }

    private void CurrentMap_MapChanged(object sender, ValueEventArgs<int> e)
    {
        _currentmap= e.Value;
        _markers = CommanderMarkers.Service.MarkersListing.GetMarkersForMap(e.Value);
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

        GameService.Gw2Mumble.CurrentMap.MapChanged -= CurrentMap_MapChanged;
        _setting._settingInteractKeyBinding.Value.Enabled= false;
        _setting._settingInteractKeyBinding.Value.Activated -= _interactKeybind_Activated;
    }
}
