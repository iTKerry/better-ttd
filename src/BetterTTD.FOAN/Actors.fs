namespace BetterTTD.FOAN.Actors

module Actors =

    open Akka.FSharp
    
    type ProcessorMessage = ProcessJob of string
    
    let myActor (mailbox: Actor<_>) = 
        let rec loop () = actor {
            let! ProcessJob(message) = mailbox.Receive ()
            printfn $"{message}"
            return! loop ()
        }
        loop ()