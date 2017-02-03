# Hello World from Suave

Now we can add NuGet reference to Suave. To do that, add Suave package to paket.dependencies: 

```
nuget Suave 2.0.1
```

> Note: we pin Suave version to the latest (as of the time of writing) version of package so that this tutorial doesn't get out of date when newer versions of Suave arrive.

Also, add Suave package to your paket.references:

```
Suave
```

Rename the `SuaveMusicStore.fs` file to `App.fs` to better reflect the purpose of the file.
With Forge we can rename the file using command:

```
> forge rename file --name .\SuaveMusicStore.fs `
>>                  --rename .\App.fs `
>>                  --project .\SuaveMusicStore.fsproj
```

> Note: make sure you specify current directory (``.``) when passing to Forge files from current directory.

Replace contents of App.fs completely with following code:

==> App.fs

Guess what - you already have a working web app!
Just invoke the build script and run the executable again and you should see:

```
[07:42:13 INF] Smooth! Suave listener started in 24.899 with binding 127.0.0.1:8083
```

If you browse that url, you should be greeted with the classic `Hello World!`.
The `open` statements at the top of the file are the same as `using` statements in C#.
Note there is no `Main` method defined in `App.fs` - what happens here is that the `startWebServer` function is invoked immediately after the program is run and Suave starts listening for incoming request till the process is killed.