using Blish_HUD;
using Blish_HUD.Controls;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Presets.Model;

public class BasicMarker : IMapEntity
{
    private MapData _mapData;
    private Vector3 _trigger;
    private string? _name;
    private string? _description;
    public BasicMarker(MapData mapData, Vector3 trigger, string? name, string? description)
    {
        _mapData = mapData;
        _trigger = trigger;
        _name = name;
        _description = description; 
    }
    public float DistanceFrom(Vector3 playerPosition)
    {
        return (playerPosition - _trigger).Length();
    }

    public void DrawToMap(SpriteBatch spriteBatch, IMapBounds map, Control control, Vector3 playerPosition)
    {
        //don't render when not within the same "floor" level
        var heightDelta = playerPosition.Z - _trigger.Z;
        if (Math.Abs(heightDelta) > 30) return;
        
        var mapCoordinates = _mapData.WorldToScreenMap(_trigger);
        var blishIcon = new Rectangle((int)mapCoordinates.X - 16, (int)mapCoordinates.Y - 16, 32, 32);
        spriteBatch.Draw(CommanderMarkers.Service.Textures!._blishHeart, blishIcon, Color.White);
        //spriteBatch.DrawOnCtrl(control, CommanderMarkers.Service.Textures!._blishHeart, blishIcon);

        //var r = new Rectangle((int)mapCoordinates.X - 10, (int)mapCoordinates.Y - 10, 20, 20);
        //spriteBatch.DrawRectangleFill(r, new Color(255, 0, 0, 128));
        //spriteBatch.DrawCircle(mapCoordinates, 15, 12, Color.Black, 3f, 0); 
        
    }
    public string GetMarkerText()
    {
        return $"{_name}\n{_description}";
    }

}
