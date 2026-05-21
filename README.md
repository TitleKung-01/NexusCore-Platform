# NexusCore Platform

Fullstack demo: React (Vite) + YARP Gateway + ASP.NET Core API (EF Core, JWT, FluentValidation, Swagger).

## Quick start

```bash
make install
make dev
```

Open http://localhost:5173

## Docker (practice deploy locally)

```bash
make stop
make docker-up
```

Open http://localhost:8081

## Documentation

See **[docs/README.md](./docs/README.md)** — environments, DevOps practice plan, CI, deploy checklist.

## Requirements

- .NET 10 SDK
- Node.js 20+
- Docker Desktop (for `make docker-up`)
- `make` (Git for Windows or Chocolatey)
