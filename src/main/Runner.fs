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
        let terminal = getViewport()
        let input: string = File.ReadAllText(path)
        if input = "" then  
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
        let baseBuffer = processRenderTree renderOps (terminal.ViewWidth) (terminal.ViewHeight)
        let buffer = Depth.composeDepthLayers baseBuffer depthLayers

        if Utils.shouldWindow = false then
            Console.WindowWidth <- 203
            Console.WindowHeight <- 30
            Console.Write("\x1b[2J\x1b[H")
            Output.printAnsiBuffer(buffer)
            printfn ""
        else
            Console.Write("\x1b[2J\x1b[H")
            Output.printAnsiBuffer(buffer)
            printfn ""
        Status.Success
