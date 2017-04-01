# Shopping cart

What's a shop without cart feature?
We would like to let the user add albums to a cart while shopping at our Store.
To fill in the gap, let's start by declaring new routes in `Path` module:

==> Path.fs:`module Cart`

Before we move to the `View`, add new type annotation for yet another database view `CartDetails`.
`CartDetails` is a view that joins `Cart` with its corresponding `Album` in order to contain album's title and its price.

==> Db.fs:`type CartDetails`

