open System
    let rec countArray(array: 'a array) = 
        if Array.isEmpty array then
            0
        else
            1 + countArray array.[1..]
            
    type TitleFormat =
    | Outdoor
    | Banner

    type TitleFont =
    | Common
    | Block

    type TitleOutput = {
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

    let matchHyperFont(str: string): TitleFont =
        match str.ToUpper() with
        | "COMMON"  -> Common
        | "BLOCK"   -> Block 
        | _         -> failwith $"Unknown font for hyper text: {str}" 

    let matchHyperType(str: string): TitleFormat =
        match str.ToUpper() with
        | "OUTDOOR" -> Outdoor
        | "BANNER"  -> Banner
        | _         -> failwith $"Unknown type of hyper text: {str}"

    let private replaceBaseFont(font: TitleFont): Map<char, string array> =
        let baseFont = getBaseFont()

        match font with
        | Common -> baseFont
        | Block  ->
            baseFont 
            |> Map.map (fun _ strA -> 
                    strA 
                    |> Array.map ( fun str -> str.Replace('#', '█') )
                )

    let private handleHyperType(glyphs: string array array) (spacing: int) (t: TitleFormat): string array =
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

    let titleRender(text: string) (spacing: int) (format: string) (fontStyle: string): TitleOutput = 
        if spacing < 0 then
            failwith $"Espaçamento estava negativo: '{spacing}'"

        let orient: TitleFormat = matchHyperType format

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

        let format: TitleOutput = {
            output      = output
            totalWidth  = 
                output
                    |> Array.map String.length
                    |> Array.fold max 0
            totalHeight = output.Length
        }
        format

    [<Obsolete("Do not use. This is for debugging porpouses only.")>]
    let printTitle(text: string) =
        let format = titleRender text 1 "outdoor" "common"
        
        for f in format.output do
            for c in f do
                Console.Write c
            Console.Write "\n"
        
        printfn "%d x %d" format.totalWidth format.totalHeight

printTitle("ELS")