namespace PTML

open System
open PTML.Utils

module Hyper =
    type HyperFormat =
    | Outdoor
    | Banner

    type HyperFont =
    | Common
    | Block

    type HyperOutput = {
        output:         string array
        totalWidth:     int
        totalHeight:    int
    }

    let private defaultH = 5
    let private defaultW = 5

    let private blank = Array.create defaultH (String.replicate defaultW " ")

    let private getBaseFont(): Map<char, string array> = 
        Map.ofList [
            'A', [| " ### "; "#   #"; "#####"; "#   #"; "#   #" |]
            'B', [| "#### "; "#   #"; "#### "; "#   #"; "#### " |]
            'C', [| " ####"; "#    "; "#    "; "#    "; " ####" |]
            'D', [| "#### "; "#   #"; "#   #"; "#   #"; "#### " |]
            'E', [| "#####"; "#    "; "#### "; "#    "; "#####" |]
            'F', [| "#####"; "#    "; "#### "; "#    "; "#    " |]
            'G', [| " ####"; "#    "; "# ###"; "#   #"; " ####" |]
            'H', [| "#   #"; "#   #"; "#####"; "#   #"; "#   #" |]
            'I', [| "#####"; "  #  "; "  #  "; "  #  "; "#####" |]
            'J', [| "#####"; "   # "; "   # "; "#  # "; " ##  " |]
            'K', [| "#   #"; "#  # "; "###  "; "#  # "; "#   #" |]
            'L', [| "#    "; "#    "; "#    "; "#    "; "#####" |]
            'M', [| "#   #"; "## ##"; "# # #"; "#   #"; "#   #" |]
            'N', [| "#   #"; "##  #"; "# # #"; "#  ##"; "#   #" |]
            'O', [| " ### "; "#   #"; "#   #"; "#   #"; " ### " |]
            'P', [| "#### "; "#   #"; "#### "; "#    "; "#    " |]
            'Q', [| " ### "; "#   #"; "# # #"; "#  ##"; " ####" |]
            'R', [| "#### "; "#   #"; "#### "; "#  # "; "#   #" |]
            'S', [| " ####"; "#    "; " ### "; "    #"; "#### " |]
            'T', [| "#####"; "  #  "; "  #  "; "  #  "; "  #  " |]
            'U', [| "#   #"; "#   #"; "#   #"; "#   #"; " ### " |]
            'V', [| "#   #"; "#   #"; "#   #"; " # # "; "  #  " |]
            'W', [| "#   #"; "#   #"; "# # #"; "## ##"; "#   #" |]
            'X', [| "#   #"; " # # "; "  #  "; " # # "; "#   #" |]
            'Y', [| "#   #"; " # # "; "  #  "; "  #  "; "  #  " |]
            'Z', [| "#####"; "   # "; "  #  "; " #   "; "#####" |]

            '0', [| " ### "; "#  ##"; "# # #"; "##  #"; " ### " |]
            '1', [| "  #  "; " ##  "; "  #  "; "  #  "; " ### " |]
            '2', [| " ### "; "#   #"; "   # "; "  #  "; "#####" |]
            '3', [| "#### "; "    #"; " ### "; "    #"; "#### " |]
            '4', [| "#   #"; "#   #"; "#####"; "    #"; "    #" |]
            '5', [| "#####"; "#    "; "#### "; "    #"; "#### " |]
            '6', [| " ### "; "#    "; "#### "; "#   #"; " ### " |]
            '7', [| "#####"; "    #"; "   # "; "  #  "; "  #  " |]
            '8', [| " ### "; "#   #"; " ### "; "#   #"; " ### " |]
            '9', [| " ### "; "#   #"; " ####"; "    #"; " ### " |]

            ' ', blank

            '?', [| " ### "; "#   #"; "   # "; "     "; "  #  " |]
            '!', [| "  #  "; "  #  "; "  #  "; "     "; "  #  " |]
            '-', [| "     "; "     "; "#####"; "     "; "     " |]
            '.', [| "     "; "     "; "     "; "     "; "  #  " |]
        ]

    let matchHyperFont(str: string): HyperFont =
        match str.ToUpper() with
        | "COMMON"  -> Common
        | "BLOCK"   -> Block 
        | _         -> failwith $"Unknown font for hyper text: {str}" 

    let matchHyperType(str: string): HyperFormat =
        match str.ToUpper() with
        | "OUTDOOR" -> Outdoor
        | "BANNER"  -> Banner
        | _         -> failwith $"Unknown type of hyper text: {str}"

    let private replaceBaseFont(font: HyperFont): Map<char, string array> =
        let baseFont = getBaseFont()

        match font with
        | Common -> baseFont
        | Block  ->
            baseFont 
            |> Map.map (fun _ strA -> 
                    strA 
                    |> Array.map ( fun str -> str.Replace('#', '█') )
                )

    let private handleHyperType(glyphs: string array array) (spacing: int) (t: HyperFormat): string array =
        match t with
        | Outdoor ->
            let gap = String.replicate spacing " "

            Array.init defaultH (fun row ->
                glyphs
                |> Array.map (fun glyph -> glyph.[row])
                |> String.concat gap)
        | Banner ->
            let gap = Array.create spacing ""

            glyphs
            |> Array.collect (fun glyph ->
                Array.append glyph gap)

    let HyperRender(text: string) (spacing: int) (format: string) (fontStyle: string): HyperOutput = 
        if spacing < 0 then
            failwith $"Espaçamento estava negativo: '{spacing}'"

        let orient: HyperFormat = matchHyperType format

        let font: Map<char, string array> = 
            matchHyperFont fontStyle 
            |> replaceBaseFont
            

        let glyphs = 
            text.ToUpperInvariant() 
            |> Seq.map (fun c -> 
                Map.tryFind c font
                |> Option.defaultValue font.['?'])
            |> Seq.toArray

        let output = handleHyperType glyphs spacing orient

        let format: HyperOutput = {
            output      = output
            totalWidth  = 
                output
                    |> Array.map String.length
                    |> Array.fold max 0
            totalHeight = output.Length
        }

        format

    [<Obsolete("Do not use. This is for debugging porpouses only.")>]
    let printHyper(text: string) =
        let format = HyperRender text 1 "outdoor" "block"
        
        for f in format.output do
            for c in f do
                Console.Write c
            Console.Write "\n"
        
        printfn "%d x %d" format.totalWidth format.totalHeight
