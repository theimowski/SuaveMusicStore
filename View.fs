module SuaveMusicStore.View

open Suave.Html

let cssLink href = link [ "href", href; " rel", "stylesheet"; " type", "text/css" ]

let home = [
    Text "Home"
]

let store = [
    Text "Store"
]

let browse genre = [
    Text (sprintf "Genre: %s" genre)
]

let details id = [
    Text (sprintf "Details %d" id)
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