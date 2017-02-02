# Hello World from Suave

Now we can add NuGet reference to Suave. To do that, add Suave package to paket.dependencies: 

```
nuget Suave 2.0.1
```

Note that we pin Suave version to the latest (as of the time of writing) version of package so that this tutorial doesn't get out of date.

Also, add Suave package to your paket.references:

```
Suave
```

Rename the `SuaveMusicStore.fs` file to `App.fs` to better reflect the purpose of the file.
With Forge we can rename the file using command:

```
> forge rename file --name .\SuaveMusicStore.fs --rename .\App.fs --project .\SuaveMusicStore.fsproj
```

Note: make sure you specify current directory (``.``) when giving files to Forge

Replace its contents completely with the following code:

==> App.fs

Guess what, if you press F5 to run the project, your application is now up and running!
By default it should be available under `http://localhost:8083`.
If you browse that url, you should be greeted with the classic `Hello World!`.
The `open` statements at the top of the file are the same as `using` statements in C#.
Note there is no `Main` method defined in `App.fs` - what happens here is that the `startWebServer` function is invoked immediately after the program is run and Suave starts listening for incoming request till the process is killed.