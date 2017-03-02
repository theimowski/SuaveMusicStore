# Genre

Moving to our next WebPart "browse", let's first adjust it in `View` module:

==> View.fs:25-31

so that it takes two arguments: name of the genre (string) and a list of albums for that genre.
For each album we'll display a list item with a direct link to album's details.

> Note: Here we used the `Path.Store.details` of type `IntPath` in conjunction with `sprintf` function to format the direct link. Again this gives us safety in regards to static typing.

Now, we can modify the `browse` WebPart itself:

==> App.fs:12-20

Again, usage of pipe operator makes it clear what happens in case the `genre` is resolved from the query parameter.

> Note: in the example above we adopted "partial application", both for `Db.getAlbumsForGenre` and `View.browse`. 
> This could be achieved because the return type between the pipes is the last argument for these functions.
