module Result

    let retn x = Ok x

    let apply fResult xResult =
        match fResult,xResult with
        | Ok f, Ok x -> Ok (f x)
        | Error errs, Ok x -> Error errs
        | Ok f, Error errs -> Error errs
        | Error errs1, Error errs2 ->
            // concat both lists of errors
            Error (List.concat [errs1; errs2])

