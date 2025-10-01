<template>
  <div class="min-h-screen bg-slate-50">
    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-6 py-8">
      <!-- Page Header -->
      <div class="mb-8">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
          <div class="flex items-center gap-4">
            <div class="p-3 bg-blue-600 rounded-xl shadow-lg">
              <Activity :size="28" class="text-white" />
            </div>
            <div>
              <h1 class="text-3xl font-bold text-slate-900">Event Streaming Dashboard</h1>
              <p class="text-slate-600 mt-1">Real-time container and port operations monitoring</p>
            </div>
          </div>
          <div class="flex items-center gap-4">
            <div class="flex items-center gap-2 bg-green-50 px-4 py-2 rounded-lg border border-green-200">
              <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
              <span class="text-green-700 font-medium">Live Stream Active</span>
            </div>
            <div class="flex items-center gap-2 bg-slate-100 px-4 py-2 rounded-lg">
              <Zap :size="16" class="text-blue-600" />
              <span class="font-medium text-slate-700">Kafka Connected</span>
            </div>
          </div>
        </div>
      </div>
      <!-- Event Statistics -->
      <section class="mb-8">
        <div class="mb-6">
          <h2 class="text-2xl font-bold text-slate-900 mb-2">Event Overview</h2>
          <p class="text-slate-600">Real-time analytics and event processing statistics</p>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <div
            v-for="(stat, index) in eventStats"
            :key="index"
            class="bg-white rounded-xl border border-slate-200 p-6 hover:shadow-lg transition-all duration-300 hover:-translate-y-1 animate-slideIn"
            :style="{ animationDelay: `${index * 100}ms` }"
          >
            <div class="flex items-start justify-between mb-4">
              <div class="p-3 rounded-lg" :class="stat.bgColor">
                <component :is="stat.icon" :size="24" :class="stat.iconColor" />
              </div>
            </div>
            <div class="mb-3">
              <p class="text-3xl font-bold text-slate-900">{{ stat.value }}</p>
              <p class="text-sm font-medium text-slate-600">{{ stat.label }}</p>
              <div class="flex items-center gap-1 mt-2">
                <TrendingUp :size="14" class="text-green-600" />
                <span class="text-sm font-medium text-green-600">+{{ stat.change }}%</span>
                <span class="text-sm text-slate-500">vs last hour</span>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Main Dashboard Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Live Event Feed -->
        <div class="lg:col-span-2 bg-white rounded-xl border border-slate-200 shadow-sm">
          <div class="border-b border-slate-200 p-6">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="p-2 bg-blue-50 rounded-lg">
                  <Radio :size="20" class="text-blue-600" />
                </div>
                <div>
                  <h3 class="text-xl font-semibold text-slate-900">Live Event Feed</h3>
                  <p class="text-sm text-slate-600">Real-time event stream from port operations</p>
                </div>
              </div>
              <button class="px-4 py-2 text-sm font-medium text-blue-600 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors flex items-center gap-2">
                <Eye :size="16" />
                View All
              </button>
            </div>
          </div>
          
          <div class="p-6">
            <div class="space-y-4 max-h-[600px] overflow-y-auto">
              <div
                v-for="(event, index) in liveEvents"
                :key="event.id"
                class="p-4 rounded-lg transition-all duration-300 hover:shadow-md group animate-slideIn"
                :class="!event.acknowledged ? 'border-2 border-blue-200 bg-blue-50' : 'border border-slate-200 bg-white'"
                :style="{ animationDelay: `${(index + 4) * 100}ms` }"
              >
                <div class="flex items-start justify-between mb-3">
                  <div class="flex items-center gap-3">
                    <div class="w-10 h-10 rounded-lg bg-blue-50 border-2 border-blue-200 flex items-center justify-center">
                      <component :is="getEventIcon(event.type)" :size="18" class="text-blue-600" />
                    </div>
                    <div>
                      <div class="font-semibold text-slate-900">{{ event.type }}</div>
                      <div class="text-sm text-slate-600">{{ event.container }}</div>
                    </div>
                  </div>
                  <div class="text-right">
                    <div class="flex items-center gap-2 mb-1">
                      <span 
                        class="inline-flex items-center px-2 py-1 text-xs font-semibold rounded-full"
                        :class="getPriorityColor(event.priority)"
                      >
                        {{ event.priority }}
                      </span>
                      <span 
                        class="inline-flex items-center px-2 py-1 text-xs font-semibold rounded-full"
                        :class="getStatusColor(event.status)"
                      >
                        {{ event.status }}
                      </span>
                    </div>
                    <div class="flex items-center gap-1 text-xs text-slate-500">
                      <Clock :size="12" />
                      <span>{{ event.timestamp }}</span>
                    </div>
                  </div>
                </div>
                
                <p class="text-sm text-slate-700 mb-3 leading-relaxed">{{ event.description }}</p>
                
                <div class="flex gap-2">
                  <button
                    v-if="!event.acknowledged"
                    class="px-3 py-1 text-sm bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                  >
                    Acknowledge
                  </button>
                  <button class="px-3 py-1 text-sm border border-slate-300 rounded-lg hover:bg-slate-50 transition-colors">
                    View Details
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Stream Controls & Filters -->
        <div class="space-y-6">
          <!-- Connection Status -->
          <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div class="border-b border-slate-200 p-6">
              <h3 class="text-lg font-semibold text-slate-900">Stream Status</h3>
            </div>
            <div class="p-6 space-y-4">
              <div class="p-4 bg-gradient-to-r from-green-50 to-green-100 rounded-lg border border-green-200">
                <div class="flex items-center gap-2 mb-2">
                  <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
                  <span class="text-sm font-semibold text-green-800">Kafka Stream</span>
                </div>
                <p class="text-lg font-bold text-green-900">Connected</p>
                <p class="text-sm text-green-700 mt-1">Last update: Just now</p>
              </div>

              <div class="grid grid-cols-2 gap-3 text-center">
                <div class="p-3 bg-slate-50 rounded-lg">
                  <p class="text-lg font-bold text-slate-900">45</p>
                  <p class="text-xs text-slate-600">Events/min</p>
                </div>
                <div class="p-3 bg-slate-50 rounded-lg">
                  <p class="text-lg font-bold text-slate-900">2.3s</p>
                  <p class="text-xs text-slate-600">Avg Latency</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Event Filters -->
          <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div class="border-b border-slate-200 p-6">
              <h3 class="text-lg font-semibold text-slate-900">Event Filters</h3>
            </div>
            <div class="p-6 space-y-4">
              <div class="space-y-3">
                <h4 class="font-medium text-slate-700">Event Types</h4>
                <div class="space-y-2">
                  <label class="flex items-center gap-2 text-sm cursor-pointer">
                    <input
                      v-model="filters.containerEvents"
                      type="checkbox"
                      class="rounded text-blue-600 focus:ring-blue-500"
                    />
                    <span class="text-slate-700">Container Events</span>
                  </label>
                  <label class="flex items-center gap-2 text-sm cursor-pointer">
                    <input
                      v-model="filters.shipOperations"
                      type="checkbox"
                      class="rounded text-blue-600 focus:ring-blue-500"
                    />
                    <span class="text-slate-700">Ship Operations</span>
                  </label>
                  <label class="flex items-center gap-2 text-sm cursor-pointer">
                    <input
                      v-model="filters.berthAssignments"
                      type="checkbox"
                      class="rounded text-blue-600 focus:ring-blue-500"
                    />
                    <span class="text-slate-700">Berth Assignments</span>
                  </label>
                  <label class="flex items-center gap-2 text-sm cursor-pointer">
                    <input
                      v-model="filters.systemAlerts"
                      type="checkbox"
                      class="rounded text-blue-600 focus:ring-blue-500"
                    />
                    <span class="text-slate-700">System Alerts</span>
                  </label>
                </div>
              </div>

              <div class="space-y-3">
                <h4 class="font-medium text-slate-700">Priority Level</h4>
                <div class="flex gap-2">
                  <span
                    v-for="priority in ['High', 'Medium', 'Low']"
                    :key="priority"
                    class="px-3 py-1 text-xs rounded-full cursor-pointer transition-colors"
                    :class="priorityFilters[priority.toLowerCase() as keyof typeof priorityFilters] 
                      ? getPriorityColor(priority) 
                      : 'bg-slate-100 text-slate-600 hover:bg-slate-200'"
                    @click="priorityFilters[priority.toLowerCase() as keyof typeof priorityFilters] = !priorityFilters[priority.toLowerCase() as keyof typeof priorityFilters]"
                  >
                    {{ priority }}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
            <div class="border-b border-slate-200 p-6">
              <h3 class="text-lg font-semibold text-slate-900">Quick Actions</h3>
            </div>
            <div class="p-6 space-y-3">
              <button class="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-3 px-4 rounded-lg transition-colors duration-200 flex items-center justify-center gap-2">
                <Download :size="16" />
                Export Events
              </button>
              <button class="w-full border border-slate-300 rounded-lg hover:bg-slate-50 font-medium py-3 px-4 transition-colors flex items-center justify-center gap-2">
                <Trash2 :size="16" />
                Clear Acknowledged
              </button>
              <button class="w-full border border-slate-300 rounded-lg hover:bg-slate-50 font-medium py-3 px-4 transition-colors flex items-center justify-center gap-2">
                <Settings :size="16" />
                Stream Settings
              </button>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { 
  Activity, 
  Radio, 
  Eye, 
  Clock, 
  TrendingUp, 
  Zap, 
  Download, 
  Trash2, 
  Settings,
  Container,
  Ship,
  Anchor,
  AlertTriangle,
  CheckCircle
} from 'lucide-vue-next';

