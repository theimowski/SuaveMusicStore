module SuaveMusicStore.View

open System

open Suave.Html

let divId id = divAttr ["id", id]
let h1 xml = tag "h1" [] xml
let h2 s = tag "h2" [] (text s)
let aHref href = tag "a" ["href", href]
let cssLink href = linkAttr [ "href", href; " rel", "stylesheet"; " type", "text/css" ]
let ul xml = tag "ul" [] (flatten xml)
let li = tag "li" []
let imgSrc src = imgAttr [ "src", src ]
let em s = tag "em" [] (text s)

let table x = tag "table" [] (flatten x)
let th x = tag "th" [] (flatten x)
let tr x = tag "tr" [] (flatten x)
let td x = tag "td" [] (flatten x)

let formatDec (d : Decimal) = d.ToString(Globalization.CultureInfo.InvariantCulture)

let truncate k (s : string) =
    if s.Length > k then
        s.Substring(0, k - 3) + "..."
    else s

let home = [
    h2 "Home"
]

let store genres = [
    h2 "Browse Genres"
    p [
        text (sprintf "Select from %d genres:" (List.length genres))
    ]
    ul [
        for g in genres -> 
            li (aHref (Path.Store.browse |> Path.withParam (Path.Store.browseKey, g)) (text g))
    ]
]

let browse genre (albums : Db.Album list) = [
    h2 (sprintf "Genre: %s" genre)
    ul [
        for a in albums ->
            li (aHref (sprintf Path.Store.details a.AlbumId) (text a.Title))
    ]
]

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

let manage (albums : Db.AlbumDetails list) = [ 
    h2 "Index"
    table [
        yield tr [
            for t in ["Artist";"Title";"Genre";"Price"] -> th [ text t ]
        ]

        for album in albums -> 
        tr [
            for t in [ truncate 25 album.Artist; truncate 25 album.Title; album.Genre; formatDec album.Price ] ->
                td [ text t ]
        ]
    ]
]

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

let index container = 
    html [
        head [
            title "Suave Music Store"
            cssLink "/Site.css"
        ]

        body [
            divId "header" [
                h1 (aHref Path.home (text "F# Suave Music Store"))
            ]

            divId "container" container

            divId "footer" [
                text "built with "
                aHref "http://fsharp.org" (text "F#")
                text " and "
                aHref "http://suave.io" (text "Suave.IO")
            ]
        ]
    ]
    |> xmlToString