# 💻 Frontend UI — Task Management System

Modern, responsive web application built with **React 18**, **TypeScript**, **Vite**, and **Glassmorphism Vanilla CSS**.

---

## 🎨 UI Highlights & Modules

1. **Dashboard Overview**: Summary counters for active projects, assigned tasks, completed items, and total staff.
2. **User Administration**: Staff roster table with job roles (`Backend Developer`, `Frontend Developer`, `Application Developer`), departments, and system roles.
3. **Share Credentials Modal**: Interactive card displaying generated employee email, password, and job title with a one-click copy button.
4. **Task Management & Developer Roster**: Visual developer cards showing staff member ID, role, department, and task load. Task assignment dropdown with live assignee preview.
5. **My Assignments (Employee View)**: Specialized task board for employees to update status and post progress comments.
6. **My Profile & Security**: Self-service modal for employees to change their job role and update password.

---

## 🛠️ Local Development

### Installation
```bash
npm install
```

### Run Dev Server
```bash
npm run dev
```
> Server runs locally at `http://localhost:5173`. Proxies `/api` calls to backend at `http://localhost:5000`.

### Build for Production
```bash
npm run build
```
> Outputs production bundle to `dist/`.
