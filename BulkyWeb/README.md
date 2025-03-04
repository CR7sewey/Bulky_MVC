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

### Create table (MySQL + entity framework)

- ApplicationDbContext.cs
NuGet packages -> NuGet shell -> add-migration __name__
NuGet packages -> NuGet shell -> update-database

### Identity

- NuGet packages -> Microsoft.AspNetCore.Identity.EntityFrameworkCore
- tables/entities? -> on migrationm, tables are created automatically