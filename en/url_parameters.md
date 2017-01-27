# Url parameters

In addition to that static string path, we can specify route arguments.
Suave comes with a cool feature called "typed routes", which gives you statically typed control over arguments for your route. As an example, let's see how we can add `id` of an album to the details route:

```fsharp
pathScan "/store/details/%d" (fun id -> OK (sprintf "Details: %d" id))
```

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

```fsharp
pathScan "/store/details/%s/%d" (fun (a, id) -> OK (sprintf "Artist: %s; Id: %d" a id))
```

for request `http://localhost:8083/store/details/abba/1`

For more information on working with strings in a statically typed way, visit [this site](http://fsharpforfunandprofit.com/posts/printf/)
