#nullable enable

using ReactiveUI;
using Splat;

namespace BetterTTD.App.UI.Main
{
    public class MainPresenter : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment => "/main";
        
        public IScreen HostScreen { get; }

        public MainPresenter(IScreen? screen)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
        }
    }
}