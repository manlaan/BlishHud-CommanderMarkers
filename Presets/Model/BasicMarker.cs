using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
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

    public void DrawToMap(SpriteBatch spriteBatch, IMapBounds map)
    {

       
        var mapCoordinates = _mapData.WorldToScreenMap(_trigger);

        var r = new Rectangle((int)mapCoordinates.X - 10, (int)mapCoordinates.Y - 10, 20, 20);
        spriteBatch.DrawRectangleFill(r, new Color(255, 0, 0, 128));
        spriteBatch.DrawCircle(mapCoordinates, 15, 12, Color.Black, 3f, 0); 
        
    }
    public string GetMarkerText()
    {
        return $"{_name}\n{_description}";
    }

}
