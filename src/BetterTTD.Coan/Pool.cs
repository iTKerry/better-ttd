using BetterTTD.Coan.Pools;

namespace BetterTTD.Coan
{
    public class Pool
    {
        public ClientPool ClientPool { get; } = new();
        public CompanyPool CompanyPool { get; } = new();
    }
}