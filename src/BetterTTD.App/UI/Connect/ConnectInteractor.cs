#nullable enable

using System.Threading.Tasks;
using BetterTTD.Actors.ClientGroup;
using BetterTTD.App.BL;
using BetterTTD.App.UI.Connect.Abstractions;
using Splat;

namespace BetterTTD.App.UI.Connect
{
    public class ConnectInteractor : IConnectInteractor, IConnectorView
    {
        private readonly IConnectInteractorNotifier _notifier;
        
        private ClientSystem? _system;
        private IClientConnector? _connector;
        
        public ConnectInteractor(IConnectInteractorNotifier notifier)
        {
            _notifier = notifier;
        }

        public async Task ConnectAsync(string host, int port, string password) =>
            await Task.Run(() =>
            {
                _system = new ClientSystem("ottd-system");
                
                Locator.CurrentMutable.RegisterConstant(_system);
                
                _connector = _system.CreateClientConnector(this);
                _connector.Connect(host, port, password);
            });

        public async Task ConnectResponse(bool connected, string? error = null)
        {
            if (connected && _system != null)
            {
                _notifier.Connected();
            }
            else
            {
                await _system?.TerminateAsync()!;

                Locator.CurrentMutable.UnregisterAll<ClientSystem>();
                
                _notifier.ConnectionFailed(error ?? "Error occurred!");
            }
        }
    }
}