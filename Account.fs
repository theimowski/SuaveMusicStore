module SuaveMusicStore.Account

open System

open Suave
open Suave.Authentication
open Suave.Filters
open Suave.Form
open Suave.Operators
open Suave.State.CookieStateStore

let passHash (pass: string) =
    use sha = Security.Cryptography.SHA256.Create()
    Text.Encoding.UTF8.GetBytes(pass)
    |> sha.ComputeHash
    |> Array.map (fun b -> b.ToString("x2"))
    |> String.concat ""

let returnPathOrHome = 
    request (fun x -> 
        let path = 
            match (x.queryParam "returnPath") with
            | Choice1Of2 path -> path
            | _ -> Path.home
        Redirection.FOUND path)

let upgradeCarts (cartId : string, username :string) (ctx : DbContext) =
    for cart in getCarts cartId ctx do
        match getCart username cart.Albumid ctx with
        | Some existing ->
            existing.Count <- existing.Count +  cart.Count
            cart.Delete()
        | None ->
            cart.Cartid <- username
    ctx.SubmitUpdates()

let authenticateUser (user : User) =
    authenticated Cookie.CookieLife.Session false 
    >=> session (function
        | CartIdOnly cartId ->
            let ctx = getContext()
            upgradeCarts (cartId, user.Username) ctx
            sessionStore (fun store -> store.set "cartid" "")
        | _ -> succeed)
    >=> sessionStore (fun store ->
        store.set "username" user.Username
        >=> store.set "role" user.Role)
    >=> returnPathOrHome

let validateUser (username, password) (ctx : DbContext) : User option =
    query {
        for user in ctx.Public.Users do
            where (user.Username = username && user.Password = password)
            select user
    } |> Seq.tryHead

let logon =
    choose [
        GET >=> (View.logon "" |> html)
        POST >=> bindToForm Form.logon (fun form ->
            let ctx = getContext()
            let (Password password) = form.Password
            match validateUser(form.Username, passHash password) ctx with
            | Some user ->
                    authenticateUser user
            | _ ->
                View.logon "Username or password is invalid." |> html
        )
    ]

let register =
    choose [
        GET >=> (View.register "" |> html)
        POST >=> bindToForm Form.register (fun form ->
            let ctx = getContext()
            match getUser form.Username ctx with
            | Some existing -> 
                View.register "Sorry this username is already taken. Try another one." |> html
            | None ->
                let (Password password) = form.Password
                let email = form.Email.Address
                let user = newUser (form.Username, passHash password, email) ctx
                authenticateUser user
        )
    ]

let webPart =
    choose [
        path Path.Account.logon >=> logon
        path Path.Account.logoff >=> reset
        path Path.Account.register >=> register
    ]