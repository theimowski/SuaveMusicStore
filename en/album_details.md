# Album details

It's time to read album's details from the database. 
Start by adjusting the `details` in `View` module:

```fsharp
let details (album : Db.AlbumDetails) = [
    h2 album.Title
    p [ imgSrc album.AlbumArtUrl ]
    divId "album-details" [
        for (caption,t) in ["Genre:",album.Genre;"Artist:",album.Artist;"Price:",formatDec album.Price] ->
            p [
                em caption
                text t
            ]
    ]
]
```

Above snippet requires defining a few more helper functions in `View`:

```fsharp
let imgSrc src = imgAttr [ "src", src ]
let em s = tag "em" [] (text s)

let formatDec (d : Decimal) = d.ToString(Globalization.CultureInfo.InvariantCulture)
```

as well as opening the `System` namespace at the top of the file.

> Note: It's a good habit to open the `System` namespace every single time - in practice it usually turns out to be helpful.

In the `details` function we used list comprehension syntax with an inline list of tuples (`["Genre:",album.Genre;...`).
This is just to save us some time from typing the `p` element three times for all those properties.
You're welcome to change the implementation so that it doesn't use this shortcut if you like.

The `AlbumDetails` database view turns out to be handy now, because we can use all the attributes we need in a single step (no explicit joins required).

To read the album's details in `App` module we can do following:

```fsharp
let details id =
    match Db.getAlbumDetails id (Db.getContext()) with
    | Some album ->
        html (View.details album)
    | None ->
        never
```

```fsharp
    pathScan Path.Store.details details
```

A few remarks regarding above snippet:

- `details` takes `id` as parameter and returns WebPart
- `Path.Store.details` of type IntPath guarantees type safety
- `Db.getAlbumDetails` can return `None` if no album with given id is found
- If an album is found, html WebPart with the `View.details` container is returned
- If no album is found, `None` WebPart is returned with help of `never`.

No pipe operator was used this time, but as an exercise you can think of how you could apply it to the `details` WebPart.

Before testing the app, add the "placeholder.gif" image asset. 
You can download it from [here](https://raw.githubusercontent.com/theimowski/SuaveMusicStore/master/placeholder.gif).
Don't forget to set "Copy To Output Directory", as well as add new file extension to the `pathRegex` in `App` module.

You might have noticed, that when you try to access a missing resource (for example entering album details url with arbitrary album id) then no response is sent.
In order to fix that, let's add a "Page Not Found" handler to our main `choose` WebPart as a last resort:

```fsharp
let webPart = 
    choose [
        ...

        html View.notFound
    ]
```

the `View.notFound` can then look like:

```fsharp
let notFound = [
    h2 "Page not found"
    p [
        text "Could not find the requested resource"
    ]
    p [
        text "Back to "
        aHref Path.home (text "Home")
    ]
]
```

Results of the section can be seen here: [Tag - Database](https://github.com/theimowski/SuaveMusicStore/tree/database)

