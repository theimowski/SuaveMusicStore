# Add to cart

To implement `App` handler, we need the following `Db` module functions:

```fsharp
let getCart cartId albumId (ctx : DbContext) : Cart option =
    query {
        for cart in ctx.``[dbo].[Carts]`` do
            where (cart.CartId = cartId && cart.AlbumId = albumId)
            select cart
    } |> firstOrNone
```

```fsharp
let addToCart cartId albumId (ctx : DbContext)  =
    match getCart cartId albumId ctx with
    | Some cart ->
        cart.Count <- cart.Count + 1
    | None ->
        ctx.``[dbo].[Carts]``.Create(albumId, cartId, 1, DateTime.UtcNow) |> ignore
    ctx.SubmitUpdates()
```

`addToCart` takes `cartId` and `albumId`. 
If there's already such cart entry in the database, we do increment the `Count` column, otherwise we create a new row.
To check if a cart entry exists in database, we use `getCart` - it does a standard lookup on the cartId and albumId.

Now open up the `View` module and find the `details` function to append a new button "Add to cart", at the very bottom of "album-details" div:

```fsharp
yield pAttr ["class", "button"] [
    aHref (sprintf Path.Cart.addAlbum album.AlbumId) (text "Add to cart")
]
```

With above in place, we're ready to define the handler in `App` module:

```fsharp
let addToCart albumId =
    let ctx = Db.getContext()
    session (function
            | NoSession -> 
                let cartId = Guid.NewGuid().ToString("N")
                Db.addToCart cartId albumId ctx
                sessionStore (fun store ->
                    store.set "cartid" cartId)
            | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
                Db.addToCart cartId albumId ctx
                succeed)
        >=> Redirection.FOUND Path.Cart.overview
```

```fsharp
pathScan Path.Cart.addAlbum addToCart
```

`addToCart` invokes our `session` function with two flavors:

- if `NoSession` then create a new GUID, save the record in database and update the session store with "cartid" key
- if `UserLoggedOn` or `CartIdOnly` then only add the album to the user's cart. Note that we could bind the `cartId` string value here to both cases - as described earlier `cartId` equals GUID if user is not logged on, otherwise it's the user's name.
