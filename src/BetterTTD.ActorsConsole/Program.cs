using System;
using Akka.Actor;
using BetterTTD.Actors;

namespace BetterTTD.ActorsConsole
{
    internal static class Program
    {
        private static void Main()
        {
            var system = ActorSystem.Create("ottd-system");
            var clientRef = system.ActorOf(Props.Create<NetworkClientActor>(), nameof(NetworkClientActor));
            
            clientRef.Tell(new AdminConnectMessage("127.0.0.1", 3977, "p7gvv"));
            
            Console.Read();
        }
    }
}