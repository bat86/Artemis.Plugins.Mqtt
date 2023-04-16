using System.Reactive;
using Artemis.UI.Shared;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace Artemis.Plugins.Mqtt.Screens;

public class ServerConnectionDialogViewModel : DialogViewModelBase<DialogResult>
{
    private readonly MqttConnectionSettings _connectionSettings;
    private string _displayName;
    private string _serverUrl;
    private int _serverPort;
    private string _clientId;
    private string _username;
    private string _password;

    public ServerConnectionDialogViewModel(MqttConnectionSettings connectionSettings)
    {
        _connectionSettings = connectionSettings;
        _displayName = connectionSettings.DisplayName;
        _serverUrl = connectionSettings.ServerUrl;
        _serverPort = connectionSettings.ServerPort;
        _clientId = connectionSettings.ClientId;
        _username = connectionSettings.Username;
        _password = connectionSettings.Password;

        this.ValidationRule(vm => vm.DisplayName, s => !string.IsNullOrWhiteSpace(s), "Enter a name for this server");
        this.ValidationRule(vm => vm.ServerUrl, s => !string.IsNullOrWhiteSpace(s), "Enter a server URL");
        this.ValidationRule(vm => vm.ServerPort, port => port is > 0 and < 65536, "Enter a valid port number");
        this.ValidationRule(vm => vm.ClientId, s => !string.IsNullOrWhiteSpace(s), "Enter a client ID");
        
        Save = ReactiveCommand.Create(ExecuteSave, ValidationContext.Valid);
        Cancel = ReactiveCommand.Create(ExecuteCancel);
    }
    
    public ReactiveCommand<Unit, Unit> Save { get; }
    
    public ReactiveCommand<Unit, Unit> Cancel { get; }
    
    public string DisplayName
    {
        get => _displayName;
        set => RaiseAndSetIfChanged(ref _displayName, value);
    }
    
    public string ServerUrl
    {
        get => _serverUrl;
        set => RaiseAndSetIfChanged(ref _serverUrl, value);
    }
    
    public int ServerPort
    {
        get => _serverPort;
        set => RaiseAndSetIfChanged(ref _serverPort, value);
    }
    
    public string ClientId
    {
        get => _clientId;
        set => RaiseAndSetIfChanged(ref _clientId, value);
    }
    
    public string Username
    {
        get => _username;
        set => RaiseAndSetIfChanged(ref _username, value);
    }
    
    public string Password
    {
        get => _password;
        set => RaiseAndSetIfChanged(ref _password, value);
    }

    public void ExecuteSave()
    {
        if (HasErrors)
            return;
        
        _connectionSettings.DisplayName = DisplayName;
        _connectionSettings.ServerUrl = ServerUrl;
        _connectionSettings.ServerPort = ServerPort;
        _connectionSettings.ClientId = ClientId;
        
        Close(DialogResult.Save);
    }
    
    public void ExecuteCancel()
    {
        Close(DialogResult.Cancel);
    }

    public void ExecuteRemove()
    {
        Close(DialogResult.Remove);
    }
}

public enum DialogResult
{
    Save,
    Cancel,
    Remove
}