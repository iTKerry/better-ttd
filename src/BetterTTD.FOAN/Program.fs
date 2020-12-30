namespace BetterTTD.FOAN

open System
open Akka.FSharp
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI

open BetterTTD.FOAN.Actors.ActorsModule

type App() =
    inherit Application()

    let system = System.create "System" <| Configuration.load ()
    
    override this.Initialize() =
        let senderRef = spawn system "sender" sender
        let receiverRef = spawn system "receiver" receiver
        let _ = spawn system "adminCoordinator" <| adminCoordinator senderRef receiverRef
        
        system.Scheduler.ScheduleTellRepeatedly(
            TimeSpan.FromMilliseconds (0.),
            TimeSpan.FromMilliseconds (1.),
            receiverRef,
            ReceiveMsg)
        
        this.Styles.Load "avares://Avalonia.Themes.Default/DefaultTheme.xaml"
        this.Styles.Load "avares://Avalonia.Themes.Default/Accents/BaseDark.xaml"
        this.Styles.Load "avares://BetterTTD.FOAN/Styles.xaml"

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- Shell.MainWindow()
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main (args: string []) =
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
