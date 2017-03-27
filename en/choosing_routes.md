# Choosing routes

Let's move on to configuring a few routes in our application. 
Before we do that, we'll need to open up two more modules at the beginning of `App.fs`:

==> App.fs:1-4

Now, let's create a couple of routing rules, so that our application responds with different content for different paths.
To achieve that, we can use the `choose` function, which takes a list of WebParts, and chooses the first one that applies (returns `Some`), or if none WebPart applies, then choose will also return `None`:

==> App.fs:`let webPart`

Hover over `path` symbol in above snippet to inspect this function's type.
What it means is that if we give it a string, it will return a WebPart.
Under the hood, the function looks at the incoming request and returns `Some` if the path matches, and `None` otherwise.
The `>=>` operator comes also from Suave library. It composes two WebParts into one by first evaluating the WebPart on the left, and applying the WebPart on the right only if the first one returned `Some`.

After applying the change try out all paths from the snippet by appending them to `localhost:8083`.