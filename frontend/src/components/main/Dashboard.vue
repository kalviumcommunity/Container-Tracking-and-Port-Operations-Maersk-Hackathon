<template>
  <div class="min-h-screen bg-slate-50">
    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-6 py-8">
      <!-- Dashboard Header Component -->
      <DashboardHeader 
        :current-time="currentTime"
        :is-admin-user="isAdminUser"
        :port-name="portName"
        class="mb-8"
      />

      <!-- Welcome & Information Section -->
      <div class="bg-gradient-to-r from-blue-600 to-blue-700 rounded-2xl text-white p-8 mb-12 shadow-lg">
        <div class="text-center">
          <h1 class="text-3xl font-bold mb-2">Welcome to Port Operations Control Center</h1>
          <p class="text-blue-100 text-lg mb-6 max-w-2xl mx-auto">
            Monitor real-time cargo movements, ship arrivals, and port capacity. This dashboard provides a comprehensive view of all port activities and operational metrics.
          </p>
          <div class="flex justify-center flex-wrap gap-4 text-sm">
            <div class="flex items-center gap-2 bg-blue-500/30 px-3 py-2 rounded-lg">
              <div class="w-2 h-2 bg-green-400 rounded-full animate-pulse"></div>
              <span>Live Data Feed</span>
            </div>
            <div class="flex items-center gap-2 bg-blue-500/30 px-3 py-2 rounded-lg">
              <span>🚢</span>
              <span>Multi-vessel Tracking</span>
            </div>
            <div class="flex items-center gap-2 bg-blue-500/30 px-3 py-2 rounded-lg">
              <span>📊</span>
              <span>Performance Analytics</span>
            </div>
          </div>
        </div>

      </div>

      <!-- Key Metrics Section -->
      <MetricsGrid 
        :stats="metrics"
        :loading="loading"
        :error="metricsError"
        :containers="filteredContainers"
        :ships="filteredShips" 
        :berths="filteredBerths"
        :selected-port-id="selectedPortId"
        @retry="loadDashboardData"
        @port-changed="onPortChanged"
      />

      <!-- Analytics Charts Section -->
      <div class="space-y-8">
        <!-- Container Activity Panel -->
        <div class="transition-all duration-500 ease-in-out" :class="{ 'opacity-50': loading }">
          <ContainerActivity2 
            :containers="filteredContainers"
            :analytics-data="portSpecificAnalytics"
            :loading="loading"
            :error="metricsError"
            @retry="loadDashboardData"
            :key="`containers-${selectedPortId || 'all'}`"
          />
        </div>

        <!-- Berth Activity Panel -->
        <div class="transition-all duration-500 ease-in-out" :class="{ 'opacity-50': loading }">
          <BerthActivity2 
            :berths="filteredBerths"
            :berth-assignments="filteredBerthAssignments"
            :analytics-data="portSpecificAnalytics"
            :loading="loading"
            :error="metricsError"
            @retry="loadDashboardData"
            :key="`berths-${selectedPortId || 'all'}`"
          />
        </div>


      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed, watch } from 'vue';
import DashboardHeader from '../dashboard/DashboardHeader.vue';
import MetricsGrid from '../dashboard/MetricsGrid.vue';
import ContainerActivity2 from '../dashboard/ContainerActivity2.vue';
import BerthActivity2 from '../dashboard/BerthActivity2.vue';
import { containerApi, portApi, shipApi, berthApi, berthAssignmentApi } from '../../services/api';
import { analyticsService } from '../../services/analyticsService';
import { Container as ContainerIcon, Ship as ShipIcon, Anchor, Activity } from 'lucide-vue-next';

// Import proper types
import type { Container } from '../../types/container';
import type { Ship } from '../../types/ship';  
import type { Berth, BerthAssignment } from '../../types/berth';
import type { Port } from '../../types/port';

// Reactive state
const currentTime = ref(new Date().toLocaleTimeString());
const loading = ref(true);
const metricsError = ref<string | null>(null);
const selectedPortId = ref<string | null>(null);

// Data arrays - ensure they are always arrays
const containers = ref<Container[]>([]);
const ships = ref<Ship[]>([]);
const berths = ref<Berth[]>([]);
const berthAssignments = ref<BerthAssignment[]>([]);
const ports = ref<Port[]>([]);

// Helper function to ensure data is always an array
const ensureArray = (data: any): any[] => {
  if (Array.isArray(data)) return data;
  if (data && typeof data === 'object' && data.data && Array.isArray(data.data)) return data.data;
  return [];
};

