using ReactiveUI;

namespace BetterTTD.App.UI.Chat
{
    public class ChatPresenter : ReactiveObject
    {
        public ChatPresenter()
        {
            System.Console.WriteLine($"{nameof(ChatPresenter)}");
        }
    }
}