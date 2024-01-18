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

namespace Manlaan.CommanderMarkers.Presets.Model;

public class BillBoardPreview
{
    private MapData _mapData;
    private Vector3 _trigger;
    private List<Billboard> _billboard = new List<Billboard>();
    public BillBoardPreview(MapData mapData, MarkerSet markerSet)
    {
        _mapData = mapData;
        markerSet.marks.ForEach(mark =>
        {
            Texture2D markerIcon = ((SquadMarker)mark.icon).GetIcon();
            _billboard.Add(new Billboard(markerIcon, mark.ToVector3(),new Vector2(2,2)));
        });

    }
    public float DistanceFrom(Vector3 playerPosition)
    {
        return (playerPosition - _trigger).Length();
    }

    public void Draw()
    {
        
       /* _markerSet.marks.ForEach(mark =>
        {
            Texture2D markerIcon = ((SquadMarker)mark.icon).GetIcon();
            var mapCoordinates = _mapData.WorldToScreenMap(mark.ToVector3());
            var bounds = new Rectangle((int)mapCoordinates.X - 16, (int)mapCoordinates.Y - 16, 32, 32);
            spriteBatch.Draw(markerIcon, bounds, Color.White);
            //spriteBatch.DrawString(_bitmapFont, $"{mapCoordinates}", mapCoordinates, Color.Orange);

        });*/
        using (var ctx = GameService.Graphics.LendGraphicsDeviceContext())
        {
            for (var i = 0; i < _billboard.Count; i++)
            {
                _billboard[i].HandleRebuild(ctx.GraphicsDevice);
                _billboard[i].Draw(ctx.GraphicsDevice);
            }
        }
    }

    public string GetMarkerText()
    {
        return  "marker";  
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

        var originalMousePos = Mouse.GetState().Position;

        var screenBounds = ScreenMap.Data.ScreenBounds;
        InputHelper.DoHotKey(keys[0]);
        Thread.Sleep((int)delay / 2);
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

        Mouse.SetPosition(originalMousePos.X, originalMousePos.Y);

        return Task.CompletedTask;

    }
}
