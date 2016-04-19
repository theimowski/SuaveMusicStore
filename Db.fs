module SuaveMusicStore.Db

open System
open FSharp.Data.Sql

type Sql = 
    SqlDataProvider< 
        Common.DatabaseProviderTypes.MSSQLSERVER, 
        "Server=(LocalDb)\\MSSQLLocalDB;Database=SuaveMusicStore;Trusted_Connection=True;MultipleActiveResultSets=true" 
        >

type DbContext = Sql.dataContext
type Album = DbContext.``dbo.AlbumsEntity``
type Artist = DbContext.``dbo.ArtistsEntity``
type Genre = DbContext.``dbo.GenresEntity``
type AlbumDetails = DbContext.``dbo.AlbumDetailsEntity``
type User = DbContext.``dbo.UsersEntity``
type Cart = DbContext.``dbo.CartsEntity``
type CartDetails = DbContext.``dbo.CartDetailsEntity``
type BestSeller = DbContext.``dbo.BestSellersEntity``

let getContext() = Sql.GetDataContext()

let firstOrNone s = s |> Seq.tryFind (fun _ -> true)

let getGenres (ctx : DbContext) : Genre list = 
    ctx.Dbo.Genres |> Seq.toList

let getArtists (ctx : DbContext) : Artist list = 
    ctx.Dbo.Artists |> Seq.toList

let getAlbumsForGenre genreName (ctx : DbContext) : Album list = 
    query { 
        for album in ctx.Dbo.Albums do
            join genre in ctx.Dbo.Genres on (album.GenreId = genre.GenreId)
            where (genre.Name = genreName)
            select album
    }
    |> Seq.toList

let getAlbumDetails id (ctx : DbContext) : AlbumDetails option = 
    query { 
        for album in ctx.Dbo.AlbumDetails do
            where (album.AlbumId = id)
            select album
    } |> firstOrNone

let getAlbumsDetails (ctx : DbContext) : AlbumDetails list = 
    ctx.Dbo.AlbumDetails |> Seq.toList

let getBestSellers (ctx : DbContext) : BestSeller list  =
    ctx.Dbo.BestSellers |> Seq.toList

let getAlbum id (ctx : DbContext) : Album option = 
    query { 
        for album in ctx.Dbo.Albums do
            where (album.AlbumId = id)
            select album
    } |> firstOrNone

let validateUser (username, password) (ctx : DbContext) : User option =
    query {
        for user in ctx.Dbo.Users do
            where (user.UserName = username && user.Password = password)
            select user
    } |> firstOrNone

let getUser username (ctx : DbContext) : User option = 
    query {
        for user in ctx.Dbo.Users do
        where (user.UserName = username)
        select user
    } |> firstOrNone

let getCart cartId albumId (ctx : DbContext) : Cart option =
    query {
        for cart in ctx.Dbo.Carts do
            where (cart.CartId = cartId && cart.AlbumId = albumId)
            select cart
    } |> firstOrNone

let getCarts cartId (ctx : DbContext) : Cart list =
    query {
        for cart in ctx.Dbo.Carts do
            where (cart.CartId = cartId)
            select cart
    } |> Seq.toList

let getCartsDetails cartId (ctx : DbContext) : CartDetails list =
    query {
        for cart in ctx.Dbo.CartDetails do
            where (cart.CartId = cartId)
            select cart
    } |> Seq.toList

let createAlbum (artistId, genreId, price, title) (ctx : DbContext) =
    ctx.Dbo.Albums.Create(artistId, genreId, price, title) |> ignore
    ctx.SubmitUpdates()

let updateAlbum (album : Album) (artistId, genreId, price, title) (ctx : DbContext) =
    album.ArtistId <- artistId
    album.GenreId <- genreId
    album.Price <- price
    album.Title <- title
    ctx.SubmitUpdates()

let deleteAlbum (album : Album) (ctx : DbContext) = 
    album.Delete()
    ctx.SubmitUpdates()

let addToCart cartId albumId (ctx : DbContext)  =
    match getCart cartId albumId ctx with
    | Some cart ->
        cart.Count <- cart.Count + 1
    | None ->
        ctx.Dbo.Carts.Create(albumId, cartId, 1, DateTime.UtcNow) |> ignore
    ctx.SubmitUpdates()

let removeFromCart (cart : Cart) albumId (ctx : DbContext) = 
    cart.Count <- cart.Count - 1
    if cart.Count = 0 then cart.Delete()
    ctx.SubmitUpdates()

let upgradeCarts (cartId : string, username :string) (ctx : DbContext) =
    for cart in getCarts cartId ctx do
        match getCart username cart.AlbumId ctx with
        | Some existing ->
            existing.Count <- existing.Count +  cart.Count
            cart.Delete()
        | None ->
            cart.CartId <- username
    ctx.SubmitUpdates()

let newUser (username, password, email) (ctx : DbContext) =
    let user = ctx.Dbo.Users.Create(email, password, "user", username)
    ctx.SubmitUpdates()
    user

let placeOrder (username : string) (ctx : DbContext) =
    let carts = getCartsDetails username ctx
    let total = carts |> List.sumBy (fun c -> (decimal) c.Count * c.Price)
    let order = ctx.Dbo.Orders.Create(DateTime.UtcNow, total)
    order.Username <- username
    ctx.SubmitUpdates()
    for cart in carts do
        let orderDetails = ctx.Dbo.OrderDetails.Create(cart.AlbumId, order.OrderId, cart.Count, cart.Price)
        getCart cart.CartId cart.AlbumId ctx
        |> Option.iter (fun cart -> cart.Delete())
    ctx.SubmitUpdates()