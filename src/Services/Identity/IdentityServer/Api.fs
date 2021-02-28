namespace IdentityServer

module Api =

    open System
    open System.Text
    open System.IdentityModel.Tokens.Jwt
    open System.Security.Claims

    open FSharp.Control.Tasks.V2.ContextInsensitive
        
    open IdentityServer.Models
    
    open Microsoft.IdentityModel.JsonWebTokens
    open Microsoft.IdentityModel.Tokens
    open Microsoft.AspNetCore.Http
    open Microsoft.AspNetCore.Identity
    open Microsoft.AspNetCore.Authentication.JwtBearer

    open Giraffe

    let authorize : HttpHandler =
        requiresAuthentication (challenge JwtBearerDefaults.AuthenticationScheme)

    let private generateToken userName (tokenSecret : string) =
        let claims = [|
            Claim(JwtRegisteredClaimNames.Sub, userName);
            Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) |]

        let expires = Nullable(DateTime.UtcNow.AddHours(1.0))
        let notBefore = Nullable(DateTime.UtcNow)
        let securityKey = SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret))
        let signingCredentials = SigningCredentials(key = securityKey, algorithm = SecurityAlgorithms.HmacSha256)

        let token =
            JwtSecurityToken(
                issuer = "betterttd.net",
                audience = "betterttd.net",
                claims = claims,
                expires = expires,
                notBefore = notBefore,
                signingCredentials = signingCredentials)

        let tokenResult = JwtSecurityTokenHandler().WriteToken(token)
        tokenResult

    let tokenHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! model = ctx.BindJsonAsync<LoginModel>()
                let signInManager = ctx.GetService<SignInManager<IdentityUser>>()
                let! result = signInManager.PasswordSignInAsync(model.UserName, model.Password, true, false)
                match result.Succeeded with
                | true  ->
                    let tokenResult = generateToken model.UserName "1ade4cd4-32a5-4e39-b57d-103b6d157744"
                    return! json tokenResult next ctx
                | false -> return! setStatusCode 401 earlyReturn ctx
            }