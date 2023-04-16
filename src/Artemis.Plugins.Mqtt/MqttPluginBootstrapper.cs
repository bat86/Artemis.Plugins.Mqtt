using Artemis.Core;
using Artemis.Plugins.Mqtt.Screens;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Mqtt;

public class MqttPluginBootstrapper : PluginBootstrapper
{
    public override void OnPluginEnabled(Plugin plugin)
    {
        plugin.ConfigurationDialog = new PluginConfigurationDialog<MqttPluginConfigurationViewModel>();
    }
}