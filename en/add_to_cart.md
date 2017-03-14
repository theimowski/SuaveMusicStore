# Add to cart

To implement `App` handler, we need the following `Db` module functions:

==> Db.fs:`let getCart`

==> Db.fs:`let addToCart`

`addToCart` takes `cartId` and `albumId`. 
If there's already such cart entry in the database, we do increment the `Count` column, otherwise we create a new row.
To check if a cart entry exists in database, we use `getCart` - it does a standard lookup on the cartId and albumId.

Now open up the `View` module and find the `details` function to append a new button "Add to cart", at the very bottom of "album-details" div:

==> View.fs:`div ["id", "album-details"]`

With above in place, we're ready to define the handler in `App` module:

==> App.fs:`let addToCart`

==> App.fs:`pathScan Path.Cart.addAlbum`

`addToCart` invokes our `session` function with two flavors:

- if `NoSession` then create a new GUID, save the record in database and update the session store with "cartid" key
- if `UserLoggedOn` or `CartIdOnly` then only add the album to the user's cart. Note that we could bind the `cartId` string value here to both cases - as described earlier `cartId` equals GUID if user is not logged on, otherwise it's the user's name.
