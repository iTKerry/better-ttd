using System.Collections.Generic;
using System.Linq;
using Akka.Util.Internal;
using BetterTTD.Actors.Abstractions;
using BetterTTD.App.BL;
using BetterTTD.App.UI.Main.Abstractions;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Enums;
using BetterTTD.Network;
using Splat;

namespace BetterTTD.App.UI.Main
{
    public class MainInteractor : IMainInteractor, IClientView
    {
        private readonly IClientBridge _bridge;
        private readonly Dictionary<int, string> _commands;
        private Protocol? _protocol;
        private Game? _game;

        public MainInteractor()
        {
            _commands = new Dictionary<int, string>();
            _bridge = Locator.Current.GetService<ClientSystem>().CreateClientBridge(this);
        }

        public void OnProtocol(Protocol protocol)
        {
            _protocol = protocol;
        }

        public void OnServerWelcome(Game game)
        {
            _game = game;
            
            _bridge.SetDefaultUpdateFrequency(_protocol);
            _bridge.PollAll(_protocol);
        }

        public void OnServerCmdNames(Dictionary<int, string> cmdNames)
        {
            cmdNames
                .Where(cmd => !_commands.Keys.Contains(cmd.Key))
                .ForEach(kv => _commands.Add(kv.Key, kv.Value));
        }

        public void OnServerConsole(string origin, string message)
        {
            throw new System.NotImplementedException();
        }

        public void OnServerClientInfo(Client client)
        {
            throw new System.NotImplementedException();
        }

        public void OnServerChat(NetworkAction action, DestType dest, long clientId, string message, long data)
        {
            throw new System.NotImplementedException();
        }

        public void OnServerClientUpdate(long clientId, int companyId, string name)
        {
            throw new System.NotImplementedException();
        }

        public void OnServerClientQuit(long clientId)
        {
            throw new System.NotImplementedException();
        }

        public void OnServerClientError(long clientId, NetworkErrorCode errorCode)
        {
            throw new System.NotImplementedException();
        }

        public void OnServerCompanyStats(int companyId, Dictionary<VehicleType, int> vehicles, Dictionary<VehicleType, int> stations)
        {
            throw new System.NotImplementedException();
        }

        public void OnServerCompanyRemove(int companyId, AdminCompanyRemoveReason removeReason)
        {
            throw new System.NotImplementedException();
        }
    }
}