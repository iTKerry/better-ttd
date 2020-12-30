namespace BetterTTD.FOAN.Actors


open Akka.FSharp
open Akka.Actor

open System
open System.Net.Sockets

open BetterTTD.FOAN.Actors.ActorMessages
open BetterTTD.FOAN.Actors.AdminMessages
open BetterTTD.FOAN.Actors.MessageTransformModule
open BetterTTD.FOAN.Network.PacketModule
open BetterTTD.FOAN.Network.Enums

module ActorsModule =

    let sender (socket : Socket) (mailbox : Actor<_>) =
        let rec loop() = actor {
            match! mailbox.Receive () with
            | Packet { Buffer = buf; Position = pos } ->
                socket.Send (buf, pos, SocketFlags.None) |> ignore
            return! loop()
        }
        loop ()
    
    let receiver (socket : Socket) (mailbox : Actor<_>) =
        let rec loop () = actor {
            match! mailbox.Receive () with
            | ReceiveMsg ->
                if socket.Connected then
                    let pac = createPacket
                    socket.Receive pac.Buffer |> ignore
                    let (x, pac) = readByte pac
                    let pacType = enum<AdminUpdateType>(Convert.ToInt32 x)
                    
                    printfn "received %A" pacType
                else
                    ()
            
            return! loop ()
        }
        loop ()
            
    
    let adminCoordinator (mailbox : Actor<_>) =
        
        let rec connected (receiver : IActorRef) (sender : IActorRef) (socket : Socket) = actor {
            return! connected receiver sender socket
        }
        
        let rec connecting (receiver : IActorRef) (sender : IActorRef) (socket : Socket) = actor {
            return! connecting receiver sender socket
        }
        
        let rec idle () = actor {
            match! mailbox.Receive() with
            | Connect(host, pass, port) ->
                let soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                soc.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true)
                soc.Connect(host, port)
                
                let senderRef = spawn mailbox "sender" (sender soc)
                let receiverRef = spawn mailbox "receiver" (receiver soc)
                let pac = AdminJoin { Password = pass; AdminName = "BetterTTD"; AdminVersion = "1.0" } |> msgToPacket
                
                mailbox.Context.System.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromMilliseconds (0.),
                    TimeSpan.FromMilliseconds (1.),
                    receiverRef,
                    ReceiveMsg)
                
                senderRef <! pac
                return! connecting receiverRef senderRef soc
                
            return! idle ()
        }
        
        (*
        let rec errored (receiver : IActorRef) (sender : IActorRef) (socket : Socket) = actor {
            return! errored receiver sender socket
        }
        
        let rec fatal (receiver : IActorRef) (sender : IActorRef) (socket : Socket) = actor {
            return! fatal receiver sender socket
        }
        *)
    
        idle ()