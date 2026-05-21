# HR-Lite

Intranet HR-lite: React (TypeScript, shadcn/ui) + YARP Gateway + ASP.NET Core API + PostgreSQL + **n8n** (อีเมลแจ้งเตือน).

ฟีเจอร์หลัก: โปรไฟล์, ลา/โควต้า/ปฏิทิน, OT, ลงเวลา, สลิปเงินเดือน, เบิกค่าใช้จ่าย, onboarding, ประเมินผล, ประกาศ HR, รายงาน CSV, แจ้งเตือนในแอป + อีเมลผ่าน n8n.

## Quick start

```bash
make install
make dev    # starts PostgreSQL (Docker) + backend + gateway + frontend
```

Open http://localhost:5173

Demo logins (password `password123`): `employee`, `manager`, `admin`

## Docker (practice deploy locally)

```bash
make stop
make docker-up
```

Open http://localhost:8081

Stack: `postgres` + `n8n` + `backend-service` + `gateway-service` + `frontend-service`

- n8n UI: http://localhost:5678 — ดู [docs/n8n/README.md](./docs/n8n/README.md)

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
