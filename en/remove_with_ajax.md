# Remove with AJAX

Now that we can add albums to the cart, and see the total number both in `cart` overview and navigation menu, it would be nice to be able to remove albums from the cart as well.
This is a great occasion to employ AJAX in our application.
We'll write a simple script in JS that makes use of jQuery to remove selected album from the cart and update the cart view.

Download jQuery from [here](https://jquery.com/download/) (I used the compressed / minified version) and add it to the project.
Don't forget to set the "Copy to Output Directory" property.

Now add new JS file to the project `script.js`, and fill in its contents:

```js
$('.removeFromCart').click(function () {
    var albumId = $(this).attr("data-id");
    var albumTitle = $(this).closest('tr').find('td:first-child > a').html();
    var $cartNav = $('#navlist').find('a[href="/cart"]');
    var count = parseInt($cartNav.html().match(/\d+/));

    $.post("/cart/remove/" + albumId, function (data) {
        $('#main').html(data);
        $('#update-message').html(albumTitle + ' has been removed from your shopping cart.');
        $cartNav.html('Cart (' + (count - 1) + ')');
    });
});
```

We won't go into much details about the code itself, however it's important to know that the script:

- subscribes to click event on each `removeFromCart` element
- parses information such as:
    - album id
    - album title
    - count of this album in cart
- sends a POST request to "/cart/remove" endpoint 
- upon successful POST response it updates:
    - html of the container element
    - message, to indicate which album has been removed
    - navigation menu to decrement count of albums

The `update-message` div should be added to the `nonEmptyCart` view, before the table:

```fsharp
    divId "update-message" [text " "]
```

We explicitly have to pass in non-empty text, because we cannot have an empty div element in HTML markup.
With jQuery and our `script.js` files, we can now attach them to the end of `nonEmptyCart` view, just after the table:

```fsharp
scriptAttr [ "type", "text/javascript"; " src", "/jquery-1.11.3.min.js" ] [ text "" ]
scriptAttr [ "type", "text/javascript"; " src", "/script.js" ] [ text "" ]
```

We also need to allow browsing for files with "js" extension in our handler:

```fsharp
pathRegex "(.*)\.(css|png|gif|js)" >=> Files.browseHome
```

The script tries to reach route that is not mapped to any handler yet.
Let's change that by first adding `removeFromCart` to `Db` module:

```fsharp
let removeFromCart (cart : Cart) albumId (ctx : DbContext) = 
    cart.Count <- cart.Count - 1
    if cart.Count = 0 then cart.Delete()
    ctx.SubmitUpdates()
```

then adding the `removeFromCart` handler in `App` module:

```fsharp
let removeFromCart albumId =
    session (function
    | NoSession -> never
    | UserLoggedOn { Username = cartId } | CartIdOnly cartId ->
        let ctx = Db.getContext()
        match Db.getCart cartId albumId ctx with
        | Some cart -> 
            Db.removeFromCart cart albumId ctx
            Db.getCartsDetails cartId ctx |> View.cart |> Html.flatten |> Html.xmlToString |> OK
        | None -> 
            never)
```

and finally mapping the route to the handler in main `choose` WebPart:

```fsharp
pathScan Path.Cart.removeAlbum removeFromCart
```

A few comments to the `removeFromCart` WebPart:

- this handler should not be invoked with `NoSession`, `never` prevents from unwanted requests
- the same happens, when someone tries to invoke `removeFromCart` for `albumId` not present in his cart (Db.getCart returns `None`)
- if proper cart has been found, `Db.removeFromCart` is invoked, and
- an inline portion of HTML is returned. Note that we don't go through our `html` helper function here like before, but instead return just the a part that `script.js` will inject into the "main" div on our page with AJAX.

This almost concludes the cart feature.
One more thing before we finish this section: 
What should happen if user first adds some albums to his cart and later decides to log on?
