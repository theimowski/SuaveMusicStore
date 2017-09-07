# CSS

It's high time we added some CSS styles to our HTML markup.
We'll not deep-dive into the details about the styles itself, as this is not a tutorial on Web Design.
The stylesheet can be downloaded [from here](https://raw.githubusercontent.com/theimowski/SuaveMusicStore/v{{book.version}}_src/Site.css) in its final shape.
Place the `Site.css` stylesheet in the root directory of the project, and manually add following to the `SuaveMusicStore.fsproj`:

```xml
<Content Include="Site.css">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</Content>
```

> Note: Specifying `CopyToOutputDirectory` element with `PreserveNewest` value makes sure the stylesheet is accessible after the program has been compiled.

In order to include the stylesheet in our HTML markup, let's add the following to our `View`:

==> View.fs:5-12

This enables us to output the link HTML element with `href` attribute pointing to the CSS stylesheet.

The CSS depends on `logo.png` asset, which can be downloaded from [here](https://raw.githubusercontent.com/theimowski/SuaveMusicStore/v{{book.version}}_src/logo.png).

Again, place the `logo.png` in root directory, and add an entry to fsproj:

```xml
<Content Include="logo.png">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</Content>
```

A browser, when asked to include a CSS file, sends back a request to the server with the given url.
In similar fashion, when the browser wants to render an image asset, it needs to GET it from the server.

If we have a look at our main `WebPart` we'll notice that there's really no handler capable of serving this file.
That's why we need to add another alternative to our `choose` `WebPart`:

==> App.fs:21-21

The `pathRegex` `WebPart` returns `Some` if an incoming request concerns path that matches the regular expression pattern. 
If that's the case, the `Files.browseHome` WebPart will be applied.
The given pattern matches every file with either `.css` or `.png` extension, which protects us from accessing other (e.g. binary or config) files.
`Files.browseHome` is a `WebPart` from Suave that serves static files from the root application directory.

Now you should be able to see the styles applied to our HTML markup.
