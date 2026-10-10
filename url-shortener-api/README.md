# Shortpath API

ASP.NET backend for the Vue app in `url-shortener-web`. The frontend is already calling the endpoints below. This folder currently has a partial implementation (shorten, redirect-ish GET, stats, delete, incomplete auth). Use this README as the contract to finish against, then follow [`implementation-steps/`](./implementation-steps) in order.

## What the frontend needs

- JSON API over HTTP
- CORS for `http://localhost:5173`
- JWT bearer auth for account routes
- Public shorten + public `302` redirect
- Click logging and aggregated stats

Frontend env (already documented in the web README):

```
VITE_API_BASE_URL=http://localhost:5062
VITE_PUBLIC_SHORT_BASE_URL=http://localhost:5062
```

Run the API on `http://localhost:5062` (see `Properties/launchSettings.json`).

## Configuration the API should expose

Use `appsettings.Development.json`, environment variables, or `dotnet user-secrets`. Names below match typical ASP.NET env-var syntax (`Section__Key`).

| Key | Example | Used for |
| --- | --- | --- |
| `ConnectionStrings:DefaultConnection` | `Host=localhost;Port=5432;Database=URLShortener;Username=postgres;Password=...` | EF Core + PostgreSQL |
| `Jwt:Issuer` | `Shortpath` | Token issuer |
| `Jwt:Audience` | `ShortpathWeb` | Token audience |
| `Jwt:Key` | 32+ random characters | HMAC signing key |
| `Jwt:ExpiresMinutes` | `10080` | Access token lifetime (7 days is fine for learning) |
| `PublicBaseUrl` | `http://localhost:5062` | Building `shortUrl` in responses |
| `Cors:Origin` | `http://localhost:5173` | Allowed browser origin |

See `.env.example` in this folder. If you load a `.env` file, map those keys into configuration; ASP.NET does not read `.env` by default.

Do not commit real passwords, JWT keys, or third-party API keys.

## JSON conventions

- camelCase property names (`longUrl`, not `LongUrl` in JSON)
- `Content-Type: application/json`
- Errors: `{ "message": "human readable text" }` (ProblemDetails `title` is also accepted by the client)
- Dates: ISO-8601 UTC strings, e.g. `2026-10-10T11:00:00Z`

## Auth header

```
Authorization: Bearer <jwt>
```

The Vue Axios client attaches this automatically from `localStorage`.

Guest shorten is allowed. `GET /api/url/mine`, delete of another user's link, and authenticated extras require a valid token.

---

## Endpoints

Base URL: `http://localhost:5062`

### Health / smoke

`GET /` can stay as a simple 200. Not used by the UI.

---

### `POST /api/url/shorten`

Create a short link. Auth optional. If a JWT is present, set `userId` on the row.

**Request**

```json
{
  "longUrl": "https://example.com/articles/very-long-slug",
  "customCode": "launch"
}
```

| Field | Required | Rules |
| --- | --- | --- |
| `longUrl` | yes | Absolute `http` or `https` URI |
| `customCode` | no | 3–32 chars, `[a-zA-Z0-9_-]`, unique |

If `customCode` is omitted, generate a Base62 code (you already have `Utilities.Encode`).

**201 Created** (200 is acceptable)

```json
{
  "id": 12,
  "originalUrl": "https://example.com/articles/very-long-slug",
  "shortCode": "launch",
  "shortUrl": "http://localhost:5062/launch",
  "createdAt": "2026-10-10T11:00:00Z",
  "clickCount": 0
}
```

**400** invalid URL or custom code  
**409** custom code already taken

---

### `GET /{shortCode}`

Public redirect. This is what people open in a browser. **Not** under `/api`.

1. Look up `Urls` by `shortCode`
2. Insert a `Clicks` row (IP, user-agent, referrer, language)
3. Increment `clickCount`
4. Return `302` with `Location: originalUrl`

**404** unknown code

Do not return JSON here. The current controller returns JSON from `GET /api/url/{shortCode}`; keep that as a metadata endpoint and add a separate redirect map in `Program.cs`.

---

### `GET /api/url/{shortCode}`

JSON metadata for one link. Used if you later need it; the UI mainly uses list + stats.

**200** same object as shorten response  
**404** not found

---

### `GET /api/url/mine`

**Auth required.**

**200** array of short-link objects for the current user, newest first.

```json
[
  {
    "id": 12,
    "originalUrl": "https://example.com/articles/very-long-slug",
    "shortCode": "launch",
    "shortUrl": "http://localhost:5062/launch",
    "createdAt": "2026-10-10T11:00:00Z",
    "clickCount": 41
  }
]
```

