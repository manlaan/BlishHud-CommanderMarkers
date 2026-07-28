using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Manlaan.CommanderMarkers.Library.Enums;
using Manlaan.CommanderMarkers.Pathing.Entities;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using Manlaan.CommanderMarkers.Presets.Services;

namespace Manlaan.CommanderMarkers.Presets.Model;

/**
 * 1032324
 * 1032325
 * 1701859 Back with write top/bot borner
 * 358411 blue header
 * 2420385 masked bubble
 * 
 * 155208
 * https://assets.gw2dat.com/156112.png
 **/

public class BillBoardPreview
{
    private MapData _mapData;
    private Vector3 _trigger;
    private List<Billboard> _billboard = [];
    private Billboard trigger = new();
    private MarkerSet _markerSet;
    public BillBoardPreview(MapData mapData, MarkerSet markerSet)
    {
        _markerSet = markerSet;
        _mapData = mapData;
        _trigger = markerSet.trigger?.ToVector3() ?? Vector3.Zero;
        markerSet.marks.ForEach(mark =>
        {
            Texture2D markerIcon = ((SquadMarker)mark.icon).GetIcon();
            _billboard.Add(new Billboard(markerIcon, mark.ToVector3(),new Vector2(1,1)));
        });
        if (markerSet.trigger != null)
        {
            Texture2D markerIcon = Service.Textures!._blishHeart;
            trigger = new Billboard(markerIcon, markerSet.trigger.ToVector3(), new Vector2(1, 1));
        }

    }
    public float DistanceFrom(Vector3 playerPosition)
    {
        return (playerPosition - _trigger).Length();
    }

    public void Draw()
    {
        using (var ctx = GameService.Graphics.LendGraphicsDeviceContext())
        {
            if (GameService.Gw2Mumble.PlayerCharacter.IsInCombat && !Service.Settings.AutoMarker_Allow_Combat_Placement.Value) return;

            var d = trigger.DistanceFromPlayer;
            trigger.HandleRebuild(ctx.GraphicsDevice);
            trigger.Opacity = d < MapWatchService.TRIGGER_DISTANCE_CLOSED_MAP ? 0.25f : 0.8f;
            trigger.Draw(ctx.GraphicsDevice);

            if (!Service.Settings.AutoMarker_Billboard_Preview.Value) return;

            if (d < MapWatchService.TRIGGER_DISTANCE_CLOSED_MAP)
            {
                for (var i = 0; i < _billboard.Count; i++)
                {
                    _billboard[i].HandleRebuild(ctx.GraphicsDevice);
                    _billboard[i].Draw(ctx.GraphicsDevice);
                }
            }
        }
    }

    public string GetMarkerText()
    {
        return _markerSet?.name ?? "";  
    }

    public bool PlayerWithinTriggerDistance()
    {
        return trigger.DistanceFromPlayer < MapWatchService.TRIGGER_DISTANCE_CLOSED_MAP;
    }

    private Task PlaceMarkersInWorld(MarkerSet markers, MapData mapData)
    {
        if (markers.marks == null) return Task.CompletedTask;
        var _setting = Service.Settings;
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

        bool useScreenCoords = MarkerPlacementHelper.UseScreenCoordinatesForPlacement();
        var originalMousePos = MarkerPlacementHelper.GetPlacementCursorPosition(useScreenCoords);

        var screenBounds = ScreenMap.Data.ScreenBounds;
        InputHelper.DoHotKey(keys[0]);
        Thread.Sleep((int)delay / 2);
        var errors = new List<string>();
        for (var i = 0; i < markers.marks.Count; i++)
        {
            var marker = markers.marks[i];

            if (marker.icon > 9 || marker.icon < 0) continue;

            var blishCoord = mapData.WorldToScreenMap(marker.ToVector3());
            var placementPos = MarkerPlacementHelper.BlishToPlacementPosition(blishCoord, scale);
            if (screenBounds.Contains(blishCoord))
            {
                MarkerPlacementHelper.SetPlacementMousePosition(placementPos, useScreenCoords);
                Thread.Sleep((int)delay / 2);
                InputHelper.DoHotKey(keys[marker.icon]);
                Thread.Sleep(delay);
            }
            else
            {
                errors.Add($"{((SquadMarker)marker.icon).EnumValue()} {marker.name}");
            }

        }
        if (errors.Count > 0)
        {

            ScreenNotification.ShowNotification(
                $"Unable to place {errors.Count} marker(s)\nTry moving your map to the marker trigger",
                ScreenNotification.NotificationType.Warning, null, 6
            );
        }

        MarkerPlacementHelper.SetPlacementMousePosition(originalMousePos, useScreenCoords);

        return Task.CompletedTask;

    }
}