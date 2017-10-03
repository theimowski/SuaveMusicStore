# HTML markup

It's time to replace plain text placeholders in containers with meaningful content.
First, define `h2` in `View` module to output HTML header of level 2:

==> View.fs:`let h2`

and replace `text` with a new `h2` in each of the 4 containers.

We'd like the `/store` route to output hyperlinks to all genres in our Music Store.
Let's add a helper function in `Path` module, that will be responsible for formatting HTTP urls with a key-value parameter:

==> Path.fs:`let withParam`

The `withParam` function takes a tuple `(key,value)` as its first argument, `path` as the second and returns a properly formatted url.
A tuple (or a pair) is a widely used structure in F#. It allows us to group two values into one in an easy manner. 
Syntax for creating a tuple is as follows: `(item1, item2)` - this might look like a standard parameter passing in many other languages including C#.
Follow [this link](http://fsharpforfunandprofit.com/posts/tuples/) to learn more about tuples.

Add also a string key for the url parameter "/store/browse" in `Path.Store` module:

==> Path.fs:`let browseKey`

We'll use it in `App` module:

==> App.fs:`let browse`

Now, add the following for working with the unordered list (`ul`) and list item (`li`) elements in HTML:

==> View.fs:`let ul`-`let li`

The actual container for `store` can now look like the following:

==> View.fs:`let store`

Things worth commenting in the above snippet:

- `store` now takes a list of genres (again the type is inferred by the compiler)
- the `[ for g in genres -> ... ]` syntax is known as "list comprehension". Here we map every genre string from `genres` to a list item
- `aHref` inside list item points to the `Path.Store.browse` url with "genre" parameter - we use the `Path.withParam` function defined earlier

To use `View.store` from `App` module, let's simply pass a hardcoded list for `genres` like following:

==> App.fs:`path Path.Store.overview`

