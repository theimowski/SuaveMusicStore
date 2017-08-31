# View amends

First we'll create a left-sided navigation menu with all possible genres in our Store. 
This will allow users to find their favorite tracks faster.

==> View.fs:`let partGenres`

`partGenres` creates an unordered list with direct links to each genre.

To include it in the main index view, let's pass it as a new parameter, and render just before the `container`:

==> View.fs:`let index`

This forces us to extend the invocation in nested `result` function in the `html` WebPart:

==> App.fs:`let html`

How about we make the album list in `View.browse` look better?

==> View.fs:`let browse`

The above view will remain a plain unordered list, but in addition to the title we'll also display the album's art as an image.

You've probably noticed that our home page is not very sophisticated. In fact it doesn't display anything other than a plain "Home" caption. Why don't we add an image banner as well as a list of best-seller albums to the home page?

First, let's fetch the best-sellers from `Db`: 

==> Db.fs:`type BestSeller`

==> Db.fs:`let getBestSellers`

Now we can alter the `View.home`:

==> View.fs:`let home`

and the `home` handler in `App` module:

==> App.fs:`let home`

==> App.fs:`let webPart`

The "home-showcase.png" asset can be downloaded from [here](https://raw.githubusercontent.com/theimowski/SuaveMusicStore/master/home-showcase.png). Don't forget about the "Copy To Output Directory" property!
