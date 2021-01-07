﻿namespace BetterTTD.FOAN.Actors

open System

open BetterTTD.FOAN.Network.Enums
open BetterTTD.FOAN.Actors.Messages
open BetterTTD.FOAN.Network.PacketModule

module MessageTransform =
        
    let private (~%) (pacType : PacketType) =
        Convert.ToByte pacType
    
    let msgToPacket = function
        | AdminJoin { Password = pass; AdminName = name; AdminVersion = version } ->
            createPacketForType PacketType.ADMIN_PACKET_ADMIN_JOIN
            |> writeString pass
            |> writeString name
            |> writeString version
    