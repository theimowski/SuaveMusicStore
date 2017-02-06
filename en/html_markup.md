# HTML markup

It's time to replace plain text placeholders in containers with meaningful content.
First, define `h2` in `View` module to output HTML header of level 2:

```fsharp
let h2 s = tag "h2" [] (text s)
```

and replace `text` with a new `h2` in each of the 4 containers.

We'd like the "/store" route to output hyperlinks to all genres in our Music Store.
Let's add a helper function in `Path` module, that will be responsible for formatting HTTP urls with a key-value parameter:

```fsharp
let withParam (key,value) path = sprintf "%s?%s=%s" path key value
```

The `withParam` function takes a tuple `(key,value)` as its first argument, `path` as the second and returns a properly formatted url.
A tuple (or a pair) is a widely used structure in F#. It allows us to group two values into one in an easy manner. 
Syntax for creating a tuple is as follows: `(item1, item2)` - this might look like a standard parameter passing in many other languages including C#.
Follow [this link](http://fsharpforfunandprofit.com/posts/tuples/) to learn more about tuples.

Add also a string key for the url parameter "/store/browse" in `Path.Store` module:

```fsharp
    let browseKey = "genre"
```

We'll use it in `App` module:

```fsharp
    match r.queryParam Path.Store.browseKey with
    ...
```

Now, add the following for working with the unordered list (`ul`) and list item (`li`) elements in HTML:

```fsharp
let ul xml = tag "ul" [] (flatten xml)
let li = tag "li" []
```

`flatten` takes a list of `Xml` and "flattens" it into a single `Xml` object model.
The actual container for `store` can now look like the following:

```fsharp
let store genres = [
    h2 "Browse Genres"
    p [
        text (sprintf "Select from %d genres:" (List.length genres))
    ]
    ul [
        for g in genres -> 
            li (aHref (Path.Store.browse |> Path.withParam (Path.Store.browseKey, g)) (text g))
    ]
]
```

Things worth commenting in the above snippet:

- `store` now takes a list of genres (again the type is inferred by the compiler)
- the `[ for g in genres -> ... ]` syntax is known as "list comprehension". Here we map every genre string from `genres` to a list item
- `aHref` inside list item points to the `Path.Store.browse` url with "genre" parameter - we use the `Path.withParam` function defined earlier

To use `View.store` from `App` module, let's simply pass a hardcoded list for `genres` like following:

```fsharp
    path Path.Store.overview >=> html (View.store ["Rock"; "Disco"; "Pop"])
```

Here is what the solution looks like up to this point: [Tag - View](https://github.com/theimowski/SuaveMusicStore/tree/view)

