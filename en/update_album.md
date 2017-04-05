# Update album

We have delete, we have create, so we're left with the update part only.
This one will be fairly easy, as it's gonna be very similar to create (we can reuse the album form we declared in `Form` module).

`editAlbum` in `View`:

==> View.fs:`let editAlbum`

Path:

==> Path.fs:`module Admin`

Link in `manage` view to edit the album, also with additional link to details and pipe separators (Text):

==> View.fs:`yield td`

`updateAlbum` in `Db` module:

==> Db.fs:`let updateAlbum`

`editAlbum` WebPart in `App` module:

==> App.fs:`let editAlbum`

and finally `pathScan` in main `choose` WebPart:

==> App.fs:`pathScan Path.Admin.editAlbum`

Comments to above snippets:

- `editAlbum` View looks very much the same as the `createAlbum`. The only significant difference is that it has all the filed values pre-filled. 
- in `Db.updateAlbum` we can see examples of property setters. This is the way SQLProvider mutates our `Album` value, while keeping track on what has changed before `SubmitUpdates()`
- `warbler` is needed in `editAlbum` GET handler to prevent eager evaluation
- but it's not necessary for POST, because POST needs to parse the incoming request, thus the evaluation is postponed upon the successful parsing.
- after the album is updated, a redirection to `manage` is applied

> Note: SQLProvider allows to change `Album` properties after the object has been instantiated - that's generally against the immutability concept that's propagated in the functional programming paradigm. We need to remember however, that F# is not pure functional programming language, but rather "functional first". This means that while it encourages to write in functional style, it still allows to use Object Oriented constructs. This often turns out to be useful, for example when we need to improve performance.

Pheeew, this section was long, but also very productive. Looks like we can already do some serious interaction with the application!
