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
        <div class="flex items-start justify-between">
          <div class="flex-1">
            <h1 class="text-3xl font-bold mb-2">Welcome to Port Operations Control Center</h1>
            <p class="text-blue-100 text-lg mb-4 max-w-2xl">
              Monitor real-time cargo movements, ship arrivals, and port capacity. This dashboard provides a comprehensive view of all port activities and operational metrics.
            </p>
            <div class="flex flex-wrap gap-4 text-sm">
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
          <div class="hidden lg:block ml-8">
            <div class="w-32 h-32 bg-blue-500/20 rounded-full flex items-center justify-center">
              <svg class="w-16 h-16 text-blue-200" fill="currentColor" viewBox="0 0 24 24">
                <path d="M3 13h8V3H3v10zm0 8h8v-6H3v6zm10 0h8V11h-8v10zm0-18v6h8V3h-8z"/>
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- Key Metrics Section -->
      <section class="mb-12">
        
        
        <MetricsGrid 
          :stats="metrics"
          :loading="loading"
          :error="metricsError"
          @retry="loadDashboardData"
        />
      </section>

      <!-- Main Dashboard Grid -->
      <!-- Enhanced Operational Insights Banner -->
      <div class="bg-white rounded-xl border border-slate-200 p-6 mb-8 shadow-sm">
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-4">
            <div class="p-3 bg-emerald-100 rounded-full">
              <svg class="w-6 h-6 text-emerald-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              </svg>
            </div>
            <div>
              <h3 class="text-lg font-semibold text-slate-900">Port Operations Running Smoothly</h3>
              <p class="text-slate-600">All systems operational • Average processing time: 3.2 hours • No weather delays</p>
            </div>
          </div>
          <div class="hidden md:flex items-center gap-6 text-sm">
            <div class="text-center">
              <div class="font-semibold text-slate-900">98.5%</div>
              <div class="text-slate-500">Efficiency Rate</div>
            </div>
            <div class="text-center">
              <div class="font-semibold text-slate-900">2.1 hrs</div>
              <div class="text-slate-500">Avg. Turnaround</div>
            </div>
            <div class="text-center">
              <div class="font-semibold text-slate-900">Zero</div>
              <div class="text-slate-500">Safety Incidents</div>
            </div>
            <div class="text-center">
              <div class="font-semibold text-slate-900">{{ financialMetrics.totalDailyRevenue }}</div>
              <div class="text-slate-500">Daily Revenue</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Enhanced Analytics Row -->
      <div class="grid grid-cols-1 lg:grid-cols-4 gap-6 mb-8">
        <!-- Ship Tracking Panel -->
        <div class="bg-white rounded-xl border border-slate-200 p-6 shadow-sm">
          <div class="flex items-center gap-3 mb-4">
            <div class="p-2 bg-blue-100 rounded-lg">
              <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              </svg>
            </div>
            <h3 class="text-sm font-semibold text-slate-900">Live Ship Tracking</h3>
          </div>
          <div class="space-y-3">
            <div class="flex justify-between text-xs">
              <span class="text-slate-600">In Port</span>
              <span class="font-medium text-slate-900">{{ ships.length }}</span>
            </div>
            <div class="flex justify-between text-xs">
              <span class="text-slate-600">Arriving Today</span>
              <span class="font-medium text-emerald-600">3</span>
            </div>
            <div class="flex justify-between text-xs">
              <span class="text-slate-600">Average Speed</span>
              <span class="font-medium text-slate-900">12.5 knots</span>
            </div>
          </div>
        </div>

        <!-- Financial Summary Panel -->
        <div class="bg-white rounded-xl border border-slate-200 p-6 shadow-sm">
          <div class="flex items-center gap-3 mb-4">
            <div class="p-2 bg-green-100 rounded-lg">
              <svg class="w-5 h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"></path>
              </svg>
            </div>
            <h3 class="text-sm font-semibold text-slate-900">Revenue Today</h3>
          </div>
          <div class="space-y-3">
            <div class="flex justify-between text-xs">
              <span class="text-slate-600">Berth Charges</span>
              <span class="font-medium text-slate-900">{{ financialMetrics.berthCharges }}</span>
            </div>
            <div class="flex justify-between text-xs">
              <span class="text-slate-600">Storage Fees</span>
              <span class="font-medium text-slate-900">{{ financialMetrics.containerStorageCharges }}</span>
            </div>
            <div class="flex justify-between text-xs">
              <span class="text-slate-600">Service Charges</span>
              <span class="font-medium text-slate-900">{{ financialMetrics.serviceCharges }}</span>
            </div>
          </div>
        </div>

        <!-- Equipment Status Panel -->
        <div class="bg-white rounded-xl border border-slate-200 p-6 shadow-sm">
          <div class="flex items-center gap-3 mb-4">
            <div class="p-2 bg-purple-100 rounded-lg">
              <svg class="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19.428 15.428a2 2 0 00-1.022-.547l-2.387-.477a6 6 0 00-3.86.517l-.318.158a6 6 0 01-3.86.517L6.05 15.21a2 2 0 00-1.806.547M8 4h8l-1 1v5.172a2 2 0 00.586 1.414l5 5c1.26 1.26.367 3.414-1.415 3.414H4.828c-1.782 0-2.674-2.154-1.414-3.414l5-5A2 2 0 009 10.172V5L8 4z"></path>
              </svg>
            </div>
            <h3 class="text-sm font-semibold text-slate-900">Equipment Status</h3>
          </div>
          <div class="space-y-3">
            <div class="flex justify-between text-xs">
              <span class="text-slate-600">Cranes Active</span>
              <span class="font-medium text-emerald-600">8/12</span>
            </div>
            <div class="flex justify-between text-xs">
              <span class="text-slate-600">Trucks Available</span>
              <span class="font-medium text-slate-900">15/20</span>
            </div>
            <div class="flex justify-between text-xs">
              <span class="text-slate-600">Efficiency</span>
              <span class="font-medium text-slate-900">94%</span>
            </div>
          </div>
        </div>

        <!-- Weather & Environment Panel -->
        <div class="bg-white rounded-xl border border-slate-200 p-6 shadow-sm">
          <div class="flex items-center gap-3 mb-4">
            <div class="p-2 bg-orange-100 rounded-lg">
              <svg class="w-5 h-5 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 15a4 4 0 004 4h9a5 5 0 10-.1-9.999 5.002 5.002 0 10-9.78 2.096A4.001 4.001 0 003 15z"></path>
              </svg>
            </div>
            <h3 class="text-sm font-semibold text-slate-900">Conditions</h3>
          </div>
          <div class="space-y-3">
            <div class="flex justify-between text-xs">
              <span class="text-slate-600">Weather</span>
              <span class="font-medium text-emerald-600">Clear</span>
            </div>
            <div class="flex justify-between text-xs">
              <span class="text-slate-600">Wind Speed</span>
              <span class="font-medium text-slate-900">8 knots</span>
            </div>
            <div class="flex justify-between text-xs">
              <span class="text-slate-600">Tide Level</span>
              <span class="font-medium text-slate-900">High</span>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Container Activity Panel -->
        <ContainerActivity 
          :containers="recentContainers"
          :loading="loading"
          :error="containerError"
          :total-operations="156"
          @retry="loadDashboardData"
          @view-all="handleViewAllContainers"
          class="lg:col-span-2"
        />

        <!-- Status & Analytics Panel -->
        <div class="space-y-6">
          <!-- Port Status Component -->
          <PortStatus 
            :status-data="portStatus"
            :berth-utilization="utilizationMetrics.berthUtilization"
            :container-capacity="utilizationMetrics.containerCapacity"
            :loading="loading"
            :error="statusError"
            @retry="loadDashboardData"
            @view-analytics="handleViewAnalytics"
          />

          <!-- Real-time Movement History Panel -->
          <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div class="p-6 border-b border-slate-100">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="p-2 bg-indigo-100 rounded-lg">
                    <svg class="w-5 h-5 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z"></path>
                    </svg>
                  </div>
                  <div>
                    <h3 class="text-lg font-semibold text-slate-900">Live Operations</h3>
                    <p class="text-sm text-slate-500">Real-time equipment and movement tracking</p>
                  </div>
                </div>
                <div class="flex items-center gap-2 text-sm text-emerald-600">
                  <div class="w-2 h-2 bg-emerald-500 rounded-full animate-pulse"></div>
                  <span class="font-medium">Live Feed</span>
                </div>
              </div>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="movement in recentMovements" :key="movement.id" 
                     class="flex items-start gap-4 p-4 bg-slate-50 rounded-lg border border-slate-100">
                  <div class="p-2 rounded-full" :class="getMovementIconStyle(movement.type)">
                    <component :is="getMovementIcon(movement.type)" :size="16" />
                  </div>
                  <div class="flex-1 min-w-0">
                    <div class="flex items-center justify-between mb-2">
                      <h4 class="text-sm font-semibold text-slate-900">{{ movement.containerNumber }}</h4>
                      <span class="text-xs text-slate-500">{{ movement.time }}</span>
                    </div>
                    <p class="text-sm text-slate-600 mb-2">{{ movement.description }}</p>
                    <div class="flex items-center gap-4 text-xs text-slate-500">
                      <span class="flex items-center gap-1">
                        <svg class="w-3 h-3" fill="currentColor" viewBox="0 0 24 24">
                          <path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7zm0 9.5c-1.38 0-2.5-1.12-2.5-2.5s1.12-2.5 2.5-2.5 2.5 1.12 2.5 2.5-1.12 2.5-2.5 2.5z"/>
                        </svg>
                        {{ movement.location }}
                      </span>
                      <span class="flex items-center gap-1">
                        <svg class="w-3 h-3" fill="currentColor" viewBox="0 0 24 24">
                          <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z"/>
                        </svg>
                        {{ movement.operator }}
                      </span>
                      <span class="flex items-center gap-1">
                        <svg class="w-3 h-3" fill="currentColor" viewBox="0 0 24 24">
                          <path d="M11.99 2C6.47 2 2 6.48 2 12s4.47 10 9.99 10C17.52 22 22 17.52 22 12S17.52 2 11.99 2zM12 20c-4.42 0-8-3.58-8-8s3.58-8 8-8 8 3.58 8 8-3.58 8-8 8zm.5-13H11v6l5.25 3.15.75-1.23-4.5-2.67z"/>
                        </svg>
                        {{ movement.duration }}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
              <div class="mt-4 pt-4 border-t border-slate-100 text-center">
                <button class="text-sm font-medium text-indigo-600 hover:text-indigo-700 transition-colors">
                  View Complete Movement History →
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue';
import DashboardHeader from '../dashboard/DashboardHeader.vue';
import MetricsGrid from '../dashboard/MetricsGrid.vue';
import ContainerActivity from '../dashboard/ContainerActivity.vue';
import PortStatus from '../dashboard/PortStatus.vue';
import { containerApi, portApi, shipApi, berthApi, berthAssignmentApi } from '../../services/api';
import { analyticsService } from '../../services/analyticsService';
import { Container as ContainerIcon, Ship as ShipIcon, Anchor, Activity, ArrowUpCircle, ArrowDownCircle, RefreshCw, Package } from 'lucide-vue-next';

