module SuaveMusicStore.View

open Suave.Form
open Suave.Html

let em s = tag "em" [] [Text s]
let cssLink href = link [ "href", href; " rel", "stylesheet"; " type", "text/css" ]
let h2 s = tag "h2" [] [Text s]
let ul nodes = tag "ul" [] nodes
let ulAttr attr nodes = tag "ul" attr nodes
let li = tag "li" []
let table x = tag "table" [] x
let th x = tag "th" [] x
let tr x = tag "tr" [] x
let td x = tag "td" [] x
let strong s = tag "strong" [] (text s)

let form x = tag "form" ["method", "POST"] x
let formInput = Suave.Form.input
let submitInput value = input ["type", "submit"; "value", value]

type Field<'a> = {
    Label : string
    Html : Form<'a> -> Suave.Html.Node
}

type Fieldset<'a> = {
    Legend : string
    Fields : Field<'a> list
}

type FormLayout<'a> = {
    Fieldsets : Fieldset<'a> list
    SubmitText : string
    Form : Form<'a>
}

let renderForm (layout : FormLayout<_>) =    

    form [
        for set in layout.Fieldsets -> 
            tag "fieldset" [] [
                yield tag "legend" [] [Text set.Legend]

                for field in set.Fields do
                    yield div ["class", "editor-label"] [
                        Text field.Label
                    ]
                    yield div ["class", "editor-field"] [
                        field.Html layout.Form
                    ]
            ]

        yield submitInput layout.SubmitText
    ]

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

let browse genre (albums : Db.Album list) = [
    h2 (sprintf "Genre: %s" genre)
    ul [
        for album in albums ->
            li [a (sprintf Path.Store.details album.Albumid) [] [Text album.Title]]
    ]
]

let details (album : Db.AlbumDetails) = [
    h2 album.Title
    p [] [ img ["src", album.Albumarturl] ]
    div ["id", "album-details"] [
        for (caption,t) in [ "Genre: ",  album.Genre
                             "Artist: ", album.Artist
                             "Price: ",  album.Price.ToString("0.##") ] ->
            p [] [
                em caption
                Text t
            ]
    ]
]

let truncate k (s : string) =
    if s.Length > k then
        s.Substring(0, k - 3) + "..."
    else s

let manage (albums : Db.AlbumDetails list) = [ 
    h2 "Index"
    p [] [
        a Path.Admin.createAlbum [] [Text "Create New"]
    ]
    table [
        yield tr [
            for t in ["Artist";"Title";"Genre";"Price";"Action"] -> th [ Text t ]
        ]

        for album in albums -> 
        tr [
            for t in [ truncate 25 album.Artist
                       truncate 25 album.Title
                       album.Genre
                       album.Price.ToString("0.##") ] ->
                td [ Text t ]
            
            yield td [
                a (sprintf Path.Admin.editAlbum album.Albumid) [] [Text "Edit"]
                Text " | "
                a (sprintf Path.Store.details album.Albumid) [] [Text "Details"]
                Text " | "
                a (sprintf Path.Admin.deleteAlbum album.Albumid) [] [Text "Delete"]
            ]
        ]
    ]
]

let deleteAlbum albumTitle = [
    h2 "Delete Confirmation"
    p [] [
        Text "Are you sure you want to delete the album titled"
        br []
        strong albumTitle
        Text "?"
    ]

    form [
        submitInput "Delete"
    ]

    div [] [
        a Path.Admin.manage [] [Text "Back to list"]
    ]
]

let createAlbum genres artists = [ 
    h2 "Create"

    renderForm
        { Form = Form.album
          Fieldsets = 
              [ { Legend = "Album"
                  Fields = 
                      [ { Label = "Genre"
                          Html = selectInput (fun f -> <@ f.GenreId @>) genres None }
                        { Label = "Artist"
                          Html = selectInput (fun f -> <@ f.ArtistId @>) artists None }
                        { Label = "Title"
                          Html = formInput (fun f -> <@ f.Title @>) [] }
                        { Label = "Price"
                          Html = formInput (fun f -> <@ f.Price @>) [] }
                        { Label = "Album Art Url"
                          Html = formInput 
                                    (fun f -> <@ f.ArtUrl @>) 
                                    ["value", "/placeholder.gif"] } ] } ]
          SubmitText = "Create" }

    div [] [
        a Path.Admin.manage [] [Text "Back to list"]
    ]
]

let editAlbum (album : Db.Album) genres artists = [ 
    h2 "Edit"

    renderForm
        { Form = Form.album
          Fieldsets = 
              [ { Legend = "Album"
                  Fields = 
                      [ { Label = "Genre"
                          Html = selectInput 
                                    (fun f -> <@ f.GenreId @>) 
                                    genres 
                                    (Some (decimal album.Genreid)) }
                        { Label = "Artist"
                          Html = selectInput 
                                    (fun f -> <@ f.ArtistId @>) 
                                    artists 
                                    (Some (decimal album.Artistid)) }
                        { Label = "Title"
                          Html = formInput 
                                    (fun f -> <@ f.Title @>) 
                                    ["value", album.Title] }
                        { Label = "Price"
                          Html = formInput 
                                    (fun f -> <@ f.Price @>) 
                                    ["value", formatDec album.Price] }
                        { Label = "Album Art Url"
                          Html = formInput 
                                    (fun f -> <@ f.ArtUrl @>) 
                                    ["value", "/placeholder.gif"] } ] } ]
          SubmitText = "Save Changes" }

    div [] [
        a Path.Admin.manage [] [Text "Back to list"]
    ]
]

let logon msg = [
    h2 "Log On"
    p [] [
        Text "Please enter your user name and password."
    ]

    div ["id", "logon-message"] [
        Text msg
    ]

    renderForm
        { Form = Form.logon
          Fieldsets = 
              [ { Legend = "Account Information"
                  Fields = 
                      [ { Label = "User Name"
                          Html = formInput (fun f -> <@ f.Username @>) [] }
                        { Label = "Password"
                          Html = formInput (fun f -> <@ f.Password @>) [] } ] } ]
          SubmitText = "Log On" }
]

let notFound = [
    h2 "Page not found"
    p [] [
        Text "Could not find the requested resource"
    ]
    p [] [
        Text "Back to "
        a Path.home [] [Text "Home"]
    ]
]

let partNav = 
    ulAttr ["id", "navlist"] [ 
        li [a Path.home [] [Text "Home"]]
        li [a Path.Store.overview [] [Text "Store"]]
        li [a Path.Admin.manage [] [Text "Admin"]]
    ]

let partUser (user : string option) = 
    div ["id", "part-user"] [
        match user with
        | Some user -> 
            yield Text (sprintf "Logged on as %s, " user)
            yield a Path.Account.logoff [] [Text "Log off"]
        | None ->
            yield a Path.Account.logon [] [Text "Log on"]
    ]

let index partUser container =
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
                partNav
                partUser
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