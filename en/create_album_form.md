# Create album form

Because of possible namespace conflicts between `Suave.Form.input` and `Suave.Html.input`, let's define a helper function to the former:

==> View.fs:`let formInput`

To define a form for album creation, we can invoke `renderForm` like this:

==> View.fs:`let createAlbum`

We can see that for the `Html` values we can invoke `selectInput` or `input` functions.
Both of them take as first argument function which directs to field for which the input should be generated.
`input` takes as second argument a list of optional attributes (of type `string * string` - key and value).
`selectInput` takes as second argument list of options (of type `decimal * string` - value and display name).
As third argument, `selectInput` takes an optional selected value - in case of `None`, the first one will be selected initially.

> Note: We are hardcoding the album's `ArtUrl` property with "/placeholder.gif" - we won't implement uploading images, so we'll have to stick with a placeholder image.

Now that we have the `createAlbum` view, we can write the appropriate WebPart handler.
Start by adding `getArtists` to `Db`:

==> Db.fs:`type Artist`

==> Db.fs:`let getArtists`

Then proper entry in `Path` module:

==> Path.fs:`module Admin`

and WebPart in `App` module:

==> App.fs:`let createAlbum`

==> App.fs:`path Path.Admin.createAlbum`

Once again, `warbler` will prevent from eager evaluation of the WebPart - it's vital here.
To our `View.manage` we can add a link to `createAlbum`:

==> View.fs:`let manage`

This allows us to navigate to "/admin/create", however we're still lacking the actual POST handler.
