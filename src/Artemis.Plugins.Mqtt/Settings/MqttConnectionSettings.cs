using System;
using Artemis.Core;

namespace Artemis.Plugins.Mqtt;

public sealed class MqttConnectionSettings : CorePropertyChanged
{
    private string _clientId = "Artemis";

    private string _displayName = "";
    private string _password = "";
    private int _serverPort = 1883;
    private string _serverUrl = "";
    private string _username = "";

    /// <summary>
    ///     Creates a new MQTT server connection entry with the default values.
    /// </summary>
    public MqttConnectionSettings()
    {
        ServerId = Guid.NewGuid();
    }

    // Note that private setter allows value to be set by JSON serializer.
    public Guid ServerId { get; set; }

    public string DisplayName
    {
        get => _displayName;
        set => SetAndNotify(ref _displayName, value);
    }

    public string ServerUrl
    {
        get => _serverUrl;
        set => SetAndNotify(ref _serverUrl, value);
    }

    public int ServerPort
    {
        get => _serverPort;
        set => SetAndNotify(ref _serverPort, value);
    }

    public string ClientId
    {
        get => _clientId;
        set => SetAndNotify(ref _clientId, value);
    }

    public string Username
    {
        get => _username;
        set => SetAndNotify(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetAndNotify(ref _password, value);
    }
}