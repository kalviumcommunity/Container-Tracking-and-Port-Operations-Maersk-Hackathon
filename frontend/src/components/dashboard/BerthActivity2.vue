<template>
  <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
    <!-- Header -->
    <div class="border-b border-slate-200 p-6">
      <div class="flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="p-2 bg-emerald-100 rounded-lg">
            <Anchor :size="20" class="text-emerald-700" />
          </div>
          <div>
            <h3 class="text-xl font-semibold text-slate-900">Berth Analytics</h3>
            <p class="text-sm text-slate-600">Port-specific berth utilization and operations</p>
          </div>
        </div>
        <div class="flex items-center gap-2 text-sm text-slate-600">
          <div class="w-2 h-2 bg-green-400 rounded-full animate-pulse"></div>
          <span>Live Data</span>
          <span v-if="analyticsData && typeof analyticsData.berthUtilizationRate === 'number'" class="text-xs text-slate-500">
            • {{ Math.round(analyticsData.berthUtilizationRate || 0) }}% utilization
          </span>
        </div>
      </div>
    </div>

    <!-- Content -->
    <div class="p-6">
      <!-- Loading State -->
      <div v-if="loading" class="space-y-4">
        <div class="h-64 bg-slate-100 rounded-lg animate-pulse flex items-center justify-center">
          <div class="text-slate-500">Loading berth analytics...</div>
        </div>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="text-center py-8">
        <AlertCircle :size="48" class="mx-auto text-red-500 mb-4" />
        <h3 class="text-lg font-semibold text-red-900 mb-2">Failed to Load Berth Data</h3>
        <p class="text-red-700 mb-4">{{ error }}</p>
        <button
          @click="$emit('retry')"
          class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
        >
          Retry
        </button>
      </div>

      <!-- Empty State -->
      <div v-else-if="!berths || berths.length === 0" class="text-center py-8">
        <Anchor :size="48" class="mx-auto text-slate-400 mb-4" />
        <h3 class="text-lg font-semibold text-slate-700 mb-2">No Berth Data Available</h3>
        <p class="text-slate-500 mb-4">Loading berths from backend or no berths found for the selected port</p>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Demo Charts with No Data State -->
          <div class="border border-slate-200 rounded-lg p-4">
            <h4 class="text-sm font-semibold text-slate-600 mb-4">Berth Status (No Data)</h4>
            <div class="h-48 flex items-center justify-center bg-slate-50 rounded">
              <p class="text-slate-400">Waiting for berth data...</p>
            </div>
          </div>
          <div class="border border-slate-200 rounded-lg p-4">
            <h4 class="text-sm font-semibold text-slate-600 mb-4">Capacity Utilization (No Data)</h4>
            <div class="h-48 flex items-center justify-center bg-slate-50 rounded">
              <p class="text-slate-400">Waiting for berth data...</p>
            </div>
          </div>
          <div class="border border-slate-200 rounded-lg p-4">
            <h4 class="text-sm font-semibold text-slate-600 mb-4">Berth Types (No Data)</h4>
            <div class="h-48 flex items-center justify-center bg-slate-50 rounded">
              <p class="text-slate-400">Waiting for berth data...</p>
            </div>
          </div>
          <div class="border border-slate-200 rounded-lg p-4">
            <h4 class="text-sm font-semibold text-slate-600 mb-4">Port Distribution (No Data)</h4>
            <div class="h-48 flex items-center justify-center bg-slate-50 rounded">
              <p class="text-slate-400">Waiting for berth data...</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Charts Grid: 2x2 Layout -->
      <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-6">
        
        <!-- Berth Status Distribution (Bar Chart) -->
        <div class="border border-slate-200 rounded-lg p-4">
          <h4 class="text-sm font-semibold text-slate-900 mb-4 flex items-center gap-2">
            <div class="w-3 h-3 bg-blue-500 rounded"></div>
            Berth Status
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
                    :title="`${item.label}: ${item.count} berths`"
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

        <!-- Berth Capacity Utilization (Donut Chart) -->
        <div class="border border-slate-200 rounded-lg p-4">
          <h4 class="text-sm font-semibold text-slate-900 mb-4 flex items-center gap-2">
            <div class="w-3 h-3 bg-green-500 rounded"></div>
            Capacity Utilization
          </h4>
          <div class="relative h-48 flex items-center justify-center">
            <div class="relative">
              <div 
                class="w-32 h-32 rounded-full shadow-inner" 
                :style="{ background: utilizationDonut }"
              >
              </div>
              <div class="absolute inset-0 flex items-center justify-center">
                <div class="bg-white w-20 h-20 rounded-full flex items-center justify-center shadow-sm">
                  <div class="text-center">
                    <div class="text-lg font-bold text-slate-900">{{ utilizationPercentage }}%</div>
                    <div class="text-xs text-slate-500">Used</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="grid grid-cols-2 gap-2 pt-3 border-t border-slate-200">
            <div class="flex items-center gap-2">
              <span class="w-2.5 h-2.5 rounded-full bg-blue-500"></span>
              <span class="text-xs text-slate-600">Used ({{ usedBerths }})</span>
            </div>
            <div class="flex items-center gap-2">
              <span class="w-2.5 h-2.5 rounded-full bg-green-500"></span>
              <span class="text-xs text-slate-600">Available ({{ availableBerths }})</span>
            </div>
          </div>
        </div>

        <!-- Berth Types Distribution (Bar Chart) -->
        <div class="border border-slate-200 rounded-lg p-4">
          <h4 class="text-sm font-semibold text-slate-900 mb-4 flex items-center gap-2">
            <div class="w-3 h-3 bg-purple-500 rounded"></div>
            Berth Types
          </h4>
          <div class="relative h-48">
            <div class="flex items-end justify-center h-full space-x-3 pb-8">
              <div 
                v-for="(item, index) in typeChartData" 
                :key="`type-${item.label}`"
                class="flex flex-col items-center group animate-slideIn"
                :style="{ animationDelay: `${index * 150}ms` }"
              >
                <div class="relative mb-2">
                  <div 
                    class="w-12 rounded-t-lg transition-all duration-500 group-hover:opacity-80 cursor-pointer"
                    :style="{ backgroundColor: item.color, height: `${item.height}px` }"
                    :title="`${item.label}: ${item.count} berths`"
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
            <div v-for="item in typeChartData" :key="`type-legend-${item.label}`" class="flex items-center gap-2">
              <span class="w-2.5 h-2.5 rounded-full" :style="{ backgroundColor: item.color }"></span>
              <span class="text-xs text-slate-600 truncate">{{ item.label }} ({{ item.count }})</span>
            </div>
          </div>
        </div>

        <!-- Port Distribution (Donut Chart) -->
        <div class="border border-slate-200 rounded-lg p-4">
          <h4 class="text-sm font-semibold text-slate-900 mb-4 flex items-center gap-2">
            <div class="w-3 h-3 bg-orange-500 rounded"></div>
            Port Distribution
          </h4>
          <div class="relative h-48 flex items-center justify-center">
            <div class="relative">
              <div 
                class="w-32 h-32 rounded-full shadow-inner" 
                :style="{ background: portDonut }"
              >
              </div>
              <div class="absolute inset-0 flex items-center justify-center">
                <div class="bg-white w-20 h-20 rounded-full flex items-center justify-center shadow-sm">
                  <div class="text-center">
                    <div class="text-lg font-bold text-slate-900">{{ totalBerths }}</div>
                    <div class="text-xs text-slate-500">Total</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="grid grid-cols-1 gap-2 pt-3 border-t border-slate-200">
            <div v-for="item in portChartData" :key="`port-legend-${item.label}`" class="flex items-center gap-2">
              <span class="w-2.5 h-2.5 rounded-full" :style="{ backgroundColor: item.color }"></span>
              <span class="text-xs text-slate-600 truncate">{{ item.label }} ({{ item.count }})</span>
            </div>
          </div>
        </div>

      </div>

      <!-- Additional Analytics Section -->
      <div v-if="analyticsData || (berthAssignments && berthAssignments.length)" class="mt-6 grid grid-cols-1 md:grid-cols-4 gap-4 p-4 bg-slate-50 rounded-lg">
        <div class="text-center">
          <div class="text-2xl font-bold text-blue-600">{{ (analyticsData && analyticsData.availableBerths) || availableBerths }}</div>
          <div class="text-xs text-slate-600">Available</div>
        </div>
        <div class="text-center">
          <div class="text-2xl font-bold text-purple-600">{{ usedBerths }}</div>
          <div class="text-xs text-slate-600">In Use</div>
        </div>
        <div class="text-center">
          <div class="text-2xl font-bold text-green-600">{{ (berthAssignments && berthAssignments.length) || 0 }}</div>
          <div class="text-xs text-slate-600">Assignments</div>
        </div>
        <div class="text-center">
          <div class="text-2xl font-bold text-orange-600">{{ Math.round((analyticsData && analyticsData.averageTurnaroundTime) || 0) }}h</div>
          <div class="text-xs text-slate-600">Avg Turnaround</div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { Anchor, AlertCircle } from 'lucide-vue-next';

