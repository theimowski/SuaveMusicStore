module SuaveMusicStore.View

open Suave.Html

let cssLink href = link [ "href", href; " rel", "stylesheet"; " type", "text/css" ]
let h2 s = tag "h2" [] [Text s]
let ul nodes = tag "ul" [] nodes
let li = tag "li" []

let home = [
    h2 "Home"
]

let store genres = [
    h2 "Browse Genres"
    p [] [
        Text (sprintf "Select from %d genres:" (List.length genres))
    ]
    ul [
        for genre in genres ->
            let url =
                Path.Store.browse
                |> Path.withParam (Path.Store.browseKey, genre) 
            li [ a url [] [ Text genre ] ]
    ]
]

let browse genre = [
    h2 (sprintf "Genre: %s" genre)
]

let details id = [
    h2 (sprintf "Details %d" id)
]

let index container =
    html [] [
        head [] [
            title [] "Suave Music Store"
            cssLink "/Site.css"
        ]

        body [] [
            div ["id", "header"] [
                tag "h1" [] [
                    a Path.home [] [Text "F# Suave Music Store"]
                ]
            ]

            div ["id", "main"] container

            div ["id", "footer"] [
                Text "built with "
                a "http://fsharp.org" [] [Text "F#"]
                Text " and "
                a "http://suave.io" [] [Text "Suave.IO"]
            ]
        ]
    ]
    |> htmlToString