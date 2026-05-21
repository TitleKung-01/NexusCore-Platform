# HR-Lite

Intranet HR-lite: React (TypeScript, shadcn/ui) + YARP Gateway + ASP.NET Core API + PostgreSQL.

ฟีเจอร์หลัก: โปรไฟล์, ลา/โควต้า/ปฏิทิน, OT, ลงเวลา, สลิปเงินเดือน, เบิกค่าใช้จ่าย, onboarding, ประเมินผล, ประกาศ HR, รายงาน CSV, แจ้งเตือนในแอป (กระดิ่งบนหน้าเว็บ).

## Quick start

```bash
make install
make dev    # starts PostgreSQL (Docker) + backend + gateway + frontend
```

Open http://localhost:5173

Demo logins (password `password123`):

| Username | Role | อีเมลตัวอย่าง |
|----------|------|----------------|
| `admin` | Hr | admin@hr-lite.local |
| `mgr.eng` | Manager (Engineering) | wichai.j@hr-lite.local |
| `mgr.sales` | Manager (Sales) | suda.r@hr-lite.local |
| `mgr.hr` | Manager (HR) | prapa.m@hr-lite.local |
| `emp001` … `emp030` | Employee | somchai.j@hr-lite.local ฯลฯ |

ระบบ seed พนักงาน mock **30 คน** (+ admin) อัตโนมัติตอน start API — รายชื่อเต็มใน `src/NexusCore.Infrastructure/Persistence/SeedData/MockEmployeeSeed.cs`

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
