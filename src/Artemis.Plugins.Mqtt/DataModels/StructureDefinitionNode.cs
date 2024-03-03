using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Artemis.Plugins.Mqtt.DataModels;

/// <summary>
///     Class that defines a single node of the DataModel structure.
/// </summary>
public class StructureDefinitionNode
{
    public StructureDefinitionNode(string label, Guid? server, string topic, Type? type, bool isGroup)
    {
        Label = label;
        Server = server;
        Topic = topic;
        AssemblyQualifiedTypeName = type?.AssemblyQualifiedName ?? "";
        Children = isGroup ? new List<StructureDefinitionNode>() : null;
    }

    /// <summary>
    ///     The display name this node will appear as in the Artemis Data Model.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    ///     The server that will be used to subscribe to <see cref="Topic" />.
    /// </summary>
    public Guid? Server { get; set; }

    /// <summary>
    ///     The topic that can be used to set the value of this node.
    /// </summary>
    public string Topic { get; set; }
    
    /// <summary>
    ///     The type of value stored in this node.
    /// </summary>
    public string AssemblyQualifiedTypeName { get; set; }

    /// <summary>
    ///     The type of value stored in this node.
    /// </summary>
    [JsonIgnore]
    public Type? Type => Type.GetType(AssemblyQualifiedTypeName);
    
    /// <summary>
    ///     Whether this node is a group or not.
    /// </summary>
    public bool IsGroup => Children != null;

    /// <summary>
    ///     Any children this node has.
    /// </summary>
    public List<StructureDefinitionNode>? Children { get; set; }

    /// <summary>
    ///     Returns a default root <see cref="StructureDefinitionNode" />.
    /// </summary>
    public static StructureDefinitionNode RootDefault => new("Root", null, "", typeof(object), true);
}