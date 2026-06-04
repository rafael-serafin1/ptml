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

        let renderOps = renderTree layout
        printfn "Render Tree: %A" renderOps

        let buffer = processRenderTree renderOps (terminal.ViewWidth) (terminal.ViewHeight)

        Console.Write("\x1b[2J\x1b[H")
        Output.printAnsiBuffer(buffer)
        printfn ""
        Status.Success
