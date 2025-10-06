<template>
  <div class="min-h-screen bg-slate-50">
    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-6 py-8">
      <!-- Page Header -->
      <div class="mb-8">
        <div class="flex items-center gap-4 mb-4">
          <div class="p-3 bg-blue-600 rounded-xl shadow-lg">
            <Ship :size="28" class="text-white" />
          </div>
          <div>
            <h1 class="text-3xl font-bold text-slate-900">Port Operations Dashboard</h1>
            <p class="text-slate-600 mt-1">Port Terminal - Real-time Operations</p>
            <div v-if="isAdminUser" class="mt-2">
              <span class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-emerald-100 text-emerald-800">
                <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
                </svg>
                System Admin - Full Database Access
              </span>
            </div>
          </div>
        </div>
        <div class="flex items-center gap-6 text-sm text-slate-600">
          <div class="flex items-center gap-2 bg-slate-100 px-4 py-2 rounded-lg">
            <Globe :size="16" class="text-blue-600" />
            <span class="font-medium">Chennai Port</span>
          </div>
          <div class="flex items-center gap-2 bg-green-50 px-4 py-2 rounded-lg border border-green-200">
            <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
            <span class="text-green-700 font-medium">Live</span>
          </div>
          <div class="flex items-center gap-2">
            <Clock :size="16" />
            <span>{{ currentTime }}</span>
          </div>
        </div>
      </div>
      <!-- Key Metrics Grid -->
      <section class="mb-12">
        <div class="mb-8">
          <h2 class="text-2xl font-bold text-slate-900 mb-2">Operations Overview</h2>
          <p class="text-slate-600">Real-time performance metrics updated every 5 minutes</p>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <div
            v-for="(stat, index) in stats"
            :key="index"
            class="bg-white rounded-xl border border-slate-200 p-6 hover:shadow-lg transition-all duration-300 hover:-translate-y-1 group"
            :class="{ 'animate-slideIn': true }"
            :style="{ animationDelay: `${index * 100}ms` }"
          >
            <div class="flex items-start justify-between mb-4">
              <div class="p-3 rounded-lg" :class="stat.bgColor">
                <component :is="stat.icon" :size="24" :class="stat.iconColor" />
              </div>
              <div class="text-right">
                <p class="text-xs font-medium text-slate-500 uppercase tracking-wide">{{ stat.title }}</p>
              </div>
            </div>
            <div class="mb-3">
              <p class="text-3xl font-bold text-slate-900">{{ stat.value }}</p>
              <div class="flex items-center gap-1 mt-2">
                <TrendingUp :size="14" class="text-green-600" />
                <span class="text-sm font-medium text-green-600">{{ stat.change }}</span>
                <span class="text-sm text-slate-500">vs yesterday</span>
              </div>
            </div>
            <div class="w-full bg-slate-100 rounded-full h-2">
              <div 
                class="h-2 rounded-full transition-all duration-1000 ease-out"
                :class="stat.progressColor"
                :style="{ width: stat.progress }"
              ></div>
            </div>
          </div>
        </div>
      </section>

      <!-- Main Dashboard Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Container Activity Panel -->
        <div class="lg:col-span-2 bg-white rounded-xl border border-slate-200 shadow-sm">
          <div class="border-b border-slate-200 p-6">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="p-2 bg-blue-50 rounded-lg">
                  <Container :size="20" class="text-blue-600" />
                </div>
                <div>
                  <h3 class="text-xl font-semibold text-slate-900">Container Activity</h3>
                  <p class="text-sm text-slate-600">Live tracking of container movements</p>
                </div>
              </div>
              <button class="px-4 py-2 text-sm font-medium text-blue-600 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors">
                View All
              </button>
            </div>
          </div>
          
          <div class="p-6">
            <div class="space-y-4">
              <div 
                v-for="(container, index) in recentContainers"
                :key="container.id"
                class="flex items-center justify-between p-4 bg-slate-50 rounded-lg hover:bg-slate-100 transition-all duration-200 group"
                :class="{ 'animate-slideIn': true }"
                :style="{ animationDelay: `${(index + 4) * 100}ms` }"
              >
                <div class="flex items-center gap-4">
                  <div class="w-12 h-12 rounded-lg bg-white border-2 border-slate-200 flex items-center justify-center font-bold text-slate-700 group-hover:border-blue-300 transition-colors">
                    {{ container.id.slice(-2) }}
                  </div>
                  <div class="flex flex-col">
                    <span class="font-semibold text-slate-900">{{ container.id }}</span>
                    <span class="text-sm text-slate-600">{{ container.type }} Container</span>
                  </div>
                  <span 
                    class="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold"
                    :class="getStatusColor(container.status)"
                  >
                    {{ container.status }}
                  </span>
                </div>
                <div class="text-right">
                  <div class="text-sm font-semibold text-slate-900">{{ container.berth }}</div>
                  <div class="text-xs text-slate-500">{{ container.time }}</div>
                </div>
              </div>
            </div>
            
            <div class="mt-6 pt-4 border-t border-slate-200 flex items-center justify-between">
              <p class="text-sm text-slate-600">
                Showing latest 4 activities • <span class="font-semibold">156 total operations today</span>
              </p>
              <button class="text-sm font-medium text-blue-600 hover:text-blue-700 transition-colors">
                View detailed log →
              </button>
            </div>
          </div>
        </div>

        <!-- Status & Analytics Panel -->
        <div class="space-y-6">
          <!-- Live Status -->
          <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div class="border-b border-slate-200 p-6">
              <div class="flex items-center gap-3">
                <div class="p-2 bg-green-50 rounded-lg">
                  <Activity :size="20" class="text-green-600" />
                </div>
                <div>
                  <h3 class="text-xl font-semibold text-slate-900">Port Status</h3>
                  <p class="text-sm text-slate-600">Real-time operational monitoring</p>
                </div>
              </div>
            </div>
            
            <div class="p-6 space-y-6">
              <div class="p-4 bg-gradient-to-r from-green-50 to-green-100 rounded-lg border border-green-200">
                <div class="flex items-center gap-2 mb-2">
                  <CheckCircle :size="18" class="text-green-600" />
                  <span class="text-sm font-semibold text-green-800">Fully Operational</span>
                </div>
                <p class="text-2xl font-bold text-green-900">All Systems Active</p>
                <p class="text-sm text-green-700 mt-1">No critical alerts or maintenance required</p>
              </div>
              
              <div class="space-y-5">
                <div>
                  <div class="flex justify-between items-center mb-3">
                    <span class="text-sm font-medium text-slate-700">Berth Utilization</span>
                    <span class="text-sm font-bold text-slate-900">47% (7/15)</span>
                  </div>
                  <div class="w-full bg-slate-200 rounded-full h-3">
                    <div class="bg-gradient-to-r from-blue-500 to-blue-600 h-3 rounded-full transition-all duration-1000 ease-out" style="width: 47%"></div>
                  </div>
                  <p class="text-xs text-slate-600 mt-2">8 berths available for incoming vessels</p>
                </div>
                
                <div>
                  <div class="flex justify-between items-center mb-3">
                    <span class="text-sm font-medium text-slate-700">Container Capacity</span>
                    <span class="text-sm font-bold text-slate-900">73% (2,847/3,900)</span>
                  </div>
                  <div class="w-full bg-slate-200 rounded-full h-3">
                    <div class="bg-gradient-to-r from-orange-400 to-orange-500 h-3 rounded-full transition-all duration-1000 ease-out" style="width: 73%"></div>
                  </div>
                  <p class="text-xs text-slate-600 mt-2">Approaching capacity limit - monitor closely</p>
                </div>
              </div>

              <button class="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-3 px-4 rounded-lg transition-colors duration-200 flex items-center justify-center gap-2">
                <TrendingUp :size="16" />
                View Analytics Dashboard
              </button>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div class="border-b border-slate-200 p-6">
              <h3 class="text-lg font-semibold text-slate-900">Quick Actions</h3>
            </div>
            <div class="p-6 space-y-3">
              <button 
                @click="showShipForm = true"
                class="w-full flex items-center gap-3 p-3 text-left rounded-lg hover:bg-slate-50 transition-colors"
              >
                <Ship :size="18" class="text-slate-600" />
                <span class="text-sm font-medium text-slate-700">Add New Ship</span>
              </button>
              <button class="w-full flex items-center gap-3 p-3 text-left rounded-lg hover:bg-slate-50 transition-colors">
                <Users :size="18" class="text-slate-600" />
                <span class="text-sm font-medium text-slate-700">Manage Staff Schedule</span>
              </button>
              <button class="w-full flex items-center gap-3 p-3 text-left rounded-lg hover:bg-slate-50 transition-colors">
                <AlertTriangle :size="18" class="text-slate-600" />
                <span class="text-sm font-medium text-slate-700">Emergency Protocols</span>
              </button>
              <button class="w-full flex items-center gap-3 p-3 text-left rounded-lg hover:bg-slate-50 transition-colors">
                <Container :size="18" class="text-slate-600" />
                <span class="text-sm font-medium text-slate-700">Container Search</span>
              </button>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Ship Form Modal -->
    <div v-if="showShipForm" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-4xl w-full max-h-screen overflow-y-auto">
        <ShipForm 
          :ship="selectedShip"
          :isEditing="isEditingShip"
          @submit="handleShipSubmit"
          @cancel="closeShipForm"
        />
      </div>
    </div>
  </div>
