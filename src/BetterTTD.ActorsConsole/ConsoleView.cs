using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Util.Internal;
using BetterTTD.Actors.ClientGroup;
using BetterTTD.Domain.Entities;
using BetterTTD.Domain.Enums;
using BetterTTD.Network;
using Newtonsoft.Json;

namespace BetterTTD.ActorsConsole
{
    public class ConsoleView : IClientView, IConnectorView
    {
        private readonly Dictionary<int, string> _commands;
        private readonly IClientCommander _commander;
        private readonly IClientConnector _connector;
        
        private Protocol _protocol;

        public ConsoleView(ClientSystem system)
        {
            _commands = new Dictionary<int, string>();
            _commander = system.CreateClientCommander(this);
            _connector = system.CreateClientConnector(this);
        }

        public void OnProtocol(Protocol protocol)
        {
            _protocol = protocol;
            Console.WriteLine($"{nameof(OnProtocol)}: version #{protocol.Version}");
        }

        public void OnServerWelcome(Game game)
        {
            _commander.SetDefaultUpdateFrequency(_protocol);
            _commander.PollAll(_protocol);
            
            var json = JsonConvert.SerializeObject(game, Formatting.Indented);
            Console.WriteLine($"{nameof(OnServerWelcome)}: {json}");
        }

        public void OnServerCmdNames(Dictionary<int, string> cmdNames)
        {
            cmdNames
                .Where(cmd => !_commands.Keys.Contains(cmd.Key))
                .ForEach(kv => _commands.Add(kv.Key, kv.Value));
            var json = JsonConvert.SerializeObject(cmdNames, Formatting.Indented);
            Console.WriteLine(json);
        }

        public void OnServerConsole(string origin, string message)
        {
            Console.WriteLine($"{nameof(OnServerConsole)}: origin - {origin}; message - {message}");
        }

        public void OnServerClientInfo(Client client)
        {
            var json = JsonConvert.SerializeObject(client, Formatting.Indented);
            Console.WriteLine($"{nameof(OnServerClientInfo)}: {json}");
        }

        public void OnServerChat(NetworkAction action, DestType dest, long clientId, string message, long data)
        {
            Console.WriteLine($"{nameof(OnServerChat)} | action:{action}; dest: {dest}; clientId: {clientId}; message: {message}; data: {data}");
        }

        public void OnServerClientUpdate(long clientId, int companyId, string name)
        {
            Console.WriteLine($"{nameof(OnServerClientUpdate)} | clientId: {clientId}; companyId: {companyId}; name: {name}");
        }

        public void OnServerClientQuit(long clientId)
        {
            Console.WriteLine($"{nameof(OnServerClientQuit)} | clientId: {clientId}");
        }

        public void OnServerClientError(long clientId, NetworkErrorCode errorCode)
        {
            Console.WriteLine($"{nameof(OnServerClientError)} | clientId: {clientId}; errorCode: {errorCode}");
        }

        public void OnServerCompanyStats(int companyId, Dictionary<VehicleType, int> vehicles, Dictionary<VehicleType, int> stations)
        {
            var vehiclesCount = vehicles.Values.Sum();
            var stationsCount = stations.Values.Sum();
            
            Console.WriteLine($"{nameof(OnServerCompanyStats)} | " +
                              $"companyId: {companyId}; " +
                              $"vehiclesCount: {vehiclesCount}; " +
                              $"stationsCount: {stationsCount}");
        }

        public void OnServerCompanyRemove(int companyId, AdminCompanyRemoveReason removeReason)
        {
            Console.WriteLine($"{nameof(OnServerClientError)} | companyId: {companyId}; removeReason: {removeReason}");
        }

        public void Connect(string host, int port, string pass)
        {
            _connector.Connect(host, port, pass);
        }
        
        public async Task ConnectResponse(bool connected, string? objError)
        {
            Console.WriteLine($"IsConnected:{connected}");
        }
    }
}