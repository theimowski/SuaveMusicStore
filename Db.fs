module SuaveMusicStore.Db

open FSharp.Data.Sql

[<Literal>]
let ConnectionString = 
    "Server=192.168.99.100;"    + 
    "Database=suavemusicstore;" + 
    "User Id=suave;"            + 
    "Password=1234;"

type Sql = 
    SqlDataProvider< 
        ConnectionString      = ConnectionString,
        DatabaseVendor        = Common.DatabaseProviderTypes.POSTGRESQL,
        CaseSensitivityChange = Common.CaseSensitivityChange.ORIGINAL >

type DbContext = Sql.dataContext
type Album = DbContext.``public.albumsEntity``
type Genre = DbContext.``public.genresEntity``
type AlbumDetails = DbContext.``public.albumdetailsEntity``
type Artist = DbContext.``public.artistsEntity``
type User = DbContext.``public.usersEntity``
type CartDetails = DbContext.``public.cartdetailsEntity``
type Cart = DbContext.``public.cartsEntity``

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
    ctx.Public.Albumdetails
    |> Seq.toList
    |> List.sortBy (fun a -> a.Artist)

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

let validateUser (username, password) (ctx : DbContext) : User option =
    query {
        for user in ctx.Public.Users do
            where (user.Username = username && user.Password = password)
            select user
    } |> Seq.tryHead

let getCart cartId albumId (ctx : DbContext) : Cart option =
    query {
        for cart in ctx.Public.Carts do
            where (cart.Cartid = cartId && cart.Albumid = albumId)
            select cart
    } |> Seq.tryHead

let addToCart cartId albumId (ctx : DbContext)  =
    match getCart cartId albumId ctx with
    | Some cart ->
        cart.Count <- cart.Count + 1
    | None ->
        ctx.Public.Carts.Create(albumId, cartId, 1, System.DateTime.UtcNow) |> ignore
    ctx.SubmitUpdates()
