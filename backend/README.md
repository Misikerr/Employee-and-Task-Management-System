# ⚙️ Backend API — Employee & Task Management System

High-performance RESTful Web API built with **.NET 8.0**, Entity Framework Core 8, SQLite, and JWT Authentication.

---

## 🔑 Pre-Seeded Accounts

On initial launch, `DbInitializer.cs` creates the database and populates these accounts:

- **Admin**: `admin@example.com` / `AdminPass123!`
- **Manager**: `manager@example.com` / `ManagerPass123!`
- **Backend Developer**: `backend@example.com` / `EmployeePass123!`
- **Frontend Developer**: `frontend@example.com` / `EmployeePass123!`
- **Application Developer**: `mobile@example.com` / `EmployeePass123!`

---

## 🌐 API Endpoint Reference

### Authentication (`/api/auth`)
- `POST /api/auth/login` — Authenticate user and receive JWT token
- `POST /api/auth/register` — Register a new account
- `POST /api/auth/change-password` — Change password for authenticated user
- `GET /api/auth/me` — Return current claims

### User Administration (`/api/users`)
- `GET /api/users` — Fetch all users (Admin/Manager)
- `GET /api/users/employees` — Fetch active EMPLOYEE-role users with Job Titles
- `GET /api/users/{id}` — Fetch user by ID
- `POST /api/users` — Register new staff with Job Role (Admin)
- `PUT /api/users/{id}` — Update user details or Job Title (Admin or Self)
- `DELETE /api/users/{id}` — Delete user (Admin)

### Department Management (`/api/departments`)
- `GET /api/departments` — List all departments
- `POST /api/departments` — Create new department (Admin)
- `DELETE /api/departments/{id}` — Delete department (Admin)

### Project Management (`/api/projects`)
- `GET /api/projects` — List all workspace projects
- `POST /api/projects` — Create project (Manager/Admin)
- `PUT /api/projects/{id}` — Update project status/details (Manager/Admin)
- `DELETE /api/projects/{id}` — Delete project (Manager/Admin)

### Task Items (`/api/tasks`)
- `GET /api/tasks/by-project/{projectId}` — List tasks in a project
- `GET /api/tasks/my-tasks` — List tasks assigned to logged-in employee
- `POST /api/tasks` — Assign task to specific employee (Manager/Admin)
- `PUT /api/tasks/{id}` — Update task details or status
- `DELETE /api/tasks/{id}` — Delete task (Manager/Admin)

### Task Discussion Comments (`/api/tasks/{taskId}/comments`)
- `GET /api/tasks/{taskId}/comments` — Retrieve discussion feed for a task
- `POST /api/tasks/{taskId}/comments` — Post comment on a task

---

## 🛠️ Local Commands

### Build & Run
```bash
dotnet build
dotnet run --urls "http://localhost:5000"
```

### Entity Framework Core Migrations
```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```
