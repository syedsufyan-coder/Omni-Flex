# OMNI-FLEX LMS — QUICK REFERENCE GUIDE

## Project Structure at a Glance

```
OmniFlex/
├── Controllers/          ← HTTP request handlers (3 files)
├── Models/
│   ├── Domain/          ← Database entity models (18 files)
│   ├── ViewModels/      ← UI-specific models (6 files)
│   └── Repositories/    ← Data access layer (18 files + 9 interfaces)
├── Infrastructure/      ← Database connection factory
├── Views/               ← MVC views (Home, Auth, Admin, Shared)
├── wwwroot/             ← CSS, JS, fonts (untouched)
├── Program.cs           ← Startup configuration
└── appsettings.json     ← Configuration & connection strings
```

---

## Key Files to Understand

### Program.cs
- MVC routing: `MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}")`
- Repository registration (9 scoped services)
- DbConnectionFactory singleton

### Controllers
- **HomeController** → `GET /Home/Index`
- **AuthController** → `GET/POST /Auth/Login`
- **AdminController** → `GET /Admin/{action}` (Dashboard, Courses, Sections, Users, etc.)

### Views
- `_Layout.cshtml` → Root layout with navbar
- `_AdminLayout.cshtml` → Admin layout with sidebar + navbar
- Action views use `Layout = "_AdminLayout";` or `Layout = "_Layout";`

### Repositories
Each follows the pattern:
```csharp
public async Task<Model?> GetByIdAsync(string id)
{
    using var conn = _factory.CreateConnection();
    return await conn.QueryFirstOrDefaultAsync<Model>(
        "SELECT * FROM TABLE WHERE ID = :Id",
        new { Id = id });
}
```

---

## Common Tasks

### 1. Add a New Domain Model
Create file in `Models/Domain/YourModel.cs`:
```csharp
namespace OmniFlex.Models.Domain
{
    public class YourModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
```

### 2. Create a Repository for New Model
Create interface `Models/Repositories/IYourRepository.cs`:
```csharp
public interface IYourRepository
{
    Task<YourModel?> GetByIdAsync(string id);
    Task<int> CreateAsync(YourModel model);
    // ... etc
}
```

Create implementation `Models/Repositories/YourRepository.cs`:
```csharp
public class YourRepository : IYourRepository
{
    private readonly DbConnectionFactory _factory;
    public YourRepository(DbConnectionFactory factory) => _factory = factory;
    
    public async Task<YourModel?> GetByIdAsync(string id)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<YourModel>(
            "SELECT * FROM YOUR_TABLE WHERE ID = :Id",
            new { Id = id });
    }
}
```

Register in `Program.cs`:
```csharp
builder.Services.AddScoped<IYourRepository, YourRepository>();
```

### 3. Add a ViewModel
Create file in `Models/ViewModels/YourViewModel.cs`:
```csharp
namespace OmniFlex.Models.ViewModels
{
    public class YourViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        // UI-shaped properties
    }
}
```

### 4. Add a Controller Action
In `Controllers/YourController.cs`:
```csharp
public class YourController : Controller
{
    private readonly IYourRepository _repo;
    
    public YourController(IYourRepository repo)
        => _repo = repo;
    
    public async Task<IActionResult> Index()
    {
        var models = await _repo.GetAllAsync();
        var viewModels = models.Select(m => new YourViewModel 
        { 
            Id = m.Id,
            Name = m.Name
        }).ToList();
        return View(viewModels);
    }
}
```

### 5. Create a View
Create file in `Views/YourController/Index.cshtml`:
```html
@model List<YourViewModel>

@{
    Layout = "_Layout";
}

<div class="container">
    <h1>Your Page</h1>
    @foreach(var item in Model)
    {
        <div>@item.Name</div>
    }
</div>
```

---

## Important Patterns

### Oracle Parameter Syntax
✅ Correct:
```csharp
"SELECT * FROM USERS WHERE ID = :UserId"
new { UserId = userId }
```

❌ Wrong (SQL Server style):
```csharp
"SELECT * FROM USERS WHERE ID = @UserId"
```

### Async/Await Pattern
```csharp
public async Task<int> CreateAsync(Model model)
{
    using var conn = _factory.CreateConnection();
    return await conn.ExecuteAsync(
        "INSERT INTO TABLE (ID, NAME) VALUES (:Id, :Name)",
        model);
}
```

### View to Controller
In View:
```html
<a asp-controller="Admin" asp-action="Dashboard">Dashboard</a>
```

Generates: `/Admin/Dashboard`

