# Shortpath frontend

Vue 3 + Vite + Pinia UI for the URL shortener. It is a working client template: forms, routing, auth state, and Axios calls are in place. The ASP.NET API in `url-shortener-api` still needs to implement the contract this app already uses.

## Run locally

```bash
cd url-shortener-web
cp .env.example .env
pnpm install
pnpm dev
```

Vite serves the app on `http://localhost:5173` by default.

## Environment variables

Copy `.env.example` to `.env`. Vite only exposes names that start with `VITE_`.

| Variable | Example | Purpose |
| --- | --- | --- |
| `VITE_API_BASE_URL` | `http://localhost:5062` | Axios base URL for the ASP.NET API. No trailing slash. |
| `VITE_PUBLIC_SHORT_BASE_URL` | `http://localhost:5062` | Origin used to display short links if the API omits `shortUrl`. |

Restart `pnpm dev` after changing `.env`.

## What the UI expects from the API

Full request/response shapes, status codes, CORS, and JWT details live in [`../url-shortener-api/README.md`](../url-shortener-api/README.md). Step-by-step ASP.NET work is in [`../url-shortener-api/implementation-steps`](../url-shortener-api/implementation-steps).

Short version:

- `POST /api/url/shorten`
- `GET /api/url/mine`
- `GET /api/url/{shortCode}`
- `GET /api/url/stats/{shortCode}`
- `DELETE /api/url/{shortCode}`
- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/logout`
- `GET /api/auth/me`
- Public redirect: `GET /{shortCode}` → `302` to the original URL

Auth header: `Authorization: Bearer <token>`.

## Project map

| Path | Role |
| --- | --- |
| `src/api/` | Axios client and typed queries |
| `src/stores/auth.ts` | Pinia session (token, user, login/register) |
| `src/stores/links.ts` | Shorten, list, stats, delete |
| `src/views/` | Pages |
| `src/components/` | Header, shorten form, link list, stats |

Until the API is up, shorten/login calls fail with a reachable-API error. That is expected.

## Scripts

- `pnpm dev` — Vite dev server
- `pnpm build` — typecheck and production build
- `pnpm preview` — serve the production build