// Types for our data structures
interface Container {
  id: number;
  containerNumber: string;
  type: string;
  weight: number;
  shipId: number;
}

interface Ship {
  id: number;
  name: string;
  type: string;
  capacity: number;
}

interface Berth {
  id: number;
  identifier: string;
  port: string;
  status: string;
}

interface BerthAssignment {
  id: number;
  berthId: number;
  shipId: number;
  containerId: string;
  assignedAt: string;
}

interface Port {
  id: number;
  name: string;
  country: string;
  capacity: number;
}

// Reactive state
const currentTime = ref(new Date().toLocaleTimeString());
const loading = ref(true);
const metricsError = ref<string | null>(null);
const containerError = ref<string | null>(null);
const statusError = ref<string | null>(null);

// Data arrays
const containers = ref<Container[]>([]);
const ships = ref<Ship[]>([]);
const berths = ref<Berth[]>([]);
const berthAssignments = ref<BerthAssignment[]>([]);
const ports = ref<Port[]>([]);
const recentContainers = ref<any[]>([]);
const recentMovements = ref<any[]>([]);

// Analytics and financial data
const dashboardStats = ref<any>(null);
const financialMetrics = ref<any>({
  totalDailyRevenue: '₹2.4M',
  berthCharges: '₹1.8M',
  serviceCharges: '₹450K',
  containerStorageCharges: '₹150K'
});

