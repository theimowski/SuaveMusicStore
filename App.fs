module SuaveMusicStore.App

open Suave
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open Suave.Successful

let browse =
    request (fun r ->
        match r.queryParam "genre" with
        | Choice1Of2 genre -> OK (sprintf "Genre: %s" genre)
        | Choice2Of2 msg -> BAD_REQUEST msg)

let webPart = 
    choose [
        path Path.home >=> (OK View.index)
        path Path.Store.overview >=> (OK "Store")
        path Path.Store.browse >=> browse
        pathScan Path.Store.details (fun id -> OK (sprintf "Details %d" id))
    ]

startWebServer defaultConfig webPart