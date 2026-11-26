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
        if input = "" || input = String.Empty then  
            printfn "\x1b[31mError\x1b[0m -- PTML File is empty."
            Status.Error
        else

        let tokens = lex input 0 []
        printfn "Tokens: %A" tokens
        parser(tokens, [])

        let ast: AstNode list = buildAst(tokens)
        printfn "AST: %A" ast

        let dom = buildDom ast
        printfn "DOM: %A" dom

        let semantic: Widget list = buildSemanticTreeFromDom dom
        printfn "Semantic Tree: %A" semantic

        let layout = layoutTree semantic
        printfn "Layout Pass: %A" layout

        let filteredLayout, depthLayers = Depth.extractDepthLayers layout
        printfn "Filtered Layout Pass: %A" filteredLayout
        printfn "Depth Layers: %A" depthLayers

        let renderOps = renderTree filteredLayout
        
        ContentExecution.tryExecute ast
        printfn "Render Tree: %A" renderOps

        let baseBuffer = processRenderTree renderOps (terminal.ViewWidth) (terminal.ViewHeight)
        for y = 0 to 1 do
            for x = 0 to 20 do
                printfn "Buffer: %A" baseBuffer[y, x]
        let buffer = Depth.composeDepthLayers baseBuffer depthLayers

        if Utils.shouldWindow = false then
            Console.WindowWidth <- 203
            Console.WindowHeight <- 30

        Console.Write("\x1b[2J\x1b[H")
        Output.writeAnsiBuffer(buffer)
        printfn ""
        Status.Success
