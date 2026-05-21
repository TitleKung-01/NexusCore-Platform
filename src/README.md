# HR-Lite — Clean Architecture backend (.NET)

```
src/
├── NexusCore.Domain/           # Entities, repository interfaces
├── NexusCore.Application/    # DTOs, validators, use cases (services)
├── NexusCore.Infrastructure/ # EF Core, JWT, IUserRepository
└── NexusCore.Api/            # Controllers, Program.cs, host
```

## Dependency

```
Api → Application, Infrastructure
Infrastructure → Application, Domain
Application → Domain
```

## คำสั่ง

```bash
# จาก repo root
dotnet run --project src/NexusCore.Api
make backend

# EF migrations (startup project = Api)
dotnet ef migrations add <Name> \
  --project src/NexusCore.Infrastructure \
  --startup-project src/NexusCore.Api
```

## Docker

Build context: `./src` — Dockerfile: `NexusCore.Api/Dockerfile`