// Time interval ref
let timeInterval: number | null = null;

// Port configuration
const portName = ref('Chennai Port');

// Computed properties
const isAdminUser = computed(() => {
  // Check JWT authentication and user roles
  const currentUser = localStorage.getItem('current_user');
  if (currentUser) {
    try {
      const user = JSON.parse(currentUser);
      return user.roles && (user.roles.includes('Admin') || user.roles.includes('PortManager'));
    } catch (error) {
      return false;
    }
  }
  
  // Fallback to legacy admin check
  const adminUser = localStorage.getItem('admin_user');
  return !!adminUser;
});

const metrics = computed(() => [
  { 
    title: "Cargo Containers in Port", 
    subtitle: "Total containers currently being processed",
    value: dashboardStats.value ? dashboardStats.value.containersAtPort.toLocaleString() : containers.value.length.toString(), 
    change: dashboardStats.value ? `${dashboardStats.value.containersInTransit} in transit` : "+12% from yesterday",
    icon: ContainerIcon,
    bgColor: "bg-blue-50",
    iconColor: "text-blue-600",
    progressColor: "bg-blue-500",
    progress: Math.min((containers.value.length / 50) * 100, 100).toFixed(0) + '%'
  },
  { 
    title: "Ships Currently Docked", 
    subtitle: "Vessels actively loading/unloading cargo",
    value: dashboardStats.value ? dashboardStats.value.activeShips.toString() : ships.value.length.toString(), 
    change: dashboardStats.value ? `Avg ${dashboardStats.value.averageTurnaroundTime}h turnaround` : "+3% from yesterday",
    icon: ShipIcon,
    bgColor: "bg-green-50",
    iconColor: "text-green-600",
    progressColor: "bg-green-500",
    progress: Math.min((ships.value.length / 20) * 100, 100).toFixed(0) + '%'
  },
  { 
    title: "Open Docking Spaces", 
    subtitle: "Available berths for incoming ships",
    value: dashboardStats.value ? `${dashboardStats.value.availableBerths} available` : `${berths.value.length - berthAssignments.value.length} of ${berths.value.length}`, 
    change: dashboardStats.value ? `${Math.round(dashboardStats.value.berthUtilizationRate)}% utilization` : "2 spaces occupied today",
    icon: Anchor,
    bgColor: "bg-emerald-50",
    iconColor: "text-emerald-600",
    progressColor: "bg-emerald-500",
    progress: berths.value.length > 0 ? ((berths.value.length - berthAssignments.value.length) / berths.value.length * 100).toFixed(0) + '%' : '0%'
  },
  { 
    title: "Today's Operations", 
    subtitle: "Completed loading/unloading activities",
    value: dashboardStats.value ? `${dashboardStats.value.todayArrivals + dashboardStats.value.todayDepartures}` : berthAssignments.value.length.toString(), 
    change: dashboardStats.value ? `${dashboardStats.value.todayArrivals} arrivals, ${dashboardStats.value.todayDepartures} departures` : "+8% efficiency gain",
    icon: Activity,
    bgColor: "bg-purple-50",
    iconColor: "text-purple-600",
    progressColor: "bg-purple-500",
    progress: Math.min((berthAssignments.value.length / 30) * 100, 100).toFixed(0) + '%'
  },
]);

