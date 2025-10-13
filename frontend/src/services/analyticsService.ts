import { api } from './api';

// Analytics API service for dashboard and reporting functionality
export const analyticsService = {
  // Get dashboard statistics
  async getDashboardStats() {
    try {
      const response = await api.get('/analytics/dashboard-stats');
      return response.data;
    } catch (error) {
      console.warn('Dashboard stats API not available, using fallback data');
      // Return fallback data with structure matching DashboardStatsDto
      return {
        data: {
          totalContainers: 12547,
          activeShips: 23,
          availableBerths: 7,
          totalPorts: 1,
          todayArrivals: 8,
          todayDepartures: 5,
          containersInTransit: 2847,
          containersAtPort: 9700,
          averageTurnaroundTime: 18.5,
          berthUtilizationRate: 78.5,
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
            }
          ]
        }
      };
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
  async getRealtimeMetrics() {
    try {
      const response = await api.get('/analytics/realtime-metrics');
      return response.data;
    } catch (error) {
      console.warn('Real-time metrics API not available');
      return { data: null };
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