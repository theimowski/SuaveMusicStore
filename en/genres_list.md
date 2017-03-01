# Genres list

For more convenient instantiation of `DbContext`, let's introduce a small helper function in `Db` module:

==> Db.fs:16-16

Now we're ready to finally read real data in the `App` module:

==> App.fs:18-23

==> App.fs:28-28

`overview` is a WebPart that... 
Hold on, do I really need to explain it?
The usage of pipe operator here makes the flow rather obvious - each line defines each step.
The return value is passed from one function to another, starting with DbContext and ending with the WebPart.
This is just a single example of how composition in functional programming makes functions look like building blocks "glued" together.

We also had to wrap the `overview` WebPart in a `warbler`, because otherwise the `overview` WebPart would be in a sense static - there'd be no reason not to cache the result.
`warbler` ensures that genres will be fetched from the database whenever a new request comes.
Otherwise, without the `warbler` in place, the genres would be fetched only at the start of the application - resulting in stale genres in case the list changes.

Do we need also to wrap with `warbler` the rest of WebParts?

- `browse` is parametrized with genre name - each request will result in a database query anyway, so the answer is negative.
- `details` is parametrized with the id - same as above.
- `home` is just fine - for the moment it's completely static and doesn't need to touch the database.
