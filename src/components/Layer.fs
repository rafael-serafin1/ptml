namespace PTML

open PTML.Tree
open PTML.Layout
open PTML.Render
open PTML.Buffer
open PTML.Token
open PTML.Messager

module Depth =
    type DepthLayer = {
        Index: int
        ZAlign: Align option
        Gap: int
        Width: int
        Height: int
        OffsetX: int
        OffsetY: int
        Buffer: Cell[,]
    }

    type FrameBuffer = {
        Width: int
        Height: int
        Cells: Cell[,]
    }

    type RenderContext = {
        ViewportWidth: int
        ViewportHeight: int
        Layers: DepthLayer list
    }

    let private isVisibleCell (cell: Cell) =
        cell.char <> ' '

    let private alignOffset containerSize childSize alignOption =
        match alignOption with
        | Some Start -> 0
        | Some Center -> max 0 ((containerSize - childSize) / 2)
        | Some End -> max 0 (containerSize - childSize)
        | None -> 0

    let private renderLayerBuffer width height children =
        let renderOps = children |> Render.renderTree
        Buffer.processRenderTree renderOps width height

    let private validateUniqueIndices (layers: DepthLayer list) =
        let rec loop seen rest =
            match rest with
            | [] -> ()
            | layer :: tail ->
                if Set.contains layer.Index seen then
                    failwith "não pode haver dois filhos com índice de mesmo valor"
                else
                    loop (Set.add layer.Index seen) tail
        loop Set.empty layers

    let private warnMissingSurface layers =
        if not (List.isEmpty layers) && not (List.exists (fun layer -> layer.Index = 0) layers) then
            PTMLMessage("Aviso: a superfície principal não está usando index = 0.", MessageStatus.Warning)

    let rec private extractWidget offsetX offsetY widget =
        match widget with
        | PositionedDepthWidget(index, zAlign, gap, metrics, children) ->
            let absoluteX = offsetX + metrics.x
            let absoluteY = offsetY + metrics.y
            let childResults = children |> List.map (extractWidget absoluteX absoluteY)
            let filteredChildren = childResults |> List.choose fst
            let nestedLayers = childResults |> List.collect snd
            let layerBuffer = renderLayerBuffer metrics.w metrics.h filteredChildren
            let layer = {
                Index = index
                ZAlign = zAlign
                Gap = gap
                Width = metrics.w
                Height = metrics.h
                OffsetX = absoluteX
                OffsetY = absoluteY
                Buffer = layerBuffer
            }
            (None, layer :: nestedLayers)

        | PositionedRowWidget(width, border, gap, align, metrics, children) ->
            let childResults = children |> List.map (extractWidget (offsetX + metrics.x) (offsetY + metrics.y))
            let filteredChildren = childResults |> List.choose fst
            let nestedLayers = childResults |> List.collect snd
            (Some (PositionedRowWidget(width, border, gap, align, metrics, filteredChildren)), nestedLayers)

        | PositionedColumnWidget(width, border, gap, yAlign, metrics, children) ->
            let childResults = children |> List.map (extractWidget (offsetX + metrics.x) (offsetY + metrics.y))
            let filteredChildren = childResults |> List.choose fst
            let nestedLayers = childResults |> List.collect snd
            (Some (PositionedColumnWidget(width, border, gap, yAlign, metrics, filteredChildren)), nestedLayers)

        | PositionedCellWidget(metrics, children) ->
            let childResults = children |> List.map (extractWidget (offsetX + metrics.x) (offsetY + metrics.y))
            let filteredChildren = childResults |> List.choose fst
            let nestedLayers = childResults |> List.collect snd
            (Some (PositionedCellWidget(metrics, filteredChildren)), nestedLayers)

        | PositionedBoxWidget(width, height, border, borderColor, align, paddingV, paddingH, metrics, children) ->
            let childResults = children |> List.map (extractWidget (offsetX + metrics.x) (offsetY + metrics.y))
            let filteredChildren = childResults |> List.choose fst
            let nestedLayers = childResults |> List.collect snd
            (Some (PositionedBoxWidget(width, height, border, borderColor, align, paddingV, paddingH, metrics, filteredChildren)), nestedLayers)

        | PositionedBlockWidget(width, height, border, borderColor, name, align, paddingV, paddingH, metrics, children) ->
            let childResults = children |> List.map (extractWidget (offsetX + metrics.x) (offsetY + metrics.y))
            let filteredChildren = childResults |> List.choose fst
            let nestedLayers = childResults |> List.collect snd
            (Some (PositionedBlockWidget(width, height, border, borderColor, name, align, paddingV, paddingH, metrics, filteredChildren)), nestedLayers)

        | PositionedGridWidget(border, borderColor, metrics, grids) ->
            let processedGrids, nestedLayers =
                grids
                |> List.map (fun grid ->
                    let processedCells, cellNestedLayers =
                        grid.cells
                        |> List.map (fun cell ->
                            let widgetOffsetX = offsetX + metrics.x + cell.metrics.x
                            let widgetOffsetY = offsetY + metrics.y + cell.metrics.y
                            let childResult = extractWidget widgetOffsetX widgetOffsetY cell.widget
                            let cellWidget =
                                match fst childResult with
                                | Some widget -> Some { cell with widget = widget }
                                | None -> None
                            (cellWidget, snd childResult))
                        |> List.unzip
                    let filteredCells = processedCells |> List.choose id
                    ({ grid with cells = filteredCells }, List.collect id cellNestedLayers))
                |> List.unzip
            let filteredGrids = processedGrids
            (Some (PositionedGridWidget(border, borderColor, metrics, filteredGrids)), List.collect id nestedLayers)

        | PositionedTerminalWidget(width, height, alignX, alignY, metrics, children) ->
            let childResults = children |> List.map (extractWidget (offsetX + metrics.x) (offsetY + metrics.y))
            let filteredChildren = childResults |> List.choose fst
            let nestedLayers = childResults |> List.collect snd
            (Some (PositionedTerminalWidget(width, height, alignX, alignY, metrics, filteredChildren)), nestedLayers)

        | PositionedSpinnerWidget(tp, interval, duration, completed, foreground, background, metrics) ->
            (Some (PositionedSpinnerWidget(tp, interval, duration, completed, foreground, background, metrics)), [])

        | PositionedTextWidget(text, foreground, background, font, metrics) ->
            (Some (PositionedTextWidget(text, foreground, background, font, metrics)), [])
        | PositionedFragWidget(text, foreground, background, font, metrics) ->
            (Some (PositionedFragWidget(text, foreground, background, font, metrics)), [])
        | PositionedHrWidget(ori, width, height, metrics) -> 
            (Some (PositionedHrWidget(ori, width, height, metrics)), [])
        | PositionedProgressWidget(tp, value, maxi, w, h, str, metrics) ->
            (Some (PositionedProgressWidget(tp, value, maxi, w, h, str, metrics)), [])
        | PositionedEscapeWidget(seq, multi, metrics) ->
            (Some (PositionedEscapeWidget(seq, multi, metrics)), [])

    let extractDepthLayers widgets =
        let results = widgets |> List.map (extractWidget 0 0)
        let filteredWidgets = results |> List.choose fst
        let layers = results |> List.collect snd
        validateUniqueIndices layers
        warnMissingSurface layers
        (filteredWidgets, layers)

    let composeDepthLayers (baseBuffer: Cell[,]) (layers: DepthLayer list) =
        let sortedLayers = layers |> List.sortBy (fun layer -> layer.Index)
        sortedLayers
        |> List.iter (fun layer ->
            let bufferWidth = Array2D.length2 baseBuffer
            let xOffset = layer.OffsetX + layer.Gap + alignOffset bufferWidth layer.Width layer.ZAlign
            let yOffset = layer.OffsetY + layer.Gap
            for y in 0 .. layer.Height - 1 do
                for x in 0 .. layer.Width - 1 do
                    let cell = layer.Buffer.[y, x]
                    if isVisibleCell cell then
                        setCell baseBuffer (xOffset + x) (yOffset + y) cell.char cell.foreground cell.background cell.font
            )
        baseBuffer

    let toFrameBuffer (buffer: Cell[,]) =
        { Width = Array2D.length2 buffer; Height = Array2D.length1 buffer; Cells = buffer }

    let createRenderContext width height layers =
        { ViewportWidth = width; ViewportHeight = height; Layers = layers }
