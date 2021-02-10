namespace Temp

open System.IO

module Program =
    
    open Akka.FSharp
    
    let receiver (stream : Stream) (mailbox : Actor<_>) =
        let rec loop() =
            actor {
                let! message = mailbox.Receive ()
                return! loop()
            }
            
        loop ()
    
    let sender (stream : Stream) (mailbox : Actor<_>) =
        let rec loop() =
            actor {
                let! message = mailbox.Receive ()
                return! loop()
            }
            
        loop ()
            
    [<EntryPoint>]
    let main argv =
        0