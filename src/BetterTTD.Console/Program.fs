namespace Temp

open System
open System.IO
open System.Net
open System.Net.Sockets
open System.Threading.Tasks
open Akka.FSharp
open Akka.Dispatch
open BetterTTD.FOAN.Network.Enums
open BetterTTD.FOAN.Network.PacketModule

module Program =
    
    type AdminJoinMessage =
        { Password     : string
          AdminName    : string
          AdminVersion : string }
    
    type AdminMessage =
        | AdminJoin of AdminJoinMessage

    let msgToPacket = function
        | AdminJoin { Password = pass; AdminName = name; AdminVersion = version } ->
            createPacketForType PacketType.ADMIN_PACKET_ADMIN_JOIN
            |> writeString pass
            |> writeString name
            |> writeString version
            |> prepareToSend
            
    let akkaTaskRunner (func : Task) =
        ActorTaskScheduler.RunTask
            ( fun () ->
                async {
                    do! func |> Async.AwaitTask
                } |> Async.StartAsTask :> Task
            )
    
    let receiver (stream : Stream) (mailbox : Actor<_>) =
        let rec loop () =
            actor {
                let! message = mailbox.Receive ()
                return! loop ()
            }
            
        loop ()
    
    type SenderMsg =
        | PacketMsg of Packet
        
    type CoordinatorMgs =
        | ConnectMsg of AdminJoinMessage
    
    let sender (stream : Stream) (mailbox : Actor<_>) =
        let rec loop () =
            actor {
                match! mailbox.Receive () with
                | PacketMsg pac -> stream.Write (pac.Buffer, 0, int pac.Size)
                return! loop ()
            }
            
        loop ()
        
    let coordinator (mailbox : Actor<_>) =
        let rec connecting sender receiver =
            actor {
                match! mailbox.Receive () with
                | _ -> printfn "received something"
                return! connecting sender receiver
            }

        let rec idle () =
            actor {
                match! mailbox.Receive () with
                | ConnectMsg msg ->
                    let tcpClient = new TcpClient ()
                    tcpClient.Connect (IPAddress.Parse("127.0.0.1"), 3977)
                    let networkStream = tcpClient.GetStream ()
                                
                    let senderRef = spawn mailbox.Context.System "sender" <| (sender networkStream) 
                    let receiverRef = spawn mailbox.Context.System "receiver" <| (receiver networkStream)
                    
                    senderRef <! (prepareToSend <| msgToPacket (AdminJoin msg))
                    
                    return! connecting senderRef receiverRef
            }

        idle ()
            
        
    [<EntryPoint>]
    let main argv =
        let system = System.create "betterttd-system" <| Configuration.load ()
        let coordinatorRef = spawn system "coordinator" <| coordinator
        
        let joinMsg = ConnectMsg { Password = "p7gvv"; AdminName = "BetterTTD"; AdminVersion = "1.0" }
        coordinatorRef <! joinMsg
        
        Console.Read() |> ignore
        
        0