# Hello World from Suave

Let's have a look at the boilerplate App.fs file:

==> App.fs

* The `open` statements at the top of the file are the same as `using` statements in C# - they give access to all declarations inside given module.
* Note there is no `Main` method defined in `App.fs` - what happens here is that the `startWebServer` function is invoked immediately after the program is run and Suave starts listening for incoming requests untill the process is killed (which is ok as long as we're hosting in console).

If we managed to build the project in previous section, we should be able to run it from command line:

Windows

```
> .\bin\Debug\net461\SuaveMusicStore.exe
```

Mac / Linux

```
$ mono ./bin/Debug/net461/SuaveMusicStore.exe
```

The program spins up a web server with default Suave configuration, so if everything works fine we should see following output in console:

```
[06:19:25 INF] Smooth! Suave listener started in 91.979 with binding 127.0.0.1:8080
```

> Note: Suave by default works on port 8080, but if it is already in use, Suave will look for a nearest free one.

and all HTTP requests againsts that URL should end with following response:

```
HTTP/1.1 200 OK
Server: Suave (https://suave.io)
Date: Tue, 22 Aug 2017 04:22:34 GMT
Content-Type: text/html
Content-Length: 12

Hello World!
```

Guess what - you already have a working web app!
If you browse that url, you should be greeted with the classic `Hello World!`.

## WebPart

You may be wondering what the above code actually does. Take a look at `startWebServer` type signature (hover over the symbol to display tool tip). The function from Suave library basically takes object of type `SuaveConfig` as first argument, `WebPart` as second argument, and returns `unit`.

`defaultConfig` is a value from the Suave library of type `SuaveConfig`, and as the name suggests it determines the default configuration of server. For now we only need to know that among other stuff it configures the HTTP binding, meaning that in our case the server is listening on port 8083 on loopback address 127.0.0.1.

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

If the above explanation of `WebPart` doesn't yet make much sense to you, or you don't understand why it has such type signature, bear with me - in next sections we'll try to prove that this type turns out to be really handy when it comes to one of the greatest powers of functional programming paradigm: **composition**.
