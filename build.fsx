#I @"packages/build/FAKE/tools"
#load @"packages/build/FSharp.Formatting/FSharp.Formatting.fsx"
#r @"Microsoft.Web.XmlTransform.dll"
#r @"FakeLib.dll"
#r @"System.Xml.Linq"

#r "FakeLib.dll"

#I @"packages/build/Suave/lib/net40"
#r "Suave.dll"

open System
open System.IO
open System.Text.RegularExpressions
open System.Xml.Linq
open System.Xml.XPath

open Fake
open Fake.Git

open FSharp.Literate

open Suave
open Suave.Web
open Suave.Http
open Suave.Operators
open Suave.Sockets
open Suave.Sockets.Control
open Suave.Sockets.AsyncSocket
open Suave.WebSocket

let outDir = Path.Combine ( __SOURCE_DIRECTORY__,  "out")

let repo = getBuildParamOrDefault "repo" __SOURCE_DIRECTORY__
let githubAccount = "theimowski"
let githubRepo = "SuaveMusicStore"
let branch = "v2.0_src"

let write (path, lines: list<String>) =
  if not (File.Exists path && File.ReadAllLines path |> Array.toList = lines) then
    tracefn "Writing file '%s'" path
    File.WriteAllLines (path, lines)

let (|Regex|_|) pattern input =
  let m = Regex.Match(input, pattern)
  if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
  else None
  
let (|Int32|_|) input =
  match Int32.TryParse input with
  | true, x -> Some x
  | _ -> None
  
let fileContentsAt commit file = 
  Git.CommandHelper.getGitResult repo (sprintf "show %s:%s" commit file) 
   
module List =
  let prepend xs ys = List.append ys xs

type Snippet =
| SnippetWholeFile
| SnippetLinesBounded of startLine : int * endLine : int
| SnippetStartingWith of start : string

let snipId = function
| file, SnippetWholeFile -> file
| file, SnippetLinesBounded (s, e) -> sprintf "%s_%d-%d" file s e
| file, SnippetStartingWith s -> sprintf "%s_%s" file (s.Replace(" ", "_"))

let tryParseSnippet = function
| Regex "^==> ([\w\.]+):(\d+)-(\d+)$" [file; Int32 sl; Int32 el] -> 
  Some (file, SnippetLinesBounded(sl,el))
| Regex "^==> ([\w\.]+)$" [file] -> 
  Some (file, SnippetWholeFile)
| Regex "^==> ([\w\.]+):`(.+)`" [file; snipStart] ->
  Some (file, SnippetStartingWith snipStart)
| _ -> 
  None

let (|Snip|_|) = tryParseSnippet

let inline normalizePath(path:string) = 
  let dirSeparator = Path.DirectorySeparatorChar.ToString()
  path
    .Replace("\\",dirSeparator)
    .Replace("/",dirSeparator)
    .TrimEnd(Path.DirectorySeparatorChar)
    .Replace(dirSeparator + "." + dirSeparator, dirSeparator)

let regexReplace (pattern: string) (replacement: string) (input: string) =
  Regex(pattern).Replace(input, replacement)

let markdownFileName (s: string) =
  if s = "Introduction" then
    "README.md"
  else
    let s =
      Path.GetInvalidFileNameChars()
      |> Array.fold (fun (s: string) c -> s.Replace(c.ToString(), "")) s
    s.ToLowerInvariant().Replace(" ", "_") + ".md"

let parseFirstMsgLine (firstLine: string) =
  let level = max 0 (firstLine.LastIndexOf("#"))
  let title = firstLine.Trim()
  level,title,markdownFileName title

let startingToBounded srcFiles (fileName, snippet) =
  let startsWith prefix (line: string) =
    String.startsWith prefix (line.TrimStart())

  let spacesLE spaces (line: string) =
    let trimmed = line.TrimStart()
    line.Length - trimmed.Length <= spaces &&
    trimmed <> String.Empty &&
    Char.IsLetter (trimmed.[0])

  match snippet with
  | SnippetStartingWith prefix ->
    let contents =
      List.find (fst >> ((=) fileName)) srcFiles |> snd
    let line =
      contents
      |> List.indexed
      |> List.tryFind (snd >> startsWith prefix)

    match line with
    | Some (sL,line) ->
      let spaces = line.IndexOf prefix
      let eL =
        contents
        |> List.skip (sL + 1)
        |> List.tryFindIndex (spacesLE spaces)
        |> Option.map ((+) (sL + 1))
        |> fun o -> defaultArg o contents.Length
      fileName, SnippetLinesBounded (sL + 1, eL)
    | None ->
      failwithf "Couldn't find fragment starting with `%s` in %s" prefix fileName

  | _ ->
    fileName, snippet

