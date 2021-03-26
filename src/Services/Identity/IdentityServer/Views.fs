module Views

open ViewModels
open Giraffe.GiraffeViewEngine

let private dataToggle = attr "data-toggle"
let private dataTarget = attr "data-target"
let private ariaExpanded = attr "aria-expanded"

let masterPage pageTitle content userName  =
    html [] [
        head [] [
            meta [ _charset "utf-8" ]
            meta [ _httpEquiv "X-UA-Compatible"; _content "IE=edge" ]
            meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0" ]
            title [] [ str pageTitle ]
            link [ _rel "icon"; _type "image/x-icon"; _href "/favicon.ico" ]
            link [ _rel "shortcut icon"; _type "image/x-icon"; _href "/favicon.ico" ]
            link [ _rel "stylesheet"; _href "/lib/bootstrap/css/bootstrap.css" ]
            link [ _rel "stylesheet"; _href "/css/site.css" ]
        ]
        body [] [
            div [ _class "navbar navbar-inverse navbar-fixed-top" ] [
                div [ _class "container-fluid" ] [
                    div [ _class "navbar-header" ] [
                        button [ _type "button"
                                 _class "navbar-toggle collapsed"
                                 dataToggle "collapse"
                                 ariaExpanded "false"] [
                            span [ _class "sr-only" ] [ str "Toggle navigation" ]
                            span [ _class "icon-bar" ] [ ]
                            span [ _class "icon-bar" ] [ ]
                            span [ _class "icon-bar" ] [ ]
                        ]
                        a [ _href "/" ] [
                            span [ _class "navbar-brand" ] [
                                img [ _src "/icon.png"; _class "icon-banner" ]
                                str "IdentityServer4"
                            ]
                        ]
                    ]
                    
                    match userName with
                    | Some usr ->
                        ul [ _class "nav navbar-nav" ] [
                            li [ _class "dropdown" ] [
                                a [ _href "#"; _class "dropdown-toggle"; dataToggle "dropdown" ] [
                                    str usr
                                    b [ _class "caret" ] [ ]
                                ]
                                ul [ _class "dropdown-menu" ] [
                                    li [] [
                                        a [ _href "account/logout" ] []
                                    ]
                                ]
                            ]
                        ]
                    | None     -> ()
                ]
            ]
            
            div [ _class "container body-content" ] [
                main [] content
            ]
            
            script [ _src "/lib/jquery/jquery.js" ] [ ]
            script [ _src "/lib/bootstrap/js/bootstrap.js" ] [ ]
        ]
    ]
    
let errorView model =
    let error =
        match model.Error with
        | Some err ->
            strong [] [
                em [] [ str err.Error ]
            ]
        | None -> emptyText
        
    let requestId =
        match model.Error with
        | Some err ->
            div [ _class "request-id" ] [
                str $"Request Id: {err.RequestId}"
            ]
        | None -> emptyText
        
    [
        div [ _class "error-page" ] [
            div [ _class "page-header" ] [
                h1 [] [ str "Error" ]
            ]
            
            div [ _class "row" ] [
                div [ _class "col-sm-6" ] [
                    div [ _class "alert alert-danger" ] [
                        str "Sorry, there was an error "
                        error
                    ]
                    
                    requestId
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
                div [ _class "col-sm-8" ] [
                    p [] [
                        str "Click "
                        a [ _href "grants" ] [ str "here" ]
                        str " to manage your stored grants."
                    ]
                ]
            ]
        ]
    ] |> masterPage "Home"
    