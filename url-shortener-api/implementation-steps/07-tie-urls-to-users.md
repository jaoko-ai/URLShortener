# Step 7 — Attach links to users

## Goal

Logged-in shortenings belong to that account. `/api/url/mine` lists them. Delete/stats can be owner-only.

## Read the current user in controllers

```csharp
var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
Guid? userId = Guid.TryParse(userIdValue, out var parsed) ? parsed : null;
```

Guests: `User` is unauthenticated, `userId` is null. Still allow `POST /api/url/shorten`.

## Shorten

If `userId` is present, set `entity.UserId = userId`.

## Mine

`[Authorize]`

```csharp
var urls = await db.Urls
    .Where(u => u.UserId == userId)
    .OrderByDescending(u => u.CreatedAt)
    .ToListAsync();
```

Map to the short-link JSON array.

## Ownership

For `DELETE` and `stats` (once you want it locked down):

- If the url has `UserId == null`, allow (legacy/guest)
- If it has an owner, require matching JWT
- Otherwise 403 `{ "message": "Not your link." }`

## Check

1. Register user A, shorten a link while logged in
2. `GET /api/url/mine` with A's token returns that link
3. Register user B, same `mine` call returns `[]`
4. B cannot delete A's code (403)
5. Guest shorten still works without a token

Next: [08-frontend-checklist.md](./08-frontend-checklist.md)
