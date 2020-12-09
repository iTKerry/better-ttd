#nullable enable
using System.Collections.Generic;

namespace BetterTTD.Domain.Entities
{
    public class DoCommandName
    {
        private DoCommandName(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public static void Create(string name, int value)
        {
            var command = new DoCommandName(name, value);
            
            Enumeration.Add(command);
            Lookup.Add(value, command);
        }

        public string Name { get; }
        public int Value { get; }

        public override string ToString()
        {
            return Name;
        }

        public static readonly List<DoCommandName> Enumeration = new();
        public static readonly Dictionary<int, DoCommandName> Lookup = new();

        public static DoCommandName? ValueOf(int index)
        {
            return Lookup.TryGetValue(index, out var result) 
                ? result 
                : null;
        }

        public static DoCommandName[] Values()
        {
            return Enumeration.ToArray();
        }
    }
}