using System;
using Artemis.Core.Modules;
using Artemis.Plugins.Mqtt.DataModels.Dynamic;

namespace Artemis.Plugins.Mqtt.DataModels;

public class RootDataModel : DataModel
{
    public MqttConnectorStatusCollection Statuses { get; } = new();

    /// <summary>
    ///     Handles an incoming message for a particular server and topic.
    /// </summary>
    internal void HandleMessage(Guid sourceServer, string topic, object data)
    {
        GetDynamicChild<DynamicDataModelBase>("DynamicData").Value.PropogateValue(sourceServer, topic, data);
    }

    /// <summary>
    ///     Removes and rebuilds the dynamically created DataModel.
    /// </summary>
    internal void UpdateDataModel(StructureDefinitionNode dataModelStructure)
    {
        // Remove existing
        RemoveDynamicChildByKey("DynamicData");

        // Build and add new structure
        var type = DynamicDataModelBuilder.Build(dataModelStructure);
        var inst = (DynamicDataModelBase)Activator.CreateInstance(type);
        AddDynamicChild("DynamicData", inst, "Data");
    }
}