type MyFsiEvaluator() =
  let inner = FsiEvaluator()
  do inner.EvaluationFailed.Add(fun err -> 
    traceImportant (sprintf "Evaluation failed: %A" err.Text))
  let innerInt = inner :> IFsiEvaluator
  interface IFsiEvaluator with
    member x.Format(result, kind) = 
      innerInt.Format(result, kind)
    member x.Evaluate(text:string, asExpression, ?file) =
      // Evaluate cannot invoke startWebServer as it is a blocking call
      // instead comment this out - if it appears it is probably the last line of a file anyway 
      let text = text.Replace("startWebServer", "//startWebServer")
      innerInt.Evaluate(text, asExpression, file)

let fsi = MyFsiEvaluator()

let ensureNoOverlaping (snippets: list<string * Snippet>) =
  let noOverlap (src,snipsInFile) =
    if snipsInFile |> List.contains SnippetWholeFile &&
       snipsInFile.Length > 1 then
       failwithf "%s: SnippetWholeFile and something else: %A" src snipsInFile
    
    let lines = 
      snipsInFile
      |> List.choose (function SnippetLinesBounded (s,e) -> Some (s,e) | _ -> None)
      |> List.sort
      |> List.pairwise
      |> List.tryFind (fun ((_,e1),(s2,_)) -> e1 >= s2)
    
    match lines with
    | Some ((s1,e1),(s2,e2)) -> 
      failwithf "%s: Overlapping lines: %d-%d and %d-%d" src s1 e1 s2 e2 
    | _ ->
      ()

  snippets
  |> List.groupBy fst
  |> List.map (fun (k, vs) -> k,List.map snd vs)
  |> List.iter noOverlap

