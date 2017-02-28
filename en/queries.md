# Queries

With the type aliases set up, we can move forward to creating our first queries:

```fsharp
let firstOrNone s = s |> Seq.tryFind (fun _ -> true)

let getGenres (ctx : DbContext) : Genre list = 
    ctx.``[dbo].[Genres]`` |> Seq.toList

let getAlbumsForGenre genreName (ctx : DbContext) : Album list = 
    query { 
        for album in ctx.``[dbo].[Albums]`` do
            join genre in ctx.``[dbo].[Genres]`` on (album.GenreId = genre.GenreId)
            where (genre.Name = genreName)
            select album
    }
    |> Seq.toList

let getAlbumDetails id (ctx : DbContext) : AlbumDetails option = 
    query { 
        for album in ctx.``[dbo].[AlbumDetails]`` do
            where (album.AlbumId = id)
            select album
    } |> firstOrNone
```

`getGenres` is a function for finding all genres. 
The function, as well as all functions we'll define in `Db` module, takes the `DbContext` as a parameter.
The `: Genre list` part is a type annotation, which makes sure the function returns a list of `Genre`s.
Implementation is straight forward:  ```ctx.``[dbo].[Genres]`` ``` queries all genres, so we just need to pipe it to the `Seq.toList`.

`getAlbumsForGenre` takes `genreName` as argument (inferred to be of type string) and returns a list of `Album`s.
It makes use of "query expression" (`query { }`) which is very similar to C# Linq query.
Read [here](https://msdn.microsoft.com/en-us/library/hh225374.aspx) for more info about query expressions.
Inside the query expression, we're performing an inner join of `Albums` and `Genres` with the `GenreId` foreign key, and then we apply a predicate on `genre.Name` to match the input `genreName`.
The result of the query is piped to `Seq.toList`.

`getAlbumDetails` takes `id` as argument (inferred to be of type int) and returns `AlbumDetails option` because there might be no Album with the given id.
Here, the result of the query is piped to the `firstOrNone` function, which takes care to transform the result to `option` type.
`firstOrNone` verifies if a query returned any result.
In case of any result, `firstOrNone` will return `Some x`, otherwise `None`.