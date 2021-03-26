module Startup

open App
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Microsoft.Extensions.Logging

let private configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8080").AllowAnyMethod().AllowAnyHeader() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    let filter (l : LogLevel) =
        l.Equals LogLevel.Debug
    builder.AddFilter(filter)
           .AddConsole()
           .AddDebug() |> ignore

let configureServices (services : IServiceCollection) =
    services.AddDbContext<Db.IdentityContext>(Db.cfg) |> ignore
    services.AddMvc() |> ignore
    services
      .AddIdentityServer(fun opt ->
          opt.Events.RaiseErrorEvents <- true
          opt.Events.RaiseFailureEvents <- true
          opt.Events.RaiseInformationEvents <- true
          opt.Events.RaiseSuccessEvents <- true)
      .AddDeveloperSigningCredential()
      .AddProfileService<Services.ProfileService>()
      .AddInMemoryIdentityResources(Config.identityResources)
      .AddInMemoryApiResources(Config.apiResources)
      .AddInMemoryClients(Config.clients) |> ignore
    
let configureApp (app : IApplicationBuilder) =
    app.UseCors(configureCors)
       .UseGiraffeErrorHandler(errorHandler)
       .UseIdentityServer()
       .UseStaticFiles()
       .UseGiraffe webApp
