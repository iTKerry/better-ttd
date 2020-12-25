using System.Threading.Tasks;
using System.Windows;
using Akka.Actor;
using BetterTTD.Actors.ClientGroup;
using GalaSoft.MvvmLight.Threading;

namespace BetterTTD.WPF
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherHelper.Initialize();
        }
    }

    public sealed class ClientSystem
    {
        private readonly ActorSystem _actorSystem;
        private readonly IActorRef _clientActor;

        public ClientSystem(string systemName)
        {
            _actorSystem = ActorSystem.Create(systemName);
            _clientActor = _actorSystem.ActorOf(ClientActor.Props(), nameof(ClientActor));
        }

        public async Task TerminateAsync()
        {
            await _actorSystem.Terminate();
        }
    }
}
