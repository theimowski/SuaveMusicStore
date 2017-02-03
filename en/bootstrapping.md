# Bootstrapping

For development purposes, Suave application can be hosted as a standalone console app. 
Let's start by creating a console app project named `SuaveMusicStore`.
With help of Forge, we can do this first by creating a new directory for the project and then  invoking following command in the newly created directory:

```
$ forge new project --name SuaveMusicStore --template console --folder .
```

Depending on tool of your choice, creating F# console project may vary.

As we'll be using just a single project, I prefer to keep all my files, including \*.fsproj and \*.fs in root directory.
Above Forge command creates a directory for the project, so we'll need to move contents of that directory one level above:

```
$ mv SuaveMusicStore/* .
```

Upon creation of new project, Forge adds by default Paket and FAKE scripts for us to use.
This means that we should be already able to build and run in command line:

Windows

```
> .\build.cmd; .\build\SuaveMusicStore.exe "Hello world"
```

Mac / Linux

```
$ ./build.sh && mono ./build/SuaveMusicStore.exe "Hello world"
```

The genereted boilerplate prints out all arguments passed to the executable as an array, so if everything works fine we should see following output:

```
[|"Hello world"|]
```

Below link allows you to explore initial Git commit with generated project.