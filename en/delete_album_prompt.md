# Delete album prompt

We can't make any operation on an album yet.
To fix this, let's first add the delete functionality:

==> View.fs:`let deleteAlbum`

`deleteAlbum` is to be placed in the `View` module. It requires new markup functions:

==> View.fs:`let strong`-`let submitInput`

- `strong` is just an emphasis
- `form` is HTML element for a form with it's "method" attribute set to "POST"
- `submitInput` is button to submit a form

A couple of snippets to handle `deleteAlbum` are still needed, starting with `Db`:

==> Db.fs:`let getAlbum`

for getting `Album option` (not `AlbumDetails`). 
New route in `Path`:

==> Path.fs:`module Admin`

Finally we can put following in the `App` module:

==> App.fs:`let deleteAlbum`

==> App.fs:`pathScan Path.Admin.deleteAlbum`

This will let us display a prompt to delete an album, but without any action on the database yet.