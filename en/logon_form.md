# Logon form

There's no handler for the `logon` route yet, so we need to create one.
Logon view will be rather straightforward - just a simple form with username and password.

==> Form.fs:`type Logon`

==> Form.fs:`let logon`

Above snippets shows how the `logon` form can be defined in our `Form` module.
`Password` is a type from Suave library and helps to determine the input type for HTML markup (we don't want anyone to see our secret pass as we type it).

==> View.fs:`let logon`

As I promised, nothing fancy here.
We've already seen how the `renderForm` works, so the above snippet is just another plain HTML form with some additional instructions at the top.

The GET handler for `logon` is also very simple:

==> App.fs:`let logon`

==> App.fs:`path Path.Account.logon`
