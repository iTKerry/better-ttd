namespace IdentityServer

module Models =

    open Microsoft.AspNetCore.Identity

    [<CLIMutable>]
    type RegisterModel =
        { UserName : string
          Email    : string
          Password : string }

    [<CLIMutable>]
    type LoginModel =
        { UserName : string
          Password : string }
    
    type ApplicationUser() =
        inherit IdentityUser()