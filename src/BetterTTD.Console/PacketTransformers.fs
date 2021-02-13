module BetterTTD.Console.PacketTransformers

open System
open BetterTTD.FOAN.Network.Enums
open BetterTTD.FOAN.Network.PacketModule
open FSharpx.Collections

type ServerChatMessage =
    { NetworkAction   : NetworkAction
      ChatDestination : ChatDestination
      ClientId        : uint32
      Message         : string
      Data            : uint64 }

type ServerProtocolMessage =
    { Version         : byte
      UpdateSettings  : Map<AdminUpdateType, AdminUpdateFrequency []> }

type ServerWelcomeMessage =
    { ServerName      : string 
      NetworkRevision : string 
      IsDedicated     : bool 
      MapName         : string 
      MapSeed         : uint32 
      Landscape       : Landscape 
      CurrentDate     : uint32 
      MapWidth        : int 
      MapHeight       : int }

type ServerClientJoinMessage =
    { ClientId        : uint32 }

type ServerClientQuitMessage =
    { ClientId        : uint32 }

type ServerClientErrorMessage =
    { ClientId        : uint32 }

type PacketMessage =
    | ServerProtocol     of ServerProtocolMessage
    | ServerWelcome      of ServerWelcomeMessage
    | ServerChat         of ServerChatMessage
    | ServerClientJoin   of ServerClientJoinMessage
    | ServerClientQuit   of ServerClientQuitMessage
    | ServerClientError  of ServerClientErrorMessage
    
    
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
    
    ServerProtocol
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
    
    ServerWelcome
        { ServerName = serverName
          NetworkRevision = networkRevision
          IsDedicated = isDedicated
          MapName = mapName
          MapSeed = mapSeed
          Landscape = landscape
          CurrentDate = currentDate
          MapWidth = int mapWidth
          MapHeight = int mapHeight }

let readServerChat packet =
    let (act, pac) = readByte packet
    let action = enum<NetworkAction>(int act)
    let (dest, pac) = readByte pac
    let destination = enum<ChatDestination>(int dest)
    
    let (clientId, pac) = readU32 pac
    let (message, pac) = readString pac
    let (data, _) = readU64 pac
    
    ServerChat
        { NetworkAction = action
          ChatDestination = destination
          ClientId = clientId
          Message = message
          Data = data }
    
let readServerClientJoin packet =
    let (clientId, _) = readU32 packet
    ServerClientJoin { ClientId = clientId }

let readServerClientQuit packet =
    let (clientId, _) = readU32 packet
    ServerClientQuit { ClientId = clientId }

let readServerClientError packet =
    let (clientId, _) = readU32 packet
    ServerClientError { ClientId = clientId }

let packetToMsg packet =
    let (typeVal, pac) = readByte packet
    match enum<PacketType>(int typeVal) with
    | PacketType.ADMIN_PACKET_SERVER_PROTOCOL      -> readServerProtocol    pac
    | PacketType.ADMIN_PACKET_SERVER_WELCOME       -> readServerWelcome     pac
    | PacketType.ADMIN_PACKET_SERVER_CHAT          -> readServerChat        pac
    | PacketType.ADMIN_PACKET_SERVER_CLIENT_JOIN   -> readServerClientJoin  pac
    | PacketType.ADMIN_PACKET_SERVER_CLIENT_INFO   -> failwith ""
    | PacketType.ADMIN_PACKET_SERVER_CLIENT_UPDATE -> failwith ""
    | PacketType.ADMIN_PACKET_SERVER_CLIENT_QUIT   -> readServerClientQuit  pac
    | PacketType.ADMIN_PACKET_SERVER_CLIENT_ERROR  -> readServerClientError pac
    | _ -> failwithf "PACKET TRANSFORMER ERROR: UNSUPPORTED TYPE - %d" typeVal
