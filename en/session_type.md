# Session type

Up to this point, we should be able to authenticate with "admin" -> "admin" credentials to our application.
This is however not very useful, as there are no handlers that would demand user to be authenticated yet.

To change that, let's define custom types to represent user state:

```fsharp
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

- immutability by default
- structural equality by default
- pattern matching
- copy-and-update expression

Again, if you want to learn more about Records, make sure you visit [this](http://fsharpforfunandprofit.com/posts/records/) post.

With these two types, we'll be able to distinguish from when a user is logged on to our application and when he is not.

As stated before, we'll now add a parameter to the `session` function:

```fsharp
let session f = 
    statefulForSession
    >=> context (fun x -> 
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

```fsharp
...
authenticated Cookie.CookieLife.Session false 
>=> session (fun _ -> succeed)
>=> sessionStore (fun store ->
...
```

Yes I know, I promised we'll pass something funky to the `session` function, but bear with with me - we will later.
For the moment usage of `session` in `logon` doesn't require any custom action, but we still need to invoke it to "initalize" the user state.
