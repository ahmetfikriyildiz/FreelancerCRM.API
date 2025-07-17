import React, { useState } from 'react';
import { PlayIcon, PauseIcon, ClockIcon } from '@heroicons/react/24/outline';
import { Card, CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { formatDate, formatDuration } from '../lib/utils';

// Mock data
const mockActiveEntry = {
  id: 1,
  project: { name: 'E-commerce Website', client: { companyName: 'TechCorp Inc.' } },
  startTime: '2024-01-25T09:00:00Z',
  description: 'Working on product catalog implementation',
  isRunning: true,
  elapsedTime: 125, // minutes
};

const mockTimeEntries = [
  {
    id: 1,
    project: { name: 'E-commerce Website', client: { companyName: 'TechCorp Inc.' } },
    startTime: '2024-01-24T09:00:00Z',
    endTime: '2024-01-24T17:30:00Z',
    duration: 510, // minutes (8.5 hours)
    description: 'Frontend development and API integration',
    isBillable: true,
    hourlyRate: 75,
    grossAmount: 637.5,
  },
  {
    id: 2,
    project: { name: 'Mobile App Design', client: { companyName: 'StartupXYZ' } },
    startTime: '2024-01-23T10:00:00Z',
    endTime: '2024-01-23T15:00:00Z',
    duration: 300, // minutes (5 hours)
    description: 'UI wireframes and mockups',
    isBillable: true,
    hourlyRate: 80,
    grossAmount: 400,
  },
  {
    id: 3,
    project: { name: 'Brand Identity', client: { companyName: 'Creative Agency' } },
    startTime: '2024-01-22T14:00:00Z',
    endTime: '2024-01-22T18:00:00Z',
    duration: 240, // minutes (4 hours)
    description: 'Logo concepts and brand guidelines',
    isBillable: true,
    hourlyRate: 70,
    grossAmount: 280,
  },
];

export function TimeTracking() {
  const [isTracking, setIsTracking] = useState(true);
  const [selectedProject, setSelectedProject] = useState('');

  const totalHoursToday = mockTimeEntries
    .filter(entry => new Date(entry.startTime).toDateString() === new Date().toDateString())
    .reduce((total, entry) => total + entry.duration, 0);

  const totalEarningsToday = mockTimeEntries
    .filter(entry => new Date(entry.startTime).toDateString() === new Date().toDateString())
    .reduce((total, entry) => total + entry.grossAmount, 0);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Time Tracking</h1>
        <p className="text-gray-600">Track your time and manage your productivity.</p>
      </div>

      {/* Active Timer */}
      <Card>
        <CardHeader>
          <CardTitle>Current Session</CardTitle>
        </CardHeader>
        <CardContent>
          {isTracking && mockActiveEntry ? (
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-4">
                <div className="flex items-center justify-center w-12 h-12 bg-green-100 rounded-lg">
                  <ClockIcon className="w-6 h-6 text-green-600" />
                </div>
                <div>
                  <h3 className="font-semibold text-gray-900">{mockActiveEntry.project.name}</h3>
                  <p className="text-sm text-gray-600">{mockActiveEntry.project.client.companyName}</p>
                  <p className="text-sm text-gray-500">{mockActiveEntry.description}</p>
                </div>
              </div>
              <div className="text-right">
                <div className="text-3xl font-bold text-gray-900">
                  {formatDuration(mockActiveEntry.elapsedTime)}
                </div>
                <div className="text-sm text-gray-500">
                  Started at {new Date(mockActiveEntry.startTime).toLocaleTimeString()}
                </div>
                <Button 
                  variant="outline" 
                  className="mt-2"
                  onClick={() => setIsTracking(false)}
                >
                  <PauseIcon className="w-4 h-4 mr-2" />
                  Stop
                </Button>
              </div>
            </div>
          ) : (
            <div className="text-center py-8">
              <ClockIcon className="w-12 h-12 text-gray-400 mx-auto mb-4" />
              <h3 className="text-lg font-medium text-gray-900 mb-2">No active timer</h3>
              <p className="text-gray-600 mb-4">Select a project to start tracking time</p>
              <div className="flex items-center justify-center space-x-4">
                <select 
                  value={selectedProject}
                  onChange={(e) => setSelectedProject(e.target.value)}
                  className="px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary-500"
                >
                  <option value="">Select a project</option>
                  <option value="1">E-commerce Website - TechCorp Inc.</option>
                  <option value="2">Mobile App Design - StartupXYZ</option>
                  <option value="3">Brand Identity - Creative Agency</option>
                </select>
                <Button 
                  disabled={!selectedProject}
                  onClick={() => setIsTracking(true)}
                >
                  <PlayIcon className="w-4 h-4 mr-2" />
                  Start Timer
                </Button>
              </div>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Today's Summary */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <Card>
          <CardContent className="text-center">
            <div className="text-2xl font-bold text-gray-900">
              {formatDuration(totalHoursToday)}
            </div>
            <p className="text-sm text-gray-600">Hours Today</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="text-center">
            <div className="text-2xl font-bold text-gray-900">
              ${totalEarningsToday.toFixed(2)}
            </div>
            <p className="text-sm text-gray-600">Earnings Today</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="text-center">
            <div className="text-2xl font-bold text-gray-900">
              {mockTimeEntries.length}
            </div>
            <p className="text-sm text-gray-600">Sessions</p>
          </CardContent>
        </Card>
      </div>

      {/* Recent Time Entries */}
      <Card>
        <CardHeader>
          <CardTitle>Recent Time Entries</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {mockTimeEntries.map((entry) => (
              <div key={entry.id} className="flex items-center justify-between p-4 border border-gray-200 rounded-lg">
                <div className="flex-1">
                  <h4 className="font-medium text-gray-900">{entry.project.name}</h4>
                  <p className="text-sm text-gray-600">{entry.project.client.companyName}</p>
                  <p className="text-sm text-gray-500 mt-1">{entry.description}</p>
                  <div className="flex items-center space-x-4 mt-2">
                    <Badge variant={entry.isBillable ? 'success' : 'default'}>
                      {entry.isBillable ? 'Billable' : 'Non-billable'}
                    </Badge>
                    <span className="text-sm text-gray-500">
                      {formatDate(entry.startTime)}
                    </span>
                  </div>
                </div>
                <div className="text-right">
                  <div className="text-lg font-semibold text-gray-900">
                    {formatDuration(entry.duration)}
                  </div>
                  <div className="text-sm text-gray-600">
                    ${entry.grossAmount.toFixed(2)}
                  </div>
                  <div className="text-xs text-gray-500">
                    ${entry.hourlyRate}/hr
                  </div>
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}