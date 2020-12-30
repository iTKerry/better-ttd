namespace BetterTTD.FOAN.Actors

open BetterTTD.FOAN.Network.PacketModule

module AdminMessages =
    
    type AdminJoinMessage =
        { Password     : string
          AdminName    : string
          AdminVersion : string }
        
    type AdminMessage =
        | AdminJoin of AdminJoinMessage
        
module ActorMessages =
    
    type ReceiverMessage = ReceiveMsg
    
    type SenderMessage = Packet of Packet
    
    type IdleMessage =
        Connect of host : string *
                   pass : string *
                   port : int
    