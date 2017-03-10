# Logon handler

Things get more complicated with regards to the POST handler.
As a gentle introduction, we'll add logic to verify passed credentials - by querying the database (`Db` module):

==> Db.fs:`let validateUser`

The snippet makes use of `User` type alias:

==> Db.fs:`type User`

Now, in the `App` module add more `open` statements:

==> App.fs:1-13

and add a couple of helper functions:

==> App.fs:`let passHash`

==> App.fs:`let session`

==> App.fs:`let sessionStore`

==> App.fs:`let returnPathOrHome`

Comments:

- `passHash` - from a given string it creates a SHA256 hash and formats it to hexadecimal. That's how users' passwords are stored in our database.
- `session` for now is just an alias to `statefulForSession` from Suave, which initializes a user state for a browsing session. We will however add extra argument to the `session` function in a few minutes, that's why we might want to have it extracted already.
- `sessionStore` is a higher-order function, taking `setF` as a parameter - which in turn can be used to read from or write to the session store.
- `returnPathOrHome` tries to extract "returnPath" query parameter from the url, and redirects to that path if it exists. If no "returnPath" is found, we get back redirected to the home page.

Now turn for the `logon` POST handler monster:

==> App.fs:`let logon`

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
