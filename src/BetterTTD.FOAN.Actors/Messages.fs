namespace BetterTTD.FOAN.Actors

open BetterTTD.FOAN.Network.PacketModule

module MessagesModule =
    
    type ReceiverMessage = ReceiveMsg
    
    type AdminMessage =
        | Connect of host : string *
                     pass : string *
                     port : int
                     
    type SenderMessage = Packet of Packet