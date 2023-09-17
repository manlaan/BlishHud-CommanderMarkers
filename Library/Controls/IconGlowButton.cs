using Blish_HUD.Controls;
using Blish_HUD.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Library.Controls;

public  class IconGlowButton: GlowButton
{

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
