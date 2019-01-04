# Description

This project is an attempt to implement the *Reader Monad* in F# as described by
Scott Wlaschin on his ["F# for Fun and Profit - Reinventing the Reader Monad"]
(https://fsharpforfunandprofit.com/posts/elevated-world-6) web page.

There are some differences here as Scott used a custom version of `Result` that he 
created for his website and I am reusing the standard `Result` from F#.  To
accomodate this I have a separate `Result` module that implements the `retn` and
`apply` functions he makes use of.  Elsewhere, I have replaced the use of 
`Success` with `Ok` and `Failure` with `Error`.

The reason for creating this project was that I was confused by the types involved 
and didn't see how some of the functions actually worked (I didn't think the types
lined up properly).  I wanted to see it in practice and work out for myself how 
the code was supposed to fit together in a single project.

# Current Status

The project now compiles and runs producing the code given on Scott's page.  It makes
use of some really interesting tricks.  One thing, in particular, is really interesting
to me.  Scott passes a function through a pipeline, partially applying other functions
and generating new functions.  Here is the specific code (from `Fns.fs`):

        // CustId -> ApiAction<Result<ProductInfo list, string list>>
        let getPurchaseInfo =
            let getProductInfoLifted =
                getProductInfo
                |> traverse 
                |> ApiActionResult.bind
            getPurchaseIds >> getProductInfoLifted

In particular, the `getProductInfo` is a function of type
`ProductId` -> `ApiAction<Result<ProductInfo, string list>>`.  This function is passed to
the next stage in the pipeline, which is `traverse`.

`traverse` is a function (in this case) of type
(`ProductId` -> `ApiAction<Result<ProductInfo, string list>>`) -> `ProductId list` -> 
`ApiAction<Result<ProductInfo list, string list>>`.  Since the first parameter is 
of the same type as `getProductInfo` this is partially applied to `traverse` and the
result is a function of type
`ProductId list` -> `ApiAction<Result<ProductInfo list, string list>>`, which
is passed into the next stage of the pipeline (`ApiActionResult.bind`).

So, what comes into the `bind` stage is a function with the type
(`ProductId list -> ApiAction<Result<ProductInfo, string list>>`).  `bind` has (in our case)
the type
`(ProductId list` -> `ApiAction<Result<ProductInfo list, string list>>) -> ApiAction<Result<Result<ProductId list, string list>> -> ApiAction<Result<Result<ProductInfo list, string list>>`.  Since, once
again, the first parameter matches the output of the previous stage, we get another partially
applied function of type 
`ApiAction<Result<Result<ProductId list, string list>> -> ApiAction<Result<Result<ProductInfo list, string list>>`.

This is assigned to `getPurchaseInfo`.

Thus, the composition at the end.  `getPurchaseIds` takes a `CustId` and produces a
`ApiAction<Result<ProductId list, string list>>`.  Compose this with the partially
applied `bind` function assigned to `getProductInfoLifted` and we get the correct
type out.

This is what I was having trouble seeing from Scott's website.  Now that I have implemented
it I can see what is happening.
