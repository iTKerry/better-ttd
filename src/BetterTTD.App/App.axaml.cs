using Avalonia;
using Avalonia.Markup.Xaml;
using BetterTTD.App.ViewModels;
using BetterTTD.App.Views;
using ReactiveUI;
using Splat;

namespace BetterTTD.App
{
    public class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            Locator.CurrentMutable.RegisterConstant<IScreen>(new MainWindowViewModel());
            Locator.CurrentMutable.Register<IViewFor<ConnectViewModel>>(() => new ConnectView());

            new MainWindow { DataContext = Locator.Current.GetService<IScreen>() }.Show();
            base.OnFrameworkInitializationCompleted();
        }
    }
}
