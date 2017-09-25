# Bootstrapping

Let's start by spinning up an instance of Visual Studio Code.
If Ionide extension is installed correctly, you should be able to create new F# project from Command Pallete (Ctrl+Shift+P).

* Open Command Pallete and select `F#: Refresh Project Templates` to use the latest templates
* Next, in Command Pallete trigger `F#: New Project`
* Choose `suave` from available templates
* Specify root directory for the project to be created
* Name the project `SuaveMusicStore`
* Wait for the project to be initialized - see note below

> Note: To track the progress you can view Output pane (`View` -> `Output`) and select `Forge` from dropdown. The process should end with `Done!` message entry.

As we'll be using just a single project, I prefer to keep all my files, including \*.fsproj and \*.fs in root directory.
Ionide command creates a directory for the project, so we'll need to move contents of that directory one level above:

```
> mv SuaveMusicStore/* .
```

As well as correct the relative path in `SuaveMusicStore.fsproj`:

```
    <Import Project=".paket\Paket.Restore.targets" />
```

Optionally remove the remaining empty directory:

```
> rmdir SuaveMusicStore
```

It's important to restrict `paket.dependencies` to the same .NET version that is specified in `.fsproj` file. Make sure `TargetFramework` node in `.fsproj` contains same .NET framework version as the one specified in `paket.dependencies`.

```
    <TargetFramework>net461</TargetFramework>
```

```
framework: >= net461
```

Next, let's pin versions of all dependencies, and apply binding redirects:

```
nuget FAKE 4.63.2
nuget FSharp.Core 4.2.3 redirects:force
nuget Suave 2.2.1
```

> Note: we'll deliberately pin all dependency versions so that this tutorial doesn't get out of date when newer versions of Suave or other dependencies arrive.
The [binding redirects](https://fsprojects.github.io/Paket/dependencies-file.html#Controlling-assembly-binding-redirects) for FSharp.Core is a [good practice when creating apps](https://fsharp.github.io/2015/04/18/fsharp-core-notes.html#use-binding-redirects-for-applications), and should prevent unwanted version compatiblity issues.

Invoke `paket install` to apply the changes:

```
> .\.paket\paket.exe install --create-new-binding-files
```

The flag at end will make sure to create `app.config` with bindings for you.

Upon creation of new project, we'll have already [Paket](http://fsprojects.github.io/Paket/) and [FAKE](http://fsharp.github.io/FAKE/) scripts integration for us to use.
This means that we should be already able to build the project in command line:

Windows

```
> .\build.cmd
```

Mac / Linux

```
$ ./build.sh
```

> Note: If you don't already have Dotnet SDK 2.0 installed, build command will do that for you - it can take a while.

Rename `SuaveMusicStore.fs` file to `App.fs` to make things simpler. 
Using Ionide and Visual Studio Code, you can do this with `F# Project Explorer`: 

* Trigger VS Code `Reload Window` from Command Prompt so that Ionide can refresh the .fsproj file move
* View side bar
* Navigate to `F# Project Explorer`
* Right click the file
* Select `Rename file`

Below link allows you to explore initial Git commit with generated project.