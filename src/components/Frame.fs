namespace PTML

module Frames =
    type FrameWorks =
    | Bold
    | Pixels
    | Blocks
    | Point
    | Border
    | Picture
    | Photograph
    | Pythagoras
    | Arrow
    | Ascii

    let parseFrameWork(fm: string): FrameWorks = 
        match fm with
        | "bold" -> FrameWorks.Bold
        | "pixels" -> Pixels
        | "blocks" -> FrameWorks.Blocks
        | "point" -> Point
        | "border" -> FrameWorks.Border
        | "picture" -> Picture
        | "photograph" -> Photograph
        | "pythagoras" -> Pythagoras
        | "arrow" -> FrameWorks.Arrow
        | "ascii" -> FrameWorks.Ascii
        | _ -> failwith $"Valor improvável para `framework`: {fm}"