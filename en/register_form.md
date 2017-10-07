# Register form

Register feature will be based on a standard form, so let's add one to the `Form` module:

==> Form.fs:`type Register`-`],[ passwordsMatch`

In the above snippet:

- the form consists of 4 fields:
    - `Username` of `string`
    - `Email` of type `string` (see note below)
    - `Password` of type `Password` (ensures proper HTML input type)
    - `ConfirmPassword` of the same type
- `pattern` is a regular expression pattern for password
- `passwordsMatch` is a server-side only validation function
- `register` is the definition of our form, with a few constraints:
    - `Username` must be at most 30 characters long
    - `Password` must match the regular expression
    - `ConfirmPassword` must match the regular expression

> Note: In initial version, the `Email` field used `System.Net.Mail.MailAddress` type, however I had hard times using the same type when building with .NET CLI for this version, that's why I falled back to a plain string for email field. Here's the corresponding [GitHub issue](https://github.com/fsprojects/Paket/issues/2829) to track.

Server-side only validation, like `passwordMatch` are of type `('FormType -> bool) * string`.
So this is just a tuple of a predicate function and a string error.
We can create as many validations as we like, and pass them to the `Form` definition.
These can be used for validations that lookup more than one field, or require some complex logic.
We won't create client-side validation to check if the passwords match in the tutorial, but it could be achieved with some custom JavaScript code.

With the form definition in place, let's proceed to `View`:

==> View.fs:`let register`

As you can see, we're using the `msg` parameter here similar to how it was done in `View.logon` to include possible error messages.
The rest of the snippet is rather self-explanatory.

We're now left with proper `Path.Account` entry:

==> Path.fs:`module Account`

GET handler for registration in `App`:

==> App.fs:`let register`

==> App.fs:`path Path.Account.register`

and a direct link from the `View.logon` :

==> View.fs:`let logon`-`div ["id", "logon-message"]`

This allows us to navigate to the registration form.