module SuaveMusicStore.App

open System

open Suave
open Suave.Authentication
open Suave.Cookie
open Suave.Filters
open Suave.Form
open Suave.Model.Binding
open Suave.Operators
open Suave.RequestErrors
open Suave.State.CookieStateStore
open Suave.Successful

let overview = warbler (fun _ ->
    Db.getContext() 
    |> Db.getGenres 
    |> List.map (fun g -> g.Name) 
    |> View.store 
    |> html)








let cart = 
    session (function
    | NoSession -> View.emptyCart |> html
    | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
        let ctx = Db.getContext()
        Db.getCartsDetails cartId ctx |> View.cart |> html)

let addToCart albumId =
    let ctx = Db.getContext()
    session (function
            | NoSession -> 
                let cartId = Guid.NewGuid().ToString("N")
                Db.addToCart cartId albumId ctx
                sessionStore (fun store ->
                    store.set "cartid" cartId)
            | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
                Db.addToCart cartId albumId ctx
                succeed)
        >=> Redirection.FOUND Path.Cart.overview

let removeFromCart albumId =
    session (function
    | NoSession -> never
    | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
        let ctx = Db.getContext()
        match Db.getCart cartId albumId ctx with
        | Some cart -> 
            Db.removeFromCart cart albumId ctx
            Db.getCartsDetails cartId ctx 
            |> View.cart 
            |> List.map Html.htmlToString 
            |> String.concat "" 
            |> OK
        | None -> 
            never)

let checkout =
    session (function
    | NoSession | CartIdOnly _ -> never
    | UserLoggedOn {Username = username } ->
        choose [
            GET >=> (View.checkout |> html)
            POST >=> warbler (fun _ ->
                let ctx = Db.getContext()
                Db.placeOrder username ctx
                View.checkoutComplete |> html)
        ])

let webPart = 
    choose [
        Store.webPart
        path Path.home >=> html View.home
        path Path.Store.overview >=> overview

        Admin.webPart

        Account.webPart
        
        path Path.Cart.overview >=> cart
        pathScan Path.Cart.addAlbum addToCart
        pathScan Path.Cart.removeAlbum removeFromCart
        path Path.Cart.checkout >=> loggedOn checkout

        pathRegex "(.*)\.(css|png|gif|js)" >=> Files.browseHome
        html View.notFound
    ]

startWebServer defaultConfig webPart