using Avalonia;
using Avalonia.Markup.Xaml;
using BetterTTD.App.UI.Connect;
using BetterTTD.App.UI.Root;
using ReactiveUI;
using Splat;

namespace BetterTTD.App
{
    public class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            Locator.CurrentMutable.RegisterConstant<IScreen>(new RootPresenter());
            Locator.CurrentMutable.Register<IViewFor<ConnectPresenter>>(() => new ConnectView());

            new RootWindow {DataContext = Locator.Current.GetService<IScreen>()}.Show();
            base.OnFrameworkInitializationCompleted();
        }
    }
}
