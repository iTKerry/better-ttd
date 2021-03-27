module App

open System
open Giraffe
open IdentityServer4.Extensions
open IdentityServer4.Services
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks.V2.ContextInsensitive

let private getUserName (ctx : HttpContext) =
    let username = ctx.User.GetDisplayName ()
    if (box username = null) then None
    else Some username

let homeHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let view = getUserName ctx |> Views.homeView 
            return! htmlView view next ctx
        }

let homeErrorHandler : HttpHandler =
    fun next ctx ->
        task {
            let userName = getUserName ctx
            let interaction = ctx.GetService<IIdentityServerInteractionService>()
            match ctx.GetQueryStringValue "errorId" with
            | Ok errorId ->
                let! msg = interaction.GetErrorContextAsync(errorId)
                let errorView = Views.errorView { Error = Some msg } userName
                return! htmlView errorView next ctx
            | Error _ ->
                let errorView = Views.errorView { Error = None } userName
                return! htmlView errorView next ctx
        }

let webApp : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [
        GET >=>
            choose [
                route "/" >=> homeHandler
                route "/home/error" >=> homeErrorHandler
            ]
        setStatusCode 404 >=> text "Not Found"
    ]
    
let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message
