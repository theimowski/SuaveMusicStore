# Bootstrapping

For development purposes, Suave application can be hosted as a standalone console app.
Let's start by spinning up an instance of Visual Studio Code.
If Ionide extension is installed correctly, you should be able to create new F# project from Command Pallete (Ctrl+Shift+P).

* Open Command Pallete, find `F#: New Project` and click Enter
* Choose `console` from available templates (we deliberately skip the `suave` template for now to prevent installing unnecessary packages as we'll see soon)
* Specify root directory for the project to be created
* Name the project `SuaveMusicStore`
* Wait for the project to be initialized - see note below

> Note: as of the time of writing, there's no easy way to restrict downloaded dependencies to target only .NET Framework and omit dependencies for dotnet core. That's why the process of initializing the project might take several minutes to complete. To track the progress you can view Output pane (`View` -> `Output`) and select `Forge` from dropdown. The process should end with `Done!` message entry.

Rename `SuaveMusicStore.fs` file to `App.fs` to make things simpler. 
Using Ionide and Visual Studio Code, you can do this with `F# Project Explorer`: 

* View side bar
* Navigate to `F# Project Explorer`
* Right click the file
* Select `Rename file`

As we'll be using just a single project, I prefer to keep all my files, including \*.fsproj and \*.fs in root directory.
Ionide command creates a directory for the project, so we'll need to move contents of that directory one level above:

```
> mv SuaveMusicStore/* .
```

> Note: If after moving the contents, Ionide lists duplicate projects in `F# Project Explorer`, invoke VS Code `Reload Window` command to refresh the workspace

Optionally remove the remaining empty directory:

```
> rmdir SuaveMusicStore
```

When the project is initialized, we can restrict the dependencies to be resolved only for .NET fromework (which this tutorial is targetted at) by adding line to the very top of `paket.dependencies` file:

```
framework: net461
```

> Note: It's important to restrict to the same .NET version that is specified in `.fsproj` file. Make sure `TargetFrameworkVersion` node in `.fsproj` contains same .NET framework version as the one to specify in `paket.dependencies`.

Now we can add NuGet reference to Suave. To do that, add Suave package to paket.dependencies:

```
nuget Suave 2.0.1
```

> Note: we pin Suave version to the latest (as of the time of writing) version of package so that this tutorial doesn't get out of date when newer versions of Suave arrive.

Next, let's also pin the version of FSharp.Core, and apply binding redirects:

```
nuget FSharp.Core 4.0.0.1 redirects:force
```

Also, add Suave package to paket.references file:

```
Suave
```

Finally invoke `paket install` to fetch the dependencies:

```
> .\.paket\paket.exe install
```

Below link allows you to explore initial Git commit with generated project.