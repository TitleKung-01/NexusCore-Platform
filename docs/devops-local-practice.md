# ฝึก DevOps บนเครื่องตัวเอง (ไม่ต้องเช่า VPS)

โปรเจกต์นี้ออกแบบให้ฝึก flow แบบในบริษัทจริง แต่รันบน **laptop + Docker Desktop + GitHub ฟรี** เท่านั้น

## สามโหมด

```text
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐
│  1. make dev    │ ──► │ 2. make docker-up│ ──► │ 3. Deploy จริง │
│  พัฒนา / debug  │     │ ลอง container    │     │ (ทีหลัง, เสียเงิน)│
│  :5173          │     │ :8080            │     │ domain จริง      │
└─────────────────┘     └──────────────────┘     └─────────────────┘
        ▲                         ▲
        │                         │
   ฟรี 100%                  ฟรี (Docker)
```

| โหมด | คำสั่ง | ฝึกทักษะอะไร |
|------|--------|----------------|
| **Local dev** | `make dev` | microservices, env config, hot reload |
| **Docker local** | `make docker-up` | image, compose, network, Nginx proxy |
| **CI (cloud runner ฟรี)** | push Git → Actions | pipeline, automated build |

**ห้ามรัน `make dev` กับ `make docker-up` พร้อมกัน** — พอร์ต 5000 ชนกัน

---

## สัปดาห์ที่ 1 — ให้รันซ้ำได้

### เป้าหมาย

เพื่อน (หรือคุณในเครื่องใหม่) clone แล้วรันได้โดยไม่ถาม

### ทำอะไร

1. ติดตั้ง: .NET 10 SDK, Node 20, Docker Desktop, `make` (Git Bash / Chocolatey)
2. `make install`
3. `make dev` → เปิด http://localhost:5173
4. Login ทด (admin / password123)
5. `make stop` แล้ว `make docker-up` → เปิด http://localhost:8080
6. `make health` หลัง stack รัน

### เอกสารที่อ่าน

- [environments.md](./environments.md)

---

## สัปดาห์ที่ 2 — Build อัตโนมัติบนเครื่อง

### เป้าหมาย

รู้ว่าโค้ดพังตอน build ไม่ต้องรอ deploy

### ทำอะไร

```bash
make ci-local
```

เทียบเท่า job ใน GitHub Actions (dotnet + npm build)

---

## สัปดาห์ที่ 3 — CI บน GitHub (ยังไม่ deploy)

### เป้าหมาย

ทุกครั้ง push / PR มี pipeline ตรวจให้

### ทำอะไร

1. Push repo ขึ้น GitHub
2. เปิดแท็บ **Actions** → workflow **CI** ต้องเขียว
3. อ่าน [ci-github-actions.md](./ci-github-actions.md)

### ฝึกอะไร

- Pull Request + status check
- build แยก job: dotnet, frontend, docker
- cache Docker layer (เร็วขึ้น)

**ยังไม่ต้องเช่า server** — แค่ build บน runner ของ GitHub

---

## สัปดาห์ที่ 4 — จำลอง deploy บนเครื่อง

### เป้าหมาย

เข้าใจว่า image ที่ CI build คือสิ่งที่เอาไปรันบน server

### ทำอะไร

1. `make docker-down`
2. `make docker-up` (build image ใหม่)
3. ใช้ [deploy-checklist.md](./deploy-checklist.md)
4. `make docker-logs` เมื่อ error

### ทางเลือกฟรี (ไม่เช่า VPS)

- **ngrok** / **Cloudflare Tunnel**: เปิด `localhost:8080` ให้มือถือเพื่อนเข้าทดชั่วคราว

---

## Config แยก environment (สรุป)

### Gateway → Backend

| Environment | ไฟล์ | URL |
|-------------|------|-----|
| Development | `gateway/appsettings.Development.json` | `http://localhost:5100` |
| Production | `gateway/appsettings.Production.json` | `http://backend-service:5100` |

### Frontend → API

| โหมด | ไฟล์ | พฤติกรรม |
|------|------|----------|
| Local | `.env.development` | ยิง `http://localhost:5000` |
| Docker | `.env.docker` + `nginx.conf` | ยิง `/api` ผ่าน Nginx ใน container |

### JWT (backend)

- Local: `backend/appsettings.Development.json`
- Docker: env `Jwt__Key` ใน `docker-compose.yml`
- อย่า commit key จริงของ production — ดู `backend/appsettings.example.json`

---

## คำสั่ง Makefile ทั้งหมด

| คำสั่ง | ใช้เมื่อ |
|--------|---------|
| `make help` | ดูรายการ |
| `make dev` | พัฒนาปกติ |
| `make stop` | ปิดพอร์ต 5100/5000/5173 |
| `make docker-up` | ลอง stack แบบ production local |
| `make docker-down` | หยุด container |
| `make docker-logs` | ดู log |
| `make ci-local` | ทด build แบบ CI |
| `make health` | ทด `/health` |

---

## เมื่อพร้อม deploy จริง (ไม่ใช่ขั้นฝึกบังคับ)

1. เช่า VPS หรือใช้ Railway / Render / Azure
2. ตั้ง domain + HTTPS
3. ใช้ GitHub Actions **push image** ไป registry แล้ว pull บน server
4. ใส่ secrets จริง (JWT, DB)

โปรเจกต์ตอนนี้เตรียมขั้น 1–3 บนเครื่องคุณไว้แล้ว
