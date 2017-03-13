# Albums in cart

We still have to recognize the `CartIdOnly` case - coming back to the `session` function, the pattern matching handling `CartIdOnly` looks like this:

```fsharp
match state.get "cartid", state.get "username", state.get "role" with
| Some cartId, None, None -> f (CartIdOnly cartId)
| _, Some username, Some role -> f (UserLoggedOn {Username = username; Role = role})
| _ -> f NoSession
```

This means that if the session store contains `cartid` key, but no `username` or `role` then we invoke the `f` parameter with `CartIdOnly`.

To fetch the actual `CartDetails` for a given `cartId`, let's define appropriate function in `Db` module:

```fsharp
let getCartsDetails cartId (ctx : DbContext) : CartDetails list =
    query {
        for cart in ctx.``[dbo].[CartDetails]`` do
            where (cart.CartId = cartId)
            select cart
    } |> Seq.toList
```

This allows us to implement `cart` handler correctly:

```fsharp
let cart = 
    session (function
    | NoSession -> View.emptyCart |> html
    | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
        let ctx = Db.getContext()
        Db.getCartsDetails cartId ctx |> View.cart |> html)
```

Again, we use two different patterns for the same behavior here - `CartIdOnly` and `UserLoggedOn` states will query the database for cart details.