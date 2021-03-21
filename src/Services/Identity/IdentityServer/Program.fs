module Program

open System
open System.IO
open Db
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe

let webApp =
    choose [
        setStatusCode 404 >=> text "Not Found"
    ]
    
let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8080").AllowAnyMethod().AllowAnyHeader() |> ignore

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

let configureApp (app : IApplicationBuilder) =
    app.ApplicationServices
        .GetService<IdentityContext>()
        .EnsureSeedDataAsync()
        |> Async.AwaitTask
        |> Async.RunSynchronously
    app.UseCors(configureCors)
       .UseGiraffeErrorHandler(errorHandler)
       //.UseAuthentication()
       .UseGiraffe webApp



let configureServices (services : IServiceCollection) =
    services.AddDbContext<IdentityContext>(Db.cfg) |> ignore
    services.AddMvc() |> ignore
    //services
    //  .AddIdentityServer()
    //  .AddDeveloperSigningCredential() |> ignore
    
let configureLogging (builder : ILoggingBuilder) =
    let filter (l : LogLevel) = l.Equals LogLevel.Error
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