// Analytics data
const dashboardStats = ref<any>(null);
const realtimeMetrics = ref<any>(null);
const berthUtilization = ref<any[]>([]);
const throughputData = ref<any[]>([]);

// Time interval refs
let timeInterval: number | null = null;
let dataRefreshInterval: number | null = null;
let chartRefreshInterval: number | null = null;

// Port configuration
const portName = ref('Port Operations Center');

// Computed properties for port selection
const selectedPort = computed(() => {
  if (!selectedPortId.value) return null;
  return ports.value.find(port => port.portId.toString() === selectedPortId.value) || null;
});

const getSelectedPortName = () => {
  return selectedPort.value?.name || 'All Ports';
};

// Computed properties
const isAdminUser = computed(() => {
  const currentUser = localStorage.getItem('current_user');
  if (currentUser) {
    try {
      const user = JSON.parse(currentUser);
      return user.roles && (user.roles.includes('Admin') || user.roles.includes('PortManager'));
    } catch (error) {
      return false;
    }
  }
  const adminUser = localStorage.getItem('admin_user');
  return !!adminUser;
});

// Filter containers based on selected port
// NOTE: Container table doesn't have portId foreign key, only CurrentLocation string
// For demo/showcase, show all containers regardless of port selection
const filteredContainers = computed(() => {
  // Ensure containers.value is an array
  const containerList = Array.isArray(containers.value) ? containers.value : [];
  // Always return all containers since we can't reliably filter by port
  return containerList;
});

// Filter berths based on selected port
const filteredBerths = computed(() => {
  // Ensure berths.value is an array
  const berthList = Array.isArray(berths.value) ? berths.value : [];
  if (!selectedPortId.value) return berthList;
  const portId = parseInt(selectedPortId.value);
  return berthList.filter(b => b.portId && b.portId === portId);
});

// Filter ships based on selected port
const filteredShips = computed(() => {
  // Ensure ships.value is an array
  const shipList = Array.isArray(ships.value) ? ships.value : [];
  if (!selectedPortId.value) return shipList;
  const portId = parseInt(selectedPortId.value);
  return shipList.filter(s => s.currentPortId && s.currentPortId === portId);
});

// Filter berth assignments based on selected port
const filteredBerthAssignments = computed(() => {
  // Ensure arrays are properly formatted
  const berthList = Array.isArray(berths.value) ? berths.value : [];
  const assignmentList = Array.isArray(berthAssignments.value) ? berthAssignments.value : [];
  
  const assignments = selectedPortId.value 
    ? (() => {
        const portId = parseInt(selectedPortId.value);
        const portBerthIds = berthList.filter(b => b.portId === portId).map(b => b.berthId);
        return assignmentList.filter(ba => portBerthIds.includes(ba.berthId));
      })()
    : assignmentList;
  
  // Map id to assignmentId for component compatibility and ensure proper types
  return assignments.map(assignment => ({
    assignmentId: assignment.id || assignment.assignmentId || 0,
    berthId: assignment.berthId,
    shipId: assignment.shipId || 0, // Ensure shipId is always a number
    assignedAt: assignment.assignedAt || assignment.scheduledArrival || new Date().toISOString(),
    expectedDeparture: assignment.expectedDeparture || assignment.scheduledDeparture,
    status: assignment.status
  }));
});

// Port-specific analytics data
const portSpecificAnalytics = computed(() => {
  if (!selectedPortId.value) {
    // Return global analytics when no port is selected
    return dashboardStats.value?.data || null;
  }
  
  // When a port is selected, create analytics data based on filtered local data
  return {
    totalContainers: filteredContainers.value.length,
    activeShips: filteredShips.value.length,
    availableBerths: filteredBerths.value.length,
    totalBerthAssignments: filteredBerthAssignments.value.length,
    containersInTransit: filteredContainers.value.filter(c => c.status === 'In Transit').length,
    containersAtPort: filteredContainers.value.filter(c => c.status === 'At Port').length,
    berthUtilizationRate: filteredBerths.value.length > 0 
      ? Math.round((filteredBerthAssignments.value.length / filteredBerths.value.length) * 100) 
      : 0,
    portId: parseInt(selectedPortId.value),
    portName: getSelectedPortName(),
    // Add any other analytics fields needed by the components
    todayArrivals: filteredShips.value.filter(s => {
      const today = new Date().toDateString();
      return s.arrivalTime && new Date(s.arrivalTime).toDateString() === today;
    }).length,
    todayDepartures: filteredShips.value.filter(s => {
      const today = new Date().toDateString();
      return s.departureTime && new Date(s.departureTime).toDateString() === today;
    }).length
  };
});

