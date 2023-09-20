using Blish_HUD;
using Blish_HUD.Controls;
using Manlaan.CommanderMarkers.Presets.Model;
using Microsoft.Xna.Framework;
using System;

namespace Manlaan.CommanderMarkers.Library.Controls;

public class PositionFields: Container
{
    public event EventHandler<WorldCoord>? WorldCoordChanged;

    private StandardButton _locBtn;
    private Label _xPos;
    private Label _yPos;
    private Label _zPos;
    private WorldCoord _worldCoord;
    public PositionFields(WorldCoord? marker): base()
    {

        Size = new(400, 30);

        _worldCoord = marker ?? new WorldCoord();

        _locBtn = new StandardButton()
        {
            Parent = this,
            Text = "Set Location",
            BasicTooltipText = "Set the X, Y, Z location to where you are currently standing",
            Size = new Point(100, 30),
            Location = new Point(0, 0)
        };
        var xLbl = new Label()
        {
            Parent = this,
            Location = new Point(110, 0),
            Size = new Point(15, 30),
            Text ="X:"
        };
        _xPos = new Label()
        {
            Parent = this,
            Text = _worldCoord.x.ToString(),
            Size = new Point(85, 30),
            Location = new Point(125, 0)
        };
        var yLbl = new Label()
        {
            Parent = this,
            Location = new Point(210, 0),
            Size = new Point(15, 30),
            Text = "Y:"
        };
        _yPos = new Label()
        {
            Parent = this,
            Text = _worldCoord.y.ToString(),
            Size = new Point(85, 30),
            Location = new Point(225, 0)
        };
        var zLbl = new Label()
        {
            Parent = this,
            Location = new Point(310, 0),
            Size = new Point(15, 30),
            Text = "Z:"
        };
        _zPos = new Label()
        {
            Parent = this,
            Text = _worldCoord.z.ToString(),
            Size = new Point(85, 30),
            Location = new Point(325, 0)
        };

        _locBtn.Click += _locBtn_Click;
        

    }

    private void _locBtn_Click(object sender, Blish_HUD.Input.MouseEventArgs e)
    {
        var pos = Gw2MumbleService.Gw2Mumble.PlayerCharacter.Position;
        _worldCoord.x = pos.X;
        _worldCoord.y = pos.Y;
        _worldCoord.z = pos.Z;
        _xPos.Text = _worldCoord.x.ToString();
        _yPos.Text = _worldCoord.y.ToString();
        _zPos.Text = _worldCoord.z.ToString();

        WorldCoordChanged?.Invoke(this, _worldCoord);
    }

 

    protected override void DisposeControl()
    {
        _locBtn.Click -= _locBtn_Click; 
    }
}
