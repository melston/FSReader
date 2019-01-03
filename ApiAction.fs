module ApiAction 

    open Types

    /// Evaluate the action with a given api
    /// ApiClient -> ApiAction<'a> -> 'a
    let run api (ApiAction action) = 
        let resultOfAction = action api
        resultOfAction

    /// ('a -> 'b) -> ApiAction<'a> -> ApiAction<'b>
    let map f action = 
        let newAction api =
            let x = run api action 
            f x
        ApiAction newAction

    /// 'a -> ApiAction<'a>
    let retn x = 
        let newAction api =
            x
        ApiAction newAction

    /// ApiAction<('a -> 'b)> -> ApiAction<'a> -> ApiAction<'b>
    let apply fAction xAction = 
        let newAction api =
            let f = run api fAction 
            let x = run api xAction 
            f x
        ApiAction newAction

    /// ('a -> ApiAction<'b>) -> ApiAction<'a> -> ApiAction<'b>
    let bind f xAction = 
        let newAction api =
            let x = run api xAction 
            run api (f x)
        ApiAction newAction

    /// Create an ApiClient and run the action on it
    /// ApiAction<'a> -> 'a
    let execute action =
        use api = new ApiClient()
        api.Open()
        let result = run api action
        api.Close()
        result
