module BetterTTD.Console.MessageTransformers

open BetterTTD.FOAN.Network.Enums
open BetterTTD.FOAN.Network.PacketModule

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
       