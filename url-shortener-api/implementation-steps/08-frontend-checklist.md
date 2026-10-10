# Step 8 — Click through the Vue app

## Run both sides

Terminal 1:

```bash
dotnet run --project url-shortener-api
```

Terminal 2:

```bash
cd url-shortener-web
pnpm dev
```

Confirm `.env`:

```
VITE_API_BASE_URL=http://localhost:5062
VITE_PUBLIC_SHORT_BASE_URL=http://localhost:5062
```

## Manual path

1. Open `http://localhost:5173`
2. Shorten `https://example.com` — card shows a copyable short URL
3. Open that short URL — browser follows a 302
4. Back in the studio, click the link in the sidebar — traffic panel fills
5. Sign up on `/register` — header shows your username
6. Shorten another URL while logged in
7. Refresh — still logged in
8. Log out, log in on `/login`
9. Delete a link — it disappears from the list
10. DevTools → Network: no CORS errors, `Authorization` header on authenticated calls

## Common mismatches

| Symptom | Likely cause |
| --- | --- |
| "Cannot reach the API" | API not on 5062, or wrong `VITE_API_BASE_URL` |
| CORS error | `UseCors` missing or origin not `http://localhost:5173` |
| 404 on `/api/url/shorten` | Controller renamed; path must stay `/api/url/...` |
| Shorten 200 but UI looks empty | Response missing `shortCode` / `shortUrl` |
| Stats panel empty | `byDay` / `byReferrer` not objects with `label` + `count` |
| Login succeeds but refresh logs out | `GET /api/auth/me` not implemented or token invalid |
| Redirect shows JSON | Click handler still on `/api/url/{code}` instead of `GET /{code}` |

When this checklist passes, the frontend template and your API are speaking the same language.
