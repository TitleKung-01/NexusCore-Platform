# Folder structure reference (ASP.NET Core + React only)

## Layered — ASP.NET Core monolith (`backend/`)

Current NexusCore pattern:

```
backend/
├── Controllers/
├── Services/             # I*Service + implementations
├── DTOs/
├── Validators/           # FluentValidation
├── Models/               # EF entities
├── Data/
│   ├── AppDbContext.cs
│   └── Migrations/
├── Program.cs
├── backend.csproj
└── appsettings*.json
```

Optional as complexity grows:

```
├── Repositories/
├── Mappings/             # manual or Mapster
└── Middleware/
```

## Layered — class libraries (light split)

```
src/
├── Api/
├── Application/
├── Domain/
└── Infrastructure/
```

References: `Api → Application, Infrastructure` | `Application → Domain` | `Infrastructure → Application, Domain`

## Clean Architecture — .NET multi-project

```
src/
├── NexusCore.Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Enums/
│   ├── Exceptions/
│   └── Interfaces/
├── NexusCore.Application/
│   ├── Users/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── DTOs/
│   │   └── handlers or I*Service
│   └── Common/
├── NexusCore.Infrastructure/
│   ├── Persistence/
│   │   ├── AppDbContext.cs
│   │   ├── Repositories/
│   │   └── Migrations/
│   └── Identity/         # JWT (see backend/Services/JwtTokenService.cs)
└── NexusCore.Api/
    ├── Controllers/
    ├── Program.cs
    └── appsettings.json
```

**Domain.csproj**: no project refs; no EF/Swagger packages.

**Application.csproj**: references Domain only.

**Infrastructure.csproj**: references Application + Domain.

**Api.csproj**: references Application + Infrastructure.

## Microservices — NexusCore repo layout

```
HR-Lite/
├── gateway/                    # YARP (existing)
├── backend/                    # optional: keep until split complete
├── services/
│   ├── auth-service/
│   └── users-service/
├── contracts/                  # optional OpenAPI
├── frontend/
├── docker-compose.yml
└── docs/api.md
```

Per service (Layered or Clean inside each folder):

```
services/auth-service/
├── Controllers/ or Api/
├── Services/ or Application/
├── Data/ or Infrastructure/
├── Dockerfile
└── appsettings.json
```

**Do not** share one SQLite DB file across services in production.

## Gateway (`gateway/`)

```
gateway/
├── Program.cs
├── appsettings.json      # YARP routes → backend clusters
├── gateway.csproj
└── Dockerfile
```

Proxy only — auth termination/rate limits OK; no user/domain rules.

## React + Vite (`frontend/`)

```
frontend/
├── src/
│   ├── main.jsx
│   ├── App.jsx
│   ├── api.js
│   ├── features/
│   │   ├── auth/
│   │   └── users/
│   ├── shared/
│   │   └── components/
│   └── pages/              # optional
├── package.json
└── vite.config.js
```

## Tests (.NET + optional frontend)

| Style | Layout |
|-------|--------|
| Layered | `tests/Unit/Services`, `tests/Integration/Api` |
| Clean | `*.Domain.Tests`, `*.Application.Tests`, `*.Api.IntegrationTests` |
| Microservices | `services/{name}/tests/` |
| React | `frontend/src/**/*.test.jsx` (Vitest) co-located or `__tests__/` |
