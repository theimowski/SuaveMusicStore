open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config

let webPart = OK "Hello World!"

startWebServer defaultConfig webPart