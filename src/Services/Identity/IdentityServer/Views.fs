namespace IdentityServer

module Views =

    open Giraffe.GiraffeViewEngine
    open Microsoft.AspNetCore.Identity

    let masterPage (pageTitle : string) (content : XmlNode list) =
        html [] [
            head [] [
                title [] [ str pageTitle ]
                style [] [ rawText "label { display: inline-block; width: 80px; }" ]
            ]
            body [] [
                h1 [] [ str pageTitle ]
                main [] content
             ]
        ]

    let indexPage =
        [
            p [] [
                a [ _href "/register" ] [ str "Register" ]
            ]
            p [] [
                a [ _href "/user" ] [ str "User page" ]
            ]
        ] |> masterPage "Home"

    let registerPage =
        [
            form [ _action "/register"; _method "POST" ] [
                div [] [
                    label [] [ str "Email:" ]
                    input [ _name "Email"; _type "text" ]
                ]
                div [] [
                    label [] [ str "User name:" ]
                    input [ _name "UserName"; _type "text" ]
                ]
                div [] [
                    label [] [ str "Password:" ]
                    input [ _name "Password"; _type "password" ]
                ]
                input [ _type "submit" ]
            ]
        ] |> masterPage "Register"

    let loginPage (loginFailed : bool) =
        [
            if loginFailed then yield p [ _style "color: Red;" ] [ str "Login failed." ]

            yield form [ _action "/login"; _method "POST" ] [
                div [] [
                    label [] [ str "User name:" ]
                    input [ _name "UserName"; _type "text" ]
                ]
                div [] [
                    label [] [ str "Password:" ]
                    input [ _name "Password"; _type "password" ]
                ]
                input [ _type "submit" ]
            ]
            yield p [] [
                str "Don't have an account yet?"
                a [ _href "/register" ] [ str "Go to registration" ]
            ]
        ] |> masterPage "Login"

    let userPage (user : IdentityUser) =
        [
            p [] [
                sprintf "User name: %s, Email: %s" user.UserName user.Email
                |> str
            ]
        ] |> masterPage "User details"
