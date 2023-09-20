using Blish_HUD;
using Blish_HUD.Controls;
using Manlaan.CommanderMarkers.Library.Enums;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Presets.Model;

public class MarkerPreview : IMapEntity
{
    private MapData _mapData;
    private Vector3 _trigger;
    private MarkerSet _markerSet;
    private BitmapFont _bitmapFont = GameService.Content.DefaultFont16;
    public MarkerPreview(MapData mapData, MarkerSet markerSet)
    {
        _mapData = mapData;
        _markerSet = markerSet;
    }
    public float DistanceFrom(Vector3 playerPosition)
    {
        return (playerPosition - _trigger).Length();
    }

    public void DrawToMap(SpriteBatch spriteBatch, IMapBounds map, Control control, Vector3 playerPosition)
    {
        
        _markerSet.marks.ForEach(mark =>
        {
            Texture2D markerIcon = ((SquadMarker)mark.icon).GetIcon();
            var mapCoordinates = _mapData.WorldToScreenMap(mark.ToVector3());
            var bounds = new Rectangle((int)mapCoordinates.X - 16, (int)mapCoordinates.Y - 16, 32, 32);
            spriteBatch.Draw(markerIcon, bounds, Color.White);
            //spriteBatch.DrawString(_bitmapFont, $"{mapCoordinates}", mapCoordinates, Color.Orange);

        });    
    }

    public string GetMarkerText()
    {
        return _markerSet.name ?? "marker";
    }
}
