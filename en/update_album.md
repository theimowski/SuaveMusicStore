# Update album

We have delete, we have create, so we're left with the update part only.
This one will be fairly easy, as it's gonna be very similar to create (we can reuse the album form we declared in `Form` module).

`editAlbum` in `View`:

```fsharp
let editAlbum (album : Db.Album) genres artists = [ 
    h2 "Edit"
        
    renderForm
        { Form = Form.album
          Fieldsets = 
              [ { Legend = "Album"
                  Fields = 
                      [ { Label = "Genre"
                          Xml = selectInput (fun f -> <@ f.GenreId @>) genres (Some (decimal album.GenreId)) }
                        { Label = "Artist"
                          Xml = selectInput (fun f -> <@ f.ArtistId @>) artists (Some (decimal album.ArtistId))}
                        { Label = "Title"
                          Xml = input (fun f -> <@ f.Title @>) ["value", album.Title] }
                        { Label = "Price"
                          Xml = input (fun f -> <@ f.Price @>) ["value", formatDec album.Price] }
                        { Label = "Album Art Url"
                          Xml = input (fun f -> <@ f.ArtUrl @>) ["value", "/placeholder.gif"] } ] } ]
          SubmitText = "Save Changes" }

    div [
        aHref Path.Admin.manage (text "Back to list")
    ]
]
```

Path:

```fsharp
    let editAlbum : IntPath = "/admin/edit/%d"    
```

Link in `manage` in `View`:

```fsharp
for album in albums -> 
        tr [
            ...

            yield td [
                aHref (sprintf Path.Admin.editAlbum album.AlbumId) (text "Edit")
                text " | "
                aHref (sprintf Path.Admin.deleteAlbum album.AlbumId) (text "Delete")
            ]
        ]
```

`updateAlbum` in `Db` module:

```fsharp
let updateAlbum (album : Album) (artistId, genreId, price, title) (ctx : DbContext) =
    album.ArtistId <- artistId
    album.GenreId <- genreId
    album.Price <- price
    album.Title <- title
    ctx.SubmitUpdates()
```

`editAlbum` WebPart in `App` module:

```fsharp
let editAlbum id =
    let ctx = Db.getContext()
    match Db.getAlbum id ctx with
    | Some album ->
        choose [
            GET >=> warbler (fun _ ->
                let genres = 
                    Db.getGenres ctx 
                    |> List.map (fun g -> decimal g.GenreId, g.Name)
                let artists = 
                    Db.getArtists ctx
                    |> List.map (fun g -> decimal g.ArtistId, g.Name)
                html (View.editAlbum album genres artists))
            POST >=> bindToForm Form.album (fun form ->
                Db.updateAlbum album (int form.ArtistId, int form.GenreId, form.Price, form.Title) ctx
                Redirection.FOUND Path.Admin.manage)
        ]
    | None -> 
        never
```

and finally `pathScan` in main `choose` WebPart:

```fsharp
    pathScan Path.Admin.editAlbum editAlbum
```

Comments to above snippets:

- `editAlbum` View looks very much the same as the `createAlbum`. The only significant difference is that it has all the filed values pre-filled. 
- in `Db.updateAlbum` we can see examples of property setters. This is the way SQLProvider mutates our `Album` value, while keeping track on what has changed before `SubmitUpdates()`
- `warbler` is needed in `editAlbum` GET handler to prevent eager evaluation
- but it's not necessary for POST, because POST needs to parse the incoming request, thus the evaluation is postponed upon the successful parsing.
- after the album is updated, a redirection to `manage` is applied

> Note: SQLProvider allows to change `Album` properties after the object has been instantiated - that's generally against the immutability concept that's propagated in the functional programming paradigm. We need to remember however, that F# is not pure functional programming language, but rather "functional first". This means that while it encourages to write in functional style, it still allows to use Object Oriented constructs. This often turns out to be useful, for example when we need to improve performance.

As the icing on the cake, let's also add link to details for each of the albums in `View.manage`:

```fsharp
aHref (sprintf Path.Admin.editAlbum album.AlbumId) (text "Edit")
text " | "
aHref (sprintf Path.Store.details album.AlbumId) (text "Details")
text " | "
aHref (sprintf Path.Admin.deleteAlbum album.AlbumId) (text "Delete")
```

Pheeew, this section was long, but also very productive. Looks like we can already do some serious interaction with the application!
Results can be seen here: [Tag - crud_and_forms](https://github.com/theimowski/SuaveMusicStore/tree/crud_and_forms)

