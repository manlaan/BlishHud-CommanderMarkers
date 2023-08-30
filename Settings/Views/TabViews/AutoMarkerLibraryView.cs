using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Settings.UI.Views;
using Manlaan.CommanderMarkers.Settings.Enums;
using Manlaan.CommanderMarkers.Settings.Services;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Manlaan.CommanderMarkers.Settings.Views.SubViews;

public class AutoMarkerLibraryView : View
{
    protected override void Build(Container buildPanel)
    {
        var _markers = Service.MarkersListing.GetAllMarkerSets();

        base.Build(buildPanel);
        var header = new Panel()
        {
            Parent = buildPanel,
            Size = new Point(buildPanel.Width, 30),
            Location = new Point(0, 0),
            ShowBorder= true,
        };
        new Label()
        {
            Text = "Auto Marker Library",
            Parent = header,
            Location = new Point(0, 0),
            AutoSizeWidth= true,
        };

        var panel = new FlowPanel()
            .BeginFlow(buildPanel, new(0), new(0, 30));

        panel.CanScroll= true;


        Texture2D editIcon = Service.Textures!._imgArrow;
        Point editSize =  new Point(editIcon.Width, editIcon.Height);
        int DetailButtonWidth = panel.Width - ((int)panel.OuterControlPadding.X*2)-10;
        
        _markers.ForEach(marker =>
        {
            var mapName = marker.mapId != null ? Service.MapDataCache.Describe((int)marker.mapId!) : "unknown map";

           // panel.AddString($"{marker.name} - {mapName}: {marker.marks?.Count} markers");

            var btn = new DetailsButton()
            {
                Text = $"{marker.name}\n{mapName}",
                //Icon = GameService.Content.GetRenderServiceTexture("0AAD072E707AE02AE1B9984FD8BCE1A113E759B7/2221432"),
                //MaxFill = 8,
                //CurrentFill = marker.marks?.Count ?? 0,
                //ShowFillFraction = true,
                //ShowToggleButton = true,
                //FillColor = Color.LightBlue,
                Width = DetailButtonWidth,
                IconSize = DetailsIconSize.Small,
                ShowToggleButton = true ,
            };

            var img = new Image()
            {
                Texture = editIcon,
                Size = editSize,
                Parent = btn
            };

            img.Click += (s, e) =>
            {
                Debug.WriteLine($"marker img click- set={marker.name}");
            };


            panel.AddFlowControl(btn);
        });

        
    
      

    }
}