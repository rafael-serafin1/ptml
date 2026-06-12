if shouldRenderSpinner cell then
                        sb.Append(cursorTo x y) |> ignore
                        let T = Thread(ThreadStart(fun () -> drawSpinner(c.tp, x, y, c.interval, c.dur, c.complete)))
                        T.Start()
                        match ansiStyle cell with
                        | Some style when style <> currentStyle ->
                            if currentStyle <> "" then
                                sb.Append(resetCode) |> ignore
                            sb.Append(style) |> ignore
                            currentStyle <- style
                        | None when currentStyle <> "" ->
                            sb.Append(resetCode) |> ignore
                            currentStyle <- ""
                        | _ -> ()