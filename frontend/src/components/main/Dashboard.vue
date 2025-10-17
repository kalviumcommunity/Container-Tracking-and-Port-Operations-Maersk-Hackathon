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
          :ports="ports"
          :selected-port="selectedPort"
          @retry="loadDashboardData"
          @port-changed="handlePortChange"
        />
      </section>

      <!-- Main Dashboard Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
        <!-- Container Activity Panel -->
        <ContainerActivity 
          :containers="filteredContainers"
          :loading="loading"
          :error="containerError"
          :total-operations="filteredContainers.length"
          @retry="loadDashboardData"
          @view-all="handleViewAllContainers"
          class="lg:col-span-1"
        />

        <!-- Available Berths Panel -->
        <BerthActivity 
          :berths="filteredBerths"
          :loading="loading"
          :error="berthError"
          @retry="loadDashboardData"
          class="lg:col-span-1"
        />
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue';
import DashboardHeader from '../dashboard/DashboardHeader.vue';
import MetricsGrid from '../dashboard/MetricsGrid.vue';
import ContainerActivity from '../dashboard/ContainerActivity.vue';
import BerthActivity from '../dashboard/BerthActivity.vue';
import { containerApi, portApi, shipApi, berthApi, berthAssignmentApi, authApi } from '../../services/api';
import { analyticsService } from '../../services/analyticsService';
import { Container as ContainerIcon, Ship as ShipIcon, Anchor, Activity } from 'lucide-vue-next';

// Types for our data structures
interface Container {
  id: string | number;
  containerNumber: string;
  type: string;
  weight: number;
  shipId: number;
  status?: string;
  currentLocation?: string;
  destination?: string;
  cargoType?: string;
  condition?: string;
  size?: string;
  createdAt?: string;
  updatedAt?: string;
}

interface Ship {
  id: number;
  name: string;
  type: string;
  capacity: number;
}

interface Berth {
  berthId: number;
  name: string;
  identifier?: string;
  type?: string;
  capacity: number;
  currentLoad: number;
  maxShipLength?: number;
  maxDraft?: number;
  status: string;
  availableServices?: string;
  craneCount?: number;
  hourlyRate?: number;
  priority?: string;
  notes?: string;
  portId: number;
  portName: string;
  activeAssignmentCount: number;
}

interface BerthAssignment {
  id: number;
  berthId: number;
  shipId: number;
  containerId: string;
  assignedAt: string;
}

interface Port {
  portId: number;
  name: string;
  location: string;
  country: string;
  capacity: number;
}

// Reactive state
const currentTime = ref(new Date().toLocaleTimeString());
const loading = ref(true);
const metricsError = ref<string | null>(null);
const containerError = ref<string | null>(null);
const berthError = ref<string | null>(null);

// Port selection state
const selectedPort = ref<Port | null>(null);

// Data arrays
const containers = ref<Container[]>([]);
const ships = ref<Ship[]>([]);
const berths = ref<Berth[]>([]);
const berthAssignments = ref<BerthAssignment[]>([]);
const ports = ref<Port[]>([]);
const recentContainers = ref<any[]>([]);

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

