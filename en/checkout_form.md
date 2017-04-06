# Checkout form

Let's take a deep breath and roll to the last significant feature in our application!

Again, checkout will be based on a form:

==> Form.fs:`type Checkout`

Note that, while first three fields are mandatory, the last one "PromoCode" is optional (it's type is `string option`).

Following is the view for the checkout form:

==> View.fs:`let checkout`

Note that there are 2 fieldsets here, whereas before we used only 1.
Thanks to multiple fieldsets we can group fields that have something in common.

Now let's add a route for checkout (`Path.Cart`):

==> Path.fs:`module Cart`

a navigation button in `View.nonEmptyCart` :

==> View.fs:`let nonEmptyCart`-`div ["id", "update-message"]`

and GET handler for checkout in `App` module:

==> App.fs:`let checkout`

==> App.fs:`path Path.Cart.checkout`

Remarks:

- `checkout` can only be invoked if user is logged on. If there is `NoSession` or `CartIdOnly` we don't want to let anyone in.
- additionally we decorated the `checkout` WebPart with `loggedOn` in the routing code. This ensures to redirect user to logon form if necessary.