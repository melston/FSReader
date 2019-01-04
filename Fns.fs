module Fns

open Types

// ('a -> ApiAction<Result<'b, 'c list>>) -> 'a list 
//            -> ApiAction<Result<'b list, 'c list>>
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

// CustId -> ApiAction<Result<ProductId list, string list>>
let getPurchaseIds (custId:CustId) =
     // create the api-consuming function
    let action (api:ApiClient) = 
        api.Get<ProductId list> custId

    // wrap it in the single case
    ApiAction action

// ProductId -> ApiAction<Result<ProductInfo, string list>>
let getProductInfo (productId:ProductId) =
    // create the api-consuming function
    let action (api:ApiClient) = 
        api.Get<ProductInfo> productId

    // wrap it in the single case
    ApiAction action

// CustId -> ApiAction<Result<ProductInfo list, string list>>
let getPurchaseInfo =
    let getProductInfoLifted =
        getProductInfo
        |> traverse 
        |> ApiActionResult.bind
    getPurchaseIds >> getProductInfoLifted
