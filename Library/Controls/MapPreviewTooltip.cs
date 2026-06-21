using Blish_HUD.Common.UI.Views;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Manlaan.CommanderMarkers;
using Manlaan.CommanderMarkers.Library.Enums;
using Manlaan.CommanderMarkers.Library.Models;
using Manlaan.CommanderMarkers.Presets.Model;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Library.Controls;

public sealed class MapPreviewTooltipView : View, ITooltipView
{
    private const int PreviewSize = 768;

    private readonly MapPreviewTarget _target;
    private Container? _buildPanel;
    private Image? _previewImage;
    private FlowPanel? _legendPanel;
    private int _detailFetchGeneration;

    public MapPreviewTooltipView(MapPreviewTarget target)
    {
        _target = target;
    }

    protected override void Build(Container buildPanel)
    {
        if (_buildPanel != null)
        {
            return;
        }

        _buildPanel = buildPanel;

        var root = new FlowPanel
        {
            Parent = buildPanel,
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            ControlPadding = new Vector2(0, 4),
            WidthSizingMode = SizingMode.AutoSize,
            HeightSizingMode = SizingMode.AutoSize
        };

        if (!string.IsNullOrWhiteSpace(_target.Label))
        {
            _ = new Label
            {
                Parent = root,
                Text = _target.Label,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                ShowShadow = true
            };
        }

        var description = _target.Description?.Replace("\r\n", "\n").Replace("\r", "\n") ?? "";
        if (!string.IsNullOrWhiteSpace(description))
        {
            _ = new Label
            {
                Parent = root,
                Text = description,
                Width = PreviewSize,
                AutoSizeHeight = true,
                WrapText = true,
                ShowShadow = true
            };
        }

        _previewImage = new Image
        {
            Parent = root,
            Size = new Point(PreviewSize, PreviewSize),
            Visible = !string.IsNullOrEmpty(_target.CommunitySetId)
        };

        _legendPanel = new FlowPanel
        {
            Parent = root,
            Width = PreviewSize,
            //Height = 72,
            FlowDirection = ControlFlowDirection.SingleTopToBottom,
            ControlPadding = new Vector2(4, 2),
            Visible = false
        };

        ApplyPreviewTexture();
        RenderLegend(_target.Markers);
        RequestPreviewIfNeeded();
        RequestDetailIfNeeded();
    }

    private void RequestPreviewIfNeeded()
    {
        if (string.IsNullOrEmpty(_target.CommunitySetId))
        {
            return;
        }

        Service.PreviewImageCache.RequestPreview(_target.CommunitySetId, _target.PreviewLargeUrl, _ =>
        {
            GameThreadUtil.Enqueue(ApplyPreviewTexture);
        });
    }

    private void RequestDetailIfNeeded()
    {
        if (_target.Markers.Count > 0 || string.IsNullOrEmpty(_target.CommunitySetId))
        {
            return;
        }

        var setId = _target.CommunitySetId;
        var generation = ++_detailFetchGeneration;
        Task.Run(() =>
        {
            var fetched = Service.CommunityCatalog.FetchSetDetail(setId);
            if (fetched == null)
            {
                return;
            }

            GameThreadUtil.Enqueue(() =>
            {
                if (generation != _detailFetchGeneration || _legendPanel?.Parent == null)
                {
                    return;
                }

                RenderLegend(fetched.marks);
            });
        });
    }

    private void ApplyPreviewTexture()
    {
        if (_previewImage == null || string.IsNullOrEmpty(_target.CommunitySetId))
        {
            return;
        }

        var texture = Service.PreviewImageCache.GetPreviewTexture(_target.CommunitySetId);
        if (texture != null)
        {
            _previewImage.Texture = texture;
            _previewImage.Visible = true;
            InvalidateLayout();
        }
    }

    private void InvalidateLayout()
    {
        _buildPanel?.Invalidate();
    }

    private void RenderLegend(IEnumerable<MarkerCoord> markers)
    {
        if (_legendPanel == null)
        {
            return;
        }

        _legendPanel.Children.Clear();
        var entries = markers
            .Where(m => !string.IsNullOrWhiteSpace(m.name) && m.icon != (int)SquadMarker.Clear)
            .OrderBy(m => m.icon)
            .ToList();

        _legendPanel.Visible = entries.Count > 0;
        foreach (var mark in entries)
        {
            var row = new Panel
            {
                Parent = _legendPanel,
                Width = 140,
                Height = 22
            };

            _ = new Image
            {
                Parent = row,
                Size = new Point(18, 18),
                Location = new Point(2, 2),
                Texture = ((SquadMarker)mark.icon).GetIcon()
            };

            _ = new Label
            {
                Parent = row,
                Text = mark.name,
                Location = new Point(24, 3),
                AutoSizeWidth = true,
                TextColor = Color.White,
                ShowShadow = true
            };
        }

        InvalidateLayout();
    }
}

public static class MapPreviewTooltip
{
    public static void Apply(Control control, MapPreviewTarget target)
    {
        if (string.IsNullOrWhiteSpace(target.CommunitySetId) &&
            string.IsNullOrWhiteSpace(target.Label))
        {
            return;
        }

        if (control is DetailsButton detailsButton)
        {
            // ScrollingHighlight uses an immediate-mode shader (menuitem.fx) on the
            // DetailsButton that can leak GPU state into the SpriteScreen tooltip pass.
            detailsButton.HighlightType = DetailsHighlightType.LightHighlight;
        }

        control.BasicTooltipText = null;
        control.Tooltip = new Tooltip(new MapPreviewTooltipView(target));
    }
}
