Pgwire
==========================================================================

SQL Server, behind PostgreSQL wire protocol.


Usage
--------------------------------------------------------------------------

```bash
cd src\Pgwire
dotnet run --sqlserver="Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;"
```


Types
--------------------------------------------------------------------------

| .NET Type   | PostgreSQL  | DbType      |
|-------------|-------------|-------------|
| Boolean     | bit         | Boolean     |
| Boolean     | bool        | Boolean     |
| Int16       | int2        | Int16       |
| Int32       | int4        | Int32       |
| Int64       | int8        | Int64       |
| Byte[]      | bytea       | Binary      |
| Single      | float4      | Single      |
| Double      | float8      | Double      |
| Decimal     | money       | Decimal     |
| Decimal     | numeric     | Decimal     |
| DateTime    | date        | Date        |
| DateTime    | time        | Time        |
| DateTime    | timetz      | Time        |
| DateTime    | timestamp   | DateTime    |
| DateTime    | timestamptz | DateTime    |
| TimeSpan    | interval    | Object      |
| String      | text        | String      |
| String      | varchar     | String      |
| Guid        | uuid        | Guid        |
| IPAddress   | inet        | Object      |
| Array       | array       | Object      |


References
--------------------------------------------------------------------------

* https://www.postgresql.org/docs/current/protocol.html
* https://gavinray97.github.io/blog/postgres-wire-protocol-jdk-21