// Props interface
interface Props {
  berths: Array<{
    berthId: number;
    name: string;
    status: string;
    type?: string;
    capacity?: number;
    currentLoad?: number;
    portId?: number;
    portName?: string;
    hourlyRate?: number;
    maxLength?: number;
    maxWidth?: number;
    maxDepth?: number;
  }>;
  berthAssignments?: Array<{
    assignmentId: number;
    berthId: number;
    shipId: number;
    assignedAt: string;
    expectedDeparture?: string;
    status: string;
  }>;
  analyticsData?: {
    availableBerths?: number;
    berthUtilizationRate?: number;
    averageTurnaroundTime?: number;
  } | null;
  loading?: boolean;
  error?: string | null;
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  error: null,
  berths: () => [],
  berthAssignments: () => []
});

defineEmits<{
  retry: [];
}>();

// Color palettes
const statusColors: Record<string, { class: string; hex: string }> = {
  'Available': { class: 'bg-green-500', hex: '#10b981' },
  'Occupied': { class: 'bg-blue-500', hex: '#3b82f6' },
  'Under Maintenance': { class: 'bg-red-500', hex: '#ef4444' },
  'Reserved': { class: 'bg-yellow-500', hex: '#eab308' },
  'Full': { class: 'bg-purple-500', hex: '#a855f7' },
  'Partially Occupied': { class: 'bg-orange-500', hex: '#f97316' },
  'Inactive': { class: 'bg-gray-400', hex: '#9ca3af' }
};

