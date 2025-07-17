import React, { useState } from 'react';
import { PlusIcon, MagnifyingGlassIcon } from '@heroicons/react/24/outline';
import { Card, CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Badge } from '../components/ui/Badge';
import { Avatar } from '../components/ui/Avatar';
import { formatDate, getPriorityColor, getPriorityLabel } from '../lib/utils';

// Mock data
const mockClients = [
  {
    id: 1,
    companyName: 'TechCorp Inc.',
    contactName: 'John Smith',
    email: 'john@techcorp.com',
    phoneNumber: '+1 (555) 123-4567',
    industry: 'Technology',
    priority: 3,
    isActive: true,
    createdAt: '2024-01-15T10:00:00Z',
    projectsCount: 3,
    totalRevenue: 25000,
  },
  {
    id: 2,
    companyName: 'StartupXYZ',
    contactName: 'Sarah Johnson',
    email: 'sarah@startupxyz.com',
    phoneNumber: '+1 (555) 987-6543',
    industry: 'Fintech',
    priority: 4,
    isActive: true,
    createdAt: '2024-01-20T14:30:00Z',
    projectsCount: 1,
    totalRevenue: 8500,
  },
  {
    id: 3,
    companyName: 'Creative Agency',
    contactName: 'Mike Wilson',
    email: 'mike@creative.com',
    phoneNumber: '+1 (555) 456-7890',
    industry: 'Marketing',
    priority: 2,
    isActive: true,
    createdAt: '2024-01-10T09:15:00Z',
    projectsCount: 2,
    totalRevenue: 15000,
  },
];

export function Clients() {
  const [searchTerm, setSearchTerm] = useState('');
  const [showAddModal, setShowAddModal] = useState(false);

  const filteredClients = mockClients.filter(client =>
    client.companyName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    client.contactName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    client.email.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Clients</h1>
          <p className="text-gray-600">Manage your client relationships and contacts.</p>
        </div>
        <Button onClick={() => setShowAddModal(true)}>
          <PlusIcon className="w-4 h-4 mr-2" />
          Add Client
        </Button>
      </div>

      {/* Search and Filters */}
      <Card>
        <CardContent>
          <div className="flex items-center space-x-4">
            <div className="flex-1 relative">
              <MagnifyingGlassIcon className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
              <input
                type="text"
                placeholder="Search clients..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              />
            </div>
            <Button variant="outline">Filter</Button>
          </div>
        </CardContent>
      </Card>

      {/* Clients Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {filteredClients.map((client) => (
          <Card key={client.id} className="hover:shadow-md transition-shadow cursor-pointer">
            <CardContent>
              <div className="flex items-start justify-between mb-4">
                <div className="flex items-center space-x-3">
                  <Avatar name={client.contactName} size="md" />
                  <div>
                    <h3 className="font-semibold text-gray-900">{client.companyName}</h3>
                    <p className="text-sm text-gray-600">{client.contactName}</p>
                  </div>
                </div>
                <Badge className={getPriorityColor(client.priority)}>
                  {getPriorityLabel(client.priority)}
                </Badge>
              </div>

              <div className="space-y-2 text-sm">
                <div className="flex items-center text-gray-600">
                  <span className="font-medium">Email:</span>
                  <span className="ml-2">{client.email}</span>
                </div>
                <div className="flex items-center text-gray-600">
                  <span className="font-medium">Phone:</span>
                  <span className="ml-2">{client.phoneNumber}</span>
                </div>
                <div className="flex items-center text-gray-600">
                  <span className="font-medium">Industry:</span>
                  <span className="ml-2">{client.industry}</span>
                </div>
              </div>

              <div className="mt-4 pt-4 border-t border-gray-200">
                <div className="flex items-center justify-between text-sm">
                  <div>
                    <span className="text-gray-600">Projects:</span>
                    <span className="ml-1 font-medium">{client.projectsCount}</span>
                  </div>
                  <div>
                    <span className="text-gray-600">Revenue:</span>
                    <span className="ml-1 font-medium">${client.totalRevenue.toLocaleString()}</span>
                  </div>
                </div>
                <div className="mt-2 text-xs text-gray-500">
                  Added {formatDate(client.createdAt)}
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {filteredClients.length === 0 && (
        <Card>
          <CardContent className="text-center py-12">
            <div className="text-gray-500">
              <p className="text-lg font-medium">No clients found</p>
              <p className="mt-1">Try adjusting your search criteria or add a new client.</p>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}