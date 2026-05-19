#load "./output.fsx"
open System
open System.IO
open Lexer          (*LEXER*)
open Parser         (*PARSER*)
open Tree           (*AST & SEMANTIC TREE*)
open Layout         (*LAYOUT PASS*)
open Render         (*TREE RENDER*)
open Buffer         (*TERMINAL BUFFER*)

type Terminal = {
    W: int
    H: int
}

let getViewport(): Terminal = 
    let w = Console.WindowWidth - 2
    let h = Console.WindowHeight - 2
    { W = w; H = h }

let run() = 
    let t: Terminal = getViewport()
    let path: string array = Environment.GetCommandLineArgs()
    let input: string = File.ReadAllText(path.[2])
    let tokens = lex input 0 []
    parser(tokens, [])

    let ast: AstNode list = buildAst(tokens)
    let semantic: Widget list = buildSemanticTree(ast)
    let layout = layoutTree semantic
    let renderOps = renderTree layout
    let buffer = processRenderTree renderOps t.W t.H

    Console.Write("\x1b[2J\x1b[H")
    Output.printAnsiBuffer buffer
    printfn ""

run()