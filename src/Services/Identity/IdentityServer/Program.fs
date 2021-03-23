module Program

open App
open System.IO
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Http
open Microsoft.EntityFrameworkCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8080").AllowAnyMethod().AllowAnyHeader() |> ignore

let migrate (ctx : Db.IdentityContext) =
    ctx.Database.Migrate()
    ctx.EnsureSeedDataAsync()
        |> Async.AwaitTask
        |> Async.RunSynchronously

let configureApp (app : IApplicationBuilder) =
    let cookiePolicy = CookiePolicyOptions()
    cookiePolicy.MinimumSameSitePolicy <- SameSiteMode.Lax
    
    migrate (app.ApplicationServices.GetService<Db.IdentityContext>())
    
    app.UseCors(configureCors)
       .UseGiraffeErrorHandler(errorHandler)
       .UseIdentityServer()
       .UseCookiePolicy(cookiePolicy)
       .UseStaticFiles()
       .UseGiraffe webApp

let configureServices (services : IServiceCollection) =
    services.AddDbContext<Db.IdentityContext>(Db.cfg) |> ignore
    services.AddMvc() |> ignore
    services
      .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
      .AddCookie() |> ignore
    services
      .AddIdentityServer()
      .AddDeveloperSigningCredential()
      .AddProfileService<Services.ProfileService>()
      .AddInMemoryIdentityResources(Config.identityResources)
      .AddInMemoryApiResources(Config.apiResources)
      .AddInMemoryClients(Config.clients) |> ignore
    
let configureLogging (builder : ILoggingBuilder) =
    let filter (l : LogLevel) = l.Equals LogLevel.Debug
    builder.AddFilter(filter).AddConsole().AddDebug() |> ignore

[<EntryPoint>]
let main _ =
    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .ConfigureServices(configureServices)
                    .Configure(configureApp)
                    .ConfigureLogging(configureLogging)
                    |> ignore)
        .Build()
        .Run()
    0