namespace Identity.API

module App =

    open System
    open System.IO
    
    open Giraffe

    open Identity.API.Views
    open Identity.API.Handlers
    
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
    
    let webApp =
        choose [
            GET >=>
                choose [
                    route "/"         >=> htmlView indexPage
                    route "/register" >=> htmlView registerPage
                    route "/login"    >=> htmlView (loginPage false)

                    route "/logout"   >=> mustBeLoggedIn >=> logoutHandler
                    route "/user"     >=> mustBeLoggedIn >=> userHandler
                ]
            POST >=>
                choose [
                    route "/register" >=> registerHandler
                    route "/login"    >=> loginHandler
                ]
            setStatusCode 404 >=> text "Not Found" ]

    let configureCors (builder : CorsPolicyBuilder) =
        builder.WithOrigins("http://localhost:8080").AllowAnyMethod().AllowAnyHeader() |> ignore

    let configureApp (app : IApplicationBuilder) =
        app.UseCors(configureCors)
           .UseGiraffeErrorHandler(errorHandler)
           .UseAuthentication()
           .UseGiraffe webApp

    let configureServices (services : IServiceCollection) =
        // Configure InMemory Db for sample application
        services.AddDbContext<IdentityDbContext<IdentityUser>>(
            fun options ->
                options.UseInMemoryDatabase("NameOfDatabase") |> ignore
            ) |> ignore

        // Register Identity Dependencies
        services.AddIdentity<IdentityUser, IdentityRole>(
            fun options ->
                // Password settings
                options.Password.RequireDigit   <- true
                options.Password.RequiredLength <- 8
                options.Password.RequireNonAlphanumeric <- false
                options.Password.RequireUppercase <- true
                options.Password.RequireLowercase <- false

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan  <- TimeSpan.FromMinutes 30.0
                options.Lockout.MaxFailedAccessAttempts <- 10

                // User settings
                options.User.RequireUniqueEmail <- true
            )
            .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>()
            .AddDefaultTokenProviders()
            |> ignore

        // Configure app cookie
        services.ConfigureApplicationCookie(
            fun options ->
                options.ExpireTimeSpan <- TimeSpan.FromDays 150.0
                options.LoginPath      <- PathString "/login"
                options.LogoutPath     <- PathString "/logout"
            ) |> ignore

        // Enable CORS
        services.AddCors() |> ignore

        // Configure Giraffe dependencies
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