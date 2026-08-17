namespace PTML
open PTML.Tree
open PTML.Layout
open PTML.Spinner
open PTML.Drawers

module Render =
    let rec private renderWidget offsetX offsetY widget =
        match widget with
        | PositionedCursorWidget(sh, blk, clr, v, metrics) ->
            let baseX = offsetX + metrics.x
            let baseY = offsetY + metrics.y
            [ CursorStyle(sh, baseX, baseY, blk, clr, v) ]
        | PositionedEscapeWidget(esc, _, _, metrics) -> 
            let baseX = offsetX + metrics.x
            let baseY = offsetY + metrics.y
            drawEscapedText baseX baseY esc None None None None
        | PositionedProgressWidget(tp, value, max, width, height, show, metrics) ->
            let mutable baseX = offsetX + metrics.x
            let baseY = offsetY + metrics.y
            let mutable charFrames: string = ""
            match show with
            | None -> 
                charFrames <- Progress.framefy(metrics.w, metrics.h, tp, value, max, "false")
            | Some str ->
                charFrames <- Progress.framefy(metrics.w, metrics.h, tp, value, max, str)
            [ DrawChar(charFrames, baseX, baseY, None, None, None, None) ]
        | PositionedHrWidget(ori, _, _, metrics) ->
            let baseX = offsetX + metrics.x
            let baseY = offsetY + metrics.y
            match ori with
            | Horizontal ->
                drawHorizontal baseX (baseX + metrics.w - 1) baseY (hrChars Horizontal) None
            | Vertical ->
                drawVertical baseX baseY (baseY + metrics.h - 1) (hrChars Vertical) None

        | PositionedTextWidget(text, fg, bg, font, url, metrics) ->
            [ DrawChar(text, offsetX + metrics.x, offsetY + metrics.y, fg, bg, font, url) ]
        | PositionedFragWidget(text, fg, bg, font, url, metrics) ->
            [ DrawChar(text, offsetX + metrics.x, offsetY + metrics.y, fg, bg, font, url) ]
        
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
                                    yield DrawChar(middleCross, baseX + vx, baseY + hy, borderColor, None, None, None)
                                | _ -> () ]

                    let borderTopIntersections =
                        if border <> NoBorder then
                            grid.separators
                            |> List.choose (function
                                | VerticalSeparator x -> Some (DrawChar(topCross, baseX + x, baseY - 1, borderColor, None, None, None))
                                | _ -> None)
                        else []

                    let borderBottomIntersections =
                        if border <> NoBorder then
                            grid.separators
                            |> List.choose (function
                                | VerticalSeparator x -> Some (DrawChar(bottomCross, baseX + x, baseY + metrics.h, borderColor, None, None, None))
                                | _ -> None)
                        else []

                    let borderLeftIntersections =
                        if border <> NoBorder then
                            grid.separators
                            |> List.choose (function
                                | HorizontalSeparator y -> Some (DrawChar(leftCross, baseX - 1, baseY + y, borderColor, None, None, None))
                                | _ -> None)
                        else []

                    let borderRightIntersections =
                        if border <> NoBorder then
                            grid.separators
                            |> List.choose (function
                                | HorizontalSeparator y -> Some (DrawChar(rightCross, baseX + metrics.w, baseY + y, borderColor, None, None, None))
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

        | PositionedFrameWidget(fw, fc, w, h, align, paddingV, paddingH, metrics, children) ->
            let baseX = offsetX + metrics.x
            let baseY = offsetY + metrics.y

            let borderOps = drawFrame baseX baseY metrics.w metrics.h fw fc

            let childBaseX = baseX + 1
            let childBaseY = baseY + 1
            
            let childOps = children |> List.collect (renderWidget childBaseX childBaseY)
            borderOps @ childOps

        | PositionedDepthWidget(_, _, _, _, _) -> []

    let renderTree(widgets: PositionedWidget list): RenderOperation list =
        widgets |> List.collect (renderWidget 0 0)
