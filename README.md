# 🚀 Employee & Task Management System

<div align="center">

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![React](https://img.shields.io/badge/React-18-61DAFB.svg)
![TypeScript](https://img.shields.io/badge/TypeScript-5.0-3178C6.svg)
![SQLite](https://img.shields.io/badge/SQLite-Database-003B57.svg)

**A modern, full-stack web application for managing employees, projects, and tasks — built with .NET 8 and React TypeScript.**

🔗 **Live Demo**: [employee-and-task-management-system-ten.vercel.app](https://employee-and-task-management-system-ten.vercel.app)  
🔗 **Backend API**: [employee-and-task-management-system.onrender.com](https://employee-and-task-management-system.onrender.com/api/health)

</div>

---

## 📌 Project Overview

### The Problem

In many small-to-medium software development teams and organizations, task and project tracking is often handled through informal channels — spreadsheets, group chats, or disconnected tools. This creates several real challenges:

- **No clear accountability**: When tasks aren't formally assigned, it's hard to track who is responsible for what.
- **Lack of role-based visibility**: Managers need a bird's-eye view of all projects and tasks; employees only need to see their own assignments — most tools don't enforce this cleanly.
- **No centralized communication**: Progress updates and clarifications are scattered across chat threads, making it impossible to build a searchable audit trail per task.
- **Poor onboarding experience for new staff**: Administrators have to manually share login credentials outside the system, creating security risks.
- **No structured hierarchy**: Organizations with Admins, Managers, and Employees operate on different access levels, but most lightweight tools treat everyone the same.

### The Solution

The **Employee & Task Management System** solves these problems by providing a centralized, role-aware platform where:

- **Administrators** manage the full workforce — registering new staff, assigning system roles, organizing departments, and generating shareable credentials.
- **Managers** plan and oversee all project work — creating projects, breaking them down into tasks, and assigning each task to a specific developer based on their specialization.
- **Employees** have a personal task board showing only their own assignments — they can update progress in real time and leave comments directly on each task for transparent communication.

This creates a clean chain of accountability: every task is owned, tracked, and discussed in one place — with data protected by JWT-based authentication and enforced role-based access at the API level.

---

## ✨ Key Features

### 👨‍💼 Admin Role
- **Full Staff Management** — Register, edit, and deactivate employee accounts with role and job title assignments
- **Credential Sharing Card** — One-click copy of new employee login details to share securely
- **Department Management** — Create and manage organizational departments
- **System-Wide Overview** — Dashboard with total staff count, department breakdown, project count, and task statistics

### 🧑‍💼 Manager Role
- **Project Management** — Create and track projects with statuses (Planning → Active → Completed)
- **Task Assignment** — Assign tasks to specific employees with due dates, priorities (Low / Medium / High / Critical), and status tracking
- **Employee Roster** — Browse the full employee list with their roles and departments to make informed assignment decisions
- **Cross-Project Task View** — See all tasks across all projects at once

### 👩‍💻 Employee Role
- **Personal Task Board** — A clean view of all tasks assigned specifically to the logged-in user
- **Real-time Status Updates** — Change task status (Pending → In Progress → Completed / Cancelled) with a single click
- **Discussion Threads** — Post comments and progress updates directly on each assigned task, building a chronological activity log
- **Profile & Security** — Update job title and change password at any time through the profile modal

---

## 🛠️ Technology Stack

### Backend
| Technology | Purpose |
|---|---|
| **.NET 8 Web API** | REST API server and business logic layer |
| **Entity Framework Core 8** | ORM for database access and migrations |
| **SQLite** | Lightweight, self-contained relational database (no server required) |
| **JWT (JSON Web Tokens)** | Stateless authentication with role-based claims |
| **BCrypt.Net** | Secure password hashing |
| **Swagger / OpenAPI** | Interactive API documentation at `/swagger` |
| **Custom Exception Middleware** | Centralized error handling and consistent error responses |
| **Render.com** | Cloud hosting for the backend |

### Frontend
| Technology | Purpose |
|---|---|
| **React 18** | Component-based UI framework |
| **TypeScript** | Type-safe JavaScript for robust, maintainable code |
| **Vite** | Lightning-fast build tool and dev server |
| **Vanilla CSS** | Custom glassmorphism design system — no CSS frameworks |
| **SVG Icon System** | Inline custom icons — no external icon dependencies |
| **Vercel** | Cloud hosting and API proxy for the frontend |

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────┐
│                     FRONTEND (Vercel)                   │
│          React 18 + TypeScript + Vite                   │
│   Role-based SPA: Admin / Manager / Employee views      │
└──────────────────────┬──────────────────────────────────┘
                       │  HTTPS (REST API calls via Vercel proxy)
                       ▼
┌─────────────────────────────────────────────────────────┐
│                     BACKEND (Render)                    │
│          .NET 8 Web API + Entity Framework              │
│     JWT Auth │ CORS │ Role-Based Authorization          │
└──────────────────────┬──────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────┐
│                  SQLite Database                        │
│   Users │ Departments │ Projects │ Tasks │ Comments     │
└─────────────────────────────────────────────────────────┘
```

### Project Structure

```
Employee-and-Task-Management-System/
│
├── 📁 frontend/                   # React + TypeScript SPA
│   ├── src/
│   │   ├── App.tsx                # Main app: auth, routing, all role views
│   │   ├── main.tsx               # Entry point + production API URL patch
│   │   └── index.css              # Complete design system (glassmorphism)
│   ├── vite.config.ts             # Dev proxy → backend:5000
│   └── vercel.json                # Vercel rewrite rules
│
├── 📁 backend/                    # .NET 8 Web API
│   ├── Controllers/               # Auth, Users, Departments, Projects, Tasks, Comments
│   ├── Services/                  # Business logic layer (interfaces + implementations)
│   ├── Models/                    # EF Core entity models
│   ├── Dtos/                      # Request/Response data transfer objects
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── DbInitializer.cs       # Auto-migration + seed data on startup
│   ├── Authorization/             # Policy definitions
│   ├── Middleware/                # Global exception handling
│   └── Program.cs                 # App configuration and middleware pipeline
│
└── vercel.json                    # Root-level Vercel build + proxy config
```

---

## 🔑 Default Login Credentials

The backend automatically seeds the database with the following test accounts on first startup:

| Role | Name | Email | Password | Job Title |
|---|---|---|---|---|
| **ADMIN** | System Admin | `admin@example.com` | `AdminPass123!` | System Administrator |
| **MANAGER** | Project Manager | `manager@example.com` | `ManagerPass123!` | Engineering Lead |
| **EMPLOYEE** | Alex Rivera | `backend@example.com` | `EmployeePass123!` | Backend Developer |
| **EMPLOYEE** | Sarah Chen | `frontend@example.com` | `EmployeePass123!` | Frontend Developer |
| **EMPLOYEE** | David Kim | `mobile@example.com` | `EmployeePass123!` | Application Developer |

---

## 🚀 Local Development Setup

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js v18+](https://nodejs.org/)

### 1. Clone the Repository
```bash
git clone https://github.com/Misikerr/Employee-and-Task-Management-System.git
cd Employee-and-Task-Management-System
```

### 2. Start the Backend
```bash
cd backend
dotnet run
```
> The API starts at `http://localhost:5000` and automatically creates and seeds the SQLite database on first run.  
> Visit `http://localhost:5000/swagger` to explore all API endpoints interactively.

### 3. Start the Frontend
Open a second terminal:
```bash
cd frontend
npm install
npm run dev
```
> The React app starts at `http://localhost:5173` and proxies all `/api` requests to the backend.

---

## 🌐 Live Deployment

| Service | URL |
|---|---|
| **Frontend (Vercel)** | https://employee-and-task-management-system-ten.vercel.app |
| **Backend API (Render)** | https://employee-and-task-management-system.onrender.com |
| **API Health Check** | https://employee-and-task-management-system.onrender.com/api/health |
| **Swagger Docs** | https://employee-and-task-management-system.onrender.com/swagger |

> ⚠️ **Note:** The backend is hosted on Render's free tier, which may spin down after inactivity. The first request after a sleep period may take 30–60 seconds to respond while the server wakes up.

---

## 📡 API Reference (Key Endpoints)

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/api/auth/login` | Public | Authenticate and receive JWT token |
| `POST` | `/api/auth/register` | Public | Register a new user account |
| `GET` | `/api/users` | Admin/Manager | List all users |
| `GET` | `/api/users/employees` | Admin/Manager | List employees only (for task assignment) |
| `POST` | `/api/users` | Admin | Create a new user |
| `GET` | `/api/departments` | All roles | List all departments |
| `POST` | `/api/departments` | Admin | Create a department |
| `GET` | `/api/projects` | All roles | List all projects |
| `POST` | `/api/projects` | Manager/Admin | Create a project |
| `GET` | `/api/tasks/my-tasks` | Employee | Get tasks assigned to current user |
| `GET` | `/api/tasks/by-project/{id}` | Manager/Admin | Get tasks for a project |
| `POST` | `/api/tasks` | Manager/Admin | Create and assign a task |
| `PUT` | `/api/tasks/{id}` | All roles | Update task (status, details) |
| `GET` | `/api/tasks/{id}/comments` | All roles | Get comments on a task |
| `POST` | `/api/tasks/{id}/comments` | All roles | Post a comment on a task |

---

## 🔐 Security Design

- **Password Hashing**: All passwords stored using BCrypt with salt (never stored in plaintext)
- **JWT Authentication**: Tokens include user ID, name, email, and role claims; validated on every protected request
- **Role-Based Authorization**: API policies enforce `AdminOnly`, `ManagerOrAdmin`, and `EmployeeOrManagerOrAdmin` rules — not just UI-level hiding
- **CORS Policy**: Configured to accept requests from any origin in development; can be locked to specific domains in production

---

## 👨‍💻 Author

Built by **Misikir** as part of a software engineering internship evaluation project.

> This project demonstrates the ability to design and implement a complete, production-ready full-stack application — from database schema and secure REST API design, to a responsive, role-based React frontend — deployed to cloud platforms.
