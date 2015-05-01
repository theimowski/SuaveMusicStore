open Suave
open Suave.Http
open Suave.Http.Successful
open Suave.Http.Applicatives
open Suave.Web

let webPart = 
    choose [
        path "/" >>= (OK "Home")
        path "/store" >>= (OK "Store")
        path "/store/browse" >>= (OK "Genre")
        path "/store/details" >>= (OK "Details")
    ]

startWebServer defaultConfig webPart