# Albums in cart

We still have to recognize the `CartIdOnly` case - coming back to the `session` function, the pattern matching handling `CartIdOnly` looks like this:

==> App.fs:`let session`

This means that if the session store contains `cartid` key, but no `username` or `role` then we invoke the `f` parameter with `CartIdOnly`.

To fetch the actual `CartDetails` for a given `cartId`, let's define appropriate function in `Db` module:

==> Db.fs:`let getCartsDetails`

This allows us to implement `cart` handler correctly:

==> App.fs:`let cart`

Again, we use two different patterns for the same behavior here - `CartIdOnly` and `UserLoggedOn` states will query the database for cart details.