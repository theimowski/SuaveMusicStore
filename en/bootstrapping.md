# Bootstrapping

For development purposes, Suave application can be hosted as a standalone console app.
Let's start by spinning up an instance of Visual Studio Code.
If Ionide extension is installed correctly, you should be able to create new F# project from Command Pallete (Ctrl+Shift+P).

* Open Command Pallete, find `F#: New Project` and click Enter
* Choose `suave` from available templates
* Specify root directory for the project to be created
* Name the project `SuaveMusicStore`
* Wait for the project to be initialized - see note below

> Note: as of the time of writing, there's no easy way to restrict downloaded dependencies to target only .NET Framework and omit dependencies for dotnet core. That's why the process of initializing the project might take several minutes to complete. To track the progress you can view Output pane (`View` -> `Output`) and select `Forge` from dropdown. The process should end with `Done!` message entry.

When the project is initialized, we can restrict the dependencies to be resolved only for .NET fromework, which this tutorial is targetted at by adding line to the very top of `paket.dependencies` file:

```
framework: net46
```

and invoking `paket install`:

```
> .\.paket\paket.exe install
```

As we'll be using just a single project, I prefer to keep all my files, including \*.fsproj and \*.fs in root directory.
Ionide command creates a directory for the project, so we'll need to move contents of that directory one level above:

```
> mv SuaveMusicStore/* .
```

Upon creation of new project, we'll have already [Paket](http://fsprojects.github.io/Paket/) and [FAKE](http://fsharp.github.io/FAKE/) scripts integration for us to use.
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