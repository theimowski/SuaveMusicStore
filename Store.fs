module SuaveMusicStore.Store

open Suave
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors

let overview = warbler (fun _ ->
    getContext() 
    |> getGenres 
    |> List.map (fun g -> g.Name) 
    |> View.store 
    |> html)

let getAlbumsForGenre genreName (ctx : DbContext) : Album list = 
    query { 
        for album in ctx.Public.Albums do
            join genre in ctx.Public.Genres on (album.Genreid = genre.Genreid)
            where (genre.Name = genreName)
            select album
    }
    |> Seq.toList

let browse =
    request (fun r ->
        match r.queryParam Path.Store.browseKey with
        | Choice1Of2 genre -> 
            getContext()
            |> getAlbumsForGenre genre
            |> View.browse genre
            |> html
        | Choice2Of2 msg -> BAD_REQUEST msg)

let getAlbumDetails id (ctx : DbContext) : AlbumDetails option = 
    query { 
        for album in ctx.Public.Albumdetails do
            where (album.Albumid = id)
            select album
    } 
    |> Seq.tryHead

let details id =
    match getAlbumDetails id (getContext()) with
    | Some album ->
        html (View.details album)
    | None ->
        never

let webPart =
    choose [
        path Path.home >=> html View.home
        path Path.Store.overview >=> overview
        path Path.Store.browse >=> browse
        pathScan Path.Store.details details
    ]