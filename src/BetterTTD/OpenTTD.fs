module BetterTTD.OpenTTDModule

open System
open System.Net
open Akka.FSharp
open BetterTTD.CoordinatorModule
open BetterTTD.ActorMessagesModule
open BetterTTD.PacketTransformers
open BetterTTD.ServicesModule

type ServerInfo =
    { Address : IPAddress
      Port    : int }

type OpenTTD (serverInfo : ServerInfo) =
    let system =
        System.create "tempServName" <| Configuration.load ()
        
    let subscriber =
        { new IServerSubscriber with
            member __.OnPacketReceived pac =
                match pac with
                | ServerChat chat            -> printfn "chat %A" chat
                | ServerClientJoin client    -> printfn "join %A" client
                | ServerClientInfo client    -> printfn "info %A" client
                | ServerClientUpdate client  -> printfn "update %A" client
                | ServerClientError client   -> printfn "error %A" client
                | ServerClientQuit client    -> printfn "quit %A" client
                | _ -> printfn "other %A" pac }
    
    let notifier =
        let coordinatorRef = spawn system "coordinator" <| coordinator subscriber
        { new IServerNotifier with
            member __.SendConnect msg =
                coordinatorRef <! Connect msg
            member __.SendPollClient msg =
                coordinatorRef <! PollClient msg }
        
    member this.TellConnect (pass : string) =
        let msg = { Address = serverInfo.Address
                    Port = serverInfo.Port
                    Password = pass }
        notifier.SendConnect msg
        
    member this.AskPollClient (clientId : uint32) =
        let msg = { ClientID = clientId }
        notifier.SendPollClient msg
