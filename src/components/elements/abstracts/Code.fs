namespace PTML

module Code =
    type ExecMethod =
    | AsyncMethod
    | DeferMethod

    let getExecutionMethod(code: string): ExecMethod =
        if code = "async" then
            ExecMethod.AsyncMethod
        else
            ExecMethod.DeferMethod