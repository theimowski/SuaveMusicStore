open Suave                 // always open suave
open Suave.Successful      // for OK-result

let webPart = OK "Hello World!"

startWebServer defaultConfig webPart