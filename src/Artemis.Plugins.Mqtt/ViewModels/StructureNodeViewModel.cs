using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Artemis.Plugins.Mqtt.DataModels.Dynamic;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;

namespace Artemis.Plugins.Mqtt.Screens;

/// <summary>
///     ViewModel representing a <see cref="DynamicStructureNode" /> model.
/// </summary>
public class StructureNodeViewModel : ViewModelBase
{
    private readonly IWindowService _windowService;
    private readonly StructureNodeViewModel parent;
    private bool generateEvent;

    private string label;
    private Guid? server;
    private string topic;
    private Type type;

    /// <summary>
    ///     Creates a new, blank ViewModel that represents a non-materialized <see cref="DynamicStructureNode" />.
    /// </summary>
    private StructureNodeViewModel(IWindowService windowService, StructureNodeViewModel parent)
    {
        this._windowService = windowService;
        this.parent = parent;
    }

    /// <summary>
    ///     Creates a new ViewModel that represents the given <see cref="DynamicStructureNode" />.
    /// </summary>
    public StructureNodeViewModel(IWindowService windowService, StructureNodeViewModel parent, StructureDefinitionNode model) : this(windowService,
        parent)
    {
        label = model.Label;
        server = model.Server;
        topic = model.Topic;
        type = model.Type;
        generateEvent = model.GenerateEvent;
        if (model.Children != null)
            Children = new ObservableCollection<StructureNodeViewModel>(
                model.Children.Select(c => new StructureNodeViewModel(windowService, this, c))
            );
    }

    /// <summary>
    ///     Converts this ViewModel into a <see cref="DynamicStructureNode" /> model that can be saved, and used
    ///     by the <see cref="DynamicClassBuilder" />.
    /// </summary>
    public StructureDefinitionNode ViewModelToModel()
    {
        return new StructureDefinitionNode()
        {
            Label = label,
            Server = server,
            Topic = topic,
            Type = type,
            GenerateEvent = generateEvent,
            Children = IsGroup ? new List<StructureDefinitionNode>(Children.Select(c => c.ViewModelToModel())) : null
        };
    }

    #region Properties

    public string Label
    {
        get => label;
        set => RaiseAndSetIfChanged(ref label, value);
    }

    public Guid? Server
    {
        get => server;
        set => RaiseAndSetIfChanged(ref server, value);
    }

    public string Topic
    {
        get => topic;
        set => RaiseAndSetIfChanged(ref topic, value);
    }

    public Type Type
    {
        get => type;
        set => RaiseAndSetIfChanged(ref type, value);
    }

    public bool GenerateEvent
    {
        get => generateEvent;
        set => RaiseAndSetIfChanged(ref generateEvent, value);
    }

    public ObservableCollection<StructureNodeViewModel> Children { get; init; }

    public bool IsGroup => Children != null;
    public bool IsValue => Children == null;

    #endregion

    #region Actions

    /// <summary>
    ///     Triggers a dialog to edit this ViewModel, and stores changes on confirm.
    /// </summary>
    public async Task EditNode()
    {
        var r = await _windowService.ShowDialogAsync<StructureNodeConfigurationDialogViewModel, StructureNodeConfigurationDialogResult>(this);
        Label = r.Label;
        Server = r.Server;
        Topic = r.Topic;
        Type = r.Type;
        generateEvent = r.GenerateEvent;
    }

    /// <summary>
    ///     Trigers a dialog that asks the user to confirm to delete this structural node.
    /// </summary>
    public async Task DeleteNode()
    {
        // If Children is null or does not have this child, throw an error
        if (parent.Children?.Contains(this) != true)
            throw new InvalidOperationException("This node does not support child or child does not exist in this node.");

        var result = await _windowService.ShowConfirmContentDialog(
            $"Delete {(IsGroup ? "Group" : "Value")}",
            "Are you sure you wish to delete this " + (IsGroup
                ? "group?" + Environment.NewLine + Environment.NewLine + $"This will also delete {Children.Count} child item(s)."
                : "value?"),
            "Delete",
            "Don't delete"
        );
        if (result)
            parent.Children.Remove(this);
    }

    /// <summary>
    ///     Attempts to add a new child node ViewModel to this node's children collection.
    /// </summary>
    /// <param name="isGroup">If <c>true</c>, adds a new group node. If <c>false</c>, adds a new value node.</param>
    /// <exception cref="InvalidOperationException">If this node is a value-type node that does not support children.</exception>
    public async Task AddChildNode(bool isGroup)
    {
        if (IsValue)
            throw new InvalidOperationException("Cannot add a child item to an item that does not support children.");

        var r = await _windowService.ShowDialogAsync<StructureNodeConfigurationDialogViewModel, StructureNodeConfigurationDialogResult>(isGroup);
        Children.Add(new StructureNodeViewModel(_windowService, this)
        {
            Label = r.Label,
            Server = isGroup ? null : r.Server,
            Topic = isGroup ? null : r.Topic,
            Type = isGroup ? null : r.Type,
            GenerateEvent = !isGroup && generateEvent,
            Children = isGroup ? new ObservableCollection<StructureNodeViewModel>() : null
        });
    }

    #endregion
}