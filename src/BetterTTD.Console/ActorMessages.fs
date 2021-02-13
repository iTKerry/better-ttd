module BetterTTD.Console.ActorMessagesModule

open System.Net
open BetterTTD.Console.PacketTransformers

type ReceiverMessage = Receive
    
type ConnectMessage = 
    { Address      : IPAddress
      Port         : int
      Password     : string }

type CoordinatorMessage =
    | Connect of ConnectMessage
    | ReceivedPacket of PacketMessage