const metrics = computed(() => {
  // Get port-specific counts
  const portContainers = selectedPort.value ? 
    containers.value.filter(c => c.currentLocation?.includes(selectedPort.value!.name)) : 
    containers.value;
  
  const portBerths = selectedPort.value ? 
    berths.value.filter(b => b.portName === selectedPort.value!.name) : 
    berths.value;
  
  const portAvailableBerths = selectedPort.value ? 
    filteredBerths.value : 
    availableBerths.value;

  return [
    { 
      title: "Cargo Containers in Port", 
      subtitle: "Total containers currently being processed",
      value: portContainers.length.toString(), 
      change: `${portContainers.filter(c => c.status === 'In Transit').length} in transit`,
      icon: ContainerIcon,
      bgColor: "bg-blue-50",
      iconColor: "text-blue-600",
      progressColor: "bg-blue-500",
      progress: Math.min((portContainers.length / 50) * 100, 100).toFixed(0) + '%'
    },
    { 
      title: "Ships Currently Docked", 
      subtitle: "Vessels actively loading/unloading cargo",
      value: ships.value.length.toString(), 
      change: `Avg 41h turnaround`,
      icon: ShipIcon,
      bgColor: "bg-green-50",
      iconColor: "text-green-600",
      progressColor: "bg-green-500",
      progress: Math.min((ships.value.length / 20) * 100, 100).toFixed(0) + '%'
    },
    { 
      title: "Total Berths in Port", 
      subtitle: "Available berths for incoming ships",
      value: `${portBerths.length} total`, 
      change: `${portAvailableBerths.length} available`,
      icon: Anchor,
      bgColor: "bg-emerald-50",
      iconColor: "text-emerald-600",
      progressColor: "bg-emerald-500",
      progress: portBerths.length > 0 ? ((portAvailableBerths.length / portBerths.length) * 100).toFixed(0) + '%' : '0%'
    },
    { 
      title: "Today's Operations", 
      subtitle: "Completed loading/unloading activities",
      value: berthAssignments.value.length.toString(), 
      change: "0 arrivals, 2 departures",
      icon: Activity,
      bgColor: "bg-purple-50",
      iconColor: "text-purple-600",
      progressColor: "bg-purple-500",
      progress: Math.min((berthAssignments.value.length / 30) * 100, 100).toFixed(0) + '%'
    },
  ];
});

// Computed property for available berths
const availableBerths = computed(() => {
  return berths.value.filter(berth => 
    berth.status === 'Available' || 
    berth.status === 'Free' || 
    berth.status === 'Ready'
  );
});

// Port-filtered computed properties
const filteredContainers = computed(() => {
  if (!selectedPort.value) {
    return recentContainers.value;
  }
  return recentContainers.value.filter(container => 
    container.location?.includes(selectedPort.value!.name) ||
    container.berth?.includes(selectedPort.value!.name)
  );
});

const filteredBerths = computed(() => {
  return availableBerths.value.filter(berth => {
    if (!selectedPort.value) return true;
    return berth.portName === selectedPort.value.name;
  });
});

