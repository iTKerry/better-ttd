module BetterTTD.Shell

open BetterTTD.OpenTTDModule
open Elmish
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.Components.Hosts
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.DSL

type Model =
    { OpenTTD : OpenTTD option }

type Msg =
    | Msg

let init =
    { OpenTTD = None }, Cmd.none

let update msg model =
    match msg with
    | Msg -> model, Cmd.none

let view model dispatch =
    DockPanel.create [
        DockPanel.children [
            TextBlock.create [
                TextBlock.text "Temp"
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