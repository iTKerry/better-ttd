using System;
using BetterTTD.Actors;
using BetterTTD.Network;

namespace BetterTTD.ActorsConsole
{
    public class ConsoleView : IClientView
    {
        private readonly IClientBridge _bridge;

        public ConsoleView(ClientSystem system)
        {
            _bridge = system.CreateClientBridge(this);
        }

        public void Connect(string host, int port, string adminPassword)
        {
            _bridge.Connect(host, port, adminPassword);
        }
        
        public void OnProtocol(Protocol protocol)
        {
            Console.WriteLine($"{nameof(OnProtocol)}: version #{protocol.Version}");
        }

        public void OnServerWelcome()
        {
            Console.WriteLine($"{nameof(OnServerWelcome)}");
        }
    }
}