const typeColors: Record<string, string> = {
  'Container': '#2563eb',
  'Bulk': '#059669',
  'General': '#d97706',
  'Oil': '#dc2626',
  'LNG': '#7c3aed',
  'Other': '#6b7280'
};

const portColors = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', '#06b6d4'];

// Chart data computations with enhanced reactivity
const statusChartData = computed(() => {
  const statusCounts: Record<string, number> = {};
  
  // Use real data if available, otherwise use demo data
  const berthsToProcess = props.berths.length > 0 ? props.berths : [
    { berthId: 1, name: 'Berth A1', status: 'Available', type: 'Container', portName: 'Port A' },
    { berthId: 2, name: 'Berth A2', status: 'Occupied', type: 'Container', portName: 'Port A' },
    { berthId: 3, name: 'Berth B1', status: 'Under Maintenance', type: 'Bulk', portName: 'Port B' },
    { berthId: 4, name: 'Berth B2', status: 'Available', type: 'General', portName: 'Port B' },
    { berthId: 5, name: 'Berth C1', status: 'Occupied', type: 'Oil', portName: 'Port C' }
  ];
  
  // Enhanced logging for debugging port changes
  console.log('⚓ BerthActivity2: Processing berths for chart:', {
    berthCount: berthsToProcess.length,
    assignmentCount: props.berthAssignments?.length || 0,
    isRealData: props.berths.length > 0,
    analyticsData: !!props.analyticsData
  });
  
  berthsToProcess.forEach(berth => {
    const status = berth.status || 'Unknown';
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
  const typeCounts: Record<string, number> = {};
  
  // Use real data if available, otherwise use demo data
  const berthsToProcess = props.berths.length > 0 ? props.berths : [
    { berthId: 1, name: 'Berth A1', status: 'Available', type: 'Container', portName: 'Port A' },
    { berthId: 2, name: 'Berth A2', status: 'Occupied', type: 'Container', portName: 'Port A' },
    { berthId: 3, name: 'Berth B1', status: 'Under Maintenance', type: 'Bulk', portName: 'Port B' },
    { berthId: 4, name: 'Berth B2', status: 'Available', type: 'General', portName: 'Port B' },
    { berthId: 5, name: 'Berth C1', status: 'Occupied', type: 'Oil', portName: 'Port C' }
  ];
  
  berthsToProcess.forEach(berth => {
    const type = berth.type || 'Other';
    typeCounts[type] = (typeCounts[type] || 0) + 1;
  });

  const maxCount = Math.max(...Object.values(typeCounts), 1);
  const minHeight = 20;
  const maxHeight = 120;

  return Object.entries(typeCounts).map(([type, count]) => ({
    label: type,
    count,
    height: Math.max((count / maxCount) * maxHeight, minHeight),
    color: typeColors[type] || typeColors['Other']
  }));
});

const portChartData = computed(() => {
  const portCounts: Record<string, number> = {};
  
  // Use real data if available, otherwise use demo data
  const berthsToProcess = props.berths.length > 0 ? props.berths : [
    { berthId: 1, name: 'Berth A1', status: 'Available', type: 'Container', portName: 'Port A' },
    { berthId: 2, name: 'Berth A2', status: 'Occupied', type: 'Container', portName: 'Port A' },
    { berthId: 3, name: 'Berth B1', status: 'Under Maintenance', type: 'Bulk', portName: 'Port B' },
    { berthId: 4, name: 'Berth B2', status: 'Available', type: 'General', portName: 'Port B' },
    { berthId: 5, name: 'Berth C1', status: 'Occupied', type: 'Oil', portName: 'Port C' }
  ];
  
  berthsToProcess.forEach(berth => {
    const port = berth.portName || 'Unknown Port';
    portCounts[port] = (portCounts[port] || 0) + 1;
  });

  return Object.entries(portCounts).map(([port, count], index) => ({
    label: port,
    count,
    color: portColors[index % portColors.length]
  }));
});

// Utilization metrics
const totalBerths = computed(() => props.berths.length);
const usedBerths = computed(() => 
  props.berths.filter(b => b.status !== 'Available').length
);
const availableBerths = computed(() => totalBerths.value - usedBerths.value);
const utilizationPercentage = computed(() => 
  totalBerths.value > 0 ? Math.round((usedBerths.value / totalBerths.value) * 100) : 0
);

// Donut chart gradients
const utilizationDonut = computed(() => {
  const usedPercent = utilizationPercentage.value;
  const availablePercent = 100 - usedPercent;
  
  if (totalBerths.value === 0) return '#e5e7eb';
  
  return `conic-gradient(#3b82f6 0% ${usedPercent}%, #10b981 ${usedPercent}% 100%)`;
});

const portDonut = computed(() => {
  if (portChartData.value.length === 0) return '#e5e7eb';
  
  const total = portChartData.value.reduce((sum, item) => sum + item.count, 0);
  let currentPercent = 0;
  
  const segments = portChartData.value.map(item => {
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