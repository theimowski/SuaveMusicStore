# Views

We've seen how to define basic routing in a Suave application. 
In this section we'll see how we can deal with returning good looking HTML markup in a HTTP response.
Templating HTML views is quite a big topic itself, that we don't want to go into much details about.
Keep in mind that the concept can be approached in many different ways, and the way presented here is not the only proper way of rendering HTML views.
Having said that, I hope you'll still find the following implementation concise and easy to understand.
In this application we'll use server-side HTML templating with the help of a separate Suave package called `Suave.Experimental`.

> Note: As of the time of writing, `Suave.Experimental` is a separate package. It's likely that next releases of the package will include breaking changes. It's also possible that the modules we're going to use from within the package will be extracted to the core Suave package.

To use the package, we need to take a dependency on the following NuGet (add to paket.dependencies):

```
nuget Suave.Experimental 2.2.1
```

Also as was the case with `Suave` package, make sure to add `Suave.Experimental` to paket.references file and invoke paket install.

Before we start defining views, let's organize our `App.fs` source file by adding following line at the beginning of the file:

==> App.fs:1-3

The line means that whatever we define in the file will be placed in `SuaveMusicStore.App` module.
Read [here](http://fsharpforfunandprofit.com/posts/recipe-part3/) for more info about organizing and structuring F# code.
Now let's add a new file `View.fs` to the project. With Ionide you can simply create the file and then having it open in editor, trigger `F#: Add Current File To Project` command.

We need to move the newly created file just before the `App.fs` in the \*.fsproj project, e.g. by invoking **twice** following Ionide command: `F#: Move File Up`

Now let's place following module definition at the very top:

==> View.fs:1-3

We'll follow this convention throughout the tutorial to have a clear understanding of the project structure.

> Note: It's very important that the `View.fs` file comes before `App.fs` in `fsproj`. F# compiler requires the referenced items to be defined before their usage. At first glance, that might seem like a big drawback, however after a while you start realizing that you can have much better control of your dependencies. Read the [following](http://fsharpforfunandprofit.com/posts/cyclic-dependencies/) for further benefits of lack of cyclic dependencies in F# project.
