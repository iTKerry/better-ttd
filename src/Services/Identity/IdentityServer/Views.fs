module Views

open ViewModels
open Giraffe.GiraffeViewEngine

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
    
let errorView (model : ErrorViewModel) =
    let error =
        if (box model.Error.Error = null) then None
        else Some model.Error.Error
    let requestId =
        if (box model.Error.RequestId = null) then None
        else Some model.Error.RequestId
    [
        div [ _class "error-page" ] [
            div [ _class "page-header" ] [
                h1 [] [ str "Error" ]
            ]
            
            div [ _class "row" ] [
                div [ _class "col-sm-6" ] [
                    div [ _class "alert alert-danger" ] [
                        str "Sorry, there was an error"
                        
                        match error with
                        | Some err ->
                            strong [] [
                                em [] [ str err ]
                            ]
                        | None -> ()
                    ]
                    
                    match requestId with
                    | Some rId ->
                        div [ _class "request-id" ] [
                            str $"Request Id: {rId}"
                        ]
                    | None -> ()
                ]
            ]
        ]
    ] |> masterPage "Error"
    
let homeView =
    [
        div [ _class "welcome-page" ] [
            div [ _class "row page-header" ] [
                div [ _class "col-sm-10" ] [
                    h1 [] [ str "Welcome to IdentityServer4" ]
                ]
            ]
            
            div [ _class "row" ] [
                div [ _class "col-sm-8" ] [
                    p [] [
                        str "IdentityServer publishes a "
                        a [ _href ".well-known/openid-configuration" ] [ str "discovery document" ]
                        str " where you can find metadata and links to all the endpoints, key material, etc."
                    ]
                ]
            ]
            
            div [ _class "row" ] [
                p [] [
                    str "Click "
                    a [ _href "grants" ] [ str "here" ]
                    str " to manage your stored grants."
                ]
            ]
        ]
    ] |> masterPage "Home"
    