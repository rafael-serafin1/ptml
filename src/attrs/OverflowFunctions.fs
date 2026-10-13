namespace PTML
open System
open PTML.Drawers
open PTML.Layout
open PTML.Tree
open PTML.Overflow

module OverflowFunctions = 
    let textCells operations =
        operations
        |> List.collect (function
            | DrawChar(text, x, y, fg, bg, font, url) ->
                text
                |> Seq.mapi (fun index ch -> (ch, x + index, y, fg, bg, font, url))
                |> Seq.toList
            | _ -> [])

    let cellsToOperations cells =
        cells
        |> List.map (fun (ch, x, y, fg, bg, font, url) -> DrawChar(string ch, x, y, fg, bg, font, url))

    let constrainChildren overflow metrics operations =
        let left = metrics.x
        let top = metrics.y
        let right = left + metrics.w
        let bottom = top + metrics.h
        let cells = textCells operations

        match overflow with
        | Clip ->
            cells
            |> List.filter (fun (_, x, y, _, _, _, _) -> x >= left && x < right && y >= top && y < bottom)
            |> cellsToOperations
        | Cut ->
            cells
            |> List.filter (fun (_, x, y, _, _, _, _) -> x >= left && x < right && y >= top && y < bottom)
            |> cellsToOperations
        | Break ->
            cells
            |> List.sortBy (fun (_, x, y, _, _, _, _) -> y, x)
            |> List.mapi (fun index (ch, _, _, fg, bg, font, url) ->
                let x = left + index % max 1 metrics.w
                let y = top + index / max 1 metrics.w
                (ch, x, y, fg, bg, font, url))
            |> List.truncate (max 0 (metrics.w * metrics.h))
            |> cellsToOperations
        | Wrap ->
            let ordered = cells |> List.sortBy (fun (_, x, y, _, _, _, _) -> y, x)
            let mutable x = left
            let mutable y = top
            let mutable pendingSpace = false
            let result = ResizeArray<_>()

            for cell in ordered do
                let ch, _, _, fg, bg, font, url = cell

                if Char.IsWhiteSpace ch then
                    pendingSpace <- true
                else
                    let nextWordLength =
                        ordered
                        |> List.skipWhile (fun candidate -> candidate <> cell)
                        |> List.takeWhile (fun (candidate, _, _, _, _, _, _) -> not (Char.IsWhiteSpace candidate))
                        |> List.length
                    if pendingSpace && x < left + metrics.w then
                        result.Add((' ', x, y, fg, bg, font, url))
                        x <- x + 1
                    if x + nextWordLength > left + metrics.w then
                        x <- left
                        y <- y + 1
                    if y < bottom then
                        result.Add((ch, x, y, fg, bg, font, url))
                        x <- x + 1
                    pendingSpace <- false
            result |> Seq.toList |> cellsToOperations

    let applyOverflow overflow width height metrics operations =
        let cells = textCells operations
        let exceedsWidth =
            cells
            |> List.exists (fun (_, x, _, _, _, _, _) -> x < metrics.x || x >= metrics.x + metrics.w)
        let exceedsHeight =
            cells
            |> List.exists (fun (_, _, y, _, _, _, _) -> y < metrics.y || y >= metrics.y + metrics.h)
        let hasOverflow =
            match width, height with
            | Auto, Auto -> exceedsWidth || exceedsHeight
            | Auto, _ -> exceedsHeight
            | _, Auto -> exceedsWidth
            | _ -> false

        if not hasOverflow then
            operations
        else 
            constrainChildren overflow metrics operations

    let rec containsFramedChild widget =
        match widget with
        | PositionedBoxWidget _
        | PositionedBlockWidget _
        | PositionedFrameWidget _
        | PositionedGridWidget _ -> true
        | PositionedRowWidget(_, _, _, _, _, _, children)
        | PositionedColumnWidget(_, _, _, _, _, _, children)
        | PositionedCellWidget(_, children)
        | PositionedTerminalWidget(_, _, _, _, _, children) ->
            children |> List.exists containsFramedChild
        | _ -> false

    let applyContainerOverflow overflow width height metrics children operations =
        if children |> List.exists containsFramedChild then 
            operations
        else 
            applyOverflow overflow width height metrics operations