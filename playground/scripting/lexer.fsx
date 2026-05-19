#load "token.fsx"
open Token

let rec lex (input: string) (pos: int) (acc: list<LexToken>) : list<LexToken> =
    if pos >= input.Length then acc |> List.rev
    else
        match input.[pos] with
        | '<' ->
            if pos + 1 < input.Length then
                match input.[pos + 1] with
                | '?' ->
                    // processing instruction
                    let start = pos
                    let mutable endPos = pos + 2
                    while endPos + 1 < input.Length && not (input.[endPos] = '?' && input.[endPos + 1] = '>') do
                        endPos <- endPos + 1
                    if endPos + 1 < input.Length then
                        endPos <- endPos + 2
                    let pi = input.[start..endPos - 1]
                    lex input endPos (ProcInst pi :: acc)
                | '/' ->
                    // end tag
                    let start = pos + 2
                    let mutable endPos = start
                    while endPos < input.Length && input.[endPos] <> '>' do
                        endPos <- endPos + 1
                    let tag = input.[start..endPos - 1].Trim()
                    lex input (endPos + 1) (EndTag tag :: acc)
                | '!' when pos + 3 < input.Length && input.[pos + 2] = '-' && input.[pos + 3] = '-' ->
                    // comment
                    let start = pos
                    let mutable endPos = pos + 4
                    while endPos < input.Length && not (input.[endPos] = '-' && endPos + 2 < input.Length && input.[endPos + 1] = '-' && input.[endPos + 2] = '>') do
                        endPos <- endPos + 1
                    endPos <- endPos + 3
                    let comment = input.[start..endPos - 1]
                    lex input endPos (Comment comment :: acc)
                | _ ->
                    // start tag
                    let mutable tagPos = pos + 1
                    let mutable tagName = ""
                    while tagPos < input.Length && input.[tagPos] <> ' ' && input.[tagPos] <> '>' && input.[tagPos] <> '/' do
                        tagName <- tagName + string input.[tagPos]
                        tagPos <- tagPos + 1
                    let mutable attrs = []
                    while tagPos < input.Length && input.[tagPos] <> '>' && input.[tagPos] <> '/' do
                        if input.[tagPos] = ' ' then
                            tagPos <- tagPos + 1
                        else
                            let attrStart = tagPos
                            while tagPos < input.Length && input.[tagPos] <> '=' do
                                tagPos <- tagPos + 1
                            let attrName = input.[attrStart..tagPos - 1].Trim()
                            tagPos <- tagPos + 1
                            if tagPos < input.Length && input.[tagPos] = '"' then
                                tagPos <- tagPos + 1
                                let valueStart = tagPos
                                while tagPos < input.Length && input.[tagPos] <> '"' do
                                    tagPos <- tagPos + 1
                                let attrValue = input.[valueStart..tagPos - 1]
                                tagPos <- tagPos + 1
                                attrs <- (attrName, attrValue) :: attrs
                            else
                                let valueStart = tagPos
                                while tagPos < input.Length && input.[tagPos] <> ' ' && input.[tagPos] <> '>' && input.[tagPos] <> '/' do
                                    tagPos <- tagPos + 1
                                let attrValue = input.[valueStart..tagPos - 1]
                                attrs <- (attrName, attrValue) :: attrs
                    let selfClosing = tagPos < input.Length && input.[tagPos] = '/'
                    // skip '/'
                    if selfClosing then tagPos <- tagPos + 1
                    // skip '>'
                    if tagPos < input.Length && input.[tagPos] = '>' then tagPos <- tagPos + 1
                    // validate tag name
                    lex input tagPos (StartTag (tagName, selfClosing, attrs |> List.rev) :: acc)
            else
                // invalid, treat as text
                lex input (pos + 1) (Text (string input.[pos]) :: acc)
        | _ ->
            // text
            let start = pos
            let mutable endPos = pos
            while endPos < input.Length && input.[endPos] <> '<' do
                endPos <- endPos + 1
            let text = input.[start..endPos - 1]
            lex input endPos (Text text :: acc)