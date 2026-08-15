export type Role = 'ADMIN' | 'MANAGER' | 'EMPLOYEE';

export interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  departmentId?: number;
  role: Role;
  isActive: boolean;
}

export interface Department {
  id: number;
  name: string;
  description?: string;
  userCount: number;
}

export interface Project {
  id: number;
  name: string;
  description?: string;
  status: 'Planning' | 'Active' | 'Completed' | 'Cancelled';
  createdById: number;
  createdByName?: string;
  taskCount: number;
}

export interface TaskItem {
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

export interface TaskComment {
  id: number;
  content: string;
  taskId: number;
  userId: number;
  userName?: string;
  createdAt: string;
}

export interface AuthResponse {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  token: string;
}
