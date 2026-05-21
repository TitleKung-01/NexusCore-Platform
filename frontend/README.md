# NexusCore Frontend — React + TypeScript + shadcn/ui

## Stack

- Vite 8 + React 19 + TypeScript
- Tailwind CSS v4 + shadcn/ui (new-york)
- React Router, Axios, Sonner toasts

## Structure

```
src/
  app/App.tsx           # routes
  api.ts                # Axios + JWT
  components/ui/        # shadcn primitives
  components/layout/    # AppShell sidebar
  features/
    auth/               # login, AuthProvider
    dashboard/
    profile/
    leave/
    approvals/
```

## API

| Mode | Config |
|------|--------|
| Local | `VITE_API_URL=http://localhost:5000` |
| Docker | empty base URL → `/api` via Nginx |

## Commands

```bash
npm install
npm run dev
npm run build
npm run build:docker
```
