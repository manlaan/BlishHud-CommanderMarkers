using Blish_HUD.Controls.Extern;
using Blish_HUD.Input;
using Microsoft.Xna.Framework.Input;

namespace Manlaan.CommanderMarkers.Utils;

public static class InputHelper
{

    public static void DoHotKey(KeyBinding key)
    {
        if (key == null) return;
        if (key.ModifierKeys != ModifierKeys.None)
        {
            if (key.ModifierKeys.HasFlag(ModifierKeys.Alt))
                Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.MENU, true);
            if (key.ModifierKeys.HasFlag(ModifierKeys.Ctrl))
                Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.CONTROL, true);
            if (key.ModifierKeys.HasFlag(ModifierKeys.Shift))
                Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.SHIFT, true);
        }
        Blish_HUD.Controls.Intern.Keyboard.Press(ToVirtualKey(key.PrimaryKey), true);
        System.Threading.Thread.Sleep(50);
        Blish_HUD.Controls.Intern.Keyboard.Release(ToVirtualKey(key.PrimaryKey), true);
        if (key.ModifierKeys != ModifierKeys.None)
        {
            if (key.ModifierKeys.HasFlag(ModifierKeys.Shift))
                Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.SHIFT, true);
            if (key.ModifierKeys.HasFlag(ModifierKeys.Ctrl))
                Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.CONTROL, true);
            if (key.ModifierKeys.HasFlag(ModifierKeys.Alt))
                Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.MENU, true);
        }
    }
    private static VirtualKeyShort ToVirtualKey(Keys key)
    {
        try
        {
            return (VirtualKeyShort)key;
        }
        catch
        {
            return new VirtualKeyShort();
        }
    }
}