interface Event {
  id: string;
  timestamp: string;
  type: string;
  container: string;
  description: string;
  priority: string;
  status: string;
  acknowledged: boolean;
}

interface EventStat {
  label: string;
  value: string;
  color: string;
  bgColor: string;
  iconColor: string;
  icon: any;
  change: string;
}

interface Filters {
  containerEvents: boolean;
  shipOperations: boolean;
  berthAssignments: boolean;
  systemAlerts: boolean;
}

interface PriorityFilters {
  high: boolean;
  medium: boolean;
  low: boolean;
}

const liveEvents = ref<Event[]>([
  {
    id: "EVT-001",
    timestamp: "14:32:15",
    type: "Container Arrival",
    container: "CTR-2024-156",
    description: "Container arrived at Chennai Port via MV Ocean Star. Customs inspection required.",
    priority: "High",
    status: "New",
    acknowledged: false
  },
  {
    id: "EVT-002", 
    timestamp: "14:28:42",
    type: "Inspection Complete",
    container: "CTR-2024-143",
    description: "Customs inspection completed successfully. Container cleared for loading operations.",
    priority: "Medium",
    status: "Acknowledged",
    acknowledged: true
  },
  {
    id: "EVT-003",
    timestamp: "14:25:18", 
    type: "Loading Started",
    container: "CTR-2024-139",
    description: "Loading operation initiated on MV Blue Wave. Estimated completion in 2 hours.",
    priority: "High",
    status: "In Progress",
    acknowledged: true
  },
  {
    id: "EVT-004",
    timestamp: "14:20:05",
    type: "Berth Assignment",
    container: "CTR-2024-156",
    description: "Container assigned to Berth B-07 for temporary storage awaiting ship departure.",
    priority: "Medium", 
    status: "Completed",
    acknowledged: true
  },
  {
    id: "EVT-005",
    timestamp: "14:15:30",
    type: "Ship Departure",
    container: "CTR-2024-098",
    description: "MV Atlantic departed successfully with 45 containers bound for Mumbai Port.",
    priority: "Low",
    status: "Completed",
    acknowledged: true
  }
]);

