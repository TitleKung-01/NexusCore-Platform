# Local vs Docker

> แผนฝึก DevOps แบบไม่เช่า server: [devops-local-practice.md](./devops-local-practice.md)

## Stack overview

| Service | Local `make dev` | Docker `make docker-up` |
|---------|------------------|-------------------------|
| PostgreSQL | `localhost:5432` (`make db-up`) | `postgres` container |
| Backend (REST API) | `localhost:5100` | `backend-service` (internal) |
| Gateway (YARP) | `localhost:5000` | `gateway-service` :5000 |
| Frontend | Vite `localhost:5173` | Nginx `localhost:8081` |

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
| **Docker** | `.env.docker` + `nginx.conf` | Browser → `http://localhost:8081/api/...` → Nginx → gateway |

## Health checks

| Service | URL |
|---------|-----|
| Backend | http://localhost:5100/health |
| Gateway | http://localhost:5000/health |
| Swagger (dev, backend direct) | http://localhost:5100/swagger |

```bash
make health   # หลัง make dev หรือ docker-up (gateway/backend บน host)
```

## URLs

### Local — `make dev`

| Service | URL |
|---------|-----|
| PostgreSQL | localhost:5432 |
| Frontend (Vite) | http://localhost:5173 |
| Gateway | http://localhost:5000 |
| Backend | http://localhost:5100 |

### Docker — `make docker-up`

| Service | Open in browser |
|---------|-----------------|
| **Frontend** | **http://localhost:8081** |
| Gateway (optional) | http://localhost:5000 |
| Backend | internal only |
| PostgreSQL | localhost:5432 (exposed for tools) |

## Commands

```bash
make db-up        # PostgreSQL only
make db-down
make dev          # db-up + backend + gateway + frontend
make docker-up
make docker-down
make docker-logs
make ci-local
make health
```

Do not run `make dev` and `make docker-up` at the same time (ports **5000** and **5432** conflict).

## Frontend build modes

```bash
npm run dev           # local, port 5173
npm run build         # uses .env.production
npm run build:docker  # uses .env.docker (empty base URL for Nginx)
```
