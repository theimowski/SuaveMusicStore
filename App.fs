module SuaveMusicStore.App

open Suave
open Suave.Filters
open Suave.Form
open Suave.Model.Binding
open Suave.Operators
open Suave.RequestErrors
open Suave.Successful

let html container =
    OK (View.index container)
    >=> Writers.setMimeType "text/html; charset=utf-8"

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

let createAlbum =
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ -> 
            let genres = 
                Db.getGenres ctx 
                |> List.map (fun g -> decimal g.GenreId, g.Name)
            let artists = 
                Db.getArtists ctx
                |> List.map (fun g -> decimal g.ArtistId, g.Name)
            html (View.createAlbum genres artists))
        POST >=> bindToForm Form.album (fun form ->
            Db.createAlbum (int form.ArtistId, int form.GenreId, form.Price, form.Title) ctx
            Redirection.FOUND Path.Admin.manage)
    ]

let deleteAlbum id =
    let ctx = Db.getContext()
    match Db.getAlbum id ctx with
    | Some album ->
        choose [ 
            GET >=> warbler (fun _ -> 
                html (View.deleteAlbum album.Title))
            POST >=> warbler (fun _ -> 
                Db.deleteAlbum album ctx; 
                Redirection.FOUND Path.Admin.manage)
        ]
    | None ->
        never

let webPart = 
    choose [
        path Path.home >=> html View.home
        path Path.Store.overview >=> overview
        path Path.Store.browse >=> browse
        pathScan Path.Store.details details

        path Path.Admin.manage >=> manage
        path Path.Admin.createAlbum >=> createAlbum
        pathScan Path.Admin.deleteAlbum deleteAlbum

        pathRegex "(.*)\.(css|png|gif)" >=> Files.browseHome

        html View.notFound
    ]

startWebServer defaultConfig webPart
