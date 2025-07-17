import React, { useState } from 'react';
import { PlusIcon, MagnifyingGlassIcon, DocumentTextIcon } from '@heroicons/react/24/outline';
import { Card, CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { formatDate, formatCurrency, getStatusColor } from '../lib/utils';

// Mock data
const mockInvoices = [
  {
    id: 1,
    invoiceNumber: 'INV-2024-001',
    client: { companyName: 'TechCorp Inc.' },
    project: { name: 'E-commerce Website' },
    invoiceDate: '2024-01-15T00:00:00Z',
    dueDate: '2024-02-15T00:00:00Z',
    totalAmount: 5500,
    paidAmount: 5500,
    outstandingAmount: 0,
    status: 'Paid',
    isOverdue: false,
    paidAt: '2024-01-20T00:00:00Z',
  },
  {
    id: 2,
    invoiceNumber: 'INV-2024-002',
    client: { companyName: 'StartupXYZ' },
    project: { name: 'Mobile App Design' },
    invoiceDate: '2024-01-20T00:00:00Z',
    dueDate: '2024-02-20T00:00:00Z',
    totalAmount: 3200,
    paidAmount: 0,
    outstandingAmount: 3200,
    status: 'Sent',
    isOverdue: false,
  },
  {
    id: 3,
    invoiceNumber: 'INV-2024-003',
    client: { companyName: 'Creative Agency' },
    project: { name: 'Brand Identity' },
    invoiceDate: '2024-01-10T00:00:00Z',
    dueDate: '2024-01-25T00:00:00Z',
    totalAmount: 2800,
    paidAmount: 0,
    outstandingAmount: 2800,
    status: 'Overdue',
    isOverdue: true,
  },
  {
    id: 4,
    invoiceNumber: 'INV-2024-004',
    client: { companyName: 'TechCorp Inc.' },
    project: { name: 'Website Maintenance' },
    invoiceDate: '2024-01-25T00:00:00Z',
    dueDate: '2024-02-25T00:00:00Z',
    totalAmount: 1200,
    paidAmount: 0,
    outstandingAmount: 1200,
    status: 'Draft',
    isOverdue: false,
  },
];

export function Invoices() {
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');

  const filteredInvoices = mockInvoices.filter(invoice => {
    const matchesSearch = invoice.invoiceNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         invoice.client.companyName.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         (invoice.project?.name || '').toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = statusFilter === 'all' || invoice.status === statusFilter;
    return matchesSearch && matchesStatus;
  });

  const totalOutstanding = mockInvoices.reduce((sum, invoice) => sum + invoice.outstandingAmount, 0);
  const totalPaid = mockInvoices.reduce((sum, invoice) => sum + invoice.paidAmount, 0);
  const overdueCount = mockInvoices.filter(invoice => invoice.isOverdue).length;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Invoices</h1>
          <p className="text-gray-600">Manage your invoices and track payments.</p>
        </div>
        <Button>
          <PlusIcon className="w-4 h-4 mr-2" />
          Create Invoice
        </Button>
      </div>

      {/* Summary Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <Card>
          <CardContent className="text-center">
            <div className="text-2xl font-bold text-orange-600">
              {formatCurrency(totalOutstanding)}
            </div>
            <p className="text-sm text-gray-600">Outstanding</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="text-center">
            <div className="text-2xl font-bold text-green-600">
              {formatCurrency(totalPaid)}
            </div>
            <p className="text-sm text-gray-600">Paid This Month</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="text-center">
            <div className="text-2xl font-bold text-red-600">
              {overdueCount}
            </div>
            <p className="text-sm text-gray-600">Overdue</p>
          </CardContent>
        </Card>
      </div>

      {/* Search and Filters */}
      <Card>
        <CardContent>
          <div className="flex items-center space-x-4">
            <div className="flex-1 relative">
              <MagnifyingGlassIcon className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
              <input
                type="text"
                placeholder="Search invoices..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              />
            </div>
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
            >
              <option value="all">All Status</option>
              <option value="Draft">Draft</option>
              <option value="Sent">Sent</option>
              <option value="Paid">Paid</option>
              <option value="Overdue">Overdue</option>
            </select>
          </div>
        </CardContent>
      </Card>

      {/* Invoices List */}
      <Card>
        <CardHeader>
          <CardTitle>All Invoices</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {filteredInvoices.map((invoice) => (
              <div key={invoice.id} className="flex items-center justify-between p-4 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors">
                <div className="flex items-center space-x-4">
                  <div className="flex items-center justify-center w-10 h-10 bg-blue-100 rounded-lg">
                    <DocumentTextIcon className="w-5 h-5 text-blue-600" />
                  </div>
                  <div>
                    <h4 className="font-medium text-gray-900">{invoice.invoiceNumber}</h4>
                    <p className="text-sm text-gray-600">{invoice.client.companyName}</p>
                    {invoice.project && (
                      <p className="text-xs text-gray-500">{invoice.project.name}</p>
                    )}
                  </div>
                </div>

                <div className="flex items-center space-x-6">
                  <div className="text-center">
                    <div className="text-sm text-gray-600">Amount</div>
                    <div className="font-semibold text-gray-900">
                      {formatCurrency(invoice.totalAmount)}
                    </div>
                  </div>

                  <div className="text-center">
                    <div className="text-sm text-gray-600">Due Date</div>
                    <div className="text-sm text-gray-900">
                      {formatDate(invoice.dueDate)}
                    </div>
                  </div>

                  <div className="text-center">
                    <Badge className={getStatusColor(invoice.status)}>
                      {invoice.status}
                    </Badge>
                  </div>

                  <div className="flex space-x-2">
                    <Button variant="outline" size="sm">
                      View
                    </Button>
                    {invoice.status === 'Draft' && (
                      <Button size="sm">
                        Send
                      </Button>
                    )}
                    {invoice.status === 'Sent' && (
                      <Button variant="outline" size="sm">
                        Mark Paid
                      </Button>
                    )}
                  </div>
                </div>
              </div>
            ))}
          </div>

          {filteredInvoices.length === 0 && (
            <div className="text-center py-12">
              <DocumentTextIcon className="w-12 h-12 text-gray-400 mx-auto mb-4" />
              <p className="text-lg font-medium text-gray-900">No invoices found</p>
              <p className="text-gray-600 mt-1">Try adjusting your search criteria or create a new invoice.</p>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}