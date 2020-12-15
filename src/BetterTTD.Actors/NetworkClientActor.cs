using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Akka.Actor;
using Akka.Event;
using BetterTTD.Domain.Enums;
using BetterTTD.Network;
using CSharpFunctionalExtensions;

namespace BetterTTD.Actors
{
    public sealed class NetworkClientCoordinatorActor : ReceiveActor
    {
        private readonly Dictionary<Socket, IActorRef> _clients;
        
        public NetworkClientCoordinatorActor()
        {
            _clients = new();
        }
    }

    public sealed class NetworkClientActor : ReceiveActor
    {
        private Socket _socket;
        private IActorRef _sender;
        private IActorRef _receiver;
        private readonly ILoggingAdapter _log;
        
        public NetworkClientActor()
        {
            _log = Context.GetLogger();
            
            Receive<AdminConnectMessage>(ConnectMessageHandler);
            
            _log.Info($"Initialized {nameof(NetworkClientActor)}");
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
            
            _receiver = Context.ActorOf(NetworkReceiverActor.Props(_socket), nameof(NetworkReceiverActor));
            _sender = Context.ActorOf(NetworkSenderActor.Props(_socket), nameof(NetworkSenderActor));
            
            _sender.Tell(new SendAdminJoinMessage(password, "BetterTTD", "1.0"));
        }
    }

    public sealed class NetworkSenderActor : ReceiveActor
    {
        private readonly Socket _socket;
        private readonly ILoggingAdapter _log;
        
        public NetworkSenderActor(Socket socket)
        {
            _socket = socket;
            _log = Context.GetLogger();

            Receive<SendAdminJoinMessage>(SendAdminJoinMessageHandler);
            
            _log.Info($"Initialized {nameof(NetworkSenderActor)}");
        }

        public static Props Props(Socket socket)
        {
            return Akka.Actor.Props.Create(() => new NetworkSenderActor(socket));
        }

        private void SendAdminJoinMessageHandler(SendAdminJoinMessage msg)
        {
            var (adminPassword, botName, botVersion) = msg;
            
            _log.Info($"Handled {nameof(SendAdminJoinMessage)}. " +
                      $"Pass:{adminPassword}; " +
                      $"BotName:{botName}; " +
                      $"BotVersion:{botVersion}");
            
            var packet = new Packet(_socket, PacketType.ADMIN_PACKET_ADMIN_JOIN);
            
            packet.WriteString(adminPassword);
            packet.WriteString(botName);
            packet.WriteString(botVersion);

            packet.SendTo(_socket);
        }
    }

    public class NetworkReceiverActor : ReceiveActor, IWithTimers
    {
        private readonly Socket _socket;
        private readonly ILoggingAdapter _log;
        
        public ITimerScheduler Timers { get; set; }

        public NetworkReceiverActor(Socket socket)
        {
            _socket = socket;
            _log = Context.GetLogger();
            
            Receive<ReceiveBufMessage>(ReceiveBufMessageHandler);
            
            _log.Info($"Initialized {nameof(NetworkReceiverActor)}");
        }

        public static Props Props(Socket socket)
        {
            return Akka.Actor.Props.Create(() => new NetworkReceiverActor(socket));
        }

        protected override void PreStart()
        {
            _log.Info($"PreStart for {nameof(NetworkReceiverActor)}");

            Context.ActorOf(DispatcherActor.Props(), nameof(DispatcherActor));
            
            Timers.StartPeriodicTimer(
                nameof(NetworkReceiverActor), 
                new ReceiveBufMessage(), 
                TimeSpan.FromMilliseconds(1));
        }
        
        private void ReceiveBufMessageHandler(ReceiveBufMessage _)
        {
            var (isSuccess, _, packet, error) = Packet.Create(_socket);
            if (isSuccess)
            {
                Context.Parent.Tell(new ReceivedBufMessage(packet));
                _log.Info($"Received Packet Type: {packet.GetPacketType()}");
            }
            else
            {
               _log.Error($"Received Packet Error: {error}");
            }
        }
    }

    public class DispatcherActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log;
        
        public DispatcherActor()
        {
            _log = Context.GetLogger();

            Receive<ReceivedBufMessage>(ReceivedBufMessageHandler);
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new DispatcherActor());
        }
        
        private void ReceivedBufMessageHandler(ReceivedBufMessage msg)
        {
            var dispatchName = msg.Packet.GetPacketType().GetDispatchName();
            _log.Info(dispatchName);
        }
    } 

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
}