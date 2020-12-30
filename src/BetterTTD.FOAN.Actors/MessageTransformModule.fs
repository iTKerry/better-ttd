namespace BetterTTD.FOAN.Actors

open System

open BetterTTD.FOAN.Actors.AdminMessages
open BetterTTD.FOAN.Network.Enums
open BetterTTD.FOAN.Network.PacketModule

module MessageTransformModule =
        
    let private (~%%) (pacType : PacketType) =
        Convert.ToByte pacType
    
    let msgToPacket = function
        | AdminJoin { Password = pass; AdminName = name; AdminVersion = version } ->
            createPacket
            |> writeByte (%% PacketType.ADMIN_PACKET_ADMIN_JOIN)
            |> writeString pass
            |> writeString name
            |> writeString version
    