# Logon handler

Things get more complicated with regards to the POST handler.
As a gentle introduction, we'll add logic to verify passed credentials - by querying the database (`Db` module):

```fsharp
let validateUser (username, password) (ctx : DbContext) : User option =
    query {
        for user in ctx.``[dbo].[Users]`` do
            where (user.UserName = username && user.Password = password)
            select user
    } |> firstOrNone
```

The snippet makes use of `User` type alias:

```fsharp
type User = DbContext.``[dbo].[Users]Entity``
```

Now, in the `App` module add more `open` statements:

```fsharp
open System
...
open Suave.Authentication
open Suave.State.CookieStateStore
```

and add a couple of helper functions:

```fsharp
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

```fsharp
let logon =
    choose [
        GET >=> (View.logon |> html)
        POST >=> bindToForm Form.logon (fun form ->
            let ctx = Db.getContext()
            let (Password password) = form.Password
            match Db.validateUser(form.Username, passHash password) ctx with
            | Some user ->
                    authenticated Cookie.CookieLife.Session false 
                    >=> session
                    >=> sessionStore (fun store ->
                        store.set "username" user.UserName
                        >=> store.set "role" user.Role)
                    >=> returnPathOrHome
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
First, `authenticated` sets proper cookies which live till the session ends. The second (`false`) argument specifies the cookie isn't "HttpsOnly".
Then we bind the result to `session`, which as described earlier, sets up the user session state.
Next, we write two values to the session store: "username" and "role".
Finally, we bind to `returnPathOrHome` - we'll shortly see how this one can be useful.

You might have noticed, that the above code will results in "Not found" page in case `Db.validateUser` returns None.
That's because we temporarily assigned `never` to the latter match.
Ideally, we'd like to see some kind of a validation message next to the form.
To achieve that, let's add `msg` parameter to `View.logon`:

```fsharp
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

```fsharp
GET >=> (View.logon "" |> html)

...

View.logon "Username or password is invalid." |> html
```

The first one being GET `logon` handler, and the other one being returned if provided credentials are incorrect.
