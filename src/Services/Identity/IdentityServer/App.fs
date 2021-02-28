namespace IdentityServer

module App =

    open System.Text
    open FSharp.Control.Tasks.V2.ContextInsensitive
    
    open IdentityServer.Models
    
    open Microsoft.AspNetCore.Http
    open Microsoft.AspNetCore.Identity

    open Giraffe

    let mustBeLoggedIn : HttpHandler =
        requiresAuthentication (redirectTo false "/login")

    let private showErrors (errors : IdentityError seq) =
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
                | false -> return! htmlView (Views.loginPage true) next ctx
            }
           
    let userHandler : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let userManager = ctx.GetService<UserManager<IdentityUser>>()
                printf "%A" ctx.User.Identity.IsAuthenticated
                let! user = userManager.GetUserAsync ctx.User
                return! (user |> Views.userPage |> htmlView) next ctx
            }

    let logoutHandler : HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let signInManager = ctx.GetService<SignInManager<IdentityUser>>()
                do! signInManager.SignOutAsync()
                return! (redirectTo false "/") next ctx
            }
    