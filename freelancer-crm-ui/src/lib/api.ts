import axios from 'axios';
import type { AuthResponse, LoginRequest, RegisterRequest, User, Client, Project, Assignment, TimeEntry, Invoice } from '../types';

const API_BASE_URL = 'http://localhost:5172/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add auth token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor to handle auth errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Auth API
export const authApi = {
  login: (data: LoginRequest) => api.post<AuthResponse>('/user/login', data),
  register: (data: RegisterRequest) => api.post<AuthResponse>('/user/register', data),
  refreshToken: (refreshToken: string) => api.post('/user/refresh-token', { refreshToken }),
  logout: () => api.post('/user/revoke-all-tokens'),
};

// Users API
export const usersApi = {
  getProfile: () => api.get<User>('/user/me'),
  updateProfile: (id: number, data: Partial<User>) => api.put<User>(`/user/${id}`, data),
  changePassword: (data: { currentPassword: string; newPassword: string; confirmPassword: string }) => 
    api.post('/user/change-password', data),
};

// Clients API
export const clientsApi = {
  getAll: () => api.get<Client[]>('/client'),
  getById: (id: number) => api.get<Client>(`/client/${id}`),
  create: (data: Omit<Client, 'id' | 'createdAt' | 'updatedAt'>) => api.post<Client>('/client', data),
  update: (id: number, data: Partial<Client>) => api.put<Client>(`/client/${id}`, data),
  delete: (id: number) => api.delete(`/client/${id}`),
};

// Projects API
export const projectsApi = {
  getAll: () => api.get<Project[]>('/project'),
  getById: (id: number) => api.get<Project>(`/project/${id}`),
  create: (data: Omit<Project, 'id' | 'createdAt' | 'updatedAt'>) => api.post<Project>('/project', data),
  update: (id: number, data: Partial<Project>) => api.put<Project>(`/project/${id}`, data),
  delete: (id: number) => api.delete(`/project/${id}`),
  getActive: () => api.get<Project[]>('/project/active'),
  getByClient: (clientId: number) => api.get<Project[]>(`/project/client/${clientId}`),
};

// Assignments API
export const assignmentsApi = {
  getAll: () => api.get<Assignment[]>('/assignment'),
  getById: (id: number) => api.get<Assignment>(`/assignment/${id}`),
  create: (data: Omit<Assignment, 'id' | 'createdAt' | 'updatedAt'>) => api.post<Assignment>('/assignment', data),
  update: (id: number, data: Partial<Assignment>) => api.put<Assignment>(`/assignment/${id}`, data),
  delete: (id: number) => api.delete(`/assignment/${id}`),
  getByProject: (projectId: number) => api.get<Assignment[]>(`/assignment/project/${projectId}`),
  getOverdue: () => api.get<Assignment[]>('/assignment/overdue'),
};

// Time Entries API
export const timeEntriesApi = {
  getAll: () => api.get<TimeEntry[]>('/timeentry'),
  getById: (id: number) => api.get<TimeEntry>(`/timeentry/${id}`),
  create: (data: Omit<TimeEntry, 'id' | 'createdAt' | 'updatedAt'>) => api.post<TimeEntry>('/timeentry', data),
  update: (id: number, data: Partial<TimeEntry>) => api.put<TimeEntry>(`/timeentry/${id}`, data),
  delete: (id: number) => api.delete(`/timeentry/${id}`),
  start: (data: { projectId: number; assignmentId?: number }) => api.post<TimeEntry>('/timeentry/start', data),
  stop: (userId: number) => api.post<TimeEntry>(`/timeentry/stop/${userId}`),
  getActive: (userId: number) => api.get<TimeEntry>(`/timeentry/active/${userId}`),
  getByProject: (projectId: number) => api.get<TimeEntry[]>(`/timeentry/project/${projectId}`),
};

// Invoices API
export const invoicesApi = {
  getAll: () => api.get<Invoice[]>('/invoice'),
  getById: (id: number) => api.get<Invoice>(`/invoice/${id}`),
  create: (data: Omit<Invoice, 'id' | 'createdAt' | 'updatedAt'>) => api.post<Invoice>('/invoice', data),
  update: (id: number, data: Partial<Invoice>) => api.put<Invoice>(`/invoice/${id}`, data),
  delete: (id: number) => api.delete(`/invoice/${id}`),
  getByClient: (clientId: number) => api.get<Invoice[]>(`/invoice/client/${clientId}`),
  getOverdue: () => api.get<Invoice[]>('/invoice/overdue'),
  markAsPaid: (id: number, paidDate: string) => api.post(`/invoice/${id}/mark-paid`, { paidDate }),
  send: (id: number) => api.post(`/invoice/${id}/send`),
};

export default api;