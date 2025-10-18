import { api } from './api';

// Analytics API service for dashboard and reporting functionality
export const analyticsService = {
  // Get dashboard statistics
  async getDashboardStats(portId?: number) {
    try {
      const queryParam = portId ? `?portId=${portId}` : '';
      const response = await api.get(`/analytics/dashboard-stats${queryParam}`);
      console.log('Dashboard stats API response:', response.data);
      return response.data;
    } catch (error) {
      console.warn('Dashboard stats API not available, using fallback data:', error);
      // Return fallback data with structure matching DashboardStatsDto
      return {
        data: {
          totalContainers: portId ? 4235 : 12547,
          activeShips: portId ? 8 : 23,
          availableBerths: portId ? 3 : 7,
          totalPorts: portId ? 1 : 3,
          todayArrivals: portId ? 3 : 8,
          todayDepartures: portId ? 2 : 5,
          containersInTransit: portId ? 950 : 2847,
          containersAtPort: portId ? 3285 : 9700,
          averageTurnaroundTime: 18.5,
          berthUtilizationRate: portId ? 85.5 : 78.5,
          recentActivities: [
            {
              activity: 'Container Loading',
              description: 'MSC Oscar - 150 containers loaded',
              timestamp: new Date(Date.now() - 45 * 60000).toISOString(),
              type: 'loading',
              entityId: '2',
              entityName: 'MSC Oscar'
            },
            {
              activity: 'Ship Arrival',
              description: 'Ever Given arrived at Berth 3',
              timestamp: new Date(Date.now() - 2 * 60 * 60000).toISOString(),
              type: 'arrival',
              entityId: '4',
              entityName: 'Ever Given'
            },
            {
              activity: 'Container Unloading',
              description: 'CMA CGM Antoine de Saint Exupery - 200 containers unloaded',
              timestamp: new Date(Date.now() - 3 * 60 * 60000).toISOString(),
              type: 'unloading',
              entityId: '3',
              entityName: 'CMA CGM Antoine de Saint Exupery'
            },
            {
              activity: 'Ship Departure',
              description: 'OOCL Hong Kong departed from Berth 1',
              timestamp: new Date(Date.now() - 4 * 60 * 60000).toISOString(),
              type: 'departure',
              entityId: '1',
              entityName: 'OOCL Hong Kong'
            }
          ],
          alerts: [
            {
              id: 1,
              title: 'High Berth Utilization',
              message: 'Port approaching capacity - 15 of 18 berths occupied',
              severity: 'Warning',
              createdAt: new Date(Date.now() - 30 * 60000).toISOString(),
              isRead: false
            },
            {
              id: 2,
              title: 'Weather Advisory',
              message: 'Strong winds expected in 2 hours - secure all cargo operations',
              severity: 'Info',
              createdAt: new Date(Date.now() - 60 * 60000).toISOString(),
              isRead: false
            },
            {
              id: 3,
              title: 'Delayed Arrival',
              message: 'MSC Gulsun delayed by 3 hours due to port congestion',
              severity: 'Warning',
              createdAt: new Date(Date.now() - 90 * 60000).toISOString(),
              isRead: true
            }
          ]
        }
      };
    }
  },

  // Get containers by port
  async getContainersByPort(portId?: number) {
    try {
      const queryParam = portId ? `?portId=${portId}` : '';
      const response = await api.get(`/analytics/containers-by-port${queryParam}`);
      return response.data;
    } catch (error) {
      console.warn('Containers API not available, using fallback data');
      // Return demo containers data based on port
      const baseContainers = [
        { containerId: 'MSCU1234567', type: 'Dry', status: 'At Port', cargoType: 'Electronics', weight: 24500, currentLocation: 'Port A', size: '40ft' },
        { containerId: 'MSCU1234568', type: 'Refrigerated', status: 'In Transit', cargoType: 'Food', weight: 22000, currentLocation: 'Port A', size: '20ft' },
        { containerId: 'MSCU1234569', type: 'Tank', status: 'Loading', cargoType: 'Chemicals', weight: 26000, currentLocation: 'Port B', size: '40ft' },
        { containerId: 'MSCU1234570', type: 'Dry', status: 'Available', cargoType: 'Machinery', weight: 21000, currentLocation: 'Port B', size: '20ft' },
        { containerId: 'MSCU1234571', type: 'Dry', status: 'At Port', cargoType: 'Automotive', weight: 23500, currentLocation: 'Port C', size: '40ft' }
      ];
      
      if (portId) {
        const portName = `Port ${String.fromCharCode(64 + portId)}`; // Port A, Port B, Port C
        const filteredContainers = baseContainers.filter(c => c.currentLocation === portName);
        return { data: filteredContainers };
      }
      
      return { data: baseContainers };
    }
  },

  // Get berths by port
  async getBerthsByPort(portId?: number) {
    try {
      const queryParam = portId ? `?portId=${portId}` : '';
      const response = await api.get(`/analytics/berths-by-port${queryParam}`);
      return response.data;
    } catch (error) {
      console.warn('Berths API not available, using fallback data');
      // Return demo berths data based on port
      const allBerths = [
        { berthId: 1, name: 'Berth A1', status: 'Available', type: 'Container', capacity: 150, currentLoad: 0, portId: 1, portName: 'Port A' },
        { berthId: 2, name: 'Berth A2', status: 'Occupied', type: 'Container', capacity: 200, currentLoad: 180, portId: 1, portName: 'Port A' },
        { berthId: 3, name: 'Berth B1', status: 'Under Maintenance', type: 'Bulk', capacity: 100, currentLoad: 0, portId: 2, portName: 'Port B' },
        { berthId: 4, name: 'Berth B2', status: 'Available', type: 'General', capacity: 175, currentLoad: 45, portId: 2, portName: 'Port B' },
        { berthId: 5, name: 'Berth C1', status: 'Occupied', type: 'Oil', capacity: 120, currentLoad: 95, portId: 3, portName: 'Port C' }
      ];
      
      if (portId) {
        const filteredBerths = allBerths.filter(b => b.portId === portId);
        return { data: filteredBerths };
      }
      
      return { data: allBerths };
    }
  },

  // Get container throughput data
  async getContainerThroughput(period = 'daily', days = 30) {
    try {
      const response = await api.get(`/analytics/throughput?period=${period}&days=${days}`);
      return response.data;
    } catch (error) {
      console.warn('Container throughput API not available');
      return { data: [] };
    }
  },

  // Get berth utilization data
  async getBerthUtilization(portId?: number, days = 30) {
    try {
      const url = portId 
        ? `/analytics/berth-utilization?portId=${portId}&days=${days}`
        : `/analytics/berth-utilization?days=${days}`;
      const response = await api.get(url);
      return response.data;
    } catch (error) {
      console.warn('Berth utilization API not available');
      return { data: [] };
    }
  },

  // Get real-time metrics
  async getRealtimeMetrics(portId?: number) {
    try {
      const queryParam = portId ? `?portId=${portId}` : '';
      const response = await api.get(`/analytics/realtime-metrics${queryParam}`);
      return response.data;
    } catch (error) {
      console.warn('Real-time metrics API not available');
      return { 
        data: {
          shipsInPort: portId ? 3 : 8,
          containersProcessingToday: portId ? 245 : 687,
          berthsOccupied: portId ? 4 : 12,
          averageWaitTime: 2.5,
          throughputRate: portId ? 85 : 92,
          lastUpdated: new Date().toISOString()
        }
      };
    }
  },

  // Get port-specific comprehensive statistics
  async getPortSpecificStats(portId: number) {
    try {
      // Try to get comprehensive port data in one call
      const [dashboardStats, containers, berths, realtimeMetrics] = await Promise.allSettled([
        this.getDashboardStats(portId),
        this.getContainersByPort(portId),
        this.getBerthsByPort(portId),
        this.getRealtimeMetrics(portId)
      ]);

      return {
        success: true,
        data: {
          dashboard: dashboardStats.status === 'fulfilled' ? dashboardStats.value : null,
          containers: containers.status === 'fulfilled' ? containers.value : { data: [] },
          berths: berths.status === 'fulfilled' ? berths.value : { data: [] },
          realtime: realtimeMetrics.status === 'fulfilled' ? realtimeMetrics.value : { data: null }
        }
      };
    } catch (error) {
      console.error('Error fetching comprehensive port statistics:', error);
      return {
        success: false,
        error: error instanceof Error ? error.message : 'Failed to fetch port statistics'
      };
    }
  },

  // Generate custom report
  async generateCustomReport(request: {
    reportType: string;
    fromDate: string;
    toDate: string;
    portIds?: number[];
    berthIds?: number[];
    containerTypes?: string[];
    granularity?: string;
    metrics: string[];
    filters?: Record<string, any>;
  }) {
    try {
      const response = await api.post('/analytics/custom-report', request);
      return response.data;
    } catch (error) {
      console.warn('Custom report API not available');
      return { data: null };
    }
  },

  // Export analytics data
  async exportAnalytics(reportType: string, fromDate: string, toDate: string) {
    try {
      const response = await api.get(`/analytics/export?reportType=${reportType}&fromDate=${fromDate}&toDate=${toDate}`, {
        responseType: 'blob'
      });
      return response.data;
    } catch (error) {
      console.warn('Export analytics API not available');
      throw error;
    }
  },

  // Helper method to calculate financial metrics from berth data
  calculateFinancialMetrics(berths: any[], berthAssignments: any[]) {
    try {
      let totalBerthCharges = 0;
      let totalServiceCharges = 0;

      // Calculate berth charges based on hourly rates and assignments
      berthAssignments.forEach(assignment => {
        const berth = berths.find(b => b.berthId === assignment.berthId || b.id === assignment.berthId);
        if (berth && berth.hourlyRate) {
          // Assume average 8 hours per assignment for demo
          const hoursAssigned = 8;
          totalBerthCharges += berth.hourlyRate * hoursAssigned;
          
          // Add service charges (estimated 20% of berth charges)
          totalServiceCharges += (berth.hourlyRate * hoursAssigned) * 0.2;
        }
      });

      // Format as Indian Rupees
      const formatCurrency = (amount: number) => {
        if (amount >= 10000000) { // 1 crore
          return `₹${(amount / 10000000).toFixed(1)}Cr`;
        } else if (amount >= 100000) { // 1 lakh
          return `₹${(amount / 100000).toFixed(1)}L`;
        } else if (amount >= 1000) { // 1 thousand
          return `₹${(amount / 1000).toFixed(1)}K`;
        } else {
          return `₹${amount.toFixed(0)}`;
        }
      };

      return {
        totalDailyRevenue: formatCurrency(totalBerthCharges + totalServiceCharges),
        berthCharges: formatCurrency(totalBerthCharges),
        serviceCharges: formatCurrency(totalServiceCharges),
        containerStorageCharges: formatCurrency(totalServiceCharges * 0.3) // Estimated
      };
    } catch (error) {
      console.warn('Error calculating financial metrics:', error);
      // Return fallback values
      return {
        totalDailyRevenue: '₹2.4M',
        berthCharges: '₹1.8M',
        serviceCharges: '₹450K',
        containerStorageCharges: '₹150K'
      };
    }
  }
};

// Export types for TypeScript
export interface DashboardStats {
  totalContainers: number;
  activeShips: number;
  availableBerths: number;
  totalPorts: number;
  todayArrivals: number;
  todayDepartures: number;
  containersInTransit: number;
  containersAtPort: number;
  averageTurnaroundTime: number;
  berthUtilizationRate: number;
  recentActivities: RecentActivity[];
  alerts: Alert[];
}

export interface RecentActivity {
  activity: string;
  description: string;
  timestamp: string;
  type: string;
  entityId?: string;
  entityName?: string;
}

export interface Alert {
  id: number;
  title: string;
  message: string;
  severity: string;
  createdAt: string;
  isRead: boolean;
}

export interface ThroughputData {
  date: string;
  containersProcessed: number;
  containersLoaded: number;
  containersUnloaded: number;
  avgProcessingTime: number;
  period: string;
}

export interface BerthUtilization {
  berthId: number;
  berthName: string;
  portId: number;
  portName: string;
  utilizationRate: number;
  totalCapacity: number;
  averageOccupancy: number;
  utilizationHistory: UtilizationDataPoint[];
}

export interface UtilizationDataPoint {
  timestamp: string;
  utilization: number;
  occupancy: number;
}

export default analyticsService;