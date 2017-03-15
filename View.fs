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
        
        yield p ["class", "button"] [
            a (sprintf Path.Cart.addAlbum album.Albumid) [] [Text "Add to cart"]
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
        a Path.Account.register [] [Text " Register"]
        Text " if you don't have an account yet."
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

let partNav cartItems = 
    ulAttr ["id", "navlist"] [ 
        li [a Path.home [] [Text "Home"]]
        li [a Path.Store.overview [] [Text "Store"]]
        li [a Path.Cart.overview [] [Text (sprintf "Cart (%d)" cartItems)]]
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

let emptyCart = [
    h2 "Your cart is empty"
    Text "Find some great music in our "
    a Path.home [] [Text "store"]
    Text "!"
]

let nonEmptyCart (carts : Db.CartDetails list) = [
    h2 "Review your cart:"
    p ["class", "button"] [
        a Path.Cart.checkout [] [Text "Checkout >>"]
    ]
    div ["id", "update-message"] [Text " "]
    table [
        yield tr [
            for h in ["Album Name"; "Price (each)"; "Quantity"; ""] ->
            th [Text h]
        ]
        for cart in carts ->
            tr [
                td [
                    a (sprintf Path.Store.details cart.Albumid) 
                      [] 
                      [Text cart.Albumtitle]
                ]
                td [
                    Text (formatDec cart.Price)
                ]
                td [
                    Text (cart.Count.ToString())
                ]
                td [
                    a "#" 
                      ["class", "removeFromCart"; 
                       "data-id", cart.Albumid.ToString()] 
                      [Text "Remove from cart"]
                ]
            ]
        yield tr [
            let total = 
                carts 
                |> List.sumBy (fun c -> c.Price * (decimal c.Count))
            for d in ["Total"; ""; ""; formatDec total] ->
                td [Text d]
        ]
    ]
    script [ "type", "text/javascript"; "src", "/jquery-3.1.1.min.js" ] []
    script [ "type", "text/javascript"; "src", "/script.js" ] []
]

let cart = function
    | [] -> emptyCart
    | list -> nonEmptyCart list

let register msg = [
    h2 "Create a New Account"
    p [] [
        Text "Use the form below to create a new account."
    ]

    div ["id", "register-message"] [
        Text msg
    ]

    renderForm
        { Form = Form.register
          Fieldsets = 
              [ { Legend = "Create a New Account"
                  Fields = 
                      [ { Label = "User name (max 30 characters)"
                          Html = formInput (fun f -> <@ f.Username @>) [] }
                        { Label = "Email address"
                          Html = formInput (fun f -> <@ f.Email @>) [] }
                        { Label = "Password (between 6 and 20 characters)"
                          Html = formInput (fun f -> <@ f.Password @>) [] }
                        { Label = "Confirm password"
                          Html = formInput (fun f -> <@ f.ConfirmPassword @>) [] }
                        ] } ]
          SubmitText = "Register" }
]

let checkout = [
    h2 "Address And Payment"
    renderForm
        { Form = Form.checkout 
          Fieldsets = 
              [ { Legend = "Shipping Information"
                  Fields = 
                      [ { Label = "First Name"
                          Html = formInput (fun f -> <@ f.FirstName @>) [] }
                        { Label = "Last Name"
                          Html = formInput (fun f -> <@ f.LastName @>) [] }
                        { Label = "Address"
                          Html = formInput (fun f -> <@ f.Address @>) [] } ] }

                { Legend = "Payment"
                  Fields = 
                      [ { Label = "Promo Code"
                          Html = formInput (fun f -> <@ f.PromoCode @>) [] } ] } ]
          SubmitText = "Submit Order"
        }
]

let index partNav partUser container =
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