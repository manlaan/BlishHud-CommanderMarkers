using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Library.Enums;

public enum SquadMarker
{
    [Description("----")]
    None,
    [Description("Arrow")]
    Arrow,
    [Description("Circle")]
    Circle,
    [Description("Heart")]
    Heart,
    [Description("Square")]
    Square,
    [Description("Star")]
    Star,
    [Description("Spiral")]
    Spiral,
    [Description("Triangle")]
    Triangle,
    [Description("Cross")]
    Cross,
    [Description("Clear")]
    Clear
}
public static class SquadMarkerExtension
{
    public static Texture2D GetIcon(this SquadMarker marker)
    {
        return marker switch
        {
            SquadMarker.Arrow => Service.Textures!._imgArrow,
            SquadMarker.Circle => Service.Textures!._imgCircle,
            SquadMarker.Heart => Service.Textures!._imgHeart,
            SquadMarker.Square => Service.Textures!._imgSquare,
            SquadMarker.Star => Service.Textures!._imgStar,
            SquadMarker.Spiral => Service.Textures!._imgSpiral,
            SquadMarker.Triangle => Service.Textures!._imgTriangle,
            SquadMarker.Cross  => Service.Textures!._imgX,
            _ => Service.Textures!._blishHeart
        };
    }
}

public static class SquadMarkerExtensions
{ 
    public static string EnumValue(this SquadMarker e)
    {
        switch (e)
        {
            case SquadMarker.None:
                return "None";
            case SquadMarker.Arrow:
                return "Arrow";
            case SquadMarker.Circle:
                return "Circle";
            case SquadMarker.Heart:
                return "Heart";
            case SquadMarker.Square:
                return "Square";
            case SquadMarker.Star:
                return "STAR";
            case SquadMarker.Spiral:
                return "Sprial";
            case SquadMarker.Triangle:
                return "Triangle";
            case SquadMarker.Cross:
                return "Cross";
            case SquadMarker.Clear:
                return "Clear";

        }
        return "None";
    }
    public static SquadMarker EnumValue(this SquadMarker e, string readable)
    {
        switch (readable)
        {
            case "None":
                return SquadMarker.None;
            case "Arrow":
                return SquadMarker.Arrow;
            case "Circle":
                return SquadMarker.Circle;
            case "Heart":
                return SquadMarker.Heart;
            case "Square":
                return SquadMarker.Square;
            case "STAR":
                return SquadMarker.Star;
            case "Sprial":
                return SquadMarker.Spiral;
            case "Triangle":
                return SquadMarker.Triangle;
            case "Cross":
                return SquadMarker.Cross;
            case "Clear":
                return SquadMarker.Clear;

        }
        return SquadMarker.None;
    }


}