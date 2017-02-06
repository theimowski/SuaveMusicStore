# Index view

With the `View.fs` file in place, let's add our first view:

```fsharp
module SuaveMusicStore.View

open Suave.Html

let divId id = divAttr ["id", id]
let h1 xml = tag "h1" [] xml
let aHref href = tag "a" ["href", href]

let index = 
    html [
        head [
            title "Suave Music Store"
        ]

        body [
            divId "header" [
                h1 (aHref "/" (text "F# Suave Music Store"))
            ]

            divId "footer" [
                text "built with "
                aHref "http://fsharp.org" (text "F#")
                text " and "
                aHref "http://suave.io" (text "Suave.IO")
            ]
        ]
    ]
    |> xmlToString
```

This will serve as a common layout in our application.
A few remarks about the above snippet:

- open `Suave.Html` module, for functions to generate HTML markup.
- 3 helper functions come next:
    - `divId` which appends "div" element with a string attribute `id`
    - `h1` which takes inner markup to generate HTML header level 1.
    - `aHref` which takes string attribute `href` and inner HTML markup to output "a" element.
- `tag` function comes from Suave. It's of type `string -> Attribute list -> Xml -> Xml`. First arg is name of the HTML element, second - a list of attributes, and third - inner markup
- `Xml` is an internal Suave type holding object model for the HTML markup
- `index` is our representation of HTML markup. 
- `html` is a function that takes a list of other tags as its argument. So do `head` and `body`.
- `text` serves outputting plain text into an HTML element.
- `xmlToString` transforms the object model into the resulting raw HTML string.

> Note: `tag` function from Suave takes 3 arguments ().
> We've defined the `aHref` function by invoking `tag` with only 2 arguments, and the compiler is perfectly happy with that - Why?
> This concept is called "partial application", and allows us to invoke a function by passing only a subset of arguments.
> When we invoke a function with only a subset of arguments, the function will return another function that will expect the rest of arguments.
> In our case this means `aHref` is of type `string -> Xml -> Xml`, so the second "hidden" argument to `aHref` is of type `Xml`.
> Read [here](http://fsharpforfunandprofit.com/posts/partial-application/) for more info about partial application.

We can see usage of the "pipe" operator `|>` in the above code. 
The operator might look familiar if you have some UNIX background.
In F#, the `|>` operator basically means: take the value on the left side and apply it to the function on the right side of the operator.
In this very case it simply means: invoke the `xmlToString` function on the HTML object model.

Let's test the `index` view in our `App.fs`:
```fsharp
    path "/" >=> (OK View.index)
```

If you navigate to the root url of the application, you should see that proper HTML has been returned.