const eventStats = ref<EventStat[]>([
  { 
    label: "Events Today", 
    value: "342", 
    color: "text-blue-600",
    bgColor: "bg-blue-50",
    iconColor: "text-blue-600",
    icon: Activity,
    change: "12"
  },
  { 
    label: "Pending", 
    value: "23", 
    color: "text-orange-600",
    bgColor: "bg-orange-50",
    iconColor: "text-orange-600",
    icon: Clock,
    change: "8"
  },
  { 
    label: "Acknowledged", 
    value: "319", 
    color: "text-green-600",
    bgColor: "bg-green-50",
    iconColor: "text-green-600",
    icon: CheckCircle,
    change: "15"
  },
  { 
    label: "High Priority", 
    value: "8", 
    color: "text-red-600",
    bgColor: "bg-red-50",
    iconColor: "text-red-600",
    icon: AlertTriangle,
    change: "4"
  }
]);

const filters = ref<Filters>({
  containerEvents: true,
  shipOperations: true,
  berthAssignments: true,
  systemAlerts: false
});

const priorityFilters = ref<PriorityFilters>({
  high: true,
  medium: true,
  low: true
});

const getPriorityColor = (priority: string): string => {
  const priorityColors = {
    "High": "bg-red-100 text-red-800 border-red-200",
    "Medium": "bg-orange-100 text-orange-800 border-orange-200",
    "Low": "bg-green-100 text-green-800 border-green-200",
  };
  return priorityColors[priority as keyof typeof priorityColors] || "bg-slate-100 text-slate-800 border-slate-200";
};

