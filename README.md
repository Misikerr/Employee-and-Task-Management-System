# 🚀 Employee & Task Management System

A full-stack, enterprise-grade **Employee & Task Management System** built with **.NET 8 Web API** (Backend) and **React + TypeScript + Vite** (Frontend). Designed with a modern, glassmorphic UI, role-based security (JWT), and specialized task assignment workflows by developer roles.

---

## 🔑 Default Login Credentials

The application automatically seeds a default database on startup with the following user accounts:

| System Role | Job Role / Specialization | Email Address | Password |
|---|---|---|---|
| **ADMIN** | System Administrator | `admin@example.com` | `AdminPass123!` |
| **MANAGER** | Engineering Lead | `manager@example.com` | `ManagerPass123!` |
| **EMPLOYEE** | Backend Developer | `backend@example.com` | `EmployeePass123!` |
| **EMPLOYEE** | Frontend Developer | `frontend@example.com` | `EmployeePass123!` |
| **EMPLOYEE** | Application Developer | `mobile@example.com` | `EmployeePass123!` |

---

## ✨ Key Features

### 👨‍💼 Administrator & Manager Features
- **Staff Registration & Job Role Selection**: Register new staff with specific developer roles (**Backend Developer**, **Frontend Developer**, **Application Developer**, or type a custom role).
- **Share Credentials Card**: Instantly view and one-click copy generated employee login credentials to share with newly created staff.
- **Active Developer Roster**: View available employees as interactive cards showing their avatar, ID `#`, job role, department, and current task load.
- **Specific Task Assignment**: Assign tasks specifically to individual developers based on their role and department.
- **Project & Department Management**: Organize work items by departments and project milestones.

### 👩‍💻 Employee Features
- **Personal Task Board ("My Tasks")**: Review assignments allocated specifically to you, update task status (Pending, In Progress, Completed), and view due dates.
- **Discussion Feed**: Post progress updates, questions, and comments directly on assigned task items.
- **Self Profile & Security**: Update your own job role and change your password anytime via the profile modal.

---

## 🛠️ Technology Stack

- **Backend**: .NET 8 Web API, Entity Framework Core 8, SQLite Database, BCrypt.Net Password Hashing, JWT Authentication, Custom Exception Handling Middleware.
- **Frontend**: React 18, TypeScript, Vite, Modern Glassmorphism Vanilla CSS, SVG Icon system.

---

## 🚀 Quick Start Guide (Local Setup)

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js (v18+)](https://nodejs.org/)

### 1. Start the Backend API
```bash
cd backend
dotnet build
dotnet run --urls "http://localhost:5000"
```
> The API will run locally at `http://localhost:5000` and automatically create/seed `employee_task_mgmt.db`.

### 2. Start the Frontend UI
Open a second terminal window:
```bash
cd frontend
npm install
npm run dev
```
> The React app will run at `http://localhost:5173`.

---

## 🌐 Online Deployment Guide

### Deploying Frontend (Vercel / Netlify / Render)
1. Import your GitHub repository into **Vercel** or **Render Static Site**.
2. Set root directory to `frontend`.
3. Build command: `npm run build`, Output directory: `dist`.

### Deploying Backend (Render / Railway / VPS)
1. Create a Web Service on **Render.com** or **Railway.app**.
2. Set root directory to `backend`.
3. Set Environment Variables:
   - `Jwt__Key`: `YourProductionSuperSecretKey32CharsMin!`
   - `Jwt__Issuer`: `EmployeeTaskManagement`
   - `Jwt__Audience`: `EmployeeTaskManagement`

---

## 📁 Repository Structure

```
Employee-and-Task-Management-System/
├── 📁 frontend/     # React UI, TypeScript, Glassmorphism CSS, Components
└── 📁 backend/      # .NET 8 Web API, Controllers, Models, EF Core Migrations
```
