namespace PTML
open PTML.Tree
open PTML.Layout
open PTML.Spinner

module Drawers = 
    type RenderOperation =
        | DrawChar of string * int * int * string option * string option * string option * string option
        | DrawSpinner of Types * int * int * string * string * string * string option * string option
        | CursorStyle of Cursor.Shape * int * int * string option * string option * string option

    let borderChars(border: Border) =
        match border with
        | Single -> ("┌", "┐", "└", "┘", "─", "│")
        | Double -> ("╔", "╗", "╚", "╝", "═", "║")
        | Classic -> ("┍", "┑", "┕", "┙", "─", "│")
        | Bold -> ("┏", "┓", "┗", "┛", "━", "┃")
        | Strange -> ("╒", "╕", "╘", "╛", "═", "│")
        | Rounded -> ("╭", "╮", "╰", "╯", "─", "│")
        | Border.Ascii -> ("+", "+", "+", "+", "-", "|")
        | Borderless -> ("", "", "", "", "", "")
        | NoBorder -> ("", "", "", "", "", "")

    let borderCharsContinuity(border: Border) =
        match border with
        | Single -> ("├", "┤", "┬", "┴", "┼")
        | Double -> ("╟", "╢", "╤", "╧", "╪")
        | Classic -> ("┝", "┥", "┯", "┷", "┿")
        | Bold -> ("┝", "┥", "┯", "┷", "┿")
        | Strange -> ("╞", "╡", "╤", "╧", "╪")
        | Rounded -> ("├", "┤", "┬", "┴", "┼")
        | Border.Ascii -> ("+", "+", "+", "+", "+")
        | Borderless -> ("", "", "", "", "")
        | NoBorder -> ("", "", "", "", "")

//#region HR_DRAWER
    let hrChars(ori: Orientation) =
        match ori with
        | Vertical -> "│"
        | Horizontal -> "─"
    let drawHorizontal xStart xEnd y char fore =
        [ for x in xStart .. xEnd -> DrawChar(char, x, y, fore, None, None, None) ]

    let drawVertical x yStart yEnd char fore =
        [ for y in yStart .. yEnd -> DrawChar(char, x, y, fore, None, None, None) ]
//#endregion

//#region BOX / BLOCK / FRAME
    let drawBorder x y width height border borderColor =
        match border with
        | NoBorder
        | Borderless -> []
        | _ ->
            let topLeft, topRight, bottomLeft, bottomRight, horizontal, vertical = borderChars border
            let left = x
            let right = x + width + 1
            let top = y
            let bottom = y + height + 1
            let fore = borderColor

            [ 
            DrawChar(topLeft, left, top, fore, None, None, None)
            DrawChar(topRight, right, top, fore, None, None, None)
            DrawChar(bottomLeft, left, bottom, fore, None, None, None)
            DrawChar(bottomRight, right, bottom, fore, None, None, None) ]
            @ drawHorizontal (left + 1) (right - 1) top horizontal fore
            @ drawHorizontal (left + 1) (right - 1) bottom horizontal fore
            @ drawVertical left (top + 1) (bottom - 1) vertical fore
            @ drawVertical right (top + 1) (bottom - 1) vertical fore

    let drawBorderWithTitle x y width height border borderColor title =
        match border with
        | NoBorder
        | Borderless -> []
        | _ ->
            let topLeft, topRight, bottomLeft, bottomRight, horizontal, vertical = borderChars border
            let left = x
            let right = x + width + 1
            let top = y
            let bottom = y + height + 1
            let fore = borderColor
            let interiorStart = left + 1
            let interiorEnd = right - 1
            let titleOps =
                match title with
                | Some text when text <> "" ->
                    let textValue = text
                    let textLength = textValue.Length
                    let interiorWidth = interiorEnd - interiorStart + 1
                    let leftFill = min 2 interiorWidth
                    let titleText = if textLength > interiorWidth - leftFill then textValue.Substring(0, max 0 (interiorWidth - leftFill)) else textValue
                    let rightFill = interiorWidth - leftFill - titleText.Length
                    let leftOps = drawHorizontal interiorStart (interiorStart + leftFill - 1) top horizontal fore
                    let titleOps =
                        [ for i in 0 .. titleText.Length - 1 -> DrawChar(string titleText.[i], interiorStart + leftFill + i, top, fore, None, None, None) ]
                    let rightOps = if rightFill > 0 then drawHorizontal (interiorStart + leftFill + titleText.Length) interiorEnd top horizontal fore else []
                    leftOps @ titleOps @ rightOps
                | _ -> drawHorizontal interiorStart interiorEnd top horizontal fore

            [ 
            DrawChar(topLeft, left, top, fore, None, None, None)
            DrawChar(topRight, right, top, fore, None, None, None)
            DrawChar(bottomLeft, left, bottom, fore, None, None, None)
            DrawChar(bottomRight, right, bottom, fore, None, None, None) ]
            @ titleOps
            @ drawHorizontal interiorStart interiorEnd bottom horizontal fore
            @ drawVertical left (top + 1) (bottom - 1) vertical fore
            @ drawVertical right (top + 1) (bottom - 1) vertical fore
    
    let drawFrame x y width height fw fwColor =
        match fw with
        | _ ->
            let topLeft, topRight, bottomLeft, bottomRight = Frames.frameChars(fw)
            let left = x
            let right = x + width + 1
            let top = y
            let bottom = y + height + 1
            let fore = fwColor

            [ 
            DrawChar(topLeft, left, top, fore, None, None, None)
            DrawChar(topRight, right, top, fore, None, None, None)
            DrawChar(bottomLeft, left, bottom, fore, None, None, None)
            DrawChar(bottomRight, right, bottom, fore, None, None, None) ]
//#endregion

//#region ESCAPE DRAWER
    let drawEscapedText baseX baseY text fg bg font url =
        let mutable x = baseX
        let mutable y = baseY
        let mutable ops = []

        for ch in text do
            match ch with
            | '\n' ->
                y <- y + 1
                x <- baseX
            | '\r' ->
                x <- baseX
            | '\t' ->
                let spaces = 4 - ((x - baseX) % 4)
                for offset in 0 .. spaces - 1 do
                    ops <- DrawChar(" ", x + offset, y, fg, bg, font, url) :: ops
                x <- x + spaces
            | '\b' ->
                x <- max baseX (x - 1)
            | '\f' ->
                y <- y + 1
                x <- baseX
            | '\v' ->
                y <- y + 1
                x <- baseX
            | _ ->
                ops <- DrawChar(string ch, x, y, fg, bg, font, url) :: ops
                x <- x + 1
        List.rev ops
//#endregion