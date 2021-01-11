namespace BetterTTD.FOAN.Actors

module Receiver =

    open System.Net.Sockets

    open Akka.FSharp

    open BetterTTD.FOAN.Actors.Messages
    open BetterTTD.FOAN.Actors.Transformers.PacketTransformer
    open BetterTTD.FOAN.Network.PacketModule

    let private matchPacket = function
        | AdminServerProtocol protocol -> Connecting (Protocol protocol)
        | AdminServerWelcome  welcome  -> Connecting (Welcome  welcome )
    
    let receiver (socket : Socket) (mailbox : Actor<_>) =
        let rec loop () =
            actor {
                let! _ = mailbox.Receive ()
                
                if socket.Connected then
                    let pac = createPacket
                    // TODO: there is an error for socket.Receive on MacOS
                    socket.Receive pac.Buffer |> ignore
                    mailbox.Context.Parent <! (matchPacket <| packetToMsg pac)
                else
                    mailbox.Context.Parent <! (ErroredOut SocketConnectionClosed)

                return! loop ()
            }
        
        loop ()
    