### Dependency Injection
Constructor injection:
```csharp
public class MyController : Controller
{
    private readonly IUserRepository _userRepo;
    
    public MyController(IUserRepository userRepo)
        => _userRepo = userRepo;
}
```

---

## Database Connection

Update `appsettings.json`:
```json
{
    "ConnectionStrings": {
        "OracleDb": "User Id=YOUR_USER;Password=YOUR_PASS;Data Source=YOUR_HOST:YOUR_PORT/YOUR_SID;"
    }
}
```

Example:
```json
{
    "ConnectionStrings": {
        "OracleDb": "User Id=admin;Password=myPassword123;Data Source=localhost:1521/XEPDB1;"
    }
}
```

---

## Useful Dapper Methods

### Query (returns IEnumerable<T>)
```csharp
var users = await conn.QueryAsync<User>(
    "SELECT * FROM USERS WHERE ROLE = :Role",
    new { Role = "admin" });
```

### QueryFirstOrDefaultAsync (returns T?)
```csharp
var user = await conn.QueryFirstOrDefaultAsync<User>(
    "SELECT * FROM USERS WHERE ID = :Id",
    new { Id = userId });
```

### ExecuteAsync (INSERT/UPDATE/DELETE)
```csharp
int rowsAffected = await conn.ExecuteAsync(
    "DELETE FROM USERS WHERE ID = :Id",
    new { Id = userId });
```

### ExecuteScalarAsync (COUNT, SUM, etc.)
```csharp
int count = await conn.ExecuteScalarAsync<int>(
    "SELECT COUNT(*) FROM USERS WHERE ROLE = :Role",
    new { Role = "student" });
```

---

## Testing a Repository

```csharp
[Fact]
public async Task GetByIdAsync_ReturnsUser_WhenUserExists()
{
    // Arrange
    var repo = new UserRepository(new DbConnectionFactory(config));
    
    // Act
    var user = await repo.GetByIdAsync("U001");
    
    // Assert
    Assert.NotNull(user);
    Assert.Equal("U001", user.UserId);
}
```

---

## Common Issues & Solutions

### Issue: No rows returned
- Check Oracle table name casing (USERS vs Users)
- Verify column names match property names
- Check connection string in appsettings.json

### Issue: Parameter not recognized
- Use `:ParamName` for Oracle (not `@ParamName`)
- Parameter name must match object property name (case-sensitive)

### Issue: Null reference exception
- Verify repository is registered in Program.cs
- Check constructor injection is correct
- Ensure connection string exists

### Issue: View not found
- Check controller name matches folder name (e.g., Admin → Views/Admin/)
- Verify .cshtml file exists and has correct name
- Check _ViewStart.cshtml references correct Layout

---

## Build & Run

```bash
# Build
dotnet build

# Run with watch (auto-reload on changes)
dotnet watch

# Run tests
dotnet test

# Publish to production
dotnet publish -c Release -o ./publish
```

---

## File Naming Conventions

- **Controllers:** `{Name}Controller.cs` (not plural)
  - `UserController.cs` ✓
  - `UsersController.cs` ✗

- **Views:** Match controller name
  - Controller: `UserController`
  - Folder: `Views/User/`
  - View: `Views/User/Index.cshtml`

- **ViewModels:** `{Name}ViewModel.cs`
  - `UserViewModel.cs` ✓

- **Domain Models:** `{Name}.cs` (match table name)
  - `User.cs` → USERS table ✓

- **Repositories:** `I{Name}Repository.cs` + `{Name}Repository.cs`
  - `IUserRepository.cs` ✓
  - `UserRepository.cs` ✓

---

## Next Phase Checklist

- [ ] Implement authentication in AuthController
- [ ] Add BCrypt password hashing in login
- [ ] Implement controller logic for CRUD operations
- [ ] Populate views with data binding
- [ ] Add form validation (client + server)
- [ ] Test database connectivity
- [ ] Add error handling & logging
- [ ] Implement role-based authorization
- [ ] Add unit tests for repositories
- [ ] Delete legacy Pages/ folder

---

## Quick Links

- **Dapper Docs:** https://github.com/DapperLib/Dapper
- **ASP.NET Core MVC:** https://docs.microsoft.com/aspnet/core/mvc
- **BCrypt.Net:** https://github.com/BcryptNet/bcrypt.net
- **Oracle.ManagedDataAccess:** https://www.oracle.com/database/technologies/net/

---

*Last Updated: March 12, 2026*
*Build Status: ✅ Clean (0 Warnings, 0 Errors)*
