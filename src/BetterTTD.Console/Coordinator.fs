module BetterTTD.Console.CoordinatorModule

open System
open System.Net
open System.Net.Sockets
open Akka.FSharp
open BetterTTD.Console.ActorMessagesModule
open BetterTTD.Console.MessageTransformers
open BetterTTD.Console.PacketTransformers
open BetterTTD.Console.ReceiverModule
open BetterTTD.Console.SenderModule

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

let coordinator (mailbox : Actor<CoordinatorMessage>) =
    let schedule = scheduleMailbox mailbox
    
    let rec connecting sender receiver =
        actor {
            match! mailbox.Receive () with
            | ReceivedPacket (AdminServerProtocol protocol) -> printfn "%A" protocol
            | ReceivedPacket (AdminServerWelcome welcome) -> printfn "%A" welcome
            | _ -> failwith "INVALID CONNECTING STATE CAPTURED"
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
            | _ -> failwith "INVALID IDLE STATE CAPTURED"
        }

    idle ()
       