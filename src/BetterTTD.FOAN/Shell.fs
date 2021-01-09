namespace BetterTTD.FOAN

module Shell =
    
    open Elmish
    open Avalonia.Controls
    open Avalonia.FuncUI
    open Avalonia.FuncUI.Components.Hosts
    open Avalonia.FuncUI.Elmish
    open Avalonia.FuncUI.DSL

    open Akka.FSharp
    open Akka.Actor
    
    open BetterTTD.FOAN.Actors.Actors.AdminCoordinator
    open BetterTTD.FOAN.Actors.Messages

    type ConnectionState =
        | Connected
        | Disconnected
    
    type State =
        { aboutState: About.State
          counterState: Counter.State
          loginState: Login.State
          
          connectionState: ConnectionState
          
          coordinatorRef: IActorRef option }

    type Msg =
        | AboutMsg of About.Msg
        | CounterMsg of Counter.Msg
        | LoginMsg of Login.Msg
        | UiMsg of UiMessage

    let actorDispatch (state : State) =
        let sub dispatch =
            let system = System.create "System" <| Configuration.load ()
            let adminRef = spawn system "adminCoordinator" <| adminCoordinator dispatch
            
            ()
        Cmd.ofSub sub
    
    let init =
        let aboutState, aboutCmd = About.init
        let counterState = Counter.init
        let loginState = Login.init
        
        { aboutState      = aboutState
          counterState    = counterState
          loginState      = loginState
          connectionState = Disconnected
          coordinatorRef  = None },
        Cmd.batch [ aboutCmd ]

    let update (msg: Msg) (state: State): State * Cmd<_> =
        match msg with
        | AboutMsg bpmsg ->
            let aboutState, cmd = About.update bpmsg state.aboutState
            { state with aboutState = aboutState },
            Cmd.map AboutMsg cmd
            
        | CounterMsg countermsg ->
            let counterMsg =
                Counter.update countermsg state.counterState
            { state with counterState = counterMsg },
            Cmd.none
            
        | LoginMsg loginMsg ->
            match loginMsg with
            | Login.Connect(host, port, pass) ->
                match state.coordinatorRef with
                | Some ref -> ref <! (Idle <| Connect(host, pass, port))
                | None -> printfn "Trying to login but actor system is not created!"
                state, Cmd.none
            | _ ->
                let loginState = Login.update loginMsg state.loginState
                printfn "%A" loginState
                { state with loginState = loginState },
                Cmd.none
                
        | UiMsg msg ->
            match msg with
            | ReceivedProtocol protocol -> state, Cmd.none
            | ReceivedWelcome  welcome  -> { state with connectionState = Connected }, Cmd.none

    let view (state: State) (dispatch) =
        match state.connectionState with
        | Connected -> 
            DockPanel.create [
                DockPanel.children [
                    TabControl.create [
                        TabControl.tabStripPlacement Dock.Left
                        TabControl.viewItems [
                            TabItem.create [
                                TabItem.header "Counter Sample"
                                TabItem.content (Counter.view state.counterState (CounterMsg >> dispatch))
                            ]
                            TabItem.create [
                                TabItem.header "About"
                                TabItem.content (About.view state.aboutState (AboutMsg >> dispatch))
                            ]
                        ]
                    ]
                ]
            ]
        | Disconnected ->
            DockPanel.create [
                DockPanel.children [
                    (Login.view state.loginState (LoginMsg >> dispatch))
                ]
            ]
        
    let subscription state =
        Cmd.batch [ Cmd.map UiMsg (actorDispatch state)]
        
    type MainWindow() as this =
        inherit HostWindow()
        do
            base.Title <- "Full App"
            base.Width <- 800.0
            base.Height <- 600.0
            base.MinWidth <- 800.0
            base.MinHeight <- 600.0

            Elmish.Program.mkProgram (fun () -> init) update view
            |> Program.withHost this
            |> Program.withSubscription subscription
            |> Program.run