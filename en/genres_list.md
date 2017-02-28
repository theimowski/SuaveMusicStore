# Genres list

For more convenient instantiation of `DbContext`, let's introduce a small helper function in `Db` module:

```fsharp
let getContext() = Sql.GetDataContext()
```

Now we're ready to finally read real data in the `App` module:

```fsharp
let overview =
    Db.getContext() 
    |> Db.getGenres 
    |> List.map (fun g -> g.Name) 
    |> View.store 
    |> html

...

    path Path.Store.overview >=> overview
```

`overview` is a WebPart that... 
Hold on, do I really need to explain it?
The usage of pipe operator here makes the flow rather obvious - each line defines each step.
The return value is passed from one function to another, starting with DbContext and ending with the WebPart.
This is just a single example of how composition in functional programming makes functions look like building blocks "glued" together.

We also need to wrap the `overview` WebPart in a `warbler`:

```fsharp
let overview = warbler (fun _ ->
    Db.getContext() 
    |> Db.getGenres 
    |> List.map (fun g -> g.Name) 
    |> View.store 
    |> html)
```

That's because our `overview` WebPart is in some sense static - there is no parameter for it that could influence the outcome.
`warbler` ensures that genres will be fetched from the database whenever a new request comes.
Otherwise, without the `warbler` in place, the genres would be fetched only at the start of the application - resulting in stale genres in case the list changes.
How about the rest of WebParts?

- `browse` is parametrized with the genre name - each request will result in a database query.
- `details` is parametrized with the id - the same as above applies.
- `home` is just fine - for the moment it's completely static and doesn't need to touch the database.
