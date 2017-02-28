# SQL Provider

There are many ways to talk with a database from .NET code including ADO.NET, light-weight libraries like Dapper, ORMs like Entity Framework or NHibernate.
To have more fun, we'll do something completely different, namely try an awesome F# feature called Type Providers.
In short, Type Providers allows to automatically generate a set of types based on some type of schema.
To learn more about Type Providers, check out [this resource](https://msdn.microsoft.com/en-us/library/hh156509.aspx).

SQLProvider is example of a Type Provider library, which gives ability to cooperate with a relational database.
Because we'll be using Postgres, we'll also need a .NET driver to Postgres - `Npgsql`.
We can install both SQLProvider and Npgsql with Paket:

```
nuget SQLProvider 1.0.43
nuget Npgsql 3.1.4
```

> Note: Once again, we pin versions so that this tutorial doesn't get outdated with newer versions of packages.

Don't forget to both packages to `paket.references` as well as running `paket install`!

--- TODO below

Having installed the SQLProvider, let's add `Db.fs` file to the beginning of our project - before any other `*.fs` file.

In the newly created file, open `FSharp.Data.Sql` module:

```fsharp
module SuaveMusicStore.Db

open FSharp.Data.Sql
```

Next, comes the most interesting part:

```fsharp
type Sql = 
    SqlDataProvider< 
        "Server=(LocalDb)\\v11.0;Database=SuaveMusicStore;Trusted_Connection=True;MultipleActiveResultSets=true", 
        DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER >
```

You'll need to adjust the above connection string, so that it can access the `SuaveMusicStore` database. At least you need to make sure that the server instance part is correct. If you're not sure how to configure it, [here](https://www.connectionstrings.com/sql-server/) is a great resource on dealing with connection strings in SQL Server.
After the SQLProvider can access the database, it will generate a set of types in background - each for single database table, as well as each for single database view.
This might be similar to how Entity Framework generates models for your tables, except there's no explicit code generation involved - all of the types reside under the `Sql` type defined.

The generated types have a bit cumbersome names, but we can define type aliases to keep things simpler:

```fsharp
type DbContext = Sql.dataContext
type Album = DbContext.``[dbo].[Albums]Entity``
type Genre = DbContext.``[dbo].[Genres]Entity``
type AlbumDetails = DbContext.``[dbo].[AlbumDetails]Entity``
```

`DbContext` is our data context.
`Album` and `Genre` reflect database tables.
`AlbumDetails` reflects database view - it will prove useful when we'll need to display names for the album's genre and artist.
