using System;
using System.Collections.Concurrent;
using Artemis.Core.Modules;
using Artemis.Plugins.Mqtt.DataModels.Dynamic;

namespace Artemis.Plugins.Mqtt.DataModels;

public class RootDataModel : DataModel
{
    public MqttConnectorStatusCollection Statuses { get; } = new();

    public NodeDataModel Mqtt { get; } = new();

    /// <summary>
    ///     Handles an incoming message for a particular server and topic.
    /// </summary>
    internal void HandleMessage(Guid sourceServer, string topic, object data)
    {
        Mqtt.PropagateValue(sourceServer, topic, data);
    }

    /// <summary>
    ///     Removes and rebuilds the dynamically created DataModel.
    /// </summary>
    internal void UpdateDataModel(StructureDefinitionNode dataModelStructure)
    {
        ClearDynamicChildren();
        Mqtt.CreateStructure(dataModelStructure);
    }
}

public class NodeDataModel : DataModel
{
    private static ConcurrentDictionary<string, DynamicChild> _allDynamicChildren = new();

    internal void CreateStructure(StructureDefinitionNode dataModelStructure)
    {
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