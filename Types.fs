module Types

type CustId = CustId of string
type ProductId = ProductId of string
type ProductInfo = {ProductName: string; }

type ApiClient() =
    // static storage
    static let mutable data = Map.empty<string,obj>

    /// Try casting a value
    /// Return Success of the value or Failure on failure
    member private this.TryCast<'a> key (value:obj) =
        match value with
        | :? 'a as a ->
            Result.Ok a 
        | _  ->                 
            let typeName = typeof<'a>.Name
            Result.Error [sprintf "Can't cast value at %s to %s" key typeName]

    /// Get a value
    member this.Get<'a> (id:obj) = 
        let key =  sprintf "%A" id
        printfn "[API] Get %s" key
        match Map.tryFind key data with
        | Some o -> 
            this.TryCast<'a> key o
        | None -> 
            Result.Error [sprintf "Key %s not found" key]

    /// Set a value
    member this.Set (id:obj) (value:obj) = 
        let key =  sprintf "%A" id
        printfn "[API] Set %s" key
        if key = "bad" then  // for testing failure paths
            Result.Error [sprintf "Bad Key %s " key]
        else
            data <- Map.add key value data 
            Result.Ok ()

    member this.Open() =
        printfn "[API] Opening"

    member this.Close() =
        printfn "[API] Closing"

    interface System.IDisposable with
        member this.Dispose() =
            printfn "[API] Disposing"

type ApiAction<'a> = ApiAction of (ApiClient -> 'a)
