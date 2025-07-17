import React, { useState } from 'react';
import { PlusIcon, MagnifyingGlassIcon, CalendarIcon } from '@heroicons/react/24/outline';
import { Card, CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { formatDate, formatCurrency, getStatusColor, calculateProgress } from '../lib/utils';

// Mock data
const mockProjects = [
  {
    id: 1,
    name: 'E-commerce Website',
    description: 'Complete redesign and development of online store',
    client: { companyName: 'TechCorp Inc.' },
    startDate: '2024-01-15T00:00:00Z',
    endDate: '2024-02-15T00:00:00Z',
    budget: 15000,
    hourlyRate: 75,
    status: 'InProgress',
    priority: 3,
    totalTimeSpent: 45.5,
    totalEarnings: 3412.5,
    completionPercentage: 75,
  },
  {
    id: 2,
    name: 'Mobile App Design',
    description: 'UI/UX design for iOS and Android application',
    client: { companyName: 'StartupXYZ' },
    startDate: '2024-02-01T00:00:00Z',
    endDate: '2024-03-01T00:00:00Z',
    budget: 8000,
    hourlyRate: 80,
    status: 'Planning',
    priority: 4,
    totalTimeSpent: 12.0,
    totalEarnings: 960,
    completionPercentage: 25,
  },
  {
    id: 3,
    name: 'Brand Identity',
    description: 'Logo design and brand guidelines',
    client: { companyName: 'Creative Agency' },
    startDate: '2024-01-01T00:00:00Z',
    endDate: '2024-01-30T00:00:00Z',
    budget: 5000,
    hourlyRate: 70,
    status: 'Completed',
    priority: 2,
    totalTimeSpent: 35.0,
    totalEarnings: 2450,
    completionPercentage: 100,
  },
];

export function Projects() {
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');

  const filteredProjects = mockProjects.filter(project => {
    const matchesSearch = project.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         project.client.companyName.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = statusFilter === 'all' || project.status === statusFilter;
    return matchesSearch && matchesStatus;
  });

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Projects</h1>
          <p className="text-gray-600">Track and manage your ongoing projects.</p>
        </div>
        <Button>
          <PlusIcon className="w-4 h-4 mr-2" />
          New Project
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
                placeholder="Search projects..."
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
              <option value="Planning">Planning</option>
              <option value="InProgress">In Progress</option>
              <option value="OnHold">On Hold</option>
              <option value="Completed">Completed</option>
            </select>
          </div>
        </CardContent>
      </Card>

      {/* Projects List */}
      <div className="space-y-4">
        {filteredProjects.map((project) => (
          <Card key={project.id} className="hover:shadow-md transition-shadow cursor-pointer">
            <CardContent>
              <div className="flex items-start justify-between mb-4">
                <div className="flex-1">
                  <div className="flex items-center space-x-3 mb-2">
                    <h3 className="text-lg font-semibold text-gray-900">{project.name}</h3>
                    <Badge className={getStatusColor(project.status)}>
                      {project.status}
                    </Badge>
                  </div>
                  <p className="text-gray-600 mb-2">{project.description}</p>
                  <p className="text-sm text-gray-500">Client: {project.client.companyName}</p>
                </div>
                <div className="text-right">
                  <div className="text-2xl font-bold text-gray-900">
                    {project.completionPercentage}%
                  </div>
                  <div className="text-sm text-gray-500">Complete</div>
                </div>
              </div>

              <div className="mb-4">
                <div className="flex items-center justify-between text-sm text-gray-600 mb-1">
                  <span>Progress</span>
                  <span>{project.completionPercentage}%</span>
                </div>
                <div className="w-full bg-gray-200 rounded-full h-2">
                  <div 
                    className="bg-primary-600 h-2 rounded-full transition-all duration-300" 
                    style={{ width: `${project.completionPercentage}%` }}
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
                <div>
                  <span className="text-gray-600">Budget:</span>
                  <div className="font-medium">{formatCurrency(project.budget)}</div>
                </div>
                <div>
                  <span className="text-gray-600">Earned:</span>
                  <div className="font-medium">{formatCurrency(project.totalEarnings)}</div>
                </div>
                <div>
                  <span className="text-gray-600">Hours:</span>
                  <div className="font-medium">{project.totalTimeSpent}h</div>
                </div>
                <div>
                  <span className="text-gray-600">Rate:</span>
                  <div className="font-medium">{formatCurrency(project.hourlyRate)}/hr</div>
                </div>
              </div>

              <div className="mt-4 pt-4 border-t border-gray-200 flex items-center justify-between text-sm text-gray-500">
                <div className="flex items-center">
                  <CalendarIcon className="w-4 h-4 mr-1" />
                  {formatDate(project.startDate)} - {formatDate(project.endDate)}
                </div>
                <div className="flex space-x-2">
                  <Button variant="outline" size="sm">View</Button>
                  <Button variant="outline" size="sm">Edit</Button>
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {filteredProjects.length === 0 && (
        <Card>
          <CardContent className="text-center py-12">
            <div className="text-gray-500">
              <p className="text-lg font-medium">No projects found</p>
              <p className="mt-1">Try adjusting your search criteria or create a new project.</p>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}