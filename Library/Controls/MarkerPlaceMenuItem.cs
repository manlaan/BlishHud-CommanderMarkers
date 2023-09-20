using Blish_HUD.Controls;
using Blish_HUD.Input;
using Manlaan.CommanderMarkers.Presets.Model;

namespace Manlaan.CommanderMarkers.Library.Controls;


public class MarkerPlaceMenuItem : ContextMenuStripItem
{
    private MarkerSet _markerSet;
    public MarkerPlaceMenuItem( MarkerSet markerSet) : base($"Place {markerSet.name}")
    {
        _markerSet = markerSet;
        BasicTooltipText = markerSet.description;

    }

    protected override void OnClick(MouseEventArgs e)
    {
        base.OnClick(e);
        Service.MapWatch.PlaceMarkers(_markerSet);
        Service.MapWatch.RemovePreviewMarkerSet();

    }

    protected override void OnMouseEntered(MouseEventArgs e)
    {
        base.OnMouseEntered(e);
        Service.MapWatch.PreviewMarkerSet(_markerSet);
    }
    protected override void OnMouseLeft(MouseEventArgs e)
    {
        base.OnMouseLeft(e);
        Service.MapWatch.RemovePreviewMarkerSet();
    }

}


