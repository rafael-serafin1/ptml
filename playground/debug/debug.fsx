#load "../scripting/output.fsx"
open System
open System.IO
open Lexer          (*LEXER*)
open Parser         (*PARSER*)
open Tree           (*AST & SEMANTIC TREE*)
open Layout         (*LAYOUT PASS*)
open Render         (*TREE RENDER*)
open Buffer         (*TERMINAL BUFFER*)
open Diff           (*DIFF ENGINE*)
open Output        

type Terminal = {
    W: int
    H: int
}

let getViewport(): Terminal = 
    let w = Console.WindowWidth - 2
    let h = Console.WindowHeight - 2
    { W = w; H = h }

type Status = 
    | SUCCESS = 0
    | FAILURE = 1

let run(): Status =
    let t: Terminal = getViewport()
    let path: string = "index.ptml"
    let input: string = File.ReadAllText(path)
    let tokens = lex input 0 []
    parser(tokens, [])

    let ast: AstNode list = buildAst(tokens)
    let semantic: Widget list = buildSemanticTree(ast)
    let layout = layoutTree semantic
    let renderOps = renderTree layout
    let buffer = processRenderTree renderOps t.W t.H
    let emptyBuffer = createBuffer t.W t.H

    printfn "Tokens:\n%A" tokens
    printfn "\nAST:\n%A" ast
    printfn "\nSemantic tree:\n%A" semantic
    printfn "\nLayout:\n%A" layout
    printfn "\nRender tree:\n%A" renderOps
    printfn "\nTerminal Buffer (first 6 rows):"
    for y in 0 .. min 7 (Array2D.length1 buffer - 1) do
        for x in 0 .. Array2D.length2 buffer - 1 do
            printf "%s" buffer.[y, x].char
        printfn ""

    printfn "\nDiff from empty screen to current screen:"
    Diff.diffBuffers emptyBuffer buffer
    |> Diff.diffToLines
    |> List.iter (printfn "%s")

    printfn "\nRendering ANSI output to terminal..."
    Console.Write("\x1b[2J\x1b[H")
    Output.printAnsiBuffer buffer
    printfn ""

    Status.SUCCESS

run()