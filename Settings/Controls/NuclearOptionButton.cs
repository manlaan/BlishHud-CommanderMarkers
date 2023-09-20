using Blish_HUD;
using Blish_HUD.Controls;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Input;
namespace Manlaan.CommanderMarkers.Settings.Controls;

public class NuclearOptionButton: StandardButton
{
    private bool _safetyKeysPressed = false;
    public NuclearOptionButton():base()
    {
        Enabled = false;
        GameService.Input.Keyboard.KeyPressed += SafetySwitch;
        GameService.Input.Keyboard.KeyReleased += SafetySwitch;
       
    }


    private void SafetySwitch(object sender, Blish_HUD.Input.KeyboardEventArgs e)
    {
        var KeysDown = GameService.Input.Keyboard.KeysDown;
        _safetyKeysPressed = 
            (KeysDown.Contains(Keys.LeftControl) || KeysDown.Contains(Keys.RightControl) )
            && 
            (KeysDown.Contains(Keys.LeftShift) || KeysDown.Contains(Keys.RightShift) )
            ;

        Enabled = _safetyKeysPressed;

    }

    protected override void DisposeControl()
    {
        GameService.Input.Keyboard.KeyPressed -= SafetySwitch;
        GameService.Input.Keyboard.KeyReleased -= SafetySwitch;
    }
    
}
