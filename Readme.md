# Description

This project is an attempt to implement the *Reader Monad* in F# as described by
Scott Wlaschin on his 
["F# for Fun and Profit - Reinventing the Reader Monad"](https://fsharpforfunandprofit.com/posts/elevated-world-6) 
web page.

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

``` fsharp
// CustId -> ApiAction<Result<ProductInfo list, string list>>
let getPurchaseInfo =
    let getProductInfoLifted =
        getProductInfo
        |> traverse 
        |> ApiActionResult.bind
    getPurchaseIds >> getProductInfoLifted
```

In particular, the `getProductInfo` is a function of type:

``` fsharp
ProductId -> ApiAction<Result<ProductInfo, string list>>
```

This function is passed to the next stage in the pipeline, which is `traverse`.

`traverse` is a generic function but, in this case, it is of type:

``` fsharp
(ProductId -> ApiAction<Result<ProductInfo, string list>>) ->
    ProductId list -> 
    ApiAction<Result<ProductInfo list, string list>>
```

Since the first parameter is of the same type as `getProductInfo` this can be 
partially applied to `traverse` and the result is a function of type:

``` fsharp
ProductId list -> ApiAction<Result<ProductInfo list, string list>>
```

This is passed into the next stage of the pipeline (`ApiActionResult.bind`).

So, what comes into the `bind` stage is a function with the above type.
`bind` has (in our case) the type:

``` fsharp
(ProductId list -> ApiAction<Result<ProductInfo list, string list>>) ->
    ApiAction<Result<ProductId list, string list>> ->
    ApiAction<Result<ProductInfo list, string list>>
```

Since, once again, the first parameter matches the output of the previous stage, 
we get another partially applied function, this time of type:

``` fsharp
ApiAction<Result<ProductId list, string list>> ->
    ApiAction<Result<ProductInfo list, string list>>
```

This is assigned to `getProductInfoLifted`.

Thus, the composition at the end.  `getPurchaseIds` has the type:

``` fsharp
CustId -> 
    ApiAction<Result<ProductId list, string list>>
```

Compose this with the partially applied `bind` function assigned to 
`getProductInfoLifted` and we get a function of type:

``` fsharp
(CustId -> ApiAction<Result<ProductId list, string list>>) >>
        (ApiAction<Result<ProductId list, string list>> ->
             ApiAction<Result<ProductInfo list, string list>>)
```
becomes
```
CustId -> 
    ApiAction<Result<ProductInfo list, string list>>
```

This is what I was having trouble seeing from Scott's website.  Now that I have
implemented it I can see what is happening.

## Note

One of the things I found most confusing from Scott's original presentation was that I 
couldn't make the types line up.  This was because I was looking at the flow of data
through the pipeline incorrectly (I think).  I started with the `getPurchaseIds` which
produced a `ApiAction<Result<ProductId list, string list>>` and tried to pass that
into `getProductInfo`, which takes a `ProductId` and I got stuck.

It wasn't until I shifted my perspective and saw `getPurchaseIds` as a value which was
passed to `traverse` resulting in a partially applied function which, in turn is passed
to `ApiActionResult.bind` which also results in a partially applied function which is
then assigned to `getProductInfoLifted` that I finally began to make sense of it.

Until I worked through this I had never considered a pipeline as a mechanism of 
creating a partially applied function just waiting for a value.  I don't think Scott's
website ever makes a point of this (though I just might have missed it) and I don't
remember ever seeing this anywhere else, either.  It is a neat language feature and
one I will think on and try to use effectively.