let fillSnippets commit msg =
  let fsproj = 
    fileContentsAt commit "SuaveMusicStore.fsproj"
    |> String.concat "\n"
    |> fun x -> if String.IsNullOrWhiteSpace x then None else Some x
    |> Option.map XDocument.Parse

  let srcFiles,refDlls =
    match fsproj with
    | Some fsproj ->
      let ns = System.Xml.XmlNamespaceManager(System.Xml.NameTable())
      ns.AddNamespace("msbuild", "http://schemas.microsoft.com/developer/msbuild/2003")
      let files = 
        fsproj.Root.XPathSelectElements ("//msbuild:Compile", ns)
        |> Seq.map (fun e -> e.Attribute(XName.op_Implicit "Include").Value)
        |> Seq.toList
        |> List.filter ((<>) "AssemblyInfo.fs")
        |> List.map (fun src ->
          src, fileContentsAt commit src
               |> Seq.cast<string>
               |> Seq.toList)
      let dlls = 
        fsproj.Root.XPathSelectElements ("//msbuild:Reference/msbuild:HintPath", ns)
        |> Seq.map (fun e -> repo </> e.Value |> normalizePath)
        |> Seq.toList
      files, dlls
    | None ->
      tracefn "No fsproj for commit %s" commit
      [], []

  refDlls
  |> List.iter (fun path -> if not <| File.Exists path then failwithf "Cannot find '%s'" path)

  let snippets = 
    List.choose tryParseSnippet msg
    |> List.map (startingToBounded srcFiles)

  ensureNoOverlaping snippets

  let snippetOrder =
    let srcFiles = List.map fst srcFiles
    let f (_,(srcA,snipA)) (_,(srcB,snipB)) = 
      let indexA = List.findIndex ((=)srcA) srcFiles
      let indexB = List.findIndex ((=)srcB) srcFiles
      let srcOrder = compare indexA indexB
      if srcOrder <> 0 then
        srcOrder
      else
        match snipA, snipB with
        | SnippetStartingWith _, _ 
        | _, SnippetStartingWith _ ->
          failwith "all SnippetStartingWith should be already converted here to SnippetLinesBounded"
        | _ ->
          compare snipA snipB

    snippets
    |> List.indexed
    |> List.sortWith f
    |> List.map fst

  let verboseTopLvlModule lines =
    match lines with
    | Regex "module .+\.(.+)" [name] :: t ->
      let mid, last = List.take (List.length t - 1) t, List.last t
      sprintf "module %s = begin" name :: mid @ [last + " end"]
    | _ ->
      lines

  let srcFileContent (src, contents) =
    let snippets = 
      snippets
      |> List.filter (fst >> ((=) src))
      |> List.map snd
      |> List.sort

    let rec chunk line chunkAcc (contents, snippets) =
      match snippets,contents with
      | [SnippetWholeFile], contents ->
        [Some SnippetWholeFile, contents]
      | [], [] ->
        List.rev chunkAcc
      | [], _ -> 
        List.rev ((None, contents) :: chunkAcc)
      | SnippetLinesBounded (sL, eL) :: snippets, _ 
          when sL = line + 1 && eL - line > 0 ->
        let lines = contents |> List.take (eL - line)
        let rest  = contents |> List.skip (eL - line)
        let s = Some (SnippetLinesBounded (sL, eL)), lines
        chunk eL (s :: chunkAcc) (rest,snippets)
      | SnippetLinesBounded (sL, _) :: _, _
          when sL - line - 1 > 0 ->
        let lines = contents |> List.take (sL - line - 1)
        let rest  = contents |> List.skip (sL - line - 1)
        let n = None, lines
        chunk (sL - 1) (n :: chunkAcc) (rest,snippets)
      | s,c -> 
        failwithf "unexpected case, line: %d; snippets: %A; contents: %A" line s c

    let formatChunk = function
      | None, lines -> 
        [ [ "(*** hide ***)" ]
          lines ] |> List.concat
      | (Some snippet), lines ->
        [ [ sprintf "(*** define: %s ***)" (snipId (src,snippet)) ]
          lines 
          [ sprintf "(*** include: %s ***)" (snipId (src,snippet)) ] ] |> List.concat
    
    chunk 0 [] (verboseTopLvlModule contents, snippets)
    |> List.collect formatChunk

  
  let lines = 
    match refDlls with 
    | [] -> []
    | refDlls -> "(*** hide ***)" :: (List.map (sprintf "#r @\"%s\"") refDlls)

  let lines = 
    srcFiles 
    |> List.collect srcFileContent 
    |> List.append lines

  let projectName = "SuaveMusicStore"
  let _,_,outName = parseFirstMsgLine (Seq.head msg)
  let scriptOutName = Path.Combine ( __SOURCE_DIRECTORY__, "temp", outName, projectName )
  CleanDir (Path.GetDirectoryName scriptOutName)
  write(scriptOutName + ".fsx", lines)
  tracefn "processing %s.fsx..." scriptOutName
  Literate.ProcessScriptFile(scriptOutName + ".fsx",lineNumbers = false)
  let rawHtml = File.ReadAllText (scriptOutName + ".html")

  let html = XDocument.Parse ("<root>" + rawHtml + "</root>", LoadOptions.PreserveWhitespace)
  let snippets =
    html.Root.XPathSelectElements "pre"
    |> Seq.map (fun x -> x.ToString(SaveOptions.DisableFormatting)
                          .Replace("<code", "<div")
                          .Replace("</code", "</div")
                          //.Replace("\n","&#10;")
                          .Replace(""" <span class="k">end</span>""","")
                          |> regexReplace 
                              """class="t">(\w+)</span> <span class="o">=</span> <span class="k">begin</span>""" 
                              ("""class="t">""" + projectName + """.$1</span>"""))
    |> Seq.toList
    |> List.zip snippetOrder
    |> List.sortBy fst
    |> List.map snd

  let tips =
    html.Root.XPathSelectElements "div[@class='tip']"
    |> Seq.map (fun x -> x.ToString())
    |> Seq.toList
      
  let rec insertSnippets acc snippets content =
    match content,snippets with
      | [],[] -> List.rev acc
      | Snip _ :: t, s :: ss ->
        insertSnippets (s :: acc) ss t
      | h :: t, s -> 
        insertSnippets (h :: acc) s t
      | _ ->
        failwith "different amount of snippets found"
  let insertTips content = 
    tips
    |> List.append content
  
  msg
  |> insertSnippets [] snippets
  |> insertTips
  
let insertGithubCommit commit code = 
  sprintf "Show code from this section on [GitHub](https://github.com/%s/%s/commit/%s)"
          githubAccount
          githubRepo
          commit
  |> List.singleton
  |> List.append ["";"---";""]
  |> List.append code

let generate (changedFile : FileInfo option) =
  let hashLen = 40
  let commits = 
    Git.CommandHelper.getGitResult repo ("log --reverse --pretty=oneline " + branch)
    |> Seq.map (fun log -> log.Substring(0,hashLen), log.Substring(hashLen + 1))

  match changedFile with
  | None ->
    CreateDir outDir
    [ "LANGS.md"
      "book.json"
      "custom.css"
      "tips.js"
      "suave_reload.js" ]
    |> Copy outDir
    CopyDir (outDir </> "en") "en" (fun _ -> true)
    CleanDir "temp"
    commits
    |> Seq.iter (fun (commitHash,body) ->
      let fileName = markdownFileName body
      let outFile = outDir </> "en" </> fileName
      let original = File.ReadAllLines outFile |> List.ofArray
      let contents = 
        original
        |> fillSnippets commitHash
        |> insertGithubCommit commitHash
      write (outFile, contents))
  | Some changedFile ->
    let original = File.ReadAllLines changedFile.FullName |> List.ofArray
    let commit = 
      commits 
      |> Seq.tryFind (fun (c,body) -> markdownFileName body = changedFile.Name)
    let fileName = changedFile.Name
    let outFile = outDir </> "en" </> fileName
    let contents = 
      match commit with
      | Some (commitHash,body) ->
          original
          |> fillSnippets commitHash
          |> insertGithubCommit commitHash
      | None ->
        original
    write (outFile, contents)
  
