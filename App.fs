module SuaveMusicStore.App

open Suave
open Suave.Filters
open Suave.Operators

let app = 
    choose [
        Store.webPart
        Admin.webPart
        Account.webPart
        Cart.webPart
        pathRegex "(.*)\.(css|png|gif|js)" >=> Files.browseHome
        html View.notFound
    ]

startWebServer defaultConfig app