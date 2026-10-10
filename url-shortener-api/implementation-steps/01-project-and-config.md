# Step 1 — Project, configuration, and CORS

## Goal

The API starts, reads connection/JWT/CORS settings, and accepts browser calls from Vite.

## Context

- Project file: `URLShortener.csproj` (SDK web, `net10.0`)
- Entry: `Program.cs`
- Local URL: `http://localhost:5062` (`Properties/launchSettings.json`)
- Vue origin: `http://localhost:5173`

ASP.NET Core does not load `.env` files by itself. Prefer:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=URLShortener;Username=postgres;Password=secret"
dotnet user-secrets set "Jwt:Key" "a-long-random-string-at-least-32-chars"
```

Or put non-secret defaults in `appsettings.json` and secrets in `appsettings.Development.json` (already gitignored? **Do not commit passwords.** `appsettings.json` currently has a local connection string — move the password out).

## Add configuration keys

In `appsettings.json`:

```json
{
  "PublicBaseUrl": "http://localhost:5062",
  "Cors": { "Origin": "http://localhost:5173" },
  "Jwt": {
    "Issuer": "Shortpath",
    "Audience": "ShortpathWeb",
    "ExpiresMinutes": 10080
  }
}
```

Keep `Jwt:Key` and the database password in user-secrets or environment variables:

```
Jwt__Key=...
ConnectionStrings__DefaultConnection=...
```

## CORS

In `Program.cs`, before `MapControllers()`:

```csharp
var corsOrigin = builder.Configuration["Cors:Origin"] ?? "http://localhost:5173";

builder.Services.AddCors(options =>
{
    options.AddPolicy("Web", policy =>
        policy.WithOrigins(corsOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();
app.UseCors("Web");
```

Order matters: `UseCors` before endpoints.

## JSON

ASP.NET Core already serializes as camelCase. Keep that. If you add Newtonsoft, set camelCase there too — the Vue client sends `longUrl`, not `LongUrl`.

C# records can still use `LongUrl`; the JSON name will be `longUrl`.

## WorkOS

`Program.cs` currently boots WorkOS and maps `/auth/login` (redirect). The Vue app does **not** use those routes. Two options:

1. Comment WorkOS out until JWT works (recommended for learning).
2. Keep it, but do not let a missing API key crash `WebApplication.CreateBuilder`.

A missing `WorkOS:RedirectUri` currently throws at startup. Guard it if you park WorkOS.

## Check

```bash
dotnet run --project url-shortener-api
```

- Process listens on `http://localhost:5062`
- In the browser, `http://localhost:5062/scalar` opens (Development)
- From the Vue origin, an OPTIONS preflight to `/api/url/shorten` is not blocked (you can confirm in DevTools after step 3)

Next: [02-database.md](./02-database.md)
