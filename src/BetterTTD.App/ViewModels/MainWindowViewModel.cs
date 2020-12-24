using System.Reactive;
using System.Runtime.Serialization;
using System.Windows.Input;
using ReactiveUI;

namespace BetterTTD.App.ViewModels
{
    [DataContract]
    public class MainWindowViewModel : ReactiveObject, IScreen
    {
        private readonly ReactiveCommand<Unit, Unit> _connect;

        private RoutingState _router = new RoutingState();

        public MainWindowViewModel()
        {
            _connect = ReactiveCommand.Create(() =>
            {
                Router.NavigateAndReset.Execute(new ConnectViewModel(this));
            });
        }
        
        [DataMember]
        public RoutingState Router
        {
            get => _router;
            set => this.RaiseAndSetIfChanged(ref _router, value);
        }
        
        public ICommand ConnectCommand => _connect;
    }
}