const metrics = computed(() => {
  // When a specific port is selected, use filtered local data for accurate counts
  // Otherwise, use analytics data from backend if available
  const analyticsData = dashboardStats.value?.data;
  const realtimeData = realtimeMetrics.value?.data;
  const isPortSelected = !!selectedPortId.value;
  
  return [
    { 
      title: "Total Containers", 
      subtitle: isPortSelected ? "Containers in selected port" : "All containers in the port system",
      value: isPortSelected ? filteredContainers.value.length.toString() : (analyticsData?.totalContainers?.toString() || filteredContainers.value.length.toString()), 
      change: selectedPortId.value ? `Port: ${getSelectedPortName()}` : "All ports combined",
      trend: isPortSelected ? `${filteredContainers.value.length} total containers` : (analyticsData?.containersInTransit > 0 ? `${analyticsData.containersInTransit} in transit` : "Live data"),
      icon: ContainerIcon,
      bgColor: "bg-blue-50",
      iconColor: "text-blue-600",
      progressColor: "bg-blue-500",
      progress: "75%"
    },
    { 
      title: "Total Ships", 
      subtitle: isPortSelected ? "Ships in selected port" : "All ships in the system",
      value: isPortSelected ? filteredShips.value.length.toString() : filteredShips.value.length.toString(), 
      change: selectedPortId.value ? `Port: ${getSelectedPortName()}` : "All ports combined",
      trend: isPortSelected ? `${filteredShips.value.length} ships total` : `${filteredShips.value.length} ships total`,
      icon: ShipIcon,
      bgColor: "bg-green-50",
      iconColor: "text-green-600",
      progressColor: "bg-green-500",
      progress: "60%"
    },
    { 
      title: "Total Berths", 
      subtitle: isPortSelected ? "Berths in selected port" : "All berths in the system",
      value: isPortSelected ? filteredBerths.value.length.toString() : filteredBerths.value.length.toString(), 
      change: selectedPortId.value ? `Port: ${getSelectedPortName()}` : "All ports combined",
      trend: isPortSelected ? `${filteredBerths.value.length} berths total` : `${filteredBerths.value.length} berths total`,
      icon: Anchor,
      bgColor: "bg-emerald-50",
      iconColor: "text-emerald-600",
      progressColor: "bg-emerald-500",
      progress: "85%"
    },
    { 
      title: "Daily Operations", 
      subtitle: isPortSelected ? "Assignments in selected port" : "Berthing assignments and movements",
      value: isPortSelected ? filteredBerthAssignments.value.length.toString() : (analyticsData?.todayArrivals && analyticsData?.todayDepartures 
        ? (analyticsData.todayArrivals + analyticsData.todayDepartures).toString()
        : filteredBerthAssignments.value.length.toString()), 
      change: selectedPortId.value ? `Port: ${getSelectedPortName()}` : "All ports combined",
      trend: isPortSelected ? `${filteredBerthAssignments.value.length} assignments` : (analyticsData ? `${analyticsData.averageTurnaroundTime || 0}h avg turnaround` : "Live data"),
      icon: Activity,
      bgColor: "bg-purple-50",
      iconColor: "text-purple-600",
      progressColor: "bg-purple-500",
      progress: "70%"
    },
  ];
});

// Methods
const onPortChange = async () => {
  const previousPortId = selectedPortId.value;
  const newPortName = getSelectedPortName();
  


  
  // Show loading state while switching ports
  loading.value = true;
  metricsError.value = null;
  
  try {
    // Add a small delay for better UX (shows loading state)
    await new Promise(resolve => setTimeout(resolve, 300));
    
    // Store the before state for comparison
    const beforeState = {
      containers: containers.value.length,
      berths: berths.value.length,
      ships: ships.value.length,
      assignments: berthAssignments.value.length
    };
    
    // Refresh all port-specific data

    await refreshDashboardData();
    
    // Store the after state and calculate differences
    const afterState = {
      containers: filteredContainers.value.length,
      berths: filteredBerths.value.length,
      ships: filteredShips.value.length,
      assignments: filteredBerthAssignments.value.length
    };
  } catch (error) {
    console.error('❌ Error switching ports:', error);
    metricsError.value = `Failed to load data for ${newPortName}. Please try again.`;
  } finally {
    loading.value = false;
  }
};

