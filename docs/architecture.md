# NexusCore — สถาปัตยกรรมและโครงสร้างโฟลเดอร์

สแต็ก: **ASP.NET Core** (`src/`, `gateway/`) + **React (Vite)** (`frontend/`)

## ภาพรวม

```
frontend → gateway (YARP) → NexusCore.Api (src/)
```

| ส่วน | บทบาท | สถาปัตยกรรม |
|------|--------|-------------|
| `frontend/` | UI | Feature folders |
| `gateway/` | YARP proxy | Edge only |
| `src/` | REST API | **Clean Architecture** |

## `src/` — Clean Architecture

```
src/
├── NexusCore.Domain/         # Entities, IUserRepository
├── NexusCore.Application/  # DTOs, validators, I*Service
├── NexusCore.Infrastructure/ # EF, JWT, repositories
└── NexusCore.Api/            # Controllers, Program.cs
```

```
Api → Application, Infrastructure
Infrastructure → Application, Domain
Application → Domain
```

- **Domain**: ไม่มี EF / ASP.NET packages
- **Application**: use cases — ไม่รู้จัก `DbContext`
- **Infrastructure**: `AppDbContext`, migrations, adapters
- **Api**: HTTP + DI composition + JWT/Swagger config

ดู [src/README.md](../src/README.md)

## `frontend/src/`

```
features/auth/   features/users/   shared/components/   api.js
```

## Docker

`docker-compose.yml` — build context `./src`, Dockerfile `NexusCore.Api/Dockerfile`

## โฟลเดอร์ `backend/`

เก็บเฉพาะ README ชี้ทาง — โค้ดย้ายไป `src/` แล้ว

## ขยายต่อ

| ความต้องการ | ทางเลือก |
|-------------|----------|
| แยก deploy ต่อ domain | `services/*` + YARP routes |
