# Delete album action

Note that the code from previous section allows us to navigate to to `/admin/delete/%d`, but we still are not able to actually delete an album.
That's because there's no handler in our app to delete the album from database.
For the moment both GET and POST requests will do the same, which is return HTML page asking whether to delete the album.

In order to implement the deletion, add `deleteAlbum` to `Db` module:

==> Db.fs:`let getAlbum `

The snippet takes an `Album` as a parameter - instance of this type comes from database, and we can invoke `Delete()` member on it - SQLProvider keeps track of such changes, and upon `ctx.SubmitUpdates()` executes necessary SQL commands. This is somewhat similar to the "Active Record" concept.

Now, in `App` module we can distinguish between GET and POST requests:

==> App.fs:`let deleteAlbum`

- `deleteAlbum` WebPart delegates the request to `choose` with two possible WebParts (GET or POST requests). 
- `GET` and `POST` are WebParts that succeed (return `Some x`) only if the incoming HTTP request is of GET or POST verb respectively.
- after successful deletion of album, the `POST` case redirects us to the `Path.Admin.manage` page

> Important: We have to wrap both GET and POST handlers with a `warbler` - otherwise they would be evaluated just after `Some album` match, resulting in invoking e.g. `Db.deleteAlbum` even if POST does not apply.

The grid can now contain a column with link to delete the album in question:

==> View.fs:`let manage`

- there's a new "Action" column header in the first row
- in the last column of each album row comes a cell with link to delete the album
- note, we had to use the `yield` keyword again
