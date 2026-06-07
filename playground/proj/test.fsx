open System
open System.Threading
open System.Diagnostics

type Types =
    | Braille
    | Dots
    | Waiting
    | Beam
    | Ascii
    | Moon
    | Circle
    | Square
    | Arrow
    | Bounce

let ParseInterval(s: string) =
    let value =
        if String.IsNullOrWhiteSpace(s) then
            250
        elif s.EndsWith("ms") then
            s.Substring(0, s.Length - 2) |> int
        elif s.EndsWith("s") then
            s.Substring(0, s.Length - 1) |> int |> (*) 1000
        else
            failwith "Invalid interval format. Use 'ms' or 's'."

    match value with
    | v when v < 0 -> abs v
    | 0 -> 250
    | v -> v

let framesOfType tp =
    match tp with
    | Braille ->
        [|
            "⠋"; "⠙"; "⠹"; "⠸"; "⠼"
            "⠴"; "⠦"; "⠧"; "⠇"; "⠏"
        |]
    | Dots ->
        [|
            "⣾"; "⣽"; "⣻"; "⢿"
            "⡿"; "⣟"; "⣯"; "⣷"
        |]
    | Waiting -> [| "."; ".."; "..."; "...." |]
    | Beam -> [| "[=  ]"; "[== ]"; "[===]"; "[ ==]"; "[  =]" |]
    | Ascii -> [| "|"; "/"; "-"; "\\" |]
    | Circle -> [| "◐"; "◓"; "◑"; "◒" |]
    | Square -> [| "◰"; "◳"; "◲"; "◱" |]
    | Moon -> [| "◜"; "◝"; "◞"; "◟" |]
    | Arrow -> [| "←"; "↖"; "↑"; "↗"; "→"; "↘"; "↓"; "↙" |]
    | Bounce -> [| "⠁"; "⠂"; "⠄"; "⠂" |]

type Duration =
    {
        tp: string
        value: int
    }

let ParseDuration(s: string, interval: string, frames: string array) =
    if String.IsNullOrWhiteSpace(s) then
        {
            tp = "ms"
            value = 3000
        }
    elif s.EndsWith("ms") then
        {
            tp = "ms"
            value = s.Substring(0, s.Length - 2) |> int
        }
    elif s.EndsWith("s") && not(s.EndsWith("laps")) then
        {
            tp = "ms"
            value = s.Substring(0, s.Length - 1) |> int |> (*) 1000
        }
    elif s.EndsWith("laps") then
        let laps = s.Substring(0, s.Length - 4) |> int
        {
            tp = "laps"
            value = laps
        }
    else
        failwith "Invalid duration format. Use 'ms', 's' or 'laps'."

let drawSpinner(tp: Types, x, y, inter: string, dur: string, complete: string, fg, bg) =
    let frames = framesOfType tp
    let duration = ParseDuration(dur, inter, frames)
    let intervalMs = ParseInterval(inter)

    Console.SetCursorPosition(x, y)
    let stopwatch = Stopwatch.StartNew()

    let mutable frameIndex = 0
    let mutable laps = 0
    let mutable running = true

    while running do
        Console.SetCursorPosition(x, y)
        Console.Write(frames[frameIndex])

        Thread.Sleep(intervalMs)

        frameIndex <- frameIndex + 1

        if frameIndex >= frames.Length then
            frameIndex <- 0
            laps <- laps + 1

        match duration.tp with
        | "ms" ->
            if stopwatch.ElapsedMilliseconds >= int64 duration.value then
                running <- false
        | "laps" ->
            if laps >= duration.value then
                running <- false
        | _ -> ()
    stopwatch.Stop()

    Console.SetCursorPosition(x, y)

    if String.IsNullOrWhiteSpace(complete) then
        Console.Write(" ")
    else
        Console.Write(complete)


drawSpinner(Types.Dots, 0, 0, "100ms", "3s", "✓", "b", "b")
drawSpinner(Types.Arrow, 0, 1, "150ms", "5laps", "Done", "b", "b")
Environment.Exit(0)