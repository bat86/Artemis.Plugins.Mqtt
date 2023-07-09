using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core.Modules;
using Artemis.Plugins.Mqtt.DataModels.Dynamic;

namespace Artemis.Plugins.Mqtt.DataModels;

public class MqttDataModel : DataModel
{
    public StatusesDataModel Statuses { get; } = new();

    public NodeDataModel Root { get; } = new();

    /// <summary>
    ///     Handles an incoming message for a particular server and topic.
    /// </summary>
    internal void HandleMessage(Guid sourceServer, string topic, object data)
    {
        Root.PropagateValue(sourceServer, topic, data);
    }

    /// <summary>
    ///     Removes and rebuilds the dynamically created DataModel.
    /// </summary>
    internal void UpdateDataModel(StructureDefinitionNode dataModelStructure)
    {
        Root.CreateStructure(dataModelStructure);
    }
}

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


public class NodeDataModel : DataModel
{
    private static readonly ConcurrentDictionary<string, DynamicChild> _allDynamicChildren = new();

    internal void CreateStructure(StructureDefinitionNode dataModelStructure)
    {
        ClearDynamicChildren();
        _allDynamicChildren.Clear();
        
        foreach (var childDefinition in dataModelStructure.Children)
        {
            var id = GetNodeId(childDefinition.Server ?? Guid.NewGuid(), childDefinition.Topic);
            DynamicChild dynamicChild;
            if (childDefinition.Type == null)
            {
                var folderNode = new NodeDataModel();
                folderNode.CreateStructure(childDefinition);
                dynamicChild = AddDynamicChild(id, folderNode, childDefinition.Label);
            }
            else
            {
                var type = childDefinition.Type;
                dynamicChild = type switch
                {
                    _ when type == typeof(bool) => AddDynamicChild<bool>(id, false, childDefinition.Label),
                    _ when type == typeof(int) => AddDynamicChild<int>(id, 0, childDefinition.Label),
                    _ when type == typeof(double) => AddDynamicChild<double>(id, 0, childDefinition.Label),
                    _ when type == typeof(string) => AddDynamicChild<string>(id, "", childDefinition.Label),
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported type")
                };
            }
            
            _allDynamicChildren.TryAdd(id, dynamicChild);
        }
    }

    private static string GetNodeId(Guid serverGuid, string topic)
    {
        return $"{serverGuid}_{topic}";
    }

    public void PropagateValue(Guid sourceServer, string topic, object data)
    {
        if (!_allDynamicChildren.TryGetValue(GetNodeId(sourceServer, topic), out var dynamicChild)) 
            return;
        
        switch (dynamicChild)
        {
            case DynamicChild<int> intChild:
                intChild.Value = (int)data;
                return;
            case DynamicChild<double> doubleChild:
                doubleChild.Value = (double)data;
                return;
            case DynamicChild<bool> boolChild:
                boolChild.Value = (bool)data;
                return;
            case DynamicChild<string> stringChild:
                stringChild.Value = (string)data;
                return;
        }
    }
}