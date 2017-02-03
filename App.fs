open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

let webPart = 
    choose [
        path "/" >=> (OK "Home")
        path "/store" >=> (OK "Store")
        path "/store/browse" >=> (OK "Store")
        path "/store/details" >=> (OK "Details")
    ]

startWebServer defaultConfig webPart