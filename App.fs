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
        | Choice1Of2 genre -> html (View.browse genre)
        | Choice2Of2 msg -> BAD_REQUEST msg)

let webPart = 
    choose [
        path Path.home >=> html View.home
        path Path.Store.overview >=> html (View.store ["Rock"; "Disco"; "Pop"])
        path Path.Store.browse >=> browse
        pathScan Path.Store.details (fun id -> html (View.details id))
        pathRegex "(.*)\.(css|png)" >=> Files.browseHome
    ]

startWebServer defaultConfig webPart