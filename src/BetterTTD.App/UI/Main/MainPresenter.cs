#nullable enable

using BetterTTD.App.BL.Models;
using BetterTTD.App.UI.Main.Abstractions;
using DynamicData.Binding;
using ReactiveUI;
using Splat;

namespace BetterTTD.App.UI.Main
{
    public class MainPresenter : ReactiveObject, IRoutableViewModel, IMainInteractorNotifier
    {
        private readonly IMainInteractor _interactor;

        public string UrlPathSegment => "/main";
        
        public IScreen HostScreen { get; }

        public MainPresenter(IScreen? screen)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            _interactor = new MainInteractor(this);
        }

        private string _clients = "Clients: 0";
        public string Clients
        {
            get => _clients;
            set => this.RaiseAndSetIfChanged(ref  _clients, value);
        }

        private string _companies = "Companies: 0";
        public string Companies
        {
            get => _companies;
            set => this.RaiseAndSetIfChanged(ref  _companies, value);
        }

        public void ClientCountUpdate(int count)
        {
            Clients = $"Clients: {count}";
        }

        public void CompaniesCountUpdate(int count)
        {
            Companies = $"Companies: {count}";
        }

        public void ChatMessageUpdate(ChatModel message)
        {
            //ChatMessages.Add(message);
        }
    }
}