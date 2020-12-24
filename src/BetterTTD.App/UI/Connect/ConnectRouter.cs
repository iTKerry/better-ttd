using BetterTTD.App.UI.Connect.Abstractions;
using ReactiveUI;

namespace BetterTTD.App.UI.Connect
{
    public class ConnectRouter : IConnectRouter
    {
        private readonly RoutingState _router;

        public ConnectRouter(RoutingState router)
        {
            _router = router;
        }

        public void NavigateToHome()
        {
            _router.NavigateAndReset.Execute();
        }
    }
}