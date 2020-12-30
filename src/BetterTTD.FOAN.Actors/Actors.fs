namespace BetterTTD.FOAN.Actors

module Actors =

    open Akka.FSharp
    
    let adminCoordinator (mailbox : Actor<_>) =
        let rec loop() = actor {
            let! message = mailbox.Receive ()
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
        let rec loop() = actor {
            let! message = mailbox.Receive ()
            // handle here
            return! loop()
        }
        loop ()
            