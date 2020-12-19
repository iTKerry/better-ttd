using BetterTTD.WPF.ViewModels;
using GalaSoft.MvvmLight.Ioc;

namespace BetterTTD.WPF
{
    public class ViewModelLocator
    {
        private readonly SimpleIoc _ioc;
        
        public ViewModelLocator()
        {
            _ioc = SimpleIoc.Default;
            
            _ioc.Register<MainWindowViewModel>();
            _ioc.Register<LoginViewModel>();
            _ioc.Register<HomeViewModel>();
        }
        
        public MainWindowViewModel MainWindowViewModel => _ioc.GetInstance<MainWindowViewModel>();
        public LoginViewModel LoginViewModel => _ioc.GetInstance<LoginViewModel>();
        public HomeViewModel HomeViewModel => _ioc.GetInstance<HomeViewModel>();
    }
}