const portStatus = computed(() => ({
  operationalStatus: 'Fully Operational',
  systemStatus: 'Active',
  statusMessage: 'All Systems Active',
  description: 'No critical alerts or maintenance required'
}));

const utilizationMetrics = computed(() => ({
  berthUtilization: {
    percentage: berths.value.length > 0 ? Math.round((berthAssignments.value.length / berths.value.length) * 100) : 0,
    occupied: berthAssignments.value.length,
    available: berths.value.length - berthAssignments.value.length,
    total: berths.value.length,
    description: berths.value.length - berthAssignments.value.length > 0 
      ? `${berths.value.length - berthAssignments.value.length} docking spaces ready for incoming ships`
      : 'All docking spaces are currently occupied'
  },
  containerCapacity: {
    percentage: dashboardStats.value 
      ? Math.round((dashboardStats.value.containersAtPort / (dashboardStats.value.containersAtPort + dashboardStats.value.containersInTransit)) * 100)
      : Math.round((containers.value.length / Math.max(containers.value.length + 1000, 3900)) * 100),
    current: dashboardStats.value ? dashboardStats.value.containersAtPort : containers.value.length,
    total: dashboardStats.value 
      ? dashboardStats.value.containersAtPort + dashboardStats.value.containersInTransit
      : Math.max(containers.value.length + 1000, 3900),
    message: dashboardStats.value?.containersAtPort > 8000 
      ? 'Storage area is filling up - consider prioritizing outgoing shipments'
      : 'Container storage capacity is well within limits'
  }
}));

