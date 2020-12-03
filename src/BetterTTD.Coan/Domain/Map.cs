using BetterTTD.Coan.Enums;

namespace BetterTTD.Coan.Domain
{
    public class Map
    {
        public string Name { get; set; }
        public Landscape Landscape { get; set; }
        public GameDate StartDate { get; set; }
        public GameDate CurrentDate { get; set; }
        public long Seed { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}