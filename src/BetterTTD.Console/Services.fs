module BetterTTD.Console.ServicesModule

open BetterTTD.Console.ActorMessagesModule
open BetterTTD.Console.PacketTransformers

type IServerSubscriber =
    abstract OnPacketReceived : PacketMessage     -> unit

type IServerNotifier =
    abstract SendConnect      : ConnectMessage    -> unit
    abstract SendPollClient   : PollClientMessage -> unit