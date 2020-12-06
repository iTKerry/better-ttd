using System;
using BetterTTD.Coan;

namespace BetterTTD.ConsoleApp
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine("Welcome to BetterTTD");
            Console.WriteLine("New OpenTTD Admin tool!");

            var ottd = new OpenTTD {BotName = "BetterTTD Bot", BotVersion = "1.0.0"};
            var connected = ottd.Connect("127.0.0.1", 3977, "p7gvv");

            if (!connected)
                throw new InvalidOperationException("Not connected!");
            
            Console.Read();
        }
    }
}