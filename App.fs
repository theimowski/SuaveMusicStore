module SuaveMusicStore.App

open System

open Suave
open Suave.Cookie
open Suave.Form
open Suave.Http
open Suave.Http.Successful
open Suave.Http.RequestErrors
open Suave.Http.Applicatives
open Suave.Model.Binding
open Suave.State.CookieStateStore
open Suave.Types
open Suave.Web

let passHash (pass: string) =
    use sha = Security.Cryptography.SHA256.Create()
    Text.Encoding.UTF8.GetBytes(pass)
    |> sha.ComputeHash
    |> Array.map (fun b -> b.ToString("x2"))
    |> String.concat ""

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
    >>= context (fun x -> 
        match x |> HttpContext.state with
        | None -> f NoSession
        | Some state ->
            match state.get "cartid", state.get "username", state.get "role" with
            | Some cartId, None, None -> f (CartIdOnly cartId)
            | _, Some username, Some role -> f (UserLoggedOn {Username = username; Role = role})
            | _ -> f NoSession)

let sessionStore setF = context (fun x ->
    match HttpContext.state x with
    | Some state -> setF state
    | None -> never)

let reset =
    unsetPair Auth.SessionAuthCookie
    >>= unsetPair StateCookie
    >>= Redirection.FOUND Path.home

let redirectWithReturnPath redirection =
    request (fun x ->
        let path = x.url.AbsolutePath
        Redirection.FOUND (redirection |> Path.withParam ("returnPath", path)))

let returnPathOrHome = 
    request (fun x -> 
        let path = 
            match (x.queryParam "returnPath") with
            | Choice1Of2 path -> path
            | _ -> Path.home
        Redirection.FOUND path)

let loggedOn f_success =
    Auth.authenticate
        Cookie.CookieLife.Session
        false
        (fun () -> Choice2Of2(redirectWithReturnPath Path.Account.logon))
        (fun _ -> Choice2Of2 reset)
        f_success

let admin f_success =
    loggedOn (session (function
        | UserLoggedOn { Role = "admin" } -> f_success
        | UserLoggedOn _ -> FORBIDDEN "Only for admin"
        | _ -> UNAUTHORIZED "Not logged in"
    ))

let html container =
    let result user =
        OK (View.index (View.partUser user) container)
        >>= Writers.setMimeType "text/html; charset=utf-8"

    session (function
    | UserLoggedOn { Username = username } -> result (Some username)
    | _ -> result None)

let browse =
    request (fun r -> 
        match r.queryParam Path.Store.browseKey with
        | Choice1Of2 genre -> 
            Db.getContext()
            |> Db.getAlbumsForGenre genre
            |> View.browse genre
            |> html
        | Choice2Of2 msg -> BAD_REQUEST msg)

let overview = warbler (fun _ ->
    Db.getContext() 
    |> Db.getGenres 
    |> List.map (fun g -> g.Name) 
    |> View.store 
    |> html)

let details id =
    match Db.getAlbumDetails id (Db.getContext()) with
    | Some album ->
        html (View.details album)
    | None ->
        never

let manage = warbler (fun _ ->
    Db.getContext()
    |> Db.getAlbumsDetails
    |> View.manage
    |> html)

let bindToForm form handler =
    bindReq (bindForm form) handler BAD_REQUEST

let logon =
    choose [
        GET >>= (View.logon "" |> html)
        POST >>= bindToForm Form.logon (fun form ->
            let ctx = Db.getContext()
            let (Password password) = form.Password
            match Db.validateUser(form.Username, passHash password) ctx with
            | Some user ->
                    Auth.authenticated Cookie.CookieLife.Session false 
                    >>= session (fun _ -> succeed)
                    >>= sessionStore (fun store ->
                        store.set "username" user.UserName
                        >>= store.set "role" user.Role)
                    >>= returnPathOrHome
            | _ ->
                View.logon "Username or password is invalid." |> html
        )
    ]

let cart = View.cart [] |> html

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
        >>= Redirection.FOUND Path.Cart.overview

let createAlbum =
    let ctx = Db.getContext()
    choose [
        GET >>= warbler (fun _ -> 
            let genres = 
                Db.getGenres ctx 
                |> List.map (fun g -> decimal g.GenreId, g.Name)
            let artists = 
                Db.getArtists ctx
                |> List.map (fun g -> decimal g.ArtistId, g.Name)
            html (View.createAlbum genres artists))
        POST >>= bindToForm Form.album (fun form ->
            Db.createAlbum (int form.ArtistId, int form.GenreId, form.Price, form.Title) ctx
            Redirection.FOUND Path.Admin.manage)
    ]

let editAlbum id =
    let ctx = Db.getContext()
    match Db.getAlbum id ctx with
    | Some album ->
        choose [
            GET >>= warbler (fun _ ->
                let genres = 
                    Db.getGenres ctx 
                    |> List.map (fun g -> decimal g.GenreId, g.Name)
                let artists = 
                    Db.getArtists ctx
                    |> List.map (fun g -> decimal g.ArtistId, g.Name)
                html (View.editAlbum album genres artists))
            POST >>= bindToForm Form.album (fun form ->
                Db.updateAlbum album (int form.ArtistId, int form.GenreId, form.Price, form.Title) ctx
                Redirection.FOUND Path.Admin.manage)
        ]
    | None -> 
        never

let deleteAlbum id =
    let ctx = Db.getContext()
    match Db.getAlbum id ctx with
    | Some album ->
        choose [ 
            GET >>= warbler (fun _ -> 
                html (View.deleteAlbum album.Title))
            POST >>= warbler (fun _ -> 
                Db.deleteAlbum album ctx; 
                Redirection.FOUND Path.Admin.manage)
        ]
    | None ->
        never

let webPart = 
    choose [
        path Path.home >>= html View.home
        path Path.Store.overview >>= overview
        path Path.Store.browse >>= browse
        pathScan Path.Store.details details

        path Path.Account.logon >>= logon
        path Path.Account.logoff >>= reset

        path Path.Cart.overview >>= cart
        pathScan Path.Cart.addAlbum addToCart

        path Path.Admin.manage >>= admin manage
        path Path.Admin.createAlbum >>= admin createAlbum
        pathScan Path.Admin.editAlbum (fun id -> admin (editAlbum id))
        pathScan Path.Admin.deleteAlbum (fun id -> admin (deleteAlbum id))

        pathRegex "(.*)\.(css|png|gif)" >>= Files.browseHome

        html View.notFound
    ]

startWebServer defaultConfig webPart
