# HR-Lite — Clean Architecture backend (.NET)

```
src/
├── NexusCore.Domain/           # Entities, repository interfaces
├── NexusCore.Application/    # DTOs, validators, use cases (services)
├── NexusCore.Infrastructure/ # EF Core, JWT, repositories, migrations
└── NexusCore.Api/            # Controllers, Program.cs, host
```

## Dependency

```
Api → Application, Infrastructure
Infrastructure → Application, Domain
Application → Domain
```

## คำสั่ง (รันจาก repo root เท่านั้น)

```powershell
cd "...\NexusCore-Platform"

make db-up

# หลังแก้ entity / AppDbContext — ต้อง add ก่อน update
dotnet ef migrations add <Name> `
  --project src/NexusCore.Infrastructure/NexusCore.Infrastructure.csproj `
  --startup-project src/NexusCore.Api/NexusCore.Api.csproj `
  --output-dir Persistence/Migrations

dotnet ef database update `
  --project src/NexusCore.Infrastructure/NexusCore.Infrastructure.csproj `
  --startup-project src/NexusCore.Api/NexusCore.Api.csproj

make dev
```

ถ้าอยู่ใน `src/NexusCore.Api` ใช้ path แบบนี้แทน (อย่าใส่ `src/` ซ้ำ):

```powershell
dotnet ef database update `
  --project ..\NexusCore.Infrastructure\NexusCore.Infrastructure.csproj `
  --startup-project .
```

`make dev` เรียก `MigrateAsync` ตอน start — ถ้า model กับ migration ไม่ตรงจะ crash แบบ `PendingModelChangesWarning`

## Docker

Build context: `./src` — Dockerfile: `NexusCore.Api/Dockerfile`
