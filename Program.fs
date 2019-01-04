// Learn more about F# at http://fsharp.org

open System
open Types
open Fns

let showResult result =
    match result with
    | Ok (productInfoList) -> 
        printfn "SUCCESS: %A" productInfoList
    | Error errs -> 
        printfn "FAILURE: %A" errs


let setupTestData (api:ApiClient) =
    //setup purchases
    api.Set (CustId "C1") [ProductId "P1"; ProductId "P2"] |> ignore
    api.Set (CustId "C2") [ProductId "PX"; ProductId "P2"] |> ignore

    //setup product info
    api.Set (ProductId "P1") {ProductName="P1-Name"} |> ignore
    api.Set (ProductId "P2") {ProductName="P2-Name"} |> ignore
    // P3 missing

// setupTestData is an api-consuming function
// so it can be put in an ApiAction
// and then that apiAction can be executed
let setupAction = ApiAction setupTestData
ApiAction.execute setupAction 

[<EntryPoint>]
let main argv =
    printfn ""

    CustId "C1"
    |> getPurchaseInfo
    |> ApiAction.execute
    |> showResult |> ignore

    printfn ""

    CustId "C2"
    |> getPurchaseInfo
    |> ApiAction.execute
    |> showResult |> ignore

    printfn ""

    CustId "CX"
    |> getPurchaseInfo
    |> ApiAction.execute
    |> showResult |> ignore

    0 // return an integer exit code
