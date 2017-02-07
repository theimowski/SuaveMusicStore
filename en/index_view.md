# Index view

With the `View.fs` file in place, let's add our first view:

==> View.fs

This will serve as a common layout in our application.
A few remarks about the above snippet:

- open `Suave.Html` module, for functions to generate HTML markup.
- `Node` is an internal Suave type holding object model for the HTML markup
- `index` is our representation of HTML markup.
- `html` is a function that takes a list of attributes and child nodes as its arguments (hover over the function to inspect its type signature). The same applies to `head`, `title`, `body` and `div` functions.
- `tag` function allows us to declare additional nodes. Take a look at its type signature - it's very similar to the mentioned functions but has an additional name argument at first position.
- `a` goes for creating hyperlinks.
- `Text` serves outputting plain text into an HTML element.
- `htmlToString` transforms the object model into the resulting raw HTML string.

We can see usage of the "pipe" operator `|>` in the above code. 
The operator might look familiar if you have some *nix background.
In F#, the `|>` operator basically means: take the value on the left side and apply it to the function on the right side of the operator.
In this very case it simply means: invoke the `htmlToString` function on the HTML object model.

Let's test the `index` view in our `App.fs`:

==> App.fs:15-17

If you navigate to the root url of the application, you should see that proper HTML is now being returned.
