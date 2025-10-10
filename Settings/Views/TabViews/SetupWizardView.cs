using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Manlaan.CommanderMarkers.Markers;
using Manlaan.CommanderMarkers.Settings.Services;
using Manlaan.CommanderMarkers.Utils;

namespace Manlaan.CommanderMarkers.Settings.Views.SubViews;

public class SetupWizardView : View
{
    protected SettingService _settings;
    protected override void Build(Container buildPanel)
    {
        _settings = Service.Settings;

        base.Build(buildPanel);

        var panel = new FlowPanel()
            .BeginFlow(buildPanel)
            
            .AddControl(new FlowPanel()
            {


            }, out var GeneralSettingsPanel)
            ;

        ((FlowPanel)GeneralSettingsPanel)
            .AddSetting(_settings.CornerIconTexture);

    }
}