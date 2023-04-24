using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    public BasicMarker(MapData mapData, Vector3 trigger)
    {
        _mapData = mapData;
        _trigger = trigger;
    }

    public void DrawToMap(SpriteBatch spriteBatch, IMapBounds map)
    {

       
        var mapCoordinates = _mapData.WorldToScreenMap(_trigger);

        var r = new Rectangle((int)mapCoordinates.X - 10, (int)mapCoordinates.Y - 10, 20, 20);
        spriteBatch.DrawRectangleFill(r, new Color(0, 0, 0, 200));
    }

}
