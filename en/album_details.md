# Album details

It's time to read album's details from the database. 
Start by adjusting the `details` in `View` module:

==> View.fs:34-46

Above snippet requires defining one more helper functions in `View`:

==> View.fs:5-5

In the `details` function we used list comprehension syntax with an inline list of tuples (`["Genre:",album.Genre;...`).
This is just to save us some time from typing the `p` element three times for all those properties.
You're welcome to change the implementation so that it doesn't use this shortcut if you like.

The `AlbumDetails` database view turns out to be handy now, because we can use all the attributes we need in a single step (no explicit joins required).

To read the album's details in `App` module we can do following:

==> App.fs:29-34

==> App.fs:41-41

A few remarks regarding above snippet:

- `details` takes `id` as parameter and returns WebPart
- `Path.Store.details` of type IntPath guarantees type safety
- `Db.getAlbumDetails` can return `None` if no album with given id is found
- If an album is found, html WebPart with the `View.details` container is returned
- If no album is found, `None` WebPart is returned with help of `never`.

No pipe operator was used this time, but as an exercise you can think of how you could apply it to the `details` WebPart.

Before testing the app, add the "placeholder.gif" image asset. 
You can download it from [here](https://raw.githubusercontent.com/theimowski/SuaveMusicStore/master/placeholder.gif).
Don't forget to set "Copy To Output Directory", as well as add new file extension to the `pathRegex` in `App` module.
