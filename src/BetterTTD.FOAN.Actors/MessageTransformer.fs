namespace BetterTTD.FOAN.Actors

open System

open BetterTTD.FOAN.Network.Enums
open BetterTTD.FOAN.Actors.Messages
open BetterTTD.FOAN.Network.PacketModule

module MessageTransformer =
        
    let msgToPacket = function
        | AdminJoin { Password = pass; AdminName = name; AdminVersion = version } ->
            createPacketForType PacketType.ADMIN_PACKET_ADMIN_JOIN
            |> writeString pass
            |> writeString name
            |> writeString version
            
    let readServerProtocol packet =
        let (version, packet) = readByte packet
        let rec readFreq (dict : Map<AdminUpdateType, AdminUpdateFrequency>) pac =
            let (next, pac) = readBool pac
            if (next) then
                let (x, pac) = readU16 pac
                let updateType = enum<AdminUpdateType>(int x)
                let (x, pac) = readU16 pac
                let freq = enum<AdminUpdateFrequency>(int x)
                let newDict = dict.Add(updateType, freq)
                readFreq newDict pac
            else
                dict, pac
            
        let dict, _ = readFreq Map.empty packet
        AdminServerProtocol { Version = version; UpdateSettings = dict }
    
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
        | PacketType.ADMIN_PACKET_SERVER_WELCOME -> readServerWelcome pac
        | _ -> failwithf "PACKET TRANSFORMER ERROR."