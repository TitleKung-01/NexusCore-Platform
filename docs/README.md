# NexusCore Platform — Documentation

เอกสารสำหรับพัฒนาและฝึก DevOps บนเครื่องตัวเอง (ไม่ต้องเช่า server)

| เอกสาร | เนื้อหา |
|--------|---------|
| [architecture.md](./architecture.md) | โครงสร้าง repo — Layered backend, feature folders frontend |
| [environments.md](./environments.md) | แยก Local vs Docker — พอร์ต, config files |
| [devops-local-practice.md](./devops-local-practice.md) | **แผนฝึก DevOps ฟรีบน laptop** (หลัก) |
| [ci-github-actions.md](./ci-github-actions.md) | CI pipeline บน GitHub Actions |
| [deploy-checklist.md](./deploy-checklist.md) | Checklist ก่อน/หลัง `make docker-up` |
| [api.md](./api.md) | REST API, Swagger, EF Core, JWT |

## คำสั่งด่วน

```bash
make dev          # Local: Vite + Gateway + Backend
make docker-up    # Docker: http://localhost:8081
make ci-local     # Build เหมือน CI บนเครื่อง
make health       # ทด /health (ต้องรัน stack ก่อน)
```

ดูรายละเอียดทุก target: `make help`
