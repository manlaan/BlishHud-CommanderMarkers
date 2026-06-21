using Blish_HUD;
using Blish_HUD.Controls;
using Manlaan.CommanderMarkers.Presets.Model;
using Manlaan.CommanderMarkers.RtApi;
using Microsoft.Xna.Framework;
using System;

namespace Manlaan.CommanderMarkers.Library.Controls;

public class PositionFields: Container
{
    public event EventHandler<WorldCoord>? WorldCoordChanged;

    private StandardButton _locBtn;
    private StandardButton? _importBtn;
    private Label _xPos;
    private Label _yPos;
    private Label _zPos;
    private WorldCoord _worldCoord;
    private readonly Func<int?>? _getRtApiSlotIndex;

    public PositionFields(WorldCoord? marker, Func<int?>? getRtApiSlotIndex = null): base()
    {
        _getRtApiSlotIndex = getRtApiSlotIndex;
        Size = _getRtApiSlotIndex != null ? new(510, 30) : new(400, 30);

        _worldCoord = marker ?? new WorldCoord();

        _locBtn = new StandardButton()
        {
            Parent = this,
            Text = "Set Location",
            BasicTooltipText = "Set the X, Y, Z location to where you are currently standing",
            Size = new Point(100, 30),
            Location = new Point(0, 0)
        };

        var coordX = _getRtApiSlotIndex != null ? 210 : 110;
        if (_getRtApiSlotIndex != null)
        {
            _importBtn = new StandardButton()
            {
                Parent = this,
                Text = "Import",
                Size = new Point(80, 30),
                Location = new Point(105, 0),
                Icon = Service.Textures!.IconImport,
                BasicTooltipText = "Import this marker's position from squad markers placed in-game.\nRequires the Real-Time API addon.",
                Enabled = Service.RtApiConnection?.IsActive == true,
            };
            _importBtn.Click += ImportBtn_Click;
            Service.RtApiConnection!.ConnectionStateChanged += OnRtApiConnectionStateChanged;
        }

        var xLbl = new Label()
        {
            Parent = this,
            Location = new Point(coordX, 0),
            Size = new Point(15, 30),
            Text ="X:"
        };
        _xPos = new Label()
        {
            Parent = this,
            Text = _worldCoord.x.ToString(),
            Size = new Point(85, 30),
            Location = new Point(coordX + 15, 0)
        };
        var yLbl = new Label()
        {
            Parent = this,
            Location = new Point(coordX + 100, 0),
            Size = new Point(15, 30),
            Text = "Y:"
        };
        _yPos = new Label()
        {
            Parent = this,
            Text = _worldCoord.y.ToString(),
            Size = new Point(85, 30),
            Location = new Point(coordX + 115, 0)
        };
        var zLbl = new Label()
        {
            Parent = this,
            Location = new Point(coordX + 200, 0),
            Size = new Point(15, 30),
            Text = "Z:"
        };
        _zPos = new Label()
        {
            Parent = this,
            Text = _worldCoord.z.ToString(),
            Size = new Point(85, 30),
            Location = new Point(coordX + 215, 0)
        };

        _locBtn.Click += _locBtn_Click;
    }

    private void OnRtApiConnectionStateChanged(object? sender, RtApiConnectionState state)
    {
        if (_importBtn == null)
        {
            return;
        }

        _importBtn.Enabled = state == RtApiConnectionState.Active;
    }

    private void ImportBtn_Click(object sender, Blish_HUD.Input.MouseEventArgs e)
    {
        var slotIndex = _getRtApiSlotIndex?.Invoke();
        if (!slotIndex.HasValue || Service.RtApiConnection == null)
        {
            return;
        }

        if (!Service.RtApiConnection.EnsureActive())
        {
            ScreenNotification.ShowNotification(
                "Real-Time API is not available.",
                ScreenNotification.NotificationType.Error,
                null,
                4);
            return;
        }

        if (!Service.RtApiConnection.TryGetSquadMarkerPosition(slotIndex.Value, out var position))
        {
            ScreenNotification.ShowNotification(
                "No squad marker is placed for this slot.",
                ScreenNotification.NotificationType.Error,
                null,
                4);
            return;
        }

        _worldCoord.x = position.X;
        _worldCoord.y = position.Y;
        _worldCoord.z = position.Z;
        ApplyWorldCoord(_worldCoord);
    }

    private void _locBtn_Click(object sender, Blish_HUD.Input.MouseEventArgs e)
    {
        var pos = Gw2MumbleService.Gw2Mumble.PlayerCharacter.Position;
        _worldCoord.x = pos.X;
        _worldCoord.y = pos.Y;
        _worldCoord.z = pos.Z;
        ApplyWorldCoord(_worldCoord);
    }

    private void ApplyWorldCoord(WorldCoord coord)
    {
        _xPos.Text = coord.x.ToString();
        _yPos.Text = coord.y.ToString();
        _zPos.Text = coord.z.ToString();
        WorldCoordChanged?.Invoke(this, coord);
    }

    protected override void DisposeControl()
    {
        _locBtn.Click -= _locBtn_Click;
        if (_importBtn != null)
        {
            _importBtn.Click -= ImportBtn_Click;
        }

        if (Service.RtApiConnection != null)
        {
            Service.RtApiConnection.ConnectionStateChanged -= OnRtApiConnectionStateChanged;
        }
    }
}
