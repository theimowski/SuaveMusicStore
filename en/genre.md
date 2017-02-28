# Genre

Moving to our next WebPart "browse", let's first adjust it in `View` module:

```fsharp
let browse genre (albums : Db.Album list) = [
    h2 (sprintf "Genre: %s" genre)
    ul [
        for a in albums ->
            li (aHref (sprintf Path.Store.details a.AlbumId) (text a.Title))
    ]
]
```

so that it takes two arguments: name of the genre (string) and a list of albums for that genre.
For each album we'll display a list item with a direct link to album's details.

> Note: Here we used the `Path.Store.details` of type `IntPath` in conjunction with `sprintf` function to format the direct link. Again this gives us safety in regards to static typing.

Now, we can modify the `browse` WebPart itself:

```fsharp
let browse =
    request (fun r ->
        match r.queryParam Path.Store.browseKey with
        | Choice1Of2 genre -> 
            Db.getContext()
            |> Db.getAlbumsForGenre genre
            |> View.browse genre
            |> html
        | Choice2Of2 msg -> BAD_REQUEST msg)
```

Again, usage of pipe operator makes it clear what happens in case the `genre` is resolved from the query parameter.

> Note: in the example above we adopted "partial application", both for `Db.getAlbumsForGenre` and `View.browse`. 
> This could be achieved because the return type between the pipes is the last argument for these functions.

If you navigate to "/store/browse?genre=Latin", you may notice there are some characters displayed incorrectly.
Let's fix this by setting the "Content-Type" header with correct charset for each HTTP response:

```fsharp
let html container =
    OK (View.index container)
    >=> Writers.setMimeType "text/html; charset=utf-8"
```
