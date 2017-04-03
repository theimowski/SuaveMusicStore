# Create album action

Before we define the handler, let's add another helper function to `App` module:

==> App.fs:44-45

It requires a few modules to be open, namely:

==> App.fs:5-5
==> App.fs:6-6

What `bindToForm` does is:

- it takes as first argument a form of type `Form<'a>`
- it takes as second argument a handler of type `'a -> WebPart`
- if the incoming request contains form fields filled correctly, meaning they can be parsed to corresponding types, and hold all `Prop`s defined in `Form` module, then the `handler` argument is applied with the values of `'a` filled in
- otherwise the 400 HTTP Status Code "Bad Request" is returned with information about what was malformed.

There are just 2 more things before we're good to go with creating album functionality.

We need `createAlbum` for the `Db` module (the created album is piped to `ignore` function, because we don't need it afterwards):

==> Db.fs:56-58

as well as POST handler inside the `createAlbum` WebPart:

==> App.fs:`let createAlbum`


