Suave Music Store
=================

Introduction
------------

This is a tutorial on how to create an application with [F#](http://fsharp.org) and [Suave.IO](http://suave.io) framework. 
It's inspired by the Music Store tutorial created by the ASP.NET team [available here](http://www.asp.net/mvc/overview/older-versions/mvc-music-store/mvc-music-store-part-1).
Check out this link if you want to find out what the application is going to offer.

Target audience for this tutorial are mainly C# developers familiar with ASP.NET MVC, who want to learn how to write a real application in F#.
No prior experience with F# is required - the tutorial will cover basic concepts of the language.
You can still benefit from the tutorial if you don't have C# / .NET background, however you may find some aspects not clear - From time to time there will be a comparison with how the same functionality could be written in ASP.NET MVC & C#.

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
In this application we'll use server-side HTML templating with help of a seperate Suave package called `Suave.Experimental`.

> Note: As of the time of writing, Suave.Experimental is a separate package. It's likely that the public interface of the package will change in a breaking way in next releases. It's also possible the modules that we're going to use will be extracted to the core Suave package.

To use the package, we need to take the dependency on the following NuGet:
```install-package Suave.Experimental -version 0.26.2```

Before we start defining views, let's organize our `App.fs` source file by adding following line at the beginning of the file:

```
module SuaveMusicStore.App

open Suave
...
```

The line means that whatever we define in the file, it will be placed in `SuaveMusicStore.App`. 
Now let's add a new file `View.fs` to the project just before the `App.fs` file and place the following module definition at the very top:

```
module SuaveMusicStore.View
```

We'll follow such convention throughout the tutorial to have a clear understanding of the project structure.

> Note: It's very important that the `View.fs` file comes before `App.fs`. F# compiler requires the referenced items to be defined before their usage. At first glance, that might seem like a big drawback, however after a while you start realizing that you have better control of your dependencies. Read the [following](http://fsharpforfunandprofit.com/posts/cyclic-dependencies/) for further explanation.

With the `View.fs` file in place, let's add first view:

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

`Suave.Html` is a module which contains functions to generate HTML markup.
Next, we defined 3 helper functions for our purpose.
In the `index` value we can see the actual usage of those functions. 
`html` is a function that takes a list of other tags as its argument as well as `head` and `body`.
**// TODO: Describe the functions **

`xmlToString` transformates the object model into the resulting raw HTML string.
We can see usage of the "pipe" operator `|>` here. 
The operator might look familiar if you have some UNIX background.
In F#, the `|>` operator basically means: take the value on the left side and apply it to the function on the right side of the operator.
In our case it simply means: invoke the `xmlToString` function on the HTML object model.

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
We defined a submodule called `Store` in order to group routes related to themselves - later in the tutorial we'll have more submodules, each of them reflecting a specific set of functionality of the application.

The `IntPath` type alias that we declared will let use our routes in conjunction with strongly-typed routes with `pathScan` (`App.fs`). 
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

as well as in our `View`:

```
    divId "header" [
        h1 (aHref Path.home (text "F# Suave Music Store"))
    ]
```

Note, that in `App` module we still benefit from the static typed routes feature that Suave gives us - the `id` parameter is inferred by the compiler to be of integer type.

It's time to add some CSS styles to our HTML markup.
We'll not go into much details about the styles itself, as this is not a tutorial on Web Design.
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
If we have a look at our main `WebPart` we'll notice that there's really no handler capable of serving this very static file.
That's why we need to add another alternative to our `WebPart`:

```
    pathRegex "(.*)\.css" >>= Files.browseHome
```

The `pathRegex` `WebPart` returns `Some` if a incoming request concerns path that matches the regular expression pattern. 
If that's the case, the `Files.browseHome` WebPart will be applied.
`(.*)\.css` pattern matches every file with `.css` extension.
`Files.browseHome` is a `WebPart` from Suave that serves static files from the root application directory.

The CSS depends on `logo.png` asset, which can be downloaded from [here](https://raw.githubusercontent.com/theimowski/SuaveMusicStore/master/logo.png).

Add `logo.png` to the project, and again don't forget to select `Copy If Newer` for `Copy To Output Directory` property for the asset.

Now you should be able to see the styles applied to our HTML markup.

With our styles in place, let's get our hands on extracting a shared layout for all future views.
Start by adding `container` parameter to `index` in `View`:

```
let index container = 
    html [
...
```

and div "container" just after the div "header":

```
    divId "header" [
        h1 (aHref Path.home (text "F# Suave Music Store"))
    ]

    divId "container" container
```

`index` previosly was a constant value, but now becomes a function taking `container` as parameter.

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

Note that both `home` and `store` are constant values, while `browse` and `details` are parametrized with
`genre` and `id` respectively.

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

It's time to replace plain text in containers with meaningful content.
First, define `h2` in `View` module to output HTML header of level 2:

```
let h2 s = tag "h2" [] (text s)
```

and replace `text` with new `h2` in each of 4 containers.

We want the "/store" route to output hyperlinks to all genres in our Music Store.
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

Now, add the following for working with `ul` unordered list and `li` list item elements in HTML:

```
let ul xml = tag "ul" [] (flatten xml)
let li = tag "li" []
```

The actual container for `store` can now look something like this:

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

To use it from `App` module, let's simply pass a hardcoded list value for `genres` like following:

```
    path Path.Store.overview >>= html (View.store ["Rock"; "Disco"; "Pop"])
```

Here is what the solution looks like up to this point: [Tag - View](https://github.com/theimowski/SuaveMusicStore/tree/view)