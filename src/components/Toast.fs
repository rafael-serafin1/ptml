namespace PTML
open System

module Toast = 
    type TimeMetric =
    | Seconds
    | MiliSeconds

    type Duration = {
        value: int
        metric: TimeMetric
    }

    let ParseDuration(dur: string): Duration = 
        if String.IsNullOrWhiteSpace(dur) then
            {
                value = 500
                metric = MiliSeconds
            }
        elif dur.EndsWith("ms") then
            {
                value = dur.Substring(0, dur.Length - 2) |> int
                metric = MiliSeconds
            } 
        elif dur.EndsWith("s") then
            {
                value = dur.Substring(0, dur.Length - 1) |> int
                metric = Seconds
            }
        else
            failwith $"Non time metric or not accepted value: '{dur}'. Use 'ms' or 's'."