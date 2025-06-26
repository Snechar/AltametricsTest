# Altametrics Backend (.NET 8)

This project is a backend system built with ASP.NET Core (.NET 8), designed to manage event RSVPs, user authentication via JWT, and includes auditing and protection features such as rate limiting. The solution also supports robust integration testing using an in-memory database.

# Important notes

-Due to an ongoing issue in Swagger UI (see https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/3329), I was not able to add annotations to the API calls, as it causes cache problems.

-Using the **Test Explorer** window does not work due to the testing platform automatically targeting net9, manually adjust or run using
```bash
dotnet test
```
.


##  Tech Stack

- ASP.NET Core 8
- Entity Framework Core
- PostgreSQL (production) / InMemory (testing)
- JWT Authentication
- AutoMapper
- Swagger UI (OpenAPI)
- MSTest (integration testing)
- AspNetCoreRateLimit (optional)

---

##  Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/altametrics-backend.git
cd "Altametrics Backend C# .NET"
```

### 2. Set Up Environment Variables

Create a `.env` file in the root of the project with the following content:

```env
DB_HOST=localhost
DB_PORT=5432
DB_NAME=event_api
DB_USER=youruser
DB_PASSWORD=yourpassword

JWT_KEY=TestSecretKeyThatIsLongEnough123!
JWT_ISSUER=Altametrics
JWT_AUDIENCE=User
```

>  Make sure PostgreSQL is running and the database exists or let the application create it (might fail on Linux/Mac).

---

##  Running the Application

### Run via CLI

```bash
dotnet run --project "Altametrics Backend C# .NET"
```

### Run via Visual Studio

1. Set `"Altametrics Backend C# .NET"` as the startup project.
2. Press `F5` or click `Run`.

---

##  API Documentation

Swagger is enabled in development. You can access it at:

```
https://localhost:44314/swagger
```

Use the JWT token generated via login (or seed one manually) to authorize protected endpoints.

---

##  Running Tests

### Prerequisites

Ensure `.env` is **not required** for running tests — the `TestApplicationFactory` handles in-memory DB setup.

### Run All Tests (CLI)

```bash
dotnet test
```

### Or run from Visual Studio

Using the **Test Explorer** window does not work due to the testing platform automatically targeting .net9.

---

##  Features

-  JWT-secured user and event management
-  RSVP creation, update, and deletion
-  Audit logging
-  Rate limiting (optional)
-  Environment-aware configuration
-  In-memory integration test setup

---

##  Seeding Test Data

The `TestApplicationFactory` seeds:

- One test user
- Multiple events
- Several RSVPs

This allows integration tests to simulate realistic workflows.

---

##  Folder Structure

```
.
├── Controllers/
├── Models/
│   └── Entities/
├── Data/
│   └── AppDbContext.cs
├── Services/
│   └── AuditLoggerService.cs
├── Database/
│   └── DBInitializer.cs
├── Altametrics.Tests/
│   └── TestApplicationFactory.cs
│   └── RSVPTests.cs
│   └── EventTests.cs
├── Program.cs
├── appsettings.json
├── .env
```

---

##  Notes

- Make sure your environment variables are loaded correctly (e.g., via `DotNetEnv`).


---
