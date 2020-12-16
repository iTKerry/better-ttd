using Akka.Actor;
using Akka.Event;
using Microsoft.VisualBasic;

namespace BetterTTD.Actors
{
    public class BridgeActor : ReceiveActor
    {
        private readonly IClientView _clientView;
        private readonly IActorRef _clientActor;
        private readonly ILoggingAdapter _log;

        public BridgeActor(IClientView clientView, IActorRef clientActor)
        {
            _clientView = clientView;
            _clientActor = clientActor;
            _log = Context.GetLogger();
            
            Become(Active);

            _log.Info("Initialized");
        }

        public static Props Props(IClientView clientView, IActorRef clientActor)
        {
            return Akka.Actor.Props.Create(() => new BridgeActor(clientView, clientActor));
        }

        private void Active()
        {
            Receive<OnProtocolMessage>(msg => _clientView.OnProtocol(msg.Protocol));
            Receive<OnServerWelcomeMessage>(msg => _clientView.OnServerWelcome(msg.Game));
            Receive<OnServerCmdNamesMessage>(msg => _clientView.OnServerCmdNames(msg.CmdNames));
            Receive<OnServerConsoleMessage>(msg => _clientView.OnServerConsole(msg.Origin, msg.Message));
            Receive<OnServerClientInfoMessage>(msg => _clientView.OnServerClientInfo(msg.Client));
            
            Receive<AdminConnectMessage>(_clientActor.Tell);
            Receive<SetDefaultUpdateFrequencyMessage>(_clientActor.Tell);
            Receive<PollAllMessage>(_clientActor.Tell);
        }
    }
}