namespace Identity.API

module Handlers =

    open System
    open System.Text
    open FSharp.Control.Tasks.V2.ContextInsensitive
    
    open Identity.API.Models
    open Identity.API.Views
    
    open Microsoft.AspNetCore.Http
    open Microsoft.AspNetCore.Identity
    open Microsoft.Extensions.Logging
    
    open Giraffe

    let showErrors (errors : IdentityError seq) =
        errors
        |> Seq.fold (fun acc err ->
            sprintf "Code: %s, Description: %s" err.Code err.Description
            |> acc.AppendLine : StringBuilder) (StringBuilder(""))
        |> (fun x -> x.ToString())
        |> text

    let registerHandler : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! model       = ctx.BindFormAsync<RegisterModel>()
                let  user        = IdentityUser(UserName = model.UserName, Email = model.Email)
                let  userManager = ctx.GetService<UserManager<IdentityUser>>()
                let! result      = userManager.CreateAsync(user, model.Password)

                match result.Succeeded with
                | false -> return! showErrors result.Errors next ctx
                | true  ->
                    let signInManager = ctx.GetService<SignInManager<IdentityUser>>()
                    do! signInManager.SignInAsync(user, true)
                    return! redirectTo false "/user" next ctx
            }

    let loginHandler : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! model = ctx.BindFormAsync<LoginModel>()
                let signInManager = ctx.GetService<SignInManager<IdentityUser>>()
                let! result = signInManager.PasswordSignInAsync(model.UserName, model.Password, true, false)
                match result.Succeeded with
                | true  -> return! redirectTo false "/user" next ctx
                | false -> return! htmlView (loginPage true) next ctx
            }

    let userHandler : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let userManager = ctx.GetService<UserManager<IdentityUser>>()
                let! user = userManager.GetUserAsync ctx.User
                return! (user |> userPage |> htmlView) next ctx
            }

    let mustBeLoggedIn : HttpHandler =
        requiresAuthentication (redirectTo false "/login")

    let logoutHandler : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let signInManager = ctx.GetService<SignInManager<IdentityUser>>()
                do! signInManager.SignOutAsync()
                return! (redirectTo false "/") next ctx
            }
    
    let errorHandler (ex : Exception) (logger : ILogger) =
        logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> setStatusCode 500 >=> text ex.Message
