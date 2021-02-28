namespace IdentityServer

module App =

    open System
    open System.IO
    open System.Text

    open Giraffe

    open Microsoft.IdentityModel.Tokens
    open Microsoft.AspNetCore.Authentication.JwtBearer
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
           .UseGiraffe webApp

    let configureServices (services : IServiceCollection) =
        services.AddDbContext<IdentityDbContext<IdentityUser>>(
            fun options ->
                options.UseInMemoryDatabase("NameOfDatabase") |> ignore
            ) |> ignore

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(fun options ->
                options.TokenValidationParameters <- TokenValidationParameters(
                    ValidateActor = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "betterttd.net",
                    ValidAudience = "betterttd.net",
                    IssuerSigningKey = SymmetricSecurityKey(Encoding.UTF8.GetBytes("1ade4cd4-32a5-4e39-b57d-103b6d157744")))
            ) |> ignore
        
        services.AddIdentity<IdentityUser, IdentityRole>(
            fun options ->
                options.Password.RequireDigit   <- false
                options.Password.RequiredLength <- 5
                options.Password.RequireNonAlphanumeric <- false
                options.Password.RequireUppercase <- false
                options.Password.RequireLowercase <- false

                options.Lockout.DefaultLockoutTimeSpan  <- TimeSpan.FromMinutes 30.0
                options.Lockout.MaxFailedAccessAttempts <- 10

                options.User.RequireUniqueEmail <- true
            )
            .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
            .AddDefaultTokenProviders()
            |> ignore

        services.ConfigureApplicationCookie(
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