const refreshDashboardData = async () => {
  try {
    // Convert string port ID to number for API calls
    const portIdNumber = selectedPortId.value ? parseInt(selectedPortId.value) : undefined;
    
    if (portIdNumber) {
      // Use comprehensive port-specific statistics for single port

      
      const portStatsResult = await analyticsService.getPortSpecificStats(portIdNumber);
      
      if (portStatsResult.success && portStatsResult.data) {
        // Update all data from comprehensive response
        dashboardStats.value = portStatsResult.data.dashboard;
        containers.value = ensureArray(portStatsResult.data.containers);
        berths.value = portStatsResult.data.berths.data || [];
        realtimeMetrics.value = portStatsResult.data.realtime;
      } else {

        
        // Fallback to individual API calls
        const [dashboardStatsResult, containersResult, berthsResult] = await Promise.allSettled([
          analyticsService.getDashboardStats(portIdNumber),
          analyticsService.getContainersByPort(portIdNumber),
          analyticsService.getBerthsByPort(portIdNumber)
        ]);

        dashboardStats.value = dashboardStatsResult.status === 'fulfilled' ? dashboardStatsResult.value : null;
        containers.value = containersResult.status === 'fulfilled' ? ensureArray(containersResult.value) : [];
        berths.value = berthsResult.status === 'fulfilled' ? (berthsResult.value.data || []) : [];
      }
    } else {
      // Fetch all ports overview data

      
      const [dashboardStatsResult, containersResult, berthsResult, realtimeResult] = await Promise.allSettled([
        analyticsService.getDashboardStats(),
        analyticsService.getContainersByPort(),
        analyticsService.getBerthsByPort(),
        analyticsService.getRealtimeMetrics()
      ]);

      dashboardStats.value = dashboardStatsResult.status === 'fulfilled' ? dashboardStatsResult.value : null;
      containers.value = containersResult.status === 'fulfilled' ? ensureArray(containersResult.value) : [];
      berths.value = berthsResult.status === 'fulfilled' ? (berthsResult.value.data || []) : [];
      realtimeMetrics.value = realtimeResult.status === 'fulfilled' ? realtimeResult.value : null;
      

    }


  } catch (error) {
    console.error('❌ Error refreshing dashboard data:', error);
    throw error; // Re-throw to be handled by the calling function
  }
};

const loadDashboardData = async () => {
  loading.value = true;
  metricsError.value = null;
  
  try {
    // Load all data in parallel for better performance
    const [
      portsResult,
      containersResult,
      shipsResult,
      berthsResult,
      berthAssignmentsResult,
      dashboardStatsResult,
      realtimeMetricsResult,
      berthUtilizationResult,
      throughputDataResult
    ] = await Promise.allSettled([
      portApi.getAll(),
      containerApi.getAll(),
      shipApi.getAll(),
      berthApi.getAll(),
      berthAssignmentApi.getAll(),
      analyticsService.getDashboardStats(),
      analyticsService.getRealtimeMetrics(),
      analyticsService.getBerthUtilization(),
      analyticsService.getContainerThroughput()
    ]);

    // Handle successful results and type safety
    ports.value = portsResult.status === 'fulfilled' ? (portsResult.value.data || []) : [];
    containers.value = containersResult.status === 'fulfilled' ? ensureArray(containersResult.value) : [];
    ships.value = shipsResult.status === 'fulfilled' ? (shipsResult.value.data || []) : [];
    berths.value = berthsResult.status === 'fulfilled' ? (berthsResult.value.data || []) : [];
    berthAssignments.value = berthAssignmentsResult.status === 'fulfilled' ? (berthAssignmentsResult.value.data || []) : [];
    
    // Analytics data
    dashboardStats.value = dashboardStatsResult.status === 'fulfilled' ? dashboardStatsResult.value : null;
    realtimeMetrics.value = realtimeMetricsResult.status === 'fulfilled' ? realtimeMetricsResult.value : null;
    berthUtilization.value = berthUtilizationResult.status === 'fulfilled' ? (berthUtilizationResult.value.data || []) : [];
    throughputData.value = throughputDataResult.status === 'fulfilled' ? (throughputDataResult.value.data || []) : [];

  } catch (err) {
    console.error('Error loading dashboard data:', err);
    metricsError.value = 'Failed to load dashboard data. Please try again.';
  } finally {
    loading.value = false;
  }
};

