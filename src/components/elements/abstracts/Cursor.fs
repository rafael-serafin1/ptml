namespace PTML

module Cursor = 
    type Shape =    
    | Block
    | Bar
    | Underline

    let parseShape = function
        | "block" -> Shape.Block
        | "bar" -> Shape.Bar
        | "underline" -> Shape.Underline
        | value -> failwith $"No such shape avaliable called: {value}"

    let parseCursorColor = function
        | "red" -> "#f00"
        | "yellow" -> "#ff0"
        | "white" -> "#fff" 
        | "black" -> "#000"
        | "blue" -> "#00f"
        | "purple" -> "#50f"
        | "teal" -> "#0ff"
        | "pink" -> "#f0f"
        | "green" -> "#0f0"
        | value -> value