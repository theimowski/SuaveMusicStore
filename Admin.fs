module SuaveMusicStore.Admin

open Suave
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
let getAlbum id (ctx : Db.DbContext) : Db.Album option =
    query {
        for album in ctx.Public.Albums do
            where (album.Albumid = id)
            select album
    } |> Seq.tryHead

let updateAlbum (album : Db.Album) (artistId, genreId, price, title) (ctx : Db.DbContext) =
    album.Artistid <- artistId
    album.Genreid <- genreId
    album.Price <- price
    album.Title <- title
    ctx.SubmitUpdates()
let manage = warbler (fun _ ->
    Db.getContext()
    |> Db.getAlbumsDetails
    |> View.manage
    |> html)

let createAlbum =
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ -> 
            let genres = 
                Db.getGenres ctx 
                |> List.map (fun g -> decimal g.Genreid, g.Name)
            let artists = 
                Db.getArtists ctx
                |> List.map (fun g -> decimal g.Artistid, g.Name)
            html (View.createAlbum genres artists))

        POST >=> bindToForm Form.album (fun form ->
            ctx.Public.Albums.Create(int form.ArtistId, int form.GenreId, form.Price, form.Title) |> ignore
            ctx.SubmitUpdates()
            Redirection.FOUND Path.Admin.manage)
    ]
let editAlbum id =
    let ctx = Db.getContext()
    match getAlbum id ctx with
    | Some album ->
        choose [
            GET >=> warbler (fun _ ->
                let genres = 
                    Db.getGenres ctx 
                    |> List.map (fun g -> decimal g.Genreid, g.Name)
                let artists = 
                    Db.getArtists ctx
                    |> List.map (fun g -> decimal g.Artistid, g.Name)
                html (View.editAlbum album genres artists))
            POST >=> bindToForm Form.album (fun form ->
                updateAlbum album (int form.ArtistId, int form.GenreId, form.Price, form.Title) ctx
                Redirection.FOUND Path.Admin.manage)
        ]
    | None -> 
        never

let deleteAlbum id =
    let ctx = Db.getContext()
    match getAlbum id ctx with
    | Some album ->
        choose [ 
            GET >=> warbler (fun _ -> 
                html (View.deleteAlbum album.Title))
            POST >=> warbler (fun _ -> 
                album.Delete()
                ctx.SubmitUpdates()
                Redirection.FOUND Path.Admin.manage)
        ]
    | None ->
        never

let admin f_success =
    loggedOn (session (function
        | UserLoggedOn { Role = "admin" } -> f_success
        | UserLoggedOn _ -> FORBIDDEN "Only for admin"
        | _ -> UNAUTHORIZED "Not logged in"
    ))
let webPart = 
    choose [
        path Path.Admin.manage >=> admin manage
        path Path.Admin.createAlbum >=> admin createAlbum
        pathScan Path.Admin.editAlbum (fun id -> admin (editAlbum id))
        pathScan Path.Admin.deleteAlbum (fun id -> admin (deleteAlbum id))
    ]