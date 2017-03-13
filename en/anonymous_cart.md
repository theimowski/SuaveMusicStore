# Anonymous cart

It's time to revisit the `Session` type from `App` module.
Business requirement is that in order to checkout, a user must be logged on but he doesn't have to when adding albums to the cart.
That seems reasonable, as we don't want to stop him from his shopping spree with boring logon forms.
For this purpose we'll add another case `CartIdOnly` to the `Session` type.
This state will be valid for users who are not yet logged on to the store, but have already some albums in their cart:

==> App.fs:`type Session`

`CartIdOnly` contains string with a GUID generated upon adding first item to the cart.

Switching back to `Db` module, let's create a type alias for `Carts` table:

==> Db.fs:`type Cart`

`Cart` has following properties:

- CartId - a GUID if user is not logged on, otherwise username
- AlbumId
- Count