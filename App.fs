open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Web

let webPart = 
    choose [
        path "/" >=> (OK "Home")
        path "/store" >=> (OK "Store")
        path "/store/browse" >=> (OK "Browse")
        path "/store/details" >=> (OK "Details")
    ]

startWebServer defaultConfig webPart