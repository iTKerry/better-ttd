namespace IdentityServer

open IdentityServer.Models

module Config =
    
    open IdentityServer4.Models

    let identityResources : IdentityResource list =
        [ IdentityResources.OpenId()
          IdentityResources.Profile() ]
    
    let apiScopes : ApiScope list =
        [ ApiScope("api", "API") ]
        
    let clients : Client list =
        let apiClient = Client()
        apiClient.ClientId <- "apiClient"
        apiClient.ClientSecrets <- [| Secret("secret".Sha256()) |]
        apiClient.AllowedGrantTypes <- GrantTypes.ClientCredentials
        apiClient.AllowedScopes <- [| "api" |]
        [ apiClient ]
    

module Program =

    open System
    open System.IO

    open Giraffe

    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Cors.Infrastructure
    open Microsoft.AspNetCore.Hosting
    open Microsoft.AspNetCore.Http
    open Microsoft.Extensions.Hosting
    open Microsoft.Extensions.Logging
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.AspNetCore.Identity
    open Microsoft.AspNetCore.Identity.EntityFrameworkCore
    open Microsoft.EntityFrameworkCore
        
    let guiApp =
        choose [
            route "/login" >=>
                choose [
                    GET  >=> htmlView (Views.loginPage false)
                    POST >=> App.loginHandler
                ]
            route "/register" >=>
                choose [
                    GET  >=> htmlView Views.registerPage
                    POST >=> App.registerHandler
                ]
            GET >=>
                choose [
                    route "/"     >=> htmlView Views.indexPage
                    route "/user" >=> App.mustBeLoggedIn >=> App.userHandler
                ]
        ]
    
    let apiApp =
        subRoute "/api"
            (choose [
                route "/token" >=> POST >=> Api.tokenHandler
                route "/user"  >=> GET  >=> Api.authorize >=> text "some user info"
             ])
    
    let webApp =
        choose [
            guiApp
            apiApp
            setStatusCode 404 >=> text "Not Found"
        ]

    let errorHandler (ex : Exception) (logger : ILogger) =
        logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> setStatusCode 500 >=> text ex.Message

    let configureCors (builder : CorsPolicyBuilder) =
        builder.WithOrigins("http://localhost:5000").AllowAnyMethod().AllowAnyHeader() |> ignore

    let configureApp (app : IApplicationBuilder) =
        app.UseCors(configureCors)
           .UseGiraffeErrorHandler(errorHandler)
           .UseAuthentication()
           .UseIdentityServer()
           .UseRouting()
           .UseGiraffe webApp

    let configureServices (services : IServiceCollection) =
        services.AddDbContext<IdentityDbContext<IdentityUser>>(
            fun options ->
                options.UseInMemoryDatabase("NameOfDatabase") |> ignore
            ) |> ignore

        services
            .AddIdentityServer(
                fun options ->
                    options.Events.RaiseErrorEvents       <- true
                    options.Events.RaiseInformationEvents <- true
                    options.Events.RaiseFailureEvents     <- true
                    options.Events.RaiseSuccessEvents     <- true
                    options.EmitStaticAudienceClaim       <- true)
            .AddAspNetIdentity<ApplicationUser>()
            .AddInMemoryIdentityResources(Config.identityResources)
            .AddInMemoryApiScopes(Config.apiScopes)
            .AddInMemoryClients(Config.clients)
            .AddDeveloperSigningCredential()
            |> ignore
        
        services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityDbContext<ApplicationUser>>()
            .AddDefaultTokenProviders() |> ignore
        
        services.AddAuthentication() |> ignore
        
        services
            .ConfigureApplicationCookie(
            fun options ->
                options.ExpireTimeSpan <- TimeSpan.FromDays 150.0
                options.LoginPath      <- PathString "/login"
                options.LogoutPath     <- PathString "/logout"
            ) |> ignore

        services.AddCors() |> ignore

        services.AddGiraffe() |> ignore

    let configureLogging (builder : ILoggingBuilder) =
        let filter (l : LogLevel) = l.Equals LogLevel.Trace
        builder.AddFilter(filter).AddConsole().AddDebug() |> ignore

    [<EntryPoint>]
    let main _ =
        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(
                fun webHostBuilder ->
                    webHostBuilder
                        .UseKestrel()
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .Configure(configureApp)
                        .ConfigureServices(configureServices)
                        .ConfigureLogging(configureLogging)
                        |> ignore)
            .Build()
            .Run()
        0