# Hello World from Suave


Upon creation of new project, we'll have already [Paket](http://fsprojects.github.io/Paket/) and [FAKE](http://fsharp.github.io/FAKE/) scripts integration for us to use.
This means that we should be already able to build and run in command line:

Windows

```
> .\build.cmd; .\build\SuaveMusicStore.exe
```

Mac / Linux

```
$ ./build.sh && mono ./build/SuaveMusicStore.exe
```

The genereted boilerplate spins up a web server with default Suave configuration, so if everything works fine we should see following output:

```
[06:19:25 INF] Smooth! Suave listener started in 91.979 with binding 127.0.0.1:8080
```

and all HTTP requests againsts that URL should end with following response:

```
HTTP/1.1 200 OK
Server: Suave (https://suave.io)
Date: Tue, 22 Aug 2017 04:22:34 GMT
Content-Type: text/html
Content-Length: 12

Hello World!
```

Guess what - you already have a working web app!
Just invoke the build script and run the executable again and you should see:

```
[07:42:13 INF] Smooth! Suave listener started in 24.899 with binding 127.0.0.1:8083
```

If you browse that url, you should be greeted with the classic `Hello World!`.
The `open` statements at the top of the file are the same as `using` statements in C#.
Note there is no `Main` method defined in `App.fs` - what happens here is that the `startWebServer` function is invoked immediately after the program is run and Suave starts listening for incoming request till the process is killed.