const getStatusColor = (status: string): string => {
  const statusColors = {
    "New": "bg-blue-100 text-blue-800 border-blue-200",
    "In Progress": "bg-purple-100 text-purple-800 border-purple-200",
    "Acknowledged": "bg-yellow-100 text-yellow-800 border-yellow-200",
    "Completed": "bg-green-100 text-green-800 border-green-200",
  };
  return statusColors[status as keyof typeof statusColors] || "bg-slate-100 text-slate-800 border-slate-200";
};

const getEventIcon = (type: string) => {
  const iconMap = {
    "Container Arrival": Container,
    "Inspection Complete": CheckCircle,
    "Loading Started": Activity,
    "Berth Assignment": Anchor,
    "Ship Departure": Ship,
  };
  return iconMap[type as keyof typeof iconMap] || Activity;
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
  
  .grid-cols-1.lg\:grid-cols-3 {
    grid-template-columns: repeat(1, minmax(0, 1fr));
  }
  
  .lg\:col-span-2 {
    grid-column: span 1;
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
  
  .text-lg {
    font-size: 1rem;
    line-height: 1.5rem;
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
  
  .space-y-6 > :not([hidden]) ~ :not([hidden]) {
    --tw-space-y-reverse: 0;
    margin-top: calc(1rem * calc(1 - var(--tw-space-y-reverse)));
    margin-bottom: calc(1rem * var(--tw-space-y-reverse));
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
button:focus-visible,
input:focus-visible {
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

/* Event feed specific styles */
.max-h-\[600px\] {
  max-height: 600px;
}

/* Custom checkbox styles */
input[type="checkbox"] {
  accent-color: #3b82f6;
}

/* Hover effects for interactive elements */
.cursor-pointer:hover {
  transform: scale(1.02);
}

/* Loading state placeholder */
.loading-shimmer {
  background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
  background-size: 200px 100%;
  animation: shimmer 1.5s infinite;
}

@keyframes shimmer {
  0% {
    background-position: -200px 0;
  }
  100% {
    background-position: calc(200px + 100%) 0;
  }
}
</style>