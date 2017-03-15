# Checkout handler

Now it's time to implement `placeOrder` function in the `Db` module:

==> Db.fs:`let placeOrder`

Explanation of the above snippet:

1. we retrieve all carts that belong to the given user
2. we count the total amount of cash to pay
3. a new order is created with current timestamp, total, and username
4. we invoke `SubmitUpdates` so that the `order` instance gets its `OrderId` property filled in
5. we iterate through all the carts and for each of them:
    1. we create `OrderDetails` with all properties
    2. we delete the corresponding `cart` from database. Note that we cannot delete the cart which is currently being iterated through, because it represents a row from database view `CartDetails`, not the table `Cart` itself.
6. finally we `SubmitUpdates` at the very end.

When user checks out the cart, we want to show him a following confirmation (`View` module):

==> View.fs:`let checkoutComplete`

With that in place, we can now implement the POST checkout handler:

==> App.fs:`let checkout`

We need to invoke the `Db.placeOrder` in a warbler, to ensure that it's not called for the GET handler as well.

Phew, I think we're done for now!
