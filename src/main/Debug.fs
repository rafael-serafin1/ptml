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

module Debug =
    let debug(path: string): Status = 
        let terminal: Terminal = getOutputViewport()
        let input: string = File.ReadAllText(path)
        if input = "" then  
            printfn "\x1b[31mError\x1b[0m -- PTML File is empty."
            Status.Error
        else

        let tokens = lex input 0 []
        printfn "Tokens: %A" tokens
        parser(tokens, [])

        let ast: AstNode list = buildAst(tokens)
        printfn "AST: %A" ast

        let semantic: Widget list = buildSemanticTree(ast)
        printfn "Semantic Tree: %A" semantic

        let layout = layoutTree semantic
        printfn "Layout Pass: %A" layout

        let filteredLayout, depthLayers = Depth.extractDepthLayers layout
        printfn "Filtered Layout Pass: %A" filteredLayout
        printfn "Depth Layers: %A" depthLayers

        let renderOps = renderTree filteredLayout
        printfn "Render Tree: %A" renderOps

        let baseBuffer = processRenderTree renderOps (terminal.ViewWidth) (terminal.ViewHeight)
        let buffer = Depth.composeDepthLayers baseBuffer depthLayers

        if Utils.shouldWindow = false then
            Console.WindowWidth <- 203
            Console.WindowHeight <- 30

        Console.Write("\x1b[2J\x1b[H")
        Output.writeAnsiBuffer(buffer)
        for y = 0 to buffer.GetLength(0) - 1 do
            for x = 0 to buffer.GetLength(0) - 1 do
                let cell = buffer[y, x]
                match cell.spinner with
                | Some c -> 
                    Spinner.threadDraw(c.tp, x, y, c.interval, c.dur, c.complete)
                | None -> ()
        printfn ""
        Status.Success
