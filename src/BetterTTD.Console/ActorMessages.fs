module BetterTTD.Console.ActorMessagesModule

open System.Net
open BetterTTD.Console.PacketTransformers

type ReceiverMessage = Receive
    
type ConnectMessage = 
    { Address      : IPAddress
      Port         : int
      Password     : string }

type PollClientMessage =
    { ClientID : uint32 }

type CoordinatorMessage =
    | Connect of ConnectMessage
    | PacketReceived of PacketMessage
    | PollClient of PollClientMessage 
