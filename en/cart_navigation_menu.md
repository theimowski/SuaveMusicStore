# Cart navigation menu

Remember our navigation menu with `Cart` tab? 
Why don't we add a number of total albums in our cart there?
To do that, let's parametrize the `partNav` view:

```fsharp
let partNav cartItems = 
    ulAttr ["id", "navlist"] [ 
        li (aHref Path.home (text "Home"))
        li (aHref Path.Store.overview (text "Store"))
        li (aHref Path.Cart.overview (text (sprintf "Cart (%d)" cartItems)))
        li (aHref Path.Admin.manage (text "Admin"))
    ]
```

as well as add the `partNav` parameter to the main `index` view:

```fsharp
let index partNav partUser container = 
```

In order to create the navigation menu, we now need to pass `cartItems` parameter.
It has to be resolved in the `html` function from `App` module, which can now look like following:

```fsharp
let html container =
    let ctx = Db.getContext()
    let result cartItems user =
        OK (View.index (View.partNav cartItems) (View.partUser user) container)
        >=> Writers.setMimeType "text/html; charset=utf-8"

    session (function
    | UserLoggedOn { Username = username } -> 
        let items = Db.getCartsDetails username ctx |> List.sumBy (fun c -> c.Count)
        result items (Some username)
    | CartIdOnly cartId ->
        let items = Db.getCartsDetails cartId ctx |> List.sumBy (fun c -> c.Count)
        result items None
    | NoSession ->
        result 0 None)
```

The change is that the nested `result` function in `html` now takes two arguments: `cartItems` and `user`. 
Additionally, we handle all cases inside the `session` invocation.
Note how the `result` function is passed different set of parameters in different cases.
