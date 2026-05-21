# Examples (ASP.NET Core + React — NexusCore)

## Layered — new “Products” in `backend/`

```
backend/Controllers/ProductsController.cs
backend/Services/IProductService.cs
backend/Services/ProductService.cs
backend/DTOs/ProductDtos.cs
backend/Validators/CreateProductRequestValidator.cs
backend/Models/Product.cs
backend/Data/AppDbContext.cs    # DbSet<Product>
```

```csharp
[ApiController]
[Route("api/products")]
public class ProductsController(IProductService products) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> List(CancellationToken ct)
        => Ok(await products.ListAsync(ct));
}
```

`Program.cs`: `builder.Services.AddScoped<IProductService, ProductService>();`

---

## Clean — same feature

**Domain:** `Entities/Product.cs`, `Interfaces/IProductRepository.cs`

**Application:** `Products/Queries/ListProductsQuery.cs`, handler, `ProductDto` record

**Infrastructure:** `ProductRepository` + EF configuration

**Api:** controller → handler only (no `DbContext`)

---

## Microservices — split auth vs users

**Before:** single `backend/` DB with Users + Auth.

**After:**

```
services/auth-service/
services/users-service/
gateway/                  # /api/auth, /api/users clusters
```

1. Move auth controllers/services/tables → `auth-service`.
2. Move user profile CRUD → `users-service`.
3. Users service validates JWT (shared issuer from config) — no auth DB access.
4. Update `gateway/appsettings.json` routes.
5. Separate migrations per service database.

---

## Layered → Clean (incremental)

1. Add `NexusCore.Domain` — move `backend/Models/*` → `Entities/`.
2. Add `NexusCore.Application` — move `IUserService` / logic from `Services/`.
3. Add `NexusCore.Infrastructure` — `AppDbContext`, migrations.
4. Rename host to `NexusCore.Api` — keep Swagger/JWT in `Program.cs` here only.

---

## React — `frontend/src/features/auth/`

**Before:** all logic in `App.jsx`.

**After:**

```
frontend/src/features/auth/useAuth.js
frontend/src/features/auth/LoginForm.jsx
frontend/src/api.js
```

`App.jsx` = routes + layout only. API base URL follows existing `api.js` / Vite proxy.

---

## Anti-patterns

| Smell | Fix |
|-------|-----|
| Controller injects `AppDbContext` | Use `I*Service` or repository |
| EF package reference in Domain | Infrastructure only |
| Business logic in `gateway/Program.cs` | Move to `backend/` or service |
| `frontend` calls backend port directly in Docker | Call gateway (`:8081`) |
| Suggest Express/Spring structure | Use tables in structures.md (.NET/React only) |
