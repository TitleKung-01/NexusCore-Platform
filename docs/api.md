# REST API (ASP.NET Core)

Backend อยู่ที่ `backend/` — เรียกจาก React ผ่าน **Gateway** (`/api/...`) ไม่ยิงตรงพอร์ต 5100 ใน production/Docker

## Architecture

```text
React (Axios + JWT interceptor) → Gateway :5000 → Backend :5100 → SQLite (EF Core)
```

| Mode | API base URL |
|------|----------------|
| Local `make dev` | `http://localhost:5000` (`.env.development`) |
| Docker | same origin `/api` (Nginx → gateway) |

## OpenAPI / Swagger

รัน backend โดยตรง (Development):

- http://localhost:5100/swagger

ผ่าน Gateway ไม่ proxy `/swagger` — ใช้พอร์ต backend ตอนพัฒนา

## Endpoints

| Method | Path | Auth | คำอธิบาย |
|--------|------|------|----------|
| POST | `/api/auth/login` | — | Body: `{ "username", "password" }` → JWT |
| GET | `/api/auth/secret-data` | Bearer | ข้อมูลลับ (demo) |
| GET | `/api/users` | Bearer | รายการผู้ใช้ |
| GET | `/api/users/{id}` | Bearer | ผู้ใช้ตาม id |
| POST | `/api/users` | Bearer + **Admin** | สร้างผู้ใช้ |
| GET | `/health` | — | Health (ไม่ผ่าน `/api` prefix ของ gateway route) |

### Seed user (ครั้งแรกที่ migrate)

- Username: `admin`
- Password: `password123`
- Role: `Admin`

## Frontend

- `frontend/src/api.js` — Axios instance + **JWT request interceptor** + ลบ token เมื่อ 401
- ไม่ต้องใส่ `Authorization` เองในแต่ละ request

## Validation

FluentValidation บน `LoginRequest`, `CreateUserRequest` — 400 พร้อมรายละเอียด field

## Database

- **EF Core + SQLite** (`ConnectionStrings:DefaultConnection`)
- Local: `backend/nexuscore.db` (gitignored)
- Docker: volume `backend-data` ที่ `/app/data/nexuscore.db`

```bash
cd backend
dotnet ef migrations add <Name> --output-dir Data/Migrations
dotnet ef database update
```
