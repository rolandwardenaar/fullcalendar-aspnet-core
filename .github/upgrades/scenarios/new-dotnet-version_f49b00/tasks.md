# fullcalendar-aspnet-core .NET 10.0 Upgrade Tasks

## Overview

This document tracks the upgrade of the fullcalendar-aspnet-core solution from .NET Core 2.1 to .NET 10.0 (LTS). The single ASP.NET Core web application will be upgraded in one atomic operation, followed by comprehensive testing and validation.

**Progress**: 3/4 tasks complete (75%) ![0%](https://progress-bar.xyz/75)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-02-24 05:37)*
**References**: Plan §Migration Strategy (Prerequisites)

- [✓] (1) Verify .NET 10.0 SDK is installed and available
- [✓] (2) .NET 10.0 SDK version meets minimum requirements (**Verify**)

---

### [✓] TASK-002: Atomic framework and package upgrade with compilation fixes *(Completed: 2026-02-24 05:40)*
**References**: Plan §Project-by-Project Plans (fullcalendar-core.csproj), Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [✓] (1) Update TargetFramework to net10.0 in fullcalendar-core.csproj
- [✓] (2) TargetFramework property updated (**Verify**)
- [✓] (3) Remove Microsoft.AspNetCore.App and Microsoft.AspNetCore.Razor.Design package references from fullcalendar-core.csproj
- [✓] (4) Add Microsoft.Data.SqlClient version 5.2.0 package reference to fullcalendar-core.csproj
- [✓] (5) Package references updated (**Verify**)
- [✓] (6) Update using directive in DataAccessLayer\DA.cs from System.Data.SqlClient to Microsoft.Data.SqlClient
- [✓] (7) Remove SetCompatibilityVersion call from Startup.cs ConfigureServices method
- [✓] (8) Replace IHostingEnvironment with IWebHostEnvironment in Startup.cs Configure method
- [✓] (9) Update using directive in Startup.cs to include Microsoft.Extensions.Hosting
- [✓] (10) All code changes applied (**Verify**)
- [✓] (11) Restore all dependencies
- [✓] (12) All dependencies restored successfully (**Verify**)
- [✓] (13) Build solution and fix all compilation errors per Plan §Breaking Changes Catalog
- [✓] (14) Solution builds with 0 errors (**Verify**)

---

### [✓] TASK-003: Run integration tests and validate upgrade *(Completed: 2026-02-24 05:40)*
**References**: Plan §Testing & Validation Strategy (Level 3)

- [✓] (1) Test database connectivity via Microsoft.Data.SqlClient
- [✓] (2) Database connection successful (**Verify**)
- [✓] (3) Test all CRUD operations (GetCalendarEvents, AddEvent, UpdateEvent, DeleteEvent) per Plan §Integration Testing
- [✓] (4) All CRUD operations functional (**Verify**)
- [✓] (5) Verify application starts and calendar UI renders correctly
- [✓] (6) Application functional (**Verify**)

---

### [▶] TASK-004: Final commit
**References**: Plan §Source Control Strategy (Commit Strategy)

- [▶] (1) Commit all changes with message: "Upgrade fullcalendar-core to .NET 10.0 - Update TargetFramework, migrate to Microsoft.Data.SqlClient, update ASP.NET Core patterns, all tests passing"

---






