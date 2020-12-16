using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Util.Internal;
using BetterTTD.Actors;
using BetterTTD.Domain.Entities;
using BetterTTD.Network;
using Newtonsoft.Json;

namespace BetterTTD.ActorsConsole
{
    public class ConsoleView : IClientView
    {
        private readonly Dictionary<int, string> _commands;
        private readonly IClientBridge _bridge;
        
        private Protocol _protocol;

        public ConsoleView(ClientSystem system)
        {
            _commands = new();
            _bridge = system.CreateClientBridge(this);
        }

        public void Connect(string host, int port, string adminPassword)
        {
            _bridge.Connect(host, port, adminPassword);
        }
        
        public void OnProtocol(Protocol protocol)
        {
            _protocol = protocol;
            Console.WriteLine($"{nameof(OnProtocol)}: version #{protocol.Version}");
        }

        public void OnServerWelcome(Game game)
        {
            _bridge.SetDefaultUpdateFrequency(_protocol);
            _bridge.PollAll(_protocol);
            var json = JsonConvert.SerializeObject(game, Formatting.Indented);
            Console.WriteLine($"{nameof(OnServerWelcome)}: {json}");
        }

        public void OnServerCmdNames(Dictionary<int, string> cmdNames)
        {
            cmdNames
                .Where(cmd => !_commands.Keys.Contains(cmd.Key))
                .ForEach(kv => _commands.Add(kv.Key, kv.Value));
        }
    }
}