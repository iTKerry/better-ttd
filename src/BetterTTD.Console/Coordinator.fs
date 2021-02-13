module BetterTTD.Console.CoordinatorModule

open System
open System.Net
open System.Net.Sockets
open Akka.FSharp
open BetterTTD.Console.MessageTransformers
open BetterTTD.Console.ReceiverModule
open BetterTTD.Console.SenderModule

type ConnectMessage = 
    { Address      : IPAddress
      Port         : int
      Password     : string }

type CoordinatorMgs =
    | Connect of ConnectMessage

let connectToStream (ipAddress : IPAddress) (port : int) =
    let tcpClient = new TcpClient ()
    tcpClient.Connect (ipAddress, port)
    tcpClient.GetStream ()
    
let scheduleMailbox (mailbox : Actor<_>) ref interval msg =
    mailbox.Context.System.Scheduler.ScheduleTellRepeatedly(
        TimeSpan.FromMilliseconds (0.),
        TimeSpan.FromMilliseconds (interval),
        ref,
        msg)

let coordinator (mailbox : Actor<_>) =
    let schedule = scheduleMailbox mailbox
    
    let rec connecting sender receiver =
        actor {
            match! mailbox.Receive () with
            | _ -> printfn "received something"
            return! connecting sender receiver
        }
        
    and idle () =
        actor {
            match! mailbox.Receive () with
            | Connect { Address = addr; Port = port; Password = pass} ->
                let networkStream = connectToStream addr port
                            
                let receiverRef = spawn mailbox "receiver" <| (receiver networkStream)
                schedule receiverRef 100.0 Receive
                
                let senderRef = spawn mailbox "sender" <| (sender networkStream)
                senderRef <! AdminJoin {Password = pass; AdminName = "BetterTTD"; AdminVersion = "1.0"}
                
                return! connecting senderRef receiverRef
        }

    idle ()
       