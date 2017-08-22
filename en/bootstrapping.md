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

As we'll be using just a single project, I prefer to keep all my files, including \*.fsproj and \*.fs in root directory.
Ionide command creates a directory for the project, so we'll need to move contents of that directory one level above:

```
> mv SuaveMusicStore/* .
```

Optionally remove the remaining empty directory:

```
> rmdir SuaveMusicStore
```

When the project is initialized, we can restrict the dependencies to be resolved only for .NET fromework, which this tutorial is targetted at by adding line to the very top of `paket.dependencies` file:

```
framework: net461
```

> Note: It's important to restrict to the same .NET version that is specified in `.fsproj` file. Make sure `TargetFrameworkVersion` node in `.fsproj` contains same .NET framework version as the one to specify in `paket.dependencies`.

Now we can pin Suave package version in paket.dependencies: 

```
nuget Suave 2.0.1
```

> Note: we pin Suave version to the latest (as of the time of writing) version of package so that this tutorial doesn't get out of date when newer versions of Suave arrive.

Invoke `paket install`:

```
> .\.paket\paket.exe install
```

Rename the `SuaveMusicStore.fs` file to `App.fs` to better reflect the purpose of the file.
> TODO : Use project explorer
With Forge we can rename the file using command:

```
> forge rename file --name .\SuaveMusicStore.fs `
>>                  --rename .\App.fs `
>>                  --project .\SuaveMusicStore.fsproj
```

Make sure you specify current directory (``.``) when passing to Forge files from current directory.

> Note: We can't just rename the *.fs file only, because the *.fsproj project file keeps track of sources to compile. That's why we need to make sure those are in sync by e.g. running Forge command.


Below link allows you to explore initial Git commit with generated project.