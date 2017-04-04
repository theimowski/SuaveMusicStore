# Queries

With the type aliases for DB objects set up, we can move forward to creating our first queries in Db module:

==> Db.fs:`let getGenres`

`getGenres` is a function for finding all genres. 
The function, as well as all functions we'll define in `Db` module, takes the `DbContext` as a parameter.
The `: Genre list` part is a type annotation, which makes sure the function returns a list of `Genre`s.
Implementation is straight forward: Using dot notation we can access all genres, and then just pipe it to `Seq.toList` to match the expected return type.

==> Db.fs:`let getAlbumsForGenre`

`getAlbumsForGenre` takes `genreName` as argument (inferred to be of type string) and returns a list of `Album`s.
It makes use of "query expression" (`query { }`) which is very similar to C# Linq query.
Read [here](https://msdn.microsoft.com/en-us/library/hh225374.aspx) for more info about query expressions.
Inside the query expression, we're performing an inner join of `Albums` and `Genres` with the `GenreId` foreign key, and then we apply a predicate on `genre.Name` to match the input `genreName`.
The result of the query is piped to `Seq.toList`.

==> Db.fs:`let getAlbumDetails`

`getAlbumDetails` takes `id` as argument (inferred to be of type int) and returns `AlbumDetails option` because there might be no Album with the given id.
Here, the result of the query is piped to the `Seq.tryHead` function, which takes care to transform the result to `option` type.
`Seq.tryHead` verifies if a query returned any result: If the sequence contains any element
`Seq.tryHead` returns `Some`, otherwise `None`.