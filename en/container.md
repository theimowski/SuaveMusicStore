# Container

With styles in place, let's get our hands on extracting a shared layout for all future views to come.
Start by adding `container` parameter to `index` in `View`:

==> View.fs:`let index`-`let index`

and div with id "main" just after the div "header":

==> View.fs:`body []`-`div ["id", "main"]`

`index` previously was a constant value, but it has now become a function taking `container` as parameter.

Let's also extract a common function for creating the WebPart, parametrized with the container itself.
Add to `App` module, just before the `browse` WebPart following function:

==> App.fs:`let html`

We can now define the actual container for the "home" page:

==> View.fs:`let home`

For now it will only contain plain "Home" text.

Next, containers for rest of valid routes in our application can be defined in `View` module:

==> View.fs:`let store`

==> View.fs:`let browse`

==> View.fs:`let details`

Note that both `home` and `store` are constant values, while `browse` and `details` are parametrized with `genre` and `id` respectively.

`html` can be now reused for all 4 views:

==> App.fs:`let browse`

==> App.fs:`let webPart`

