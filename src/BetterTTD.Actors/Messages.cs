using BetterTTD.Network;

namespace BetterTTD.Actors
{
    public record ReceivedBufMessage(Packet Packet);
    public record ReceiveBufMessage;
    
    public record SendAdminJoinMessage(
        string AdminPassword, 
        string BotName, 
        string BotVersion);
    
    public record AdminConnectMessage(
        string Host, 
        int Port, 
        string AdminPassword);

    public record OnProtocolMessage(Protocol Protocol);

    public record OnServerWelcomeMessage;
}