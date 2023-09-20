using Blish_HUD.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.CornerIcon;

public class CornerIconContextMenu : ContextMenuStrip
{
    public CornerIconContextMenu(Func<IEnumerable<ContextMenuStripItem>> getItemsDelegate) : base(getItemsDelegate)
    {

    }
    protected override void OnHidden(EventArgs e)
    {
        base.OnHidden(e);
        Service.MapWatch.RemovePreviewMarkerSet();
    }
}
