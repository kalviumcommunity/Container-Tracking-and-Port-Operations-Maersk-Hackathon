<template>
  <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
    <!-- Header -->
    <div class="border-b border-slate-200 p-6">
      <div class="flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="p-2 bg-blue-100 rounded-lg">
            <Container :size="20" class="text-blue-700" />
          </div>
          <div>
            <h3 class="text-xl font-semibold text-slate-900">Container Analytics</h3>
            <p class="text-sm text-slate-600">Port-specific container tracking and operations</p>
          </div>
        </div>
        <div class="flex items-center gap-2 text-sm text-slate-600">
          <div class="w-2 h-2 bg-green-400 rounded-full animate-pulse"></div>
          <span>Live Data</span>
          <span v-if="analyticsData && analyticsData.totalContainers" class="text-xs text-slate-500">
            • {{ analyticsData.totalContainers || totalContainers }} total
          </span>
        </div>
      </div>
    </div>

    <!-- Content -->
    <div class="p-6">
      <!-- Loading State -->
      <div v-if="loading" class="space-y-4">
        <div class="h-64 bg-slate-100 rounded-lg animate-pulse flex items-center justify-center">
          <div class="text-slate-500">Loading container analytics...</div>
        </div>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="text-center py-8">
        <AlertCircle :size="48" class="mx-auto text-red-500 mb-4" />
        <h3 class="text-lg font-semibold text-red-900 mb-2">Failed to Load Container Data</h3>
        <p class="text-red-700 mb-4">{{ error }}</p>
        <button
          @click="$emit('retry')"
          class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
        >
          Retry
        </button>
      </div>

      <!-- Empty State -->
      <div v-else-if="!containers || containers.length === 0" class="text-center py-8">
        <Container :size="48" class="mx-auto text-slate-400 mb-4" />
        <h3 class="text-lg font-semibold text-slate-700 mb-2">No Container Data Available</h3>
        <p class="text-slate-500 mb-4">Loading containers from backend or no containers found for the selected port</p>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Demo Charts with No Data State -->
          <div class="border border-slate-200 rounded-lg p-4">
            <h4 class="text-sm font-semibold text-slate-600 mb-4">Container Status (No Data)</h4>
            <div class="h-48 flex items-center justify-center bg-slate-50 rounded">
              <p class="text-slate-400">Waiting for container data...</p>
            </div>
          </div>
          <div class="border border-slate-200 rounded-lg p-4">
            <h4 class="text-sm font-semibold text-slate-600 mb-4">Container Types (No Data)</h4>
            <div class="h-48 flex items-center justify-center bg-slate-50 rounded">
              <p class="text-slate-400">Waiting for container data...</p>
            </div>
          </div>
          <div class="border border-slate-200 rounded-lg p-4">
            <h4 class="text-sm font-semibold text-slate-600 mb-4">Cargo Types (No Data)</h4>
            <div class="h-48 flex items-center justify-center bg-slate-50 rounded">
              <p class="text-slate-400">Waiting for container data...</p>
            </div>
          </div>
          <div class="border border-slate-200 rounded-lg p-4">
            <h4 class="text-sm font-semibold text-slate-600 mb-4">Locations (No Data)</h4>
            <div class="h-48 flex items-center justify-center bg-slate-50 rounded">
              <p class="text-slate-400">Waiting for container data...</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Charts Grid: 2x2 Layout -->
      <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-6">
        
        <!-- Container Status Distribution (Bar Chart) -->
        <div class="border border-slate-200 rounded-lg p-4">
          <h4 class="text-sm font-semibold text-slate-900 mb-4 flex items-center gap-2">
            <div class="w-3 h-3 bg-blue-500 rounded"></div>
            Container Status
          </h4>
          <div class="relative h-48">
            <div class="flex items-end justify-center h-full space-x-3 pb-8">
              <div 
                v-for="(item, index) in statusChartData" 
                :key="`status-${item.label}`"
                class="flex flex-col items-center group animate-slideIn"
                :style="{ animationDelay: `${index * 150}ms` }"
              >
                <div class="relative mb-2">
                  <div 
                    class="w-12 rounded-t-lg transition-all duration-500 group-hover:opacity-80 cursor-pointer"
                    :class="item.colorClass"
                    :style="{ height: `${item.height}px` }"
                    :title="`${item.label}: ${item.count} containers`"
                  >
                    <div class="absolute -top-6 left-1/2 -translate-x-1/2 text-xs font-semibold text-slate-700">
                      {{ item.count }}
                    </div>
                  </div>
                </div>
                <div class="text-xs font-medium text-slate-700 text-center w-16 truncate">
                  {{ item.label }}
                </div>
              </div>
            </div>
          </div>
          <div class="grid grid-cols-2 gap-2 pt-3 border-t border-slate-200">
            <div v-for="item in statusChartData" :key="`status-legend-${item.label}`" class="flex items-center gap-2">
              <span class="w-2.5 h-2.5 rounded-full" :style="{ backgroundColor: item.color }"></span>
              <span class="text-xs text-slate-600 truncate">{{ item.label }} ({{ item.count }})</span>
            </div>
          </div>
        </div>

        <!-- Container Types (Donut Chart) -->
        <div class="border border-slate-200 rounded-lg p-4">
          <h4 class="text-sm font-semibold text-slate-900 mb-4 flex items-center gap-2">
            <div class="w-3 h-3 bg-green-500 rounded"></div>
            Container Types
          </h4>
          <div class="relative h-48 flex items-center justify-center">
            <div class="relative">
              <div 
                class="w-32 h-32 rounded-full shadow-inner" 
                :style="{ background: typeDonut }"
              >
              </div>
              <div class="absolute inset-0 flex items-center justify-center">
                <div class="bg-white w-20 h-20 rounded-full flex items-center justify-center shadow-sm">
                  <div class="text-center">
                    <div class="text-lg font-bold text-slate-900">{{ totalContainers }}</div>
                    <div class="text-xs text-slate-500">Total</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="grid grid-cols-2 gap-2 pt-3 border-t border-slate-200">
            <div v-for="item in typeChartData" :key="`type-legend-${item.label}`" class="flex items-center gap-2">
              <span class="w-2.5 h-2.5 rounded-full" :style="{ backgroundColor: item.color }"></span>
              <span class="text-xs text-slate-600 truncate">{{ item.label }} ({{ item.count }})</span>
            </div>
          </div>
        </div>

        <!-- Cargo Types Distribution (Bar Chart) -->
        <div class="border border-slate-200 rounded-lg p-4">
          <h4 class="text-sm font-semibold text-slate-900 mb-4 flex items-center gap-2">
            <div class="w-3 h-3 bg-purple-500 rounded"></div>
            Cargo Types
          </h4>
          <div class="relative h-48">
            <div class="flex items-end justify-center h-full space-x-3 pb-8">
              <div 
                v-for="(item, index) in cargoChartData" 
                :key="`cargo-${item.label}`"
                class="flex flex-col items-center group animate-slideIn"
                :style="{ animationDelay: `${index * 150}ms` }"
              >
                <div class="relative mb-2">
                  <div 
                    class="w-12 rounded-t-lg transition-all duration-500 group-hover:opacity-80 cursor-pointer"
                    :style="{ backgroundColor: item.color, height: `${item.height}px` }"
                    :title="`${item.label}: ${item.count} containers`"
                  >
                    <div class="absolute -top-6 left-1/2 -translate-x-1/2 text-xs font-semibold text-slate-700">
                      {{ item.count }}
                    </div>
                  </div>
                </div>
                <div class="text-xs font-medium text-slate-700 text-center w-16 truncate">
                  {{ item.label }}
                </div>
              </div>
            </div>
          </div>
          <div class="grid grid-cols-2 gap-2 pt-3 border-t border-slate-200">
            <div v-for="item in cargoChartData" :key="`cargo-legend-${item.label}`" class="flex items-center gap-2">
              <span class="w-2.5 h-2.5 rounded-full" :style="{ backgroundColor: item.color }"></span>
              <span class="text-xs text-slate-600 truncate">{{ item.label }} ({{ item.count }})</span>
            </div>
          </div>
        </div>

        <!-- Location Distribution (Donut Chart) -->
        <div class="border border-slate-200 rounded-lg p-4">
          <h4 class="text-sm font-semibold text-slate-900 mb-4 flex items-center gap-2">
            <div class="w-3 h-3 bg-orange-500 rounded"></div>
            Current Locations
          </h4>
          <div class="relative h-48 flex items-center justify-center">
            <div class="relative">
              <div 
                class="w-32 h-32 rounded-full shadow-inner" 
                :style="{ background: locationDonut }"
              >
              </div>
              <div class="absolute inset-0 flex items-center justify-center">
                <div class="bg-white w-20 h-20 rounded-full flex items-center justify-center shadow-sm">
                  <div class="text-center">
                    <div class="text-lg font-bold text-slate-900">{{ uniqueLocations }}</div>
                    <div class="text-xs text-slate-500">Locations</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="grid grid-cols-1 gap-2 pt-3 border-t border-slate-200">
            <div v-for="item in locationChartData" :key="`location-legend-${item.label}`" class="flex items-center gap-2">
              <span class="w-2.5 h-2.5 rounded-full" :style="{ backgroundColor: item.color }"></span>
              <span class="text-xs text-slate-600 truncate">{{ item.label }} ({{ item.count }})</span>
            </div>
          </div>
        </div>

      </div>

      <!-- Additional Analytics Section -->
      <div v-if="analyticsData && (analyticsData.containersInTransit || analyticsData.containersAtPort || analyticsData.averageTurnaroundTime)" class="mt-6 grid grid-cols-1 md:grid-cols-3 gap-4 p-4 bg-slate-50 rounded-lg">
        <div class="text-center">
          <div class="text-2xl font-bold text-blue-600">{{ analyticsData.containersInTransit || 0 }}</div>
          <div class="text-xs text-slate-600">In Transit</div>
        </div>
        <div class="text-center">
          <div class="text-2xl font-bold text-green-600">{{ analyticsData.containersAtPort || 0 }}</div>
          <div class="text-xs text-slate-600">At Port</div>
        </div>
        <div class="text-center">
          <div class="text-2xl font-bold text-orange-600">{{ Math.round(analyticsData.averageTurnaroundTime || 0) }}h</div>
          <div class="text-xs text-slate-600">Avg Processing</div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, watch } from 'vue';
