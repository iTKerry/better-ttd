namespace Identity.API

module Models =

    [<CLIMutable>]
    type RegisterModel =
        {
            UserName : string
            Email    : string
            Password : string
        }

    [<CLIMutable>]
    type LoginModel =
        {
            UserName : string
            Password : string
        }
