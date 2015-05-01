open Suave
open Suave.Successful
open Suave.Filters
open Suave.Operators

let webPart = 
    choose [
        path "/" >=> (OK "Home")
        path "/store" >=> (OK "Store")
        path "/store/browse" >=> (OK "Genre")
        path "/store/details" >=> (OK "Details")
    ]

startWebServer defaultConfig webPart
