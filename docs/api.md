# REST API (ASP.NET Core)

Backend อยู่ที่ `src/NexusCore.Api/` (Clean Architecture) — เรียกจาก React ผ่าน **Gateway** (`/api/...`)

## Architecture

```text
React (Axios + JWT) → Gateway :5000 → Backend :5100 → PostgreSQL
```

## Roles

`Employee`, `Manager`, `Hr`, `Admin`

## Seed users (password: `password123`)

| Username | Role | หมายเหตุ |
|----------|------|----------|
| admin | Hr | อนุมัติได้ทุกคำขอ |
| manager | Manager | อนุมัติลูกทีม (employee) |
| employee | Employee | ยื่นลา |

## Endpoints

### Auth

| Method | Path | Auth | คำอธิบาย |
|--------|------|------|----------|
| POST | `/api/auth/login` | — | `{ username, password }` → JWT |

### Profile

| Method | Path | Auth | คำอธิบาย |
|--------|------|------|----------|
| GET | `/api/me` | Bearer | โปรไฟล์ + role |
| PUT | `/api/me` | Bearer | แก้ fullName, email, phone |

### Reference data

| Method | Path | Auth |
|--------|------|------|
| GET | `/api/departments` | Bearer |
| GET | `/api/leave-types` | Bearer |

### Leave requests (workflow)

สถานะ: `Draft` → `Pending` → `Approved` / `Rejected` / `Cancelled`

| Method | Path | คำอธิบาย |
|--------|------|----------|
| GET | `/api/leave-requests?scope=mine` | คำขอของตัวเอง |
| GET | `/api/leave-requests?scope=pending-approval` | รออนุมัติ (Manager/Hr) |
| GET | `/api/leave-requests/{id}` | รายละเอียด |
| POST | `/api/leave-requests` | สร้าง (Draft) |
| POST | `/api/leave-requests/{id}/submit` | ส่งอนุมัติ |
| POST | `/api/leave-requests/{id}/approve` | อนุมัติ |
| POST | `/api/leave-requests/{id}/reject` | ปฏิเสธ + comment |
| POST | `/api/leave-requests/{id}/cancel` | ยกเลิก (owner) |

### Users / HR

| Method | Path | Auth |
|--------|------|------|
| GET | `/api/users` | Bearer |
| POST | `/api/users` | Admin |
| PUT | `/api/users/{id}` | Hr, Admin |
| DELETE | `/api/users/{id}` | Admin |
| GET | `/api/employees` | Hr, Admin |
| PUT | `/api/employees/{userId}` | Hr, Admin |

### Health

| GET | `/health` | — |

## Database

PostgreSQL — `make db-up` แล้ว `dotnet ef database update` (หรือ migrate อัตโนมัติตอน start API)

```bash
dotnet ef migrations add <Name> \
  --project src/NexusCore.Infrastructure \
  --startup-project src/NexusCore.Api \
  --output-dir Persistence/Migrations
```
