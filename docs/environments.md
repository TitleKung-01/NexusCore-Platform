# Local vs Docker

> แผนฝึก DevOps แบบไม่เช่า server: [devops-local-practice.md](./devops-local-practice.md)

## Gateway → Backend

| Environment | When | File | Backend |
|-------------|------|------|---------|
| **Development** | `make dev`, `dotnet run` | `gateway/appsettings.Development.json` | `http://localhost:5100` |
| **Production** | `make docker-up` | `gateway/appsettings.Production.json` | `http://backend-service:5100` |

Shared routes (rate limit, `/api` path): `gateway/appsettings.json`

## Frontend → API

| Mode | Config | How API works |
|------|--------|----------------|
| **Local** | `.env.development` | Browser → `http://localhost:5000/api/...` |
| **Docker** | `.env.docker` + `nginx.conf` | Browser → `http://localhost:8080/api/...` → Nginx → gateway |

## Health checks

| Service | URL |
|---------|-----|
| Backend | http://localhost:5100/health |
| Gateway | http://localhost:5000/health |

```bash
make health   # หลัง make dev หรือ docker-up (gateway/backend บน host)
```

## URLs

### Local — `make dev`

| Service | URL |
|---------|-----|
| Frontend (Vite) | http://localhost:5173 |
| Gateway | http://localhost:5000 |
| Backend | http://localhost:5100 |

### Docker — `make docker-up`

| Service | Open in browser |
|---------|-----------------|
| **Frontend** | **http://localhost:8080** |
| Gateway (optional) | http://localhost:5000 |
| Backend | internal only |

## Commands

```bash
make dev
make docker-up
make docker-down
make docker-logs
make ci-local
make health
```

Do not run `make dev` and `make docker-up` at the same time (port 5000 conflict).

## Frontend build modes

```bash
npm run dev           # local, port 5173
npm run build         # uses .env.production
npm run build:docker  # uses .env.docker (empty base URL for Nginx)
```
