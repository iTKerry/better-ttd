using System.Reactive;
using BetterTTD.App.BL.Messages;
using BetterTTD.App.UI.Chat.Abstractions;
using ReactiveUI;

namespace BetterTTD.App.UI.Chat
{
    public class ChatInteractor : IChatInteractor 
    {
        private readonly IChatInteractorNotifier _notifier;

        public ChatInteractor(IChatInteractorNotifier notifier)
        {
            _notifier = notifier;

            MessageBus.Current
                .Listen<ChatUpdateMessage>()
                .Subscribe(Observer.Create<ChatUpdateMessage>(msg => _notifier.ChatMessageUpdate(msg.Message)));
        }
    }
}