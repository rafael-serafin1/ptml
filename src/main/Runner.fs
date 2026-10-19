namespace PTML
open System
open System.Text
open System.IO
open PTML.Token
open PTML.Lexer
open PTML.Parser
open PTML.Tree
open PTML.Layout
open PTML.Buffer
open PTML.Render
open PTML.Depth

module Runner =
    let run(path: string): Status =
        let mutable terminal: Terminal option = None

        let input: string = File.ReadAllText(path)
        if input = "" || input = String.Empty then  
            printfn "\x1b[31mError\x1b[0m -- PTML File is empty."
            Status.Error
        else

        let tokens = lex input 0 []
        parser(tokens, [])

        let ast: AstNode list = buildAst(tokens)
        let semantic: Widget list = buildSemanticTree(ast)
        let layout = layoutTree semantic
        let filteredLayout, depthLayers = Depth.extractDepthLayers layout
        let renderOps = renderTree filteredLayout

        let T = getViewport()
        if Utils.shouldWindow then
            terminal <- Some (basicTerminal)
        else
            terminal <- Some (getViewport())
            
        match terminal with 
        | None -> 
            Status.Error
        | Some t ->
        let requiredWidth, requiredHeight = Buffer.requiredSize 0 0 renderOps
        let overflowsViewport =
            requiredWidth > t.SafeWidth || requiredHeight > t.SafeHeight
        let bufferWidth =
            if overflowsViewport then max 1 requiredWidth else t.ViewWidth
        let bufferHeight =
            if overflowsViewport then max 1 requiredHeight else t.ViewHeight
        let baseBuffer = processRenderTree renderOps bufferWidth bufferHeight
        let buffer: Cell array2d = Depth.composeDepthLayers baseBuffer depthLayers

        if overflowsViewport then
            Output.writeScrollingBuffer t.SafeWidth buffer
        else
            if Utils.shouldWindow = false then
                Console.WindowWidth <- T.SafeWidth
                Console.WindowHeight <- T.SafeHeight

            Console.Write("\x1b[2J\x1b[H")
            Output.writeAnsiBuffer(buffer)
            for y = 0 to buffer.GetLength(0) - 1 do
                for x = 0 to buffer.GetLength(1) - 1 do
                    let cell = buffer[y, x]
                    match cell.spinner with
                    | Some c ->
                        Spinner.threadDraw(c.tp, x, y, c.interval, c.dur, c.complete)
                    | None -> ()
            printfn ""
        Status.Success
