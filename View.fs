module SuaveMusicStore.View

open System

open Suave.Html
open Suave.Form

let divId id = divAttr ["id", id]
let divClass c = divAttr ["class", c]
let h1 xml = tag "h1" [] xml
let h2 s = tag "h2" [] (text s)
let aHref href = tag "a" ["href", href]
let aHrefAttr href attr = tag "a" (("href", href) :: attr)
let cssLink href = linkAttr [ "href", href; " rel", "stylesheet"; " type", "text/css" ]
let ul xml = tag "ul" [] (flatten xml)
let ulAttr attr xml = tag "ul" attr (flatten xml)
let li = tag "li" []
let imgSrc src = imgAttr [ "src", src ]
let em s = tag "em" [] (text s)
let strong s = tag "strong" [] (text s)

let form x = tag "form" ["method", "POST"] (flatten x)
let fieldset x = tag "fieldset" [] (flatten x)
let legend txt = tag "legend" [] (text txt)
let submitInput value = inputAttr ["type", "submit"; "value", value]

let table x = tag "table" [] (flatten x)
let th x = tag "th" [] (flatten x)
let tr x = tag "tr" [] (flatten x)
let td x = tag "td" [] (flatten x)

let formatDec (d : Decimal) = d.ToString(Globalization.CultureInfo.InvariantCulture)

let truncate k (s : string) =
    if s.Length > k then
        s.Substring(0, k - 3) + "..."
    else s

type Field<'a> = {
    Label : string
    Xml : Form<'a> -> Suave.Html.Xml
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
            fieldset [
                yield legend set.Legend

                for field in set.Fields do
                    yield divClass "editor-label" [
                        text field.Label
                    ]
                    yield divClass "editor-field" [
                        field.Xml layout.Form
                    ]
            ]

        yield submitInput layout.SubmitText
    ]

let home (bestSellers : Db.BestSeller list) = [
    imgSrc "/home-showcase.png"
    h2 "Fresh off the grill"
    ulAttr ["id", "album-list"] [
            for album in bestSellers ->
                li (aHref 
                        (sprintf Path.Store.details album.Albumid) 
                        (flatten [ imgSrc album.Albumarturl
                                   span (text album.Title)]))
        ]
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
    divClass "genre" [ 
        h2 (sprintf "Genre: %s" genre)
 
        ulAttr ["id", "album-list"] [
            for album in albums ->
                li (aHref 
                        (sprintf Path.Store.details album.Albumid) 
                        (flatten [ imgSrc album.Albumarturl
                                   span (text album.Title)]))
        ]
    ]
]

let details (album : Db.AlbumDetails) = [
    h2 album.Title
    p [ imgSrc album.Albumarturl ]
    divId "album-details" [
        for (caption,t) in ["Genre:",album.Genre;"Artist:",album.Artist;"Price:",formatDec album.Price] ->
            p [
                em caption
                text t
            ]
        yield pAttr ["class", "button"] [
            aHref (sprintf Path.Cart.addAlbum album.Albumid) (text "Add to cart")
        ]
    ]
]

let manage (albums : Db.AlbumDetails list) = [ 
    h2 "Index"
    p [
        aHref Path.Admin.createAlbum (text "Create New")
    ]
    table [
        yield tr [
            for t in ["Artist";"Title";"Genre";"Price";""] -> th [ text t ]
        ]

        for album in albums -> 
        tr [
            for t in [ truncate 25 album.Artist; truncate 25 album.Title; album.Genre; formatDec album.Price ] ->
                td [ text t ]

            yield td [
                aHref (sprintf Path.Admin.editAlbum album.Albumid) (text "Edit")
                text " | "
                aHref (sprintf Path.Store.details album.Albumid) (text "Details")
                text " | "
                aHref (sprintf Path.Admin.deleteAlbum album.Albumid) (text "Delete")
            ]
        ]
    ]
]

let deleteAlbum albumTitle = [
    h2 "Delete Confirmation"
    p [ 
        text "Are you sure you want to delete the album titled"
        br
        strong albumTitle
        text "?"
    ]
    
    form [
        submitInput "Delete"
    ]

    div [
        aHref Path.Admin.manage (text "Back to list")
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
                          Xml = input (fun f -> <@ f.Title @>) [] }
                        { Label = "Price"
                          Xml = input (fun f -> <@ f.Price @>) [] }
                        { Label = "Album Art Url"
                          Xml = input (fun f -> <@ f.ArtUrl @>) ["value", "/placeholder.gif"] } ] } ]
          SubmitText = "Create" }

    div [
        aHref Path.Admin.manage (text "Back to list")
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
                          Xml = selectInput (fun f -> <@ f.GenreId @>) genres (Some (decimal album.Genreid)) }
                        { Label = "Artist"
                          Xml = selectInput (fun f -> <@ f.ArtistId @>) artists (Some (decimal album.Artistid))}
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

