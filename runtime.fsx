
#load "parser.fsx"
#load "layout.fsx"
open System.IO
open Parser
open Lexer
open Tree
open Layout

let run() =
    let path: string = "index.ptml"
    let input: string = File.ReadAllText(path)
    let tokens = lex input 0 []
    parser(tokens, [])

    let ast: AstNode list = buildAst(tokens)
    let semantic: Widget list = buildSemanticTree(ast)
    let layout = layoutTree semantic


    printfn "Tokens:\n%A" tokens
    printfn "\nAST:\n%A" ast
    printfn "\nSemantic tree:\n%A" semantic
    printfn "\nLayout:\n%A" layout
run()