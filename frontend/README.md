# NexusCore Frontend — React + Vite

## โครงสร้าง `src/`

```
src/
├── api.js                      # base URL, JWT ใน localStorage
├── App.jsx                     # จัด layout + เชื่อม features
├── features/
│   ├── auth/
│   │   ├── useAuth.js          # login, logout, secret-data
│   │   ├── LoginForm.jsx
│   │   └── DashboardPanel.jsx
│   └── users/
│       └── useUsers.js         # GET /api/users
└── shared/components/
    └── ErrorAlert.jsx
```

## API URL

Frontend เรียก REST API ผ่าน **Gateway** เท่านั้น (ไม่ยิง backend :5100 โดยตรงจาก browser)

| โหมด | การตั้งค่า |
|------|------------|
| Local dev | `VITE_API_URL=http://localhost:5000` (gateway) |
| Docker | ว่าง — เรียก `/api` ผ่าน Nginx → gateway |

ข้อมูลอยู่ที่ **PostgreSQL** (`make db-up` หรือรวมใน `make docker-up`) — ดู [docs/environments.md](../docs/environments.md)

## คำสั่ง

```bash
npm run dev
npm run build
npm run build:docker   # ใช้ใน Dockerfile
```
