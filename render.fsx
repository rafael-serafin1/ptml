#load "layout.fsx"
open Tree
open Layout

type RenderOperation =
    | DrawChar of string * int * int

let private borderChars border =
    match border with
    | Single -> ("┌", "┐", "└", "┘", "─", "│")
    | Double -> ("╔", "╗", "╚", "╝", "═", "║")
    | Bold -> ("┏", "┓", "┗", "┛", "━", "┃")
    | Rounded -> ("╭", "╮", "╰", "╯", "─", "│")
    | Ascii -> ("+", "+", "+", "+", "-", "|")
    | NoBorder -> ("", "", "", "", "", "")

let private drawHorizontal xStart xEnd y char =
    [ for x in xStart .. xEnd -> DrawChar(char, x, y) ]

let private drawVertical x yStart yEnd char =
    [ for y in yStart .. yEnd -> DrawChar(char, x, y) ]

let private drawBorder x y width height border =
    match border with
    | NoBorder -> []
    | _ ->
        let topLeft, topRight, bottomLeft, bottomRight, horizontal, vertical = borderChars border
        let left = x
        let right = x + width + 1
        let top = y
        let bottom = y + height + 1

        [ DrawChar(topLeft, left, top)
          DrawChar(topRight, right, top)
          DrawChar(bottomLeft, left, bottom)
          DrawChar(bottomRight, right, bottom) ]
        @ drawHorizontal (left + 1) (right - 1) top horizontal
        @ drawHorizontal (left + 1) (right - 1) bottom horizontal
        @ drawVertical left (top + 1) (bottom - 1) vertical
        @ drawVertical right (top + 1) (bottom - 1) vertical

let rec private renderWidget offsetX offsetY widget =
    let renderChildren border metrics children =
        let baseX = offsetX + metrics.x
        let baseY = offsetY + metrics.y
        let contentX = if border = NoBorder then baseX else baseX + 1
        let contentY = if border = NoBorder then baseY else baseY + 1
        let borderOps = drawBorder baseX baseY metrics.w metrics.h border
        let childOps = children |> List.collect (renderWidget contentX contentY)
        if border = NoBorder then childOps else borderOps @ childOps

    match widget with
    | PositionedTextWidget(text, _, _, _, metrics) ->
        [ DrawChar(text, offsetX + metrics.x, offsetY + metrics.y) ]

    | PositionedRowWidget(_, border, _, _, metrics, children)
    | PositionedColumnWidget(_, border, _, _, metrics, children) ->
        renderChildren border metrics children

    | PositionedBoxWidget(_, _, border, _, _, metrics, children) ->
        renderChildren border metrics children

let renderTree widgets =
    widgets |> List.collect (renderWidget 0 0)
