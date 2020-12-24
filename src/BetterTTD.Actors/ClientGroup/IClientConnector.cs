#nullable enable
using System.Threading.Tasks;
using Akka.Actor;

namespace BetterTTD.Actors.ClientGroup
{
    public interface IClientConnector
    {
        void Connect(string host, int port, string password);
    }
    
    public class ClientConnector : IClientConnector
    {
        private readonly IActorRef _bridgeActor;

        public ClientConnector(IActorRef bridgeActor)
        {
            _bridgeActor = bridgeActor;
        }

        public void Connect(string host, int port, string password)
        {
            _bridgeActor.Tell(new AdminConnectMessage(host, port, password));
        }
    }

    public interface IConnectorView
    {
        Task ConnectResponse(bool connected, string? error = null);
    }
}