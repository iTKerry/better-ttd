﻿module App

open System
open Giraffe
open IdentityServer4.Extensions
open IdentityServer4.Services
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks.V2.ContextInsensitive
open Helpers

let htmlResult next ctx view =
    htmlView (Views.layout ctx view) next ctx

let homeHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        htmlResult next ctx Views.home

let homeErrorHandler : HttpHandler =
    fun next ctx ->
        task {
            let userName = ctx.User.GetDisplayName()
            let interaction = ctx.GetService<IIdentityServerInteractionService>()
            let env = ctx.GetService<IWebHostEnvironment>()
            
            let result = htmlResult next ctx 
            let getView msg = { Views.Error = msg } |> Views.errorView
            
            match ctx.GetQueryStringValue "errorId" with
            | Ok errorId ->
                let! msg = interaction.GetErrorContextAsync(errorId)
                match toOption msg with
                | Some msg ->
                    if env.IsDevelopment() then return! result (Some msg |> getView)
                    else msg.ErrorDescription <- null; return! result (Some msg |> getView)
                | None -> return! result (None |> getView)
            | Error _  -> return! result (None |> getView)
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
