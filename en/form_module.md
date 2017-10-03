# Form module

We can delete an album, so why don't we proceed to add album functionality now?
It will require a bit more effort, because we actually need some kind of a form with fields to create a new album.
Fortunately, there's a helper module in Suave library exactly for this purpose.

> Note: `Suave.Form` module at the time of writing is still in `Experimental` package - just as `Suave.Html` which we're already using.

First, let's create a separate module `Form` to keep all of our forms in there (yes there will be more soon).
Add the `Form.fs` file just before `View.fs` - both `View` and `App` module will depend on `Form`.
As with the rest of modules, don't forget to follow our modules naming convention.

Now declare the first `Album` form:

==> Form.fs

`Album` type contains all fields needed for the form.
For the moment, `Suave.Form` supports following types of fields:

- decimal
- string
- System.Net.Mail.MailAddress
- Suave.Form.Password

> Note: the int type is not supported yet, however we can easily convert from decimal to int and vice versa

Afterwards comes a declaration of the album form itself.
It consists of list of "Props" (Properties), of which we can think as of validations:

- First, we declared that the `Title` must be of max length 100
- Second, we declared the same for `ArtUrl`
- Third, we declared that the `Price` must be between 0.01 and 100.0 with a step of 0.01 (this means that for example 1.001 is invalid)

Those properties can be now used as both client and server side.
For client side we will use the `album` declaration in our `View` module in order to output HTML5 input validation attributes.
For server side we will use an utility WebPart that will parse the form field values from a request.

> Note: the above snippet uses F# Quotations - a feature that you can read more about [here](https://msdn.microsoft.com/en-us/library/dd233212.aspx).
> For the sake of tutorial, you only need to know that they allow Suave to lookup the name of a Field from a property getter.

To see how we can use the form in `View` module, add `open Suave.Form` to the beginning:

==> View.fs:`module SuaveMusicStore.View`-`open Suave.Html`

Next, add this block of code:

==> View.fs:20-53

Above snippet is quite long but, as we'll soon see, we'll be able to reuse it a few times.
The `FormLayout` types defines a layout for a form and consists of:

- `SubmitText` that will be used for the string value of submit button
- `Fieldsets` - a list of `Fieldset` values
- `Form` - instance of the form to render

The `Fieldset` type defines a layout for a fieldset:

- `Legend` is a string value for a set of fields
- `Fields` is a list of `Field` values

The `Field` type has:

- a `Label` string
- `Html` - function which takes `Form` and returns `Node` (object model for HTML markup). It might seem cumbersome, but the signature is deliberate in order to make use of partial application

> Note: all of above types are generic, meaning they can accept any type of form, but the form's type must be consistent with the `FormLayout` hierarchy.

`renderForm` is a reusable function that takes an instance of `FormLayout` and returns HTML object model:

- it creates a form element
- the form contains a list of fieldsets, each of which:
    - outputs its legend first
    - iterates over its `Fields` and
        - outputs div element with label element for the field
        - outputs div element with target input element for the field
- the form ends with a submit button