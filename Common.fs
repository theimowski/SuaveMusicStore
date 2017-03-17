[<AutoOpen>]
module SuaveMusicStore.Common

open System
open Suave
open Suave.Authentication
open Suave.Cookie
open Suave.Html
open Suave.Successful
open Suave.Form
open Suave.Operators
open Suave.RequestErrors
open Suave.State.CookieStateStore

open FSharp.Data.Sql

type IntPath = PrintfFormat<(int -> string),unit,string,string,int>

type Sql = 
    SqlDataProvider< 
        ConnectionString      = "Server=192.168.99.100;Database=suavemusicstore;User Id=suave;Password=1234;",
        DatabaseVendor        = Common.DatabaseProviderTypes.POSTGRESQL,
        CaseSensitivityChange = Common.CaseSensitivityChange.ORIGINAL >

type DbContext = Sql.dataContext
type Album = DbContext.``public.albumsEntity``
type Genre = DbContext.``public.genresEntity``
type AlbumDetails = DbContext.``public.albumdetailsEntity``
type Artist = DbContext.``public.artistsEntity``
type User = DbContext.``public.usersEntity``
type CartDetails = DbContext.``public.cartdetailsEntity``
type Cart = DbContext.``public.cartsEntity``

let getContext() = Sql.GetDataContext()

let getGenres (ctx : DbContext) : Genre list = 
    ctx.Public.Genres |> Seq.toList

let getArtists (ctx : DbContext) : Artist list = 
    ctx.Public.Artists |> Seq.toList

let getAlbumsDetails (ctx : DbContext) : AlbumDetails list = 
    ctx.Public.Albumdetails |> Seq.toList

let getUser username (ctx : DbContext) : User option = 
    query {
        for user in ctx.Public.Users do
        where (user.Username = username)
        select user
    } |> Seq.tryHead

let newUser (username, password, email) (ctx : DbContext) =
    let user = ctx.Public.Users.Create(email, password, "user", username)
    ctx.SubmitUpdates()
    user

let getCart cartId albumId (ctx : DbContext) : Cart option =
    query {
        for cart in ctx.Public.Carts do
            where (cart.Cartid = cartId && cart.Albumid = albumId)
            select cart
    } |> Seq.tryHead

let getCarts cartId (ctx : DbContext) : Cart list =
    query {
        for cart in ctx.Public.Carts do
            where (cart.Cartid = cartId)
            select cart
    } |> Seq.toList

let getCartsDetails cartId (ctx : DbContext) : CartDetails list =
    query {
        for cart in ctx.Public.Cartdetails do
            where (cart.Cartid = cartId)
            select cart
    } |> Seq.toList

type UserLoggedOnSession = {
    Username : string
    Role : string
}

type Session = 
    | NoSession
    | CartIdOnly of string
    | UserLoggedOn of UserLoggedOnSession

let session f = 
    statefulForSession
    >=> context (fun x -> 
        match x |> HttpContext.state with
        | None -> f NoSession
        | Some state ->
            match state.get "cartid", state.get "username", state.get "role" with
            | Some cartId, None, None -> f (CartIdOnly cartId)
            | _, Some username, Some role -> f (UserLoggedOn {Username = username; Role = role})
            | _ -> f NoSession)

let bindToForm form handler =
    Model.Binding.bindReq (bindForm form) handler BAD_REQUEST

let reset =
    unsetPair SessionAuthCookie
    >=> unsetPair StateCookie
    >=> Redirection.FOUND Path.home

let sessionStore setF = context (fun x ->
    match HttpContext.state x with
    | Some state -> setF state
    | None -> never)

let redirectWithReturnPath redirection =
    request (fun x ->
        let path = x.url.AbsolutePath
        Redirection.FOUND (redirection |> Path.withParam ("returnPath", path)))

let loggedOn f_success =
    authenticate
        Cookie.CookieLife.Session
        false
        (fun () -> Choice2Of2(redirectWithReturnPath Path.Account.logon))
        (fun _ -> Choice2Of2 reset)
        f_success

let index partNav partUser container =
    html [] [
        head [] [
            title [] "Suave Music Store"
            link [ "href","/Site.css"; " rel", "stylesheet"; " type", "text/css" ]
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

let ul = tag "ul"
let li = tag "li"

let partNav cartItems = 
    ul ["id", "navlist"] [ 
        li [] [a Path.home [] [Text "Home"]]
        li [] [a Path.Store.overview [] [Text "Store"]]
        li [] [a Path.Cart.overview [] [Text (sprintf "Cart (%d)" cartItems)]]
        li [] [a Path.Admin.manage [] [Text "Admin"]]
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


let html container =
    let ctx = getContext()
    let result cartItems user =
        OK (index (partNav cartItems) (partUser user) container)
        >=> Writers.setMimeType "text/html; charset=utf-8"
        
    session (function
    | UserLoggedOn { Username = username } -> 
        let items = getCartsDetails username ctx |> List.sumBy (fun c -> c.Count)
        result items (Some username)
    | CartIdOnly cartId ->
        let items = getCartsDetails cartId ctx |> List.sumBy (fun c -> c.Count)
        result items None
    | NoSession ->
        result 0 None)