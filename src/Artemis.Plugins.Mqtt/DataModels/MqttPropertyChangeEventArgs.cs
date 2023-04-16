using Artemis.Core;

namespace Artemis.Plugins.Mqtt.DataModels;

public class MqttPropertyChangeEventArgs<T> : DataModelEventArgs
{
    public MqttPropertyChangeEventArgs(string topic, T value)
    {
        Topic = topic;
        Value = value;
    }

    public string Topic { get; init; }
    public T Value { get; init; }
}