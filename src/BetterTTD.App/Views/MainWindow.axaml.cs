using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using BetterTTD.App.ViewModels;
using ReactiveUI;

namespace BetterTTD.App.Views
{
    public sealed class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            this.WhenActivated(disposables =>
            {
                ViewModel.ConnectCommand.Execute(null);
            });
            AvaloniaXamlLoader.Load(this);
        }
    }
}
