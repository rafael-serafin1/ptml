type final_status =
    | SUCCESS = 0
    | FAILURE = 1

type progression_status = 
    | ON_GOING = 100
    | WAITING = 101
    | FINISHED = 102

let rec define_status(status: int8) =
    if status = 0y then
        final_status.SUCCESS
    else 
        final_status.FAILURE
    
let s: final_status = define_status 1y
