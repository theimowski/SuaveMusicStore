open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

let webPart = 
    choose [
        path "/" >=> (OK "Home")
        path "/store" >=> (OK "Store")
        path "/store/browse" >=> (OK "Store")
        pathScan "/store/details/%d" (fun id -> OK (sprintf "Details: %d" id))
        pathScan 
            "/store/details/%s/%d" 
            (fun (a, id) -> OK (sprintf "Artist: %s; Id: %d" a id))
    ]

startWebServer defaultConfig webPart