# View amends

First we'll create a left-sided navigation menu with all possible genres in our Store. 
This will allow users to find their favorite tracks faster.

Add following `partGenres` to the `View` module:

```fsharp
let partGenres (genres : Db.Genre list) =
    ulAttr ["id", "categories"] [
        for genre in genres -> 
            li (aHref 
                    (Path.Store.browse |> Path.withParam (Path.Store.browseKey, genre.Name)) 
                    (text genre.Name))
    ]
```

`partGenres` creates an unordered list with direct links to each genre.

To include it in the main index view, let's pass it as a new parameter, and render just before the `container`:

```fsharp
let index partNav partUser partGenres container = 
    html [

    ...
    
        partGenres
    
        divId "main" container
        
        ...
```

This forces us to extend the invocation in nested `result` function in the `html` WebPart:

```fsharp
let result cartItems user =
        OK (View.index 
                (View.partNav cartItems) 
                (View.partUser user) 
                (View.partGenres (Db.getGenres ctx))
                container)
        >=> Writers.setMimeType "text/html; charset=utf-8"
```

How about we make the album list in `View.browse` look better?

```fsharp
let browse genre (albums : Db.Album list) = [
    divClass "genre" [ 
        h2 (sprintf "Genre: %s" genre)
    
        ulAttr ["id", "album-list"] [
            for album in albums ->
                li (aHref 
                        (sprintf Path.Store.details album.AlbumId) 
                        (flatten [ imgSrc album.AlbumArtUrl
                                   span (text album.Title)]))
        ]
    ]
]
```

The above view will remain a plain unordered list, but in addition to the title we'll also display the album's art as an image.

You've probably noticed that our home page is not very sophisticated. In fact it doesn't display anything other than a plain "Home" caption. Why don't we add an image banner as well as a list of best-seller albums to the home page?

First, let's fetch the best-sellers from `Db`: 

```fsharp
type BestSeller = DbContext.``[dbo].[BestSellers]Entity``
```

```fsharp
let getBestSellers (ctx : DbContext) : BestSeller list  =
    ctx.``[dbo].[BestSellers]`` |> Seq.toList
```

Now we can alter the `View.home`:

```fsharp
let home (bestSellers : Db.BestSeller list) = [
    imgSrc "/home-showcase.png"
    h2 "Fresh off the grill"
    ulAttr ["id", "album-list"] [
            for album in bestSellers ->
                li (aHref 
                        (sprintf Path.Store.details album.AlbumId) 
                        (flatten [ imgSrc album.AlbumArtUrl
                                   span (text album.Title)]))
        ]
]
```

and the `home` handler in `App` module:

```fsharp
let home =
    let ctx = Db.getContext()
    let bestsellers = Db.getBestSellers ctx
    View.home bestsellers |> html
```

```fsharp
    path Path.home >=> home
```

The "home-showcase.png" asset can be downloaded from [here](https://raw.githubusercontent.com/theimowski/SuaveMusicStore/master/home-showcase.png). Don't forget about the "Copy To Output Directory" property!


That's all for now. Hope you liked the tutorial.
If you have any questions or comments please feel free to post an issue on GitHub.
The code for the application can be found 
[here](https://github.com/theimowski/SuaveMusicStore/tree/v1.0), and the contents of this book are available [here](https://github.com/theimowski/SuaveMusicStoreTutorial/blob/master/SUMMARY.md).

Take care!
