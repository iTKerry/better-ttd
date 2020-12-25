using ReactiveUI;

namespace BetterTTD.App.UI.Console
{
    public class ConsolePresenter : ReactiveObject
    {
        public ConsolePresenter()
        {
            System.Console.WriteLine($"{nameof(ConsolePresenter)}");
        }
    }
}