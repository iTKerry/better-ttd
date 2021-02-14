module BetterTTD.Shell

open System
open System.Reactive.Linq
open BetterTTD.OpenTTDModule
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

let init =
    let login, loginCmd = Login.init ()
    { OpenTTD = None; Login = login }, Cmd.batch [ Cmd.map LoginMsg loginCmd ]

let update (msg : Msg) (model : Model) =
    match msg with
    | LoginMsg loginMsg ->
        let loginModel, loginCmd, extraMsg = Login.update loginMsg model.Login
        let newModel =
            match extraMsg with
            | Login.ExternalMsg.NoOp -> model
            | Login.ExternalMsg.Connected ottd ->
                ottd.OnChat.Publish |> Observable.subscribe (printfn "%A") |> ignore
                { model with OpenTTD = Some ottd }
        { newModel with Login = loginModel }, Cmd.map LoginMsg loginCmd
    | PollClient ->
        match model.OpenTTD with
        | Some ottd -> ottd.AskPollClient UInt32.MaxValue
        | None -> failwith "Invalid operation"
        model, Cmd.none

let view (model : Model) dispatch =
    match model.OpenTTD with
    | None -> (Login.view model.Login (LoginMsg >> dispatch))
    | Some _ ->
         Grid.create [
             Grid.children [
                 Button.create [
                     Button.onClick (fun _ -> dispatch <| PollClient)
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