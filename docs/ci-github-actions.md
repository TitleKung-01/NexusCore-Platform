# CI — GitHub Actions

ไฟล์ workflow: `.github/workflows/ci.yml`

## ทำงานเมื่อไหร่

- Push ไป branch `main` หรือ `master`
- เปิด Pull Request เข้า `main` / `master`

## Job ทั้งหมด (รันขนาน)

```text
        ┌─────────────┐
        │ dotnet      │  dotnet build HR-Lite.sln
        └─────────────┘
        ┌─────────────┐
        │ frontend    │  npm ci, npm run build, npm run build:docker
        └─────────────┘
                │
                ▼
        ┌─────────────┐
        │ docker      │  build 3 images (ไม่ push ขึ้น registry)
        └─────────────┘
```

| Job | ฝึกทักษะ |
|-----|-----------|
| **dotnet** | compile backend + gateway |
| **frontend** | production + docker build ของ Vite |
| **docker** | Dockerfile ถูกต้อง, layer cache |

## รันบนเครื่องก่อน push

```bash
make ci-local
```

## ดูผลบน GitHub

1. ไปที่ repo → **Actions**
2. เลือก workflow run ล่าสุด
3. ถ้าแดง → คลิก job ที่ fail → อ่าน log

## ขั้นถัดไป (ฝึกเพิ่มเมื่อพร้อม)

ยังไม่ได้ทำใน repo นี้ — เป็นขั้นหลังฝึก:

1. **Push image ไป GHCR** หลัง build สำเร็จ
2. **Deploy** ขึ้น VPS ด้วย SSH + `docker compose pull`
3. **Branch protection** — บังคับ CI ผ่านก่อน merge

### ตัวอย่าง push image (อ้างอิง — ยังไม่เปิดใน workflow)

ต้องตั้ง GitHub Secrets / permissions `packages: write` แล้วเพิ่ม step:

```yaml
- uses: docker/login-action@v3
  with:
    registry: ghcr.io
    username: ${{ github.actor }}
    password: ${{ secrets.GITHUB_TOKEN }}

- uses: docker/build-push-action@v6
  with:
    push: true
    tags: ghcr.io/${{ github.repository }}/backend:latest
```

## ค่าใช้จ่าย

- Repo **public**: Actions ฟรี (มี quota)
- Repo **private**: มีนาทีฟรีต่อเดือน

ไม่ต้องเช่า server เพื่อใช้ CI แค่ build
