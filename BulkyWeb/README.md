### NuGet packages

- EntityFrameworkCore
- EntityFrameworkCore.SqlServer
- EntityFrameworkCore.Tools

### Create db (MySQL + entity framework)

- ApplicationDbContext.cs
- Program.cs (builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));)
- ConectionString in appsettings.json
NuGet packages -> NuGet shell -> update-database
