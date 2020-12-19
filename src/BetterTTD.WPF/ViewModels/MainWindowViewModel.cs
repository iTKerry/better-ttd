using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

namespace BetterTTD.WPF.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public BaseViewModel CurrentViewModel
        {
            get => Get<BaseViewModel>();
            set => Set(value);
        }
        
        public MainWindowViewModel()
        {
            Messenger.Default.Register<OnShowLoginMessage>(this, OnShowLoginMessageHandler);
            Messenger.Default.Register<OnShowHomeMessage>(this, OnShowHomeMessageHandler);
            
            CurrentViewModel = SimpleIoc.Default.GetInstance<LoginViewModel>();
        }

        private void OnShowHomeMessageHandler(OnShowHomeMessage msg)
        {
            var vm = SimpleIoc.Default.GetInstance<HomeViewModel>();
            CurrentViewModel = vm;
        }

        private void OnShowLoginMessageHandler(OnShowLoginMessage msg)
        {
            var vm = SimpleIoc.Default.GetInstance<HomeViewModel>();
            CurrentViewModel = vm;
        }
    }

    public record OnShowLoginMessage;

    public record OnShowHomeMessage;
}