// Methods
const loadDashboardData = async () => {
  try {
    loading.value = true;
    metricsError.value = null;
    containerError.value = null;
    statusError.value = null;

    // Load data individually to better handle errors
    let containersData: Container[] = [];
    let shipsData: Ship[] = [];
    let berthsData: Berth[] = [];
    let berthAssignmentsData: BerthAssignment[] = [];
    let portsData: Port[] = [];

    try {
      const containersResponse = await containerApi.getAll();
      containersData = (containersResponse.data as any[])?.map((c: any) => ({
        id: c.id || 0,
        containerNumber: c.containerNumber || c.number || `CNT-${c.id}`,
        type: c.type || 'Dry',
        weight: c.weight || 0,
        shipId: c.shipId || 0
      })) || [];
    } catch (error) {
      containerError.value = 'Failed to load container data';
      // Mock container data as fallback
      containersData = [
        { id: 1, containerNumber: 'MAEU1234567', type: 'Dry', weight: 25000, shipId: 1 },
        { id: 2, containerNumber: 'MAEU2345678', type: 'Refrigerated', weight: 28000, shipId: 2 },
        { id: 3, containerNumber: 'MAEU3456789', type: 'Tank', weight: 30000, shipId: 1 },
        { id: 4, containerNumber: 'MAEU4567890', type: 'OpenTop', weight: 22000, shipId: 3 }
      ];
    }

    try {
      const shipsResponse = await shipApi.getAll();
      shipsData = (shipsResponse.data as any[])?.map((s: any) => ({
        id: s.id || 0,
        name: s.name || 'Unknown Ship',
        type: s.type || 'Container Ship',
        capacity: s.capacity || 0
      })) || [];
    } catch (error) {
      // Mock ship data as fallback
      shipsData = [
        { id: 1, name: 'Maersk Edmonton', type: 'Container Ship', capacity: 13092 },
        { id: 2, name: 'CMA CGM Bougainville', type: 'Container Ship', capacity: 18000 },
        { id: 3, name: 'MSC Oscar', type: 'Container Ship', capacity: 19224 }
      ];
    }

    try {
      const berthsResponse = await berthApi.getAll();
      berthsData = (berthsResponse.data as any[])?.map((b: any) => ({
        id: b.id || 0,
        identifier: b.identifier || `B-${b.id}`,
        port: b.port || 'Unknown Port',
        status: b.status || 'Available'
      })) || [];
    } catch (error) {
      statusError.value = 'Failed to load berth data';
      // Mock berth data as fallback
      berthsData = [
        { id: 1, identifier: 'B-001', port: 'Singapore', status: 'Available' },
        { id: 2, identifier: 'B-002', port: 'Singapore', status: 'Occupied' },
        { id: 3, identifier: 'B-003', port: 'Rotterdam', status: 'Available' },
        { id: 4, identifier: 'B-004', port: 'Shanghai', status: 'Maintenance' }
      ];
    }

    try {
      const berthAssignmentsResponse = await berthAssignmentApi.getAll();
      berthAssignmentsData = (berthAssignmentsResponse.data as any[])?.map((ba: any) => ({
        id: ba.id || 0,
        berthId: ba.berthId || 0,
        shipId: ba.shipId || 0,
        containerId: ba.containerId || '',
        assignedAt: ba.assignedAt || new Date().toISOString()
      })) || [];
    } catch (error) {
      // Mock berth assignment data as fallback
      berthAssignmentsData = [
        { id: 1, berthId: 2, shipId: 1, containerId: 'MAEU1234567', assignedAt: new Date().toISOString() },
        { id: 2, berthId: 4, shipId: 3, containerId: 'MAEU3456789', assignedAt: new Date().toISOString() }
      ];
    }

    try {
      const portsResponse = await portApi.getAll();
      portsData = (portsResponse.data as any[])?.map((p: any) => ({
        id: p.id || 0,
        name: p.name || 'Unknown Port',
        country: p.country || 'Unknown',
        capacity: p.capacity || 0
      })) || [];
    } catch (error) {
      // Mock port data as fallback
      portsData = [
        { id: 1, name: 'Port of Singapore', country: 'Singapore', capacity: 37.2 },
        { id: 2, name: 'Port of Rotterdam', country: 'Netherlands', capacity: 15.3 },
        { id: 3, name: 'Port of Shanghai', country: 'China', capacity: 47.0 }
      ];
    }

    // Set the data to component state
    containers.value = containersData;
    ships.value = shipsData;
    berths.value = berthsData;
    berthAssignments.value = berthAssignmentsData;
    ports.value = portsData;

    // Load analytics data and calculate financial metrics
    await loadAnalyticsData();

    // Update recent containers and movements
    updateRecentContainers();
    updateRecentMovements();

  } catch (error) {
    console.error('Error loading dashboard data:', error);
    metricsError.value = 'Failed to load dashboard data';
    // Still update with whatever data we have
    updateRecentContainers();
    updateRecentMovements();
  } finally {
    loading.value = false;
  }
};

