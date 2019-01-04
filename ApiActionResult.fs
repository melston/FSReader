module ApiActionResult

    open Types

    let map f  = 
        ApiAction.map (Result.map f)

    let retn x = 
        ApiAction.retn (Result.retn x)

    let apply fActionResult xActionResult = 
        let newAction api =
            let fResult = ApiAction.run api fActionResult 
            let xResult = ApiAction.run api xActionResult 
            Result.apply fResult xResult 
        ApiAction newAction

    // let bind (f : 'a -> ApiAction<Result<'b, 'c list>>) xActionResult : ApiAction<Result<'b, 'c list>> = 
    let bind f xActionResult = 
        let newAction api =
            let xResult = ApiAction.run api xActionResult 
            // create a new action based on what xResult is
            let yAction = 
                match xResult with
                | Ok x -> 
                    // Success? Run the function
                    f x
                | Error err -> 
                    // Failure? wrap the error in an ApiAction
                    (Error err) |> ApiAction.retn
            ApiAction.run api yAction  
        ApiAction newAction

