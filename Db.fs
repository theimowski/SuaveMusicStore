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