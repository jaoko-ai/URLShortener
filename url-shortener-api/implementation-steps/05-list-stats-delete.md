# Step 5 — List, stats, delete

## Goal

Sidebar, donut chart, and delete button in the Vue app work.

## `GET /api/url/mine`

Add a new action. For this step, if there is no auth yet, return an empty array or all links created in this process. In step 7 you will filter by `UserId`.

Shape: JSON array of the same short-link object as shorten.

Order: `CreatedAt` descending.

## `GET /api/url/stats/{shortCode}`

Replace the raw `Clicks[]` with the object the chart expects (`src/api/types.ts` → `LinkStats`).

Server-side aggregation is easier than making Vue group rows:

```csharp
var clicks = await db.Clicks.Where(c => c.UrlId == url.Id).ToListAsync();

var byDay = clicks
    .GroupBy(c => c.Timestamp.UtcDateTime.Date)
    .OrderBy(g => g.Key)
    .Select(g => new { label = g.Key.ToString("yyyy-MM-dd"), count = g.Count() });

var byReferrer = clicks
    .GroupBy(c => string.IsNullOrWhiteSpace(c.Referrer) ? "direct" : c.Referrer)
    .Select(g => new { label = g.Key, count = g.Count() })
    .OrderByDescending(x => x.count);
```

Return:

- `shortCode`, `originalUrl`, `clickCount`
- `clicks`: last 100, newest first
- `byDay`, `byReferrer`

404 if the short code does not exist.

## `DELETE /api/url/{shortCode}`

You already delete the `Urls` row. With cascade, clicks go away. Return `204` or keep `{ message }`. Vue ignores the body.

## Check

```bash
curl http://localhost:5062/api/url/stats/YOURCODE
curl -X DELETE http://localhost:5062/api/url/YOURCODE
```

In the UI: shorten, click the link in the list, donut + bars render. Delete removes the row.

Next: [06-jwt-auth.md](./06-jwt-auth.md)
