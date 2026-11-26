namespace PTML

open System
open System.Collections.Concurrent
open System.Threading
open complexs.components.domain
open complexs.components.structure

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
        | Fill

    let private lifecycleLock = obj()
    let private activeSpinners = ConcurrentDictionary<int, Thread * CancellationTokenSource>()
    let mutable private nextSpinnerId = 0
    let private cursorPos = Console.GetCursorPosition()

    type Duration =
        {
            tp: string
            value: int
        }

    let ParseInterval (s: string) =
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
            [| "⠋"; "⠙"; "⠹"; "⠸"; "⠼"; "⠴"; "⠦"; "⠧"; "⠇"; "⠏" |]
        | Dots ->
            [| "⣾"; "⣽"; "⣻"; "⢿"; "⡿"; "⣟"; "⣯"; "⣷" |]
        | Waiting -> [| "."; ".."; "..."; "...." |]
        | Burger -> [| "-"; "="; "≡" |]
        | Beam -> [| "[=  ]"; "[== ]"; "[===]"; "[ ==]"; "[  =]" |]
        | Ascii -> [| "|"; "/"; "-"; "\\" |]
        | Circle -> [| "◐"; "◓"; "◑"; "◒" |]
        | Square -> [| "◰"; "◳"; "◲"; "◱" |]
        | Moon -> [| "◜"; "◝"; "◞"; "◟" |]
        | Arrow -> [| "←"; "↖"; "↑"; "↗"; "→"; "↘"; "↓"; "↙" |]
        | Bounce -> [| "⠁"; "⠂"; "⠄"; "⠂" |]
        | Fill -> [| "▁"; "▂"; "▃"; "▄"; "▅"; "▆"; "▇"; "█" |]

    let firstFrame tp =
        framesOfType tp
        |> Array.head

    let maxFrameWidth tp =
        framesOfType tp
        |> Array.map (fun frame -> frame.Length)
        |> Array.max

    let ParseDuration (s: string, _interval: string, _frames: string array) =
        if String.IsNullOrWhiteSpace(s) then
            { tp = "ms"; value = 3000 }
        elif s.EndsWith("ms") then
            { tp = "ms"; value = s.Substring(0, s.Length - 2) |> int }
        elif s.EndsWith("s") && not (s.EndsWith("laps")) then
            { tp = "ms"; value = s.Substring(0, s.Length - 1) |> int |> (*) 1000 }
        elif s.EndsWith("laps") then
            { tp = "laps"; value = s.Substring(0, s.Length - 4) |> int }
        else
            failwith "Invalid duration format. Use 'ms', 's' or 'laps'."

    let private drawSpinnerWithCancellation
        (cancellationToken: CancellationToken)
        (tp: Types, x: int, y: int, inter: string, dur: string, complete: string) =
        let frames = framesOfType tp
        let duration = ParseDuration(dur, inter, frames)
        let intervalMs = ParseInterval inter
        let durationMs =
            if tp = Waiting || duration.tp = "laps" then Int32.MaxValue
            else duration.value
        let laps =
            if tp <> Waiting && duration.tp = "laps" then Nullable<int>(duration.value)
            else Nullable<int>()
        let spinnerComponent =
            new complexs.components.domain.Spinner(
                Coordinates(x, y),
                maxFrameWidth tp,
                1,
                frames,
                intervalMs,
                durationMs,
                complete,
                Orchestrator.Compartilhado)

        let isWaiting = tp = Waiting
        spinnerComponent.Draw(cancellationToken, laps, isWaiting, isWaiting)

    let drawSpinner (tp: Types, x: int, y: int, inter: string, dur: string, complete: string) =
        drawSpinnerWithCancellation CancellationToken.None (tp, x, y, inter, dur, complete)

    let threadDraw (tp: Types, x: int, y: int, inter: string, dur: string, complete: string) =
        lock lifecycleLock (fun () ->
            nextSpinnerId <- nextSpinnerId + 1
            let spinnerId = nextSpinnerId
            let cancellation = new CancellationTokenSource()
            let worker =
                Thread(ThreadStart(fun () ->
                    drawSpinnerWithCancellation cancellation.Token (tp, x, y, inter, dur, complete)))

            worker.IsBackground <- true
            activeSpinners[spinnerId] <- (worker, cancellation)
            worker.Start())

    let stopAll () =
        lock lifecycleLock (fun () ->
            let running = activeSpinners.ToArray()
            for entry in running do
                let _, cancellation = entry.Value
                cancellation.Cancel()

            for entry in running do
                let worker, _ = entry.Value
                worker.Join()

            for entry in running do
                let _, cancellation = entry.Value
                activeSpinners.TryRemove(entry.Key) |> ignore
                cancellation.Dispose())