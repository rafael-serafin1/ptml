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

module Runner =
    let run(path: string): Status =
        let terminal = getViewport()
        let input: string = File.ReadAllText(path)

        let tokens = lex input 0 []
        parser(tokens, [])

        let ast: AstNode list = buildAst(tokens)
        let semantic: Widget list = buildSemanticTree(ast)
        let layout = layoutTree semantic
        let renderOps = renderTree layout
        let buffer = processRenderTree renderOps (terminal.ViewWidth) (terminal.ViewHeight)

        Console.Write("\x1b[2J\x1b[H")
        Output.printAnsiBuffer(buffer)
        printfn ""
        Status.Success