const loadAnalyticsData = async () => {
  try {
    // Load dashboard statistics from analytics service
    const statsResponse = await analyticsService.getDashboardStats();
    dashboardStats.value = statsResponse.data;

    // Calculate financial metrics using real berth data
    financialMetrics.value = analyticsService.calculateFinancialMetrics(
      berths.value, 
      berthAssignments.value
    );
  } catch (error) {
    console.warn('Failed to load analytics data, using fallback values:', error);
    // Keep the default financial metrics values
  }
};

const updateRecentContainers = () => {
  // Get recent containers from the first few containers
  recentContainers.value = containers.value.slice(0, 4).map((container, index) => {
    const statuses = [
      { label: 'Just Arrived', description: 'Container arrived at port' },
      { label: 'Being Loaded', description: 'Loading cargo onto ship' },
      { label: 'Quality Check', description: 'Undergoing inspection' },
      { label: 'Shipped Out', description: 'Left the port' }
    ];
    const times = ['14:30', '13:45', '12:20', '11:55'];
    
    // Find assigned berth for this container
    const assignment = berthAssignments.value.find(a => a.shipId === container.shipId);
    const berth = assignment ? berths.value.find(b => b.id === assignment.berthId) : null;
    
    const currentStatus = statuses[index] || statuses[0];
    
    return {
      id: container.containerNumber || `CNT-${container.id}`,
      status: currentStatus.label,
      statusDescription: currentStatus.description,
      berth: berth ? `Dock ${berth.identifier}` : 'Awaiting Assignment',
      time: times[index] || new Date().toLocaleTimeString().slice(0, 5),
      type: container.type === 'Dry' ? 'Standard Cargo' : 
            container.type === 'Refrigerated' ? 'Temperature Controlled' :
            container.type === 'Tank' ? 'Liquid Cargo' :
            container.type === 'OpenTop' ? 'Oversized Cargo' : 'Standard Cargo'
    };
  });
};

const updateRecentMovements = () => {
  // Generate mock movement data based on enhanced backend features
  const movementTypes = ['Load', 'Unload', 'Transfer', 'Storage'];
  const operators = ['Crane Operator A', 'Truck Driver B', 'System Auto', 'Harbor Pilot'];
  const equipment = ['Crane-01', 'Truck-15', 'Forklift-03', 'Ship Crane'];
  const locations = ['Dock A1', 'Storage Yard B', 'Ship MSC Oscar', 'Gate 3'];

  recentMovements.value = containers.value.slice(0, 3).map((container, index) => {
    const movementType = movementTypes[index % movementTypes.length];
    const operator = operators[index % operators.length];
    const location = locations[index % locations.length];
    const equipmentUsed = equipment[index % equipment.length];
    
    const timeOffset = [5, 12, 18][index] || 5;
    const now = new Date();
    now.setMinutes(now.getMinutes() - timeOffset);
    
    return {
      id: `mov-${container.id}`,
      containerNumber: container.containerNumber,
      type: movementType,
      description: `${movementType} operation using ${equipmentUsed}`,
      location: location,
      operator: operator,
      equipment: equipmentUsed,
      duration: `${Math.floor(Math.random() * 3) + 1}.${Math.floor(Math.random() * 6)}h`,
      time: now.toLocaleTimeString().slice(0, 5)
    };
  });
};

