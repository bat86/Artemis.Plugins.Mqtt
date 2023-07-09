using System;
using System.Collections.Generic;
using Artemis.Core.Modules;

namespace Artemis.Plugins.Mqtt.DataModels;

public class StatusesDataModel : DataModel
{
    private readonly Dictionary<Guid, MqttConnectorStatus> _statuses;

    public StatusesDataModel()
    {
        _statuses = new();
    }

    public MqttConnectorStatus this[Guid serverId] => _statuses[serverId];
    
    internal void UpdateConnectorList(List<MqttConnectionSettings> serverList)
    {
        ClearDynamicChildren();
        _statuses.Clear();
        foreach (var server in serverList)
        {
            var status = new MqttConnectorStatus(server.DisplayName);
            AddDynamicChild(server.ServerId.ToString(), status, status.Name);
            _statuses.Add(server.ServerId, status);
        }
    }
}