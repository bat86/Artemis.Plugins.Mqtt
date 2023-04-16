namespace Artemis.Plugins.Mqtt.DataModels;

/// <summary>
///     Holds data about a single MQTT connector.
/// </summary>
public class MqttConnectorStatus
{
    public MqttConnectorStatus(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public bool IsConnected { get; internal set; }
}