import { Container, AlertCircle } from 'lucide-vue-next';

// Props interface
interface Props {
  containers: Array<{
    containerId: string;
    status: string;
    type?: string;
    cargoType?: string;
    currentLocation?: string;
    portId?: number;
    shipId?: number;
    weight?: number;
    destination?: string;
    origin?: string;
    createdAt?: string;
    updatedAt?: string;
  }>;
  analyticsData?: {
    totalContainers?: number;
    containersInTransit?: number;
    containersAtPort?: number;
    averageTurnaroundTime?: number;
  } | null;
  loading?: boolean;
  error?: string | null;
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  error: null,
  containers: () => []
});

defineEmits<{
  retry: [];
}>();

// Color palettes
const statusColors: Record<string, { class: string; hex: string }> = {
  'Loading': { class: 'bg-blue-500', hex: '#3b82f6' },
  'Loaded': { class: 'bg-green-500', hex: '#10b981' },
  'Unloading': { class: 'bg-orange-500', hex: '#f97316' },
  'In Transit': { class: 'bg-purple-500', hex: '#a855f7' },
  'Delivered': { class: 'bg-emerald-500', hex: '#059669' },
  'Pending': { class: 'bg-yellow-500', hex: '#eab308' },
  'Other': { class: 'bg-gray-400', hex: '#9ca3af' }
};

