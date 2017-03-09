# Navigation partial

As a warm-up, let's add navigation menu at the very top of the view.
We'll call it `partNav` and keep in a separate value:

==> View.fs:`let partNav`

`partNav` consists of 3 main tabs: "Home", "Store" and "Admin". `ulAttr` can be defined like following:

==> View.fs:`let ulAttr`

We want to specify the `id` attribute here so that our CSS can make the menu nice and shiny.
Add the `partNav` to main index view, in the "header" `div`:

==> View.fs:`div ["id", "header"]`

This gives a possibility to navigate through main features of our Music Store.