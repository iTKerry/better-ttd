using System;
using System.Text;
using Akka.Actor;
using BetterTTD.Actors.ClientBridgeGroup;
using BetterTTD.Actors.ClientGroup;

namespace BetterTTD.ActorsConsole
{
    internal static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var system = new ClientSystem("ottd-system");

            var view = new ConsoleView(system);
            view.Connect("127.0.0.1", 3977, "p7gvv");
            
            Console.Read();
        }
    }

    public sealed class ClientSystem
    {
        private readonly ActorSystem _actorSystem;
        private readonly IActorRef _clientActor;

        public ClientSystem(string systemName)
        {
            _actorSystem = ActorSystem.Create(systemName);
            _clientActor = _actorSystem.ActorOf(ClientActor.Props(), nameof(ClientActor));
        }

        public IClientCommander CreateClientCommander(IClientView view)
        {
            var bridgeActor = _actorSystem.ActorOf(ClientBridgeActor.Props(view, _clientActor), nameof(ClientBridgeActor));
            return new ClientCommander(bridgeActor);
        }

        public IClientConnector CreateClientConnector(IConnectorView view)
        {
            var connectorActor = _actorSystem.ActorOf(ConnectBridgeActor.Props(_clientActor, view), nameof(ConnectBridgeActor));
            return new ClientConnector(connectorActor);
        }
    }
}