# Create album

We can delete an album, so why don't we proceed to add album functionality now.
It will require a bit more effort, because we actually need some kind of a form with fields to create a new album.
Fortunately, there's a helper module in Suave library exactly for this purpose.

> Note: `Suave.Form` module at the time of writing is still in `Experimental` package - just as `Suave.Html` which we're already using.

First, let's create a separate module `Form` to keep all of our forms in there (yes there will be more soon).
Add the `Form.fs` file just before `View.fs` - both `View` and `App` module will depend on `Form`.
As with the rest of modules, don't forget to follow our modules naming convention.

Now declare the first `Album` form:

```fsharp
module SuaveMusicStore.Form

open Suave.Form

type Album = {
    ArtistId : decimal
    GenreId : decimal
    Title : string
    Price : decimal
    ArtUrl : string
}

let album : Form<Album> = 
    Form ([ TextProp ((fun f -> <@ f.Title @>), [ maxLength 100 ])
            TextProp ((fun f -> <@ f.ArtUrl @>), [ maxLength 100 ])
            DecimalProp ((fun f -> <@ f.Price @>), [ min 0.01M; max 100.0M; step 0.01M ])
            ],
          [])
```

`Album` type contains all fields needed for the form.
For the moment, `Suave.Form` supports following types of fields:

- decimal
- string
- System.Net.Mail.MailAddress
- Suave.Form.Password

> Note: the int type is not supported yet, however we can easily convert from decimal to int and vice versa

Afterwards comes a declaration of the album form (let album : `Form<Album>` =).
It consists of list of "Props" (Properties), of which we can think as of validations:

- First, we declared that the `Title` must be of max length 100
- Second, we declared the same for `ArtUrl`
- Third, we declared that the `Price` must be between 0.01 and 100.0 with a step of 0.01 (this means that for example 1.001 is invalid)

Those properties can be now used as both client and server side.
For client side we will the `album` declaration in our `View` module in order to output HTML5 input validation attributes.
For server side we will use an utility WebPart that will parse the form field values from a request.

> Note: the above snippet uses F# Quotations - a feature that you can read more about [here](https://msdn.microsoft.com/en-us/library/dd233212.aspx).
> For the sake of tutorial, you only need to know that they allow Suave to lookup the name of a Field from a property getter.

To see how we can use the form in `View` module, add `open Suave.Form` to the beginning:

```fsharp
module SuaveMusicStore.View

open System

open Suave.Html
open Suave.Form
```

Next, add a couple of helper functions:

```fsharp
let divClass c = divAttr ["class", c]

...

let fieldset x = tag "fieldset" [] (flatten x)
let legend txt = tag "legend" [] (text txt)
```

And finally this block of code:

```fsharp
type Field<'a> = {
    Label : string
    Xml : Form<'a> -> Suave.Html.Xml
}

type Fieldset<'a> = {
    Legend : string
    Fields : Field<'a> list
}

type FormLayout<'a> = {
    Fieldsets : Fieldset<'a> list
    SubmitText : string
    Form : Form<'a>
}

let renderForm (layout : FormLayout<_>) =    
    
    form [
        for set in layout.Fieldsets -> 
            fieldset [
                yield legend set.Legend

                for field in set.Fields do
                    yield divClass "editor-label" [
                        text field.Label
                    ]
                    yield divClass "editor-field" [
                        field.Xml layout.Form
                    ]
            ]

        yield submitInput layout.SubmitText
    ]
```

Above snippet is quite long but, as we'll soon see, we'll be able to reuse it a few times.
The `FormLayout` types defines a layout for a form and consists of:

- `SubmitText` that will be used for the string value of submit button
- `Fieldsets` - a list of `Fieldset` values
- `Form` - instance of the form to render

The `Fieldset` type defines a layout for a fieldset:

- `Legend` is a string value for a set of fields
- `Fields` is a list of `Field` values

The `Field` type has:

- a `Label` string
- `Xml` - function which takes `Form` and returns `Xml` (object model for HTML markup). It might seem cumbersome, but the signature is deliberate in order to make use of partial application

> Note: all of above types are generic, meaning they can accept any type of form, but the form's type must be consistent in the `FormLayout` hierarchy.

`renderForm` is a reusable function that takes an instance of `FormLayout` and returns HTML object model:

