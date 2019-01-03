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

The project won't compile as is.  There is a problem in `Fns.fs` in the 
`getPurchaseInfo` function.  The call to `ApiActionResult.bind` in the pipeline
requires a function and none is supplied in the pipeline.  This is what 
Scott Wlaschin had on his webpage and he says it works but I'm still trying to
figure out where I made a mistake.
