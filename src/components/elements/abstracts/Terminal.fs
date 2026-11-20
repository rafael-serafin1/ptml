namespace PTML
open PTML.OSC
open PTML.Global
open PTML.Utils

module Terminal = 
    let backgroundColor (color: string) =
        terminalColorChange color Background

    let foregroundColor (color: string) =
        terminalColorChange color Foreground

    let windowTitle (title: string) = 
        changeWindowTitle title
    
    let iconName (name: string) =
        changeIconName name