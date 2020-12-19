using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace BetterTTD.WPF.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public RelayCommand LoginCommand => new RelayCommand(LoginCommandHandler);

        private void LoginCommandHandler()
        {
            Messenger.Default.Send(new OnShowHomeMessage());
        }
    }
}