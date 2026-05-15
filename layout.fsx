module Layout
#load "tree.fsx"
open Tree

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

let private charWidth = 1
let private lineHeight = 1

let private resolveDimension width parentSize fallback =
    match width with
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
        PositionedRowWidget(width, border, gap, align, shiftMetrics metrics, children |> List.map (shiftWidget dx dy))
    | PositionedColumnWidget(width, border, gap, yAlign, metrics, children) ->
        PositionedColumnWidget(width, border, gap, yAlign, shiftMetrics metrics, children |> List.map (shiftWidget dx dy))
    | PositionedBoxWidget(width, height, border, borderColor, align, metrics, children) ->
        PositionedBoxWidget(width, height, border, borderColor, align, shiftMetrics metrics, children |> List.map (shiftWidget dx dy))

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
        | PositionedBoxWidget(_, _, _, _, _, m, _) -> m

    match widget with
    (* Layout logic for each widget type *)
    (* Entra em loop até chegar aqui     *)
    | TextWidget(text, fg, bg, font) ->
        let w = text.Length * charWidth
        let h = lineHeight
        PositionedTextWidget(text, fg, bg, font, { x = 0; y = 0; w = w; h = h })

    | RowWidget(width, border, gap, align, children) ->
        let positionedChildren = children |> List.map (fun child -> layoutWidget child None None)
        let childWidths = positionedChildren |> List.map (fun child -> (metricsOf child).w)
        let childHeights = positionedChildren |> List.map (fun child -> (metricsOf child).h)
        let totalChildWidth = if List.isEmpty childWidths then 0 else List.sum childWidths + gap * (List.length childWidths - 1)
        let maxChildHeight = if List.isEmpty childHeights then 0 else List.max childHeights
        let resolvedWidth = resolveDimension width parentWidth totalChildWidth
        let resolvedHeight = max maxChildHeight lineHeight
        let positionedChildren =
            let rec place children xOffset acc =
                match children with
                | [] -> List.rev acc
                | child :: rest ->
                    let childMetrics = metricsOf child
                    let yOffset = alignOffset resolvedHeight childMetrics.h align
                    let positioned = shiftWidget xOffset yOffset child
                    let nextX = xOffset + childMetrics.w + gap
                    place rest nextX (positioned :: acc)
            place positionedChildren 0 []
        PositionedRowWidget(width, border, gap, align, { x = 0; y = 0; w = resolvedWidth; h = resolvedHeight }, positionedChildren)

    | ColumnWidget(width, border, gap, yAlign, children) ->
        let positionedChildren = children |> List.map (fun child -> layoutWidget child None None)
        let childWidths = positionedChildren |> List.map (fun child -> (metricsOf child).w)
        let childHeights = positionedChildren |> List.map (fun child -> (metricsOf child).h)
        let maxChildWidth = if List.isEmpty childWidths then 0 else List.max childWidths
        let totalChildHeight = if List.isEmpty childHeights then 0 else List.sum childHeights + gap * (List.length childHeights - 1)
        let resolvedWidth = resolveDimension width parentWidth maxChildWidth
        let resolvedHeight = resolveDimension (Fixed totalChildHeight) parentHeight totalChildHeight
        let positionedChildren =
            let rec place children yOffset acc =
                match children with
                | [] -> List.rev acc
                | child :: rest ->
                    let childMetrics = metricsOf child
                    let xOffset = alignOffset resolvedWidth childMetrics.w yAlign
                    let positioned = shiftWidget xOffset yOffset child
                    let nextY = yOffset + childMetrics.h + gap
                    place rest nextY (positioned :: acc)
            place positionedChildren 0 []
        PositionedColumnWidget(width, border, gap, yAlign, { x = 0; y = 0; w = resolvedWidth; h = resolvedHeight }, positionedChildren)

    | BoxWidget(width, height, border, borderColor, align, children) ->
        let positionedChildren = children |> List.map (fun child -> layoutWidget child None None)
        let childWidths = positionedChildren |> List.map (fun child -> (metricsOf child).w)
        let childHeights = positionedChildren |> List.map (fun child -> (metricsOf child).h)
        let maxChildWidth = if List.isEmpty childWidths then 0 else List.max childWidths
        let totalChildHeight = if List.isEmpty childHeights then 0 else List.sum childHeights
        let resolvedWidth = resolveDimension width parentWidth maxChildWidth
        let resolvedHeight = resolveDimension height parentHeight totalChildHeight
        let positionedChildren =
            positionedChildren
            |> List.map (fun child ->
                let childMetrics = metricsOf child
                let xOffset = alignOffset resolvedWidth childMetrics.w align
                shiftWidget xOffset 0 child)
        PositionedBoxWidget(width, height, border, borderColor, align, { x = 0; y = 0; w = resolvedWidth; h = resolvedHeight }, positionedChildren)

let layoutTree widgets =
    widgets |> List.map (fun widget -> layoutWidget widget None None)
