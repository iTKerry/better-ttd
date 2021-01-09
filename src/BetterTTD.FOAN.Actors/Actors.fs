namespace BetterTTD.FOAN.Actors

open Akka.FSharp
open Akka.Actor

open System
open System.Net.Sockets

open BetterTTD.FOAN.Actors.Messages
open BetterTTD.FOAN.Actors.MessageTransformer
open BetterTTD.FOAN.Network.PacketModule

module ActorsModule =

    let createSocket (host : string) (port : int) = 
        let soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        soc.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true)
        soc.Connect(host, port)
        soc

    let matchPacket = function
        | AdminServerProtocol protocol -> Connecting (Protocol protocol)
        | AdminServerWelcome welcome -> Connecting (Welcome welcome)
    
    let sender (socket : Socket) (mailbox : Actor<_>) =
        let rec loop() =
            actor {
                match! mailbox.Receive () with
                | Packet packet ->
                    let { Buffer = buf; Size = size; } = prepareToSend packet
                    socket.Send (buf, int size, SocketFlags.None) |> ignore
                    ()
                return! loop()
            }
        loop ()
    
    let receiver (socket : Socket) (mailbox : Actor<_>) =
        let rec loop () =
            actor {
                match! mailbox.Receive () with
                | ReceiveMsg ->
                    if socket.Connected then
                        let pac = createPacket
                        socket.Receive pac.Buffer |> ignore
                        mailbox.Context.Parent <! (matchPacket <| packetToMsg pac)
                    else
                        ()
                return! loop ()
            }
        loop ()
    
    let adminCoordinator (mailbox : Actor<_>) =
        
        let rec connected (receiver : IActorRef) (sender : IActorRef) (socket : Socket) =
            actor {
                return! connected receiver sender socket
            }
            
        and connecting (receiver : IActorRef) (sender : IActorRef) (socket : Socket) =
            actor {
                let matchConnecting = function
                | Protocol protocol ->
                    printfn "protocol received %A" protocol
                    connecting receiver sender socket
                | Welcome welcome ->
                    printfn "welcome received %A" welcome
                    connected receiver sender socket
                        
                match! mailbox.Receive () with
                | Connecting msg -> return! matchConnecting msg
                | _ -> failwithf "Invalid state operation occured"
                
                return! connecting receiver sender socket
            }
            
        and idle () =
            actor {
                let matchIdle = function
                | Connect(host, pass, port) ->
                    let soc = createSocket host port
                    let senderRef = spawn mailbox "sender" (sender soc)
                    let receiverRef = spawn mailbox "receiver" (receiver soc)
                    let msg = AdminJoin { Password = pass; AdminName = "BetterTTD"; AdminVersion = "1.0" }
                    let packet = msgToPacket msg
                    
                    mailbox.Context.System.Scheduler.ScheduleTellRepeatedly(
                        TimeSpan.FromMilliseconds (0.),
                        TimeSpan.FromMilliseconds (1.),
                        receiverRef,
                        ReceiveMsg)
                    
                    senderRef <! Packet packet
                    connecting receiverRef senderRef soc
                    
                match! mailbox.Receive () with
                | Idle msg -> return! matchIdle msg
                | _ -> failwithf "Invalid state operation occured"
                    
                return! idle ()
            }
        
        idle ()