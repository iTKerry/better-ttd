using System.Collections.Generic;
using System.Linq;
using Akka.Util.Internal;
using BetterTTD.Actors.ClientGroup;
using BetterTTD.Actors.ClientGroup.ReceiverGroup.DispatcherGroup;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Enums;
using BetterTTD.Network;
using ReactiveUI;

namespace BetterTTD.App.BL
{
    public class ClientSubscriber : IClientView
    {
        private readonly IClientCommander _commander;
        private readonly Dictionary<int, string> _commands;
        
        private Protocol _protocol;

        public ClientSubscriber(ClientSystem system)
        {
            _commands = new Dictionary<int, string>();
            _commander = system.CreateClientCommander(this);
        }
        
        public void OnProtocol(Protocol protocol)
        {
            _protocol = protocol;
        }

        public void OnServerWelcome(Game game)
        {
            _commander.SetDefaultUpdateFrequency(_protocol);
            _commander.PollAll(_protocol);
            
            MessageBus.Current.SendMessage(new OnServerWelcomeMessage(game));
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