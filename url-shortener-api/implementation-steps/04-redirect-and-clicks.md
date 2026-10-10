# Step 4 — Public redirect and click logging

## Goal

Opening a short URL in a browser lands on the original site. Each visit writes a `Clicks` row.

## Split two GETs

Today `GET /api/url/{shortCode}` both looks up the URL **and** records a click **and** returns JSON. That fights the UI (stats would count themselves) and does not redirect.

### A. Redirect — `GET /{shortCode}`

In `Program.cs`:

```csharp
app.MapGet("/{shortCode:required}", async (string shortCode, AppDbContext db, HttpContext http) =>
{
    var record = await db.Urls.SingleOrDefaultAsync(u => u.ShortCode == shortCode);
    if (record is null) return Results.NotFound();

    record.ClickCount++;
    db.Clicks.Add(new Clicks
    {
        UrlId = record.Id,
        Referrer = http.Request.Headers.Referer.ToString(),
        UserAgent = http.Request.Headers.UserAgent.ToString(),
        IpAddress = http.Connection.RemoteIpAddress?.ToString(),
        Language = http.Request.Headers.AcceptLanguage.ToString(),
        Timestamp = DateTimeOffset.UtcNow
    });
    await db.SaveChangesAsync();

    return Results.Redirect(record.OriginalUrl);
});
```

Put this **after** `/auth/*` and OpenAPI maps, and make sure it does not steal `/api/...` (those are mapped by controllers). Constraint: short codes should not contain `/`.

### B. Metadata — `GET /api/url/{shortCode}`

Return the short-link JSON **without** incrementing clicks.

```csharp
[HttpGet("{shortCode}")]
public async Task<IActionResult> Get(string shortCode) { ... }
```

404 if missing.

## Check

1. Shorten a URL, copy `shortCode`
2. `curl -I http://localhost:5062/{shortCode}` → `302` and `Location: https://...`
3. A new row in `Clicks`
4. `GET /api/url/{shortCode}` does **not** add another click

Next: [05-list-stats-delete.md](./05-list-stats-delete.md)
