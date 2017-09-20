open System
open System.IO
open System.Text.RegularExpressions

let (|Regex|_|) pattern input =
    let m = Regex.Match(input, pattern)
    if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
    else None

let tryUrl (url: string) =
    use wc = new System.Net.WebClient()
    try
        wc.DownloadString url |> ignore
        None
    with e ->
        Some (e.Message)

let clean (x : string) =
    x.Replace ("{{book.version}}", "2.0")

let x =
    DirectoryInfo("en").EnumerateFiles()
    |> Seq.toList
    |> List.filter (fun fi -> fi.Name <> "SUMMARY.md")
    //|> List.map (fun fi -> fi.Name)
    |> List.collect (fun fi -> File.ReadAllLines fi.FullName
                              |> Array.toList
                              |> List.collect (function
                              | Regex "\[.*?\]?\((.*?)\)" urls -> 
                                    urls
                                    |> List.map 
                                        (fun x -> fi.Name, clean x)
                              | _ -> []))
    //|> List.iter (printfn "%A")                      
    //|> List.length               
    |> List.map (fun (fi, url) -> Uri(url))
    |> List.groupBy (fun uri -> uri.Host)
    //|> List.toArray
    //|> Array.Parallel.map 
    //    (fun (fi,url) -> match tryUrl url with
    //                     | Some err -> Some (fi,url,err)
    //                     | None -> None)      
    //|> Array.choose id                 