namespace BetterTTD.FOAN.Actors

module ActorsModule =

    open Akka.Actor
    open Akka.FSharp
    
    type ReceiverMsg = ReceiveMsg
    
    let adminCoordinator (sender : IActorRef) (receiver : IActorRef) (mailbox : Actor<_>) =
        let rec loop() = actor {
            let! message = mailbox.Receive ()
            printfn "admin is working and doing well"
            // handle here
            return! loop()
        }
        loop ()
    
    let sender (mailbox : Actor<_>) =
        let rec loop() = actor {
            let! message = mailbox.Receive ()
            // handle here
            return! loop()
        }
        loop ()
    
    let receiver (mailbox : Actor<_>) =
        let rec loop () = actor {
            let! message = mailbox.Receive ()
            match message with
            | ReceiveMsg -> printfn "receiver receiving"
            
            return! loop ()
        }
        loop ()
            