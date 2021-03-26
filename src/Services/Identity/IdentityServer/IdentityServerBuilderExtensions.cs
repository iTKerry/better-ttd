using IdentityServer.DataAccess;
using IdentityServer.Services;
using IdentityServer.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddIdentityUserStore(this IIdentityServerBuilder builder)
        {
            builder.Services
                .AddTransient<IIdentityUserRepository, IdentityUserRepository>()
                .AddDbContext<IdentityContext>(opt =>
                    opt.UseSqlServer("Server=localhost,8015;Database=BetterTTD;User=sa;Password=Your_password123;"));
            builder.AddProfileService<ProfileService>();
            return builder;
        }
    }
}
