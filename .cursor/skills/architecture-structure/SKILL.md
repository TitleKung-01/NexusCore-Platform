---
name: architecture-structure
description: >-
  Organizes NexusCore folders and implements Layered, Clean, or Microservices
  architecture for ASP.NET Core and React (Vite) only. Use when structuring
  backend/, gateway/, frontend/, refactoring layout, or when the user mentions
  Clean Architecture, Layered Architecture, Microservices, จัดไฟล์,
  โครงสร้างโปรเจกต์, สถาปัตยกรรม.
---

# Architecture & File Structure (NexusCore)

**Stack only:** ASP.NET Core (`backend/`, `gateway/`), React + Vite (`frontend/`). Do not suggest Java, Node, Python, or other backend/frontend stacks.

## NexusCore layout (current baseline)

```
NexusCore-Platform/
├── backend/          # Layered API (EF Core, JWT, FluentValidation)
├── gateway/          # YARP reverse proxy
├── frontend/         # React (Vite)
├── docker-compose.yml
└── docs/
```

Default architecture today: **Layered monolith** + **gateway edge**. Evolve to Clean or `services/*` only when complexity justifies it.

## Workflow

1. **Read the repo first** — use existing `backend/`, `gateway/`, `frontend/` trees; do not invent parallel roots.
2. **Pick architecture** (table below). If the user named one, use it.
3. **Propose or apply layout** — show target tree + dependency rules before large moves.
4. **Place new code** in the correct layer; never violate dependency direction.
5. **Migrate incrementally** — one bounded context or feature at a time; keep `dotnet build` and `npm run build` green.

## Choose architecture

| Situation | Prefer |
|-----------|--------|
| Small app, single team, CRUD API | **Layered** (folders in `backend/`) |
| Rich domain rules, long-lived product, heavy testing | **Clean** (multi-project under `src/` or solution) |
| Independent deploy/scale per domain, separate teams | **Microservices** (`services/` + `gateway/`) |

## Dependency rules (non-negotiable)

### Layered (monolith — `backend/`)

```
Controllers → Services → Data/Repositories
     ↓            ↓
   DTOs      Models (EF entities)
```

- **Controllers**: HTTP only — map request/response, call services; no `DbContext`.
- **Services**: orchestration; expose `I*Service`; consumers do not depend on concrete EF types in interfaces.
- **Data**: `AppDbContext`, migrations, optional repositories.
- **DTOs / Validators**: API boundary; map to/from entities in services.

### Clean Architecture (.NET)

```
Api → Application → Domain ← Infrastructure
```

- **Domain**: entities, value objects, exceptions, repository interfaces — no EF/Swagger package refs.
- **Application**: use cases, application DTOs, FluentValidation, external port interfaces.
- **Infrastructure**: EF Core, JWT, adapters implementing ports.
- **Api**: controllers, `Program.cs`, DI composition root.

Forbidden: Domain → Application/Infrastructure/Api; Application → Infrastructure/Api.

### Microservices (NexusCore-style)

```
frontend → gateway (YARP) → backend | services/*
```

- One **database per service**; no shared EF model across deployables.
- Cross-service: HTTP + OpenAPI/contracts in `docs/` or `contracts/` — not shared DB libraries.
- **gateway/**: routes, CORS, proxy — no domain/business rules.

## Folder templates

See [structures.md](structures.md) — .NET monolith, Clean multi-project, microservices repo, React feature folders.

## Adding a feature (checklist)

```
- [ ] Architecture confirmed (layered / clean / microservices)
- [ ] Files under backend/ | gateway/ | frontend/ (correct layer)
- [ ] Dependencies inward (Clean) or downward (Layered)
- [ ] Public API uses DTOs, not EF entities
- [ ] DI in Program.cs (backend or Api project)
- [ ] EF migrations only in Data or Infrastructure/Persistence
- [ ] gateway/appsettings route if new deployable service
- [ ] make dev / dotnet build still passes
```

## Naming & files (.NET)

| Kind | Layered (`backend/`) | Clean |
|------|----------------------|-------|
| HTTP | `Controllers/{Feature}Controller.cs` | `*.Api/Controllers/` |
| Logic | `Services/I{Feature}Service.cs`, `{Feature}Service.cs` | `Application/{Feature}/` |
| Persistence | `Data/`, `Models/` | `Infrastructure/Persistence/`, `Domain/Entities/` |
| API contracts | `DTOs/`, `Validators/` | `Application/DTOs/` + validators |
| Migrations | `Data/Migrations/` | `Infrastructure/Persistence/Migrations/` |

Namespace: match project name (e.g. `backend.Services`). One public type per file.

## Frontend (React + Vite only)

Prefer **feature folders** as `frontend/` grows:

```
frontend/src/
├── features/{feature}/   # components, hooks, *Api.js
├── shared/components|hooks
└── api.js                # base URL, JWT interceptor (gateway :8081 or dev proxy)
```

Calls go through gateway in Docker; local dev may proxy to backend — match existing `frontend/src/api.js`.

## Refactoring between styles

| From → To | Strategy |
|-----------|----------|
| Layered → Clean | Add Domain/Application/Infrastructure projects; move entities + ports first |
| Monolith → Microservices | Extract bounded context to `services/{name}/`; YARP route in `gateway/` |
| Clean → Layered | Merge only if team agrees — document tradeoff |

Examples: [examples.md](examples.md)

## Agent behavior

- **NexusCore conventions first** — match `backend/Controllers`, `Makefile`, `docker-compose.yml`.
- **Minimize diff** — no unrelated restructures in one PR.
- **Ask once** if architecture is ambiguous and >5 files move.
- Verify with `make dev` targets or `dotnet build` + `npm run build` after structural changes.

## Additional resources

- [structures.md](structures.md)
- [examples.md](examples.md)
