#load "parser.fsx"
#load "diff.fsx"
open System.IO
open Lexer          (*LEXER*)
open Parser         (*PARSER*)
open Tree           (*AST & SEMANTIC TREE*)
open Layout         (*LAYOUT PASS*)
open Render         (*TREE RENDER*)
open Buffer         (*TERMINAL BUFFER*)
open Diff           (*DIFF ENGINNE*)

type Status = 
    | SUCCESS = 0
    | FAILURE = 1

let run(): Status =
    let path: string = "index.ptml"
    let input: string = File.ReadAllText(path)
    let tokens = lex input 0 []
    parser(tokens, [])

    let ast: AstNode list = buildAst(tokens)
    let semantic: Widget list = buildSemanticTree(ast)
    let layout = layoutTree semantic
    let renderOps = renderTree layout
    let buffer = processRenderTree renderOps 80 24

    printfn "Tokens:\n%A" tokens
    printfn "\nAST:\n%A" ast
    printfn "\nSemantic tree:\n%A" semantic
    printfn "\nLayout:\n%A" layout
    printfn "\nRender tree:\n%A" renderOps
    printfn "\nTerminal Buffer (first 5 rows):"
    for y in 0 .. min 6 (Array2D.length1 buffer - 1) do
        for x in 0 .. Array2D.length2 buffer - 1 do
            printf "%s" buffer.[y, x].char
        printfn ""
    Status.SUCCESS
run()