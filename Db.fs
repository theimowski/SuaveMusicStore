module SuaveMusicStore.Db

open FSharp.Data.Sql

type Sql = 
    SqlDataProvider< 
        ConnectionString      = "Server=192.168.99.100;Database=suavemusicstore;User Id=suave;Password=1234;",
        DatabaseVendor        = Common.DatabaseProviderTypes.POSTGRESQL,
        CaseSensitivityChange = Common.CaseSensitivityChange.ORIGINAL >

type DbContext = Sql.dataContext
type Album = DbContext.``public.albumsEntity``
type Genre = DbContext.``public.genresEntity``
type AlbumDetails = DbContext.``public.albumdetailsEntity``
type Artist = DbContext.``public.artistsEntity``

let getContext() = Sql.GetDataContext()

let getGenres (ctx : DbContext) : Genre list = 
    ctx.Public.Genres |> Seq.toList

let getAlbumsForGenre genreName (ctx : DbContext) : Album list = 
    query { 
        for album in ctx.Public.Albums do
            join genre in ctx.Public.Genres on (album.Genreid = genre.Genreid)
            where (genre.Name = genreName)
            select album
    }
    |> Seq.toList

let getAlbumDetails id (ctx : DbContext) : AlbumDetails option = 
    query { 
        for album in ctx.Public.Albumdetails do
            where (album.Albumid = id)
            select album
    } 
    |> Seq.tryHead

let getAlbumsDetails (ctx : DbContext) : AlbumDetails list = 
    ctx.Public.Albumdetails |> Seq.toList

let getAlbum id (ctx : DbContext) : Album option =
    query {
        for album in ctx.Public.Albums do
            where (album.Albumid = id)
            select album
    } |> Seq.tryHead

let deleteAlbum (album : Album) (ctx : DbContext) = 
    album.Delete()
    ctx.SubmitUpdates()

let getArtists (ctx : DbContext) : Artist list = 
    ctx.Public.Artists |> Seq.toList

let createAlbum (artistId, genreId, price, title) (ctx : DbContext) =
    ctx.Public.Albums.Create(artistId, genreId, price, title) |> ignore
    ctx.SubmitUpdates()

let updateAlbum (album : Album) (artistId, genreId, price, title) (ctx : DbContext) =
    album.Artistid <- artistId
    album.Genreid <- genreId
    album.Price <- price
    album.Title <- title
    ctx.SubmitUpdates()