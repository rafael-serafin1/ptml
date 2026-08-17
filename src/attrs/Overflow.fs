namespace PTML

module Overflow =
    type OverflowValues = 
    | Break
    | Wrap
    | Clip
    | Cut

    let parseOverflow(str: string): OverflowValues = 
        match str with
        | "break" ->  Break
        | "wrap"  ->  Wrap
        | "clip"  ->  Clip
        | "cut"   ->  Cut
        | _ -> failwith $"Invalid value for 'overflow': '{str}'"