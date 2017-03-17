module SuaveMusicStore.View

open Suave.Form
open Suave.Html

let em s = tag "em" [] [Text s]
let h2 s = tag "h2" [] [Text s]
let table x = tag "table" [] x
let th x = tag "th" [] x
let tr x = tag "tr" [] x
let td x = tag "td" [] x
let strong s = tag "strong" [] (text s)

let form x = tag "form" ["method", "POST"] x
let submitInput value = input ["type", "submit"; "value", value]

type Field<'a> = {
    Label : string
    Xml : Form<'a> -> Suave.Html.Node
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
                        field.Xml layout.Form
                    ]
            ]

        yield submitInput layout.SubmitText
    ]

let home = [
    h2 "Home"
]

let ul = tag "ul"
let li = tag "li"

let store genres = [
    h2 "Browse Genres"
    p [] [
        Text (sprintf "Select from %d genres:" (List.length genres))
    ]
    ul [] [
        for g in genres -> 
            li [] [a (Path.Store.browse |> Path.withParam (Path.Store.browseKey, g)) [] [Text g]]
    ]
]

let browse genre (albums : Album list) = [
    h2 (sprintf "Genre: %s" genre)
    ul [] [
        for album in albums ->
            li [] [a (sprintf Path.Store.details album.Albumid) [] [Text album.Title]]
    ]
]

let details (album : AlbumDetails) = [
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

let manage (albums : AlbumDetails list) = [ 
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
                          Xml = selectInput (fun f -> <@ f.GenreId @>) genres None }
                        { Label = "Artist"
                          Xml = selectInput (fun f -> <@ f.ArtistId @>) artists None }
                        { Label = "Title"
                          Xml = Suave.Form.input (fun f -> <@ f.Title @>) [] }
                        { Label = "Price"
                          Xml = Suave.Form.input (fun f -> <@ f.Price @>) [] }
                        { Label = "Album Art Url"
                          Xml = Suave.Form.input (fun f -> <@ f.ArtUrl @>) ["value", "/placeholder.gif"] } ] } ]
          SubmitText = "Create" }

    div [] [
        a Path.Admin.manage [] [Text "Back to list"]
    ]
]

let editAlbum (album : Album) genres artists = [ 
    h2 "Edit"

    renderForm
        { Form = Form.album
          Fieldsets = 
              [ { Legend = "Album"
                  Fields = 
                      [ { Label = "Genre"
                          Xml = selectInput (fun f -> <@ f.GenreId @>) genres (Some (decimal album.Genreid)) }
                        { Label = "Artist"
                          Xml = selectInput (fun f -> <@ f.ArtistId @>) artists (Some (decimal album.Artistid))}
                        { Label = "Title"
                          Xml = Suave.Form.input (fun f -> <@ f.Title @>) ["value", album.Title] }
                        { Label = "Price"
                          Xml = Suave.Form.input (fun f -> <@ f.Price @>) ["value", formatDec album.Price] }
                        { Label = "Album Art Url"
                          Xml = Suave.Form.input (fun f -> <@ f.ArtUrl @>) ["value", "/placeholder.gif"] } ] } ]
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
                          Xml = Suave.Form.input (fun f -> <@ f.Username @>) [] }
                        { Label = "Password"
                          Xml = Suave.Form.input (fun f -> <@ f.Password @>) [] } ] } ]
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



let emptyCart = [
    h2 "Your cart is empty"
    Text "Find some great music in our "
    a Path.home [] [Text "store"]
    Text "!"
]

let nonEmptyCart (carts : CartDetails list) = [
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
                    a (sprintf Path.Store.details cart.Albumid) [] [Text cart.Albumtitle]
                ]
                td [
                    Text (formatDec cart.Price)
                ]
                td [
                    Text (cart.Count.ToString())
                ]
                td [
                    a "#" ["class", "removeFromCart"; "data-id", cart.Albumid.ToString()] [Text "Remove from cart"]
                ]
            ]
        yield tr [
            for d in ["Total"; ""; ""; carts |> List.sumBy (fun c -> c.Price * (decimal c.Count)) |> formatDec] ->
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
                          Xml = Suave.Form.input (fun f -> <@ f.Username @>) [] }
                        { Label = "Email address"
                          Xml = Suave.Form.input (fun f -> <@ f.Email @>) [] }
                        { Label = "Password (between 6 and 20 characters)"
                          Xml = Suave.Form.input (fun f -> <@ f.Password @>) [] }
                        { Label = "Confirm password"
                          Xml = Suave.Form.input (fun f -> <@ f.ConfirmPassword @>) [] } ] } ]
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
                          Xml = Suave.Form.input (fun f -> <@ f.FirstName @>) [] }
                        { Label = "Last Name"
                          Xml = Suave.Form.input (fun f -> <@ f.LastName @>) [] }
                        { Label = "Address"
                          Xml = Suave.Form.input (fun f -> <@ f.Address @>) [] } ] }

                { Legend = "Payment"
                  Fields = 
                      [ { Label = "Promo Code"
                          Xml = Suave.Form.input (fun f -> <@ f.PromoCode @>) [] } ] } ]
          SubmitText = "Submit Order"
        }
]

let checkoutComplete = [
    h2 "Checkout Complete"
    p [] [
        Text "Thanks for your order!"
    ]
    p [] [
        Text "How about shopping for some more music in our "
        a Path.home [] [Text "store"]
        Text "?"
    ]
]
