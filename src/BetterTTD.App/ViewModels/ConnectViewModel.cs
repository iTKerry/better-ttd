#nullable enable

using System.Reactive;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using BetterTTD.Actors.ClientGroup;
using BetterTTD.ActorsConsole;
using ReactiveUI;
using Splat;

namespace BetterTTD.App.ViewModels
{
    [DataContract]
    public class ConnectViewModel : ReactiveObject, IRoutableViewModel, IConnectorView
    {
        private ClientSystem? _system;
        private IClientConnector? _connector;
        
        public string UrlPathSegment => "/connect";
        
        public IScreen HostScreen { get; }
        
        public ConnectViewModel(IScreen? screen)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            var canConnect = this
                .WhenAnyValue(
                    x => x.Host,
                    x => x.Port,
                    x => x.Password,
                    (host, port, password) =>
                        !(string.IsNullOrWhiteSpace(host) ||
                        string.IsNullOrWhiteSpace(password) ||
                        port is 0));

            _connect = ReactiveCommand.CreateFromTask(TryConnect, canConnect);
        }

        private readonly ReactiveCommand<Unit, Unit> _connect;
        public ICommand ConnectCommand => _connect;

        [DataMember]
        private string? _host;
        public string Host
        {
            get => _host ?? string.Empty; 
            set => this.RaiseAndSetIfChanged(ref _host, value);
        }

        [DataMember]
        private int _port;
        public int Port
        {
            get => _port; 
            set => this.RaiseAndSetIfChanged(ref _port, value);
        }

        private string? _password;
        public string Password
        {
            get => _password ?? string.Empty; 
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }
        
        private async Task TryConnect() =>
            await Task.Run(() =>
            {
                _system = new ClientSystem("ottd-system");
                _connector = _system.CreateClientConnector(this);
                _connector.Connect(Host, Port, Password);
            });

        public void ConnectResponse(bool connected, string? error = null)
        {
            throw new System.NotImplementedException();
        }
    }
}