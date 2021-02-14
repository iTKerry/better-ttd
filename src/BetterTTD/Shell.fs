module BetterTTD.Shell

open System
open BetterTTD.OpenTTDModule
open BetterTTD.PacketTransformers
open Elmish
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.Components.Hosts
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.DSL

type Model =
    { OpenTTD : OpenTTD option
      Login   : Login.Model }

type Msg =
    | LoginMsg of Login.Msg
    | PollClient
    | ServerUpdate of PacketMessage

let init =
    let login, loginCmd = Login.init ()
    { OpenTTD = None; Login = login }, Cmd.batch [ Cmd.map LoginMsg loginCmd ]

let handleUpdate (model : Model) = function
    | ServerChat         chat    -> printfn "chat %A" chat; model
    | ServerClientJoin   client  -> printfn "join %A" client; model
    | ServerClientInfo   client  -> printfn "info %A" client; model
    | ServerClientUpdate client  -> printfn "update %A" client; model
    | ServerClientError  client  -> printfn "error %A" client; model
    | ServerClientQuit   client  -> printfn "quit %A" client; model
    | _ -> model

let subscribe (ottd : OpenTTD) =
    let sub dispatch =
        ottd.OnUpdate.Publish
        |> Observable.subscribe (fun pac -> dispatch (ServerUpdate pac))
        |> ignore
    Cmd.ofSub sub

let update (msg : Msg) (model : Model) =
    match msg with
    | LoginMsg loginMsg ->
        let loginModel, loginCmd, extraMsg = Login.update loginMsg model.Login
        let newModel, newCmd =
            match extraMsg with
            | Login.ExternalMsg.NoOp -> model, Cmd.none
            | Login.ExternalMsg.Connected (ottd, welcomeMsg) ->
                printfn "%A" welcomeMsg
                { model with OpenTTD = Some ottd },
                Cmd.batch [ subscribe ottd; Cmd.ofMsg PollClient ] 
        { newModel with Login = loginModel },
        Cmd.batch [ Cmd.map LoginMsg loginCmd; newCmd ]
    | PollClient ->
        match model.OpenTTD with
        | Some ottd -> ottd.AskPollClient UInt32.MaxValue
        | None -> failwith "Invalid operation"
        model, Cmd.none
    | ServerUpdate upd -> handleUpdate model upd, Cmd.none  

let view (model : Model) dispatch =
    match model.OpenTTD with
    | None -> (Login.view model.Login (LoginMsg >> dispatch))
    | Some _ ->
         Grid.create [
             Grid.children [
                 Button.create [
                     Button.onClick (fun _ -> dispatch PollClient)
                     Button.content "Poll"
                 ]
             ]
         ]
        
type MainWindow() as this =
    inherit HostWindow()
    do
        base.Title     <- "BetterTTD - FOAN"
        base.Width     <- 800.0
        base.Height    <- 600.0
        base.MinWidth  <- 800.0
        base.MinHeight <- 600.0

        Elmish.Program.mkProgram (fun () -> init) update view
        |> Program.withHost this
        |> Program.run