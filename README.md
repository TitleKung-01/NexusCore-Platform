# HR-Lite — ระบบ HR ภายในองค์กร (Intranet)

**HR-Lite** เป็นแพลตฟอร์ม HR แบบเบา (HR-lite) สำหรับใช้งานภายในองค์กร ครอบคลุมการลา ลงเวลา ล่วงเวลา เบิกค่าใช้จ่าย สลิปเงินเดือน onboarding ประกาศ และรายงานสำหรับทีม HR — พัฒนาด้วย **React + TypeScript** ฝั่งหน้าเว็บ, **ASP.NET Core** ฝั่ง API, **YARP Gateway** เป็นตัวกลาง, และ **PostgreSQL** เป็นฐานข้อมูล

โปรเจกต์นี้ออกแบบให้รันบนเครื่องพัฒนาได้ง่าย (`make dev`) และฝึก deploy แบบ production-like ด้วย Docker (`make docker-up`) โดยไม่ต้องเช่า server

---

## สารบัญ

1. [ภาพรวมและจุดประสงค์](#ภาพรวมและจุดประสงค์)
2. [เทคโนโลยีที่ใช้](#เทคโนโลยีที่ใช้)
3. [สถาปัตยกรรมระบบ](#สถาปัตยกรรมระบบ)
4. [ฟีเจอร์หลักตามบทบาท](#ฟีเจอร์หลักตามบทบาท)
5. [โครงสร้างโปรเจกต์](#โครงสร้างโปรเจกต์)
6. [ความต้องการของระบบ](#ความต้องการของระบบ)
7. [เริ่มต้นใช้งาน](#เริ่มต้นใช้งาน)
8. [บัญชีทดสอบ (Demo)](#บัญชีทดสอบ-demo)
9. [ฐานข้อมูลและ Migration](#ฐานข้อมูลและ-migration)
10. [API และเอกสารเพิ่มเติม](#api-และเอกสารเพิ่มเติม)
11. [คำสั่ง Make ที่ใช้บ่อย](#คำสั่ง-make-ที่ใช้บ่อย)
12. [การพัฒนาและ CI](#การพัฒนาและ-ci)
13. [ข้อจำกัดและขอบเขต](#ข้อจำกัดและขอบเขต)

---

## ภาพรวมและจุดประสงค์

| หัวข้อ | รายละเอียด |
|--------|-------------|
| **ประเภท** | Intranet HR Portal — ไม่ใช่ payroll ระดับ enterprise เต็มรูปแบบ |
| **ผู้ใช้หลัก** | พนักงาน, หัวหน้างาน (Manager), ทีม HR |
| **ภาษา UI** | ไทยเป็นหลัก (เมนูและข้อความในหน้าเว็บ) |
| **การยืนยันตัวตน** | JWT (Bearer token) หลัง login |
| **ข้อมูลเริ่มต้น** | Seed พนักงาน mock ~30 คน + admin อัตโนมัติเมื่อ API เริ่มทำงาน |

ระบบเน้น workflow ที่พบบ่อยใน HR ขนาดกลาง: ยื่นคำขอ → ส่งอนุมัติ → ผู้อนุมัติดำเนินการ → แจ้งเตือนในแอป (กระดิ่ง) — **ไม่ส่งอีเมล** ในตัวอย่างนี้

---

## เทคโนโลยีที่ใช้

### Frontend (`frontend/`)

| เทคโนโลยี | รายละเอียด |
|-----------|-------------|
| **Vite 8** | Dev server และ build |
| **React 19** | UI framework |
| **TypeScript** | Type safety |
| **Tailwind CSS v4** | Styling |
| **shadcn/ui** (new-york) | คอมโพเนนต์ UI |
| **React Router** | Routing หน้าเว็บ |
| **Axios** | เรียก REST API |
| **Sonner** | Toast แจ้งผลการทำงาน |
| **Lucide React** | ไอคอน |

### Backend (`src/`)

| เทคโนโลยี | รายละเอียด |
|-----------|-------------|
| **.NET 10** | Runtime และ SDK |
| **ASP.NET Core** | Web API host |
| **Entity Framework Core** | ORM + migrations |
| **PostgreSQL 16** | ฐานข้อมูลหลัก |
| **JWT Bearer** | Authentication |
| **BCrypt** | Hash รหัสผ่าน |
| **Swagger** | เอกสาร API (โหมด Development) |

### Gateway (`gateway/`)

| เทคโนโลยี | รายละเอียด |
|-----------|-------------|
| **YARP** | Reverse proxy จาก frontend → backend |
| **Rate limiting** | จำกัดจำนวน request (เข้มขึ้นใน Production) |

### DevOps / โครงสร้าง

| เครื่องมือ | การใช้งาน |
|-----------|------------|
| **Docker Compose** | PostgreSQL + stack เต็ม (backend, gateway, frontend) |
| **Makefile** | คำสั่งรวมสำหรับ dev / build / docker |
| **GitHub Actions** | CI build (ดู `docs/ci-github-actions.md`) |

---

## สถาปัตยกรรมระบบ

### การไหลของ request

```
เบราว์เซอร์ (React)
    │
    ▼
Gateway (YARP) — พอร์ต 5000
    │
    ▼
NexusCore.Api (ASP.NET Core) — พอร์ต 5100
    │
    ▼
PostgreSQL — พอร์ต 5432
```

- **โหมดพัฒนา (`make dev`)**: เปิด frontend ที่ Vite `http://localhost:5173` — เรียก API ผ่าน `http://localhost:5000/api/...`
- **โหมด Docker (`make docker-up`)**: เปิด `http://localhost:8081` — Nginx ใน container ส่ง `/api` ไป gateway

### Clean Architecture (Backend)

```
src/
├── NexusCore.Domain/           # Entity, ค่าคงที่, interface repository
├── NexusCore.Application/      # DTO, business logic (services), validators
├── NexusCore.Infrastructure/   # EF Core, repositories, migrations, seed
└── NexusCore.Api/              # Controllers, Program.cs, JWT, Swagger
```

**กฎการอ้างอิงชั้น (Dependency Rule)**

```
Api → Application, Infrastructure
Infrastructure → Application, Domain
Application → Domain
```

- **Domain** ไม่ผูกกับ EF หรือ ASP.NET
- **Application** ไม่รู้จัก `DbContext` โดยตรง
- **Infrastructure** เป็นที่ implement repository และ migration
- **Api** รับ HTTP, ประกอบ DI, ตั้งค่า security

รายละเอียดเพิ่ม: [docs/architecture.md](./docs/architecture.md), [src/README.md](./src/README.md)

### Frontend — โครงสร้าง Feature

```
frontend/src/
├── app/App.tsx              # กำหนด route ทั้งหมด
├── api.ts                   # Axios + แนบ JWT
├── components/
│   ├── ui/                  # shadcn primitives
│   └── layout/AppShell.tsx  # Sidebar + เมนู
└── features/
    ├── auth/                # Login, AuthProvider
    ├── dashboard/           # แดชบอร์ด
    ├── profile/             # โปรไฟล์ส่วนตัว
    ├── leave/               # คำขอลา
    ├── calendar/            # ปฏิทินลา (ทีม)
    ├── attendance/          # ลงเวลาเข้า-ออก
    ├── overtime/            # ล่วงเวลา (OT)
    ├── expenses/            # เบิกค่าใช้จ่าย
    ├── payslips/            # สลิปเงินเดือน
    ├── onboarding/          # งาน onboarding
    ├── approvals/           # รออนุมัติ (Manager/HR)
    ├── announcements/       # ประกาศ HR
    ├── notifications/       # กระดิ่งแจ้งเตือน
    ├── hr/                  # พนักงาน, วันหยุด, รายงาน
    └── shared/              # คอมโพเนนต์ร่วม (ตารางพนักงาน ฯลฯ)
```

---

## ฟีเจอร์หลักตามบทบาท

### บทบาทในระบบ (`UserRoles`)

| บทบาท | คำอธิบาย |
|--------|----------|
| **Employee** | พนักงานทั่วไป — ยื่นคำขอ ลงเวลา ดูสลิปของตนเอง |
| **Manager** | หัวหน้างาน — อนุมัติ/ปฏิเสธคำขอของลูกน้องในทีม |
| **Hr** | ทีม HR — จัดการพนักงาน วันหยุด รายงาน อัปโหลดสลิป ประกาศ onboarding |
| **Admin** | สิทธิ์ระดับสูง (รวมกับ HR ในหลาย endpoint) |

ผู้ที่ **อนุมัติได้**: Manager, Hr, Admin  
ผู้ที่ **เข้าเมนู HR ได้**: Hr, Admin

### พนักงาน (Employee)

| ฟีเจอร์ | รายละเอียด |
|---------|-------------|
| **แดชบอร์ด** | สรุปภาพรวมการใช้งาน |
| **โปรไฟล์** | ดู/แก้ข้อมูลส่วนตัว (`/api/me`) |
| **คำขอลา** | สร้างแบบร่าง → ส่งอนุมัติ → ติดตามสถานะ; แนบไฟล์ได้ |
| **โควต้าลา** | ดูยอดคงเหลือตามประเภทลา |
| **ปฏิทินลา** | ดูการลาของทีมตามช่วงวันที่ |
| **ลงเวลา** | Check-in / Check-out (เวลาไทย UTC+7); สรุปงานตอนออกได้ |
| **ล่วงเวลา (OT)** | ยื่นคำขอ OT workflow คล้ายลา |
| **เบิกค่าใช้จ่าย** | ยื่นเคลมค่าใช้จ่าย |
| **สลิปเงินเดือน** | ดาวน์โหลด PDF ที่ HR อัปโหลด |
| **Onboarding** | ทำ checklist งานต้อนรับพนักงานใหม่ |
| **ประกาศ** | อ่านประกาศจาก HR |
| **แจ้งเตือน** | กระดิ่งบนแถบด้านข้าง — อ่าน/อ่านทั้งหมด |

**กฎลงเวลา (ตัวอย่างในระบบ)**

- เวลาเข้างานมาตรฐาน: **09:00** (เขตเวลา Bangkok)
- เวลาออกงานมาตรฐาน: **18:00**
- สถานะ: ตรงเวลา / เข้าสาย / ออกก่อนเวลา / ยังไม่ลงเวลา

### หัวหน้างาน (Manager)

| ฟีเจอร์ | รายละเอียด |
|---------|-------------|
| **รออนุมัติ** | รวมคำขอลา, OT, เบิกค่าใช้จ่ายที่รอการอนุมัติ |
| **ลงเวลาทีม** | ดู attendance ของลูกน้อง (`scope=team`) |
| **ปฏิทินลา** | กรองตามแผนกได้ |

ใน seed มี Manager 3 คน: `mgr.eng` (วิศวกรรม), `mgr.sales` (ขาย), `mgr.hr` (HR)

### ทีม HR (Hr / Admin)

| ฟีเจอร์ | รายละเอียด |
|---------|-------------|
| **จัดการพนักงาน** | ดู/แก้โปรไฟล์พนักงาน |
| **วันหยุดบริษัท** | CRUD วันหยุดนักขัตฤกษ์/บริษัท |
| **รายงาน CSV** | สรุปการลาตามปี, รายงานลงเวลาตามช่วงวันที่ |
| **อัปโหลดสลิป** | PDF สลิปเงินเดือนต่อพนักงาน |
| **ประกาศ** | สร้าง/แก้ประกาศ HR |
| **Onboarding** | จัดการเทมเพลตและมอบหมายงาน |
| **โอนย้ายแผนก** | ประวัติการโอนย้าย (`employee-transfers`) |
| **แผนก / บทบาท** | API แผนกและ role definitions |

---

## โครงสร้างโปรเจกต์

```
Hr-Life/                          # ชื่อ repo (ผลิตภัณฑ์: HR-Lite)
├── frontend/                     # React + Vite
├── gateway/                      # YARP reverse proxy
├── src/                          # ASP.NET Clean Architecture
│   ├── NexusCore.Domain/
│   ├── NexusCore.Application/
│   ├── NexusCore.Infrastructure/
│   └── NexusCore.Api/
├── docs/                         # เอกสารสถาปัตยกรรม, API, DevOps
├── .github/workflows/            # CI
├── docker-compose.yml            # Stack Docker
├── Makefile                      # คำสั่ง dev รวม
├── HR-Lite.sln                   # Solution .NET
├── .env.example                  # ตัวอย่างตัวแปร Docker
└── README.md                     # ไฟล์นี้
```

### Entity หลักในฐานข้อมูล

| Entity | ความหมาย |
|--------|----------|
| `User` / `EmployeeProfile` | บัญชีและโปรไฟล์พนักงาน |
| `Department` | แผนก (ENG, SAL, HR ใน seed) |
| `LeaveRequest`, `LeaveType`, `LeaveEntitlement` | คำขอลา ประเภท โควต้า |
| `LeaveAttachment` | ไฟล์แนบคำขอลา |
| `AttendanceRecord` | บันทึกลงเวลารายวัน |
| `OvertimeRequest` | คำขอล่วงเวลา |
| `ExpenseClaim` | เบิกค่าใช้จ่าย |
| `Payslip` | สลิปเงินเดือน (ไฟล์) |
| `Announcement` | ประกาศ |
| `OnboardingTemplate` | เทมเพลต onboarding |
| `EmployeeTransfer` | ประวัติโอนย้าย |
| `CompanyHoliday` | วันหยุด |
| `AppNotification` | แจ้งเตือนในแอป |
| `RoleDefinition` | นิยามบทบาทในองค์กร |

ไฟล์อัปโหลดเก็บที่ `src/NexusCore.Api/uploads/` (หรือ volume ใน Docker)

---

## ความต้องการของระบบ

| รายการ | เวอร์ชันที่แนะนำ |
|--------|------------------|
| **.NET SDK** | 10 |
| **Node.js** | 20 ขึ้นไป |
| **npm** | มากับ Node |
| **Docker Desktop** | สำหรับ PostgreSQL และ `make docker-up` |
| **make** | Git for Windows หรือ Chocolatey (Windows) |

---

## เริ่มต้นใช้งาน

### 1) ติดตั้ง dependencies

```bash
make install
```

คำสั่งนี้จะ `dotnet restore` และ `npm install` ใน frontend

### 2) รันแบบพัฒนา (แนะนำ)

```bash
make dev
```

ลำดับที่เกิดขึ้น:

1. หยุด process ที่ค้างพอร์ต 5100 / 5000 / 5173 (`make stop`)
2. เปิด PostgreSQL ใน Docker (`make db-up`)
3. รัน backend, gateway, frontend พร้อมกัน

| บริการ | URL |
|--------|-----|
| **Frontend (เปิดในเบราว์เซอร์)** | http://localhost:5173 |
| Gateway | http://localhost:5000 |
| Backend | http://localhost:5100 |
| Swagger (ตรง backend) | http://localhost:5100/swagger |
| PostgreSQL | localhost:5432 |

> **Windows**: ถ้าต้องการ log แยกหน้าต่าง ใช้ `make dev-win` แทน

### 3) รันด้วย Docker (จำลอง production)

```bash
make stop          # ปิด dev ก่อน — อย่ารันคู่กับ make dev (พอร์ตชน)
make docker-up
```

| บริการ | URL |
|--------|-----|
| **Frontend** | http://localhost:8081 |
| Gateway (ถ้าต้องการ) | http://localhost:5000 |

คัดลอก [`.env.example`](.env.example) เป็น `.env` เพื่อเปลี่ยน `POSTGRES_PASSWORD` ได้ (อย่า commit `.env` ที่มีรหัสจริง)

### 4) รันเฉพาะ PostgreSQL

```bash
make db-up
make db-down
```

---

## บัญชีทดสอบ (Demo)

รหัสผ่านทุกบัญชี: **`password123`**

| Username | บทบาท | หมายเหตุ |
|----------|--------|----------|
| `admin` | Hr | บัญชี HR หลัก |
| `mgr.eng` | Manager | หัวหน้าแผนกวิศวกรรม (ENG) |
| `mgr.sales` | Manager | หัวหน้าแผนกขาย (SAL) |
| `mgr.hr` | Manager | หัวหน้าแผนก HR |
| `emp001` … `emp030` | Employee | พนักงาน mock 30 คน |

อีเมลตัวอย่าง: `{localPart}@hr-lite.local` เช่น `somchai.j@hr-lite.local` สำหรับ `emp001`

รายชื่อเต็มและโครงสร้างทีม:  
`src/NexusCore.Infrastructure/Persistence/SeedData/MockEmployeeSeed.cs`

---

## ฐานข้อมูลและ Migration

- ตอน **API เริ่มทำงาน** จะรัน `MigrateAsync()` อัตโนมัติ
- ถ้าแก้ entity / `AppDbContext` ต้องสร้าง migration ก่อน:

```powershell
dotnet ef migrations add <ชื่อMigration> `
  --project src/NexusCore.Infrastructure/NexusCore.Infrastructure.csproj `
  --startup-project src/NexusCore.Api/NexusCore.Api.csproj `
  --output-dir Persistence/Migrations

dotnet ef database update `
  --project src/NexusCore.Infrastructure/NexusCore.Infrastructure.csproj `
  --startup-project src/NexusCore.Api/NexusCore.Api.csproj
```

ถ้า model กับ migration ไม่ตรง แอปจะ error แบบ `PendingModelChangesWarning` — ต้อง add migration ให้ทัน

**Connection (ค่าเริ่มต้น local)**

- Host: `localhost`
- Port: `5432`
- Database / User: ดูใน `docker-compose.yml` หรือ `appsettings.Development.json`

---

## API และเอกสารเพิ่มเติม

Frontend เรียก API ผ่าน Gateway โดย prefix **`/api`**

### กลุ่ม endpoint สำคัญ

| กลุ่ม | Path ตัวอย่าง |
|-------|----------------|
| Auth | `POST /api/auth/login`, `POST /api/auth/change-password` |
| โปรไฟล์ | `GET/PUT /api/me` |
| ลา | `/api/leave-requests`, `/api/leave-balances`, `/api/leave-types` |
| ลงเวลา | `POST /api/attendance/check-in`, `check-out` |
| OT / เบิก | `/api/overtime-requests`, `/api/expense-claims` |
| แจ้งเตือน | `/api/notifications` |
| HR | `/api/employees`, `/api/holidays`, `/api/reports/...`, `/api/payslips` |
| อื่นๆ | `/api/announcements`, `/api/onboarding/...`, `/api/departments`, `/api/roles` |

**Workflow คำขอ (ลา / OT / เบิก)**  
สถานะทั่วไป: `Draft` → `Pending` → `Approved` / `Rejected` / `Cancelled`

เอกสาร API ฉบับเต็ม: **[docs/api.md](./docs/api.md)**

### เอกสารอื่นใน `docs/`

| ไฟล์ | เนื้อหา |
|------|---------|
| [docs/README.md](./docs/README.md) | ดัชนีเอกสารทั้งหมด |
| [architecture.md](./docs/architecture.md) | สถาปัตยกรรมและโฟลเดอร์ |
| [environments.md](./docs/environments.md) | Local vs Docker, พอร์ต, config |
| [devops-local-practice.md](./docs/devops-local-practice.md) | แผนฝึก DevOps บน laptop |
| [ci-github-actions.md](./docs/ci-github-actions.md) | CI pipeline |
| [deploy-checklist.md](./docs/deploy-checklist.md) | Checklist ก่อน/หลัง deploy |

---

## คำสั่ง Make ที่ใช้บ่อย

```bash
make help           # แสดงคำสั่งทั้งหมด
make install        # ติดตั้ง dependencies
make dev            # PostgreSQL + backend + gateway + frontend
make dev-win        # เหมือน dev แต่แยก 3 หน้าต่าง (Windows)
make stop           # ปิด process ที่ใช้พอร์ต dev
make build          # Build .NET + frontend
make lint           # ESLint frontend
make clean          # ลบ bin/obj/dist
make db-up          # เปิด PostgreSQL อย่างเดียว
make db-down        # ปิด PostgreSQL
make docker-up      # Build และรัน stack Docker
make docker-down    # หยุด stack Docker
make docker-logs    # ดู log container
make ci-local       # Build แบบเดียวกับ CI บนเครื่อง
make health         # ทด /health (ต้องรัน stack ก่อน)
```

---

## การพัฒนาและ CI

### Frontend

```bash
cd frontend
npm run dev              # พัฒนา — พอร์ต 5173
npm run build            # build production
npm run build:docker     # build สำหรับ Nginx ใน Docker
npm run lint
```

ตัวแปร `VITE_API_URL` ใน `.env.development` ชี้ไป `http://localhost:5000` (gateway)

### Backend / Gateway

```bash
make backend    # รัน API อย่างเดียว
make gateway    # รัน YARP อย่างเดียว
make frontend   # รัน Vite อย่างเดียว
```

### CI

- Push ไป GitHub จะ trigger workflow ตาม [docs/ci-github-actions.md](./docs/ci-github-actions.md)
- ทดบนเครื่องก่อน push: `make ci-local`

---

## ข้อจำกัดและขอบเขต

| หัวข้อ | สถานะ |
|--------|--------|
| อีเมล / SMS | ไม่มี — แจ้งเตือนในแอปเท่านั้น |
| Payroll คำนวณเงินเดือน | ไม่มี — มีเฉพาะอัปโหลด/ดาวน์โหลดสลิป PDF |
| Multi-tenant | ไม่ได้ออกแบบ — หนึ่งองค์กรต่อ instance |
| Performance review | ถูกถอดออกจาก codebase แล้ว (ไม่มีเมนู reviews) |

โปรเจกต์เหมาะสำหรับ **เรียนรู้ Fullstack**, **ฝึก Clean Architecture**, **ฝึก Docker/DevOps บนเครื่องตัวเอง** และเป็นฐานต่อยอดเป็นระบบ HR จริง

---

## สรุปเส้นทางสำหรับผู้เริ่มต้น

1. ติดตั้ง .NET 10, Node 20+, Docker, make  
2. `make install` → `make dev`  
3. เปิด http://localhost:5173  
4. Login ด้วย `emp001` / `password123` หรือ `mgr.eng` / `password123`  
5. ลองยื่นลา → login เป็น manager → อนุมัติที่เมนู **รออนุมัติ**  
6. อ่าน [docs/api.md](./docs/api.md) และ Swagger ที่พอร์ต 5100  

หากพบปัญหา port ชน ให้รัน `make stop` ก่อน แล้วเลือกรัน **อย่างใดอย่างหนึ่ง** ระหว่าง `make dev` กับ `make docker-up`

---

*HR-Lite — NexusCore Platform · เอกสารภาษาไทยฉบับนี้อัปเดตตามโครงสร้าง repo ปัจจุบัน*
