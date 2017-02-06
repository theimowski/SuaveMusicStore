# Path module

Before we move on to defining views for the rest of the application, let's introduce one more file - `Path.fs` and insert it **before** `View.fs`:

```fsharp
module SuaveMusicStore.Path

type IntPath = PrintfFormat<(int -> string),unit,string,string,int>

let home = "/"

module Store =
    let overview = "/store"
    let browse = "/store/browse"
    let details : IntPath = "/store/details/%d"
```

The module will contain all valid routes in our application.
We'll keep them here in one place, in order to be able to reuse both in `App` and `View` modules.
Thanks to that, we will minimize the risk of a typo in our `View` module.
We defined a submodule called `Store` in order to group routes related to one functionality - later in the tutorial we'll have a few more submodules, each of them reflecting a specific set of functionality of the application.

The `IntPath` type alias that we declared will let us use our routes in conjunction with static-typed Suave routes (`pathScan` in `App` module). 
We don't need to fully understand the signature of this type, for now we can think of it as a route parametrized with integer value.
And indeed, we annotated the `details` route with this type, so that the compiler treats this value *specially*. 
We'll see in a moment how we can use `details` in `App` and `View` modules, with the advantage of static typing.

Let's use the routes from `Path` module in our `App`:

```fsharp
let webPart = 
    choose [
        path Path.home >=> (OK View.index)
        path Path.Store.overview >=> (OK "Store")
        path Path.Store.browse >=> browse
        pathScan Path.Store.details (fun id -> OK (sprintf "Details %d" id))
    ]
```

as well as in our `View` for `aHref` to `home`:

```fsharp
divId "header" [
    h1 (aHref Path.home (text "F# Suave Music Store"))
]
```

Note, that in `App` module we still benefit from the static typed routes feature that Suave gives us - the `id` parameter is inferred by the compiler to be of integer type.
If you're not familiar with type inference mechanism, you can follow up [this link](http://fsharpforfunandprofit.com/posts/type-inference/).
