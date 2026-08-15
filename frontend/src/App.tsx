import React, { useState, useEffect, FormEvent } from 'react';

// ── Inline Types (no external module needed) ──────────────────────────────────
type Role = 'ADMIN' | 'MANAGER' | 'EMPLOYEE';

interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  departmentId?: number;
  departmentName?: string;
  role: Role;
  jobTitle?: string;
  isActive: boolean;
}

interface Department {
  id: number;
  name: string;
  description?: string;
  userCount: number;
}

interface Project {
  id: number;
  name: string;
  description?: string;
  status: 'Planning' | 'Active' | 'Completed' | 'Cancelled';
  createdById: number;
  createdByName?: string;
  taskCount: number;
}

interface TaskItem {
  id: number;
  title: string;
  description?: string;
  projectId: number;
  projectName?: string;
  createdById: number;
  createdByName?: string;
  assignedToId?: number;
  assignedToName?: string;
  dueDate: string;
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  status: 'Pending' | 'InProgress' | 'Completed' | 'Cancelled';
  commentCount: number;
}

interface TaskComment {
  id: number;
  content: string;
  taskId: number;
  userId: number;
  userName?: string;
  createdAt: string;
}

interface AuthResponse {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  jobTitle?: string;
  token: string;
}

// Inline SVG Icons for premium and clean UI (no external packages needed)
const Icons = {
  Dashboard: () => (
    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '20px', height: '20px' }}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M10.5 6a7.5 7.5 0 1 0 7.5 7.5h-7.5V6Z" />
      <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 10.5H21A7.5 7.5 0 0 0 13.5 3v7.5Z" />
    </svg>
  ),
  Users: () => (
    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '20px', height: '20px' }}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M15 19.128a9.38 9.38 0 0 0 2.625.372 9.337 9.337 0 0 0 4.121-.952 4.125 4.125 0 0 0-7.533-2.493M15 19.128v-.003c0-1.113-.285-2.16-.786-3.07M15 19.128v.109A11.978 11.978 0 0 1 12 20.25a11.98 11.98 0 0 1-3-.122v-.109m0-1.13c0-1.113.285-2.16.786-3.07M9 19.128v-.003c0-1.113-.285-2.16-.786-3.07M9 19.128A9.38 9.38 0 0 1 6.375 19.5a9.337 9.337 0 0 1-4.121-.952 4.125 4.125 0 0 1 7.533-2.493M9 19.128v.109c0 .037-.001.074-.003.11M15 8.25a3 3 0 1 1-6 0 3 3 0 0 1 6 0ZM6.697 16.203a7.486 7.486 0 0 1 2.303-3.181m0 0a3.375 3.375 0 1 1 5.998 0c.937.755 1.71 1.696 2.27 2.766" />
    </svg>
  ),
  Departments: () => (
    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '20px', height: '20px' }}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M12 21v-8.25M15.75 21v-8.25M8.25 21v-8.25M3 9l9-6 9 6m-1.5 12V10.332A48.36 48.36 0 0 0 12 9.75c-2.551 0-5.056.2-7.5.582V21M3 21h18M12 6.75h.008v.008H12V6.75Z" />
    </svg>
  ),
  Projects: () => (
    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '20px', height: '20px' }}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M2.25 21h19.5m-18-18v18m10.5-18v18m6-13.5V21M6.75 6.75h.75m-.75 3h.75m-.75 3h.75m3-6h.75m-.75 3h.75m-.75 3h.75M6.75 21v-3.375c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V21M3 3h18v3.75H3V3Z" />
    </svg>
  ),
  Tasks: () => (
    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '20px', height: '20px' }}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M9 12h3.75M9 15h3.75M9 18h3.75m3 .75H18a2.25 2.25 0 0 0 2.25-2.25V6.108c0-1.135-.845-2.098-1.976-2.192a48.424 48.424 0 0 0-1.123-.08m-5.801 0c-.065.21-.1.433-.1.664 0 .414.336.75.75.75h4.5a.75.75 0 0 0 .75-.75 2.25 2.25 0 0 0-.1-.664m-5.8 0A2.251 2.251 0 0 1 13.5 2.25H15a2.25 2.25 0 0 1 2.15 1.586m-5.8 0c-.065.21-.1.433-.1.664 0 .414.336.75.75.75h4.5a.75.75 0 0 0 .75-.75 2.25 2.25 0 0 0-.1-.664M4.5 19.5h15m-15-3.375c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V19.5H4.5v-3.375Z" />
    </svg>
  ),
  Plus: () => (
    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor" style={{ width: '16px', height: '16px' }}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
    </svg>
  ),
  Logout: () => (
    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '18px', height: '18px' }}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M8.25 9V5.25A2.25 2.25 0 0 1 10.5 3h6a2.25 2.25 0 0 1 2.25 2.25v13.5A2.25 2.25 0 0 1 16.5 21h-6a2.25 2.25 0 0 1-2.25-2.25V15M12 9l3 3m0 0-3 3m3-3H2.25" />
    </svg>
  ),
  Edit: () => (
    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '14px', height: '14px' }}>
      <path strokeLinecap="round" strokeLinejoin="round" d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L6.832 19.82a4.5 4.5 0 0 1-1.897 1.13l-2.685.8.8-2.685a4.5 4.5 0 0 1 1.13-1.897L16.863 4.487Zm0 0L19.5 7.125" />
    </svg>
  ),
  Delete: () => (
    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '14px', height: '14px' }}>
      <path strokeLinecap="round" strokeLinejoin="round" d="m14.74 9-.34 9m-4.78 0L9 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0" />
    </svg>
  ),
  Close: () => (
    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor" style={{ width: '20px', height: '20px' }}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M6 18 18 6M6 6l12 12" />
    </svg>
  ),
  Key: () => (
    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '18px', height: '18px' }}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 5.25a3 3 0 0 1 3 3m3 0a6 6 0 0 1-7.029 5.912c-.563-.097-1.159.026-1.563.43L10.5 17.25H8.25v2.25H6v2.25H2.25v-2.818c0-.597.237-1.17.659-1.591l6.499-6.499c.404-.404.527-1 .43-1.563A6 6 0 1 1 21.75 8.25Z" />
    </svg>
  ),
  Copy: () => (
    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '16px', height: '16px' }}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 17.25v3.375c0 .621-.504 1.125-1.125 1.125h-9.75a1.125 1.125 0 0 1-1.125-1.125V7.875c0-.621.504-1.125 1.125-1.125H6.75a9.06 9.06 0 0 1 1.5.124m7.5 10.376h3.375c.621 0 1.125-.504 1.125-1.125V11.25c0-4.46-3.243-8.161-7.5-8.876a9.06 9.06 0 0 0-1.5-.124H9.375c-.621 0-1.125.504-1.125 1.125v3.5m7.5 10.375H9.375a1.125 1.125 0 0 1-1.125-1.125v-9.25c0-.621.504-1.125 1.125-1.125h6.75c.621 0 1.125.504 1.125 1.125v9.25c0 .621-.504 1.125-1.125 1.125Z" />
    </svg>
  )
};

