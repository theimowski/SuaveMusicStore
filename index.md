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

You're more than welcome to create issues or pull requests for this tutorial [here](https://github.com/theimowski/SuaveMusicStore).

Hello World from Suave
----------------------

Suave application can be hosted as a standalone Console Application. 
Let's start by creating a Console Application Project named `SuaveMusicStore` (to keep all the files in single folder, uncheck the option to create folder for solution).
Now we can add NuGet reference to Suave. To do that, in Package Manager Console type: 
```install-package Suave -version 0.28.1```. 
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
        | Choice1Of2 genre -> OK (sprintf "Genre: %s" genre)
        | Choice2Of2 msg -> BAD_REQUEST msg)
```

`request` is a function that takes as parameter a function of type `HttpRequest -> WebPart`.
A function which takes as an argument another function is often called "Higher order function".
`r` in our lambda represents the `HttpRequest`. It has a `queryParam` member function of type 
`string -> Choice<string,string>`. `Choice` is a type that represents a choice between two types.
Usually you'll find that the first type of `Choice` is for happy paths, while second means something went wrong.
In our case first string stands for a value of the query parameter, and the second string stands for error message (paramater with given key was not found in query).
We can make use of pattern matching to distinguish between two possible choices.
Pattern matching is yet another really powerful feature, implemented in variety of modern programming languages. 
For now we can think of it as a switch statement with binding value to an identifier in one go.
In addition to that, F# compiler will issue an warning in case we don't provide all possible cases (`Choice1Of2 x` and `Choice2Of2 x` here).
There's actually much more for pattern matching than that, as we'll discover later.
`BAS_REQUEST` is a function from Suave library, and it returns WebPart with 400 Bad Request status code response with given message in its body.
We can summarize the `browse` WebPart as following:
If there is a "genre" parameter in the url query, return 200 OK with the value of the "genre", otherwise return 400 Bad Request with error message.

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
```install-package Suave.Experimental -version 0.28.1```

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
        | Choice1Of2 genre -> html (View.browse genre)
        | Choice2Of2 msg -> BAD_REQUEST msg)

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
To have more fun, we'll do something completely different, namely try an awesome F# feature called Type Providers.
In short, Type Providers allows to automatically generate a set of types based on some type of schema.
To learn more about Type Providers, check out [this resource](https://msdn.microsoft.com/en-us/library/hh156509.aspx).

SQLProvider is example of a Type Provider library, which gives ability to cooperate with a relational database.
We can install SQLProvider from NuGet:
```install-package SQLProvider -includeprerelease```

> Note: SQLProvider is marked on NuGet as a "prerelease". While it could be risky for more sophisticated queries, we are perfectly fine to use it in our case, as it fullfills all of our data access requirements.

If you're using Visual Studio, a dialog window can pop asking to confirm enabling the Type Provider. 
This is just to notify about capability of the Type Provider to execute its custom code in design time.
To be sure the SQLProvider is referenced correctly, select "enable".

Let's also add reference to `System.Data` assembly.

Having installed the SQLProvider, let's add `Db.fs` file to the beginning of our project - before any other `*.fs` file.

In the newly created file, open `FSharp.Data.Sql` module:

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
This might be similar to how Entity Framework generates models for your tables, except there's no explicit code generation involved - all of the types reside under the `Sql` type defined.

The generated types have a bit cumbersome names, but we can define type aliases to keep things simpler:

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
The `: Genre list` part is a type annotation, which makes sure the function returns a list of `Genre`s.
Implementation is straight forward:  ```ctx.``[dbo].[Genres]`` ``` queries all genres, so we just need to pipe it to the `Seq.toList`.

`getAlbumsForGenre` takes `genreName` as argument (infered to be of type string) and returns a list of `Album`s.
It makes use of "query expression" (`query { }`) which is very similar to C# Linq query.
Read [here](https://msdn.microsoft.com/en-us/library/hh225374.aspx) for more info about query expressions.
Inside the query expression, we're performing an inner join of `Albums` and `Genres` with the `GenreId` foreign key, and then we apply a predicate on `genre.Name` to match the input `genreName`.
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
The usage of pipe operator here makes the flow rather obvious - each line defines each step.
The return value is passed from one function to another, starting with DbContext and ending with the WebPart.
This is just a single example of how composition in functional programming makes functions look like building blocks "glued" together.

We also need to wrap the `overview` WebPart in a `warbler`:

```
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
For each album we'll display a list item with a direct link to album's details.

> Note: Here we used the `Path.Store.details` of type `IntPath` in conjunction with `sprintf` function to format the direct link. Again this gives us safety in regards to static typing.

Now, we can modify the `browse` WebPart itself:

```
let browse =
    request (fun r ->
        match r.queryParam Path.Store.browseKey with
        | Choice1Of2 genre -> 
            Db.getContext()
            |> Db.getAlbumsForGenre genre
            |> View.browse genre
            |> html
        | Choice2Of2 msg -> BAD_REQUEST msg)
```

Again, usage of pipe operator makes it clear what happens in case the `genre` is resolved from the query parameter.

> Note: in the example above we adopted "partial application", both for `Db.getAlbumsForGenre` and `View.browse`. 
> This could be achieved because the return type between the pipes is the last argument for these functions.

If you navigate to "/store/browse?genre=Latin", you may notice there are some characters displayed incorrectly.
Let's fix this by setting the "Content-Type" header with correct charset for each HTTP response:

```
let html container =
    OK (View.index container)
    >>= Writers.setMimeType "text/html; charset=utf-8"
```

It's time to read album's details from the database. 
Start by adjusting the `details` in `View` module:

```
let details (album : Db.AlbumDetails) = [
    h2 album.Title
    p [ imgSrc album.AlbumArtUrl ]
    divId "album-details" [
        for (caption,t) in ["Genre:",album.Genre;"Artist:",album.Artist;"Price:",formatDec album.Price] ->
            p [
                em caption
                text t
            ]
    ]
]
```

Above snippet requires defining a few more helper functions in `View`:

```
let imgSrc src = imgAttr [ "src", src ]
let em s = tag "em" [] (text s)

let formatDec (d : Decimal) = d.ToString(Globalization.CultureInfo.InvariantCulture)
```

as well as opening the `System` namespace at the top of the file.

> Note: It's a good habit to open the `System` namespace every single time - in practice it usually turns out to be helpful.

In the `details` function we used list comprehension syntax with an inline list of tuples (`["Genre:",album.Genre;...`).
This is just to save us some time from typing the `p` element three times for all those properties.
You're welcome to change the implementation so that it doesn't use this shortcut if you like.

The `AlbumDetails` database view turns out to be handy now, because we can use all the attributes we need in a single step (no explicit joins required).

To read the album's details in `App` module we can do following:

```
let details id =
    match Db.getAlbumDetails id (Db.getContext()) with
    | Some album ->
        html (View.details album)
    | None ->
        never

...

pathScan Path.Store.details details
```

A few remarks regarding above snippet:

- `details` takes `id` as parameter and returns WebPart
- `Path.Store.details` of type IntPath guarantees type safety
- `Db.getAlbumDetails` can return `None` if no album with given id is found
- If an album is found, html WebPart with the `View.details` container is returned
- If no album is found, `None` WebPart is returned with help of `never`.

No pipe operator was used this time, but as an exercise you can think of how you could apply it to the `details` WebPart.