const getMovementIcon = (type: string) => {
  switch (type) {
    case 'Load': return ArrowUpCircle;
    case 'Unload': return ArrowDownCircle;
    case 'Transfer': return RefreshCw;
    case 'Storage': return Package;
    default: return Package;
  }
};

const getMovementIconStyle = (type: string) => {
  switch (type) {
    case 'Load': return 'bg-emerald-100 text-emerald-600';
    case 'Unload': return 'bg-blue-100 text-blue-600';
    case 'Transfer': return 'bg-purple-100 text-purple-600';
    case 'Storage': return 'bg-orange-100 text-orange-600';
    default: return 'bg-slate-100 text-slate-600';
  }
};

const handleViewAllContainers = () => {
  // TODO: Navigate to container list
  console.log('View all containers');
};

const handleViewAnalytics = () => {
  // TODO: Navigate to analytics dashboard
  console.log('View analytics');
};

// Lifecycle hooks
onMounted(async () => {
  // Start time interval
  timeInterval = setInterval(() => {
    currentTime.value = new Date().toLocaleTimeString();
  }, 1000);

  // Load data from backend
  await loadDashboardData();
});

onUnmounted(() => {
  if (timeInterval) {
    clearInterval(timeInterval);
  }
});
</script>

<style scoped>
/* Modern animations */
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

@keyframes shimmer {
  0% {
    background-position: -200px 0;
  }
  100% {
    background-position: calc(200px + 100%) 0;
  }
}

/* Animation classes */
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

/* Custom hover effects */
.group:hover .group-hover\:scale-105 {
  transform: scale(1.05);
}

/* Responsive utilities */
@media (max-width: 768px) {
  .grid-cols-1.md\:grid-cols-2.lg\:grid-cols-4 {
    grid-template-columns: repeat(1, minmax(0, 1fr));
    gap: 1rem;
  }
  
  .lg\:col-span-2 {
    grid-column: span 1;
  }
  
  .flex-col.md\:flex-row {
    flex-direction: column;
  }
  
  .gap-6.md\:gap-8 {
    gap: 1.5rem;
  }
  
  .px-6 {
    padding-left: 1rem;
    padding-right: 1rem;
  }
  
  .py-8 {
    padding-top: 1.5rem;
    padding-bottom: 1.5rem;
  }
  
  .text-3xl {
    font-size: 1.875rem;
    line-height: 2.25rem;
  }
  
  .text-2xl {
    font-size: 1.5rem;
    line-height: 2rem;
  }
}

@media (max-width: 640px) {
  .text-3xl {
    font-size: 1.5rem;
    line-height: 2rem;
  }
  
  .text-2xl {
    font-size: 1.25rem;
    line-height: 1.75rem;
  }
  
  .text-xl {
    font-size: 1.125rem;
    line-height: 1.75rem;
  }
  
  .p-6 {
    padding: 1rem;
  }
  
  .p-4 {
    padding: 0.75rem;
  }
  
  .gap-6 {
    gap: 1rem;
  }
  
  .gap-4 {
    gap: 0.75rem;
  }
  
  .mb-8 {
    margin-bottom: 1.5rem;
  }
  
  .mb-6 {
    margin-bottom: 1rem;
  }
}

/* Loading states and shimmer effects */
.loading-shimmer {
  background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
  background-size: 200px 100%;
  animation: shimmer 1.5s infinite;
}

/* Enhanced shadows for depth */
.shadow-sm {
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
}

.shadow-lg {
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
}

/* Smooth transitions for all interactive elements */
* {
  transition-property: color, background-color, border-color, text-decoration-color, fill, stroke, opacity, box-shadow, transform, filter, backdrop-filter;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 150ms;
}

/* Focus states for accessibility */
button:focus-visible,
a:focus-visible {
  outline: 2px solid #3b82f6;
  outline-offset: 2px;
}

/* Custom scrollbar for better UX */
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

/* Print styles */
@media print {
  .no-print {
    display: none !important;
  }
  
  .bg-white {
    background: white !important;
  }
  
  * {
    print-color-adjust: exact;
    -webkit-print-color-adjust: exact;
  }
}
</style>