module SuaveMusicStore.Store

open Suave
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors

let getAlbumsForGenre genreName (ctx : Db.DbContext) : Db.Album list = 
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
            Db.getContext()
            |> getAlbumsForGenre genre
            |> View.browse genre
            |> html
        | Choice2Of2 msg -> BAD_REQUEST msg)

let getAlbumDetails id (ctx : Db.DbContext) : Db.AlbumDetails option = 
    query { 
        for album in ctx.Public.Albumdetails do
            where (album.Albumid = id)
            select album
    } 
    |> Seq.tryHead

let details id =
    match getAlbumDetails id (Db.getContext()) with
    | Some album ->
        html (View.details album)
    | None ->
        never

let webPart =
    choose [
        path Path.Store.browse >=> browse
        pathScan Path.Store.details details
    ]