// Refresh real-time data periodically
const refreshRealtimeData = async () => {
  try {
    const portIdNumber = selectedPortId.value ? parseInt(selectedPortId.value) : undefined;
    const portContext = portIdNumber ? `Port ${portIdNumber}` : 'All Ports';
    
    const [realtimeResult, dashboardResult] = await Promise.allSettled([
      analyticsService.getRealtimeMetrics(portIdNumber),
      analyticsService.getDashboardStats(portIdNumber)
    ]);

    if (realtimeResult.status === 'fulfilled') {
      realtimeMetrics.value = realtimeResult.value;
    }
    
    if (dashboardResult.status === 'fulfilled') {
      dashboardStats.value = dashboardResult.value;
    }


  } catch (err) {

  }
};

// Refresh chart data (containers and berths) less frequently
const refreshChartData = async () => {
  try {
    const portIdNumber = selectedPortId.value ? parseInt(selectedPortId.value) : undefined;
    const portContext = portIdNumber ? `Port ${portIdNumber}` : 'All Ports';
    
    // For port-specific views, use analytics endpoints for better filtering
    // For all ports view, use general APIs
    if (portIdNumber) {
      const [containersResult, berthsResult, berthAssignmentsResult] = await Promise.allSettled([
        analyticsService.getContainersByPort(portIdNumber),
        analyticsService.getBerthsByPort(portIdNumber),
        berthAssignmentApi.getAll() // Get all assignments, will be filtered by computed property
      ]);

      if (containersResult.status === 'fulfilled') {
        containers.value = containersResult.value.data || [];
      }
      
      if (berthsResult.status === 'fulfilled') {
        berths.value = berthsResult.value.data || [];
      }
      
      if (berthAssignmentsResult.status === 'fulfilled') {
        berthAssignments.value = berthAssignmentsResult.value.data || [];
      }
    } else {
      // Load all data for overview
      const [containersResult, berthsResult, berthAssignmentsResult] = await Promise.allSettled([
        containerApi.getAll(),
        berthApi.getAll(),
        berthAssignmentApi.getAll()
      ]);

      if (containersResult.status === 'fulfilled') {
        containers.value = ensureArray(containersResult.value);
      }
      
      if (berthsResult.status === 'fulfilled') {
        berths.value = ensureArray(berthsResult.value);
      }
      
      if (berthAssignmentsResult.status === 'fulfilled') {
        berthAssignments.value = berthAssignmentsResult.value.data || [];
      }
    }


  } catch (err) {

  }
};

// Port change handler
const onPortChanged = (portId: string | null) => {

  selectedPortId.value = portId;
  
  const portName = portId ? 
    ports.value.find(p => p.portId.toString() === portId)?.name || `Port ${portId}` : 
    'All Ports';
  
  // Trigger immediate data refresh for the new port
  refreshDashboardData();
};

// Lifecycle hooks
onMounted(async () => {
  // Start time update interval
  timeInterval = setInterval(() => {
    currentTime.value = new Date().toLocaleTimeString();
  }, 1000);

  // Load initial dashboard data
  await loadDashboardData();

  // Set up periodic refresh for real-time data (every 30 seconds)
  dataRefreshInterval = setInterval(() => {

    refreshRealtimeData();
  }, 30000);

  // Set up periodic refresh for chart data (every 2 minutes)
  chartRefreshInterval = setInterval(() => {

    refreshChartData();
  }, 120000);
  



});

onUnmounted(() => {
  if (timeInterval) {
    clearInterval(timeInterval);
  }
  if (dataRefreshInterval) {
    clearInterval(dataRefreshInterval);
  }
  if (chartRefreshInterval) {
    clearInterval(chartRefreshInterval);
  }
});
</script>

<style scoped>
@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.8;
  }
}

.animate-slideIn {
  animation: slideIn 0.6s ease-out forwards;
  opacity: 0;
}

.animate-fadeIn {
  animation: fadeIn 0.8s ease-out;
}

.animate-pulse {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

.group:hover .group-hover\:scale-105 {
  transform: scale(1.05);
}

@media (max-width: 768px) {
  .grid-cols-1.md\:grid-cols-2.lg\:grid-cols-4 {
    grid-template-columns: repeat(1, minmax(0, 1fr));
    gap: 1rem;
  }
}

* {
  transition-property: color, background-color, border-color, text-decoration-color, fill, stroke, opacity, box-shadow, transform, filter, backdrop-filter;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 150ms;
}

::-webkit-scrollbar {
  width: 6px;
}

::-webkit-scrollbar-track {
  background: #f1f5f9;
}

::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 3px;
}

::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}
</style>
