module SuaveMusicStore.App

open Suave
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open Suave.Successful

let html container =
    OK (View.index container)

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

let webPart = 
    choose [
        path Path.home >=> html View.home
        path Path.Store.overview >=> overview
        path Path.Store.browse >=> browse
        pathScan Path.Store.details (fun id -> html (View.details id))
        pathRegex "(.*)\.(css|png)" >=> Files.browseHome
    ]

startWebServer defaultConfig webPart