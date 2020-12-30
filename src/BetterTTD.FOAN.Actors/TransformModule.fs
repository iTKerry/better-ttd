namespace BetterTTD.FOAN.Actors

module TransformModule =
        
    open System

    open BetterTTD.FOAN.Actors.MessagesModule
    open BetterTTD.FOAN.Network.Enums
    open BetterTTD.FOAN.Network.PacketModule

    let private (~%%) (pacType : PacketType) =
        Convert.ToByte pacType
    
    let msgToPacket = function
        | Connect(host, pass, port) ->
            let pac = createPacket
            let pac = writeByte (%% PacketType.ADMIN_PACKET_ADMIN_JOIN) pac
            pac
    