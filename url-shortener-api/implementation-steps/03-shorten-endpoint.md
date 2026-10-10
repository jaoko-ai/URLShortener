# Step 3 — Shorten endpoint

## Goal

`POST /api/url/shorten` matches the Vue `shortenUrl()` helper.

Frontend call (`src/api/urls.ts`):

```http
POST /api/url/shorten
Content-Type: application/json

{ "longUrl": "https://example.com", "customCode": "optional" }
```

## Controller route

`Controllers/Urls.cs` is named `Url` with `[Route("api/[controller]")]`, so the path is already `/api/url/shorten`. Keep that name. Do not rename the controller to `Urls` or the path becomes `/api/urls/shorten` and the UI breaks.

## Request DTO

```csharp
public record ShortenRequest(string LongUrl, string? CustomCode);
```

Validate:

- `Uri.TryCreate` + http/https (you already do this)
- `customCode`: if present, `^[a-zA-Z0-9_-]{3,32}$`
- If custom code exists in DB → `409` with `{ "message": "That code is taken." }`

## Code generation

You already insert, save, then `Utilities.Encode(newUrl.Id)`. That is a solid Base62 approach. Keep it for auto codes.

For custom codes, set `ShortCode` before the first save and skip encode.

## Response

Stop returning only `{ ShortUrl }`. Return:

```csharp
return Created($"/api/url/{entity.ShortCode}", new
{
    id = entity.Id,
    originalUrl = entity.OriginalUrl,
    shortCode = entity.ShortCode,
    shortUrl = $"{publicBase}/{entity.ShortCode}",
    createdAt = entity.CreatedAt,
    clickCount = entity.ClickCount
});
```

Read `PublicBaseUrl` from configuration (`http://localhost:5062`).

`UserId`: leave null for now (step 7).

## Check

```bash
curl -s -X POST http://localhost:5062/api/url/shorten \
  -H 'Content-Type: application/json' \
  -d '{"longUrl":"https://example.com"}'
```

You should see `id`, `shortCode`, `shortUrl`, `clickCount`.

Invalid URL → 400. Duplicate custom code → 409.

Next: [04-redirect-and-clicks.md](./04-redirect-and-clicks.md)
