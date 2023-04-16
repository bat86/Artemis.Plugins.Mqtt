using Avalonia.ReactiveUI;

namespace Artemis.Plugins.Mqtt.Screens;

public partial class MqttPluginConfigurationView : ReactiveUserControl<MqttPluginConfigurationViewModel>
{
    public MqttPluginConfigurationView()
    {
        InitializeComponent();
    }
}