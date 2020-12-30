namespace BetterTTD.FOAN.Actors


module ActorsModule =

    open Akka.FSharp
    open Akka.Actor

    open System
    open System.Net.Sockets

    open BetterTTD.FOAN.Actors.MessagesModule
    open BetterTTD.FOAN.Actors.TransformModule
    open BetterTTD.FOAN.Network.PacketModule
    open BetterTTD.FOAN.Network.Enums

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
        
        let rec notConnected () = actor {
            match! mailbox.Receive () with
            | Connect(host, pass, port) ->
                let soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                soc.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true)
                soc.Connect(host, port)
                
                let senderRef = spawn mailbox "sender" (sender soc)
                let receiverRef = spawn mailbox "receiver" (receiver soc)
                let pac = Connect (host, pass, port) |> msgToPacket
                
                mailbox.Context.System.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.FromMilliseconds (0.),
                    TimeSpan.FromMilliseconds (1.),
                    receiverRef,
                    ReceiveMsg)
                
                senderRef <! pac
                return! connected receiverRef senderRef soc
                
            return! notConnected ()
        }
        
        notConnected ()
    