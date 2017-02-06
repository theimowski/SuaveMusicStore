# Container

With styles in place, let's get our hands on extracting a shared layout for all future views to come.
Start by adding `container` parameter to `index` in `View`:

```fsharp
let index container = 
    html [
    ...
```

and div with id "main" just after the div "header":

```fsharp
    divId "header" [
        h1 (aHref Path.home (text "F# Suave Music Store"))
    ]

    divId "main" container
```

`index` previously was a constant value, but it has now become a function taking `container` as parameter.

We can now define the actual container for the "home" page:

```fsharp
let home = [
    text "Home"
]
```

For now it will only contain plain "Home" text.
Let's also extract a common function for creating the WebPart, parametrized with the container itself.
Add to `App` module, just before the `browse` WebPart the following:

```fsharp
let html container =
    OK (View.index container)
```

Usage for the home page looks like this:

```fsharp
    path Path.home >=> html View.home
```

Next, containers for each valid route in our application can be defined in `View` module:

```fsharp
let home = [
    text "Home"
]

let store = [
    text "Store"
]

let browse genre = [
    text (sprintf "Genre: %s" genre)
]

let details id = [
    text (sprintf "Details %d" id)
]
```

Note that both `home` and `store` are constant values, while `browse` and `details` are parametrized with `genre` and `id` respectively.

`html` can be now reused for all 4 views:

```fsharp
let browse =
    request (fun r ->
        match r.queryParam "genre" with
        | Choice1Of2 genre -> html (View.browse genre)
        | Choice2Of2 msg -> BAD_REQUEST msg)

let webPart = 
    choose [
        path Path.home >=> html View.home
        path Path.Store.overview >=> html View.store
        path Path.Store.browse >=> browse
        pathScan Path.Store.details (fun id -> html (View.details id))

        pathRegex "(.*)\.(css|png)" >=> Files.browseHome
    ]
```
