<template>
  <div class="min-h-screen bg-slate-50">
    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-6 py-8">
      <!-- Page Header -->
      <div class="mb-8">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
          <div class="flex items-center gap-4">
            <div class="p-3 bg-blue-600 rounded-xl shadow-lg">
              <Anchor :size="28" class="text-white" />
            </div>
            <div>
              <h1 class="text-3xl font-bold text-slate-900">Port Operations Management</h1>
              <p class="text-slate-600 mt-1">Monitor berth capacity and ongoing operations</p>
            </div>
          </div>
          <div class="flex items-center gap-4">
            <div class="flex items-center gap-2 bg-green-50 px-4 py-2 rounded-lg border border-green-200">
              <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
              <span class="text-green-700 font-medium">{{ stats.availableBerths }} Available Berths</span>
            </div>
          </div>
        </div>
      </div>
      <!-- Performance Metrics -->
      <section class="mb-8">
        <div class="mb-6">
          <h2 class="text-2xl font-bold text-slate-900 mb-2">Port Performance Overview</h2>
          <p class="text-slate-600">Real-time metrics showing berth utilization and operational efficiency</p>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <div class="bg-white rounded-xl border border-slate-200 p-6 hover:shadow-lg transition-all duration-300 hover:-translate-y-1">
            <div class="flex items-start justify-between mb-4">
              <div class="p-3 bg-blue-50 rounded-lg">
                <MapPin :size="24" class="text-blue-600" />
              </div>
            </div>
            <div class="mb-3">
              <p class="text-3xl font-bold text-slate-900">{{ stats.totalBerths }}</p>
              <p class="text-sm font-medium text-slate-600">Total Berths</p>
              <div class="flex items-center gap-1 mt-2">
                <TrendingUp :size="14" class="text-green-600" />
                <span class="text-sm font-medium text-green-600">100%</span>
                <span class="text-sm text-slate-500">capacity</span>
              </div>
            </div>
          </div>

          <div class="bg-white rounded-xl border border-slate-200 p-6 hover:shadow-lg transition-all duration-300 hover:-translate-y-1">
            <div class="flex items-start justify-between mb-4">
              <div class="p-3 bg-green-50 rounded-lg">
                <CheckCircle :size="24" class="text-green-600" />
              </div>
            </div>
            <div class="mb-3">
              <p class="text-3xl font-bold text-slate-900">{{ stats.availableBerths }}</p>
              <p class="text-sm font-medium text-slate-600">Available Berths</p>
              <div class="flex items-center gap-1 mt-2">
                <span class="text-sm font-medium text-green-600">{{ stats.availabilityPercentage }}%</span>
                <span class="text-sm text-slate-500">available</span>
              </div>
            </div>
          </div>

          <div class="bg-white rounded-xl border border-slate-200 p-6 hover:shadow-lg transition-all duration-300 hover:-translate-y-1">
            <div class="flex items-start justify-between mb-4">
              <div class="p-3 bg-orange-50 rounded-lg">
                <Clock :size="24" class="text-orange-600" />
              </div>
            </div>
            <div class="mb-3">
              <p class="text-3xl font-bold text-slate-900">{{ stats.occupiedBerths }}</p>
              <p class="text-sm font-medium text-slate-600">Occupied Berths</p>
              <div class="flex items-center gap-1 mt-2">
                <span class="text-sm font-medium text-orange-600">{{ stats.occupancyPercentage }}%</span>
                <span class="text-sm text-slate-500">occupied</span>
              </div>
            </div>
          </div>

          <div class="bg-white rounded-xl border border-slate-200 p-6 hover:shadow-lg transition-all duration-300 hover:-translate-y-1">
            <div class="flex items-start justify-between mb-4">
              <div class="p-3 bg-purple-50 rounded-lg">
                <Ship :size="24" class="text-purple-600" />
              </div>
            </div>
            <div class="mb-3">
              <p class="text-3xl font-bold text-slate-900">{{ stats.totalShips }}</p>
              <p class="text-sm font-medium text-slate-600">Ships in Port</p>
              <div class="flex items-center gap-1 mt-2">
                <span class="text-sm font-medium text-purple-600">{{ stats.dockedShips }}</span>
                <span class="text-sm text-slate-500">docked</span>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Main Dashboard Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
        <!-- Berth Management -->
        <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
          <div class="border-b border-slate-200 p-6">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="p-2 bg-blue-50 rounded-lg">
                  <Anchor :size="20" class="text-blue-600" />
                </div>
                <div>
                  <h3 class="text-xl font-semibold text-slate-900">Berth Management</h3>
                  <p class="text-sm text-slate-600">Real-time berth status and assignments</p>
                </div>
              </div>
              <button class="px-4 py-2 text-sm font-medium text-blue-600 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors">
                Assign Berth
              </button>
            </div>
          </div>
          
          <div class="p-6">
            <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div
                v-for="(berth, index) in getBerthData()"
                :key="berth.id"
                class="p-4 border border-slate-200 rounded-lg hover:shadow-md transition-all duration-200 cursor-pointer group animate-slideIn"
                :style="{ animationDelay: `${index * 50}ms` }"
                :class="getBerthBorderColor(berth.status)"
              >
                <div class="text-center space-y-3">
                  <div class="text-lg font-bold text-slate-900">{{ berth.id }}</div>
                  <span 
                    class="inline-flex items-center px-2 py-1 text-xs font-semibold rounded-full"
                    :class="getBerthStatusColor(berth.status)"
                  >
                    {{ berth.status }}
                  </span>
                  <div v-if="berth.status === 'Occupied'" class="text-xs space-y-2">
                    <div class="p-2 bg-slate-50 rounded-lg">
                      <div class="text-slate-600 mb-1">Ship:</div>
                      <div class="font-medium text-slate-900 text-xs">{{ berth.ship }}</div>
                    </div>
                    <div class="flex items-center justify-between">
                      <span class="text-slate-600">Capacity:</span>
                      <span class="font-bold text-blue-600">{{ berth.capacity }}</span>
                    </div>
                    <div class="w-full bg-slate-200 rounded-full h-2">
                      <div 
                        class="bg-blue-500 h-2 rounded-full transition-all duration-1000"
                        :style="{ width: berth.capacity }"
                      ></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Active Operations -->
        <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
          <div class="border-b border-slate-200 p-6">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="p-2 bg-purple-50 rounded-lg">
                  <Activity :size="20" class="text-purple-600" />
                </div>
                <div>
                  <h3 class="text-xl font-semibold text-slate-900">Active Operations</h3>
                  <p class="text-sm text-slate-600">Current loading and unloading activities</p>
                </div>
              </div>
              <button class="px-4 py-2 text-sm font-medium text-purple-600 bg-purple-50 rounded-lg hover:bg-purple-100 transition-colors">
                New Operation
              </button>
            </div>
          </div>
          
          <div class="p-6">
            <div class="space-y-4">
              <div
                v-for="(operation, index) in operations"
                :key="operation.id"
                class="p-4 bg-slate-50 rounded-lg hover:bg-slate-100 transition-all duration-200 animate-slideIn"
                :style="{ animationDelay: `${(index + 8) * 50}ms` }"
              >
                <div class="flex items-center justify-between mb-3">
                  <div class="flex items-center gap-3">
                    <div class="w-10 h-10 rounded-lg bg-white border-2 border-slate-200 flex items-center justify-center font-bold text-slate-700">
                      {{ operation.id.slice(-2) }}
                    </div>
                    <div>
                      <div class="font-semibold text-slate-900">{{ operation.id }}</div>
                      <div class="text-sm text-slate-600">{{ operation.vessel }}</div>
                    </div>
                  </div>
                  <div class="text-right">
                    <span 
                      class="inline-flex items-center px-2 py-1 text-xs font-semibold rounded-full"
                      :class="getPriorityColor(operation.priority)"
                    >
                      {{ operation.priority }}
                    </span>
                    <div class="text-xs text-slate-600 mt-1">
                      {{ operation.type }}
                    </div>
                  </div>
                </div>
                
                <div class="space-y-3">
                  <div class="flex justify-between text-sm">
                    <span class="text-slate-600">Progress</span>
                    <span class="font-semibold text-slate-900">{{ operation.progress }}%</span>
                  </div>
                  <div class="w-full bg-slate-200 rounded-full h-3">
                    <div
                      class="bg-gradient-to-r from-blue-500 to-blue-600 h-3 rounded-full transition-all duration-1000 ease-out"
                      :style="{ width: `${operation.progress}%` }"
                    ></div>
                  </div>
                  <div class="flex items-center gap-2 text-sm text-slate-600">
                    <Clock :size="14" />
                    <span>ETA: {{ operation.eta }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Action Center -->
      <section class="mt-8">
        <div class="bg-white rounded-xl border border-slate-200 shadow-sm p-6">
          <h3 class="text-lg font-semibold text-slate-900 mb-4">Quick Actions</h3>
          <div class="flex flex-wrap gap-3">
            <button 
              @click="showBerthAssignmentForm = true"
              class="px-6 py-3 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-lg transition-colors duration-200 flex items-center gap-2"
            >
              <Plus :size="16" />
              Assign Container to Berth
            </button>
            <button 
              @click="showBerthForm = true"
              class="px-6 py-3 border border-slate-300 rounded-lg hover:bg-slate-50 font-medium transition-colors flex items-center gap-2"
            >
              <Settings :size="16" />
              Add New Berth
            </button>
            <button class="px-6 py-3 border border-slate-300 rounded-lg hover:bg-slate-50 font-medium transition-colors flex items-center gap-2">
              <FileText :size="16" />
              Generate Report
            </button>
            <button class="px-6 py-3 border border-slate-300 rounded-lg hover:bg-slate-50 font-medium transition-colors flex items-center gap-2">
              <AlertTriangle :size="16" />
              Emergency Protocols
            </button>
          </div>
        </div>
      </section>
    </main>

    <!-- Berth Assignment Form Modal -->
    <div v-if="showBerthAssignmentForm" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-4xl w-full max-h-screen overflow-y-auto">
        <BerthAssignmentForm 
          :assignment="selectedAssignment"
          :isEditing="isEditingAssignment"
          @submit="handleAssignmentSubmit"
          @cancel="closeBerthAssignmentForm"
        />
      </div>
    </div>

    <!-- Berth Form Modal -->
    <div v-if="showBerthForm" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div class="bg-white rounded-xl shadow-xl max-w-4xl w-full max-h-screen overflow-y-auto">
        <BerthForm 
          :berth="selectedBerth"
          :isEditing="isEditingBerth"
          @submit="handleBerthSubmit"
          @cancel="closeBerthForm"
        />
      </div>
    </div>
  </div>
</template>

<script>
import BerthForm from '../forms/BerthForm.vue';
import BerthAssignmentForm from '../forms/BerthAssignmentForm.vue';
import { 
  Anchor, 
  Activity, 
  MapPin, 
  CheckCircle, 
  AlertTriangle, 
  Clock, 
  TrendingUp, 
  Plus, 
  Settings, 
  FileText,
  Ship
} from 'lucide-vue-next';
import { portApi, shipApi, berthApi, berthAssignmentApi, containerApi } from '../services/api';

export default {
  name: 'PortOperationManagement',
  components: {
    BerthForm,
    BerthAssignmentForm,
    Anchor,
    Activity,
    MapPin,
    CheckCircle,
    AlertTriangle,
    Clock,
    TrendingUp,
    Plus,
    Settings,
    FileText,
    Ship
  },
  data() {
    return {
      loading: true,
      berths: [],
      ships: [],
      containers: [],
      berthAssignments: [],
      ports: [],
      operations: [],
      
      // Form state management
      showBerthForm: false,
      selectedBerth: null,
      isEditingBerth: false,

      showBerthAssignmentForm: false,
      selectedAssignment: null,
      isEditingAssignment: false
    };
  },
  computed: {
    stats() {
      const totalBerths = this.berths.length;
      const occupiedBerths = this.berthAssignments.length;
      const availableBerths = totalBerths - occupiedBerths;
      const availabilityPercentage = totalBerths > 0 ? Math.round((availableBerths / totalBerths) * 100) : 0;
      const occupancyPercentage = totalBerths > 0 ? Math.round((occupiedBerths / totalBerths) * 100) : 0;
      const totalShips = this.ships.length;
      const dockedShips = this.berthAssignments.length; // Ships with berth assignments

      return {
        totalBerths,
        occupiedBerths,
        availableBerths,
        availabilityPercentage,
        occupancyPercentage,
        totalShips,
        dockedShips
      };
    },
    totalBerths() {
      return this.berths.length;
    },
    occupiedBerths() {
      return this.berthAssignments.length;
    },
    freeBerths() {
      return this.totalBerths - this.occupiedBerths;
    },
    berthUtilization() {
      return this.totalBerths > 0 ? Math.round((this.occupiedBerths / this.totalBerths) * 100) : 0;
    },
    activeShips() {
      return this.ships.length;
    },
    totalContainers() {
      return this.containers.length;
    }
  },
  async mounted() {
    await this.loadOperationData();
  },
  methods: {
    async loadOperationData() {
      try {
        this.loading = true;
        // Loading port operation data

        // Load all data in parallel
        const [portsResponse, berthsResponse, shipsResponse, containersResponse, assignmentsResponse] = await Promise.all([
          portApi.getAll(),
          berthApi.getAll(),
          shipApi.getAll(),
          containerApi.getAll(),
          berthAssignmentApi.getAll()
        ]);

        // API data loaded successfully

        this.ports = portsResponse.data || [];
        this.berths = berthsResponse.data || [];
        this.ships = shipsResponse.data || [];
        this.containers = containersResponse.data || [];
        this.berthAssignments = assignmentsResponse.data || [];

        console.log('Loaded data counts:', {
          ports: this.ports.length,
          berths: this.berths.length,
          ships: this.ships.length,
          containers: this.containers.length,
          berthAssignments: this.berthAssignments.length
        });

        // Transform data for operations display
        this.updateOperations();

      } catch (error) {
        console.error('Error loading operation data:', error);
        // Show some mock data if API fails for testing
        this.loadMockData();
      } finally {
        this.loading = false;
      }
    },

    loadMockData() {
      console.log('Loading mock data for testing...');
      // Mock data for testing when API is not available
      this.ports = [
        { id: 1, name: 'Port of Hamburg', location: 'Hamburg, Germany' },
        { id: 2, name: 'Port of Rotterdam', location: 'Rotterdam, Netherlands' }
      ];

      this.berths = [
        { id: 1, identifier: 'B-001', status: 'Available', portId: 1 },
        { id: 2, identifier: 'B-002', status: 'Occupied', portId: 1 },
        { id: 3, identifier: 'B-003', status: 'Available', portId: 1 },
        { id: 4, identifier: 'B-004', status: 'Maintenance', portId: 1 },
        { id: 5, identifier: 'B-005', status: 'Available', portId: 2 },
        { id: 6, identifier: 'B-006', status: 'Occupied', portId: 2 }
      ];

      this.ships = [
        { id: 1, name: 'MV Ocean Explorer', status: 'Docked' },
        { id: 2, name: 'MV Cargo Master', status: 'Docked' },
        { id: 3, name: 'MV Sea Voyager', status: 'In Transit' }
      ];

      this.containers = [
        { id: 1, containerNumber: 'CONT-001', shipId: 1, status: 'Loading' },
        { id: 2, containerNumber: 'CONT-002', shipId: 1, status: 'Loaded' },
        { id: 3, containerNumber: 'CONT-003', shipId: 2, status: 'Unloading' }
      ];

      this.berthAssignments = [
        { id: 1, berthId: 2, shipId: 1, status: 'Active' },
        { id: 2, berthId: 6, shipId: 2, status: 'Active' }
      ];

      this.updateOperations();
      console.log('Mock data loaded successfully');
    },

    updateOperations() {
      // Create operations from berth assignments and ships
      this.operations = this.berthAssignments.slice(0, 4).map((assignment, index) => {
        const ship = this.ships.find(s => s.id === assignment.shipId);
        const berth = this.berths.find(b => b.id === assignment.berthId);
        
        const operationTypes = ['Loading', 'Unloading', 'Inspection', 'Refueling'];
        const priorities = ['High', 'Medium', 'Low'];
        const etas = ['2 hours', '4 hours', '30 min', '1.5 hours'];
        
        return {
          id: `OP-${assignment.id}`,
          type: operationTypes[index % operationTypes.length],
          vessel: ship ? ship.name : `Ship ${assignment.shipId}`,
          progress: Math.floor(Math.random() * 40) + 60, // Random progress between 60-100%
          eta: etas[index % etas.length],
          priority: priorities[index % priorities.length]
        };
      });
    },

    getBerthData() {
      // Transform berths with assignment information
      return this.berths.map(berth => {
        const assignment = this.berthAssignments.find(a => a.berthId === berth.id);
        const ship = assignment ? this.ships.find(s => s.id === assignment.shipId) : null;
        const container = assignment ? this.containers.find(c => c.shipId === assignment.shipId) : null;
        
        return {
          id: berth.identifier || berth.id,
          status: assignment ? 'Occupied' : (berth.status === 'Available' ? 'Free' : 'Maintenance'),
          container: container ? container.containerNumber : null,
          ship: ship ? ship.name : null,
          capacity: assignment ? `${Math.floor(Math.random() * 30) + 70}%` : '0%'
        };
      });
    },

    async handleBerthSubmit(berthData) {
      try {
        if (this.isEditingBerth) {
          // Update existing berth
          await berthApi.update(this.selectedBerth.id, berthData);
        } else {
          // Add new berth
          await berthApi.create(berthData);
        }
        
        // Reload data to reflect changes
        await this.loadOperationData();
        this.closeBerthForm();
      } catch (error) {
        console.error('Error saving berth:', error);
      }
    },

    closeBerthForm() {
      this.showBerthForm = false;
      this.selectedBerth = null;
      this.isEditingBerth = false;
    },

    async handleAssignmentSubmit(assignmentData) {
      try {
        await berthAssignmentApi.create(assignmentData);
        
        // Reload data to reflect changes
        await this.loadOperationData();
        this.closeBerthAssignmentForm();
      } catch (error) {
        console.error('Error creating assignment:', error);
      }
    },

    closeBerthAssignmentForm() {
      this.showBerthAssignmentForm = false;
      this.selectedAssignment = null;
      this.isEditingAssignment = false;
    },

    getBerthStatusColor(status) {
      const statusColors = {
        "Occupied": "bg-orange-100 text-orange-800 border-orange-200",
        "Free": "bg-green-100 text-green-800 border-green-200",
        "Maintenance": "bg-red-100 text-red-800 border-red-200",
      };
      return statusColors[status] || "bg-slate-100 text-slate-800 border-slate-200";
    },

    getBerthBorderColor(status) {
      const borderColors = {
        "Occupied": "border-orange-300 bg-orange-50",
        "Free": "border-green-300 bg-green-50",
        "Maintenance": "border-red-300 bg-red-50",
      };
      return borderColors[status] || "border-slate-200 bg-white";
    },

    getPriorityColor(priority) {
      const priorityColors = {
        "High": "bg-red-100 text-red-800 border-red-200",
        "Medium": "bg-orange-100 text-orange-800 border-orange-200",
        "Low": "bg-green-100 text-green-800 border-green-200",
      };
      return priorityColors[priority] || "bg-slate-100 text-slate-800 border-slate-200";
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
  
  .grid-cols-1.lg\:grid-cols-2 {
    grid-template-columns: repeat(1, minmax(0, 1fr));
  }
  
  .grid-cols-2.md\:grid-cols-4 {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
  
  .flex-col.md\:flex-row {
    flex-direction: column;
  }
  
  .gap-8 {
    gap: 1.5rem;
  }
  
  .gap-6 {
    gap: 1rem;
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
  
  .gap-3 {
    gap: 0.5rem;
  }
  
  .mb-8 {
    margin-bottom: 1.5rem;
  }
  
  .mb-6 {
    margin-bottom: 1rem;
  }
  
  .flex-wrap {
    flex-wrap: wrap;
  }
  
  .px-6 {
    padding-left: 0.75rem;
    padding-right: 0.75rem;
  }
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
button:focus-visible {
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
</style>