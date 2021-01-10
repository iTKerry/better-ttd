namespace BetterTTD.FOAN.Actors

module Receiver =

    open System.Net.Sockets

    open Akka.FSharp

    open BetterTTD.FOAN.Actors.Messages
    open BetterTTD.FOAN.Actors.Transformers.PacketTransformer
    open BetterTTD.FOAN.Network.PacketModule

    let private matchPacket = function
        | AdminServerProtocol protocol -> Connecting (Protocol protocol)
        | AdminServerWelcome welcome -> Connecting (Welcome welcome)
    
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
    