// Methods
const loadDashboardData = async () => {
  try {
    // Check authentication first
    if (!authApi.isAuthenticated()) {
      console.warn('User not authenticated, using fallback data');
    }
    
    loading.value = true;
    metricsError.value = null;
    containerError.value = null;
    berthError.value = null;

    // Load data individually to better handle errors
    let containersData: Container[] = [];
    let shipsData: Ship[] = [];
    let berthsData: Berth[] = [];
    let berthAssignmentsData: BerthAssignment[] = [];
    let portsData: Port[] = [];

    try {
      const containersResponse = await containerApi.getLatest(50); // Get latest 50 containers
      containersData = (containersResponse.data as any[])?.map((c: any) => ({
        id: c.containerId || c.id || 0,
        containerNumber: c.containerId || c.containerNumber || c.number || `CNT-${c.id || Math.random().toString(36).substr(2, 9)}`,
        type: c.type || 'Dry',
        weight: c.weight || 0,
        shipId: c.shipId || 0,
        status: c.status || 'Available',
        currentLocation: c.currentLocation || 'Port Area',
        destination: c.destination || '',
        cargoType: c.cargoType || '',
        condition: c.condition || 'Good',
        size: c.size || '40ft',
        createdAt: c.createdAt || new Date().toISOString(),
        updatedAt: c.updatedAt || new Date().toISOString()
      })) || [];
    } catch (error) {
      containerError.value = 'Failed to load container data';
      console.error('Container API error:', error);
      // Enhanced mock container data with more entries to test pagination (already sorted by latest first)
      containersData = [
        { id: 'MAEU2024015', containerNumber: 'MAEU2024015', type: 'Dry', weight: 25000, shipId: 1, status: 'Arrived', currentLocation: 'Terminal A', destination: 'Hamburg', cargoType: 'Electronics', condition: 'Good', size: '40ft', createdAt: new Date().toISOString(), updatedAt: new Date().toISOString() },
        { id: 'COSCO2024014', containerNumber: 'COSCO2024014', type: 'Refrigerated', weight: 28000, shipId: 2, status: 'Loading', currentLocation: 'Terminal B', destination: 'Rotterdam', cargoType: 'Food', condition: 'Good', size: '40ft', createdAt: new Date(Date.now() - 1 * 60 * 60 * 1000).toISOString(), updatedAt: new Date().toISOString() },
        { id: 'HAPAG2024013', containerNumber: 'HAPAG2024013', type: 'Tank', weight: 30000, shipId: 1, status: 'Inspection', currentLocation: 'Inspection Zone', destination: 'Antwerp', cargoType: 'Chemicals', condition: 'Good', size: '20ft', createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000).toISOString(), updatedAt: new Date().toISOString() },
        { id: 'MSC2024012', containerNumber: 'MSC2024012', type: 'OpenTop', weight: 22000, shipId: 3, status: 'Departed', currentLocation: 'En Route', destination: 'Le Havre', cargoType: 'Textiles', condition: 'Good', size: '40ft', createdAt: new Date(Date.now() - 3 * 60 * 60 * 1000).toISOString(), updatedAt: new Date().toISOString() },
        { id: 'MAEU2024011', containerNumber: 'MAEU2024011', type: 'Dry', weight: 25000, shipId: 1, status: 'Unloading', currentLocation: 'Dock 5', destination: 'Liverpool', cargoType: 'Machinery', condition: 'Good', size: '40ft', createdAt: new Date(Date.now() - 4 * 60 * 60 * 1000).toISOString(), updatedAt: new Date().toISOString() },
        { id: 'MAEU2024010', containerNumber: 'MAEU2024010', type: 'Refrigerated', weight: 28000, shipId: 2, status: 'At Port', currentLocation: 'Cold Storage', destination: 'Copenhagen', cargoType: 'Pharmaceuticals', condition: 'Good', size: '40ft', createdAt: new Date(Date.now() - 5 * 60 * 60 * 1000).toISOString(), updatedAt: new Date().toISOString() },
        { id: 'MAEU2024009', containerNumber: 'MAEU2024009', type: 'Tank', weight: 30000, shipId: 1, status: 'Loading', currentLocation: 'Tank Farm', destination: 'Marseille', cargoType: 'Petroleum', condition: 'Good', size: '20ft', createdAt: new Date(Date.now() - 6 * 60 * 60 * 1000).toISOString(), updatedAt: new Date().toISOString() },
        { id: 'MAEU2024008', containerNumber: 'MAEU2024008', type: 'OpenTop', weight: 22000, shipId: 3, status: 'Available', currentLocation: 'Storage Yard', destination: 'Valencia', cargoType: 'Steel', condition: 'Good', size: '40ft', createdAt: new Date(Date.now() - 7 * 60 * 60 * 1000).toISOString(), updatedAt: new Date().toISOString() },
        { id: 'COSCO2024007', containerNumber: 'COSCO2024007', type: 'Dry', weight: 26000, shipId: 2, status: 'In Transit', currentLocation: 'Sea Route', destination: 'Barcelona', cargoType: 'Consumer Goods', condition: 'Good', size: '40ft', createdAt: new Date(Date.now() - 8 * 60 * 60 * 1000).toISOString(), updatedAt: new Date().toISOString() },
        { id: 'HAPAG2024006', containerNumber: 'HAPAG2024006', type: 'Refrigerated', weight: 27000, shipId: 1, status: 'Loaded', currentLocation: 'Ship Hold', destination: 'Genoa', cargoType: 'Fruits', condition: 'Good', size: '40ft', createdAt: new Date(Date.now() - 9 * 60 * 60 * 1000).toISOString(), updatedAt: new Date().toISOString() },
        { id: 'MSC2024005', containerNumber: 'MSC2024005', type: 'Tank', weight: 29000, shipId: 3, status: 'Inspection', currentLocation: 'Security Check', destination: 'Naples', cargoType: 'Chemicals', condition: 'Good', size: '20ft', createdAt: new Date(Date.now() - 10 * 60 * 60 * 1000).toISOString(), updatedAt: new Date().toISOString() },
        { id: 'EVERGREEN2024004', containerNumber: 'EVERGREEN2024004', type: 'OpenTop', weight: 24000, shipId: 2, status: 'Arrived', currentLocation: 'Berth 12', destination: 'Piraeus', cargoType: 'Automotive', condition: 'Good', size: '40ft', createdAt: new Date(Date.now() - 11 * 60 * 60 * 1000).toISOString(), updatedAt: new Date().toISOString() },
        { id: 'YANG2024003', containerNumber: 'YANG2024003', type: 'Dry', weight: 25500, shipId: 1, status: 'Loading', currentLocation: 'Crane 8', destination: 'Thessaloniki', cargoType: 'Textiles', condition: 'Good', size: '40ft', createdAt: new Date(Date.now() - 12 * 60 * 60 * 1000).toISOString(), updatedAt: new Date().toISOString() },
        { id: 'CMA2024002', containerNumber: 'CMA2024002', type: 'Refrigerated', weight: 28500, shipId: 3, status: 'Departed', currentLocation: 'Open Sea', destination: 'Istanbul', cargoType: 'Dairy', condition: 'Good', size: '40ft', createdAt: new Date(Date.now() - 13 * 60 * 60 * 1000).toISOString(), updatedAt: new Date().toISOString() },
        { id: 'OOCL2024001', containerNumber: 'OOCL2024001', type: 'Tank', weight: 31000, shipId: 2, status: 'Unloaded', currentLocation: 'Processing Area', destination: 'Izmir', cargoType: 'Petroleum', condition: 'Good', size: '20ft', createdAt: new Date(Date.now() - 14 * 60 * 60 * 1000).toISOString(), updatedAt: new Date().toISOString() }
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
      // Try to get available berths specifically first
      let berthsResponse;
      try {
        berthsResponse = await berthApi.getByStatus('Available');
        console.log('Loaded available berths specifically:', berthsResponse.data.length);
      } catch (statusError) {
        console.log('Status endpoint failed, falling back to all berths:', statusError);
        // Fallback to getting all berths and filtering
        berthsResponse = await berthApi.getAll();
        console.log('Loaded all berths:', berthsResponse.data.length);
      }
      
      berthsData = (berthsResponse.data as any[])?.map((b: any) => ({
        berthId: b.berthId || b.id || 0,
        name: b.name || `Berth ${b.identifier || b.id}`,
        identifier: b.identifier || `B-${b.id}`,
        type: b.type || 'Container',
        capacity: b.capacity || 100,
        currentLoad: b.currentLoad || 0,
        maxShipLength: b.maxShipLength,
        maxDraft: b.maxDraft,
        status: b.status || 'Available',
        availableServices: b.availableServices,
        craneCount: b.craneCount,
        hourlyRate: b.hourlyRate,
        priority: b.priority,
        notes: b.notes,
        portId: b.portId || 1,
        portName: b.portName || 'Unknown Port',
        activeAssignmentCount: b.activeAssignmentCount || 0
      })) || [];
      
      console.log('Final berths data:', berthsData.length, 'Available berths:', berthsData.filter(b => ['Available', 'Free', 'Ready'].includes(b.status)).length);
    } catch (error) {
      berthError.value = 'Failed to load berth data';
      console.error('Berth API error:', error);
      // Enhanced mock berth data as fallback
      berthsData = [
        { 
          berthId: 1, name: 'Alpha Terminal', identifier: 'A-001', type: 'Container', 
          capacity: 150, currentLoad: 0, status: 'Available', portId: 1, 
          portName: 'Port of Singapore', activeAssignmentCount: 0, craneCount: 3, 
          hourlyRate: 200, availableServices: 'Loading, Unloading, Storage',
          maxShipLength: 300, maxDraft: 15, priority: 'High'
        },
        { 
          berthId: 2, name: 'Beta Wharf', identifier: 'B-002', type: 'Container', 
          capacity: 120, currentLoad: 85, status: 'Occupied', portId: 1, 
          portName: 'Port of Singapore', activeAssignmentCount: 1, craneCount: 2, 
          hourlyRate: 180, availableServices: 'Loading, Unloading',
          maxShipLength: 250, maxDraft: 12, priority: 'Medium'
        },
        { 
          berthId: 3, name: 'Gamma Terminal', identifier: 'G-003', type: 'Bulk', 
          capacity: 200, currentLoad: 0, status: 'Available', portId: 2, 
          portName: 'Port of Rotterdam', activeAssignmentCount: 0, craneCount: 4, 
          hourlyRate: 250, availableServices: 'Loading, Unloading, Storage, Refueling',
          maxShipLength: 400, maxDraft: 18, priority: 'High'
        },
        { 
          berthId: 4, name: 'Delta Dock', identifier: 'D-004', type: 'Container', 
          capacity: 180, currentLoad: 0, status: 'Ready', portId: 3, 
          portName: 'Port of Hamburg', activeAssignmentCount: 0, craneCount: 2, 
          hourlyRate: 220, availableServices: 'Loading, Unloading, Storage',
          maxShipLength: 280, maxDraft: 14, priority: 'Medium'
        },
        { 
          berthId: 5, name: 'Echo Terminal', identifier: 'E-005', type: 'RoRo', 
          capacity: 80, currentLoad: 0, status: 'Available', portId: 2, 
          portName: 'Port of Rotterdam', activeAssignmentCount: 0, craneCount: 1, 
          hourlyRate: 180, availableServices: 'Vehicle Loading, Storage',
          maxShipLength: 200, maxDraft: 8, priority: 'Low'
        },
        { 
          berthId: 6, name: 'Foxtrot Pier', identifier: 'F-006', type: 'Container', 
          capacity: 160, currentLoad: 0, status: 'Free', portId: 4, 
          portName: 'Port of Los Angeles', activeAssignmentCount: 0, craneCount: 3, 
          hourlyRate: 280, availableServices: 'Loading, Unloading, Storage, Inspection',
          maxShipLength: 350, maxDraft: 16, priority: 'High'
        }
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
        portId: p.portId || p.id || 0,
        name: p.name || 'Unknown Port',
        location: p.location || 'Unknown Location',
        country: p.country || 'Unknown',
        capacity: p.capacity || 0
      })) || [];
    } catch (error) {
      // Mock port data as fallback
      portsData = [
        { portId: 7, name: 'Port of Singapore', location: 'Singapore', country: 'Singapore', capacity: 37.2 },
        { portId: 8, name: 'Port of Rotterdam', location: 'Rotterdam, Netherlands', country: 'Netherlands', capacity: 15.3 },
        { portId: 9, name: 'Port of Los Angeles', location: 'Los Angeles, CA, USA', country: 'United States', capacity: 47.0 }
      ];
    }

    // Set the data to component state
    containers.value = containersData;
    ships.value = shipsData;
    berths.value = berthsData;
    berthAssignments.value = berthAssignmentsData;
    ports.value = portsData;

    // Set default port (Singapore) if no port selected
    if (!selectedPort.value && portsData.length > 0) {
      selectedPort.value = portsData[0]; // Port of Singapore as default
    }

    // Load analytics data and calculate financial metrics
    await loadAnalyticsData();

    // Update recent containers and movements
    updateRecentContainers();

  } catch (error) {
    console.error('Error loading dashboard data:', error);
    metricsError.value = 'Failed to load dashboard data';
    // Still update with whatever data we have
    updateRecentContainers();
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
  // Get ALL containers and sort by creation date (latest first)
  const sortedContainers = [...containers.value].sort((a, b) => {
    // If containers have createdAt dates, use those
    const aCreated = (a as any).createdAt;
    const bCreated = (b as any).createdAt;
    if (aCreated && bCreated) {
      return new Date(bCreated).getTime() - new Date(aCreated).getTime();
    }
    // Otherwise, sort by ID in descending order (assuming higher ID = newer)
    const aId = typeof a.id === 'string' ? parseInt(a.id) || 0 : (a.id || 0);
    const bId = typeof b.id === 'string' ? parseInt(b.id) || 0 : (b.id || 0);
    return bId - aId;
  });

  recentContainers.value = sortedContainers.map((container, index) => {
    const statuses = [
      'Arrived', 'Loading', 'Inspection', 'Departed', 'In Transit', 
      'At Port', 'Loaded', 'Unloaded', 'Available'
    ];
    
    // Find assigned berth for this container
    const assignment = berthAssignments.value.find(a => a.shipId === container.shipId);
    const berth = assignment ? berths.value.find(b => b.berthId === assignment.berthId) : null;
    
    // Use actual container data if available, otherwise generate realistic status
    const currentStatus = container.status || statuses[index % statuses.length];
    const currentLocation = container.currentLocation || (berth ? `Dock ${berth.identifier}` : 'Port Area');
    
    // Use existing createdAt if available, otherwise generate realistic timestamps
    let createdAt = container.createdAt;
    if (!createdAt) {
      const hoursAgo = Math.floor(index / 2) + Math.random() * 2; // Spread over recent hours
      const generatedDate = new Date();
      generatedDate.setHours(generatedDate.getHours() - hoursAgo);
      createdAt = generatedDate.toISOString();
    }
    
    return {
      id: container.containerNumber || container.id || `CNT-${container.id}`,
      containerNumber: container.containerNumber || `CNT-${container.id}`,
      status: currentStatus,
      berth: berth ? `Dock ${berth.identifier}` : currentLocation,
      location: currentLocation, 
      time: new Date(createdAt).toLocaleTimeString().slice(0, 5),
      type: container.type === 'Dry' ? 'Standard' : 
            container.type === 'Refrigerated' ? 'Refrigerated' :
            container.type === 'Tank' ? 'Tank' :
            container.type === 'OpenTop' ? 'OpenTop' : 'Standard',
      createdAt: createdAt, // Use proper createdAt timestamp
      weight: container.weight,
      shipId: container.shipId,
      cargoType: container.cargoType || 'General',
      destination: container.destination || 'Unknown',
      condition: container.condition || 'Good',
      size: container.size || '40ft'
    };
  });
};

