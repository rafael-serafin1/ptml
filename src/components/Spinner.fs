namespace PTML
open System
open System.Threading
open System.Diagnostics
open System.Collections.Concurrent

module Spinner = 
    type Types =
    | Braille
    | Dots
    | Waiting
    | Burger
    | Beam
    | Ascii
    | Moon
    | Circle
    | Square
    | Arrow
    | Bounce

    let private consoleLock = obj()

    let private writeAt x y (text: string) =
        lock consoleLock (fun () ->
            Console.SetCursorPosition(x, y)
            Console.Write(text)
        )

    let private clearAt x y =
        lock consoleLock (fun () ->
            Console.SetCursorPosition(x, y)
            Console.Write(" ")
        )

    type SpinnerInstance = {
        Tp: Types
        X: int
        Y: int
        Interval: string
        Duration: string
        Complete: string
        mutable FrameIndex: int
        mutable Laps: int
        Stopwatch: Stopwatch
    }

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
        | Burger -> [| "-"; "="; "≡" |]
        | Beam -> [| "[=  ]"; "[== ]"; "[===]"; "[ ==]"; "[  =]" |]
        | Ascii -> [| "|"; "/"; "-"; "\\" |]
        | Circle -> [| "◐"; "◓"; "◑"; "◒" |]
        | Square -> [| "◰"; "◳"; "◲"; "◱" |]
        | Moon -> [| "◜"; "◝"; "◞"; "◟" |]
        | Arrow -> [| "←"; "↖"; "↑"; "↗"; "→"; "↘"; "↓"; "↙" |]
        | Bounce -> [| "⠁"; "⠂"; "⠄"; "⠂" |]

    let firstFrame tp =
        let fm = framesOfType tp
        fm[0]

    let maxFrameWidth(tp: Types) =
        framesOfType tp
        |> Array.map (fun frame -> frame.Length)
        |> Array.max

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

    let CleanSpinner(length: int, x: int, y: int) =
        let mutable index = x
        let final = length + x

        while index <> final do
            clearAt index y
            index <- index + 1

    let drawSpinner(tp: Types, x, y, inter: string, dur: string, complete: string) =
        let frames = framesOfType tp
        let duration = ParseDuration(dur, inter, frames)
        let intervalMs = ParseInterval(inter)
        let cursorPos = Console.GetCursorPosition()

        lock consoleLock (fun () -> Console.CursorVisible <- false)
        let stopwatch = Stopwatch.StartNew()

        let mutable frameIndex = 0
        let mutable helpIndex = 0
        let mutable laps = 0
        let mutable running = true
        if tp = Types.Waiting then
            while running do
                writeAt x y frames[frameIndex]

                Thread.Sleep(intervalMs)

                frameIndex <- frameIndex + 1
                helpIndex <- helpIndex + 1
                if frameIndex >= frames.Length then
                    let mutable helpX: int = x
                    while helpIndex <> 0 do
                        clearAt helpX y
                        helpIndex <- helpIndex - 1
                        helpX <- helpX + 1
                    frameIndex <- 0
                    laps <- laps + 1
                match duration.tp with
                | "s" ->
                    if stopwatch.ElapsedMilliseconds >= int64(duration.value * 1000) then
                        CleanSpinner(frames.Length, x, y)
                        running <- false
                | "ms" ->
                    if stopwatch.ElapsedMilliseconds >= int64 duration.value then
                        CleanSpinner(frames.Length, x, y)
                        running <- false
                | "laps" ->
                    if laps >= duration.value then
                        CleanSpinner(frames.Length, x, y)
                        running <- false
                | _ -> 
                    CleanSpinner(frames.Length, x, y)
                    ()
        else
            while running do
                writeAt x y frames[frameIndex]

                Thread.Sleep(intervalMs)

                frameIndex <- frameIndex + 1
                if frameIndex >= frames.Length then
                    frameIndex <- 0
                    laps <- laps + 1
                match duration.tp with
                | "s" ->
                    if stopwatch.ElapsedMilliseconds >= int64(duration.value * 1000) then
                        running <- false
                | "ms" ->
                    if stopwatch.ElapsedMilliseconds >= int64 duration.value then
                        running <- false
                | "laps" ->
                    if laps >= duration.value then
                        running <- false
                | _ -> ()
        stopwatch.Stop()

        if frames[frames.Length - 1].Length > 1 then
            writeAt (x + 1) y complete
        else    
            writeAt x y complete
        lock consoleLock (fun () -> Console.CursorVisible <- true)
        ()                                                      // return

    let threadDraw(tp: Types, x, y, inter: string, dur: string, complete: string) =
        let T = Thread(ThreadStart(fun () -> drawSpinner(tp, x, y, inter, dur, complete)))
        T.Start()