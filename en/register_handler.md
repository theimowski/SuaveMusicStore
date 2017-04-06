# Register handler

Moving on to implementing the actual POST handler, let's first create necessary functions in `Db` module:

==> Db.fs:`let getUser`

==> Db.fs:`let newUser`

`getUser` will be crucial to check if a user with given `username` already exists in the database - we don't want two users with the same `username`.
`newUser` is a simple function that creates and returns the new user.
Note that we hardcode "user" for each new user's role. 
This way, they can be distinguished from admin's role.

After a successful registration, we'd like to authenticate user at once - in other words apply the same logic which happens after successful logon.
In a real application, you'd probably use a confirmation mail mechanism, but for the sake of simplicity we'll skip that.
In order to reuse the logic from logon POST handler, extract a separate function:

==> App.fs:`let authenticateUser`

after extraction, `logon` POST handler looks like this:

==> App.fs:`POST >=> bindToForm Form.logon`-`View.logon "Username or password is invalid."`

Finally, the full register handler can be implemented following:

==> App.fs:`let register`

Comments for POST part:

- bind to `Form.register` to validate the request
- check if a user with given `username` already exists
- if that's the case then show the `View.register` form again with a proper error message
- otherwise read the form fields' values, create new user and invoke the `authenticateUser` function

This concludes register feature - we're now set for new customers to do the shopping.
Wait a second, they can put albums to the cart, but how do they checkout?
Ah yes, we haven't implemented that yet.