const handleViewAllContainers = () => {
  // TODO: Navigate to container list
  console.log('View all containers');
};

const handlePortChange = async (port: Port | null) => {
  selectedPort.value = port;
  
  // Reload data for the selected port if we have backend integration for port filtering
  if (port) {
    try {
      loading.value = true;
      
      // Load port-specific berths
      const berthsResponse = await berthApi.getByPort(port.portId);
      if (berthsResponse.data) {
        // Update berths with port-specific data
        const portBerths = berthsResponse.data.map((b: any) => ({
          berthId: b.berthId || b.id,
          name: b.name || `Berth ${b.berthId}`,
          identifier: b.identifier || b.name,
          type: b.type || 'General Cargo',
          capacity: b.capacity || 0,
          currentLoad: b.currentLoad || 0,
          maxShipLength: b.maxShipLength || 0,
          maxDraft: b.maxDraft || 0,
          status: b.status || 'Available',
          availableServices: b.availableServices || '',
          craneCount: b.craneCount || 0,
          hourlyRate: b.hourlyRate || 0,
          priority: b.priority || 'Normal',
          notes: b.notes || '',
          portId: b.portId || port.portId,
          portName: b.portName || port.name,
          activeAssignmentCount: b.activeAssignmentCount || 0
        }));
        
        berths.value = portBerths;
      }
    } catch (error) {
      console.error('Error loading port-specific data:', error);
    } finally {
      loading.value = false;
    }
  }
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