**401** missing/invalid token

---

### `GET /api/url/stats/{shortCode}`

Auth optional for learning; prefer requiring the owner when auth is done.

**200**

```json
{
  "shortCode": "launch",
  "originalUrl": "https://example.com/articles/very-long-slug",
  "clickCount": 41,
  "clicks": [
    {
      "id": 90,
      "timestamp": "2026-10-10T12:01:00Z",
      "referrer": "https://news.example",
      "userAgent": "Mozilla/5.0 ...",
      "ipAddress": "127.0.0.1",
      "language": "en-US"
    }
  ],
  "byDay": [
    { "label": "2026-10-08", "count": 10 },
    { "label": "2026-10-09", "count": 12 },
    { "label": "2026-10-10", "count": 19 }
  ],
  "byReferrer": [
    { "label": "https://news.example", "count": 20 },
    { "label": "direct", "count": 21 }
  ]
}
```

Empty referrer should become `"direct"`. Limit `clicks` to the latest 100. Aggregate `byDay` for the last 14 days.

**404** unknown code  
**403** if you enforce ownership and the caller is not the owner

---

### `DELETE /api/url/{shortCode}`

Delete the link and its clicks (cascade).

**204 No Content** (200 with a message is fine)  
**404** not found  
**401/403** when auth ownership is enabled

---

### `POST /api/auth/register`

**Request**

```json
{
  "username": "ada",
  "email": "ada@example.com",
  "password": "correct-horse-battery"
}
```

**201**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "username": "ada",
    "email": "ada@example.com"
  }
}
```

**400** validation  
**409** email or username already used

Hash passwords with ASP.NET Identity `PasswordHasher<T>`. Never store plaintext.

---

### `POST /api/auth/login`

**Request**

```json
{
  "email": "ada@example.com",
  "password": "correct-horse-battery"
}
```

**200** same shape as register (`token` + `user`)  
**401** unknown email or bad password

---

### `POST /api/auth/logout`

Stateless JWT: this can be a no-op `204`. The frontend still calls it, then clears local storage.

If you later add refresh-token denylists, invalidate here.

**204**

---

### `GET /api/auth/me`

**Auth required.**

**200**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "username": "ada",
  "email": "ada@example.com"
}
```

**401** otherwise

---

## CORS

Allow the Vite origin:

- Origin: `http://localhost:5173`
- Methods: `GET, POST, DELETE, OPTIONS`
- Headers: `Content-Type, Authorization`

Without this, the browser blocks every call.

## Suggested tables

You already have `Urls`, `Clicks`, and `Users`. Align them with:

**Users**

- `Id` uniqueidentifier / uuid PK
- `Username` unique
- `Email` unique
- `PasswordHash`

**Urls**

- `Id` int identity
- `OriginalUrl`
- `ShortCode` unique
- `CreatedAt` timestamptz (prefer a real timestamp over string)
- `ClickCount`
- `UserId` uuid nullable (null = guest)

**Clicks**

- `Id` int identity
- `UrlId` FK cascade
- `Timestamp` timestamptz
- `IpAddress`
- `UserAgent`
- `Referrer`
- `Language`

## Current code vs this contract

| Existing | Gap |
| --- | --- |
| `POST /api/url/shorten` | Returns only `{ shortUrl }`. Return the full object. Honor `customCode`. |
| `GET /api/url/{shortCode}` | Increments clicks and returns JSON. Split: redirect at `GET /{code}`, metadata at this route. |
| `GET /api/url/stats/{code}` | Returns raw `Clicks[]`. Add `byDay` / `byReferrer` / `clickCount`. |
| `DELETE /api/url/{code}` | Mostly done. |
| `POST /api/auth/register` | In-memory single user, no email, no JWT. Persist to `Users` and return token. |
| WorkOS routes in `Program.cs` | Not used by this Vue app. Park them until JWT works. |
| CORS | Not configured. |
| JWT middleware | Missing. |

## How to verify with the frontend

1. `dotnet run --project url-shortener-api` (port 5062)
2. `pnpm --dir url-shortener-web dev`
3. Shorten `https://example.com` on the home page
4. Open the printed short URL — it should 302
5. Click the link in the sidebar — stats donut should fill
6. Register, log out, log in — header shows the username
7. `GET /api/url/mine` should list only that user's links

OpenAPI/Scalar is already wired in Development (`/scalar`). Keep it.

## Implementation order

Follow numbered files in [`implementation-steps/`](./implementation-steps).
