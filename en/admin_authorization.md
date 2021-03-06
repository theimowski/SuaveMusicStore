# Admin authorization

There are a few more helper functions needed before we can set up proper authorization for `/admin` handlers.
Add following to `App` module:

==> App.fs:`open Suave.Cookie`

==> App.fs:`let reset`-`))`

Remarks:

- `reset` is a WebPart to clean up auth and state cookie values, and redirect to home page afterwards. We'll use it for logging user off.
- `redirectWithReturnPath` aims to point user to some url, with the "returnPath" query parameter baked into the url. We'll use it for redirecting user to logon page if specific action requires authentication.
- `loggedOn` takes `f_success` WebPart as argument, which will be applied if user is authenticated. Here we use the library function `authenticate`, to which `f_success` comes as last parameter. The rest of parameters, starting with first are are respectively:
    - `CookieLife` - `Session` in our case
    - `httpsOnly` - we pass false as we won't cover HTTPS bindings in the tutorial (however Suave does support it).
    - 3rd parameter is a function applied if auth cookie is missing - that's where we want to redirect user to the logon page with a "returnPath"
    - 4th parameter is a function applied if there occurred a decryption error. In real world this could mean a malformed request, however at current stage we'll stick to reseting the state. This way we can re-run the server multiple times during development, without worrying about the browser to pass a cookie value encrypted with stale server key (Suave regenerates a new server key each time it is run).
- `admin` also takes `f_success` WebPart as argument. Here, we invoke `loggedOn` with an inline function using `session`. The interesting part is inside the `session` function:
    - syntax `function | ... -> ` is just a shorter version of `match x with | ... -> ` but the `x` param is implicit here, and `x` is of type `Session`
    - first pattern shows the real power of the pattern matching technique - `f_success` will be applied only if user is logged on, and his Role is "admin" (we'll distinguish between "admin" and "user" roles)
    - second pattern holds if user is logged on but with different role, thus we return 403 Forbidden status code
    - the last "otherwise" (`_`) pattern should never hold, because we're already inside `loggedOn`. We use it anyway, just as a safety net.

That was quite long, but worth it. Finally we're able to guard the `/admin` actions:

==> App.fs:`path Path.Admin.manage`-`pathScan Path.Admin.deleteAlbum`

Go and have a look what happens when you try to navigate to `/admin/manage` route.

We still need to update the `partUser`, when a user is logged on (remember we hardcoded `None` for username).
To do this, we can pass `partUser` as parameter to `View.index`:

==> View.fs:`let index`-`partUser`

and determine whether a user is logged on in the `html` WebPart in `App` module:

==> App.fs:`let html`

We declared a sub function `result` which takes the `user` parameter.
`session` can be used to determine user state.
Effectively, we invoke the `result` function always but with `user` argument based on the user state.

The last thing we need to support is `logoff`. In the main `choose` WebPart add:

==> App.fs:`path Path.Account.logoff`

`logoff` doesn't require separate WebPart, `reset` can be reused instead.

That concludes our journey to Auth and Session features in `Suave` library. 
We'll revisit the concepts in next section, but much of the implementation can be reused.
