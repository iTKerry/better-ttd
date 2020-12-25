using Avalonia.Threading;
using BetterTTD.App.UI.Connect.Abstractions;
using BetterTTD.App.UI.Main;
using ReactiveUI;

namespace BetterTTD.App.UI.Connect
{
    public class ConnectRouter : IConnectRouter
    {
        private readonly IScreen _screen;
        private readonly RoutingState _router;

        public ConnectRouter(IScreen screen)
        {
            _screen = screen;
            _router = screen.Router;
        }

        public void NavigateToMain()
        {
            Dispatcher.UIThread.Post(() =>
            {
                _router.NavigateAndReset.Execute(new MainPresenter(_screen));
            });
        }
    }
}