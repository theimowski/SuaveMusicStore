[<AutoOpen>]
module SuaveMusicStore.Common

open Suave
open Suave.Authentication
open Suave.Cookie
open Suave.Html
open Suave.Successful
open Suave.Form
open Suave.Model.Binding
open Suave.Operators
open Suave.RequestErrors
open Suave.State.CookieStateStore

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
    bindReq (bindForm form) handler BAD_REQUEST

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
    let ctx = Db.getContext()
    let result cartItems user =
        OK (index (partNav cartItems) (partUser user) container)
        >=> Writers.setMimeType "text/html; charset=utf-8"
        
    session (function
    | UserLoggedOn { Username = username } -> 
        let items = Db.getCartsDetails username ctx |> List.sumBy (fun c -> c.Count)
        result items (Some username)
    | CartIdOnly cartId ->
        let items = Db.getCartsDetails cartId ctx |> List.sumBy (fun c -> c.Count)
        result items None
    | NoSession ->
        result 0 None)