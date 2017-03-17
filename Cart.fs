module SuaveMusicStore.Cart

open System

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

let addToCartDb cartId albumId (ctx : DbContext)  =
    match getCart cartId albumId ctx with
    | Some cart ->
        cart.Count <- cart.Count + 1
    | None ->
        ctx.Public.Carts.Create(albumId, cartId, 1, System.DateTime.UtcNow) |> ignore
    ctx.SubmitUpdates()

let placeOrder (username : string) (ctx : DbContext) =
    let carts = getCartsDetails username ctx
    let total = carts |> List.sumBy (fun c -> (decimal) c.Count * c.Price)
    let order = ctx.Public.Orders.Create(System.DateTime.UtcNow, total)
    order.Username <- username
    ctx.SubmitUpdates()
    for cart in carts do
        let orderDetails = ctx.Public.Orderdetails.Create(cart.Albumid, order.Orderid, cart.Count, cart.Price)
        getCart cart.Cartid cart.Albumid ctx
        |> Option.iter (fun cart -> cart.Delete())
    ctx.SubmitUpdates()

let cart = 
    session (function
    | NoSession -> View.emptyCart |> html
    | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
        let ctx = getContext()
        getCartsDetails cartId ctx |> View.cart |> html)

let addToCart albumId =
    let ctx = getContext()
    session (function
            | NoSession -> 
                let cartId = Guid.NewGuid().ToString("N")
                addToCartDb cartId albumId ctx
                sessionStore (fun store ->
                    store.set "cartid" cartId)
            | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
                addToCartDb cartId albumId ctx
                succeed)
        >=> Redirection.FOUND Path.Cart.overview

let removeFromCart albumId =
    session (function
    | NoSession -> never
    | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
        let ctx = getContext()
        match getCart cartId albumId ctx with
        | Some cart -> 
            cart.Count <- cart.Count - 1
            if cart.Count = 0 then cart.Delete()
            ctx.SubmitUpdates()
            getCartsDetails cartId ctx 
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
                let ctx = getContext()
                placeOrder username ctx
                View.checkoutComplete |> html)
        ])

let webPart = 
    choose [
        path Path.Cart.overview >=> cart
        pathScan Path.Cart.addAlbum addToCart
        pathScan Path.Cart.removeAlbum removeFromCart
        path Path.Cart.checkout >=> loggedOn checkout
    ]