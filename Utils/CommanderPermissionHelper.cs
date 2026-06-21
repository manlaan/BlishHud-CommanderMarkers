using Blish_HUD;
using Manlaan.CommanderMarkers.Services;

namespace Manlaan.CommanderMarkers.Utils;

public static class CommanderPermissionHelper
{
    public static bool HasCommanderPermissions()
    {
        if (Service.LtMode.Value)
        {
            return true;
        }

        if (GameService.Gw2Mumble.IsAvailable && GameService.Gw2Mumble.PlayerCharacter.IsCommander)
        {
            return true;
        }

        return Service.RtApiEvents?.GrantsCommanderPermissions() == true;
    }

    public static bool RequiresCommanderGate()
    {
        return Service.Settings._settingOnlyWhenCommander.Value || Service.LtMode.Value;
    }

    public static bool PassesCommanderGate()
    {
        if (!RequiresCommanderGate())
        {
            return true;
        }

        return HasCommanderPermissions();
    }
}
