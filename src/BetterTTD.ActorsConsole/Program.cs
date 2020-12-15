using System;
using Akka.Actor;

namespace BetterTTD.ActorsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("actor-system");

            Console.Read();
        }
    }
}