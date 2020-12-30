namespace BetterTTD.FOAN.Actors

open System
open System.Net.Sockets
open BetterTTD.FOAN.Network.Enums

module ActorsModule =

    open Akka.FSharp
    
    open BetterTTD.FOAN.Actors.MessagesModule
    open BetterTTD.FOAN.Actors.TransformModule
    open BetterTTD.FOAN.Network.PacketModule

    let (~%) (msg : AdminMessage) = msgToPacket msg
    
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
            
    
    let getSocket (host : string) (port : int) =
        let soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        soc.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true)
        soc.Connect(host, port)
        soc
        
    let adminCoordinator (mailbox : Actor<_>) =
        let rec loop (socket : Socket option) = actor {
            match! mailbox.Receive () with
            | Connect(host, pass, port) ->
                match socket with
                | Some soc ->      
                    failwithf "Invalid operation captured"
                    return! loop (Some soc)
                    
                | None ->
                    let soc = getSocket host port
                    let senderRef = spawn mailbox "sender" (sender soc)
                    let receiverRef = spawn mailbox "receiver" (receiver soc)
                    let x = % Connect(host, pass, port)
                    
                    mailbox.Context.System.Scheduler.ScheduleTellRepeatedly(
                        TimeSpan.FromMilliseconds (0.),
                        TimeSpan.FromMilliseconds (1.),
                        receiverRef,
                        ReceiveMsg)
                    
                    senderRef <! x
                    return! loop (Some soc)
            
            return! loop socket
        }
        loop None
    