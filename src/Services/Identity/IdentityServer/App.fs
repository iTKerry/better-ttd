module App

open System
open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging

let webApp : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [
        GET >=>
            choose [
                route "/" >=> htmlView Views.homeView
            ]
        setStatusCode 404 >=> text "Not Found"
    ]
    
let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message
