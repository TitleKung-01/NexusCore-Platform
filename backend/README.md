# Backend moved to Clean Architecture

โค้ด API ย้ายไปที่ **`src/`** แล้ว:

| เดิม (`backend/`) | ใหม่ |
|-------------------|------|
| `Models/` | `src/NexusCore.Domain/Entities/` |
| `Services/`, `DTOs/`, `Validators/` | `src/NexusCore.Application/` |
| `Data/` | `src/NexusCore.Infrastructure/Persistence/` |
| `Controllers/`, `Program.cs` | `src/NexusCore.Api/` |

ดู [src/README.md](../src/README.md) และ [docs/architecture.md](../docs/architecture.md)

```bash
make backend
dotnet run --project src/NexusCore.Api
```
