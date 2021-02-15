module BetterTTD.Home

open System
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Layout
open BetterTTD.PacketTransformers
open Elmish

type Client =
    { ClientID : uint32 }
type ChatMessage =
    { ClientID : uint32 }

type Model =
    { Clients  : Client list
      Messages : ChatMessage list }

type ExternalMsg =
    | NoOp
    | PollClient   of uint32

type Msg =
    | ServerUpdate of PacketMessage
    | RefreshClients
    
let remove (clients : Client list) clientId =
    clients |> List.filter (fun x -> x.ClientID <> clientId)
    
let add (clients : Client list) (client : Client) =
    if clients |> List.exists (fun x -> x.ClientID = client.ClientID) then
        clients
    else 
        let newClient = { Client.ClientID = client.ClientID }
        clients @ [newClient]

let handleUpdate (model : Model) = function
    | ServerWelcome      welcome -> printfn "welcome %A" welcome; model
    | ServerChat         chat    -> printfn "chat %A" chat; model
    | ServerClientJoin   client  ->
        printfn "join %A" client
        let newClient = { Client.ClientID = client.ClientId }
        { model with Clients = add model.Clients newClient }
    | ServerClientInfo   client  ->
        printfn "info %A" client
        let newClient = { Client.ClientID = client.ClientId }
        { model with Clients = add model.Clients newClient }
    | ServerClientUpdate client  ->
        printfn "update %A" client
        let newClient = { Client.ClientID = client.ClientId }
        { model with Clients = add model.Clients newClient }
    | ServerClientError  client  ->
        printfn "error %A" client
        { model with Clients = remove model.Clients client.ClientId }
    | ServerClientQuit   client  ->
        printfn "quit %A" client
        { model with Clients = remove model.Clients client.ClientId }
    | _ -> model

let init () =
    { Clients  = List.empty
      Messages = List.empty },
    Cmd.none
    
let update msg model =
    match msg with
    | ServerUpdate upd ->
        handleUpdate model upd, Cmd.none,
        NoOp
    | RefreshClients ->
        model,
        Cmd.none,
        PollClient UInt32.MaxValue

let view (model : Model) dispatch =
    Grid.create [
         Grid.children [
             StackPanel.create [
                 StackPanel.verticalAlignment VerticalAlignment.Center
                 StackPanel.horizontalAlignment HorizontalAlignment.Center
                 StackPanel.children [
                     Button.create [
                         Button.onClick (fun _ -> dispatch RefreshClients)
                         Button.content "Refresh Clients"
                     ]
                     TextBlock.create [
                         TextBlock.text <| model.Clients.Length.ToString ()
                     ]
                 ]
             ]
         ]
     ]