# Step 2 — PostgreSQL and Entity Framework

## Goal

Tables match what the frontend and later auth steps need: users, urls, clicks.

## Install / confirm packages

Already referenced:

- `Microsoft.EntityFrameworkCore`
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- Design + Tools packages

CLI (once per machine):

```bash
dotnet tool install --global dotnet-ef
```

## Model changes to make

`database.cs` is the current `AppDbContext`. Improve it:

1. **`Users`**
   - `Guid Id`
   - `string Username` unique
   - `string Email` unique
   - `string PasswordHash`
   - Index email and username

2. **`Urls`**
   - Keep `Id`, `OriginalUrl`, `ShortCode` (unique), `ClickCount`
   - Change `CreatedAt` from `string` to `DateTimeOffset` (or `DateTime` UTC)
   - Change `UserId` to `Guid?` so guest links are allowed
   - Optional navigation `Users? User`

3. **`Clicks`**
   - Keep FK to `Urls` with cascade delete
   - Prefer `DateTimeOffset Timestamp`
   - Rename `referrer` → `Referrer` in C# (JSON will be `referrer`)
   - Store empty referrer as `""` or `"direct"` consistently

4. Register `DbSet<Users> Users` (already there)

## Migration

After model edits:

```bash
cd url-shortener-api
dotnet ef migrations add AlignSchemaWithFrontend
dotnet ef database update
```

If the existing `InitialCreate` database is throwaway, you can drop it and update again. If you already have data, write the migration carefully (`UserId` nullable, timestamp type change).

PostgreSQL must be running. Connection string key: `ConnectionStrings:DefaultConnection`.

## Check

```sql
-- psql
\dt
\d "Urls"
\d "Users"
\d "Clicks"
```

`Urls.ShortCode` unique, `Clicks.UrlId` FK, `Users.Email` unique.

Next: [03-shorten-endpoint.md](./03-shorten-endpoint.md)
