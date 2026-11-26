namespace PTML
open PTML.Token

module ContentExecution =

    let private bufferToText (buffer: Buffer.Cell[,]) =
        [|
            for y in 0 .. Array2D.length1 buffer - 1 do
                yield System.String(Array.init (Array2D.length2 buffer) (fun x -> buffer[y, x].char))
        |] |> String.concat "\n"

    let private renderElement node =
        let widgets = Tree.buildWidget node
        let positioned = Layout.layoutTree widgets
        let visibleWidgets, depthLayers = Depth.extractDepthLayersForContent positioned
        let renderOperations = Render.renderTreeForContent visibleWidgets
        let width, height = Layout.renderedSize visibleWidgets
        let width, height = Buffer.requiredSize width height renderOperations
        let buffer = Buffer.processRenderTree renderOperations width height
        Depth.composeDepthLayers buffer depthLayers |> bufferToText

    let execute ast =
        let rec executeNode node =
            match node with
            | TextNode _ -> ()
            | Element(_, globalAttributes, _, children) ->
                match globalAttributes.Content with
                | Some contentType ->
                    let content = renderElement node
                    Content.parseAndExecute (string contentType) content
                | None -> ()

                children |> List.iter executeNode

        ast |> List.iter executeNode

    let tryExecute ast =
        if Utils.SEC then execute ast
        else ()
