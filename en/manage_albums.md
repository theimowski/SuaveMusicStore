# Manage albums

Let's start by adding `manage` to our `View` module:

==> View.fs:57-73

The view requires a few of new helper functions for table HTML markup:

==> View.fs:10-13

as well as a `truncate` function that will ensure our cell content doesn't span over a maximum number of characters:

==> View.fs:52-55

Remarks:

- our HTML table consists of first row (`tr`) containing column headers (`th`) and a set of rows for each album with cells (`td`) to display specific values.
- we used the `yield` keyword for the first time. It is required here because we're using it in conjunction with the `for album in albums ->` list comprehension syntax inside the same list. The rule of thumb is that whenever you use the list comprehension syntax, then you need the `yield` keyword for any other item not contained in the comprehension syntax. This might seem hard to remember, but don't worry - the compiler is helpful here and will issue a warning if you forget the `yield` keyword.
- for the sake of saving a few keystrokes we used a nested list comprehension syntax to output `th`s and `td`s. Again, it's just a matter of taste, and could be also solved by enumerating each element separately

We are going to need to fetch the list of all `AlbumDetail`s from the database. 
For this reason, let's create following query in `Db` module:

==> Db.fs:38-39

Now we're ready to define an actual handler to display the list of albums.
Let's add a new sub-module to `Path`:

==> Path.fs:16-17

The `Admin` sub-module will contain all album management paths or routes if you will.

`manage` WebPart in `App` module can be implemented in following way:

==> App.fs:36-40

and used in the main `choose` WebPart:

==> App.fs:42-53

Don't forget about the `warbler` for `manage` WebPart - we don't use any parameters for this WebPart, so we need to prevent it's eager evaluation.

If you navigate to the "/admin/manage" url in the application now, you should be presented the grid with every album in the store.
