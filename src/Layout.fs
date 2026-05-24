namespace PTML
open PTML.Tree
open PTML.Token
open System

module Layout = 
    type Metrics = {
        x: int
        y: int
        w: int
        h: int
    }

    type PositionedWidget =
        | PositionedTextWidget of text:string * foreground:string option * background:string option * font:string option * metrics:Metrics
        | PositionedRowWidget of width:Dimension * border:Border * gap:int * align:Align option * metrics:Metrics * children:PositionedWidget list
        | PositionedColumnWidget of width:Dimension * border:Border * gap:int * yAlign:Align option * metrics:Metrics * children:PositionedWidget list
        | PositionedBoxWidget of width:Dimension * height:Dimension * border:Border * borderColor:string option * align:Align option * metrics:Metrics * children:PositionedWidget list
        | PositionedBlockWidget of width:Dimension * height:Dimension * border:Border * borderColor:string option * name:string option * align:Align option * metrics:Metrics * children:PositionedWidget list
        | PositionedCellWidget of width: Dimension * height: Dimension * direction: CellOrientation * hasNextSibling: bool * border: Border * metrics: Metrics * children: PositionedWidget list
        | PositionedTerminalWidget of width:Dimension * height:Dimension * xAlign:Align option * yAlign:Align option * metrics:Metrics * children:PositionedWidget list

    let private charWidth = 1
    let private lineHeight = 1

    let private resolveDimension dimension parentSize fallback =
        match dimension with
        | Auto -> fallback
        | Fixed value -> value
        | Percent p ->
            match parentSize with
            | Some parent -> max 0 (parent * p / 100)
            | None -> fallback

    let rec private shiftWidget dx dy widget =
        let shiftMetrics metrics = { metrics with x = metrics.x + dx; y = metrics.y + dy }
        match widget with
        | PositionedTextWidget(text, fg, bg, font, metrics) -> PositionedTextWidget(text, fg, bg, font, shiftMetrics metrics)
        | PositionedRowWidget(width, border, gap, align, metrics, children) ->
            PositionedRowWidget(width, border, gap, align, shiftMetrics metrics, children)
        | PositionedColumnWidget(width, border, gap, yAlign, metrics, children) ->
            PositionedColumnWidget(width, border, gap, yAlign, shiftMetrics metrics, children)
        | PositionedCellWidget(width, height, direction, hasNextSibling, border, metrics, children) ->
            PositionedCellWidget(width, height, direction, hasNextSibling, border, shiftMetrics metrics, children)
        | PositionedBoxWidget(width, height, border, borderColor, align, metrics, children) ->
            PositionedBoxWidget(width, height, border, borderColor, align, shiftMetrics metrics, children)
        | PositionedBlockWidget(width, height, border, borderColor, name, align, metrics, children) ->
            PositionedBlockWidget(width, height, border, borderColor, name, align, shiftMetrics metrics, children)
        | PositionedTerminalWidget(width, height, xAlign, yAlign, metrics, children) ->
            PositionedTerminalWidget(width, height, xAlign, yAlign, shiftMetrics metrics, children)


    let private alignOffset containerSize childSize alignOption =
        match alignOption with
        | Some Start | None -> 0
        | Some Center -> (containerSize - childSize) / 2
        | Some End -> containerSize - childSize

    let rec layoutWidget widget parentWidth parentHeight =
        let metricsOf widget =
            match widget with
            | PositionedTextWidget(_, _, _, _, m)
            | PositionedRowWidget(_, _, _, _, m, _)
            | PositionedColumnWidget(_, _, _, _, m, _)
            | PositionedCellWidget(_, _, _, _, _, m, _)
            | PositionedBoxWidget(_, _, _, _, _, m, _)
            | PositionedBlockWidget(_, _, _, _, _, _, m, _) 
            | PositionedTerminalWidget(_, _, _, _, m, _) -> m

        let totalWidth widget =
            let metrics = metricsOf widget
            match widget with
            | PositionedBoxWidget(_, _, border, _, _, _, _) when border <> NoBorder ->
                metrics.w + 2
            | PositionedBlockWidget(_, _, border, _, _, _, _, _) when border <> NoBorder ->
                metrics.w + 2
            | _ -> metrics.w

        let totalHeight widget =
            let metrics = metricsOf widget
            match widget with
            | PositionedBoxWidget(_, _, border, _, _, _, _) when border <> NoBorder ->
                metrics.h + 2
            | PositionedBlockWidget(_, _, border, _, _, _, _, _) when border <> NoBorder ->
                metrics.h + 2
            | _ -> metrics.h

        match widget with
        (* Layout logic for each widget type *)
        (* Entra em loop até chegar aqui     *)
        | TextWidget(text, fg, bg, font) ->
            let w = text.Length * charWidth
            let h = lineHeight
            PositionedTextWidget(text, fg, bg, font, { x = 0; y = 0; w = w; h = h })

        | RowWidget(width, border, gap, align, children) ->
            let positionedChildren = children |> List.map (fun child -> layoutWidget child None None)

            let childWidths = positionedChildren |> List.map totalWidth
            let childHeights = positionedChildren |> List.map totalHeight

            let totalChildWidth = if List.isEmpty childWidths then 0 else List.sum childWidths + gap * (List.length childWidths - 1)
            let maxChildHeight = if List.isEmpty childHeights then 0 else List.max childHeights

            let resolvedWidth = resolveDimension width parentWidth totalChildWidth
            let resolvedHeight = max maxChildHeight lineHeight

            let positionedChildren =
                let rec place children xOffset acc =
                    match children with
                    | [] -> List.rev acc
                    | child :: rest ->
                        let childTotalWidth = totalWidth child
                        let childTotalHeight = totalHeight child
                        let yOffset = alignOffset resolvedHeight childTotalHeight align
                        let positioned = shiftWidget xOffset yOffset child
                        let nextX = xOffset + childTotalWidth + gap
                        place rest nextX (positioned :: acc)
                place positionedChildren 0 []
            PositionedRowWidget(width, border, gap, align, { x = 0; y = 0; w = resolvedWidth; h = resolvedHeight }, positionedChildren)

        | ColumnWidget(width, border, gap, yAlign, children) ->
            let positionedChildren = children |> List.map (fun child -> layoutWidget child None None)
            let childWidths = positionedChildren |> List.map totalWidth
            let childHeights = positionedChildren |> List.map totalHeight
            let maxChildWidth = if List.isEmpty childWidths then 0 else List.max childWidths
            let totalChildHeight = if List.isEmpty childHeights then 0 else List.sum childHeights + gap * (List.length childHeights - 1)
            let resolvedWidth = resolveDimension width parentWidth maxChildWidth
            let resolvedHeight = resolveDimension (Fixed totalChildHeight) parentHeight totalChildHeight
            let positionedChildren =
                let rec place children yOffset acc =
                    match children with
                    | [] -> List.rev acc
                    | child :: rest ->
                        let childTotalWidth = totalWidth child
                        let childTotalHeight = totalHeight child
                        let xOffset = alignOffset resolvedWidth childTotalWidth yAlign
                        let positioned = shiftWidget xOffset yOffset child
                        let nextY = yOffset + childTotalHeight + gap
                        place rest nextY (positioned :: acc)
                place positionedChildren 0 []
            PositionedColumnWidget(width, border, gap, yAlign, { x = 0; y = 0; w = resolvedWidth; h = resolvedHeight }, positionedChildren)

        | BoxWidget(width, height, border, borderColor, align, children) ->
            let positionedChildren = children |> List.map (fun child -> layoutWidget child None None)

            let childWidths = positionedChildren |> List.map totalWidth
            let childHeights = positionedChildren |> List.map totalHeight

            let maxChildWidth = if List.isEmpty childWidths then 0 else List.max childWidths
            let totalChildHeight = if List.isEmpty childHeights then 0 else List.max childHeights

            let resolvedWidth = resolveDimension width parentWidth maxChildWidth
            let resolvedHeight = resolveDimension height parentHeight totalChildHeight

            let positionedChildren =
                positionedChildren
                |> List.map (fun child ->
                    let childTotalWidth = totalWidth child
                    let xOffset = alignOffset resolvedWidth childTotalWidth align
                    shiftWidget xOffset 0 child)
            PositionedBoxWidget(width, height, border, borderColor, align, { x = 0; y = 0; w = resolvedWidth; h = resolvedHeight }, positionedChildren)
        | BlockWidget(width, height, border, borderColor, name, align, children) ->
            let positionedChildren = children |> List.map (fun child -> layoutWidget child None None)

            let childWidths = positionedChildren |> List.map totalWidth
            let childHeights = positionedChildren |> List.map totalHeight
            
            let maxChildWidth = if List.isEmpty childWidths then 0 else List.max childWidths
            let titleWidth =
                match name with
                | Some t -> t.Length
                | None -> 0

            let contentWidth = max maxChildWidth titleWidth
            let totalChildHeight = if List.isEmpty childHeights then 0 else List.max childHeights

            let resolvedWidth = resolveDimension width parentWidth contentWidth
            let resolvedHeight = resolveDimension height parentHeight totalChildHeight

            let positionedChildren =
                positionedChildren
                |> List.map (fun child ->
                    let childTotalWidth = totalWidth child
                    let xOffset = alignOffset resolvedWidth childTotalWidth align
                    shiftWidget xOffset 0 child)
            PositionedBlockWidget(width, height, border, borderColor, name, align, { x = 0; y = 0; w = resolvedWidth; h = resolvedHeight }, positionedChildren)
        | CellWidget(width, height, orientation, total, border, children) ->
            let positionedChildren = children |> List.map (fun child -> layoutWidget child None None)

            let childWidths = positionedChildren |> List.map totalWidth
            let childHeights = positionedChildren |> List.map totalHeight
            
            let maxChildWidth = if List.isEmpty childWidths then 0 else List.max childWidths

            let totalChildWidth = if List.isEmpty childWidths then 0 else List.max childWidths
            let totalChildHeight = if List.isEmpty childHeights then 0 else List.max childHeights

            let resolvedWidth = resolveDimension width parentWidth totalChildWidth
            let resolvedHeight = resolveDimension height parentHeight totalChildHeight

            let positionedChildren =
                positionedChildren
                |> List.map (fun child ->
                    let childTotalWidth = totalWidth child
                    let xOffset = alignOffset resolvedWidth childTotalWidth None
                    shiftWidget xOffset 0 child)
            PositionedCellWidget(width, height, orientation, total, border, { x = 0; y = 0; w = 10; h = 10}, positionedChildren)
        | TerminalWidget(width, height, alignX, alignY, children) -> 
            let cmd = getViewport()

            let positionedChildren = children |> List.map (fun child -> layoutWidget child (Some cmd.SafeWidth) (Some cmd.SafeHeight))

            let childWidths = positionedChildren |> List.map totalWidth
            let childHeights = positionedChildren |> List.map totalHeight

            let maxChildWidth = if List.isEmpty childWidths then 0 else List.max childWidths
            let totalChildHeight = if List.isEmpty childHeights then 0 else List.max childHeights

            let resolvedWidth = resolveDimension width (Some cmd.SafeWidth) maxChildWidth
            let resolvedHeight = resolveDimension height (Some cmd.SafeHeight) totalChildHeight

            let positionedChildren =
                positionedChildren
                |> List.map (fun child ->
                    let childTotalWidth = totalWidth child
                    let childTotalHeight = totalHeight child

                    let xOffset = alignOffset cmd.SafeWidth childTotalWidth alignX
                    let yOffset = alignOffset cmd.SafeHeight childTotalHeight alignY
                    // offsets = (0, 0) com resolvedWidt e resolvedHeight 
                    shiftWidget xOffset yOffset child)
            PositionedTerminalWidget(width, height, alignX, alignY, { x = 0; y = 0; w = cmd.SafeWidth; h = cmd.SafeHeight }, positionedChildren)

    let layoutTree widgets =
        widgets |> List.map (fun widget -> layoutWidget widget None None)
