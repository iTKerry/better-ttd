namespace Temp

open System
open System.IO
open System.Net
open System.Net.Sockets
open Akka.FSharp
open BetterTTD.FOAN.Network.Enums
open BetterTTD.FOAN.Network.PacketModule
open FSharpx.Collections
    

module Program =
    
    type AdminServerProtocolMessage =
        { Version        : byte
          UpdateSettings : Map<AdminUpdateType, AdminUpdateFrequency []> }

    type AdminServerWelcomeMessage =
        { ServerName      : string 
          NetworkRevision : string 
          IsDedicated     : bool 
          MapName         : string 
          MapSeed         : uint32 
          Landscape       : Landscape 
          CurrentDate     : uint32 
          MapWidth        : int 
          MapHeight       : int }

    type PacketMessage =
        | AdminServerProtocol of AdminServerProtocolMessage
        | AdminServerWelcome  of AdminServerWelcomeMessage
        
    let readServerProtocol packet =
        let (version, packet) = readByte packet
        
        let rec readFreq (dict : Map<AdminUpdateType, AdminUpdateFrequency []>) pac =
            let (next, pac) = readBool pac
            if (next) then
                let (updIdx, pac) = readU16 pac
                let (freqIdx, pac) = readU16 pac
                let upd = enum<AdminUpdateType>(int updIdx)
                let newFrequencies =
                    Enum.GetValues<AdminUpdateFrequency>()
                    |> Array.filter (fun freq -> (int freqIdx &&& (int freq)) <> 0)
                    |> Array.map (fun freq -> (upd, freq))
                    |> Array.groupBy fst
                    |> Array.map (fun (key, items) ->
                        key, items |> Array.map snd |> Array.ofSeq)
                    |> Map.ofSeq
                let newDict = Map.union dict newFrequencies
                readFreq newDict pac
            else
                dict, pac
                
        let (dict, _) = readFreq Map.empty packet
        AdminServerProtocol
            { Version = version
              UpdateSettings = dict }
    
    let readServerWelcome packet =
        let (serverName, pac) = readString packet
        let (networkRevision, pac) = readString pac
        let (isDedicated, pac) = readBool pac
        let (mapName, pac) = readString pac
        let (mapSeed, pac) = readU32 pac
        let (x, pac) = readByte pac
        let landscape = enum<Landscape>(int x)
        let (currentDate, pac) = readU32 pac
        let (mapWidth, pac) = readU16 pac
        let (mapHeight, _) = readU16 pac
        AdminServerWelcome
            { ServerName = serverName
              NetworkRevision = networkRevision
              IsDedicated = isDedicated
              MapName = mapName
              MapSeed = mapSeed
              Landscape = landscape
              CurrentDate = currentDate
              MapWidth = int mapWidth
              MapHeight = int mapHeight }
    
    let packetToMsg packet =
        let (x, pac) = readByte packet
        match enum<PacketType>(int x) with
        | PacketType.ADMIN_PACKET_SERVER_PROTOCOL -> readServerProtocol pac
        | PacketType.ADMIN_PACKET_SERVER_WELCOME  -> readServerWelcome pac
        | _ -> failwithf "PACKET TRANSFORMER ERROR."
    
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
            
    let read (stream : Stream) (size : int) =
        let buf = Array.zeroCreate<byte> size
        
        let rec tRead (tStream : Stream) (tSize : int) =
            if tSize < size then
                let res = tStream.Read (buf, tSize, size - tSize)
                tRead tStream (tSize + res)
            else tSize
            
        tRead stream size |> ignore
        buf
    
    let createPacket (sizeBuf : byte array) (content : byte array) =
        let buf = Array.zeroCreate<byte> (2 + content.Length)
        buf.[0] <- sizeBuf.[0]
        buf.[1] <- sizeBuf.[1]
        for i in 0 .. (content.Length - 1) do
            buf.[i + 2] <- content.[i]
        { createPacket with Buffer = buf }
    
    let waitForPacket (stream : Stream) =
        let sizeBuf = read stream 2
        let size = BitConverter.ToUInt16 (sizeBuf, 0)
        let content = read stream (int size - 2)
        createPacket sizeBuf content
    
    type ReceiverMsg =
        | Receive
        
    let receiver (stream : Stream) (mailbox : Actor<_>) =
        let rec loop () =
            actor {
                match! mailbox.Receive () with
                | _ ->
                    let pac = waitForPacket stream
                    let msg = packetToMsg pac
                    printfn "%A" msg
                return! loop ()
            }
            
        loop ()
    
    type SenderMsg =
        | PacketMsg of Packet
        
    let sender (stream : Stream) (mailbox : Actor<_>) =
        let rec loop () =
            actor {
                match! mailbox.Receive () with
                | PacketMsg pac -> 
                    let { Buffer = buf; Size = size; } = prepareToSend pac
                    stream.Write (buf, 0, int size)
                return! loop ()
            }
            
        loop ()
        
    type CoordinatorMgs =
        | ConnectMsg of AdminJoinMessage
    
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
                                
                    let senderRef = spawn mailbox "sender" <| (sender networkStream) 
                    let receiverRef = spawn mailbox "receiver" <| (receiver networkStream)
                    
                    mailbox.Context.System.Scheduler.ScheduleTellRepeatedly(
                        TimeSpan.FromMilliseconds (0.),
                        TimeSpan.FromMilliseconds (100.),
                        receiverRef,
                        Receive)
                    
                    senderRef <! (PacketMsg <| msgToPacket (AdminJoin msg))
                    
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