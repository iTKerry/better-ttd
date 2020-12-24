using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using BetterTTD.App.ViewModels;
using ReactiveUI;

namespace BetterTTD.App.Views
{
    public sealed class ConnectView : ReactiveUserControl<ConnectViewModel>
    {
        public ConnectView()
        {
            this.WhenActivated(disposable => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}