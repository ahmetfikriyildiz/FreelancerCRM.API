import React from 'react';
import { Card, CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import { Badge } from '../components/ui/Badge';
import { formatCurrency, formatDate, getStatusColor } from '../lib/utils';
import { 
  CurrencyDollarIcon, 
  ClockIcon, 
  DocumentTextIcon, 
  UsersIcon,
  TrendingUpIcon,
  CalendarIcon
} from '@heroicons/react/24/outline';

// Mock data - replace with real API calls
const mockStats = {
  totalRevenue: 45250,
  activeProjects: 8,
  hoursThisMonth: 156,
  pendingInvoices: 3,
  revenueGrowth: 12.5,
  hoursGrowth: 8.2,
};

const mockRecentProjects = [
  {
    id: 1,
    name: 'E-commerce Website',
    client: 'TechCorp Inc.',
    status: 'InProgress',
    progress: 75,
    dueDate: '2024-02-15',
  },
  {
    id: 2,
    name: 'Mobile App Design',
    client: 'StartupXYZ',
    status: 'Planning',
    progress: 25,
    dueDate: '2024-03-01',
  },
  {
    id: 3,
    name: 'Brand Identity',
    client: 'Creative Agency',
    status: 'Completed',
    progress: 100,
    dueDate: '2024-01-30',
  },
];

const mockRecentInvoices = [
  {
    id: 1,
    number: 'INV-2024-001',
    client: 'TechCorp Inc.',
    amount: 5500,
    status: 'Paid',
    dueDate: '2024-01-15',
  },
  {
    id: 2,
    number: 'INV-2024-002',
    client: 'StartupXYZ',
    amount: 3200,
    status: 'Sent',
    dueDate: '2024-02-01',
  },
  {
    id: 3,
    number: 'INV-2024-003',
    client: 'Creative Agency',
    amount: 2800,
    status: 'Overdue',
    dueDate: '2024-01-25',
  },
];

export function Dashboard() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600">Welcome back! Here's what's happening with your business.</p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <Card>
          <CardContent className="flex items-center">
            <div className="flex items-center justify-center w-12 h-12 bg-green-100 rounded-lg">
              <CurrencyDollarIcon className="w-6 h-6 text-green-600" />
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-600">Total Revenue</p>
              <div className="flex items-center">
                <p className="text-2xl font-bold text-gray-900">
                  {formatCurrency(mockStats.totalRevenue)}
                </p>
                <span className="ml-2 flex items-center text-sm text-green-600">
                  <TrendingUpIcon className="w-4 h-4 mr-1" />
                  {mockStats.revenueGrowth}%
                </span>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="flex items-center">
            <div className="flex items-center justify-center w-12 h-12 bg-blue-100 rounded-lg">
              <DocumentTextIcon className="w-6 h-6 text-blue-600" />
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-600">Active Projects</p>
              <p className="text-2xl font-bold text-gray-900">{mockStats.activeProjects}</p>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="flex items-center">
            <div className="flex items-center justify-center w-12 h-12 bg-purple-100 rounded-lg">
              <ClockIcon className="w-6 h-6 text-purple-600" />
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-600">Hours This Month</p>
              <div className="flex items-center">
                <p className="text-2xl font-bold text-gray-900">{mockStats.hoursThisMonth}</p>
                <span className="ml-2 flex items-center text-sm text-green-600">
                  <TrendingUpIcon className="w-4 h-4 mr-1" />
                  {mockStats.hoursGrowth}%
                </span>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="flex items-center">
            <div className="flex items-center justify-center w-12 h-12 bg-orange-100 rounded-lg">
              <UsersIcon className="w-6 h-6 text-orange-600" />
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-600">Pending Invoices</p>
              <p className="text-2xl font-bold text-gray-900">{mockStats.pendingInvoices}</p>
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Recent Projects */}
        <Card>
          <CardHeader>
            <CardTitle>Recent Projects</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {mockRecentProjects.map((project) => (
                <div key={project.id} className="flex items-center justify-between p-4 border border-gray-200 rounded-lg">
                  <div className="flex-1">
                    <h4 className="font-medium text-gray-900">{project.name}</h4>
                    <p className="text-sm text-gray-600">{project.client}</p>
                    <div className="mt-2 flex items-center space-x-4">
                      <Badge className={getStatusColor(project.status)}>
                        {project.status}
                      </Badge>
                      <div className="flex items-center text-sm text-gray-500">
                        <CalendarIcon className="w-4 h-4 mr-1" />
                        {formatDate(project.dueDate)}
                      </div>
                    </div>
                  </div>
                  <div className="ml-4 text-right">
                    <div className="text-sm font-medium text-gray-900">{project.progress}%</div>
                    <div className="w-20 bg-gray-200 rounded-full h-2 mt-1">
                      <div 
                        className="bg-primary-600 h-2 rounded-full" 
                        style={{ width: `${project.progress}%` }}
                      />
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Recent Invoices */}
        <Card>
          <CardHeader>
            <CardTitle>Recent Invoices</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {mockRecentInvoices.map((invoice) => (
                <div key={invoice.id} className="flex items-center justify-between p-4 border border-gray-200 rounded-lg">
                  <div className="flex-1">
                    <h4 className="font-medium text-gray-900">{invoice.number}</h4>
                    <p className="text-sm text-gray-600">{invoice.client}</p>
                    <div className="mt-2 flex items-center space-x-4">
                      <Badge className={getStatusColor(invoice.status)}>
                        {invoice.status}
                      </Badge>
                      <div className="flex items-center text-sm text-gray-500">
                        <CalendarIcon className="w-4 h-4 mr-1" />
                        Due {formatDate(invoice.dueDate)}
                      </div>
                    </div>
                  </div>
                  <div className="ml-4 text-right">
                    <div className="text-lg font-semibold text-gray-900">
                      {formatCurrency(invoice.amount)}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}