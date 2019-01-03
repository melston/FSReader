module Fns

open Types

let traverse f list =
    // define the applicative functions
    let (<*>) = ApiActionResult.apply
    let retn = ApiActionResult.retn

    // define a "cons" function
    let cons head tail = head :: tail

    // right fold over the list
    let initState = retn []
    let folder head tail = 
        retn cons <*> f head <*> tail

    List.foldBack folder list initState        

let getPurchaseIds (custId:CustId) (api:ApiClient) =
    api.Get<ProductId list> custId

let getProductInfo (productId:ProductId) =

    // create the api-consuming function
    let action (api:ApiClient) = 
        api.Get<ProductInfo> productId

    // wrap it in the single case
    ApiAction action

let getPurchaseInfo =
    let getProductInfoLifted =
        getProductInfo
        |> traverse 
        |> ApiActionResult.bind 
    getPurchaseIds >> getProductInfoLifted