export default function App() {
  const [user, setUser] = useState<AuthResponse | null>(null);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [authError, setAuthError] = useState('');
  
  // Dashboard & Navigation state
  const [activeTab, setActiveTab] = useState('dashboard');
  const [users, setUsers] = useState<User[]>([]);
  const [employees, setEmployees] = useState<User[]>([]); // EMPLOYEE-role users for task assignment
  const [departments, setDepartments] = useState<Department[]>([]);
  const [projects, setProjects] = useState<Project[]>([]);
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  
  // Dialog Modals State
  const [showUserModal, setShowUserModal] = useState(false);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [userForm, setUserForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    phoneNumber: '',
    role: 'EMPLOYEE' as Role,
    jobTitle: '',
    departmentId: ''
  });

  // Share Credentials Modal state (Admin after creating user)
  const [createdCredentials, setCreatedCredentials] = useState<{ name: string; email: string; password: string; role: string; jobTitle?: string } | null>(null);
  const [copied, setCopied] = useState(false);

  // Profile & Password Change Modal state (All Users)
  const [showProfileModal, setShowProfileModal] = useState(false);
  const [profileForm, setProfileForm] = useState({
    jobTitle: '',
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  });
  const [profileError, setProfileError] = useState('');
  const [profileSuccess, setProfileSuccess] = useState('');

  const [showDeptModal, setShowDeptModal] = useState(false);
  const [deptForm, setDeptForm] = useState({ name: '', description: '' });

  const [showProjectModal, setShowProjectModal] = useState(false);
  const [projectForm, setProjectForm] = useState({
    name: '',
    description: '',
    startDate: new Date().toISOString().split('T')[0],
    endDate: new Date(Date.now() + 30*24*60*60*1000).toISOString().split('T')[0],
    status: 'Planning'
  });

  const [showTaskModal, setShowTaskModal] = useState(false);
  const [selectedTask, setSelectedTask] = useState<TaskItem | null>(null);
  const [taskForm, setTaskForm] = useState({
    title: '',
    description: '',
    projectId: '',
    assignedToId: '',
    dueDate: new Date(Date.now() + 7*24*60*60*1000).toISOString().split('T')[0],
    priority: 'Medium',
    status: 'Pending'
  });

  // Employee Task Detail & Comments
  const [showTaskDetailModal, setShowTaskDetailModal] = useState(false);
  const [activeTaskDetail, setActiveTaskDetail] = useState<TaskItem | null>(null);
  const [comments, setComments] = useState<TaskComment[]>([]);
  const [newComment, setNewComment] = useState('');

  // Global loading and error alerts
  const [loading, setLoading] = useState(false);
  const [globalError, setGlobalError] = useState('');

  // Restore session from localStorage on load
  useEffect(() => {
    const savedSession = localStorage.getItem('session');
    if (savedSession) {
      try {
        const parsed = JSON.parse(savedSession) as AuthResponse;
        setUser(parsed);
      } catch (e) {
        localStorage.removeItem('session');
      }
    }
  }, []);

  // Fetch core data based on roles when user changes
  useEffect(() => {
    if (user) {
      fetchCoreData();
    }
  }, [user]);

  const fetchCoreData = async () => {
    if (!user) return;
    setLoading(true);
    setGlobalError('');
    try {
      const headers = { Authorization: `Bearer ${user.token}` };
      
      // Fetch departments (visible to everyone)
      const deptsRes = await fetch('/api/departments', { headers });
      if (deptsRes.ok) {
        const data = await deptsRes.json();
        setDepartments(data);
      }

      // Fetch projects (visible to everyone)
      const projectsRes = await fetch('/api/projects', { headers });
      if (projectsRes.ok) {
        const data = await projectsRes.json();
        setProjects(data);
      }

      // Fetch users (visible to ADMIN and MANAGER)
      if (user.role === 'ADMIN' || user.role === 'MANAGER') {
        const usersRes = await fetch('/api/users', { headers });
        if (usersRes.ok) {
          const data = await usersRes.json();
          setUsers(data);
        }
        // Fetch employees specifically for task assignment dropdown
        const empRes = await fetch('/api/users/employees', { headers });
        if (empRes.ok) {
          const empData = await empRes.json();
          setEmployees(empData);
        }
      }

      // Fetch tasks:
      // For EMPLOYEE, fetch my-tasks
      // For ADMIN/MANAGER, fetch all tasks for all projects
      if (user.role === 'EMPLOYEE') {
        const tasksRes = await fetch('/api/tasks/my-tasks', { headers });
        if (tasksRes.ok) {
          const data = await tasksRes.json();
          setTasks(data);
        }
      } else {
        // Load projects first, then query tasks by project and flatten
        const projRes = await fetch('/api/projects', { headers });
        if (projRes.ok) {
          const projs: Project[] = await projRes.json();
          setProjects(_ => projs);
          const allTasks: TaskItem[] = [];
          for (const proj of projs) {
            const tRes = await fetch(`/api/tasks/by-project/${proj.id}`, { headers });
            if (tRes.ok) {
              const projTasks = await tRes.json();
              allTasks.push(...projTasks);
            }
          }
          setTasks(allTasks);
        }
      }
    } catch (e) {
      setGlobalError('Failed to synchronize data with the server.');
    } finally {
      setLoading(false);
    }
  };

  const handleLogin = async (e: FormEvent) => {
    e.preventDefault();
    setAuthError('');
    if (!email || !password) {
      setAuthError('Email and Password are required.');
      return;
    }
    
    try {
      const response = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password })
      });

      if (!response.ok) {
        const text = await response.text();
        setAuthError(text || 'Invalid credentials.');
        return;
      }

      const data: AuthResponse = await response.json();
      setUser(data);
      localStorage.setItem('session', JSON.stringify(data));
      setActiveTab('dashboard');
    } catch (e) {
      setAuthError('Connection failed. Verify the server is running.');
    }
  };

  const handleLogout = () => {
    setUser(null);
    localStorage.removeItem('session');
    setEmail('');
    setPassword('');
  };

  // User Actions (ADMIN Only)
  const saveUser = async (e: FormEvent) => {
    e.preventDefault();
    if (!user) return;
    try {
      const headers = { 
        'Content-Type': 'application/json',
        Authorization: `Bearer ${user.token}` 
      };

      if (selectedUser) {
        // Edit User
        const body = {
          firstName: userForm.firstName,
          lastName: userForm.lastName,
          phoneNumber: userForm.phoneNumber,
          role: userForm.role,
          jobTitle: userForm.jobTitle,
          departmentId: userForm.departmentId ? parseInt(userForm.departmentId) : null,
          isActive: true
        };
        const res = await fetch(`/api/users/${selectedUser.id}`, {
          method: 'PUT',
          headers,
          body: JSON.stringify(body)
        });
        if (!res.ok) throw new Error();
      } else {
        // Create User
        const body = {
          firstName: userForm.firstName,
          lastName: userForm.lastName,
          email: userForm.email,
          password: userForm.password,
          phoneNumber: userForm.phoneNumber,
          role: userForm.role,
          jobTitle: userForm.jobTitle,
          departmentId: userForm.departmentId ? parseInt(userForm.departmentId) : null
        };
        const res = await fetch('/api/users', {
          method: 'POST',
          headers,
          body: JSON.stringify(body)
        });
        if (!res.ok) throw new Error();

        // Open share credentials modal for admin!
        setCreatedCredentials({
          name: `${userForm.firstName} ${userForm.lastName}`,
          email: userForm.email,
          password: userForm.password,
          role: userForm.role,
          jobTitle: userForm.jobTitle
        });
        setCopied(false);
      }

      setShowUserModal(false);
      fetchCoreData();
    } catch (e) {
      alert('Failed to save user.');
    }
  };

  const deleteUser = async (id: number) => {
    if (!user || !window.confirm('Are you sure you want to delete this user?')) return;
    try {
      const res = await fetch(`/api/users/${id}`, {
        method: 'DELETE',
        headers: { Authorization: `Bearer ${user.token}` }
      });
      if (res.ok) fetchCoreData();
    } catch (e) {
      alert('Failed to delete user.');
    }
  };

  const openUserEdit = (u: User) => {
    setSelectedUser(u);
    setUserForm({
      firstName: u.firstName,
      lastName: u.lastName,
      email: u.email,
      password: '',
      phoneNumber: u.phoneNumber || '',
      role: u.role,
      jobTitle: u.jobTitle || '',
      departmentId: u.departmentId?.toString() || ''
    });
    setShowUserModal(true);
  };

  const openUserCreate = () => {
    setSelectedUser(null);
    setUserForm({
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      phoneNumber: '',
      role: 'EMPLOYEE',
      jobTitle: 'Backend Developer',
      departmentId: departments[0]?.id.toString() || ''
    });
    setShowUserModal(true);
  };

  // Self Profile & Password Change Actions (Any Logged-in User)
  const openProfileModal = () => {
    setProfileForm({
      jobTitle: user?.jobTitle || '',
      currentPassword: '',
      newPassword: '',
      confirmPassword: ''
    });
    setProfileError('');
    setProfileSuccess('');
    setShowProfileModal(true);
  };

  const saveProfile = async (e: FormEvent) => {
    e.preventDefault();
    if (!user) return;
    setProfileError('');
    setProfileSuccess('');

    try {
      const headers = {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${user.token}`
      };

      let titleUpdated = false;
      let passwordUpdated = false;

      // 1. Update Job Title if changed
      if (profileForm.jobTitle.trim() !== (user.jobTitle || '')) {
        const res = await fetch(`/api/users/${user.id}`, {
          method: 'PUT',
          headers,
          body: JSON.stringify({
            jobTitle: profileForm.jobTitle.trim()
          })
        });

        if (!res.ok) {
          throw new Error('Failed to update job title.');
        }

        const updatedUser = { ...user, jobTitle: profileForm.jobTitle.trim() };
        setUser(updatedUser);
        localStorage.setItem('session', JSON.stringify(updatedUser));
        titleUpdated = true;
      }

      // 2. Update Password if provided
      if (profileForm.newPassword || profileForm.currentPassword) {
        if (!profileForm.currentPassword) {
          setProfileError('Current password is required to set a new password.');
          return;
        }

        if (profileForm.newPassword.length < 6) {
          setProfileError('New password must be at least 6 characters long.');
          return;
        }

        if (profileForm.newPassword !== profileForm.confirmPassword) {
          setProfileError('New password and confirm password do not match.');
          return;
        }

        const passRes = await fetch('/api/auth/change-password', {
          method: 'POST',
          headers,
          body: JSON.stringify({
            currentPassword: profileForm.currentPassword,
            newPassword: profileForm.newPassword
          })
        });

        if (!passRes.ok) {
          const txt = await passRes.text();
          setProfileError(txt || 'Current password is incorrect.');
          return;
        }

        passwordUpdated = true;
      }

      if (titleUpdated || passwordUpdated) {
        setProfileSuccess('Profile & Password updated successfully!');
        setProfileForm(f => ({ ...f, currentPassword: '', newPassword: '', confirmPassword: '' }));
        fetchCoreData();
        setTimeout(() => setShowProfileModal(false), 1500);
      } else {
        setShowProfileModal(false);
      }
    } catch (err: any) {
      setProfileError(err.message || 'An error occurred while updating profile.');
    }
  };

  // Department Actions (ADMIN Only)
  const saveDepartment = async (e: FormEvent) => {
    e.preventDefault();
    if (!user) return;
    try {
      const res = await fetch('/api/departments', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${user.token}`
        },
        body: JSON.stringify(deptForm)
      });
      if (res.ok) {
        setShowDeptModal(false);
        setDeptForm({ name: '', description: '' });
        fetchCoreData();
      } else {
        const txt = await res.text();
        alert(txt || 'Failed to create department.');
      }
    } catch (e) {
      alert('Failed to create department.');
    }
  };

  const deleteDepartment = async (id: number) => {
    if (!user || !window.confirm('Are you sure you want to delete this department?')) return;
    try {
      const res = await fetch(`/api/departments/${id}`, {
        method: 'DELETE',
        headers: { Authorization: `Bearer ${user.token}` }
      });
      if (res.ok) {
        fetchCoreData();
      } else {
        const text = await res.text();
        alert(text || 'Could not delete department.');
      }
    } catch (e) {
      alert('Failed to delete department.');
    }
  };

  // Project Actions (MANAGER & ADMIN)
  const saveProject = async (e: FormEvent) => {
    e.preventDefault();
    if (!user) return;
    try {
      const res = await fetch('/api/projects', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${user.token}`
        },
        body: JSON.stringify({
          name: projectForm.name,
          description: projectForm.description,
          startDate: new Date(projectForm.startDate).toISOString(),
          endDate: new Date(projectForm.endDate).toISOString(),
          status: projectForm.status
        })
      });
      if (res.ok) {
        setShowProjectModal(false);
        setProjectForm({
          name: '',
          description: '',
          startDate: new Date().toISOString().split('T')[0],
          endDate: new Date(Date.now() + 30*24*60*60*1000).toISOString().split('T')[0],
          status: 'Planning'
        });
        fetchCoreData();
      } else {
        alert('Failed to create project.');
      }
    } catch (e) {
      alert('Connection failed.');
    }
  };

  const deleteProject = async (id: number) => {
    if (!user || !window.confirm('Are you sure you want to delete this project? This will delete all tasks in the project.')) return;
    try {
      const res = await fetch(`/api/projects/${id}`, {
        method: 'DELETE',
        headers: { Authorization: `Bearer ${user.token}` }
      });
      if (res.ok) fetchCoreData();
    } catch (e) {
      alert('Failed to delete project.');
    }
  };

  // Task Actions (MANAGER & ADMIN)
  const saveTask = async (e: FormEvent) => {
    e.preventDefault();
    if (!user) return;
    try {
      const headers = {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${user.token}`
      };

      const body = {
        title: taskForm.title,
        description: taskForm.description,
        projectId: parseInt(taskForm.projectId),
        assignedToId: taskForm.assignedToId ? parseInt(taskForm.assignedToId) : null,
        dueDate: new Date(taskForm.dueDate).toISOString(),
        priority: taskForm.priority,
        status: taskForm.status
      };

      if (selectedTask) {
        const res = await fetch(`/api/tasks/${selectedTask.id}`, {
          method: 'PUT',
          headers,
          body: JSON.stringify(body)
        });
        if (!res.ok) throw new Error();
      } else {
        const res = await fetch('/api/tasks', {
          method: 'POST',
          headers,
          body: JSON.stringify(body)
        });
        if (!res.ok) throw new Error();
      }

      setShowTaskModal(false);
      fetchCoreData();
    } catch (e) {
      alert('Failed to save task.');
    }
  };

  const deleteTask = async (id: number) => {
    if (!user || !window.confirm('Are you sure you want to delete this task?')) return;
    try {
      const res = await fetch(`/api/tasks/${id}`, {
        method: 'DELETE',
        headers: { Authorization: `Bearer ${user.token}` }
      });
      if (res.ok) fetchCoreData();
    } catch (e) {
      alert('Failed to delete task.');
    }
  };

  const openTaskEdit = (t: TaskItem) => {
    setSelectedTask(t);
    setTaskForm({
      title: t.title,
      description: t.description || '',
      projectId: t.projectId.toString(),
      assignedToId: t.assignedToId?.toString() || '',
      dueDate: t.dueDate.split('T')[0],
      priority: t.priority,
      status: t.status
    });
    setShowTaskModal(true);
  };

  const openTaskCreate = () => {
    setSelectedTask(null);
    setTaskForm({
      title: '',
      description: '',
      projectId: projects[0]?.id.toString() || '',
      assignedToId: '',
      dueDate: new Date(Date.now() + 7*24*60*60*1000).toISOString().split('T')[0],
      priority: 'Medium',
      status: 'Pending'
    });
    setShowTaskModal(true);
  };

  // Employee Task Detail & Comments
  const openTaskDetails = async (t: TaskItem) => {
    setActiveTaskDetail(t);
    setShowTaskDetailModal(true);
    setComments([]);
    if (!user) return;
    try {
      const res = await fetch(`/api/tasks/${t.id}/comments`, {
        headers: { Authorization: `Bearer ${user.token}` }
      });
      if (res.ok) {
        const data = await res.json();
        setComments(data);
      }
    } catch (e) {
      console.error(e);
    }
  };

  const postComment = async (e: FormEvent) => {
    e.preventDefault();
    if (!user || !activeTaskDetail || !newComment.trim()) return;
    try {
      const res = await fetch(`/api/tasks/${activeTaskDetail.id}/comments`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${user.token}`
        },
        body: JSON.stringify({
          content: newComment,
          taskId: activeTaskDetail.id
        })
      });
      if (res.ok) {
        setNewComment('');
        // Reload comments
        const freshRes = await fetch(`/api/tasks/${activeTaskDetail.id}/comments`, {
          headers: { Authorization: `Bearer ${user.token}` }
        });
        if (freshRes.ok) {
          const freshData = await freshRes.json();
          setComments(freshData);
        }
        // Update task comment count in the list
        fetchCoreData();
      }
    } catch (e) {
      alert('Failed to post comment.');
    }
  };

  const changeTaskStatusDirect = async (t: TaskItem, newStatus: TaskItem['status']) => {
    if (!user) return;
    try {
      const body = {
        title: t.title,
        description: t.description,
        projectId: t.projectId,
        assignedToId: t.assignedToId,
        dueDate: t.dueDate,
        priority: t.priority,
        status: newStatus
      };
      const res = await fetch(`/api/tasks/${t.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${user.token}`
        },
        body: JSON.stringify(body)
      });
      if (res.ok) {
        fetchCoreData();
        if (activeTaskDetail && activeTaskDetail.id === t.id) {
          setActiveTaskDetail({ ...activeTaskDetail, status: newStatus });
        }
      }
    } catch (e) {
      alert('Failed to change task status.');
    }
  };

  // Login View
  if (!user) {
    return (
      <div className="auth-wrapper">
        <div className="auth-card glass">
          <div className="auth-header">
            <div className="logo-badge">TM</div>
            <h2>Task Management</h2>
            <p>Welcome back! Please enter details</p>
          </div>
          {authError && <div className="error-alert">{authError}</div>}
          <form onSubmit={handleLogin}>
            <div className="form-group">
              <label>Email Address</label>
              <input 
                type="email" 
                className="input-field" 
                placeholder="name@example.com" 
                value={email}
                onChange={e => setEmail(e.target.value)}
              />
            </div>
            <div className="form-group">
              <label>Password</label>
              <input 
                type="password" 
                className="input-field" 
                placeholder="••••••••" 
                value={password}
                onChange={e => setPassword(e.target.value)}
              />
            </div>
            <button type="submit" className="btn-primary" style={{ marginTop: '10px' }}>
              Sign In
            </button>
          </form>
        </div>
      </div>
    );
  }

  // Dashboard calculations
  const stats = {
    userCount: users.length,
    deptCount: departments.length,
    projectCount: projects.length,
    taskCount: tasks.length,
    completedTasks: tasks.filter(t => t.status === 'Completed').length,
    pendingTasks: tasks.filter(t => t.status === 'Pending').length,
    inprogressTasks: tasks.filter(t => t.status === 'InProgress').length
  };

  // Main UI Shell
  return (
    <div className="app-container">
      {/* Sidebar */}
      <aside className="sidebar">
        <div className="sidebar-logo">
          <div className="logo-badge" style={{ width: '32px', height: '32px', borderRadius: '8px', fontSize: '14px', marginBottom: 0 }}>TM</div>
          <h2>Task Management</h2>
        </div>
        
        <nav className="sidebar-nav">
          <button 
            className={`nav-item ${activeTab === 'dashboard' ? 'active' : ''}`}
            onClick={() => setActiveTab('dashboard')}
          >
            <Icons.Dashboard /> Dashboard
          </button>

          {/* Admin tabs */}
          {user.role === 'ADMIN' && (
            <>
              <button 
                className={`nav-item ${activeTab === 'users' ? 'active' : ''}`}
                onClick={() => setActiveTab('users')}
              >
                <Icons.Users /> Users
              </button>
              <button 
                className={`nav-item ${activeTab === 'departments' ? 'active' : ''}`}
                onClick={() => setActiveTab('departments')}
              >
                <Icons.Departments /> Departments
              </button>
            </>
          )}

          {/* Manager/Admin tabs */}
          {(user.role === 'ADMIN' || user.role === 'MANAGER') && (
            <>
              <button 
                className={`nav-item ${activeTab === 'projects' ? 'active' : ''}`}
                onClick={() => setActiveTab('projects')}
              >
                <Icons.Projects /> Projects
              </button>
              <button 
                className={`nav-item ${activeTab === 'tasks' ? 'active' : ''}`}
                onClick={() => setActiveTab('tasks')}
              >
                <Icons.Tasks /> Tasks
              </button>
            </>
          )}

          {/* Employee tabs */}
          {user.role === 'EMPLOYEE' && (
            <button 
              className={`nav-item ${activeTab === 'my-tasks' ? 'active' : ''}`}
              onClick={() => setActiveTab('my-tasks')}
            >
              <Icons.Tasks /> My Tasks
            </button>
          )}
        </nav>

        <div className="sidebar-user">
          <div className="user-info-brief" style={{ cursor: 'pointer' }} onClick={openProfileModal} title="Click to edit profile or change password">
            <span className="name">{user.firstName} {user.lastName}</span>
            <span className="role">{user.jobTitle ? `${user.jobTitle} · ${user.role}` : user.role}</span>
          </div>
          <div style={{ display: 'flex', gap: '4px' }}>
            <button className="btn-logout" title="Profile & Change Password" onClick={openProfileModal} style={{ background: 'rgba(99, 102, 241, 0.15)', color: '#a5b4fc' }}>
              <Icons.Key />
            </button>
            <button className="btn-logout" title="Sign Out" onClick={handleLogout}>
              <Icons.Logout />
            </button>
          </div>
        </div>
      </aside>

      {/* Main Panel */}
      <main className="main-content">
        {globalError && <div className="error-alert">{globalError}</div>}

        {/* LOADING INDICATOR */}
        {loading && <div style={{ color: 'var(--text-secondary)', marginBottom: '15px', fontSize: '14px' }}>Synchronizing...</div>}

        {/* TAB: DASHBOARD */}
        {activeTab === 'dashboard' && (
          <div>
            <div className="view-header">
              <div>
                <h1>Dashboard Summary</h1>
                <p>Welcome to the dashboard, {user.firstName}. Here is a summary of the workspace.</p>
              </div>
              <button className="btn-action" onClick={fetchCoreData}>
                Refresh Data
              </button>
            </div>

            <div className="stats-summary">
              <div className="stat-card glass">
                <div className="stat-title">Seeded Role</div>
                <div className="stat-value" style={{ fontSize: '28px', color: '#818cf8', WebkitTextFillColor: 'initial' }}>{user.role}</div>
                <div className="stat-desc">Access levels fully active</div>
              </div>
              <div className="stat-card glass">
                <div className="stat-title">Active Projects</div>
                <div className="stat-value">{stats.projectCount}</div>
                <div className="stat-desc">Created in the workspace</div>
              </div>
              <div className="stat-card glass">
                <div className="stat-title">Work Tasks</div>
                <div className="stat-value">{stats.taskCount}</div>
                <div className="stat-desc">{stats.completedTasks} completed / {stats.inprogressTasks} in progress</div>
              </div>
              {user.role === 'ADMIN' && (
                <div className="stat-card glass">
                  <div className="stat-title">Total Staff</div>
                  <div className="stat-value">{stats.userCount}</div>
                  <div className="stat-desc">Across {stats.deptCount} departments</div>
                </div>
              )}
            </div>

            {/* Quick Actions / Tips */}
            <div className="glass" style={{ padding: '30px', marginTop: '30px' }}>
              <h3 style={{ marginBottom: '12px' }}>Workspace Instructions</h3>
              <p style={{ color: 'var(--text-secondary)', fontSize: '15px', lineHeight: '1.7' }}>
                This is a premium role-based task management dashboard. Your current login represents the <strong>{user.role}</strong> scope.
                Use the sidebar navigation tabs to administer users, create and review development projects, allocate task cards, and append review comments.
              </p>
            </div>
          </div>
        )}

        {/* TAB: USERS (ADMIN ONLY) */}
        {activeTab === 'users' && user.role === 'ADMIN' && (
          <div>
            <div className="view-header">
              <div>
                <h1>User Administration</h1>
                <p>Register, modify, and delete employee credentials and roles.</p>
              </div>
              <button className="btn-action" onClick={openUserCreate}>
                <Icons.Plus /> Register Staff
              </button>
            </div>

            <div className="table-container glass">
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Name</th>
                    <th>Email Address</th>
                    <th>Job Role / Specialization</th>
                    <th>Department</th>
                    <th>System Role</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {users.map(u => {
                    const dept = departments.find(d => d.id === u.departmentId);
                    return (
                      <tr key={u.id}>
                        <td style={{ fontWeight: '600' }}>{u.firstName} {u.lastName}</td>
                        <td>{u.email}</td>
                        <td>
                          {u.jobTitle ? (
                            <span style={{ background: 'rgba(99, 102, 241, 0.15)', color: '#a5b4fc', border: '1px solid rgba(99, 102, 241, 0.3)', padding: '3px 10px', borderRadius: '12px', fontSize: '12px', fontWeight: 600 }}>
                              💻 {u.jobTitle}
                            </span>
                          ) : (
                            <span style={{ color: 'var(--text-muted)', fontSize: '12px' }}>General Staff</span>
                          )}
                        </td>
                        <td>{dept ? dept.name : <span style={{ color: 'var(--text-muted)' }}>None</span>}</td>
                        <td>
                          <span className={`badge badge-role-${u.role.toLowerCase()}`}>
                            {u.role}
                          </span>
                        </td>
                        <td>
                          <button className="btn-edit" onClick={() => openUserEdit(u)}>
                            <Icons.Edit /> Edit
                          </button>
                          <button className="btn-delete-small" onClick={() => deleteUser(u.id)}>
                            <Icons.Delete /> Delete
                          </button>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* TAB: DEPARTMENTS (ADMIN ONLY) */}
        {activeTab === 'departments' && user.role === 'ADMIN' && (
          <div>
            <div className="view-header">
              <div>
                <h1>Departments</h1>
                <p>Manage office groups and view structural user distribution.</p>
              </div>
              <button className="btn-action" onClick={() => setShowDeptModal(true)}>
                <Icons.Plus /> New Department
              </button>
            </div>

            <div className="table-container glass">
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Name</th>
                    <th>Description</th>
                    <th>User Count</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {departments.map(d => (
                    <tr key={d.id}>
                      <td style={{ fontWeight: '600' }}>{d.name}</td>
                      <td style={{ color: 'var(--text-secondary)' }}>{d.description || 'No description provided.'}</td>
                      <td>
                        <span className="badge" style={{ background: 'rgba(255,255,255,0.05)', color: 'white' }}>
                          {d.userCount} employees
                        </span>
                      </td>
                      <td>
                        <button 
                          className="btn-delete-small" 
                          onClick={() => deleteDepartment(d.id)}
                          disabled={d.userCount > 0}
                          title={d.userCount > 0 ? "Cannot delete department with active users" : ""}
                          style={d.userCount > 0 ? { opacity: 0.4, cursor: 'not-allowed' } : {}}
                        >
                          <Icons.Delete /> Delete
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* TAB: PROJECTS (MANAGER & ADMIN) */}
        {activeTab === 'projects' && (user.role === 'ADMIN' || user.role === 'MANAGER') && (
          <div>
            <div className="view-header">
              <div>
                <h1>Workspace Projects</h1>
                <p>Track, manage, and assign developmental project cards.</p>
              </div>
              <button className="btn-action" onClick={() => setShowProjectModal(true)}>
                <Icons.Plus /> Add Project
              </button>
            </div>

            <div className="grid-cols-3">
              {projects.map(p => (
                <div key={p.id} className="project-card glass">
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                    <h3>{p.name}</h3>
                    <span className={`badge badge-status-${p.status.toLowerCase()}`}>
                      {p.status}
                    </span>
                  </div>
                  <p>{p.description || 'No project description added.'}</p>
                  <div className="project-card-footer">
                    <span>{p.taskCount} active tasks</span>
                    <button 
                      className="btn-delete-small" 
                      onClick={() => deleteProject(p.id)}
                      style={{ margin: 0, padding: '4px 8px' }}
                    >
                      <Icons.Delete /> Delete
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* TAB: TASKS (MANAGER & ADMIN) */}
        {activeTab === 'tasks' && (user.role === 'ADMIN' || user.role === 'MANAGER') && (
          <div>
            <div className="view-header">
              <div>
                <h1>Task Management</h1>
                <p>Assign tasks to employees and track progress across all projects.</p>
              </div>
              <button className="btn-action" onClick={openTaskCreate}>
                <Icons.Plus /> Create Task
              </button>
            </div>

            {/* Employee Roster - quick reference before assigning */}
            {employees.length > 0 && (
              <div style={{ marginBottom: '28px' }}>
                <h3 style={{ fontSize: '14px', fontWeight: 600, color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.08em', marginBottom: '14px' }}>
                  👥 Active Employees — {employees.length} staff available for assignment
                </h3>
                <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(220px, 1fr))', gap: '12px' }}>
                  {employees.map(emp => {
                    const empTaskCount = tasks.filter(t => t.assignedToId === emp.id).length;
                    return (
                      <div key={emp.id} className="glass" style={{ padding: '14px 16px', borderRadius: '12px', display: 'flex', alignItems: 'center', gap: '12px', cursor: 'pointer', transition: 'border-color 0.2s' }}
                        onClick={() => {
                          openTaskCreate();
                          setTimeout(() => setTaskForm(f => ({ ...f, assignedToId: emp.id.toString() })), 50);
                        }}
                        title={`Click to create task for ${emp.firstName}`}
                      >
                        <div style={{ width: '38px', height: '38px', borderRadius: '50%', background: 'linear-gradient(135deg, #6366f1, #8b5cf6)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '14px', fontWeight: 700, color: 'white', flexShrink: 0 }}>
                          {emp.firstName[0]}{emp.lastName[0]}
                        </div>
                        <div style={{ minWidth: 0 }}>
                          <div style={{ fontWeight: 600, fontSize: '14px', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
                            {emp.firstName} {emp.lastName}
                          </div>
                          {emp.jobTitle && (
                            <div style={{ fontSize: '11px', color: '#a5b4fc', fontWeight: 600, margin: '1px 0' }}>
                              💻 {emp.jobTitle}
                            </div>
                          )}
                          <div style={{ fontSize: '11px', color: 'var(--text-muted)', display: 'flex', gap: '6px', alignItems: 'center', marginTop: '2px' }}>
                            <span style={{ background: 'rgba(99,102,241,0.2)', color: '#a5b4fc', padding: '1px 6px', borderRadius: '4px', fontWeight: 700, fontFamily: 'monospace' }}>ID #{emp.id}</span>
                            <span>{emp.departmentName || 'No Dept'}</span>
                          </div>
                          <div style={{ fontSize: '11px', color: 'var(--text-muted)', marginTop: '2px' }}>
                            {empTaskCount} task{empTaskCount !== 1 ? 's' : ''} assigned
                          </div>
                        </div>
                      </div>
                    );
                  })}
                </div>
              </div>
            )}

            <div className="table-container glass">
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Title</th>
                    <th>Project</th>
                    <th>Assigned To</th>
                    <th>Due Date</th>
                    <th>Priority</th>
                    <th>Status</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {tasks.map(t => {
                    const assignee = employees.find(e => e.id === t.assignedToId);
                    return (
                      <tr key={t.id}>
                        <td style={{ fontWeight: '600' }}>{t.title}</td>
                        <td>{t.projectName}</td>
                        <td>
                          {assignee ? (
                            <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                              <div style={{ width: '26px', height: '26px', borderRadius: '50%', background: 'linear-gradient(135deg, #6366f1, #8b5cf6)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '10px', fontWeight: 700, color: 'white', flexShrink: 0 }}>
                                {assignee.firstName[0]}{assignee.lastName[0]}
                              </div>
                              <div>
                                <div style={{ fontWeight: 600, fontSize: '13px' }}>{t.assignedToName}</div>
                                <div style={{ fontSize: '11px', color: 'var(--text-muted)', fontFamily: 'monospace' }}>ID #{t.assignedToId}</div>
                              </div>
                            </div>
                          ) : t.assignedToId ? (
                            <span>{t.assignedToName} <span style={{ fontSize: '11px', color: 'var(--text-muted)', fontFamily: 'monospace' }}>(ID #{t.assignedToId})</span></span>
                          ) : (
                            <span style={{ color: 'var(--text-muted)', fontStyle: 'italic' }}>Unassigned</span>
                          )}
                        </td>
                        <td>{new Date(t.dueDate).toLocaleDateString()}</td>
                        <td>
                          <span className={`badge badge-priority-${t.priority.toLowerCase()}`}>
                            {t.priority}
                          </span>
                        </td>
                        <td>
                          <span className={`badge badge-status-${t.status.toLowerCase()}`}>
                            {t.status}
                          </span>
                        </td>
                        <td>
                          <button className="btn-edit" onClick={() => openTaskEdit(t)}>
                            <Icons.Edit /> Edit
                          </button>
                          <button className="btn-delete-small" onClick={() => deleteTask(t.id)}>
                            <Icons.Delete /> Delete
                          </button>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* TAB: MY TASKS (EMPLOYEE ONLY) */}
        {activeTab === 'my-tasks' && user.role === 'EMPLOYEE' && (
          <div>
            <div className="view-header">
              <div>
                <h1>My Assignments</h1>
                <p>Select and review tasks assigned specifically to you.</p>
              </div>
            </div>

            <div className="grid-cols-3">
              {tasks.map(t => (
                <div key={t.id} className="task-card glass" style={{ cursor: 'pointer' }} onClick={() => openTaskDetails(t)}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                    <span className={`badge badge-priority-${t.priority.toLowerCase()}`}>
                      {t.priority}
                    </span>
                    <span className={`badge badge-status-${t.status.toLowerCase()}`} onClick={e => e.stopPropagation()}>
                      {t.status}
                    </span>
                  </div>
                  <h3 style={{ marginTop: '5px' }}>{t.title}</h3>
                  <p>{t.description || 'No task description available.'}</p>
                  
                  {/* Status update controls inside card */}
                  <div onClick={e => e.stopPropagation()} style={{ marginTop: '10px', display: 'flex', gap: '5px' }}>
                    <select 
                      value={t.status}
                      className="input-field"
                      style={{ padding: '6px 10px', fontSize: '12px' }}
                      onChange={e => changeTaskStatusDirect(t, e.target.value as TaskItem['status'])}
                    >
                      <option value="Pending">Pending</option>
                      <option value="InProgress">In Progress</option>
                      <option value="Completed">Completed</option>
                      <option value="Cancelled">Cancelled</option>
                    </select>
                  </div>

                  <div className="task-card-footer">
                    <span>Due: {new Date(t.dueDate).toLocaleDateString()}</span>
                    <span>{t.commentCount} comments</span>
                  </div>
                </div>
              ))}
              {tasks.length === 0 && (
                <div className="glass" style={{ gridColumn: 'span 3', padding: '40px', textAlign: 'center', color: 'var(--text-secondary)' }}>
                  No tasks currently assigned to you.
                </div>
              )}
            </div>
          </div>
        )}
      </main>

      {/* MODAL: USER CREATE/EDIT (ADMIN ONLY) */}
      {showUserModal && (
        <div className="modal-overlay">
          <div className="modal-content glass">
            <div className="modal-header">
              <h2>{selectedUser ? 'Edit User' : 'Register User'}</h2>
              <button className="btn-close" onClick={() => setShowUserModal(false)}>
                <Icons.Close />
              </button>
            </div>
            <form onSubmit={saveUser}>
              <div style={{ display: 'flex', gap: '15px' }}>
                <div className="form-group" style={{ flex: 1 }}>
                  <label>First Name</label>
                  <input 
                    type="text" 
                    className="input-field" 
                    required
                    value={userForm.firstName}
                    onChange={e => setUserForm({ ...userForm, firstName: e.target.value })}
                  />
                </div>
                <div className="form-group" style={{ flex: 1 }}>
                  <label>Last Name</label>
                  <input 
                    type="text" 
                    className="input-field" 
                    required
                    value={userForm.lastName}
                    onChange={e => setUserForm({ ...userForm, lastName: e.target.value })}
                  />
                </div>
              </div>

              {!selectedUser && (
                <div className="form-group">
                  <label>Email Address</label>
                  <input 
                    type="email" 
                    className="input-field" 
                    required
                    value={userForm.email}
                    onChange={e => setUserForm({ ...userForm, email: e.target.value })}
                  />
                </div>
              )}

              {!selectedUser && (
                <div className="form-group">
                  <label>Password</label>
                  <input 
                    type="password" 
                    className="input-field" 
                    required
                    placeholder="At least 6 characters"
                    value={userForm.password}
                    onChange={e => setUserForm({ ...userForm, password: e.target.value })}
                  />
                </div>
              )}

              <div className="form-group">
                <label>Job Role / Specialization</label>
                <select 
                  className="input-field"
                  value={['Backend Developer', 'Frontend Developer', 'Application Developer'].includes(userForm.jobTitle) ? userForm.jobTitle : (userForm.jobTitle ? 'Other' : 'Backend Developer')}
                  onChange={e => {
                    const val = e.target.value;
                    if (val === 'Other') {
                      setUserForm({ ...userForm, jobTitle: '' });
                    } else {
                      setUserForm({ ...userForm, jobTitle: val });
                    }
                  }}
                >
                  <option value="Backend Developer">Backend Developer</option>
                  <option value="Frontend Developer">Frontend Developer</option>
                  <option value="Application Developer">Application Developer</option>
                  <option value="Other">Other... (Type Custom Role)</option>
                </select>

                {(!['Backend Developer', 'Frontend Developer', 'Application Developer'].includes(userForm.jobTitle) || userForm.jobTitle === '') && (
                  <input 
                    type="text" 
                    className="input-field" 
                    style={{ marginTop: '8px' }}
                    placeholder="Type custom role (e.g. Fullstack, UI/UX, DevOps)..."
                    value={userForm.jobTitle}
                    onChange={e => setUserForm({ ...userForm, jobTitle: e.target.value })}
                    required
                  />
                )}
              </div>

              <div className="form-group">
                <label>Phone Number</label>
                <input 
                  type="text" 
                  className="input-field" 
                  value={userForm.phoneNumber}
                  onChange={e => setUserForm({ ...userForm, phoneNumber: e.target.value })}
                />
              </div>

              <div style={{ display: 'flex', gap: '15px' }}>
                <div className="form-group" style={{ flex: 1 }}>
                  <label>System Role</label>
                  <select 
                    className="input-field"
                    value={userForm.role}
                    onChange={e => setUserForm({ ...userForm, role: e.target.value as Role })}
                  >
                    <option value="EMPLOYEE">Employee</option>
                    <option value="MANAGER">Manager</option>
                    <option value="ADMIN">Admin</option>
                  </select>
                </div>
                <div className="form-group" style={{ flex: 1 }}>
                  <label>Department</label>
                  <select 
                    className="input-field"
                    value={userForm.departmentId}
                    onChange={e => setUserForm({ ...userForm, departmentId: e.target.value })}
                  >
                    <option value="">No Department</option>
                    {departments.map(d => (
                      <option key={d.id} value={d.id}>{d.name}</option>
                    ))}
                  </select>
                </div>
              </div>

              <div className="modal-footer">
                <button type="button" className="btn-cancel" onClick={() => setShowUserModal(false)}>
                  Cancel
                </button>
                <button type="submit" className="btn-primary" style={{ width: 'auto' }}>
                  Save User
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* MODAL: DEPARTMENT CREATE (ADMIN ONLY) */}
      {showDeptModal && (
        <div className="modal-overlay">
          <div className="modal-content glass">
            <div className="modal-header">
              <h2>New Department</h2>
              <button className="btn-close" onClick={() => setShowDeptModal(false)}>
                <Icons.Close />
              </button>
            </div>
            <form onSubmit={saveDepartment}>
              <div className="form-group">
                <label>Department Name</label>
                <input 
                  type="text" 
                  className="input-field" 
                  required
                  placeholder="e.g. Engineering"
                  value={deptForm.name}
                  onChange={e => setDeptForm({ ...deptForm, name: e.target.value })}
                />
              </div>
              <div className="form-group">
                <label>Description</label>
                <textarea 
                  className="input-field" 
                  style={{ height: '100px', resize: 'none' }}
                  placeholder="Optional description of this department..."
                  value={deptForm.description}
                  onChange={e => setDeptForm({ ...deptForm, description: e.target.value })}
                />
              </div>
              <div className="modal-footer">
                <button type="button" className="btn-cancel" onClick={() => setShowDeptModal(false)}>
                  Cancel
                </button>
                <button type="submit" className="btn-primary" style={{ width: 'auto' }}>
                  Create Department
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* MODAL: PROJECT CREATE (MANAGER/ADMIN) */}
      {showProjectModal && (
        <div className="modal-overlay">
          <div className="modal-content glass">
            <div className="modal-header">
              <h2>Create Project</h2>
              <button className="btn-close" onClick={() => setShowProjectModal(false)}>
                <Icons.Close />
              </button>
            </div>
            <form onSubmit={saveProject}>
              <div className="form-group">
                <label>Project Name</label>
                <input 
                  type="text" 
                  className="input-field" 
                  required
                  placeholder="e.g. Mobile Application V2"
                  value={projectForm.name}
                  onChange={e => setProjectForm({ ...projectForm, name: e.target.value })}
                />
              </div>
              <div className="form-group">
                <label>Description</label>
                <textarea 
                  className="input-field" 
                  style={{ height: '80px', resize: 'none' }}
                  value={projectForm.description}
                  onChange={e => setProjectForm({ ...projectForm, description: e.target.value })}
                />
              </div>
              <div style={{ display: 'flex', gap: '15px' }}>
                <div className="form-group" style={{ flex: 1 }}>
                  <label>Start Date</label>
                  <input 
                    type="date" 
                    className="input-field" 
                    required
                    value={projectForm.startDate}
                    onChange={e => setProjectForm({ ...projectForm, startDate: e.target.value })}
                  />
                </div>
                <div className="form-group" style={{ flex: 1 }}>
                  <label>End Date</label>
                  <input 
                    type="date" 
                    className="input-field" 
                    required
                    value={projectForm.endDate}
                    onChange={e => setProjectForm({ ...projectForm, endDate: e.target.value })}
                  />
                </div>
              </div>
              <div className="form-group">
                <label>Status</label>
                <select 
                  className="input-field"
                  value={projectForm.status}
                  onChange={e => setProjectForm({ ...projectForm, status: e.target.value })}
                >
                  <option value="Planning">Planning</option>
                  <option value="Active">Active</option>
                  <option value="Completed">Completed</option>
                  <option value="Cancelled">Cancelled</option>
                </select>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn-cancel" onClick={() => setShowProjectModal(false)}>
                  Cancel
                </button>
                <button type="submit" className="btn-primary" style={{ width: 'auto' }}>
                  Create Project
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* MODAL: TASK CREATE/EDIT (MANAGER/ADMIN) */}
      {showTaskModal && (
        <div className="modal-overlay">
          <div className="modal-content glass">
            <div className="modal-header">
              <h2>{selectedTask ? 'Edit Task' : 'Create Task'}</h2>
              <button className="btn-close" onClick={() => setShowTaskModal(false)}>
                <Icons.Close />
              </button>
            </div>
            <form onSubmit={saveTask}>
              <div className="form-group">
                <label>Task Title</label>
                <input 
                  type="text" 
                  className="input-field" 
                  required
                  placeholder="e.g. Implement login routing"
                  value={taskForm.title}
                  onChange={e => setTaskForm({ ...taskForm, title: e.target.value })}
                />
              </div>
              <div className="form-group">
                <label>Description</label>
                <textarea 
                  className="input-field" 
                  style={{ height: '80px', resize: 'none' }}
                  value={taskForm.description}
                  onChange={e => setTaskForm({ ...taskForm, description: e.target.value })}
                />
              </div>

              {!selectedTask && (
                <div className="form-group">
                  <label>Project</label>
                  <select 
                    className="input-field"
                    required
                    value={taskForm.projectId}
                    onChange={e => setTaskForm({ ...taskForm, projectId: e.target.value })}
                  >
                    <option value="" disabled>Select Project</option>
                    {projects.map(p => (
                      <option key={p.id} value={p.id}>{p.name}</option>
                    ))}
                  </select>
                </div>
              )}

              <div style={{ display: 'flex', gap: '15px' }}>
                <div className="form-group" style={{ flex: 1 }}>
                  <label>Assign To Employee</label>
                  <select 
                    className="input-field"
                    value={taskForm.assignedToId}
                    onChange={e => setTaskForm({ ...taskForm, assignedToId: e.target.value })}
                  >
                    <option value="">— Select Employee —</option>
                    {employees.length === 0 && (
                      <option disabled>No employees found</option>
                    )}
                    {employees.map(emp => (
                      <option key={emp.id} value={emp.id}>
                        #{emp.id} · {emp.firstName} {emp.lastName}{emp.jobTitle ? ` (${emp.jobTitle})` : ''}{emp.departmentName ? ` · ${emp.departmentName}` : ''}
                      </option>
                    ))}
                  </select>
                  {taskForm.assignedToId && (() => {
                    const sel = employees.find(e => e.id === parseInt(taskForm.assignedToId));
                    return sel ? (
                      <div style={{ marginTop: '8px', padding: '10px 12px', background: 'rgba(99,102,241,0.1)', borderRadius: '8px', border: '1px solid rgba(99,102,241,0.3)', display: 'flex', alignItems: 'center', gap: '10px' }}>
                        <div style={{ width: '32px', height: '32px', borderRadius: '50%', background: 'linear-gradient(135deg,#6366f1,#8b5cf6)', display:'flex', alignItems:'center', justifyContent:'center', fontSize: '12px', fontWeight: 700, color:'white' }}>
                          {sel.firstName[0]}{sel.lastName[0]}
                        </div>
                        <div>
                          <div style={{ fontWeight: 600, fontSize: '13px' }}>{sel.firstName} {sel.lastName} {sel.jobTitle ? `· ${sel.jobTitle}` : ''}</div>
                          <div style={{ fontSize: '11px', color: 'var(--text-muted)' }}>Employee ID <strong style={{ color: '#a5b4fc', fontFamily: 'monospace' }}>#{sel.id}</strong>{sel.departmentName ? ` · ${sel.departmentName}` : ''}</div>
                        </div>
                      </div>
                    ) : null;
                  })()}
                </div>
                <div className="form-group" style={{ flex: 1 }}>
                  <label>Due Date</label>
                  <input 
                    type="date" 
                    className="input-field" 
                    required
                    value={taskForm.dueDate}
                    onChange={e => setTaskForm({ ...taskForm, dueDate: e.target.value })}
                  />
                </div>
              </div>

              <div style={{ display: 'flex', gap: '15px' }}>
                <div className="form-group" style={{ flex: 1 }}>
                  <label>Priority</label>
                  <select 
                    className="input-field"
                    value={taskForm.priority}
                    onChange={e => setTaskForm({ ...taskForm, priority: e.target.value })}
                  >
                    <option value="Low">Low</option>
                    <option value="Medium">Medium</option>
                    <option value="High">High</option>
                    <option value="Critical">Critical</option>
                  </select>
                </div>
                <div className="form-group" style={{ flex: 1 }}>
                  <label>Status</label>
                  <select 
                    className="input-field"
                    value={taskForm.status}
                    onChange={e => setTaskForm({ ...taskForm, status: e.target.value })}
                  >
                    <option value="Pending">Pending</option>
                    <option value="InProgress">In Progress</option>
                    <option value="Completed">Completed</option>
                    <option value="Cancelled">Cancelled</option>
                  </select>
                </div>
              </div>

              <div className="modal-footer">
                <button type="button" className="btn-cancel" onClick={() => setShowTaskModal(false)}>
                  Cancel
                </button>
                <button type="submit" className="btn-primary" style={{ width: 'auto' }}>
                  Save Task
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* MODAL: TASK DETAIL & COMMENTS (EMPLOYEE FOCUS) */}
      {showTaskDetailModal && activeTaskDetail && (
        <div className="modal-overlay">
          <div className="modal-content glass" style={{ maxWidth: '640px' }}>
            <div className="modal-header">
              <div>
                <span className={`badge badge-priority-${activeTaskDetail.priority.toLowerCase()}`} style={{ marginRight: '8px' }}>
                  {activeTaskDetail.priority} Priority
                </span>
                <span className={`badge badge-status-${activeTaskDetail.status.toLowerCase()}`}>
                  {activeTaskDetail.status}
                </span>
              </div>
              <button className="btn-close" onClick={() => setShowTaskDetailModal(false)}>
                <Icons.Close />
              </button>
            </div>
            
            <h2 style={{ marginTop: '10px', fontSize: '26px' }}>{activeTaskDetail.title}</h2>
            <p style={{ color: 'var(--text-muted)', fontSize: '13px', margin: '5px 0 20px' }}>
              Project: <span style={{ color: 'var(--text-secondary)' }}>{activeTaskDetail.projectName}</span> &nbsp;|&nbsp; 
              Due Date: <span style={{ color: 'var(--text-secondary)' }}>{new Date(activeTaskDetail.dueDate).toLocaleDateString()}</span>
            </p>

            <div style={{ minHeight: '100px', background: 'rgba(15,21,39,0.3)', padding: '16px', borderRadius: '10px', border: '1px solid var(--border-color)', fontSize: '15px', lineHeight: '1.6' }}>
              <strong>Description</strong>
              <p style={{ marginTop: '8px', color: 'var(--text-secondary)' }}>
                {activeTaskDetail.description || 'No description provided for this task.'}
              </p>
            </div>

            {/* Comments Feed */}
            <div className="comments-section">
              <h3 style={{ fontSize: '16px', marginBottom: '14px' }}>Discussion Feed ({comments.length})</h3>
              
              <div className="comments-list">
                {comments.map(c => (
                  <div key={c.id} className="comment-item">
                    <div className="comment-meta">
                      <span className="comment-author">{c.userName}</span>
                      <span className="comment-date">{new Date(c.createdAt).toLocaleString()}</span>
                    </div>
                    <p className="comment-text">{c.content}</p>
                  </div>
                ))}
                {comments.length === 0 && (
                  <div style={{ color: 'var(--text-muted)', fontSize: '14px', fontStyle: 'italic', padding: '10px 0' }}>
                    No comments posted yet. Start the discussion below!
                  </div>
                )}
              </div>

              <form onSubmit={postComment} className="comment-input-group">
                <input 
                  type="text" 
                  className="comment-input" 
                  placeholder="Ask a question or provide progress updates..."
                  value={newComment}
                  onChange={e => setNewComment(e.target.value)}
                />
                <button type="submit" className="btn-post-comment">
                  Comment
                </button>
              </form>
            </div>
          </div>
        </div>
      )}

      {/* MODAL: SHARE CREATED USER CREDENTIALS (ADMIN ONLY) */}
      {createdCredentials && (
        <div className="modal-overlay">
          <div className="modal-content glass" style={{ maxWidth: '480px', textAlign: 'center' }}>
            <div style={{ width: '56px', height: '56px', borderRadius: '50%', background: 'rgba(34,197,94,0.15)', color: '#4ade80', display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 16px', fontSize: '24px' }}>
              ✉️
            </div>
            <h2 style={{ fontSize: '22px', marginBottom: '8px' }}>Staff Account Created!</h2>
            <p style={{ color: 'var(--text-secondary)', fontSize: '14px', marginBottom: '20px' }}>
              Share these login credentials with <strong>{createdCredentials.name}</strong> so they can sign in and start working.
            </p>

            <div style={{ background: 'rgba(15,21,39,0.5)', border: '1px solid var(--border-color)', borderRadius: '12px', padding: '16px', textAlign: 'left', marginBottom: '20px', fontSize: '14px' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '8px' }}>
                <span style={{ color: 'var(--text-muted)' }}>Staff Name:</span>
                <span style={{ fontWeight: 600 }}>{createdCredentials.name}</span>
              </div>
              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '8px' }}>
                <span style={{ color: 'var(--text-muted)' }}>Job Role:</span>
                <span style={{ color: '#a5b4fc', fontWeight: 600 }}>{createdCredentials.jobTitle || createdCredentials.role}</span>
              </div>
              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '8px' }}>
                <span style={{ color: 'var(--text-muted)' }}>Email Address:</span>
                <span style={{ fontFamily: 'monospace', fontWeight: 600, color: 'white' }}>{createdCredentials.email}</span>
              </div>
              <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                <span style={{ color: 'var(--text-muted)' }}>Initial Password:</span>
                <span style={{ fontFamily: 'monospace', fontWeight: 700, color: '#f43f5e' }}>{createdCredentials.password}</span>
              </div>
            </div>

            <div style={{ display: 'flex', gap: '10px' }}>
              <button 
                type="button" 
                className="btn-action" 
                style={{ flex: 1, justifyContent: 'center' }}
                onClick={() => {
                  const text = `Welcome to Employee & Task Management System!\n\nHere are your login credentials:\nEmail: ${createdCredentials.email}\nPassword: ${createdCredentials.password}\nRole: ${createdCredentials.jobTitle || createdCredentials.role}\n\nLogin URL: ${window.location.origin}`;
                  navigator.clipboard.writeText(text);
                  setCopied(true);
                  setTimeout(() => setCopied(false), 2000);
                }}
              >
                <Icons.Copy /> {copied ? 'Copied to Clipboard!' : 'Copy Credentials'}
              </button>
              <button 
                type="button" 
                className="btn-primary" 
                style={{ width: 'auto', padding: '0 24px' }}
                onClick={() => setCreatedCredentials(null)}
              >
                Done
              </button>
            </div>
          </div>
        </div>
      )}

      {/* MODAL: SELF PROFILE & CHANGE PASSWORD (ALL LOGGED-IN USERS) */}
      {showProfileModal && user && (
        <div className="modal-overlay">
          <div className="modal-content glass" style={{ maxWidth: '520px' }}>
            <div className="modal-header">
              <h2>My Profile & Security</h2>
              <button className="btn-close" onClick={() => setShowProfileModal(false)}>
                <Icons.Close />
              </button>
            </div>

            {profileError && <div className="error-alert">{profileError}</div>}
            {profileSuccess && <div style={{ background: 'rgba(34,197,94,0.15)', color: '#4ade80', padding: '12px', borderRadius: '8px', marginBottom: '16px', fontSize: '14px', border: '1px solid rgba(34,197,94,0.3)' }}>{profileSuccess}</div>}

            <form onSubmit={saveProfile}>
              <div style={{ background: 'rgba(15,21,39,0.3)', padding: '14px 16px', borderRadius: '10px', border: '1px solid var(--border-color)', marginBottom: '20px' }}>
                <div style={{ fontWeight: 600, fontSize: '16px' }}>{user.firstName} {user.lastName}</div>
                <div style={{ color: 'var(--text-muted)', fontSize: '13px', marginTop: '2px' }}>{user.email} &nbsp;·&nbsp; <span style={{ color: '#818cf8', fontWeight: 600 }}>{user.role}</span></div>
              </div>

              <div className="form-group">
                <label>Job Role / Specialization</label>
                <select 
                  className="input-field"
                  value={['Backend Developer', 'Frontend Developer', 'Application Developer'].includes(profileForm.jobTitle) ? profileForm.jobTitle : (profileForm.jobTitle ? 'Other' : 'Backend Developer')}
                  onChange={e => {
                    const val = e.target.value;
                    if (val === 'Other') {
                      setProfileForm({ ...profileForm, jobTitle: '' });
                    } else {
                      setProfileForm({ ...profileForm, jobTitle: val });
                    }
                  }}
                >
                  <option value="Backend Developer">Backend Developer</option>
                  <option value="Frontend Developer">Frontend Developer</option>
                  <option value="Application Developer">Application Developer</option>
                  <option value="Other">Other... (Type Custom Role)</option>
                </select>

                {(!['Backend Developer', 'Frontend Developer', 'Application Developer'].includes(profileForm.jobTitle) || profileForm.jobTitle === '') && (
                  <input 
                    type="text" 
                    className="input-field" 
                    style={{ marginTop: '8px' }}
                    placeholder="Type custom role (e.g. Fullstack, UI/UX, DevOps)..."
                    value={profileForm.jobTitle}
                    onChange={e => setProfileForm({ ...profileForm, jobTitle: e.target.value })}
                    required
                  />
                )}
                <span style={{ fontSize: '12px', color: 'var(--text-muted)', marginTop: '4px', display: 'block' }}>
                  This title appears to the admin when assigning tasks.
                </span>
              </div>

              <hr style={{ borderColor: 'var(--border-color)', margin: '20px 0', opacity: 0.5 }} />

              <h3 style={{ fontSize: '15px', marginBottom: '12px' }}>🔑 Change Password</h3>
              <div className="form-group">
                <label>Current Password</label>
                <input 
                  type="password" 
                  className="input-field" 
                  placeholder="Required only if changing password"
                  value={profileForm.currentPassword}
                  onChange={e => setProfileForm({ ...profileForm, currentPassword: e.target.value })}
                />
              </div>

              <div style={{ display: 'flex', gap: '15px' }}>
                <div className="form-group" style={{ flex: 1 }}>
                  <label>New Password</label>
                  <input 
                    type="password" 
                    className="input-field" 
                    placeholder="Min 6 characters"
                    value={profileForm.newPassword}
                    onChange={e => setProfileForm({ ...profileForm, newPassword: e.target.value })}
                  />
                </div>
                <div className="form-group" style={{ flex: 1 }}>
                  <label>Confirm New Password</label>
                  <input 
                    type="password" 
                    className="input-field" 
                    placeholder="Re-type new password"
                    value={profileForm.confirmPassword}
                    onChange={e => setProfileForm({ ...profileForm, confirmPassword: e.target.value })}
                  />
                </div>
              </div>

              <div className="modal-footer" style={{ marginTop: '24px' }}>
                <button type="button" className="btn-cancel" onClick={() => setShowProfileModal(false)}>
                  Cancel
                </button>
                <button type="submit" className="btn-primary" style={{ width: 'auto' }}>
                  Save Profile & Password
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