Before testing the app, add the "placeholder.gif" image asset. 
You can download it from [here](https://raw.githubusercontent.com/theimowski/SuaveMusicStore/master/placeholder.gif).
Don't forget to set "Copy To Output Directory", as well as add new file extension to the `pathRegex` in `App` module.

You might have noticed, that when you try to access a missing resource (for example entering album details url with arbitrary album id) then no response is sent.
In order to fix that, let's add a "Page Not Found" handler to our main `choose` WebPart as a last resort:

```
let webPart = 
    choose [
        ...

        html View.notFound
    ]
```

the `View.notFound` can then look like:

```
let notFound = [
    h2 "Page not found"
    p [
        text "Could not find the requested resource"
    ]
    p [
        text "Back to "
        aHref Path.home (text "Home")
    ]
]
```

Results of the section can be seen here: [Tag - Database](https://github.com/theimowski/SuaveMusicStore/tree/database)

CRUD and Forms
--------------

With the database in place, we can now move to implementing a management module.
This will be a simple Create, Update, Delete functionality with a grid to display all albums in the store.

Let's start by adding `manage` to our `View` module:

```
let manage (albums : Db.AlbumDetails list) = [ 
    h2 "Index"
    table [
        yield tr [
            for t in ["Artist";"Title";"Genre";"Price"] -> th [ text t ]
        ]

        for album in albums -> 
        tr [
            for t in [ truncate 25 album.Artist; truncate 25 album.Title; album.Genre; formatDec album.Price ] ->
                td [ text t ]
        ]
    ]
]
```

The view requires a few of new helper functions for table HTML markup:

```
let table x = tag "table" [] (flatten x)
let th x = tag "th" [] (flatten x)
let tr x = tag "tr" [] (flatten x)
let td x = tag "td" [] (flatten x)
```

as well as a `truncate` function that will ensure our cell content doesn't span over a maximum number of characters:

```
let truncate k (s : string) =
    if s.Length > k then
        s.Substring(0, k - 3) + "..."
    else s
```

Remarks:

- our HTML table consists of first row (`tr`) containing column headers (`th`) and a set of rows for each album with cells (`td`) to display specific values.
- we used the `yield` keyword for the first time. It is required here because we're using it in conjuction with the `for album in albums ->` list comprehension syntax inside the same list. The rule of thumb is that whenever you use the list comprehension syntax, then you need the `yield` keyword for any other item not contained in the comprehension syntax. This might seem hard to remember, but don't worry - the compiler is helpful here and will issue a warning if you forget the `yield` keyword.
- for the sake of saving a few keystrokes we used a nested list comprehension syntax to output `th`s and `td`s. Again, it's just a matter of taste, and could be also solved by enumerating each element separately

We are going to need to fetch the list of all `AlbumDetail`s from the database. 
For this reason, let's create following query in `Db` module:

```
let getAlbumsDetails (ctx : DbContext) : AlbumDetails list = 
    ctx.``[dbo].[AlbumDetails]`` |> Seq.toList
```

Now we're ready to define an actual handler to display the list of albums.
Let's add a new sub-module to `Path`:

```
module Admin =
    let manage = "/admin/manage"
```

The `Admin` sub-module will contain all album management paths or routes if you will.

`manage` WebPart in `App` module can be implemented in following way:

```
let manage = warbler (fun _ ->
    Db.getContext()
    |> Db.getAlbumsDetails
    |> View.manage
    |> html)
```

and used in the main `choose` WebPart:

```
    path Path.Admin.manage >>= manage
```

Don't forget about the `warbler` for `manage` WebPart - we don't use an parameters for this WebPart, so we need to prevent it's eager evaluation.

If you navigate to the "/admin/manage" url in the application now, you should be presented the grid with every album in the store.
We can't make any operation on an album yet.
To fix this, let's first add the delete functionality:

```
let deleteAlbum albumTitle = [
    h2 "Delete Confirmation"
    p [ 
        text "Are you sure you want to delete the album titled"
        br
        strong albumTitle
        text "?"
    ]
    
    form [
        submitInput "Delete"
    ]

    div [
        aHref Path.Admin.manage (text "Back to list")
    ]
]
```

`deleteAlbum` is to be placed in the `View` module. It requires new markup functions:

```
let strong s = tag "strong" [] (text s)

let form x = tag "form" ["method", "POST"] (flatten x)
let submitInput value = inputAttr ["type", "submit"; "value", value]
```

- `strong` is just an emphasis
- `form` is HTML element for a form with it's "method" attribute set to "POST"
- `submitInput` is button to submit a form

A couple of snippets to handle `deleteAlbum` are still needed, starting with `Db`:

```
let getAlbum id (ctx : DbContext) : Album option = 
    query { 
        for album in ctx.``[dbo].[Albums]`` do
            where (album.AlbumId = id)
            select album
    } |> firstOrNone
```

for getting `Album option` (not `AlbumDetails`). 
New route in `Path`:

```
module Admin =
    let manage = "/admin/manage"
    let deleteAlbum : IntPath = "/admin/delete/%d"
```

Finally we can put following in the `App` module:

```
let deleteAlbum id =
    match Db.getAlbum id (Db.getContext()) with
    | Some album ->
        html (View.deleteAlbum album.Title)
    | None ->
        never
```

```
    pathScan Path.Admin.deleteAlbum deleteAlbum
```

Note that the code above allows us to navigate to to "/admin/delete/%d", but we still are not able to actually delete an album.
That's because there's no handler in our app to delete the album from database.
For the moment both GET and POST requests will do the same, which is return HTML page asking whether to delete the album.

In order to implement the deletion, add `deleteAlbum` to `Db` module:

```
let deleteAlbum (album : Album) (ctx : DbContext) = 
    album.Delete()
    ctx.SubmitUpdates()
```

The snippet takes an `Album` as a parameter - instance of this type comes from database, and we can invoke `Delete()` member on it - SQLProvider keeps track of such changes, and upon `ctx.SubmitUpdates()` executes necessary SQL commands. This is somewhat similar to the "Active Record" concept.

Now, in `App` module we can distinguish between GET and POST requests:

```
let deleteAlbum id =
    let ctx = Db.getContext()
    match Db.getAlbum id ctx with
    | Some album ->
        choose [ 
            GET >>= warbler (fun _ -> 
                html (View.deleteAlbum album.Title))
            POST >>= warbler (fun _ -> 
                Db.deleteAlbum album ctx; 
                Redirection.FOUND Path.Admin.manage)
        ]
    | None ->
        never
```

- `deleteAlbum` WebPart gets passed the `choose` with two possibilities. 
- `GET` and `POST` are WebParts that succeed (return `Some x`) only if the incoming HTTP request is of GET or POST verb respectively.
- after succesfull deletion of album, the `POST` case redirects us to the `Path.Admin.manage` page

> Important: We have to wrap both GET and POST handlers with a `warbler` - otherwise they would be evaluated just after `Some album` match, resulting in invoking `Db.deleteAlbum` even if POST does not apply.

The grid can now contain a column with link to delete the album in question:

```
table [
        yield tr [
            for t in ["Artist";"Title";"Genre";"Price";""] -> th [ text t ]
        ]

        for album in albums -> 
        tr [
            for t in [ truncate 25 album.Artist; truncate 25 album.Title; album.Genre; formatDec album.Price ] ->
                td [ text t ]

            yield td [
                aHref (sprintf Path.Admin.deleteAlbum album.AlbumId) (text "Delete")
            ]
        ]
    ]
```

- there's a new empty column header in the first row
- in the last column of each album row comes a cell with link to delete the album
- note, we had to use the `yield` keyword again

We can delete an album, so why don't we proceed to add album functionality now.
It will require a bit more effort, because we actually need some kind of a form with fields to create a new album.
Fortunately, there's a helper module in Suave library exactly for this purpose.

> Note: `Suave.Form` module at the time of writing is still in `Experimental` package - just as `Suave.Html` which we're already using.

First, let's create a separate module `Form` to keep all of our forms in there (yes there will be more soon).
Add the `Form.fs` file just before `View.fs` - both `View` and `App` module will depend on `Form`.
As with the rest of modules, don't forget to follow our modules naming convention.

Now declare the first `Album` form:

```
module SuaveMusicStore.Form

open Suave.Form

type Album = {
    ArtistId : decimal
    GenreId : decimal
    Title : string
    Price : decimal
    ArtUrl : string
}

let album : Form<Album> = 
    Form ([ TextProp ((fun f -> <@ f.Title @>), [ maxLength 100 ])
            TextProp ((fun f -> <@ f.ArtUrl @>), [ maxLength 100 ])
            DecimalProp ((fun f -> <@ f.Price @>), [ min 0.01M; max 100.0M; step 0.01M ])
            ],
          [])
```

`Album` type contains all fields needed for the form.
For the moment, `Suave.Form` supports following types of fields:

- decimal
- string
- System.Net.Mail.MailAddress
- Suave.Form.Password

> Note: the int type is not supported yet, however we can easily convert from decimal to int and vice versa

Afterwards comes a declaration of the album form (let album : `Form<Album>` =).
It consists of list of "Props" (Properties), of which we can think as of validations:

- First, we declared that the `Title` must be of max length 100
- Second, we declared the same for `ArtUrl`
- Third, we declared that the `Price` must be between 0.01 and 100.0 with a step of 0.01 (this means that for example 1.001 is invalid)

Those properties can be now used as both client and server side.
For client side we will the `album` declaration in our `View` module in order to output HTML5 input validation attributes.
For server side we will use an utitlity WebPart that will parse the form field values from a request.

> Note: the above snippet uses F# Quotations - a feature that you can read more about [here](https://msdn.microsoft.com/en-us/library/dd233212.aspx).
> For the sake of tutorial, you only need to know that they allow Suave to lookup the name of a Field from a property getter.

To see how we can use the form in `View` module, add `open Suave.Form` to the beginning:

```
module SuaveMusicStore.View

open System

open Suave.Html
open Suave.Form
```

Next, add a couple of helper functions:

```
let divClass c = divAttr ["class", c]

...

let fieldset x = tag "fieldset" [] (flatten x)
let legend txt = tag "legend" [] (text txt)
```

And finally this block of code:

```
type Field<'a> = {
    Label : string
    Xml : Form<'a> -> Suave.Html.Xml
}

type Fieldset<'a> = {
    Legend : string
    Fields : Field<'a> list
}

type FormLayout<'a> = {
    Fieldsets : Fieldset<'a> list
    SubmitText : string
    Form : Form<'a>
}

let renderForm (layout : FormLayout<_>) =    
    
    form [
        for set in layout.Fieldsets -> 
            fieldset [
                yield legend set.Legend

                for field in set.Fields do
                    yield divClass "editor-label" [
                        text field.Label
                    ]
                    yield divClass "editor-field" [
                        field.Xml layout.Form
                    ]
            ]

        yield submitInput layout.SubmitText
    ]
```

Above snippet is quite long but, as we'll soon see, we'll be able to reuse it a few times.
The `FormLayout` types defines a layout for a form and consists of:

- `SubmitText` that will be used for the string value of submit button
- `Fieldsets` - a list of `Fieldset` values
- `Form` - instance of the form to render

The `Fieldset` type defines a layout for a fieldset:

- `Legend` is a string value for a set of fields
- `Fields` is a list of `Field` values

The `Field` type has:

- a `Label` string
- `Xml` - function which takes `Form` and returns `Xml` (object model for HTML markup). It might seem cumbersome, but the signature is deliberate in order to make use of partial application

> Note: all of above types are generic, meaning they can accept any type of form, but the form's type must be consistent in the `FormLayout` hierarchy.

`renderForm` is a reusable function that takes an instance of `FormLayout` and returns HTML object model:

- it creates a form element
- the form contains a list of fieldsets, each of which:
    - outputs its legend first
    - iterates over its `Fields` and
        - outputs div element with label element for the field
        - outputs div element with target input element for the field
- the form ends with a submit button

`renderForm` ca be invoked like this:

```
let createAlbum genres artists = [ 
    h2 "Create"
        
    renderForm
        { Form = Form.album
          Fieldsets = 
              [ { Legend = "Album"
                  Fields = 
                      [ { Label = "Genre"
                          Xml = selectInput (fun f -> <@ f.GenreId @>) genres None }
                        { Label = "Artist"
                          Xml = selectInput (fun f -> <@ f.ArtistId @>) artists None }
                        { Label = "Title"
                          Xml = input (fun f -> <@ f.Title @>) [] }
                        { Label = "Price"
                          Xml = input (fun f -> <@ f.Price @>) [] }
                        { Label = "Album Art Url"
                          Xml = input (fun f -> <@ f.ArtUrl @>) ["value", "/placeholder.gif"] } ] } ]
          SubmitText = "Create" }

    div [
        aHref Path.Admin.manage (text "Back to list")
    ]
]
```

We can see that for the `Xml` values we can invoke `selectInput` or `input` functions.
Both of them take as first argument function which directs to field for which the input should be generated.
`input` takes as second argument a list of optional attributes (of type `string * string` - key and value).
`selectInput` takes as second argument list of options (of type `decimal * string` - value and display name).
As third argument, `selectInput` takes an optional selected value - in case of `None`, the first one will be selected initially.

> Note: We are hardcoding the album's `ArtUrl` property with "/placeholder.gif" - we won't implement uploading images, so we'll have to stick with a placeholder image.

Now that we have the `createAlbum` view, we can write the appropriate WebPart handler.
Start by adding `getArtists` to `Db`:

```
type Artist = DbContext.``[dbo].[Artists]Entity``

...

let getArtists (ctx : DbContext) : Artist list = 
    ctx.``[dbo].[Artists]`` |> Seq.toList
```

Then proper entry in `Path` module:

```
    let createAlbum = "/admin/create"
```

and WebPart in `App` module:

```
let createAlbum =
    let ctx = Db.getContext()
    choose [
        GET >>= warbler (fun _ -> 
            let genres = 
                Db.getGenres ctx 
                |> List.map (fun g -> decimal g.GenreId, g.Name)
            let artists = 
                Db.getArtists ctx
                |> List.map (fun g -> decimal g.ArtistId, g.Name)
            html (View.createAlbum genres artists))
    ]

...

    path Path.Admin.createAlbum >>= createAlbum
```

Once again, `warbler` will prevent from eager evaluation of the WebPart - it's vital here.
To our `View.manage` we can add a link to `createAlbum`:

```
let manage (albums : Db.AlbumDetails list) = [ 
    h2 "Index"
    p [
        aHref Path.Admin.createAlbum (text "Create New")
    ]
...
```

This allows us to navigate to "/admin/create", however we're still lacking the actual POST handler.

Before we define the handler, let's add another helper function to `App` module:

```
let bindToForm form handler =
    bindReq (bindForm form) handler BAD_REQUEST
```

It requires a few modules to be open, namely:

- `Suave.Form`
- `Suave.Http.RequestErrors`
- `Suave.Model.Binding`

What `bindToForm` does is:

- it takes as first argument a form of type `Form<'a>`
- it takes as second argument a handler of type `'a -> WebPart`
- if the incoming request contains form fields filled correctly, meaning they can be parsed to corresponding types, and hold all `Prop`s defined in `Form` module, then the `handler` argument is applied with the values of `'a` filled in
- otherwise the 400 HTTP Status Code "Bad Request" is returned with information about what was mailformed.

There are just 2 more things before we're good to go with creating album functionality.

We need `createAlbum` for the `Db` module (the created album is piped to `ignore` function, because we don't need it afterwards):

```
let createAlbum (artistId, genreId, price, title) (ctx : DbContext) =
    ctx.``[dbo].[Albums]``.Create(artistId, genreId, price, title) |> ignore
    ctx.SubmitUpdates()
```

as well as POST handler inside the `createAlbum` WebPart:

```
choose [
        GET >>= ...

        POST >>= bindToForm Form.album (fun form ->
            Db.createAlbum (int form.ArtistId, int form.GenreId, form.Price, form.Title) ctx
            Redirection.FOUND Path.Admin.manage)
    ]
```

We have delete, we have create, so we're left with the update part only.
This one will be fairly easy, as it's gonna be very similar to create (we can reuse the album form we declared in `Form` module).

`editAlbum` in `View`:

```
let editAlbum (album : Db.Album) genres artists = [ 
    h2 "Edit"
        
    renderForm
        { Form = Form.album
          Fieldsets = 
              [ { Legend = "Album"
                  Fields = 
                      [ { Label = "Genre"
                          Xml = selectInput (fun f -> <@ f.GenreId @>) genres (Some (decimal album.GenreId)) }
                        { Label = "Artist"
                          Xml = selectInput (fun f -> <@ f.ArtistId @>) artists (Some (decimal album.ArtistId))}
                        { Label = "Title"
                          Xml = input (fun f -> <@ f.Title @>) ["value", album.Title] }
                        { Label = "Price"
                          Xml = input (fun f -> <@ f.Price @>) ["value", formatDec album.Price] }
                        { Label = "Album Art Url"
                          Xml = input (fun f -> <@ f.ArtUrl @>) ["value", "/placeholder.gif"] } ] } ]
          SubmitText = "Save Changes" }

    div [
        aHref Path.Admin.manage (text "Back to list")
    ]
]
```

Path:

```
    let editAlbum : IntPath = "/admin/edit/%d"    
```

Link in `manage` in `View`:

```
for album in albums -> 
        tr [
            ...

            yield td [
                aHref (sprintf Path.Admin.editAlbum album.AlbumId) (text "Edit")
                text " | "
                aHref (sprintf Path.Admin.deleteAlbum album.AlbumId) (text "Delete")
            ]
        ]
```

`updateAlbum` in `Db` module:

```
let updateAlbum (album : Album) (artistId, genreId, price, title) (ctx : DbContext) =
    album.ArtistId <- artistId
    album.GenreId <- genreId
    album.Price <- price
    album.Title <- title
    ctx.SubmitUpdates()
```

`editAlbum` WebPart in `App` module:

```
let editAlbum id =
    let ctx = Db.getContext()
    match Db.getAlbum id ctx with
    | Some album ->
        choose [
            GET >>= warbler (fun _ ->
                let genres = 
                    Db.getGenres ctx 
                    |> List.map (fun g -> decimal g.GenreId, g.Name)
                let artists = 
                    Db.getArtists ctx
                    |> List.map (fun g -> decimal g.ArtistId, g.Name)
                html (View.editAlbum album genres artists))
            POST >>= bindToForm Form.album (fun form ->
                Db.updateAlbum album (int form.ArtistId, int form.GenreId, form.Price, form.Title) ctx
                Redirection.FOUND Path.Admin.manage)
        ]
    | None -> 
        never
```

and finally `pathScan` in main `choose` WebPart:

```
    pathScan Path.Admin.editAlbum editAlbum
```

Comments to above snippets:

- `editAlbum` View looks very much the same as the `createAlbum`. The only significant difference is that it has all the filed values pre-filled. 
- in `Db.updateAlbum` we can see examples of property setters. This is the way SQLProvider mutates our `Album` value, while keeping track on what has changed before `SubmitUpdates()`
- `warbler` is needed in `editAlbum` GET handler to prevent eager evaluation
- but it's not necessary for POST, because POST needs to parse the incoming request, thus the evaluation is postponed upon the successfull parsing.
- after the album is updated, a redirection to `manage` is applied

> Note: SQLProvider allows to change `Album` properties after the object has been instantiated - that's generally against the immutability concept that's propageted in the functional programming paradigm. We need to remember however, that F# is not pure functional programming language, but rather "functional first". This means that while it encourages to write in functional style, it still allows to use Object Oriented constructs. This often turns out to be usefull, for example when we need to improve performance.

As the icing on the cake, let's also add link to details for each of the albums in `View.manage`:

```
aHref (sprintf Path.Admin.editAlbum album.AlbumId) (text "Edit")
text " | "
aHref (sprintf Path.Store.details album.AlbumId) (text "Details")
text " | "
aHref (sprintf Path.Admin.deleteAlbum album.AlbumId) (text "Delete")
```

Pheeew, this section was long, but also very productive. Looks like we can already do some serious interaction with the application!
Results can be seen here: [Tag - crud_and_forms](https://github.com/theimowski/SuaveMusicStore/tree/crud_and_forms)


Auth and Session
----------------

In the previous section we succeeded in setting up Create, Update and Delete functionality for albums in the Music Store.
All of these actions are likely to be performed by some kind of shop manager, or administrator.
In fact, `Path` module defines that all the operations are available under "/admin" route.
It would be nice if we could authorize only chosen users to mess with albums in our Store.
That's exactly what we'll do right now.

As a warmup, let's add navigation menu at the very top of the view.
We'll call it `partNav` and keep in separate function:

```
let partNav = 
    ulAttr ["id", "navlist"] [ 
        li (aHref Path.home (text "Home"))
        li (aHref Path.Store.overview (text "Store"))
        li (aHref Path.Admin.manage (text "Admin"))
    ]
```

`partNav` consists of 3 main tabs: "Home", "Store" and "Admin". `ulAttr` can be defined like following:

```
let ulAttr attr xml = tag "ul" attr (flatten xml)
```

We want to specify the `id` attribute here so that our CSS can make the menu nice and shiny.
Add the `partNav` to main index view, in the "header" `div`:

```
divId "header" [
    h1 (aHref Path.home (text "F# Suave Music Store"))
    partNav
]
```

This gives a possiblity to navigate through main features of our Music Store.
It would be good if a visitor to our site could authenticate himself.
To help him with that, we'll put a user partial view next to the navigation menu.
Just as in every other e-commerce website, if a user is logged in, he'll be shown his name and a "Log off" link.
Otherwise, we'll just display a "Log on" link.
First, open up the `Path` module and define routes for `logon` and `logoff` in `Account` submodule:

```
module Account =
    let logon = "/account/logon"
    let logoff = "/account/logoff"
```

Next, define `partUser` in the `View` module:

```
let partUser (user : string option) = 
    divId "part-user" [
        match user with
        | Some user -> 
            yield text (sprintf "Logged on as %s, " user)
            yield aHref Path.Account.logoff (text "Log off")
        | None ->
            yield aHref Path.Account.logon (text "Log on")
    ]
```

> Note: Because we're inside pattern matching, the `yield` keyword is mandatory here.

and include it in "header" `div` as well"

```
divId "header" [
    h1 (aHref Path.home (text "F# Suave Music Store"))
    partNav
    partUser (None)
]
```

The only argument to `partUser` is an optional username - if it exists, then the user is authenticated.
For now, we assume no user is logged on, thus we hardcode the `None` in call to `partUser`.

There's no handler for the `logon` route yet, so we need to create one.
Logon view will be rather straightforward - just a simple form with username and password.

```
type Logon = {
    Username : string
    Password : Password
}

let logon : Form<Logon> = Form ([],[])
```

Above snippet shows how the `logon` form can be defined in our `Form` module.
`Password` is a type from Suave library and helps to determine the input type for HTML markup (we don't want anyone to see our secret pass as we type it).

```
let logon = [
    h2 "Log On"
    p [
        text "Please enter your user name and password."
    ]

    renderForm
        { Form = Form.logon
          Fieldsets = 
              [ { Legend = "Account Information"
                  Fields = 
                      [ { Label = "User Name"
                          Xml = input (fun f -> <@ f.Username @>) [] }
                        { Label = "Password"
                          Xml = input (fun f -> <@ f.Password @>) [] } ] } ]
          SubmitText = "Log On" }
]
```

As I promised, nothing fancy here.
We've already seen how the `renderForm` works, so the above snippet is just another plain HTML form with some additional instructions at the top.

The GET handler for `logon` is also very simple:

```
let logon =
    View.logon
    |> html
```

```
path Path.Account.logon >>= logon
```

Things get more complicated with regards to the POST handler.
As a gentle introduction, we'll add logic to verify passed credentials - by querying the database (`Db` module):

```
let validateUser (username, password) (ctx : DbContext) : User option =
    query {
        for user in ctx.``[dbo].[Users]`` do
            where (user.UserName = username && user.Password = password)
            select user
    } |> firstOrNone
```

The snippet makes use of `User` type alias:

```
type User = DbContext.``[dbo].[Users]Entity``
```

Now, in the `App` module add two more `open` statements:

```
open System
...
open Suave.State.CookieStateStore
```

and add a couple of helper functions:

```
let passHash (pass: string) =
    use sha = Security.Cryptography.SHA256.Create()
    Text.Encoding.UTF8.GetBytes(pass)
    |> sha.ComputeHash
    |> Array.map (fun b -> b.ToString("x2"))
    |> String.concat ""

let session = statefulForSession

let sessionStore setF = context (fun x ->
    match HttpContext.state x with
    | Some state -> setF state
    | None -> never)

let returnPathOrHome = 
    request (fun x -> 
        let path = 
            match (x.queryParam "returnPath") with
            | Choice1Of2 path -> path
            | _ -> Path.home
        Redirection.FOUND path)
```

Comments:

- `passHash` is of type `string -> string` - from a given string it creates a SHA256 hash and formats it to hexadecimal. That's how users' passwords are stored in our database.
- `session` for now is just an alias to `statefulForSession` from Suave, which initializes a user state for a browsing session. We will however add extra argument to the `session` function in a few minutes, that's why we might want to have it extracted already.
- `sessionStore` is a higher-order function, taking `setF` as a parameter - which in turn can be used to read from or write to the session store.
- `returnPathOrHome` tries to extract "returnPath" query parameter from the url, and redirects to that path if it exists. If no "returnPath" is found, we get back redirected to the home page.

Now turn for the `logon` POST handler monster:

```
let logon =
    choose [
        GET >>= (View.logon |> html)
        POST >>= bindToForm Form.logon (fun form ->
            let ctx = Db.getContext()
            let (Password password) = form.Password
            match Db.validateUser(form.Username, passHash password) ctx with
            | Some user ->
                    Auth.authenticated Cookie.CookieLife.Session false 
                    >>= session
                    >>= sessionStore (fun store ->
                        store.set "username" user.UserName
                        >>= store.set "role" user.Role)
                    >>= returnPathOrHome
            | _ ->
                never
        )
    ]
```

Not that bad, isn't it?
What we do first here is we bind to `Form.logon`.
This means that in case the request is malformed, `bindToForm` takes care of returning 400 Bad Request status code.
If someone however decides to be polite and fill in the logon form correctly, then we reach the database and ask whether such user with such password exists.
Note, that we have to pattern match the password string in form result (`let (Password password) = form.Password`).
If `Db.validateUser` returns `Some user` then we compose 4 WebParts together in order to correctly set up the user state and redirect user to his destination.
First, `Auth.authenticated` sets proper cookies which live till the session ends. The second (`false`) argument specifies the cookie isn't "HttpsOnly".
Then we bind the result to `session`, which as described earlier, sets up the user session state.
Next, we write two values to the session store: "username" and "role".
Finally, we bind to `returnPathOrHome` - we'll shortly see how this one can be useful.

You might have noticed, that the above code will results in "Not found" page in case `Db.validateUser` returns None.
That's because we temporarily assigned `never` to the latter match.
Ideally, we'd like to see some kind of a validation message next to the form.
To achieve that, let's add `msg` parameter to `View.logon`:

```
let logon msg = [
    h2 "Log On"
    p [
        text "Please enter your user name and password."
    ]

    divId "logon-message" [
        text msg
    ]
...
```

Now we can invoke it in two ways:

```
GET >>= (View.logon "" |> html)

...

View.logon "Username or password is invalid." |> html
```

The first one being GET `logon` handler, and the other one being returned if provided credentials are incorrect.

Up to this point, we should be able to authenticate with "admin" -> "admin" credentials to our application.
This is however not very useful, as there are no handlers that would demand user to be authenticated yet.

To change that, let's define custom types to represent user state:

```
type UserLoggedOnSession = {
    Username : string
    Role : string
}

type Session = 
    | NoSession
    | UserLoggedOn of UserLoggedOnSession
```

`Session` type is so-called "Discriminated Union" in F#.
It basically means that an instance of `Session` type is either `NoSession` or `UserLoggedOn`, and no other than that.
`of UserLoggedOnSession` is an indicator that there is some data of type `UserLoggedOnSession` related.
Read [here](http://fsharpforfunandprofit.com/posts/discriminated-unions/) for more info on Discriminated Unions. 

On the other hand, `UserLoggedOnSession` is a "Record type".
We can think of Record as a Plain-Old-Class Object, or DTO, or whatever you like.
It has however a number of language built-in features that make it really awesome, including:

- imutability by default
- structural equality by default
- pattern matching
- copy-and-update expression

Again, if you want to learn more about Records, make sure you visit [this](http://fsharpforfunandprofit.com/posts/records/) post.

With these two types, we'll be able to distinguish from when a user is logged on to our application and when he is not.

As stated before, we'll now add a parameter to the `session` function:

```
let session f = 
    statefulForSession
    >>= context (fun x -> 
        match x |> HttpContext.state with
        | None -> f NoSession
        | Some state ->
            match state.get "username", state.get "role" with
            | Some username, Some role -> f (UserLoggedOn {Username = username; Role = role})
            | _ -> f NoSession)
```

Type of `f` parameter is `Session -> WebPart`.
You guessed it, it means we will be able to do different things including returning different responses, depending on the user session state.
In order to confirm that a user is logged on, session state store must contain both "username" and "role" values.

> Note: We have used a pattern matching on a tuple in the above snippet - we could pass two values separated with comma to the `match` construct, and then pattern match on both values: `Some username, Some role` means both values are present. The latter (`_`) covers all other instances.

The only usage of `session` for now is in the `logon` POST handler - let's adjust it to new version:

```
...
Auth.authenticated Cookie.CookieLife.Session false 
>>= session (fun _ -> succeed)
>>= sessionStore (fun store ->
...
```

Yes I know, I promised we'll pass something funky to the `session` function, but bear with with me - we will later.
For the moment usage of `session` in `logon` doesn't require any custom action, but we still need to invoke it to "initalize" the user state.

There are a few more helper functions needed before we can set up proper authorization for "/admin" handlers.
Add following to `App` module:

```
open Suave.Cookie

...

let reset =
    unsetPair Auth.SessionAuthCookie
    >>= unsetPair StateCookie
    >>= Redirection.FOUND Path.home

let redirectWithReturnPath redirection =
    request (fun x ->
        let path = x.url.AbsolutePath
        Redirection.FOUND (redirection |> Path.withParam ("returnPath", path)))


...

let loggedOn f_success =
    Auth.authenticate
        Cookie.CookieLife.Session
        false
        (fun () -> Choice2Of2(redirectWithReturnPath Path.Account.logon))
        (fun _ -> Choice2Of2 reset)
        f_success

let admin f_success =
    loggedOn (session (function
        | UserLoggedOn { Role = "admin" } -> f_success
        | UserLoggedOn _ -> FORBIDDEN "Only for admin"
        | _ -> UNAUTHORIZED "Not logged in"
    ))
```

Remarks:

- `reset` is a WebPart to clean up auth and state cookie values, and redirect to home page afterwards. We'll use it for logging user off.
- `redirectWithReturnPath` aims to point user to some url, with the "returnPath" query parameter baked into the url. We'll use it for redirecting user to logon page if specific action requires authentication.
- `loggedOn` takes `f_success` WebPart as argument, which will be applied if user is authenticated. Here we use the library function `Auth.authenticate`, to which `f_success` comes as last parameter. The rest of parameters, starting with first are are respectively:
    - `CookieLife` - `Session` in our case
    - `httpsOnly` - we pass false as we won't cover HTTPS bindings in the tutorial (however Suave does support it).
    - 3rd parameter is a function applied if auth cookie is missing - that's where we want to redirect user to the logon page with a "returnPath"
    - 4th parameter is a function applied if there occured a decryption error. In real world this could mean a malformed request, however at current stage we'll stick to reseting the state. This way we can re-run the server multiple times during development, without worrying about the browser to pass a cookie value encrypted with stale server key (Suave regenerates a new server key each time it is run).
- `admin` also takes `f_success` WebPart as argument. Here, we invoke `loggedOn` with an inline function using `session`. The interesting part is inside the `session` function:
    - syntax `function | ... -> ` is just a shorter version of `match x with | ... -> ` but the `x` param is implicit here, and `x` is of type `Session`
    - first pattern shows the real power of the pattern matching technique - `f_success` will be applied only if user is logged on, and his Role is "admin" (we'll distinguish between "admin" and "user" roles)
    - second pattern holds if user is logged on but with different role, thus we return 403 Forbidden status code
    - the last "otherwise" (`_`) pattern should never hold, because we're already inside `loggedOn`. We use it anyway, just as a safety net.

That was quite long, but worth it. Finally we're able to guard the "/admin" actions:

```
path Path.Admin.manage >>= admin manage
path Path.Admin.createAlbum >>= admin createAlbum
pathScan Path.Admin.editAlbum (fun id -> admin (editAlbum id))
pathScan Path.Admin.deleteAlbum (fun id -> admin (deleteAlbum id))
```

Go and have a look what happens when you try to navigate to "/admin/manage" route.

We still need to update the `partUser`, when a user is logged on (remember we hardcoded `None` for username).
To do this, we can pass `partUser` as parameter to `View.index`:

```
let index partUser container = 

...

            divId "header" [
                h1 (aHref Path.home (text "F# Suave Music Store"))
                partNav
                partUser
            ]
```

and determine whether a user is logged on in the `html` WebPart in `App` module:

```
let html container =
    let result user =
        OK (View.index (View.partUser user) container)
        >>= Writers.setMimeType "text/html; charset=utf-8"

    session (function
    | UserLoggedOn { Username = username } -> result (Some username)
    | _ -> result None)
```

We declared a sub function `result` which takes the `user` parameter.
`session` can be used to determine user state.
Effectively, we invoke the `result` function always but with `user` argument based on the user state.

The last thing we need to support is `logoff`. In the main `choose` WebPart add:

```
path Path.Account.logoff >>= reset
```

`logoff` doesn't require separate WebPart, `reset` can be reused instead.

That concludes our journey to Auth and Session features in `Suave` library. 
We'll revisit the concepts in next section, but much of the implementation can be reused.
Code up to this point can be browsed here: [Tag - auth_and_session](https://github.com/theimowski/SuaveMusicStore/tree/auth_and_session)


More cookies in shopping cart
-----------------------------

What's a shop without cart feature?
We would like to let the user add albums to a cart while shopping at our Store.
To fill in the gap, let's start by declaring new routes in `Path` module:

```
module Cart =
    let overview = "/cart"
    let addAlbum : IntPath = "/cart/add/%d"
    let removeAlbum : IntPath = "/cart/remove/%d"
```

Before we move to the `View`, add new type annotation for yet another database view `CartDetails`.
`CartDetails` is a view that joins `Cart` with its corresponding `Album` in order to contain album's title and its price.

```
type CartDetails = DbContext.``[dbo].[CartDetails]Entity``
```

"As a user I want to see that my cart is empty when my cart is empty so that I can make my cart not empty"
With such a serious business requirement, we'd better distinguish case when user has anything in his cart from case when the cart is empty.
To do that, add separate `emptyCart` in `View` module: 

```
let emptyCart = [
    h2 "Your cart is empty"
    text "Find some great music in our "
    aHref Path.home (text "store")
    text "!"
]
```

In the latter case, we're going to display a table with all albums in the cart:

```
let aHrefAttr href attr = tag "a" (("href", href) :: attr)

...

let nonEmptyCart (carts : Db.CartDetails list) = [
    h2 "Review your cart:"
    table [
        yield tr [
            for h in ["Album Name"; "Price (each)"; "Quantity"; ""] ->
            th [text h]
        ]
        for cart in carts ->
            tr [
                td [
                    aHref (sprintf Path.Store.details cart.AlbumId) (text cart.AlbumTitle)
                ]
                td [
                    text (formatDec cart.Price)
                ]
                td [
                    text (cart.Count.ToString())
                ]
                td [
                    aHrefAttr "#" ["class", "removeFromCart"; "data-id", cart.AlbumId.ToString()] (text "Remove from cart") 
                ]
            ]
        yield tr [
            for d in ["Total"; ""; ""; carts |> List.sumBy (fun c -> c.Price * (decimal c.Count)) |> formatDec] ->
            td [text d]
        ]
    ]
]
```

With these two separate views we can declare a more general one, for when we're not sure whether the cart is empty or not:

```
let cart = function
    | [] -> emptyCart
    | list -> nonEmptyCart list
```

`cart` makes use of the short `function` pattern matching syntax. `[]` case holds only for empty lists, so the second case is valid for non-empty lists.

A few remarks regarding the `nonEmptyCart` function:

- first comes the column headings row ("Name, Price, Quantity")
- then for each `CartDetail` from the list there is a row containg:
    - link to album details with album title caption
    - single album price
    - count of this very album in cart
    - link to remove the item from cart (we'll soon apply AJAX updates to this one)
- finally there's a summary row that displays the total price for albums in the cart

We can't really test the view for now, as we haven't yet implemented fetching cart items from database. 
We can however see how the `emptyCart` view looks like if we add proper handler in `App` module:

```
let cart = View.cart [] |> html

...

path Path.Cart.overview >>= cart
```

A navigation menu item can also appear handy (`View.partNav`):

```
li (aHref Path.Cart.overview (text "Cart"))
```

It's time to revisit the `Session` type from `App` module.
Business requirement is that in order to checkout, a user must be logged on but he doesn't have to when adding albums to the cart.
That seems reasonable, as we don't want to stop him from his shopping spree with boring logon forms.
For this purpose we'll add another case `CartIdOnly` to the `Session` type.
This state will be valid for users who are not yet logged on to the store, but have already some albums in their cart:

```
type Session = 
    | NoSession
    | CartIdOnly of string
    | UserLoggedOn of UserLoggedOnSession
```

`CartIdOnly` contains string with a GUID generated upon adding first item to the cart.

Switching back to `Db` module, let's create a type alias for `Carts` table:

```
type Cart = DbContext.``[dbo].[Carts]Entity``
```

`Cart` has following properties:

- CartId - a GUID if user is not logged on, otherwise username
- AlbumId
- Count

To implement `App` handler, we need the following `Db` module functions:

```
let getCart cartId albumId (ctx : DbContext) : Cart option =
    query {
        for cart in ctx.``[dbo].[Carts]`` do
            where (cart.CartId = cartId && cart.AlbumId = albumId)
            select cart
    } |> firstOrNone
```

```
let addToCart cartId albumId (ctx : DbContext)  =
    match getCart cartId albumId ctx with
    | Some cart ->
        cart.Count <- cart.Count + 1
    | None ->
        ctx.``[dbo].[Carts]``.Create(albumId, cartId, 1, DateTime.UtcNow) |> ignore
    ctx.SubmitUpdates()
```

`addToCart` takes `cartId` and `albumId`. 
If there's already such cart entry in the database, we do increment the `Count` column, otherwise we create a new row.
To check if a cart entry exists in database, we use `getCart` - it does a standard lookup on the cartId and albumId.

Now open up the `View` module and find the `details` function to append a new button "Add to cart", at the very bottom of "album-details" div:

```
yield pAttr ["class", "button"] [
    aHref (sprintf Path.Cart.addAlbum album.AlbumId) (text "Add to cart")
]
```

With above in place, we're ready to define the handler in `App` module:

```
let addToCart albumId =
    let ctx = Db.getContext()
    session (function
            | NoSession -> 
                let cartId = Guid.NewGuid().ToString("N")
                Db.addToCart cartId albumId ctx
                sessionStore (fun store ->
                    store.set "cartid" cartId)
            | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
                Db.addToCart cartId albumId ctx
                succeed)
        >>= Redirection.FOUND Path.Cart.overview
```

```
pathScan Path.Cart.addAlbum addToCart
```

`addToCart` invokes our `session` function with two flavors:

- if `NoSession` then create a new GUID, save the record in database and update the session store with "cartid" key
- if `UserLoggedOn` or `CartIdOnly` then only add the album to the user's cart. Note that we could bind the `cartId` string value here to both cases - as described earlier `cartId` equals GUID if user is not logged on, otherwise it's the user's name.

We still have to recognize the `CartIdOnly` case - coming back to the `session` function, the pattern matching handling `CartIdOnly` looks like this:

```
match state.get "cartid", state.get "username", state.get "role" with
| Some cartId, None, None -> f (CartIdOnly cartId)
| _, Some username, Some role -> f (UserLoggedOn {Username = username; Role = role})
| _ -> f NoSession
```

This means that if the session store contains `cartid` key, but no `username` or `role` then we invoke the `f` parameter with `CartIdOnly`.

To fetch the actual `CartDetails` for a given `cartId`, let's define appropriate function in `Db` module:

```
let getCartsDetails cartId (ctx : DbContext) : CartDetails list =
    query {
        for cart in ctx.``[dbo].[CartDetails]`` do
            where (cart.CartId = cartId)
            select cart
    } |> Seq.toList
```

This allows us to implement `cart` handler correctly:

```
let cart = 
    session (function
    | NoSession -> View.emptyCart |> html
    | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
        let ctx = Db.getContext()
        Db.getCartsDetails cartId ctx |> View.cart |> html)
```

Again, we use two different patterns for the same behavior here - `CartIdOnly` and `UserLoggedOn` states will query the database for cart details.

Remember our navigation menu with `Cart` tab? 
Why don't we add a number of total albums in our cart there?
To do that, let's parametrize the `partNav` view:

```
let partNav cartItems = 
    ulAttr ["id", "navlist"] [ 
        li (aHref Path.home (text "Home"))
        li (aHref Path.Store.overview (text "Store"))
        li (aHref Path.Cart.overview (text (sprintf "Cart (%d)" cartItems)))
        li (aHref Path.Admin.manage (text "Admin"))
    ]
```

as well as add the `partNav` parameter to the main `index` view:

```
let index partNav partUser container = 
```

In order to create the navigation menu, we now need to pass `cartItems` parameter.
It has to be resolved in the `html` function from `App` module, which can now look like following:

```
let html container =
    let ctx = Db.getContext()
    let result cartItems user =
        OK (View.index (View.partNav cartItems) (View.partUser user) container)
        >>= Writers.setMimeType "text/html; charset=utf-8"

    session (function
    | UserLoggedOn { Username = username } -> 
        let items = Db.getCartsDetails username ctx |> List.sumBy (fun c -> c.Count)
        result items (Some username)
    | CartIdOnly cartId ->
        let items = Db.getCartsDetails cartId ctx |> List.sumBy (fun c -> c.Count)
        result items None
    | NoSession ->
        result 0 None)
```

The change is that the nested `result` function in `html` now takes two arguments: `cartItems` and `user`. 
Additionally, we handle all cases inside the `session` invocation.
Note how the `result` function is passed different set of parameters in different cases.

Now that we can add albums to the cart, and see the total number both in `cart` overview and navigation menu, it would be nice to be able to remove albums from the cart as well.
This is a great occasion to employ AJAX in our application.
We'll write a simple script in JS that makes use of jQuery to remove selected album from the cart and update the cart view.

Download jQuery from [here](https://jquery.com/download/) (I used the compressed / minified version) and add it to the project.
Don't forget to set the "Copy to Output Directory" property.

Now add new JS file to the project `script.js`, and fill in its contents:

```
$('.removeFromCart').click(function () {
    var albumId = $(this).attr("data-id");
    var albumTitle = $(this).closest('tr').find('td:first-child > a').html();
    var $cartNav = $('#navlist').find('a[href="/cart"]');
    var count = parseInt($cartNav.html().match(/\d+/));

    $.post("/cart/remove/" + albumId, function (data) {
        $('#container').html(data);
        $('#update-message').html(albumTitle + ' has been removed from your shopping cart.');
        $cartNav.html('Cart (' + (count - 1) + ')');
    });
});
```

We won't go into much details about the code itself, however it's important to know that the script:

- subscribes to click event on each `removeFromCart` element
- parses information such as:
    - album id
    - album title
    - count of this album in cart
- sends a POST request to "/cart/remove" endpoint 
- upon successfull POST response it updates:
    - html of the container element
    - message, to indicate which album has been removed
    - navigation menu to decrement count of albums

The `update-message` div should be added to the `nonEmptyCart` view, before the table:

```
    divId "update-message" [text " "]
```

We explictly have to pass in non-empty text, because we cannot have an empty div element in HTML markup.
With jQuery and our `script.cs` files, we can now attach them to the end of `nonEmptyCart` view, just after the table:

```
scriptAttr [ "type", "text/javascript"; " src", "/jquery-1.11.3.min.js" ] [ text "" ]
scriptAttr [ "type", "text/javascript"; " src", "/script.js" ] [ text "" ]
```

We also need to allow browsing for files with "js" extension in our handler:

```
pathRegex "(.*)\.(css|png|gif|js)" >>= Files.browseHome
```

The script tries to reach route that is not mapped to any handler yet.
Let's change that by first adding `removeFromCart` to `Db` module:

```
let removeFromCart (cart : Cart) albumId (ctx : DbContext) = 
    cart.Count <- cart.Count - 1
    if cart.Count = 0 then cart.Delete()
    ctx.SubmitUpdates()
```

then adding the `removeFromCart` handler in `App` module:

```
let removeFromCart albumId =
    session (function
    | NoSession -> never
    | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
        let ctx = Db.getContext()
        match Db.getCart cartId albumId ctx with
        | Some cart -> 
            Db.removeFromCart cart albumId ctx
            Db.getCartsDetails cartId ctx |> View.cart |> Html.flatten |> Html.xmlToString |> OK
        | None -> 
            never)
```

and finally mapping the route to the handler in main `choose` WebPart:

```
pathScan Path.Cart.removeAlbum removeFromCart
```

A few comments to the `removeFromCart` WebPart:

- this handler should not be invoked with `NoSession`, `never` prevents from unwanted requests
- the same happens, when someone tries to invoke `removeFromCart` for `albumId` not present in his cart (Db.getCart returns `None`)
- if proper cart has been found, `Db.removeFromCart` is invoked, and
- an inline portion of HTML is returned. Note that we don't go through our `html` helper function here like before, but instead return just the a part that `script.js` will inject into the "container" div on our page with AJAX.

This almost concludes the cart feature.
One more thing before we finish this section: 
What should happen if user first adds some albums to his cart and later decides to log on?
If user logs on, we have his user name - so we can "upgrade" all his carts from GUID to the acutal user's name.
Add necessary functions to the `Db` module:

```
let getCarts cartId (ctx : DbContext) : Cart list =
    query {
        for cart in ctx.``[dbo].[Carts]`` do
            where (cart.CartId = cartId)
            select cart
    } |> Seq.toList
```

```
let upgradeCarts (cartId : string, username :string) (ctx : DbContext) =
    for cart in getCarts cartId ctx do
        match getCart username cart.AlbumId ctx with
        | Some existing ->
            existing.Count <- existing.Count +  cart.Count
            cart.Delete()
        | None ->
            cart.CartId <- username
    ctx.SubmitUpdates()
```

and update the `logon` handler in `App` module:

```
Auth.authenticated Cookie.CookieLife.Session false 
>>= session (function
    | CartIdOnly cartId ->
        let ctx = Db.getContext()
        Db.upgradeCarts (cartId, user.UserName) ctx
        sessionStore (fun store -> store.set "cartid" "")
    | _ -> succeed)
>>= sessionStore (fun store ->
    store.set "username" user.UserName
    >>= store.set "role" user.Role)
>>= returnPathOrHome
```

Remarks:

- `Db.getCarts` returns just a plain list of all carts for a given `cartId`
- `Db.upgradeCarts` takes `cartId` and `username` in order to iterate through all carts (returned by `Db.getCarts`), and for each of them:
    - if there's already a cart in the database for this `username` and `albumId`, it sums up the counts and deletes the GUID cart. This can happen if logged on user adds an album to cart, then logs off and adds the same album to cart, and then back again logs on - album's count should be 2
    - if there's no cart for such `username` and `albumId`, simply "upgrade" the cart by changing the `CartId` property to `username`
- `logon` handler now recognizes `CartIdOnly` case, for which it has to invoke `Db.upgradeCarts`. In addition it whipes out the `cartId` key from session store, as from now on `username` will be used as a cart id.

Whoa, we now have the cart functionality in our Music Store! 
See the following link to browse the code: [Tag - cart](https://github.com/theimowski/SuaveMusicStore/tree/cart)



Registration and checkout
-------------------------

There's already a bunch of nice features in our app, however we still lack of registration.
Currently it's only admin account without possibility to create new users.
It would be a pity if no one can register, because only registered users can buy albums in our store!

Register feature will be based on a standard form, so let's add one to the `Form` module:

```
open System.Net.Mail
```

```
type Register = {
    Username : string
    Email : MailAddress
    Password : Password
    ConfirmPassword : Password
}

let pattern = @"(\w){6,20}"

let passwordsMatch = 
    (fun f -> f.Password = f.ConfirmPassword), "Passwords must match"

let register : Form<Register> = 
    Form ([ TextProp ((fun f -> <@ f.Username @>), [ maxLength 30 ] )
            PasswordProp ((fun f -> <@ f.Password @>), [ passwordRegex pattern ] )
            PasswordProp ((fun f -> <@ f.ConfirmPassword @>), [ passwordRegex pattern ] )
            ],[ passwordsMatch ])
```

In the above snippet:

- we open the `System.Net.Mail` namespace to use the `MailAddress` type
- the form consists of 4 fields:
    - `Username` of `string`
    - `Email` of type `MailAddress` (ensures proper validation of the field)
    - `Password` of type `Password` (ensures proper HTML input type)
    - `ConfirmPassword` of the same type
- `pattern` is a regular expression pattern for password
- `passwordsMatch` is a server-side only validation function
- `register` is the definition of our form, with a few constraints:
    - `Username` must be at most 30 characters long
    - `Password` must match the regular expression
    - `ConfirmPassword` must match the regular expression

Server-side only validation, like `passwordMatch` are of type `('FormType -> bool) * string`.
So this is just a tuple of a predicate function and a string error.
We can create as many validations as we like, and pass them to the `Form` definition.
These can be used for validations that lookup more than one field, or require some complex logic.
We won't create client-side validation to check if the passwords match in the tutorial, but it could be achieved with some custom JavaScript code.

With the form definition in place, let's proceed to `View`:

```
let register msg = [
    h2 "Create a New Account"
    p [
        text "Use the form below to create a new account."
    ]
    
    divId "register-message" [
        text msg
    ]

    renderForm
        { Form = Form.register
          Fieldsets = 
              [ { Legend = "Create a New Account"
                  Fields = 
                      [ { Label = "User name (max 30 characters)"
                          Xml = input (fun f -> <@ f.Username @>) [] }
                        { Label = "Email address"
                          Xml = input (fun f -> <@ f.Email @>) [] }
                        { Label = "Password (between 6 and 20 characters)"
                          Xml = input (fun f -> <@ f.Password @>) [] }
                        { Label = "Confirm password"
                          Xml = input (fun f -> <@ f.ConfirmPassword @>) [] } ] } ]
          SubmitText = "Register" }
]
```

As you can see, we're using the `msg` parameter here similar to how it was done in `View.logon` to include possible error messages.
The rest of the snippet is rather self-explanatory.

We're now left with proper `Path.Account` entry:

```
let register = "/account/register"
```

GET handler for registration in `App`:

```
let register =
    choose [
        GET >>= (View.register "" |> html)
    ]
```

```
path Path.Account.register >>= register
```

and a direct link from the `View.logon` :

```
let logon msg = [
    h2 "Log On"
    p [
        text "Please enter your user name and password."
        aHref Path.Account.register (text " Register")
        text " if you don't have an account yet."
    ]

    ...
```

This allows us to navigate to the registration form.
Moving on to implementing the actual POST handler, let's first create necessary functions in `Db` module:

```
let getUser username (ctx : DbContext) : User option = 
    query {
        for user in ctx.``[dbo].[Users]`` do
        where (user.UserName = username)
        select user
    } |> firstOrNone
```

```
let newUser (username, password, email) (ctx : DbContext) =
    let user = ctx.``[dbo].[Users]``.Create(email, password, "user", username)
    ctx.SubmitUpdates()
    user
```

`getUser` will be crucial to check if a user with given `username` already exists in the database - we don't want two users with the same `username`.
`newUser` is a simple function that creates and returns the new user.
Note that we hardcode "user" for each new user's role. 
This way, they can be distinguished from admin's role.

After a successful registration, we'd like to authenticate user at once - in other words apply the same logic which happens after successful logon.
In a real application, you'd probably use a confirmation mail mechanism, but for the sake of simplicity we'll skip that.
In order to reuse the logic from logon POST handler, extract a separate function:

```
let authenticateUser (user : Db.User) =
    Auth.authenticated Cookie.CookieLife.Session false 
    >>= session (function
        | CartIdOnly cartId ->
            let ctx = Db.getContext()
            Db.upgradeCarts (cartId, user.UserName) ctx
            sessionStore (fun store -> store.set "cartid" "")
        | _ -> succeed)
    >>= sessionStore (fun store ->
        store.set "username" user.UserName
        >>= store.set "role" user.Role)
    >>= returnPathOrHome
```

after extraction, `logon` POST handler looks like this:

```
match Db.validateUser(form.Username, passHash password) ctx with
| Some user ->
    authenticateUser user
| _ ->
    View.logon "Username or password is invalid." |> html
```

Finally, the full register handler can be implemented following:

```
let register =
    choose [
        GET >>= (View.register "" |> html)
        POST >>= bindToForm Form.register (fun form ->
            let ctx = Db.getContext()
            match Db.getUser form.Username ctx with
            | Some existing -> 
                View.register "Sorry this username is already taken. Try another one." |> html
            | None ->
                let (Password password) = form.Password
                let email = form.Email.Address
                let user = Db.newUser (form.Username, passHash password, email) ctx
                authenticateUser user
        )
    ]
```

Comments for POST part:

- bind to `Form.register` to validate the request
- check if a user with given `username` already exists
- if that's the case then show the `View.register` form again with a proper error message
- otherwise read the form fields' values, create new user and invoke the `authenticateUser` function

This concludes register feature - we're now set for new customers to do the shopping.
Wait a second, they can put albums to the cart, but how do they checkout?
Ah yes, we haven't implemented that yet.

So let's take a deep breath and roll to the last significant feature in our application!

