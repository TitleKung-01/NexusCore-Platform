# NexusCore Platform

Fullstack demo: React (Vite) + YARP Gateway + ASP.NET Core API (`src/` Clean Architecture — EF Core, PostgreSQL, JWT, FluentValidation, Swagger).

## Quick start

```bash
make install
make dev    # starts PostgreSQL (Docker) + backend + gateway + frontend
```

Open http://localhost:5173

Login: `admin` / `password123`

## Docker (practice deploy locally)

```bash
make stop
make docker-up
```

Open http://localhost:8081

Stack: `postgres` + `backend-service` + `gateway-service` + `frontend-service`

## PostgreSQL only (optional)

```bash
make db-up      # localhost:5432
make db-down
```

Copy [`.env.example`](.env.example) to `.env` to override `POSTGRES_PASSWORD` for Docker Compose.

## Documentation

See **[docs/README.md](./docs/README.md)** — [architecture](./docs/architecture.md), environments, DevOps practice plan, CI, deploy checklist.

## Requirements

- .NET 10 SDK
- Node.js 20+
- Docker Desktop (`make dev`, `make docker-up`, PostgreSQL)
- `make` (Git for Windows or Chocolatey)
