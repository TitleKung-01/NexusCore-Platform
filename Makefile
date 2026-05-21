# NexusCore-Platform — common dev commands
# Requires: make, dotnet SDK, Node.js + npm

ifeq ($(OS),Windows_NT)
SHELL := cmd.exe
.SHELLFLAGS := /c
endif

ROOT        := $(CURDIR)
SLN         := NexusCore-Platform.sln
BACKEND     := backend
GATEWAY     := gateway
FRONTEND    := frontend

BACKEND_PORT  := 5100
GATEWAY_PORT  := 5000
FRONTEND_PORT := 5173
DOCKER_FRONTEND_PORT := 8081

DOTNET := dotnet
NPM    := npm
COMPOSE := docker compose

.PHONY: help install restore build ci-local health run backend gateway frontend lint clean stop dev dev-win docker-up docker-down docker-logs

.DEFAULT_GOAL := help

help:
	@echo NexusCore-Platform
	@echo.
	@echo Local (make dev):
	@echo   backend  - http://localhost:$(BACKEND_PORT)
	@echo   gateway  - http://localhost:$(GATEWAY_PORT)
	@echo   frontend - http://localhost:$(FRONTEND_PORT)
	@echo.
	@echo Docker (make docker-up):
	@echo   frontend - http://localhost:$(DOCKER_FRONTEND_PORT)  (open this in browser)
	@echo   gateway  - http://localhost:$(GATEWAY_PORT)  (optional)
	@echo.
	@echo Targets:
	@echo   make install      Install dotnet + npm dependencies
	@echo   make build        Build solution and frontend
	@echo   make backend      Run ASP.NET backend
	@echo   make gateway      Run YARP gateway
	@echo   make frontend     Run Vite dev server
	@echo   make lint         ESLint frontend
	@echo   make clean        Remove bin, obj, dist
	@echo   make stop         Free ports $(BACKEND_PORT), $(GATEWAY_PORT), $(FRONTEND_PORT)
	@echo   make dev          Local: run all services (one terminal)
	@echo   make dev-win      Local: 3 separate cmd windows
	@echo   make docker-up    Docker: build and start stack
	@echo   make docker-down  Docker: stop stack
	@echo   make docker-logs  Docker: follow container logs
	@echo   make ci-local     Same checks as GitHub Actions (local)
	@echo   make health       Ping /health (stack must be running)
	@echo   Docs: docs/README.md

install: restore frontend-install

restore:
	$(DOTNET) restore "$(ROOT)\$(SLN)"

frontend-install:
	cd /d "$(ROOT)\$(FRONTEND)" && $(NPM) install

build:
	$(DOTNET) build "$(ROOT)\$(SLN)"
	cd /d "$(ROOT)\$(FRONTEND)" && $(NPM) run build

ci-local: build
	cd /d "$(ROOT)\$(FRONTEND)" && $(NPM) run build:docker
	@echo ci-local OK - push to GitHub to run full CI + Docker on Actions

health:
	@echo Backend:
	@curl -sf http://localhost:$(BACKEND_PORT)/health || echo backend not reachable
	@echo.
	@echo Gateway:
	@curl -sf http://localhost:$(GATEWAY_PORT)/health || echo gateway not reachable

run: backend

backend:
	cd /d "$(ROOT)\$(BACKEND)" && $(DOTNET) run

gateway:
	cd /d "$(ROOT)\$(GATEWAY)" && $(DOTNET) run

frontend:
	cd /d "$(ROOT)\$(FRONTEND)" && $(NPM) run dev

lint:
	cd /d "$(ROOT)\$(FRONTEND)" && $(NPM) run lint

clean:
	-$(DOTNET) clean "$(ROOT)\$(SLN)" -v q
	-if exist "$(ROOT)\$(BACKEND)\bin" rmdir /s /q "$(ROOT)\$(BACKEND)\bin"
	-if exist "$(ROOT)\$(BACKEND)\obj" rmdir /s /q "$(ROOT)\$(BACKEND)\obj"
	-if exist "$(ROOT)\$(GATEWAY)\bin" rmdir /s /q "$(ROOT)\$(GATEWAY)\bin"
	-if exist "$(ROOT)\$(GATEWAY)\obj" rmdir /s /q "$(ROOT)\$(GATEWAY)\obj"
	-if exist "$(ROOT)\$(FRONTEND)\dist" rmdir /s /q "$(ROOT)\$(FRONTEND)\dist"

# Free dev ports when a previous make dev / dotnet run is still running
stop:
ifeq ($(OS),Windows_NT)
	@echo Stopping processes on ports $(BACKEND_PORT), $(GATEWAY_PORT), $(FRONTEND_PORT)...
	-for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":%$(BACKEND_PORT) " ^| findstr LISTENING') do @taskkill /F /PID %%a >nul 2>&1
	-for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":%$(GATEWAY_PORT) " ^| findstr LISTENING') do @taskkill /F /PID %%a >nul 2>&1
	-for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":%$(FRONTEND_PORT) " ^| findstr LISTENING') do @taskkill /F /PID %%a >nul 2>&1
else
	@for p in $(BACKEND_PORT) $(GATEWAY_PORT) $(FRONTEND_PORT); do \
		pid=$$(lsof -ti tcp:$$p 2>/dev/null); \
		[ -n "$$pid" ] && kill -9 $$pid || true; \
	done
endif

dev: stop
	@echo Starting backend :$(BACKEND_PORT), gateway :$(GATEWAY_PORT), frontend :$(FRONTEND_PORT)
	@echo One terminal - logs may mix. Press Ctrl+C to stop all.
	$(MAKE) --no-print-directory -j3 backend gateway frontend

# /D sets working dir — avoids broken cd quoting with spaces in path
dev-win: stop
ifeq ($(OS),Windows_NT)
	cmd /c start "NexusCore Backend" /D "$(CURDIR)" cmd /k make backend
	cmd /c start "NexusCore Gateway" /D "$(CURDIR)" cmd /k make gateway
	cmd /c start "NexusCore Frontend" /D "$(CURDIR)" cmd /k make frontend
else
	@echo dev-win is Windows only. Use: make dev
endif

docker-up:
	$(COMPOSE) up --build -d
	@echo.
	@echo Docker stack running:
	@echo   Frontend  http://localhost:$(DOCKER_FRONTEND_PORT)
	@echo   Gateway   http://localhost:$(GATEWAY_PORT)

docker-down:
	$(COMPOSE) down

docker-logs:
	$(COMPOSE) logs -f
