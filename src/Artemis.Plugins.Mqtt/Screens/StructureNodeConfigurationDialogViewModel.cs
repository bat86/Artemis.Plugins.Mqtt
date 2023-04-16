using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.UI.Shared;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace Artemis.Plugins.Mqtt.Screens;

/// <summary>
///     ViewModel for single node edit dialog.
/// </summary>
public class StructureNodeConfigurationDialogViewModel : DialogViewModelBase<StructureNodeConfigurationDialogResult>
{
    private static readonly Type[] supportedTypes = { typeof(string), typeof(bool), typeof(int), typeof(double) };
    private bool generateEvent;

    private string label;
    private Guid? server;
    private string topic;
    private Type type;

    public StructureNodeConfigurationDialogViewModel(PluginSettings settingsService, bool isGroup): this()
    {
        label = "";
        server = Guid.Empty;
        topic = isGroup ? null : "";
        type = isGroup ? null : supportedTypes[0];
        IsGroup = isGroup;
        ServerConnectionsSetting = settingsService.GetSetting<List<MqttConnectionSettings>>("ServerConnections");
    }

    public StructureNodeConfigurationDialogViewModel(PluginSettings settingsService, StructureNodeViewModel target) : this()
    {
        label = target.Label;
        server = target.Server;
        topic = target.Topic;
        type = target.Type;
        generateEvent = target.GenerateEvent;
        IsGroup = target.IsGroup;
        ServerConnectionsSetting = settingsService.GetSetting<List<MqttConnectionSettings>>("ServerConnections");
    }

    private StructureNodeConfigurationDialogViewModel()
    {
        Save = ReactiveCommand.Create(ExecuteSave);
        this.ValidationRule(vm => vm.Label, label => !string.IsNullOrWhiteSpace(label), "Label cannot be empty");
        this.ValidationRule(vm => vm.Server, server => server != null && server != Guid.Empty, "Server cannot be empty");
        this.ValidationRule(vm => vm.Topic, topic => !string.IsNullOrWhiteSpace(topic), "Topic cannot be empty");
        
        if (!IsGroup)
        {
            this.ValidationRule(vm => vm.Type, type => type != null, "Type cannot be empty");
        }
    }

    public ReactiveCommand<Unit, Unit> Save { get; }
    
    public ReactiveCommand<Unit, Unit> Cancel { get; }

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

    public bool IsGroup { get; }
    public bool IsValue => !IsGroup;

    public IEnumerable<Type> SupportedValueTypes => supportedTypes;
    public PluginSetting<List<MqttConnectionSettings>> ServerConnectionsSetting { get; }

    public void ExecuteSave()
    {
        if (!HasErrors)
            Close(new StructureNodeConfigurationDialogResult(Label, Server, Topic, Type, GenerateEvent));
    }
}

/// <summary>
///     POCO that contains the result of a successful MqttNodeConfiguration dialog.
/// </summary>
public record StructureNodeConfigurationDialogResult(string Label, Guid? Server, string Topic, Type Type, bool GenerateEvent);

// public class MqttNodeConfigurationViewModelValidator : AbstractValidator<StructureNodeConfigurationDialogViewModel>
// {
//     public MqttNodeConfigurationViewModelValidator()
//     {
//         RuleFor(m => m.Label).NotEmpty().WithMessage("Label cannot be blank");
//
//         When(m => !m.IsGroup, () => { RuleFor(m => m.Type).NotNull().WithMessage("Choose a type for this data model property"); });
//     }
// }