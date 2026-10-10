# Step 6 — JWT register / login / me / logout

## Goal

Pinia `auth` store can create a session. Vue calls:

| Method | Path | Body |
| --- | --- | --- |
| POST | `/api/auth/register` | `{ username, email, password }` |
| POST | `/api/auth/login` | `{ email, password }` |
| POST | `/api/auth/logout` | none |
| GET | `/api/auth/me` | none, Bearer token |

## Packages

```bash
cd url-shortener-api
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

## Wire authentication in `Program.cs`

```csharp
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});
builder.Services.AddAuthorization();

app.UseAuthentication();
app.UseAuthorization();
```

Place `UseAuthentication` / `UseAuthorization` after CORS and before mapping endpoints.

## Password hashing

Use `PasswordHasher<Users>` (you already imported Identity hashing in `Controllers/auth.cs`). Persist `PasswordHash` on the `Users` row. Never return the hash to the client.

## Token contents

Claims at least:

- `sub` or `ClaimTypes.NameIdentifier` = user id (Guid string)
- `email`
- `unique_name` / username

Sign with HMAC-SHA256. Expiry from `Jwt:ExpiresMinutes`.

Shared response:

```json
{
  "token": "<jwt>",
  "user": { "id": "<guid>", "username": "ada", "email": "ada@example.com" }
}
```

## Controller

Replace the static in-memory `User` in `Controllers/auth.cs`.

- Route must stay `[Route("api/[controller]")]` on class `Auth` → `/api/auth/...`
- `Register`: 409 if email or username taken
- `Login`: look up by email, verify hash, 401 on failure
- `Me`: `[Authorize]`, read id from claims, load user, return public profile
- `Logout`: `[Authorize]` optional, return 204

## Check

```bash
curl -s -X POST http://localhost:5062/api/auth/register \
  -H 'Content-Type: application/json' \
  -d '{"username":"ada","email":"ada@example.com","password":"password12"}'

curl -s -X POST http://localhost:5062/api/auth/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"ada@example.com","password":"password12"}'

curl -s http://localhost:5062/api/auth/me \
  -H 'Authorization: Bearer YOUR_TOKEN'
```

In the UI: Sign up → header shows username → refresh page still logged in (`hydrate` + `GET /me`) → Log out.

Next: [07-tie-urls-to-users.md](./07-tie-urls-to-users.md)
