Suave Music Store
=================

Introduction
------------

This is a tutorial on how to create an application with [F#](http://fsharp.org) and [Suave.IO](http://suave.io) framework. 
It's inspired by the Music Store tutorial created by the ASP.NET team [available here](http://www.asp.net/mvc/overview/older-versions/mvc-music-store/mvc-music-store-part-1).
Check out this link if you want to find out what the application is going to offer.

Target audience for this tutorial are mainly C# developers familiar with ASP.NET MVC, who want to learn how to write a real application in F#.
You can still benefit from the tutorial if you don't have C# / .NET background, however you may find some aspects not clear - From time to time there will be a comparison with how the same functionality could be written in ASP.NET MVC & C#.
No prior experience with F# is required - the tutorial will cover basic concepts of the language.
The tutorial is going to have plenty of references to the awesome [fsharpforfunandprofit.com](http://fsharpforfunandprofit.com) WebSite, which includes plenty of articles about F# written by Scott Wlaschin.

For most of the following sections there will be a direct link to a specific tagged commit that contains implementation of the application up to the point.
This allows you to follow along the process of creating the app, and get back on the track in case of any amibiguity.

Visual Studio 2013 is used throughout the tutorial, but of course you can use IDE of your choice.

Hello World from Suave
----------------------

Suave application can be hosted as a standalone Console Application. 
Let's start by creating a Console Application Project named `SuaveMusicStore` (to keep all the files in single folder, uncheck the option to create folder for solution).
Now we can add NuGet reference to Suave. To do that, in Package Manager Console type: 
```install-package Suave -version 0.26.2```. 
Alternatively, you can use the NuGet GUI to find and install the Suave package.
Rename the `Program.fs` file to `App.fs` to better reflect the purpose of the file, and replace its contents completely with the following code:


```
open Suave                 // always open suave
open Suave.Http.Successful // for OK-result
open Suave.Web             // for config

startWebServer defaultConfig (OK "Hello World!")
```

Guess what, if you press F5 to run the project, your application is now up and running!
By default it should be available under `http://localhost:8083`.
If you browse that url, you should be greeted with the classic `Hello World!`.
The `open` statements at the top of the file are the same as `using` statements in C#.
Note there is no `Main` method defined in `App.fs` - what happens here is that the `startWebServer` function is invoked immediately after the program is run and Suave starts listening for incoming request till the process is killed.

[Tag - hello_world](https://github.com/theimowski/SuaveMusicStore/tree/hello_world)

WebPart
-------

You may be wondering what the above code actually does. `startWebServer` is a function from Suave library of type: `SuaveConfig -> WebPart -> unit`. This basically means that it takes object of type `SuaveConfig` as first argument, `WebPart` as second argument, and returns `unit`.

`defaultConfig` is a value from the Suave library of type `SuaveConfig`, and as the name suggest it determines the default configuration of server. For now we only need to know that among other stuff it configures the HTTP binding, meaning that in our case the server is listening on port 8083 on loopback address 127.0.0.1.

`unit` in F# is a type for which there is only one applicable value, namely `()`. It is analogous to C# `void`, with the difference that you cannot return `void`. If you'd like to learn more about the `unit` type and how it differs from `void`, [follow this link](https://msdn.microsoft.com/en-us/library/dd483472.aspx).

The most interesting part is the `WebPart`. `WebPart` is a type alias for the following type: 
`HttpContext -> Async<HttpContext option>`
This means that `WebPart` is actually a function, which takes objects of type `HttpContext` as its first argument and returns an "asynchronous workflow" of `HttpContext option`.

`HttpContext` gathers information about the incoming request, outcoming response and other data.
`Async`, which is often called "asynchronous workflow" is a concept like "promise" or "future" in some other programming languages. C# 5 introduces the Async / Await feature, which is somewhat similar to F# Async. [Here](http://fsharpforfunandprofit.com/posts/concurrency-async-and-parallel/) is a great F# resource on async workflows and asynchronous programming in general. 
One of the biggest advantages F# has over C# is that it doesn't allow nulls. F# compiler prevents you from passing null as an argument, and as a result you will no longer have to deal with the infamous `NullReferenceException`. 
But what do you do if you want to say that something may have a value or may not have any value at all?
That's where the `Option` type comes in. Object of type `Option` might be: `None` or `Some x`.
`None` means there's no value, `Some x` means there is a value `x` and `x` is not null.

To sum up, one could explain `WebPart` in these words: 

> Based on the http context, we give you a promise (async) of optional resulting http context, where the resulting context is likely to have its response set with regards to the logic of the WebPart itself

In our case, `(OK "Hello World!")` is the simplest `WebPart` possible. No matter what comes in the HttpContext, we always return an async of `Some` `HttpContext` with response of HTTP Result `200 OK` and "Hello World!" in the response body.

To some extent you could think of the `WebPart` as a `Filter` in ASP.NET MVC application, but there's more to it than `Filters` can do.

If the above explanation of `WebPart` doesn't yet make much sense to you, or you don't understand why it has such type signature, bear with me - in next sections we'll try to prove that this type turns out to be really handy when it comes to one of the greatest powers of functional programming paradigm: **composability**.

Basic routing
-------------

It's time to extend our WebPart to support multiple routes.
First, let's extract the WebPart and bind it to an identifier. 
We can do that by typing:
`let webPart = OK "Hello World"` 
and using the `webPart` identifier in our call to function `startWebServer`:
`startWebServer defaultConfig webPart`.
In C#, one would call it "assign webPart to a variable", but in functional world there's really no concept of a variable. Instead, we can "bind" a value to an identifier, which we can reuse later.
Value, once bound, can't be mutated during runtime.
Now, let's restrict our WebPart, so that the "Hello World" response is sent only at the root path of our application (`localhost:8083/` but not `localhost:8083/anything`):
`let webPart = path "/" >>= OK "Hello World"`
`path` function is defined in `Suave.Http.Applicatives` module, thus we need to open it at the beggining of `App.fs`. `Suave.Http` and `Suave.Types` modules will also be crucial - let's open them as well.

`path` is a function of type:
`string -> WebPart`
It means that if we give it a string it will return WebPart.
Under the hood, the function looks at the incoming request and returns `Some` if the paths match, and `None` otherwise.
The `>>=` operator comes also from Suave library. It composes two WebParts into one by first evaluating the WebPart on the left, and applying the WebPart on the right only if the first one returned `Some`.

Let's move on to configuring a few routes in our application. 
To achieve that, we can use the `choose` function, which takes a list of WebParts, and chooses the first one that applies (returns `Some`), or if none WebPart applies, then choose will also return `None`:

```
let webPart = 
    choose [
        path "/" >>= (OK "Home")
        path "/store" >>= (OK "Store")
        path "/store/browse" >>= (OK "Store")
        path "/store/details" >>= (OK "Details")
    ]
```

In addition to that static string path, we can specify route arguments.
Suave comes with a cool feature called "typed routes", which gives you statically typed control over arguments for your route. As an example, let's see how we can add `id` of an album to the details route:

`pathScan "/store/details/%d" (fun id -> OK (sprintf "Details: %d" id))`

This might look familiar to print formatting from C++, but it's more powerful.
What happens here is that the compiler checks the type for the `%d` argument and complains if you pass it a value which is not an integer.
The WebPart will apply for requests like `http://localhost:8083/store/details/28`
In the above example, there are a few important aspects:
- `sprintf "Details: %d" id` is statically typed string formatting function, expecting the id as an integer 
- `(fun id -> OK ...)` is an anonymous function or lambda expression if you like, of type `int -> WebPart`
- the lambda expression is passed as the second parameter to `pathScan` function
- first argument of `pathScan` function also works as a statically typed format
- type inference mechanism built into F# glues everything together, so that you do not have to mark any type signatures

To clear things up, here is another example of how one could use typed routes in Suave:

`pathScan "/store/details/%s/%d" (fun a id -> OK (sprintf "Artist: %s; Id: %d" a id))`

for request `http://localhost:8083/store/details/abba/1`

For more information on working with strings in a statically typed way, visit [this site](http://fsharpforfunandprofit.com/posts/printf/)

Apart from passing arguments in the route itself, we can use the query part of url:
`localhost:8083/store/browse?genre=Disco`
To do this, let's create a separate WebPart:

```
let browse =
    request (fun r -> 
        match r.queryParam "genre" with
        | Some genre -> OK (sprintf "Genre: %s" genre)
        | None -> never)
```

`request` is a function that takes as parameter a function of type `HttpRequest -> WebPart`.
A function which takes as an argument another function is often called "Higher order function".
`r` in our lambda represents the `HttpRequest`. It has a `queryParam` member function of type 
`string -> string option`. This member is almost identical to how one would do `Dictionary.TryGetValue` in C#, except that we don't have to deal with `out` parameters. Instead, the function returns `string option` on which we can apply pattern matching.
Pattern matching is yet another really powerful feature, implemented in variety of modern languages. 
For now we can think of it as a switch statement with binding value to an identifier in one go.
In addition to that, F# compiler will issue an warning in case we don't provide all possible cases (`None` and `Some x` here).
There's actually much more for pattern matching than that, as we'll discover later.
`never` is a function from Suave library, which returns WebPart with `None`.
We can summarize the `browse` WebPart as following:
If there is a "genre" parameter in the url query, return 200 OK with the value of the "genre", otherwise don't bother.

Now we can compose the `browse` WebPart with routing WebPart like this:

`path "/store/browse" >>= browse`

Eventually we should end up with something similar to: [Tag - basic_routing](https://github.com/theimowski/SuaveMusicStore/tree/basic_routing)

Views
-----

We've seen how to define basic routing in a Suave application. 
In this section we'll see how we can deal with returning good looking HTML markup in a HTTP response.
Templating HTML views is quite a big topic itself, that we don't want to go into much details about.
Keep in mind that the concept can be approached in many different ways, and the way presented here is not the only proper way of rendering HTML views.
Having said that, I hope you'll still find the following implementation concise and easy to understand.
In this application we'll use server-side HTML templating with the help of a seperate Suave package called `Suave.Experimental`.

> Note: As of the time of writing, `Suave.Experimental` is a separate package. It's likely that next releases of the package will include breaking changes. It's also possible that the modules we're going to use from within the package will be extracted to the core Suave package.

To use the package, we need to take a dependency on the following NuGet:
```install-package Suave.Experimental -version 0.26.2```

Before we start defining views, let's organize our `App.fs` source file by adding following line at the beginning of the file:

```
module SuaveMusicStore.App

open Suave
...
```

The line means that whatever we define in the file will be placed in `SuaveMusicStore.App` module.
Read [here](http://fsharpforfunandprofit.com/posts/recipe-part3/) for more info about organizing and structuring F# code.
Now let's add a new file `View.fs` to the project just before the `App.fs` file and place the following module definition at the very top:

```
module SuaveMusicStore.View
```

We'll follow this convention throughout the tutorial to have a clear understanding of the project structure.

> Note: It's very important that the `View.fs` file comes before `App.fs`. F# compiler requires the referenced items to be defined before their usage. At first glance, that might seem like a big drawback, however after a while you start realizing that you can have much better control of your dependencies. Read the [following](http://fsharpforfunandprofit.com/posts/cyclic-dependencies/) for further benefits of lack of cyclic dependencies in F# project.

With the `View.fs` file in place, let's add our first view:

```
module SuaveMusicStore.View

open Suave.Html

let divId id = divAttr ["id", id]
let h1 xml = tag "h1" [] xml
let aHref href = tag "a" ["href", href]

let index = 
    html [
        head [
            title "Suave Music Store"
        ]

        body [
            divId "header" [
                h1 (aHref "/" (text "F# Suave Music Store"))
            ]

            divId "footer" [
                text "built with "
                aHref "http://fsharp.org" (text "F#")
                text " and "
                aHref "http://suave.io" (text "Suave.IO")
            ]
        ]
    ]
    |> xmlToString
```

This will serve as a common layout in our application.
A few remarks about the above snippet:

- open `Suave.Html` module, for functions to generate HTML markup.
- 3 helper functions come next:
    - `divId` which appends "div" element with a string attribute `id`
    - `h1` which takes inner markup to generate HTML header level 1.
    - `aHref` which takes string attribute `href` and inner HTML markup to output "a" element.
- `tag` function comes from Suave. It's of type `string -> Attribute list -> Xml -> Xml`. First arg is name of the HTML element, second - a list of attributes, and third - inner markup
- `Xml` is an internal Suave type holding object model for the HTML markup
- `index` is our representation of HTML markup. 
- `html` is a function that takes a list of other tags as its argument. So do `head` and `body`.
- `text` serves outputing plain text into an HTML element.
- `xmlToString` transformates the object model into the resulting raw HTML string.

> Note: `tag` function from Suave takes 3 arguments ().
> We've defined the `aHref` function by invoking `tag` with only 2 arguments, and the compilator is perfectly happy with that - Why?
> This concept is called "partial application", and allows us to invoke a function by passing only a subset of arguments.
> When we invoke a function with only a subset of arguments, the function will return another function that will expect the rest of arguments.
> In our case this means `aHref` is of type `string -> Xml -> Xml`, so the second "hidden" argument to `aHref` is of type `Xml`.
> Read [here](http://fsharpforfunandprofit.com/posts/partial-application/) for more info about partial application.

We can see usage of the "pipe" operator `|>` in the above code. 
The operator might look familiar if you have some UNIX background.
In F#, the `|>` operator basically means: take the value on the left side and apply it to the function on the right side of the operator.
In this very case it simply means: invoke the `xmlToString` function on the HTML object model.

Let's test the `index` view in our `App.fs`:
```
    path "/" >>= (OK View.index)
```

If you navigate to the root url of the application, you should see that proper HTML has been returned.

Before we move on to defining views for the rest of the application, let's introduce one more file - `Path.fs` and insert it **before** `View.fs`:

```
module SuaveMusicStore.Path

type IntPath = PrintfFormat<(int -> string),unit,string,string,int>

let home = "/"

module Store =
    let overview = "/store"
    let browse = "/store/browse"
    let details : IntPath = "/store/details/%d"
```

The module will contain all valid routes in our application.
We'll keep them here in one place, in order to be able to reuse both in `App` and `View` modules.
Thanks to that, we will minimize the risk of a typo in our `View` module.
We defined a submodule called `Store` in order to group routes related to one functionality - later in the tutorial we'll have a few more submodules, each of them reflecting a specific set of functionality of the application.

The `IntPath` type alias that we declared will let use our routes in conjunction with static-typed Suave routes (`pathScan` in `App` module). 
We don't need to fully understand the signature of this type, for now we can think of it as a route parametrized with integer value.
And indeed, we annotated the `details` route with this type, so that the compiler treats this value *specially*. 
We'll see in a moment how we can use `details` in `App` and `View` modules, with the advantage of static typing.

Let's use the routes from `Path` module in our `App`:

```
let webPart = 
    choose [
        path Path.home >>= (OK View.index)
        path Path.Store.overview >>= (OK "Store")
        path Path.Store.browse >>= browse
        pathScan Path.Store.details (fun id -> OK (sprintf "Details %d" id))
    ]
```

as well as in our `View` for `aHref` to `home`:

```
    divId "header" [
        h1 (aHref Path.home (text "F# Suave Music Store"))
    ]
```

Note, that in `App` module we still benefit from the static typed routes feature that Suave gives us - the `id` parameter is inferred by the compiler to be of integer type.
If you're not familiar with type inference mechanism, you can follow up [this link](http://fsharpforfunandprofit.com/posts/type-inference/).

It's high time we added some CSS styles to our HTML markup.
We'll not deepdive into the details about the styles itself, as this is not a tutorial on Web Design.
The stylesheet can be downloaded [from here](https://raw.githubusercontent.com/theimowski/SuaveMusicStore/master/Site.css) in its final shape.
Add the `Site.css` stylesheet to the project, and don't forget to set the `Copy To Output Directory` property to `Copy If Newer`.

In order to include the stylesheet in our HTML markup, let's add the following to our `View`:

```
let cssLink href = linkAttr [ "href", href; " rel", "stylesheet"; " type", "text/css" ]

let index = 
    html [
        head [
            title "Suave Music Store"
            cssLink "/Site.css"
        ]
...
```

This enables us to output the link HTML element with `href` attribute pointing to the CSS stylesheet.

There's two more things before we can see the styles applied on our site.

A browser, when asked to include a CSS file, sends back a request to the server with the given url.
If we have a look at our main `WebPart` we'll notice that there's really no handler capable of serving this file.
That's why we need to add another alternative to our `choose` `WebPart`:

```
    pathRegex "(.*)\.css" >>= Files.browseHome
```

The `pathRegex` `WebPart` returns `Some` if an incoming request concerns path that matches the regular expression pattern. 
If that's the case, the `Files.browseHome` WebPart will be applied.
`(.*)\.css` pattern matches every file with `.css` extension.
`Files.browseHome` is a `WebPart` from Suave that serves static files from the root application directory.

The CSS depends on `logo.png` asset, which can be downloaded from [here](https://raw.githubusercontent.com/theimowski/SuaveMusicStore/master/logo.png).

Add `logo.png` to the project, and again don't forget to select `Copy If Newer` for `Copy To Output Directory` property for the asset.
Again, when browser wants to render an image asset, it needs to GET it from the server, so we need to extend our regular expression to allow browsing of `.png` files as well:

```
    pathRegex "(.*)\.(css|png)" >>= Files.browseHome
```

Now you should be able to see the styles applied to our HTML markup.

With styles in place, let's get our hands on extracting a shared layout for all future views to come.
Start by adding `container` parameter to `index` in `View`:

```
let index container = 
    html [
...
```

and div with id "container" just after the div "header":

```
    divId "header" [
        h1 (aHref Path.home (text "F# Suave Music Store"))
    ]

    divId "container" container
```

`index` previosly was a constant value, but now became a function taking `container` as parameter.

We can now define actual container for the "home" page:

```
let home = [
    text "Home"
]
```

For now it will only contain plain "Home" text.
Let's also extract a common function for creating WebPart, parametrized with the container itself.
Add to `App` module, just before the `browse` WebPart the following:

```

let html container =
    OK (View.index container)

```

Usage for the home page looks like this:

```
    path Path.home >>= html View.home
```

Next, containers for each valid route in our application can be defined in `View` module:

```
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

```
let browse =
    request (fun r -> 
        match r.queryParam "genre" with
        | Some genre -> html (View.browse genre)
        | None -> never)

let webPart = 
    choose [
        path Path.home >>= html View.home
        path Path.Store.overview >>= html View.store
        path Path.Store.browse >>= browse
        pathScan Path.Store.details (fun id -> html (View.details id))

        pathRegex "(.*)\.(css|png)" >>= Files.browseHome
    ]
```

It's time to replace plain text placeholders in containers with meaningful content.
First, define `h2` in `View` module to output HTML header of level 2:

```
let h2 s = tag "h2" [] (text s)
```

and replace `text` with new `h2` in each of 4 containers.

We'd like the "/store" route to output hyperlinks to all genres in our Music Store.
Let's add a helper function in `Path` module, that will be responsible for formatting HTTP url with a key-value parameter:

```
let withParam (key,value) path = sprintf "%s?%s=%s" path key value
```

The `withParam` function takes a tuple `(key,value)` as its first argument, `path` as the second and returns properly formatted url.
A tuple (or a pair) is a widely used structure in F#. It allows us to group two values into one in an easy manner. 
Syntax for creating a tuple is following: `(item1, item2)` - this might look like a standard parameter passing in many other languages including C#.
Follow [this link](http://fsharpforfunandprofit.com/posts/tuples/) to learn more about tuples.

Add also a string key for the url parameter "/store/browse" in `Path.Store` module:

```
    let browseKey = "genre"
```

We'll use it in `App` module:

```
    match r.queryParam Path.Store.browseKey with
    ...
```

Now, add the following for working with unordered list (`ul`) and list item (`li`) elements in HTML:

```
let ul xml = tag "ul" [] (flatten xml)
let li = tag "li" []
```

`flatten` takes a list of `Xml` and "flattens" it into a single `Xml` object model.
The actual container for `store` can now look like following:

```
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

Things worth commenting in above snippet:

- `store` now takes a list of genres (again the type is inferred by the compiler)
- the `[ for g in genres -> ... ]` syntax is known as "list comprehension". Here we map every genre string from `genres` to a list item
- `aHref` inside list item points to the `Path.Store.browse` url with "genre" parameter - we use the `Path.withParam` function defined earlier

To use `View.store` from `App` module, let's simply pass a hardcoded list for `genres` like following:

```
    path Path.Store.overview >>= html (View.store ["Rock"; "Disco"; "Pop"])
```

Here is what the solution looks like up to this point: [Tag - View](https://github.com/theimowski/SuaveMusicStore/tree/view)

Database
--------

In this section we'll see how to add data access to our application.
We'll use SQL Server for the database - you can use the Express version bundled with Visual Studio.
Download the [`create.sql` script](https://raw.githubusercontent.com/theimowski/SuaveMusicStore/master/create.sql) to create `SuaveMusicStore` database.

There are many ways to talk with a database from .NET code including ADO.NET, light-weight libraries like Dapper, ORMs like Entity Framework or NHibernate.
To have more fun, we'll do something completely different, namely try out an awesome F# feature called Type Providers.
In short, Type Providers allows to automatically generate a set of types based on some type of schema.
To learn more about Type Providers, check out [this resource](https://msdn.microsoft.com/en-us/library/hh156509.aspx).

SQLProvider is example of a Type Provider library, which gives ability to cooperate with a relational database.
We can install SQLProvider from NuGet:
```install-package SQLProvider -includeprerelease```

> Note: SQLProvider is marked on NuGet as a "prerelease". While it could be risky for more sophisticated queries, we are perfectly fine to use it in our case, as it fullfills all of our data access requirements.

If you're using Visual Studio, a dialog window can pop asking to confirm enabling the Type Provider. 
This is just to notify about capability of the Type Provider to execute its custom code while in design time.

Let's also add reference to `System.Data` assembly.

Having installed the SQLProvider, let's add `Db.fs` file to the beginning of our project - before any other `*.fs` file.

In the newly created file, open following module:

```
module SuaveMusicStore.Db

open FSharp.Data.Sql
```

Next, comes the most interesting part:

```
type Sql = 
    SqlDataProvider< 
        "Server=(LocalDb)\\v11.0;Database=SuaveMusicStore;Trusted_Connection=True;MultipleActiveResultSets=true", 
        DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER >
```

You'll need to adjust the above connection string, so that it can access the `SuaveMusicStore` database.
After the SQLProvider can access the database, it will generate a set of types in background - each for single database table, as well as each for single database view.
This might be similar to how Entity Framework generates models for your tables, except there's no explicit code generation involved - all of the types reside under the defined `Sql` type.

The generated types have cumbersome names, but we can define type aliases as needed:

```
type DbContext = Sql.dataContext
type Album = DbContext.``[dbo].[Albums]Entity``
type Genre = DbContext.``[dbo].[Genres]Entity``
type AlbumDetails = DbContext.``[dbo].[AlbumDetails]Entity``
```

`DbContext` is our data context.
`Album` and `Genre` reflect database tables.
`AlbumDetails` reflects database view - it will prove useful when we'll need to display names for the album's genre and artist.

With the type aliases set up, we can move forward to creating our first queries:

```
let firstOrNone s = s |> Seq.tryFind (fun _ -> true)

let getGenres (ctx : DbContext) : Genre list = 
    ctx.``[dbo].[Genres]`` |> Seq.toList

let getAlbumsForGenre genreName (ctx : DbContext) : Album list = 
    query { 
        for album in ctx.``[dbo].[Albums]`` do
            join genre in ctx.``[dbo].[Genres]`` on (album.GenreId = genre.GenreId)
            where (genre.Name = genreName)
            select album
    }
    |> Seq.toList

let getAlbumDetails id (ctx : DbContext) : AlbumDetails option = 
    query { 
        for album in ctx.``[dbo].[AlbumDetails]`` do
            where (album.AlbumId = id)
            select album
    } |> firstOrNone
```

`getGenres` is a function for finding all genres. 
The function, as well as all functions we'll define in `Db` module, takes the `DbContext` as a parameter.
The `: Genre list` part is a type annotation making sure the function returns a list of `Genre`s.
Implementation is straight forward:  ```ctx.``[dbo].[Genres]`` ``` queries all genres, so we just need to pipe it to the `Seq.toList`.

`getAlbumsForGenre` takes `genreName` as argument (infered to be of type string) and returns a list of `Album`s.
It makes use of "query builder" (`query { }`) which is very similar to C# Linq query.
Inside the query builder, we're performing an inner join of `Albums` and `Genres` with the `GenreId` foreign key, and then we apply a predicate on `genre.Name` to match the input `genreName`.
The result of the query is piped to `Seq.toList`.

`getAlbumDetails` takes `id` as argument (infered to be of type int) and returns `AlbumDetails option` because there might be no Album with the given id.
Here, the result of the query is piped to the `firstOrNone` function, which takes care to transform the result to `option` type.
`firstOrNone` verifies if a query returned any result.
In case of any result, `firstOrNone` will return `Some x`, otherwise `None`.

For more convenient instantiation of `DbContext`, let's introduce a small helper function in `Db` module:

```
let getContext() = Sql.GetDataContext()
```

Now we're ready to finally read real data in the `App` module:

```
let overview =
    Db.getContext() 
    |> Db.getGenres 
    |> List.map (fun g -> g.Name) 
    |> View.store 
    |> html

...

    path Path.Store.overview >>= overview
```

`overview` is a WebPart that... 
Hold on, do I really need to explain it?
The usage of pipe operator here makes the flow rather obvious - each line defines next step.
The return value is passed from one function to another, starting with DbContext and ending with the WebPart.
This is just a single example of how composability in functional programming leads to thinking of your functions as building blocks "glued" together.

Moving to our next WebPart "browse", let's first adjust it in `View` module:

```
let browse genre (albums : Db.Album list) = [
    h2 (sprintf "Genre: %s" genre)
    ul [
        for a in albums ->
            li (aHref (sprintf Path.Store.details a.AlbumId) (text a.Title))
    ]
]
```

so that it takes two arguments: name of the genre (string) and a list of albums for that genre.
For each album we'll display a listitem with a direct link to album details.

> Note: Here we used the `Path.Store.details` of type `IntPath` in conjunction with `sprintf` function to format the direct link. Again this gives us safety in regards to static typing.

Now, we can modify the `browse` WebPart itself:

```
let browse =
    request (fun r -> 
        match r.queryParam Path.Store.browseKey with
        | Some genre -> 
            Db.getContext()
            |> Db.getAlbumsForGenre genre
            |> View.browse genre
            |> html
        | None -> never)
```

Again, usage of pipe operator makes it clear what happens in case the `genre` is resolved from the query parameter.

> Note: in the example above we did "partial application", both for `Db.getAlbumsForGenre` and `View.browse`. This could be achieved because the return type between those pipes comes as the last argument to those functions.

