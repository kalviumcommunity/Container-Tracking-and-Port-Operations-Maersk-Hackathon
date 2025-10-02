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
              <span class="text-green-700 font-medium">8 Available Berths</span>
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
              <p class="text-3xl font-bold text-slate-900">15</p>
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
              <div class="p-3 bg-orange-50 rounded-lg">
                <Activity :size="24" class="text-orange-600" />
              </div>
            </div>
            <div class="mb-3">
              <p class="text-3xl font-bold text-slate-900">7</p>
              <p class="text-sm font-medium text-slate-600">Occupied Berths</p>
              <div class="flex items-center gap-1 mt-2">
                <TrendingUp :size="14" class="text-orange-600" />
                <span class="text-sm font-medium text-orange-600">47%</span>
                <span class="text-sm text-slate-500">utilization</span>
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
              <p class="text-3xl font-bold text-slate-900">8</p>
              <p class="text-sm font-medium text-slate-600">Available Berths</p>
              <div class="flex items-center gap-1 mt-2">
                <TrendingUp :size="14" class="text-green-600" />
                <span class="text-sm font-medium text-green-600">53%</span>
                <span class="text-sm text-slate-500">available</span>
              </div>
            </div>
          </div>

          <div class="bg-white rounded-xl border border-slate-200 p-6 hover:shadow-lg transition-all duration-300 hover:-translate-y-1">
            <div class="flex items-start justify-between mb-4">
              <div class="p-3 bg-red-50 rounded-lg">
                <AlertTriangle :size="24" class="text-red-600" />
              </div>
            </div>
            <div class="mb-3">
              <p class="text-3xl font-bold text-slate-900">2</p>
              <p class="text-sm font-medium text-slate-600">Under Maintenance</p>
              <div class="flex items-center gap-1 mt-2">
                <Clock :size="14" class="text-red-600" />
                <span class="text-sm font-medium text-red-600">2-3 hrs</span>
                <span class="text-sm text-slate-500">remaining</span>
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
                v-for="(berth, index) in berths"
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

<script setup lang="ts">
import { ref } from 'vue';
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
  FileText 
} from 'lucide-vue-next';

interface Berth {
  id: string;
  status: string;
  container: string | null;
  ship: string | null;
  capacity: string;
}

interface Operation {
  id: string;
  type: string;
  vessel: string;
  progress: number;
  eta: string;
  priority: string;
}

const berths = ref<Berth[]>([
  { id: "B-01", status: "Occupied", container: "CTR-001", ship: "MV Ocean Pearl", capacity: "85%" },
  { id: "B-02", status: "Free", container: null, ship: null, capacity: "0%" },
  { id: "B-03", status: "Occupied", container: "CTR-003", ship: "MV Blue Wave", capacity: "72%" },
  { id: "B-04", status: "Maintenance", container: null, ship: null, capacity: "0%" },
  { id: "B-05", status: "Free", container: null, ship: null, capacity: "0%" },
  { id: "B-06", status: "Occupied", container: "CTR-006", ship: "MV Sea Breeze", capacity: "90%" },
  { id: "B-07", status: "Occupied", container: "CTR-007", ship: "MV Atlantic", capacity: "67%" },
  { id: "B-08", status: "Free", container: null, ship: null, capacity: "0%" },
]);

const operations = ref<Operation[]>([
  { id: "OP-001", type: "Loading", vessel: "MV Ocean Pearl", progress: 85, eta: "2 hours", priority: "High" },
  { id: "OP-002", type: "Unloading", vessel: "MV Blue Wave", progress: 45, eta: "4 hours", priority: "Medium" },
  { id: "OP-003", type: "Inspection", vessel: "MV Sea Breeze", progress: 90, eta: "30 min", priority: "High" },
  { id: "OP-004", type: "Refueling", vessel: "MV Atlantic", progress: 60, eta: "1.5 hours", priority: "Low" },
]);

// Form state management
const showBerthForm = ref(false);
const selectedBerth = ref(null);
const isEditingBerth = ref(false);

const showBerthAssignmentForm = ref(false);
const selectedAssignment = ref(null);
const isEditingAssignment = ref(false);

// Form handlers
const handleBerthSubmit = (berthData: any) => {
  if (isEditingBerth.value) {
    // Update existing berth
    const index = berths.value.findIndex(b => b.id === berthData.berthNumber);
    if (index !== -1) {
      berths.value[index] = {
        id: berthData.berthNumber,
        status: berthData.status,
        container: null,
        ship: null,
        capacity: "0%"
      };
    }
  } else {
    // Add new berth
    berths.value.push({
      id: berthData.berthNumber,
      status: berthData.status,
      container: null,
      ship: null,
      capacity: "0%"
    });
  }
  closeBerthForm();
};

const closeBerthForm = () => {
  showBerthForm.value = false;
  selectedBerth.value = null;
  isEditingBerth.value = false;
};

const handleAssignmentSubmit = (assignmentData: any) => {
  // Update berth status to occupied
  const berth = berths.value.find(b => b.id === assignmentData.berthId);
  if (berth) {
    berth.status = "Occupied";
    berth.container = `CTR-${Date.now()}`;
    berth.capacity = "75%";
  }
  closeBerthAssignmentForm();
};

const closeBerthAssignmentForm = () => {
  showBerthAssignmentForm.value = false;
  selectedAssignment.value = null;
  isEditingAssignment.value = false;
};

const getBerthStatusColor = (status: string): string => {
  const statusColors = {
    "Occupied": "bg-orange-100 text-orange-800 border-orange-200",
    "Free": "bg-green-100 text-green-800 border-green-200",
    "Maintenance": "bg-red-100 text-red-800 border-red-200",
  };
  return statusColors[status as keyof typeof statusColors] || "bg-slate-100 text-slate-800 border-slate-200";
};

const getBerthBorderColor = (status: string): string => {
  const borderColors = {
    "Occupied": "border-orange-300 bg-orange-50",
    "Free": "border-green-300 bg-green-50",
    "Maintenance": "border-red-300 bg-red-50",
  };
  return borderColors[status as keyof typeof borderColors] || "border-slate-200 bg-white";
};

const getPriorityColor = (priority: string): string => {
  const priorityColors = {
    "High": "bg-red-100 text-red-800 border-red-200",
    "Medium": "bg-orange-100 text-orange-800 border-orange-200",
    "Low": "bg-green-100 text-green-800 border-green-200",
  };
  return priorityColors[priority as keyof typeof priorityColors] || "bg-slate-100 text-slate-800 border-slate-200";
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