const typeColors: Record<string, string> = {
  'Dry': '#2563eb',
  'Refrigerated': '#059669',
  'Tank': '#dc2626',
  'Open Top': '#d97706',
  'Flat Rack': '#7c3aed',
  'Other': '#6b7280'
};

const cargoColors: Record<string, string> = {
  'Electronics': '#3b82f6',
  'Food': '#10b981',
  'Machinery': '#f59e0b',
  'Textiles': '#ef4444',
  'Chemicals': '#8b5cf6',
  'Automotive': '#06b6d4',
  'Other': '#6b7280'
};

const locationColors = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', '#06b6d4'];

// Chart data computations with enhanced reactivity
const statusChartData = computed(() => {
  // Force reactivity by referencing the reactive key
  reactiveKey.value;
  
  const statusCounts: Record<string, number> = {};
  
  // Use only real data from backend
  const containersToProcess = props.containers || [];
  
  // Enhanced logging for debugging port changes
  console.log('📦 ContainerActivity2: Processing containers for chart:', {
    containerCount: containersToProcess.length,
    isRealData: props.containers.length > 0,
    analyticsData: !!props.analyticsData,
    firstContainer: containersToProcess[0],
    containers: containersToProcess
  });
  
  containersToProcess.forEach(container => {
    const status = container.status || 'Other';
    statusCounts[status] = (statusCounts[status] || 0) + 1;
  });

  const maxCount = Math.max(...Object.values(statusCounts), 1);
  const minHeight = 20;
  const maxHeight = 120;

  return Object.entries(statusCounts).map(([status, count]) => ({
    label: status,
    count,
    height: Math.max((count / maxCount) * maxHeight, minHeight),
    colorClass: statusColors[status]?.class || 'bg-gray-400',
    color: statusColors[status]?.hex || '#9ca3af'
  }));
});

