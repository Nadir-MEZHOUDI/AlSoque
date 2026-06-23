# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

AlSoque is an Arabic-language Blazor Server site documenting the biographies, history, and heritage of scholars from the "AlSouq" region. `D:\Projects\AGENTS.md` (company-wide standards) and `D:\Projects\CLAUDE.md` apply on top of this file — don't repeat them. `AGENTS.md` in this repo's root has the same content as this file in Arabic; keep them in sync if you change one.

## Commands

From the repo root:

```bash
dotnet build                                   # build (also runs Tailwind build, see below)
dotnet build /p:SkipTailwind=true              # skip Tailwind CLI step (CI / fast edit loop)
dotnet run --project src/AlSoque.Web           # run locally: http://localhost:5094, https://localhost:7200 (https profile)

dotnet ef database update --project src/AlSoque.Data --startup-project src/AlSoque.Web
dotnet ef migrations add <Name> --project src/AlSoque.Data --startup-project src/AlSoque.Web

dotnet tool install --global dotnet-ef --version 10.*   # if dotnet-ef isn't installed
```

There is no test project in this solution.

## Prerequisites

- PostgreSQL on `localhost:5433`, database `AlSoqueDb`, user `postgres` / password `5512` (set in `src/AlSoque.Web/appsettings.json`). Override via User Secrets or environment — don't edit that file.
- Node + `npx` available — the build invokes `npx @tailwindcss/cli` as an MSBuild target before `Build` (no manual `npm install` needed if `node_modules` exists).
- Initial admin account seeded from `SeedAdmin` config in `appsettings.Development.json` (`admin@alsoque.net` / `ChangeMe!2026`). Change before deploying.

## Architecture

Vertical Slice, two projects:

- **`src/AlSoque.Data`** — entities (`Entities/`), `AppDbContext` (Identity-backed via `IdentityDbContext<ApplicationUser>`), `Identity/ApplicationUser`, EF migrations, and `Extensions/DateHelper.cs` (Hijri↔Gregorian conversion — ported by hand from the earlier A3lam project; do not import A3lam).
- **`src/AlSoque.Web`** — Blazor Web App, InteractiveServer render mode throughout.
  - `Features/<Name>/` — each feature is self-contained: a `.razor` page plus a `*Service.cs` injected with `IDbContextFactory<AppDbContext>` (never inject `AppDbContext` as Scoped in Blazor pages — see DI note below). Features: `Home`, `ScholarsBrowse`, `ScholarProfile`, `Books`, `Manuscripts`, `Contribute`, `Admin/*` (Scholars, Books, Manuscripts, Families, Specializations, Users, Contributions).
  - `Components/` — `App`/`Routes`/`MainLayout` plus `Components/Account/*`, scaffolded from the ASP.NET Identity template (don't hand-edit beyond what's needed; regenerate via scaffolder if it drifts).
  - `Shared/` — `NavBar`, `Footer`, `ScholarCard`, `SlugHelper`, contribution payload DTOs (`Shared/Contributions/`).
  - `Identity/` — `IdentitySeeder` + `AppRoles` (`Admin`, `Member` only).
  - `Seed/DemoDataSeeder` — runs automatically in `Development` only; never relied on in production.
  - `Styles/app.tailwind.css` — Tailwind v4 source with `@source` globs over all `.razor` files; compiled output is `wwwroot/app.css` (generated, never hand-edited).

One DI wrinkle in `Program.cs`: ASP.NET Identity's `UserStore`/`RoleStore` need `AppDbContext` resolved as a Scoped service directly, so it's registered via `AddScoped<AppDbContext>(sp => sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext())` alongside the factory. Feature services should still take `IDbContextFactory<AppDbContext>`, not `AppDbContext`.

## Key conventions

- RTL throughout: `html { direction: rtl; }` lives in the Tailwind base layer (`app.tailwind.css`). Don't add `dir="ltr"` to Arabic content.
- Styling only via `wwwroot/app.css` (`@layer components`, semantic classes like `navbar`, `card`, `btn-gold`) — no `*.razor.css`, no inline `style="..."`.
- No CQRS, no Repository layer: `*Service.cs` classes hold data-access logic directly.
- Roles are exactly `Admin` and `Member`. Public self-registration always lands as `Member` (see `IdentitySeeder`).
- User-submitted content never publishes directly: a `Contribution` sits `Pending` until an `Admin` approves it at `/admin/contributions`, which applies `Contribution.PayloadJson` onto the target entity.
- `Scholar`, `Family`, `Book`, `Manuscript` each have a unique `Slug` index — when generating slugs (`Shared/SlugHelper.cs`), make sure collisions are handled.
- `ScholarRelation` (teacher/student links) uses `DeleteBehavior.Restrict` on both `TeacherId` and `StudentId` — deleting a `Scholar` with relations will fail until they're removed first.
- `Contribution.PayloadJson` is stored as Postgres `jsonb`.
- Always generate migrations with `--project src/AlSoque.Data --startup-project src/AlSoque.Web` (DbContext lives in Data, startup config in Web).
- Do not touch the `AlMahmoud` or `A3lam` projects from this repo — fully separate codebases.

## Manual verification after changes

No automated test suite — verify by walking the full flow: register → log in → browse scholars → submit a contribution → log in as admin → approve it in `/admin/contributions` → confirm it appears on the public scholar page immediately.

## Deployment

Docker + GHCR + Azure DevOps SSH deploy (not systemd/direct Kestrel). Full details in [docs/DEPLOY.md](docs/DEPLOY.md).

- `src/AlSoque.Web/Dockerfile` fetches the Tailwind v4 linux-x64 binary directly (no Node in the image).
- `docker-compose.yml` (root) runs the container on `127.0.0.1:8089` on the shared `shared-net` network, against the shared `postgres` container (`Database=AlSoqueDb`). `Program.cs` calls `Database.MigrateAsync()` on startup — no manual DB creation needed.
- `DeployToVPS.yml` (root): push to `master` → build + push to `ghcr.io/nadir-mezhoudi/alsoque-web` → SSH to `/opt/AlSoque` and `docker compose up -d`. Reuses existing Azure DevOps service connections (`ghcr-login`, `vps-ssh`).
- Data Protection keys persist to the `dpkeys` volume (non-Development only) — without it, every container restart invalidates all sessions.
- `SeedAdmin__Email` / `SeedAdmin__Password` are env vars in `docker-compose.yml` — set the real password via `/opt/AlSoque/.env` on the server before first run.

## Read first

- [PLAN.html](PLAN.html) — full architectural plan, in Arabic (reference for prior decisions).
- [src/AlSoque.Web/Program.cs](src/AlSoque.Web/Program.cs) — DI wiring, Identity setup, seeders.
- [src/AlSoque.Web/AlSoque.Web.csproj](src/AlSoque.Web/AlSoque.Web.csproj) — Tailwind build target and `SkipTailwind` flag.
