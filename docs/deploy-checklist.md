# Deploy checklist (Docker local)

ใช้ก่อน/หลัง `make docker-up` เพื่อฝึกนิสัย deploy จริง

## ก่อนรัน

- [ ] ปิด `make dev` / `dotnet run` ที่ใช้พอร์ต 5000 (`make stop`)
- [ ] Docker Desktop เปิดอยู่
- [ ] `make ci-local` ผ่าน (หรือ CI บน GitHub เขียว)
- [ ] ไม่มี container เก่าค้าง (`make docker-down`)

## รัน stack

```bash
make docker-up
```

## หลังรัน — smoke test

- [ ] เปิด http://localhost:8080 — หน้าเว็บโหลด
- [ ] Login (admin / password123) สำเร็จ
- [ ] ดึงข้อมูลลับได้ (JWT ผ่าน gateway)
- [ ] `make health` — backend + gateway ตอบ `healthy`

```bash
make health
curl -s http://localhost:5000/health
curl -s http://localhost:8080/
```

## ถ้าพัง

```bash
make docker-logs
```

| อาการ | แนวทาง |
|--------|--------|
| พอร์ต 5000 ถูกใช้ | `make stop` หรือปิด process อื่น |
| 502 / API ไม่ถึง backend | ดู gateway log, ตรวจ `appsettings.Production.json` |
| หน้าเว็บว่าง | rebuild: `make docker-down` แล้ว `make docker-up` |

## หลังทดเสร็จ

```bash
make docker-down
```

## สลับกลับไปพัฒนา

```bash
make docker-down
make dev
```

เปิด http://localhost:5173 (Vite)