const typeChartData = computed(() => {
  // Force reactivity by referencing the reactive key
  reactiveKey.value;
  
  const typeCounts: Record<string, number> = {};
  
  // Use only real data from backend
  const containersToProcess = props.containers || [];
  
  containersToProcess.forEach(container => {
    const type = container.type || 'Other';
    typeCounts[type] = (typeCounts[type] || 0) + 1;
  });

  return Object.entries(typeCounts).map(([type, count]) => ({
    label: type,
    count,
    color: typeColors[type] || typeColors['Other']
  }));
});

const cargoChartData = computed(() => {
  // Force reactivity by referencing the reactive key
  reactiveKey.value;
  
  const cargoCounts: Record<string, number> = {};
  
  // Use only real data from backend
  const containersToProcess = props.containers || [];
  
  containersToProcess.forEach(container => {
    const cargo = container.cargoType || 'Other';
    cargoCounts[cargo] = (cargoCounts[cargo] || 0) + 1;
  });

  const maxCount = Math.max(...Object.values(cargoCounts), 1);
  const minHeight = 20;
  const maxHeight = 120;

  return Object.entries(cargoCounts).map(([cargo, count]) => ({
    label: cargo,
    count,
    height: Math.max((count / maxCount) * maxHeight, minHeight),
    color: cargoColors[cargo] || cargoColors['Other']
  }));
});

const locationChartData = computed(() => {
  // Force reactivity by referencing the reactive key
  reactiveKey.value;
  
  const locationCounts: Record<string, number> = {};
  
  // Use only real data from backend
  const containersToProcess = props.containers || [];
  
  containersToProcess.forEach(container => {
    const location = container.currentLocation || 'Unknown';
    locationCounts[location] = (locationCounts[location] || 0) + 1;
  });

  return Object.entries(locationCounts).map(([location, count], index) => ({
    label: location,
    count,
    color: locationColors[index % locationColors.length]
  }));
});

// Metrics
const totalContainers = computed(() => props.containers.length > 0 ? props.containers.length : 5);

// Force reactivity by creating a computed that depends on containers length and analytics data
const reactiveKey = computed(() => `${props.containers.length}-${props.analyticsData?.totalContainers || 0}`);

// Watch for container changes to help debug port selection issues
watch(() => props.containers, (newContainers, oldContainers) => {
  console.log('🔄 ContainerActivity2: Containers prop changed:', {
    oldCount: oldContainers?.length || 0,
    newCount: newContainers?.length || 0,
    isArray: Array.isArray(newContainers),
    containers: newContainers
  });
}, { deep: true, immediate: true });

// Watch the reactive key to trigger chart updates
watch(reactiveKey, (newKey) => {
  console.log('🔥 ContainerActivity2: Reactive key changed:', newKey);
});
const uniqueLocations = computed(() => locationChartData.value.length);

// Donut chart gradients
const typeDonut = computed(() => {
  if (typeChartData.value.length === 0) return '#e5e7eb';
  
  const total = typeChartData.value.reduce((sum, item) => sum + item.count, 0);
  let currentPercent = 0;
  
  const segments = typeChartData.value.map(item => {
    const startPercent = currentPercent;
    const endPercent = currentPercent + (item.count / total) * 100;
    currentPercent = endPercent;
    return `${item.color} ${startPercent}% ${endPercent}%`;
  });
  
  return `conic-gradient(${segments.join(', ')})`;
});

const locationDonut = computed(() => {
  if (locationChartData.value.length === 0) return '#e5e7eb';
  
  const total = locationChartData.value.reduce((sum, item) => sum + item.count, 0);
  let currentPercent = 0;
  
  const segments = locationChartData.value.map(item => {
    const startPercent = currentPercent;
    const endPercent = currentPercent + (item.count / total) * 100;
    currentPercent = endPercent;
    return `${item.color} ${startPercent}% ${endPercent}%`;
  });
  
  return `conic-gradient(${segments.join(', ')})`;
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

.animate-slideIn {
  animation: slideIn 0.6s ease-out forwards;
  opacity: 0;
}

.animate-pulse {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: .5;
  }
}
</style>