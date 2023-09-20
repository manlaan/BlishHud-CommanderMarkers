using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Manlaan.CommanderMarkers.Library.Controls;

public  class IconButton: Image
{

    private bool _checked;
    public bool Checked
    {
        get
        {
            return _checked;
        }
        set
        {
            SetProperty(ref _checked, value, invalidateLayout: false, "Checked");
        }
    }

    public Texture2D Icon { get => Texture; set => Texture = value; }

    protected float _desiredOpacity = 1.0f;

    protected override void OnMouseEntered(MouseEventArgs e)
    {
        _desiredOpacity = Opacity;
        Opacity = 1.0f;

        base.OnMouseEntered(e);
    }

    protected override void OnMouseLeft(MouseEventArgs e)
    {
        Opacity = _desiredOpacity;

        base.OnMouseLeft(e);
    }
    protected override void OnClick(MouseEventArgs e)
    {

        _desiredOpacity = 1.0f;
        base.OnClick(e);
    }

}
