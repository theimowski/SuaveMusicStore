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
        path "/" >=> (OK "Home")
        path "/store" >=> (OK "Store")
        path "/store/browse" >=> browse
        pathScan "/store/details/%d" (fun id -> OK (sprintf "Details: %d" id))
    ]

startWebServer defaultConfig webPart