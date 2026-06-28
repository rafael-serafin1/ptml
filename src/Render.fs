namespace PTML
open PTML.Tree
open PTML.Layout
open PTML.Spinner

module Render =
    type RenderOperation =
        | DrawChar of string * int * int * string option * string option * string option
        | DrawSpinner of Types * int * int * string * string * string * string option * string option

    let private borderChars(border: Border) =
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

    let private borderCharsContinuity(border: Border) =
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

    let private hrChars(ori: Orientation) =
        match ori with
        | Vertical -> "│"
        | Horizontal -> "─"

    let private drawHorizontal xStart xEnd y char fore =
        [ for x in xStart .. xEnd -> DrawChar(char, x, y, fore, None, None) ]

    let private drawVertical x yStart yEnd char fore =
        [ for y in yStart .. yEnd -> DrawChar(char, x, y, fore, None, None) ]

    let private drawBorder x y width height border borderColor =
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
            DrawChar(topLeft, left, top, fore, None, None)
            DrawChar(topRight, right, top, fore, None, None)
            DrawChar(bottomLeft, left, bottom, fore, None, None)
            DrawChar(bottomRight, right, bottom, fore, None, None) ]
            @ drawHorizontal (left + 1) (right - 1) top horizontal fore
            @ drawHorizontal (left + 1) (right - 1) bottom horizontal fore
            @ drawVertical left (top + 1) (bottom - 1) vertical fore
            @ drawVertical right (top + 1) (bottom - 1) vertical fore

    let private drawBorderWithTitle x y width height border borderColor title =
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
                        [ for i in 0 .. titleText.Length - 1 -> DrawChar(string titleText.[i], interiorStart + leftFill + i, top, fore, None, None) ]
                    let rightOps = if rightFill > 0 then drawHorizontal (interiorStart + leftFill + titleText.Length) interiorEnd top horizontal fore else []
                    leftOps @ titleOps @ rightOps
                | _ -> drawHorizontal interiorStart interiorEnd top horizontal fore

            [ 
            DrawChar(topLeft, left, top, fore, None, None)
            DrawChar(topRight, right, top, fore, None, None)
            DrawChar(bottomLeft, left, bottom, fore, None, None)
            DrawChar(bottomRight, right, bottom, fore, None, None) ]
            @ titleOps
            @ drawHorizontal interiorStart interiorEnd bottom horizontal fore
            @ drawVertical left (top + 1) (bottom - 1) vertical fore
            @ drawVertical right (top + 1) (bottom - 1) vertical fore

    let rec private renderWidget offsetX offsetY widget =
        match widget with
        | PositionedProgressWidget(tp, value, max, width, height, show, metrics) ->
            let mutable baseX = offsetX + metrics.x
            let baseY = offsetY + metrics.y
            let mutable charFrames: string = ""
            match show with
            | None -> 
                charFrames <- Progress.framefy(metrics.w, metrics.h, tp, value, max, "false")
            | Some str ->
                charFrames <- Progress.framefy(metrics.w, metrics.h, tp, value, max, str)
            [ DrawChar(charFrames, baseX, baseY, None, None, None) ]
        | PositionedHrWidget(ori, _, _, metrics) ->
            let baseX = offsetX + metrics.x
            let baseY = offsetY + metrics.y
            match ori with
            | Horizontal ->
                drawHorizontal baseX (baseX + metrics.w - 1) baseY (hrChars Horizontal) None
            | Vertical ->
                drawVertical baseX baseY (baseY + metrics.h - 1) (hrChars Vertical) None
        | PositionedTextWidget(text, fg, bg, font, metrics) ->
            [ DrawChar(text, offsetX + metrics.x, offsetY + metrics.y, fg, bg, font) ]
        | PositionedFragWidget(text, fg, bg, font, metrics) ->
            [ DrawChar(text, offsetX + metrics.x, offsetY + metrics.y, fg, bg, font) ]

        | PositionedSpinnerWidget(tp, inter, dur, comp, fg, bg, metrics) ->
            [ DrawSpinner(tp, offsetX + metrics.x, offsetY + metrics.y, inter, dur, comp, fg, bg) ]

        | PositionedRowWidget(_, _, _, _, metrics, children)
        | PositionedColumnWidget(_, _, _, _, metrics, children) ->
            let baseX = offsetX + metrics.x
            let baseY = offsetY + metrics.y
            children |> List.collect (renderWidget baseX baseY)

        | PositionedCellWidget(metrics, children) ->
            let baseX = offsetX + metrics.x
            let baseY = offsetY + metrics.y
            children |> List.collect (renderWidget baseX baseY)

        | PositionedBoxWidget(_, _, border, borderColor, _, _, _, metrics, children) ->
            let baseX = offsetX + metrics.x
            let baseY = offsetY + metrics.y
            let borderOps = drawBorder baseX baseY metrics.w metrics.h border borderColor
            let childBaseX = if border <> NoBorder then baseX + 1 else baseX
            let childBaseY = if border <> NoBorder then baseY + 1 else baseY
            let childOps = children |> List.collect (renderWidget childBaseX childBaseY)
            borderOps @ childOps

        | PositionedBlockWidget(_, _, border, borderColor, name, align, _, _, metrics, children) ->
            let baseX = offsetX + metrics.x
            let baseY = offsetY + metrics.y
            let borderOps = drawBorderWithTitle baseX baseY metrics.w metrics.h border borderColor name
            let childBaseX = if border <> NoBorder then baseX + 1 else baseX
            let childBaseY = if border <> NoBorder then baseY + 1 else baseY
            let childOps = children |> List.collect (renderWidget childBaseX childBaseY)
            borderOps @ childOps

        | PositionedGridWidget(border, borderColor, metrics, children: List<GridLayout>) ->
            let baseX = offsetX + metrics.x
            let baseY = offsetY + metrics.y

            let separatorOps =
                children
                |> List.collect (fun grid ->
                    let top = baseY
                    let left = baseX
                    let bottom = baseY + metrics.h - 1
                    let right = baseX + metrics.w - 1
                    let leftCross, rightCross, topCross, bottomCross, middleCross = borderCharsContinuity border
                    let horizontalLine = borderChars border |> fun (_, _, _, _, h, _) -> h
                    let verticalLine = borderChars border |> fun (_, _, _, _, _, v) -> v

                    let horizontalLines =
                        grid.separators
                        |> List.choose (function
                            | HorizontalSeparator y -> Some (drawHorizontal left right (baseY + y) horizontalLine borderColor)
                            | _ -> None)
                        |> List.collect id

                    let verticalLines =
                        grid.separators
                        |> List.choose (function
                            | VerticalSeparator x -> Some (drawVertical (baseX + x) top bottom verticalLine borderColor)
                            | _ -> None)
                        |> List.collect id

                    let crossPoints =
                        [ for xSep in grid.separators do
                            for ySep in grid.separators do
                                match xSep, ySep with
                                | VerticalSeparator vx, HorizontalSeparator hy ->
                                    yield DrawChar(middleCross, baseX + vx, baseY + hy, borderColor, None, None)
                                | _ -> () ]

                    let borderTopIntersections =
                        if border <> NoBorder then
                            grid.separators
                            |> List.choose (function
                                | VerticalSeparator x -> Some (DrawChar(topCross, baseX + x, baseY - 1, borderColor, None, None))
                                | _ -> None)
                        else []

                    let borderBottomIntersections =
                        if border <> NoBorder then
                            grid.separators
                            |> List.choose (function
                                | VerticalSeparator x -> Some (DrawChar(bottomCross, baseX + x, baseY + metrics.h, borderColor, None, None))
                                | _ -> None)
                        else []

                    let borderLeftIntersections =
                        if border <> NoBorder then
                            grid.separators
                            |> List.choose (function
                                | HorizontalSeparator y -> Some (DrawChar(leftCross, baseX - 1, baseY + y, borderColor, None, None))
                                | _ -> None)
                        else []

                    let borderRightIntersections =
                        if border <> NoBorder then
                            grid.separators
                            |> List.choose (function
                                | HorizontalSeparator y -> Some (DrawChar(rightCross, baseX + metrics.w, baseY + y, borderColor, None, None))
                                | _ -> None)
                        else []

                    horizontalLines @ verticalLines @ crossPoints @ borderTopIntersections @ borderBottomIntersections @ borderLeftIntersections @ borderRightIntersections)

            let childOps =
                children
                |> List.collect (fun grid ->
                    grid.cells
                    |> List.collect (fun cell -> renderWidget (baseX + cell.metrics.x) (baseY + cell.metrics.y) cell.widget))

            separatorOps @ childOps

        | PositionedTerminalWidget(_, _, alignX, alignY, metrics, children) ->
            let baseX = offsetX + metrics.x
            let baseY = offsetY + metrics.y
            children |> List.collect (renderWidget baseX baseY)

        | PositionedDepthWidget(_, _, _, _, _) -> []

    let renderTree(widgets: PositionedWidget list): RenderOperation list =
        widgets |> List.collect (renderWidget 0 0)
