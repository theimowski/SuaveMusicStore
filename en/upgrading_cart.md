# Upgrading cart

If user logs on, we have his user name - so we can "upgrade" all his carts from GUID to the actual user's name.
Add necessary functions to the `Db` module:

```fsharp
let getCarts cartId (ctx : DbContext) : Cart list =
    query {
        for cart in ctx.``[dbo].[Carts]`` do
            where (cart.CartId = cartId)
            select cart
    } |> Seq.toList
```

```fsharp
let upgradeCarts (cartId : string, username :string) (ctx : DbContext) =
    for cart in getCarts cartId ctx do
        match getCart username cart.AlbumId ctx with
        | Some existing ->
            existing.Count <- existing.Count +  cart.Count
            cart.Delete()
        | None ->
            cart.CartId <- username
    ctx.SubmitUpdates()
```

and update the `logon` handler in `App` module:

```fsharp
authenticated Cookie.CookieLife.Session false 
>=> session (function
    | CartIdOnly cartId ->
        let ctx = Db.getContext()
        Db.upgradeCarts (cartId, user.UserName) ctx
        sessionStore (fun store -> store.set "cartid" "")
    | _ -> succeed)
>=> sessionStore (fun store ->
    store.set "username" user.UserName
    >=> store.set "role" user.Role)
>=> returnPathOrHome
```

Remarks:

- `Db.getCarts` returns just a plain list of all carts for a given `cartId`
- `Db.upgradeCarts` takes `cartId` and `username` in order to iterate through all carts (returned by `Db.getCarts`), and for each of them:
    - if there's already a cart in the database for this `username` and `albumId`, it sums up the counts and deletes the GUID cart. This can happen if logged on user adds an album to cart, then logs off and adds the same album to cart, and then back again logs on - album's count should be 2
    - if there's no cart for such `username` and `albumId`, simply "upgrade" the cart by changing the `CartId` property to `username`
- `logon` handler now recognizes `CartIdOnly` case, for which it has to invoke `Db.upgradeCarts`. In addition it wipes out the `cartId` key from session store, as from now on `username` will be used as a cart id.

Whoa, we now have the cart functionality in our Music Store! 
See the following link to browse the code: [Tag - cart](https://github.com/theimowski/SuaveMusicStore/tree/cart)

