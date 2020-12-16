using System.Net.Sockets;
using Akka.Actor;
using Akka.Event;

namespace BetterTTD.Actors
{
    public sealed class ClientActor : ReceiveActor
    {
        private Socket _socket;
        private IActorRef _sender;
        private readonly ILoggingAdapter _log;
        
        public ClientActor()
        {
            _log = Context.GetLogger();
            
            Receive<AdminConnectMessage>(ConnectMessageHandler);
            
            _log.Info("Initialized");
        }

        private void ConnectMessageHandler(AdminConnectMessage msg)
        {
            var (host, port, password) = msg;

            _log.Info($"Handled {nameof(AdminConnectMessage)}. Host:{host}; Port:{port}; Password:{password}");
            
            if (_socket?.Connected ?? false)
                return;

            _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            _socket.Connect(host, port);
            
            _ = Context.ActorOf(ReceiverActor.Props(_socket), nameof(ReceiverActor));
            _sender = Context.ActorOf(SenderActor.Props(_socket), nameof(SenderActor));
            
            _sender.Tell(new SendAdminJoinMessage(password, "BetterTTD", "1.0"));
        }
    }
}