export interface User {
  id: number;
  username: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone?: string;
  profilePicture?: string;
  timezone?: string;
  role: UserRole;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export enum UserRole {
  Admin = 1,
  Freelancer = 2,
  Client = 3
}

export interface Client {
  id: number;
  companyName: string;
  contactName: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  industry?: string;
  priority: number;
  isActive: boolean;
  isArchived: boolean;
  createdAt: string;
  updatedAt?: string;
  projects?: Project[];
  invoices?: Invoice[];
}

export interface Project {
  id: number;
  name: string;
  description?: string;
  clientId: number;
  userId: number;
  startDate: string;
  endDate?: string;
  budget: number;
  hourlyRate: number;
  status: ProjectStatus;
  priority: number;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
  client?: Client;
  user?: User;
  assignments?: Assignment[];
  timeEntries?: TimeEntry[];
  totalTimeSpent: number;
  totalEarnings: number;
  remainingBudget: number;
  completionPercentage: number;
}

export enum ProjectStatus {
  Planning = 1,
  InProgress = 2,
  OnHold = 3,
  Completed = 4,
  Cancelled = 5
}

export interface Assignment {
  id: number;
  taskName: string;
  description?: string;
  projectId: number;
  userId: number;
  startDate: string;
  dueDate?: string;
  estimatedHours: number;
  actualHours: number;
  status: AssignmentStatus;
  priority: Priority;
  notes?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
  completedAt?: string;
  project?: Project;
  user?: User;
  timeEntries?: TimeEntry[];
  completionPercentage: number;
  isOverdue: boolean;
  daysRemaining: number;
}

export enum AssignmentStatus {
  NotStarted = 1,
  InProgress = 2,
  Completed = 3,
  OnHold = 4,
  Cancelled = 5
}

export enum Priority {
  Low = 1,
  Medium = 2,
  High = 3,
  Critical = 4,
  Urgent = 5
}

export interface TimeEntry {
  id: number;
  projectId: number;
  assignmentId?: number;
  userId: number;
  startTime: string;
  endTime?: string;
  duration: number;
  description?: string;
  isBillable: boolean;
  hourlyRate: number;
  grossAmount: number;
  withholding TaxRate: number;
  withholdingTaxAmount: number;
  netAmount: number;
  notes?: string;
  createdAt: string;
  updatedAt?: string;
  project?: Project;
  assignment?: Assignment;
  user?: User;
  isRunning: boolean;
  formattedDuration: string;
}

export interface Invoice {
  id: number;
  clientId: number;
  userId: number;
  projectId?: number;
  invoiceNumber: string;
  invoiceDate: string;
  dueDate: string;
  description?: string;
  subTotal: number;
  vatRate: number;
  vatAmount: number;
  discountRate: number;
  discountAmount: number;
  totalAmount: number;
  paidAmount: number;
  outstandingAmount: number;
  status: InvoiceStatus;
  notes?: string;
  createdAt: string;
  updatedAt?: string;
  paidAt?: string;
  client?: Client;
  user?: User;
  project?: Project;
  items?: InvoiceItem[];
  isOverdue: boolean;
  daysOverdue: number;
  daysUntilDue: number;
  formattedInvoiceNumber: string;
  statusText: string;
}

export enum InvoiceStatus {
  Draft = 1,
  Sent = 2,
  Paid = 3,
  Overdue = 4,
  Cancelled = 5
}

export interface InvoiceItem {
  id: number;
  invoiceId: number;
  description: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  unit?: string;
  notes?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  phoneNumber?: string;
}

export interface ApiResponse<T> {
  data?: T;
  message?: string;
  errors?: string[];
  success: boolean;
}