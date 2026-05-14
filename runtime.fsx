#load "token.fsx"
#load "parser.fsx"
open System.IO
open Parser
open Lexer

let run() =
    let path: string = "index.ptml"
    let input: string = File.ReadAllText(path)
    let tokens = lex input 0 []
    parser(tokens, [])

    let ast = buildAst(tokens)
    let semantic = buildSemanticTree(ast)

    printfn "Tokens:\n%A" tokens
    printfn "\nAST:\n%A" ast
    printfn "\nSemantic tree:\n%A" semantic

run()