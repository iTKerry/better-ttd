namespace BetterTTD.FOAN

module Login =

    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    
    type State = 
        { Host : string
          Port : int
          Pass : string }
    
    type Msg =
        | Connect of host : string *
                     port : int    *
                     pass : string
        | HostChanged of string
        | PortChanged of string
        | PassChanged of string
    
    let init =
        { Host = "127.0.0.1"
          Port = 3977
          Pass = "p7gvv" }
        
    let update (msg: Msg) (state: State) =
        match msg with
        | HostChanged host -> { state with Host = host }
        | PortChanged port -> { state with Port = int port }
        | PassChanged pass -> { state with Pass = pass }
        | _ -> state
        
    let view (state: State) (dispatch) =
        Grid.create [
            Grid.children [
                StackPanel.create [
                    StackPanel.verticalAlignment   VerticalAlignment.Center
                    StackPanel.horizontalAlignment HorizontalAlignment.Center
                    StackPanel.children [
                        TextBlock.create [
                            TextBlock.text "Connect to OpenTTD Admin"
                        ]
                        TextBox.create [
                            TextBox.text state.Host
                            TextBox.onTextChanged (fun text -> dispatch <| HostChanged text)
                        ]
                        TextBox.create [
                            TextBox.text <| string state.Port
                            TextBox.onTextChanged (fun text -> dispatch <| PortChanged text)
                        ]
                        TextBox.create [
                            TextBox.text state.Pass
                            TextBox.passwordChar '*'
                            TextBox.onTextChanged (fun text -> dispatch <| PassChanged text)
                        ]
                        Button.create [
                            Button.onClick (fun _ -> dispatch <| Connect (state.Host, state.Port, state.Pass))
                            Button.content "Connect"
                        ]
                    ]
                ]
            ]
        ]