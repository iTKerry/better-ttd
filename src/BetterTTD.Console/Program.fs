namespace Temp

open System
open System.Net
open Akka.FSharp
open BetterTTD.Console.CoordinatorModule
open BetterTTD.Console.ActorMessagesModule
open BetterTTD.Console.PacketTransformers
open BetterTTD.Console.ServicesModule

module Program =
     
    let subscriber =
        { new IServerSubscriber with
            member __.OnPacketReceived pac =
                match pac with
                | ServerChat chat            -> printfn "chat %A" chat
                | ServerClientJoin client    -> printfn "join %A" client
                | ServerClientInfo client    -> printfn "info %A" client
                | ServerClientUpdate client  -> printfn "update%A" client
                | ServerClientError client   -> printfn "error %A" client
                | ServerClientQuit client    -> printfn "quit %A" client
                | _ -> printfn "other %A" pac }
    
    [<EntryPoint>]
    let main _ =
        let system = System.create "betterttd-system" <| Configuration.load ()
        let coordinatorRef = spawn system "coordinator" <| coordinator subscriber
        
        let joinMsg = Connect { Address = IPAddress.Parse ("127.0.0.1"); Port = 3977; Password = "p7gvv"; }
        coordinatorRef <! joinMsg
        
        Console.Read() |> ignore
        
        coordinatorRef <! PollClient
        
        Console.Read() |> ignore
        
        0