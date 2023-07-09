using System;
using System.Collections;
using System.Collections.Generic;

namespace Artemis.Plugins.Mqtt.DataModels;

/// <summary>
///     A specialized collection that holds the status of each active connector and allows them
///     to be presented to the data model.
/// </summary>
public class MqttConnectorStatusCollection : IEnumerable<MqttConnectorStatus>
{
    private readonly Dictionary<Guid, MqttConnectorStatus> _statuses = new();

    /// <summary>
    ///     Fetches the status object for a single MQTT server connector.
    /// </summary>
    internal MqttConnectorStatus this[Guid serverId] => _statuses[serverId];

    // IEnumerable methods for data model
    public IEnumerator<MqttConnectorStatus> GetEnumerator()
    {
        return _statuses.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    ///     Clears and re-creates the internal list of server connector statuses.
    /// </summary>
    /// <param name="serverList"></param>
    internal void UpdateConnectorList(List<MqttConnectionSettings> serverList)
    {
        _statuses.Clear();
        foreach (var server in serverList)
            _statuses.Add(server.ServerId, new MqttConnectorStatus(server.DisplayName));
    }
}