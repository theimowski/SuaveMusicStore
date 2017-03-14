# Cart navigation menu

Remember our navigation menu with `Cart` tab? 
Why don't we add a number of total albums in our cart there?
To do that, let's parametrize the `partNav` view:

==> View.fs:`let partNav`

as well as add the `partNav` parameter to the main `index` view:

==> View.fs:290-290

In order to create the navigation menu, we now need to pass `cartItems` parameter.
It has to be resolved in the `html` function from `App` module, which can now look like following:

==> App.fs:`let html`

The change is that the nested `result` function in `html` now takes two arguments: `cartItems` and `user`. 
Additionally, we handle all cases inside the `session` invocation.
Note how the `result` function is passed different set of parameters in different cases.
