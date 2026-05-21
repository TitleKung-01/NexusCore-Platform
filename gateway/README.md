# NexusCore Gateway — YARP

Reverse proxy ระหว่าง `frontend` กับ `backend` — **ไม่มี business logic**

## โครงสร้าง

```
gateway/
├── Program.cs
├── appsettings.json          # routes (Development: localhost backend)
├── appsettings.Production.json # Docker: backend-service cluster
└── Dockerfile
```

## กฎ

- กำหนด route / cluster / CORS เท่านั้น
- ไม่ใส่ domain rules (auth, users) ที่นี่ — อยู่ใน `backend/`

## พอร์ต

- Local: `http://localhost:5000`
- Docker: expose `5000` (frontend เรียกผ่าน Nginx `/api`)