let refresh (fi : FileInfo) = 
  traceImportant <| sprintf "%s was changed." fi.FullName
  try
    generate (Some fi)
    true
  with e ->
    traceError "Reload failed: "
    traceError e.Message
    traceError e.StackTrace
    false

let refreshEvent = new Event<_>()

let gitbook = Environment.ExpandEnvironmentVariables  @"%APPDATA%\npm\node_modules\gitbook-cli\bin\gitbook.js"


let handleWatcherEvents (events:FileChange seq) =
    let succeed = 
      events
      |> Seq.map (fun e -> fileInfo e.FullPath)
      |> Seq.filter (fun fi -> 
        not (fi.Attributes.HasFlag FileAttributes.Hidden) && 
        not (fi.Attributes.HasFlag FileAttributes.Directory))
      |> Seq.forall refresh
    
    if succeed then
      directExec (fun si ->
              si.FileName <- "node"
              si.Arguments <- sprintf "%s %s %s" gitbook "build" outDir
      ) |> ignore

      refreshEvent.Trigger()

Target "Generate" (fun _ ->
  CleanDir outDir

  generate None
)

let socketHandler (webSocket : WebSocket) =
  fun cx -> socket {
    while true do
      let! refreshed =
        Control.Async.AwaitEvent(refreshEvent.Publish)
        |> Suave.Sockets.SocketOp.ofAsync 
      let byteResponse =
        "refreshed"
        |> System.Text.Encoding.ASCII.GetBytes
        |> ByteSegment
      do! webSocket.send Text byteResponse true
  }

let startWebServer () =
    let rec findPort port =
        let portIsTaken =
            if isMono then false else
            System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()
            |> Seq.exists (fun x -> x.Port = port)

        if portIsTaken then findPort (port + 1) else port

    let port = findPort 8083

    let serverConfig = 
        { defaultConfig with
           homeFolder = Some (FullName <| outDir </> "_book")
           bindings = [ HttpBinding.createSimple HTTP "127.0.0.1" port ]
        }
    let app =
      choose [
        Filters.path "/websocket" >=> handShake socketHandler
        Writers.setHeader "Cache-Control" "no-cache, no-store, must-revalidate"
        >=> Writers.setHeader "Pragma" "no-cache"
        >=> Writers.setHeader "Expires" "0"
        >=> Files.browseHome ]
    startWebServerAsync serverConfig app |> snd |> Async.Start
    Diagnostics.Process.Start (sprintf "http://localhost:%d/index.html" port) |> ignore

Target "Preview" (fun _ ->
  
  directExec (fun si ->
          si.FileName <- "node"
          si.Arguments <- sprintf "%s %s %s" gitbook "install" outDir
      ) //(TimeSpan.FromSeconds 10.)
  |> ignore
  
  use watcher = 
  //  !! (repo </> ".git" </> "refs" </> "heads" </> "*.*")
    !! ("en" </> "*.md") 
    |> WatchChanges handleWatcherEvents

  directExec (fun si ->
          si.FileName <- "node"
          si.Arguments <- sprintf "%s %s %s" gitbook "build" outDir
  ) |> ignore
    
  startWebServer()
  

  //traceImportant "Waiting for git edits. Press any key to stop."
  System.Console.ReadKey() |> ignore
  //watcher.Dispose()
)

Target "Publish" (fun _ ->
  let gitbookAccount = "theimowski"
  let gitbookRepo = "suave-music-store"
  let publishRepo = sprintf "https://git.gitbook.com/%s/%s.git" gitbookAccount gitbookRepo
  let publishBranch = "v2.0"
  let publishDir = "publish"

  CleanDir publishDir
  cloneSingleBranch "" publishRepo publishBranch publishDir

  fullclean publishDir
  CopyRecursive outDir publishDir true |> printfn "%A"
  StageAll publishDir
  Commit publishDir (sprintf "Update generated gitbook %s" <| DateTime.UtcNow.ToShortDateString())
  Branches.push publishDir
)

Target "All" DoNothing

"Generate"
  ==> "Preview"
  ==> "All"

"Generate"
  ==> "Publish"

RunTargetOrDefault "All"