- it creates a form element
- the form contains a list of fieldsets, each of which:
    - outputs its legend first
    - iterates over its `Fields` and
        - outputs div element with label element for the field
        - outputs div element with target input element for the field
- the form ends with a submit button

`renderForm` ca be invoked like this:

```fsharp
let createAlbum genres artists = [ 
    h2 "Create"
        
    renderForm
        { Form = Form.album
          Fieldsets = 
              [ { Legend = "Album"
                  Fields = 
                      [ { Label = "Genre"
                          Xml = selectInput (fun f -> <@ f.GenreId @>) genres None }
                        { Label = "Artist"
                          Xml = selectInput (fun f -> <@ f.ArtistId @>) artists None }
                        { Label = "Title"
                          Xml = input (fun f -> <@ f.Title @>) [] }
                        { Label = "Price"
                          Xml = input (fun f -> <@ f.Price @>) [] }
                        { Label = "Album Art Url"
                          Xml = input (fun f -> <@ f.ArtUrl @>) ["value", "/placeholder.gif"] } ] } ]
          SubmitText = "Create" }

    div [
        aHref Path.Admin.manage (text "Back to list")
    ]
]
```

We can see that for the `Xml` values we can invoke `selectInput` or `input` functions.
Both of them take as first argument function which directs to field for which the input should be generated.
`input` takes as second argument a list of optional attributes (of type `string * string` - key and value).
`selectInput` takes as second argument list of options (of type `decimal * string` - value and display name).
As third argument, `selectInput` takes an optional selected value - in case of `None`, the first one will be selected initially.

> Note: We are hardcoding the album's `ArtUrl` property with "/placeholder.gif" - we won't implement uploading images, so we'll have to stick with a placeholder image.

Now that we have the `createAlbum` view, we can write the appropriate WebPart handler.
Start by adding `getArtists` to `Db`:

```fsharp
type Artist = DbContext.``[dbo].[Artists]Entity``

...

let getArtists (ctx : DbContext) : Artist list = 
    ctx.``[dbo].[Artists]`` |> Seq.toList
```

Then proper entry in `Path` module:

```fsharp
    let createAlbum = "/admin/create"
```

and WebPart in `App` module:

```fsharp
let createAlbum =
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ -> 
            let genres = 
                Db.getGenres ctx 
                |> List.map (fun g -> decimal g.GenreId, g.Name)
            let artists = 
                Db.getArtists ctx
                |> List.map (fun g -> decimal g.ArtistId, g.Name)
            html (View.createAlbum genres artists))
    ]

...

    path Path.Admin.createAlbum >=> createAlbum
```

Once again, `warbler` will prevent from eager evaluation of the WebPart - it's vital here.
To our `View.manage` we can add a link to `createAlbum`:

```fsharp
let manage (albums : Db.AlbumDetails list) = [ 
    h2 "Index"
    p [
        aHref Path.Admin.createAlbum (text "Create New")
    ]
...
```

This allows us to navigate to "/admin/create", however we're still lacking the actual POST handler.

Before we define the handler, let's add another helper function to `App` module:

```fsharp
let bindToForm form handler =
    bindReq (bindForm form) handler BAD_REQUEST
```

It requires a few modules to be open, namely:

- `Suave.Form`
- `Suave.Http.RequestErrors`
- `Suave.Model.Binding`

What `bindToForm` does is:

- it takes as first argument a form of type `Form<'a>`
- it takes as second argument a handler of type `'a -> WebPart`
- if the incoming request contains form fields filled correctly, meaning they can be parsed to corresponding types, and hold all `Prop`s defined in `Form` module, then the `handler` argument is applied with the values of `'a` filled in
- otherwise the 400 HTTP Status Code "Bad Request" is returned with information about what was malformed.

There are just 2 more things before we're good to go with creating album functionality.

We need `createAlbum` for the `Db` module (the created album is piped to `ignore` function, because we don't need it afterwards):

```fsharp
let createAlbum (artistId, genreId, price, title) (ctx : DbContext) =
    ctx.``[dbo].[Albums]``.Create(artistId, genreId, price, title) |> ignore
    ctx.SubmitUpdates()
```

as well as POST handler inside the `createAlbum` WebPart:

```fsharp
choose [
        GET >=> ...

        POST >=> bindToForm Form.album (fun form ->
            Db.createAlbum (int form.ArtistId, int form.GenreId, form.Price, form.Title) ctx
            Redirection.FOUND Path.Admin.manage)
    ]
```