let logon msg = [
    h2 "Log On"
    p [
        text "Please enter your user name and password."
        aHref Path.Account.register (text " Register")
        text " if you don't have an account yet."
    ]

    divId "logon-message" [
        text msg
    ]

    renderForm
        { Form = Form.logon
          Fieldsets = 
              [ { Legend = "Account Information"
                  Fields = 
                      [ { Label = "User Name"
                          Xml = input (fun f -> <@ f.Username @>) [] }
                        { Label = "Password"
                          Xml = input (fun f -> <@ f.Password @>) [] } ] } ]
          SubmitText = "Log On" }
]

let register msg = [
    h2 "Create a New Account"
    p [
        text "Use the form below to create a new account."
    ]
    
    divId "register-message" [
        text msg
    ]

    renderForm
        { Form = Form.register
          Fieldsets = 
              [ { Legend = "Create a New Account"
                  Fields = 
                      [ { Label = "User name (max 30 characters)"
                          Xml = input (fun f -> <@ f.Username @>) [] }
                        { Label = "Email address"
                          Xml = input (fun f -> <@ f.Email @>) [] }
                        { Label = "Password (between 6 and 20 characters)"
                          Xml = input (fun f -> <@ f.Password @>) [] }
                        { Label = "Confirm password"
                          Xml = input (fun f -> <@ f.ConfirmPassword @>) [] } ] } ]
          SubmitText = "Register" }
]

let emptyCart = [
    h2 "Your cart is empty"
    text "Find some great music in our "
    aHref Path.home (text "store")
    text "!"
]

let nonEmptyCart (carts : Db.CartDetails list) = [
    h2 "Review your cart:"
    pAttr ["class", "button"] [
            aHref Path.Cart.checkout (text "Checkout >>")
    ]
    divId "update-message" [text " "]
    table [
        yield tr [
            for h in ["Album Name"; "Price (each)"; "Quantity"; ""] ->
            th [text h]
        ]
        for cart in carts ->
            tr [
                td [
                    aHref (sprintf Path.Store.details cart.Albumid) (text cart.Albumtitle)
                ]
                td [
                    text (formatDec cart.Price)
                ]
                td [
                    text (cart.Count.ToString())
                ]
                td [
                    aHrefAttr "#" ["class", "removeFromCart"; "data-id", cart.Albumid.ToString()] (text "Remove from cart") 
                ]
            ]
        yield tr [
            for d in ["Total"; ""; ""; carts |> List.sumBy (fun c -> c.Price * (decimal c.Count)) |> formatDec] ->
            td [text d]
        ]
    ]
    scriptAttr [ "type", "text/javascript"; " src", "/jquery-1.11.3.min.js" ] [ text "" ]
    scriptAttr [ "type", "text/javascript"; " src", "/script.js" ] [ text "" ]
]

let cart = function
    | [] -> emptyCart
    | list -> nonEmptyCart list

let checkout = [
    h2 "Address And Payment"
    renderForm
        { Form = Form.checkout 
          Fieldsets = 
              [ { Legend = "Shipping Information"
                  Fields = 
                      [ { Label = "First Name"
                          Xml = input (fun f -> <@ f.FirstName @>) [] }
                        { Label = "Last Name"
                          Xml = input (fun f -> <@ f.LastName @>) [] }
                        { Label = "Address"
                          Xml = input (fun f -> <@ f.Address @>) [] } ] }
                
                { Legend = "Payment"
                  Fields = 
                      [ { Label = "Promo Code"
                          Xml = input (fun f -> <@ f.PromoCode @>) [] } ] } ]
          SubmitText = "Submit Order"
        }
]

let checkoutComplete = [
    h2 "Checkout Complete"
    p [
        text "Thanks for your order!"
    ]
    p [
        text "How about shopping for some more music in our "
        aHref Path.home (text "store")
        text "?"
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

let partNav cartItems = 
    ulAttr ["id", "navlist"] [ 
        li (aHref Path.home (text "Home"))
        li (aHref Path.Store.overview (text "Store"))
        li (aHref Path.Cart.overview (text (sprintf "Cart (%d)" cartItems)))
        li (aHref Path.Admin.manage (text "Admin"))
    ]

let partUser (user : string option) = 
    divId "part-user" [
        match user with
        | Some user -> 
            yield text (sprintf "Logged on as %s, " user)
            yield aHref Path.Account.logoff (text "Log off")
        | None ->
            yield aHref Path.Account.logon (text "Log on")
    ]

let partGenres (genres : Db.Genre list) =
    ulAttr ["id", "categories"] [
        for genre in genres -> 
            li (aHref 
                    (Path.Store.browse |> Path.withParam (Path.Store.browseKey, genre.Name)) 
                    (text genre.Name))
    ]

let index partNav partUser partGenres container = 
    html [
        head [
            title "Suave Music Store"
            cssLink "/Site.css"
        ]

        body [
            divId "header" [
                h1 (aHref Path.home (text "F# Suave Music Store"))
                partNav
                partUser
            ]

            partGenres

            divId "main" container

            divId "footer" [
                text "built with "
                aHref "http://fsharp.org" (text "F#")
                text " and "
                aHref "http://suave.io" (text "Suave.IO")
            ]
        ]
    ]
    |> xmlToString