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
        if Utils.shouldWindow then
            terminal <- Some (basicTerminal)
        else
            terminal <- Some (getViewport())

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
        match terminal with 
        | None -> 
            Status.Error
        | Some t ->
        let baseBuffer = processRenderTree renderOps (t.ViewWidth) (t.ViewHeight)
        let buffer: Cell array2d = Depth.composeDepthLayers baseBuffer depthLayers

        if Utils.shouldWindow = false then
            Console.WindowWidth <- 203
            Console.WindowHeight <- 30
        Console.Write("\x1b[2J\x1b[H")
        Output.printAnsiBuffer(buffer)
        for y = 0 to buffer.GetLength(0) - 1 do
            for x = 0 to buffer.GetLength(0) - 1 do
                let cell = buffer[y, x]
                match cell.spinner with
                | Some c -> 
                    Spinner.threadDraw(c.tp, x, y, c.interval, c.dur, c.complete)
                | None -> ()
        printfn ""
        Status.Success
