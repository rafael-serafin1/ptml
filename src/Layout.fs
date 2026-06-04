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
        | PositionedDepthWidget of index:int * zAlign:Align option * gap:int * metrics:Metrics * children:PositionedWidget list
        | PositionedCellWidget of metrics:Metrics * children:PositionedWidget list
        | PositionedBoxWidget of width:Dimension * height:Dimension * border:Border * borderColor:string option * align:Align option * padding:int * int * metrics:Metrics * children:PositionedWidget list
        | PositionedBlockWidget of width:Dimension * height:Dimension * border:Border * borderColor:string option * name:string option * align:Align option * padding:int * int * metrics:Metrics * children:PositionedWidget list
        | PositionedGridWidget of border:Border * borderColor:string option * metrics: Metrics * children: GridLayout list
        | PositionedTerminalWidget of width:Dimension * height:Dimension * xAlign:Align option * yAlign:Align option * metrics:Metrics * children:PositionedWidget list

    /// GRID TYPES
    and GridCell = {
        col: int
        row: int
        metrics: Metrics
        widget: PositionedWidget
    }

    and GridSeparator =
        | HorizontalSeparator of y:int
        | VerticalSeparator of x:int

    and GridLayout = {
        cols: int
        rows: int
        cells: GridCell list
        separators: GridSeparator list
    }

    //HELPER
    let rec containsCell widgets =
        widgets
        |> List.exists (function
            | CellWidget _ -> true
            | RowWidget(_, _, _, _, children)
            | ColumnWidget(_, _, _, _, children)
            | DepthWidget(_, _, _, children)
            | BoxWidget(_, _, _, _, _, _, _, children)
            | BlockWidget(_, _, _, _, _, _, _, _, children)
            | TerminalWidget(_, _, _, _, children) -> containsCell children
            | _ -> false)

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
            | PositionedCellWidget(metrics, children) ->
                PositionedCellWidget(shiftMetrics metrics, children)
            | PositionedGridWidget(border, borderColor, metrics, children) ->
                let shiftedChildren =
                    children
                    |> List.map (fun grid ->
                        { grid with
                            cells =
                                grid.cells
                                |> List.map (fun cell -> {
                                    cell with
                                        metrics = shiftMetrics cell.metrics
                                        widget = shiftWidget dx dy cell.widget
                                }) })
                PositionedGridWidget(border, borderColor, shiftMetrics metrics, shiftedChildren)
            | PositionedDepthWidget(index, zAlign, gap, metrics, children) ->
                PositionedDepthWidget(index, zAlign, gap, shiftMetrics metrics, children)
            | PositionedBoxWidget(width, height, border, borderColor, align, paddingV, paddingH, metrics, children) ->
                PositionedBoxWidget(width, height, border, borderColor, align, paddingV, paddingH, shiftMetrics metrics, children)
            | PositionedBlockWidget(width, height, border, borderColor, name, align, paddingV, paddingH, metrics, children) ->
                PositionedBlockWidget(width, height, border, borderColor, name, align, paddingV, paddingH, shiftMetrics metrics, children)
            | PositionedTerminalWidget(width, height, xAlign, yAlign, metrics, children) ->
                PositionedTerminalWidget(width, height, xAlign, yAlign, shiftMetrics metrics, children)


    let private alignOffset containerSize childSize alignOption =
        match alignOption with
        | Some Start | None -> 0
        | Some Center -> (containerSize - childSize) / 2
        | Some End -> containerSize - childSize

    let private joinRowsHorizontally rowLists =
        if List.isEmpty rowLists then []
        else
            let maxRows = rowLists |> List.map List.length |> List.max
            [ for rowIndex in 0 .. maxRows - 1 do
                rowLists
                |> List.collect (fun rows -> if rowIndex < List.length rows then rows.[rowIndex] else []) ]

    let rec private collectCellRows widget =
        match widget with
        | CellWidget _ -> [ [ widget ] ]
        | RowWidget(_, _, _, _, children) ->
            children
            |> List.map collectCellRows
            |> joinRowsHorizontally
            |> List.filter (fun row -> not (List.isEmpty row))
        | ColumnWidget(_, _, _, _, children)
        | DepthWidget(_, _, _, children)
        | BoxWidget(_, _, _, _, _, _, _, children)
        | BlockWidget(_, _, _, _, _, _, _, _, children)
        | TerminalWidget(_, _, _, _, children) ->
            children |> List.collect collectCellRows
        | _ -> []

    let private collectCellRowsFromChildren widgets =
        widgets |> List.collect collectCellRows

    let private skipLast list =
        if List.isEmpty list then []
        else List.take (List.length list - 1) list

    let private distributeExtraSpace dimensions extra =
        if extra <= 0 || List.isEmpty dimensions then dimensions
        else
            let allButLast = List.take (List.length dimensions - 1) dimensions
            let last = List.last dimensions
            allButLast @ [ last + extra ]

    let private metricsOf widget =
        match widget with
        | PositionedTextWidget(_, _, _, _, m)
        | PositionedRowWidget(_, _, _, _, m, _)
        | PositionedColumnWidget(_, _, _, _, m, _)
        | PositionedCellWidget(m, _)
        | PositionedGridWidget(_, _, m, _)
        | PositionedBoxWidget(_, _, _, _, _, _, _, m, _)
        | PositionedBlockWidget(_, _, _, _, _, _, _, _, m, _)
        | PositionedTerminalWidget(_, _, _, _, m, _)
        | PositionedDepthWidget(_, _, _, m, _) -> m
    let private totalWidth widget =
        let metrics = metricsOf widget
        match widget with
        | PositionedBoxWidget(_, _, border, _, _, _, _, m, _) when border <> NoBorder ->
            m.w + 2
        | PositionedBlockWidget(_, _, border, _, _, _, _, _, m, _) when border <> NoBorder ->
            m.w + 2
        | PositionedDepthWidget(_, _, _, m, _) -> m.w
        | _ -> metrics.w

    let private totalHeight widget =
        let metrics = metricsOf widget
        match widget with
        | PositionedBoxWidget(_, _, border, _, _, _, _, m, _) when border <> NoBorder ->
            m.h + 2
        | PositionedBlockWidget(_, _, border, _, _, _, _, _, m, _) when border <> NoBorder ->
            m.h + 2
        | PositionedDepthWidget(_, _, _, m, _) -> m.h
        | _ -> metrics.h

    let rec private layoutWidget widget parentWidth parentHeight =
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

        | DepthWidget(index, zAlign, gap, children) ->
            let positionedChildren = children |> List.map (fun child -> layoutWidget child None None)
            let childWidths = positionedChildren |> List.map totalWidth
            let childHeights = positionedChildren |> List.map totalHeight
            let resolvedWidth = if List.isEmpty childWidths then 0 else List.max childWidths
            let resolvedHeight = if List.isEmpty childHeights then 0 else List.max childHeights
            PositionedDepthWidget(index, zAlign, gap, { x = 0; y = 0; w = resolvedWidth; h = resolvedHeight }, positionedChildren)

        | CellWidget(children) ->
            let positionedChildren = children |> List.map (fun child -> layoutWidget child None None)
            let childWidths = positionedChildren |> List.map totalWidth
            let childHeights = positionedChildren |> List.map totalHeight
            let resolvedWidth = if List.isEmpty childWidths then 0 else List.max childWidths
            let resolvedHeight = if List.isEmpty childHeights then 0 else List.max childHeights
            let positionedChildren =
                positionedChildren
                |> List.map (fun child ->
                    let childTotalWidth = totalWidth child
                    let childTotalHeight = totalHeight child
                    let xOffset = alignOffset resolvedWidth childTotalWidth None
                    let yOffset = alignOffset resolvedHeight childTotalHeight None
                    shiftWidget xOffset yOffset child)
            PositionedCellWidget({ x = 0; y = 0; w = resolvedWidth; h = resolvedHeight }, positionedChildren)

        | BoxWidget(width, height, border, borderColor, align, paddingV, paddingH, children) ->
            match layoutCellGrid children width parentWidth height parentHeight border borderColor with
            | Some grid ->
                let gridContentWidth =
                    match grid with
                    | PositionedGridWidget(_, _, m, _) -> m.w
                    | _ -> 0
                let gridContentHeight =
                    match grid with
                    | PositionedGridWidget(_, _, m, _) -> m.h
                    | _ -> 0
                let resolvedContentWidth = resolveDimension width parentWidth gridContentWidth
                let resolvedContentHeight = resolveDimension height parentHeight gridContentHeight
                let positionedGrid = shiftWidget paddingH paddingV grid
                PositionedBoxWidget(width, height, border, borderColor, align, paddingV, paddingH, { x = 0; y = 0; w = resolvedContentWidth + paddingH * 2; h = resolvedContentHeight + paddingV * 2 }, [ positionedGrid ])
            | None ->
                let positionedChildren = children |> List.map (fun child -> layoutWidget child None None)
                let childWidths = positionedChildren |> List.map totalWidth
                let childHeights = positionedChildren |> List.map totalHeight
                let maxChildWidth = if List.isEmpty childWidths then 0 else List.max childWidths
                let totalChildHeight = if List.isEmpty childHeights then 0 else List.max childHeights
                let resolvedContentWidth = resolveDimension width parentWidth maxChildWidth
                let resolvedContentHeight = resolveDimension height parentHeight totalChildHeight
                let availableWidth = max 0 (resolvedContentWidth - paddingH * 2)
                let positionedChildren =
                    positionedChildren
                    |> List.map (fun child ->
                        let childTotalWidth = totalWidth child
                        let xOffset = paddingH + alignOffset availableWidth childTotalWidth align
                        shiftWidget xOffset paddingV child)
                PositionedBoxWidget(width, height, border, borderColor, align, paddingV, paddingH, { x = 0; y = 0; w = resolvedContentWidth + paddingH * 2; h = resolvedContentHeight + paddingV * 2 }, positionedChildren)

        | BlockWidget(width, height, border, borderColor, name, align, paddingV, paddingH, children) ->
            let titleWidth =
                match name with
                | Some t -> t.Length
                | None -> 0
            match layoutCellGrid children width parentWidth height parentHeight border borderColor with
            | Some grid ->
                let gridContentWidth =
                    match grid with
                    | PositionedGridWidget(_, _, m, _) -> m.w
                    | _ -> 0
                let gridContentHeight =
                    match grid with
                    | PositionedGridWidget(_, _, m, _) -> m.h
                    | _ -> 0
                let contentWidth = max gridContentWidth titleWidth
                let resolvedContentWidth = resolveDimension width parentWidth contentWidth
                let resolvedContentHeight = resolveDimension height parentHeight gridContentHeight
                let positionedGrid =
                    match grid with
                    | PositionedGridWidget(_, _, _, _) as g -> g
                    | _ -> grid
                let gridChild = shiftWidget (paddingH + alignOffset resolvedContentWidth gridContentWidth align) paddingV positionedGrid
                PositionedBlockWidget(width, height, border, borderColor, name, align, paddingV, paddingH, { x = 0; y = 0; w = resolvedContentWidth + paddingH * 2; h = resolvedContentHeight + paddingV * 2 }, [ gridChild ])
            | None ->
                let positionedChildren = children |> List.map (fun child -> layoutWidget child None None)
                let childWidths = positionedChildren |> List.map totalWidth
                let childHeights = positionedChildren |> List.map totalHeight
                let maxChildWidth = if List.isEmpty childWidths then 0 else List.max childWidths
                let contentWidth = max maxChildWidth titleWidth
                let totalChildHeight = if List.isEmpty childHeights then 0 else List.max childHeights
                let resolvedContentWidth = resolveDimension width parentWidth contentWidth
                let resolvedContentHeight = resolveDimension height parentHeight totalChildHeight
                let availableWidth = max 0 (resolvedContentWidth - paddingH * 2)
                let positionedChildren =
                    positionedChildren
                    |> List.map (fun child ->
                        let childTotalWidth = totalWidth child
                        let xOffset = paddingH + alignOffset availableWidth childTotalWidth align
                        shiftWidget xOffset paddingV child)
                PositionedBlockWidget(width, height, border, borderColor, name, align, paddingV, paddingH, { x = 0; y = 0; w = resolvedContentWidth + paddingH * 2; h = resolvedContentHeight + paddingV * 2 }, positionedChildren)
        (*| CellWidget(children) ->
            let positionedChildren = children |> List.map (fun child -> layoutWidget child None None)

            let childWidths = positionedChildren |> List.map totalWidth
            let childHeights = positionedChildren |> List.map totalHeight

            let totalChildWidth = if List.isEmpty childWidths then 0 else List.max childWidths
            let totalChildHeight = if List.isEmpty childHeights then 0 else List.max childHeights

            let resolvedWidth = match parentWidth with | Some number -> number | none -> 0
            let resolvedHeight = match parentHeight with | Some number -> number | none -> 0

            let positionedChildren =
                positionedChildren
                |> List.map (fun child ->
                    let childTotalWidth = totalWidth child
                    let childTotalHeight = totalHeight child
                    let xOffset = alignOffset resolvedWidth childTotalWidth None
                    let yOffset = alignOffset resolvedHeight childTotalHeight None
                    shiftWidget xOffset yOffset child)
            PositionedGridWidget({ x = 0; y = 0; w = resolvedWidth; h = resolvedHeight}, positionedChildren)
        *)
        | TerminalWidget(width, height, alignX, alignY, children) -> 
            let cmd = getLayoutViewport()

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
    and private layoutCellGrid children width parentWidth height parentHeight border borderColor =
        let rows = collectCellRowsFromChildren children
        if List.isEmpty rows then None else
        let cols = rows |> List.map List.length |> List.max
        let cellLayouts =
            rows
            |> List.map (fun row ->
                row
                |> List.map (fun cell -> layoutWidget cell None None))

        let columnWidths =
            [ for col in 0 .. cols - 1 do
                let widths =
                    cellLayouts
                    |> List.choose (fun row -> if col < List.length row then Some (totalWidth row.[col]) else None)
                if List.isEmpty widths then 0 else List.max widths ]

        let rowHeights =
            cellLayouts
            |> List.map (fun row ->
                let heights = row |> List.map totalHeight
                if List.isEmpty heights then 0 else List.max heights)

        let separatorCountX = max 0 (cols - 1)
        let separatorCountY = max 0 (List.length rowHeights - 1)
        let baseWidth = List.sum columnWidths + separatorCountX
        let baseHeight = List.sum rowHeights + separatorCountY

        let resolvedWidth = resolveDimension width parentWidth baseWidth
        let resolvedHeight = resolveDimension height parentHeight baseHeight

        let finalColumnWidths =
            let extra = resolvedWidth - baseWidth
            let initial = columnWidths
            distributeExtraSpace initial extra

        let finalRowHeights =
            let extra = resolvedHeight - baseHeight
            distributeExtraSpace rowHeights extra

        let columnOffsets =
            finalColumnWidths
            |> List.mapi (fun index _ ->
                List.sum (List.take index finalColumnWidths) + index)

        let rowOffsets =
            finalRowHeights
            |> List.mapi (fun index _ ->
                List.sum (List.take index finalRowHeights) + index)

        let gridCells =
            cellLayouts
            |> List.mapi (fun rowIndex row ->
                row
                |> List.mapi (fun colIndex cellWidget ->
                    let x = columnOffsets.[colIndex]
                    let y = rowOffsets.[rowIndex]
                    let width = finalColumnWidths.[colIndex]
                    let height = finalRowHeights.[rowIndex]
                    let shifted =
                        let childWidth = totalWidth cellWidget
                        let childHeight = totalHeight cellWidget
                        let xOffset = alignOffset width childWidth None
                        let yOffset = alignOffset height childHeight None
                        shiftWidget xOffset yOffset cellWidget
                    { col = colIndex; row = rowIndex; metrics = { x = x; y = y; w = width; h = height }; widget = shifted }))
            |> List.collect id

        let verticalSeparators =
            finalColumnWidths
            |> skipLast
            |> List.mapi (fun index _ ->
                List.sum (List.take (index + 1) finalColumnWidths) + index)

        let horizontalSeparators =
            finalRowHeights
            |> skipLast
            |> List.mapi (fun index _ ->
                List.sum (List.take (index + 1) finalRowHeights) + index)

        let separators =
            [ yield! verticalSeparators |> List.map VerticalSeparator
              yield! horizontalSeparators |> List.map HorizontalSeparator ]

        let layout = { cols = cols; rows = List.length rowHeights; cells = gridCells; separators = separators }
        Some (PositionedGridWidget(border, borderColor, { x = 0; y = 0; w = resolvedWidth; h = resolvedHeight }, [ layout ]))

    let layoutTree widgets =
        widgets |> List.map (fun widget -> layoutWidget widget None None)
