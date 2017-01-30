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
        pathScan "/store/details/%d" 
            (fun id -> OK (sprintf "Details: %d" id))
        pathScan "/store/details/%s/%d" 
            (fun (a, id) -> OK (sprintf "Artist: %s; Id: %d" a id))
    ]

startWebServer defaultConfig webPart