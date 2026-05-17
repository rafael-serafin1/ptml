#load "layout.fsx"
open Tree
open Layout

type RenderOperation =
    | DrawChar of string * int * int * string option * string option * string option

let private borderChars(border: Border) =
    match border with
    | Single -> ("┌", "┐", "└", "┘", "─", "│")
    | Double -> ("╔", "╗", "╚", "╝", "═", "║")
    | Classic -> ("┍", "┑", "┕", "┙", "─", "│")
    | Bold -> ("┏", "┓", "┗", "┛", "━", "┃")
    | Strange -> ("╒", "╕", "╘", "╛", "═", "│")
    | Rounded -> ("╭", "╮", "╰", "╯", "─", "│")
    | Ascii -> ("+", "+", "+", "+", "-", "|")
    | NoBorder -> ("", "", "", "", "", "")

let private drawHorizontal xStart xEnd y char fore =
    [ for x in xStart .. xEnd -> DrawChar(char, x, y, fore, None, None) ]

let private drawVertical x yStart yEnd char fore =
    [ for y in yStart .. yEnd -> DrawChar(char, x, y, fore, None, None) ]

let private drawBorder x y width height border borderColor =
    match border with
    | NoBorder -> []
    | _ ->
        let topLeft, topRight, bottomLeft, bottomRight, horizontal, vertical = borderChars border
        let left = x
        let right = x + width + 1
        let top = y
        let bottom = y + height + 1
        let fore = borderColor

        [ DrawChar(topLeft, left, top, fore, None, None)
          DrawChar(topRight, right, top, fore, None, None)
          DrawChar(bottomLeft, left, bottom, fore, None, None)
          DrawChar(bottomRight, right, bottom, fore, None, None) ]
        @ drawHorizontal (left + 1) (right - 1) top horizontal fore
        @ drawHorizontal (left + 1) (right - 1) bottom horizontal fore
        @ drawVertical left (top + 1) (bottom - 1) vertical fore
        @ drawVertical right (top + 1) (bottom - 1) vertical fore

let private drawBorderWithTitle x y width height border borderColor title =
    match border with
    | NoBorder -> []
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
                    [ for i in 0 .. titleText.Length - 1 -> DrawChar(string titleText.[i], interiorStart + leftFill + i, top, fore, None, None) ]
                let rightOps = if rightFill > 0 then drawHorizontal (interiorStart + leftFill + titleText.Length) interiorEnd top horizontal fore else []
                leftOps @ titleOps @ rightOps
            | _ -> drawHorizontal interiorStart interiorEnd top horizontal fore

        [ DrawChar(topLeft, left, top, fore, None, None)
          DrawChar(topRight, right, top, fore, None, None)
          DrawChar(bottomLeft, left, bottom, fore, None, None)
          DrawChar(bottomRight, right, bottom, fore, None, None) ]
        @ titleOps
        @ drawHorizontal interiorStart interiorEnd bottom horizontal fore
        @ drawVertical left (top + 1) (bottom - 1) vertical fore
        @ drawVertical right (top + 1) (bottom - 1) vertical fore

let rec private renderWidget offsetX offsetY widget =
    match widget with
    | PositionedTextWidget(text, fg, bg, font, metrics) ->
        [ DrawChar(text, offsetX + metrics.x, offsetY + metrics.y, fg, bg, font) ]

    | PositionedRowWidget(_, _, _, _, metrics, children)
    | PositionedColumnWidget(_, _, _, _, metrics, children) ->
        let baseX = offsetX + metrics.x
        let baseY = offsetY + metrics.y
        children |> List.collect (renderWidget baseX baseY)

    | PositionedBoxWidget(_, _, border, borderColor, _, metrics, children) ->
        let baseX = offsetX + metrics.x
        let baseY = offsetY + metrics.y
        let borderOps = drawBorder baseX baseY metrics.w metrics.h border borderColor
        let childBaseX = if border <> NoBorder then baseX + 1 else baseX
        let childBaseY = if border <> NoBorder then baseY + 1 else baseY
        let childOps = children |> List.collect (renderWidget childBaseX childBaseY)
        borderOps @ childOps

    | PositionedBlockWidget(_, _, border, borderColor, name, align, metrics, children) ->
        let baseX = offsetX + metrics.x
        let baseY = offsetY + metrics.y
        let borderOps = drawBorderWithTitle baseX baseY metrics.w metrics.h border borderColor name
        let childBaseX = if border <> NoBorder then baseX + 1 else baseX
        let childBaseY = if border <> NoBorder then baseY + 1 else baseY
        let childOps = children |> List.collect (renderWidget childBaseX childBaseY)
        borderOps @ childOps

let renderTree widgets =
    widgets |> List.collect (renderWidget 0 0)
