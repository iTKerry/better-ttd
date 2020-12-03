using BetterTTD.Coan.Domain;
using static System.Console;

namespace BetterTTD.Coan.Pool
{
    public class CompanyPool : GenericPool<int, Company>
    {
        public CompanyPool()
        {
            var spectator = new Company(Company.COMPANY_SPECTATOR);
            if (!TryAdd(spectator))
            {
                WriteLine($"Not possible to add Company with ID: {Company.COMPANY_SPECTATOR} in {nameof(CompanyPool)}");
            }
        }   
    }
}