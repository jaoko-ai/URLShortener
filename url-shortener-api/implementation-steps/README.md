# ASP.NET implementation steps

Do these in order. Each step ends with a check you can run before moving on. The target contract is the parent [`README.md`](../README.md).

The project already exists (`net10.0`, EF Core, Npgsql, a `Url` controller). You are finishing it, not starting from `dotnet new`.

| Step | File | Outcome |
| --- | --- | --- |
| 1 | [01-project-and-config.md](./01-project-and-config.md) | Config, CORS, no secrets in git |
| 2 | [02-database.md](./02-database.md) | PostgreSQL + EF models that match the UI |
| 3 | [03-shorten-endpoint.md](./03-shorten-endpoint.md) | Full shorten JSON |
| 4 | [04-redirect-and-clicks.md](./04-redirect-and-clicks.md) | Public 302 + click rows |
| 5 | [05-list-stats-delete.md](./05-list-stats-delete.md) | Mine, stats aggregates, delete |
| 6 | [06-jwt-auth.md](./06-jwt-auth.md) | Register, login, me, logout |
| 7 | [07-tie-urls-to-users.md](./07-tie-urls-to-users.md) | Ownership |
| 8 | [08-frontend-checklist.md](./08-frontend-checklist.md) | Click through the Vue app |
