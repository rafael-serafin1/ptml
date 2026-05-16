#load "output.fsx"
open System
open System.IO
open Lexer          (*LEXER*)
open Parser         (*PARSER*)
open Tree           (*AST & SEMANTIC TREE*)
open Layout         (*LAYOUT PASS*)
open Render         (*TREE RENDER*)
open Buffer         (*TERMINAL BUFFER*)


let run() = 
    let path: string = "index.ptml"
    let input: string = File.ReadAllText(path)
    let tokens = lex input 0 []
    parser(tokens, [])

    let ast: AstNode list = buildAst(tokens)
    let semantic: Widget list = buildSemanticTree(ast)
    let layout = layoutTree semantic
    let renderOps = renderTree layout
    let buffer = processRenderTree renderOps 80 24

    Console.Write("\x1b[2J\x1b[H")
    Output.printAnsiBuffer buffer
    printfn ""

run()