</template>

<script>
import ShipForm from '../forms/ShipForm.vue';
import { Ship, Container, Anchor, Activity, AlertTriangle, CheckCircle, Clock, Users, TrendingUp, Globe } from 'lucide-vue-next';
import { containerApi, portApi, shipApi, berthApi, berthAssignmentApi } from '../services/api';

export default {
  name: 'Dashboard',
  components: {
    ShipForm,
    Ship,
    Container,
    Anchor,
    Activity,
    AlertTriangle,
    CheckCircle,
    Clock,
    Users,
    TrendingUp,
    Globe
  },
  data() {
    return {
      currentTime: new Date().toLocaleTimeString(),
      loading: true,
      stats: [
        { 
          title: "Total Containers", 
          value: "0", 
          icon: Container, 
          change: "+12%",
          bgColor: "bg-blue-50",
          iconColor: "text-blue-600",
          progressColor: "bg-blue-500",
          progress: "0%"
        },
        { 
          title: "Active Ships", 
          value: "0", 
          icon: Ship, 
          change: "+3%",
          bgColor: "bg-green-50",
          iconColor: "text-green-600",
          progressColor: "bg-green-500",
          progress: "0%"
        },
        { 
          title: "Available Berths", 
          value: "0/0", 
          icon: Anchor, 
          change: "-2%",
          bgColor: "bg-orange-50",
          iconColor: "text-orange-600",
          progressColor: "bg-orange-500",
          progress: "0%"
        },
        { 
          title: "Operations Today", 
          value: "0", 
          icon: Activity, 
          change: "+8%",
          bgColor: "bg-purple-50",
          iconColor: "text-purple-600",
          progressColor: "bg-purple-500",
          progress: "0%"
        },
      ],
      containers: [],
      ships: [],
      berths: [],
      berthAssignments: [],
      ports: [],
      recentContainers: [],
      // Form state management
      showShipForm: false,
      selectedShip: null,
      isEditingShip: false,
      timeInterval: null
    };
  },
  async mounted() {
    // Start time interval
    this.timeInterval = setInterval(() => {
      this.currentTime = new Date().toLocaleTimeString();
    }, 1000);

    // Load data from backend
    await this.loadDashboardData();
  },
  beforeUnmount() {
    if (this.timeInterval) {
      clearInterval(this.timeInterval);
    }
  },
  
  computed: {
    isAdminUser() {
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
    }
  },
  
  methods: {
    async loadDashboardData() {
      try {
        this.loading = true;

        // Load all data in parallel
        const [containersResponse, shipsResponse, berthsResponse, berthAssignmentsResponse, portsResponse] = await Promise.all([
          containerApi.getAll(),
          shipApi.getAll(),
          berthApi.getAll(),
          berthAssignmentApi.getAll(),
          portApi.getAll()
        ]);

        this.containers = containersResponse.data || [];
        this.ships = shipsResponse.data || [];
        this.berths = berthsResponse.data || [];
        this.berthAssignments = berthAssignmentsResponse.data || [];
        this.ports = portsResponse.data || [];

        // Update stats with real data
        this.updateStats();
        
        // Update recent containers with real data
        this.updateRecentContainers();

      } catch (error) {
        console.error('Error loading dashboard data:', error);
      } finally {
        this.loading = false;
      }
    },

    updateStats() {
      // Total Containers
      const totalContainers = this.containers.length;
      this.stats[0].value = totalContainers.toString();
      this.stats[0].progress = Math.min((totalContainers / 50) * 100, 100).toFixed(0) + '%';

      // Active Ships
      const activeShips = this.ships.length;
      this.stats[1].value = activeShips.toString();
      this.stats[1].progress = Math.min((activeShips / 20) * 100, 100).toFixed(0) + '%';

      // Available Berths
      const totalBerths = this.berths.length;
      const occupiedBerths = this.berthAssignments.length;
      const availableBerths = totalBerths - occupiedBerths;
      this.stats[2].value = `${availableBerths}/${totalBerths}`;
      this.stats[2].progress = totalBerths > 0 ? ((occupiedBerths / totalBerths) * 100).toFixed(0) + '%' : '0%';

      // Operations Today (total assignments)
      const operationsToday = this.berthAssignments.length;
      this.stats[3].value = operationsToday.toString();
      this.stats[3].progress = Math.min((operationsToday / 30) * 100, 100).toFixed(0) + '%';
    },

    updateRecentContainers() {
      // Get recent containers from the first few containers
      this.recentContainers = this.containers.slice(0, 4).map((container, index) => {
        const statuses = ['Arrived', 'Loading', 'Inspection', 'Departed'];
        const times = ['14:30', '13:45', '12:20', '11:55'];
        
        // Find assigned berth for this container
        const assignment = this.berthAssignments.find(a => a.shipId === container.shipId);
        const berth = assignment ? this.berths.find(b => b.id === assignment.berthId) : null;
        
        return {
          id: container.containerNumber || `CNT-${container.id}`,
          status: statuses[index] || 'Arrived',
          berth: berth ? berth.identifier : 'N/A',
          time: times[index] || new Date().toLocaleTimeString().slice(0, 5),
          type: container.type || 'Dry'
        };
      });
    },

    getStatusColor(status) {
      const statusColors = {
        "Arrived": "bg-blue-100 text-blue-800 border-blue-200",
        "Loading": "bg-orange-100 text-orange-800 border-orange-200",
        "Inspection": "bg-purple-100 text-purple-800 border-purple-200",
        "Departed": "bg-green-100 text-green-800 border-green-200",
      };
      return statusColors[status] || "bg-slate-100 text-slate-800 border-slate-200";
    },

    async handleShipSubmit(shipData) {
      try {
        // Create new ship using API
        await shipApi.create(shipData);
        
        // Reload dashboard data to reflect changes
        await this.loadDashboardData();
        
        this.closeShipForm();
      } catch (error) {
        console.error('Error creating ship:', error);
      }
    },

    closeShipForm() {
      this.showShipForm = false;
      this.selectedShip = null;
      this.isEditingShip = false;
    }
  }
};
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