using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Library.Controls;


public class LibrayCornerIconMenuItem : ContextMenuStripItem
{
    public LibrayCornerIconMenuItem(SettingEntry<bool> setting, string displayLabel) : base(displayLabel)
    {
        Visible = setting.Value;
        Submenu = new ContextMenuStrip(() => GetLibraryForCurrentMap());
        
        setting.SettingChanged += delegate { Visible = setting.Value; };


        Click += LibrayCornerIconMenuItem_Click;

    }

    private void LibrayCornerIconMenuItem_Click(object sender, Blish_HUD.Input.MouseEventArgs e)
    {
        Service.SettingsWindow.ShowLibrary();
    }

    protected IEnumerable<ContextMenuStripItem> GetLibraryForCurrentMap()
    {
        var currentMap = Gw2MumbleService.Gw2Mumble.CurrentMap.Id;
        var libraryMarkers = Service.MarkersListing.GetMarkersForMap(currentMap);
        var menuListItems = new List<ContextMenuStripItem>();
        libraryMarkers.ForEach( marker =>
        {
            menuListItems.Add(new ContextMenuStripItem(marker.name));
        });

        return menuListItems;
    }

    protected override void DisposeControl()
    {
        Click -= LibrayCornerIconMenuItem_Click;
    }

}


