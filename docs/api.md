# REST API (ASP.NET Core)

Backend: `src/NexusCore.Api/` — React เรียกผ่าน Gateway `http://localhost:5000`

## Roles

`Employee`, `Manager`, `Hr`, `Admin`

## Seed users (password: `password123`)

| Username | Role |
|----------|------|
| admin | Hr |
| manager | Manager |
| employee | Employee (legacy — ใช้ `emp001` แทนได้) |
| emp001 … emp030 | Employee |
| mgr.eng, mgr.sales, mgr.hr | Manager |

## Auth

| Method | Path | Auth |
|--------|------|------|
| POST | `/api/auth/login` | — |
| POST | `/api/auth/change-password` | Bearer |

## Profile & reference

| Method | Path |
|--------|------|
| GET/PUT | `/api/me` |
| GET | `/api/departments`, `/api/leave-types` |
| GET | `/api/leave-balances` |

## Notifications (in-app)

แจ้งเตือนในระบบเท่านั้น — แสดงที่กระดิ่งบนหน้าเว็บ ไม่ส่งอีเมล

| Method | Path |
|--------|------|
| GET | `/api/notifications` |
| GET | `/api/notifications/unread-count` |
| POST | `/api/notifications/{id}/read` |
| POST | `/api/notifications/read-all` |

## Leave requests

สถานะ: `Draft` → `Pending` → `Approved` / `Rejected` / `Cancelled`

| Method | Path |
|--------|------|
| GET | `/api/leave-requests?scope=mine\|pending-approval\|approval-history` |
| GET | `/api/leave-requests/calendar?from&to&departmentId` |
| GET/POST | `/api/leave-requests`, `/{id}`, `/{id}/submit`, `approve`, `reject`, `cancel` |
| GET/POST | `/api/leave-requests/{id}/attachments` |
| GET | `/api/leave-requests/attachments/{id}/download` |

## Overtime & expenses

Workflow คล้ายลา — `api/overtime-requests`, `api/expense-claims`

## Attendance

| Method | Path |
|--------|------|
| POST | `/api/attendance/check-in`, `/check-out` |
| GET | `/api/attendance?scope=mine\|team&from&to` |

## HR (Hr/Admin)

| Method | Path |
|--------|------|
| GET/PUT | `/api/employees` |
| GET | `/api/employee-transfers` |
| GET | `/api/reports/leave-summary?year` (CSV) |
| GET | `/api/reports/attendance?from&to&employeeId` (CSV, optional filter) |
| CRUD | `/api/holidays` |
| POST | `/api/payslips` (upload PDF) |
| CRUD | `/api/announcements` |
| CRUD | `/api/onboarding/templates`, assign tasks |

## Payslips (employee)

| GET | `/api/payslips?scope=mine` |

## Database

```bash
make db-up
dotnet ef database update --project src/NexusCore.Infrastructure --startup-project src/NexusCore.Api
```

Migration ล่าสุด: `AddHrPlatformFeatures`
