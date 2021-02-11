namespace Temp

open System.IO
open System.Threading.Tasks
open Akka.FSharp
open Akka.Dispatch
open BetterTTD.FOAN.Network.PacketModule

module Program =
    
    let akkaTaskRunner (func : Task) =
        ActorTaskScheduler.RunTask
            ( fun () ->
                async {
                    do! func |> Async.AwaitTask
                } |> Async.StartAsTask :> Task
            )
    
    let receiver (stream : Stream) (mailbox : Actor<_>) =
        let rec loop() =
            actor {
                let! message = mailbox.Receive ()
                return! loop()
            }
            
        loop ()
    
    type SenderMsg =
        | PacketMsg of Packet
    
    let sender (stream : Stream) (mailbox : Actor<_>) =
        let rec loop() =
            actor {
                match! mailbox.Receive () with
                | PacketMsg pac -> stream.WriteAsync (pac.Buffer, 0, int pac.Size) |> akkaTaskRunner
                return! loop()
            }
            
        loop ()
            
    [<EntryPoint>]
    let main argv =
        0