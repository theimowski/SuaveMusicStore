Suave Music Store
=================

Introduction
------------

This is a tutorial on how to create an application with [F#](http://fsharp.org) and [Suave.IO](http://suave.io) framework. 
It's inspired by the Music Store tutorial created by the ASP.NET team [available here](http://www.asp.net/mvc/overview/older-versions/mvc-music-store/mvc-music-store-part-1).
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
Now we can add NuGet reference to Suave. To do that, in Package Manager Console type: `install-package Suave`. 
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
Value, once bound, can't be mutated later during runtime.
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
Suave comes with a cool feature called "typed routes", which gives you statically typed control over arguments for your route. As example, let's see how we can add `id` of an album to the details route:

`pathScan "/store/details/%d" (fun id -> OK (sprintf "Details: %d" id))`