#nullable enable

using BetterTTD.App.UI.Main.Abstractions;
using ReactiveUI;
using Splat;

namespace BetterTTD.App.UI.Main
{
    public class MainPresenter : ReactiveObject, IRoutableViewModel
    {
        private readonly IMainInteractor _interactor;
        
        public string UrlPathSegment => "/main";
        
        public IScreen HostScreen { get; }

        public MainPresenter(IScreen? screen)
        {
            System.Console.WriteLine("Main Presenter");
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            _interactor = new MainInteractor();
        }
    }
}