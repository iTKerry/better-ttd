using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityServer.DataAccess
{
    public class IdentityContextFactory : IDesignTimeDbContextFactory<IdentityContext>
    {
        public IdentityContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<IdentityContext>()
                .UseSqlServer("Server=localhost,8015;Database=BetterTTD;User=sa;Password=Your_password123;");
            
            return new IdentityContext(builder.Options);
        }
    }
}