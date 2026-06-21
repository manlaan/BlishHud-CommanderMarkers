using Blish_HUD;
using Gw2Sharp.WebApi.V2.Models;
using System;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Library.Services;

public class SubtokenService
{
    private string? _cachedSubtoken;
    private DateTime _subtokenExpiry = DateTime.MinValue;

    public event EventHandler? SubtokenRefreshed;

    public async Task<string?> GetValidSubtokenAsync()
    {
        if (!string.IsNullOrEmpty(_cachedSubtoken) && DateTime.UtcNow < _subtokenExpiry)
        {
            return _cachedSubtoken;
        }

        return await GenerateSubtokenAsync();
    }

    public async Task<string?> GenerateSubtokenAsync()
    {
        try
        {
            if (Service.Gw2ApiManager == null ||
                !Service.Gw2ApiManager.HasPermission(TokenPermission.Account))
            {
                return null;
            }

            var subtokenResponse = await Service.Gw2ApiManager.Gw2ApiClient.V2.CreateSubtoken
                .WithPermissions(new[] { TokenPermission.Account })
                .Expires(DateTime.UtcNow.AddDays(1))
                .GetAsync();

            _cachedSubtoken = subtokenResponse.Subtoken;
            _subtokenExpiry = DateTime.UtcNow.AddDays(1);
            SubtokenRefreshed?.Invoke(this, EventArgs.Empty);
            return _cachedSubtoken;
        }
        catch (Exception ex)
        {
            Logger.GetLogger<SubtokenService>().Warn(ex, "Failed to generate GW2 subtoken.");
            return null;
        }
    }

    public void ClearSubtoken()
    {
        _cachedSubtoken = null;
        _subtokenExpiry = DateTime.MinValue;
    }

    public async Task RefreshAccountNameAsync()
    {
        try
        {
            if (Service.Gw2ApiManager == null ||
                !Service.Gw2ApiManager.HasPermission(TokenPermission.Account))
            {
                Service.AccountDisplayName = null;
                return;
            }

            var account = await Service.Gw2ApiManager.Gw2ApiClient.V2.Account.GetAsync();
            Service.AccountDisplayName = account.Name;
            await GenerateSubtokenAsync();
        }
        catch (Exception ex)
        {
            Logger.GetLogger<SubtokenService>().Warn(ex, "Failed to refresh GW2 account name.");
        }
    }
}
