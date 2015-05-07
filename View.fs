module SuaveMusicStore.View

open Suave.Html

let divId id = divAttr ["id", id]
let h1 xml = tag "h1" [] xml
let h2 s = tag "h2" [] (text s)
let aHref href = tag "a" ["href", href]
let cssLink href = linkAttr [ "href", href; " rel", "stylesheet"; " type", "text/css" ]

let home = [
    h2 "Home"
]

let store = [
    h2 "Store"
]

let browse genre = [
    h2 (sprintf "Genre: %s" genre)
]

let details id = [
    h2 (sprintf "Details %d" id)
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