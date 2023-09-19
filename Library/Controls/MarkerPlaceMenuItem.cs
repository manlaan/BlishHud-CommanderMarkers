using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Settings;
using Manlaan.CommanderMarkers.Presets.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Library.Controls;


public class MarkerPlaceMenuItem : ContextMenuStripItem
{
    private MarkerSet _markerSet;
    public MarkerPlaceMenuItem( MarkerSet markerSet) : base($"Place {markerSet.name}")
    {
        _markerSet = markerSet;
        BasicTooltipText = markerSet.description;

        Click += MarkerPlaceMenuItem_Click;

    }

    private void MarkerPlaceMenuItem_Click(object sender, Blish_HUD.Input.MouseEventArgs e)
    {
        Service.MapWatch.PlaceMarkers(_markerSet);
    }



    protected override void DisposeControl()
    {
        Click -= MarkerPlaceMenuItem_Click;
    }

}


