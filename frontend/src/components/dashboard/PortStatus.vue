<template>
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
      <!-- Loading State -->
      <div v-if="loading" class="space-y-6">
        <div class="p-4 bg-slate-100 rounded-lg border animate-pulse">
          <div class="flex items-center gap-2 mb-2">
            <div class="w-4 h-4 bg-slate-300 rounded"></div>
            <div class="h-4 bg-slate-300 rounded w-24"></div>
          </div>
          <div class="h-8 bg-slate-300 rounded w-32 mb-2"></div>
          <div class="h-4 bg-slate-300 rounded w-48"></div>
        </div>
        <div class="space-y-5">
          <div class="space-y-3">
            <div class="flex justify-between items-center">
              <div class="h-4 bg-slate-200 rounded w-24"></div>
              <div class="h-4 bg-slate-200 rounded w-16"></div>
            </div>
            <div class="w-full bg-slate-200 rounded-full h-3"></div>
          </div>
          <div class="space-y-3">
            <div class="flex justify-between items-center">
              <div class="h-4 bg-slate-200 rounded w-28"></div>
              <div class="h-4 bg-slate-200 rounded w-20"></div>
            </div>
            <div class="w-full bg-slate-200 rounded-full h-3"></div>
          </div>
        </div>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="text-center py-4">
        <AlertCircle :size="48" class="mx-auto text-red-500 mb-4" />
        <h3 class="text-lg font-semibold text-red-900 mb-2">Failed to Load Port Status</h3>
        <p class="text-red-700 mb-4">{{ error }}</p>
        <button
          @click="$emit('retry')"
          class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
        >
          Retry
        </button>
      </div>

      <!-- Port Status Content -->
      <template v-else>
        <!-- Operational Status -->
        <div class="p-4 bg-gradient-to-r from-green-50 to-green-100 rounded-lg border border-green-200">
          <div class="flex items-center gap-2 mb-2">
            <CheckCircle :size="18" class="text-green-600" />
            <span class="text-sm font-semibold text-green-800">{{ statusData.operationalStatus }}</span>
          </div>
          <p class="text-2xl font-bold text-green-900">{{ statusData.systemStatus }}</p>
          <p class="text-sm text-green-700 mt-1">{{ statusData.statusMessage }}</p>
        </div>
        
        <!-- Utilization Metrics -->
        <div class="space-y-5">
          <div>
            <div class="flex justify-between items-center mb-3">
              <span class="text-sm font-medium text-slate-700">Berth Utilization</span>
              <span class="text-sm font-bold text-slate-900">{{ berthUtilization.percentage }}% ({{ berthUtilization.occupied }}/{{ berthUtilization.total }})</span>
            </div>
            <div class="w-full bg-slate-200 rounded-full h-3">
              <div 
                class="bg-gradient-to-r from-blue-500 to-blue-600 h-3 rounded-full transition-all duration-1000 ease-out" 
                :style="{ width: berthUtilization.percentage + '%' }"
              ></div>
            </div>
            <p class="text-xs text-slate-600 mt-2">{{ berthUtilization.available }} berths available for incoming vessels</p>
          </div>
          
          <div>
            <div class="flex justify-between items-center mb-3">
              <span class="text-sm font-medium text-slate-700">Container Capacity</span>
              <span class="text-sm font-bold text-slate-900">{{ containerCapacity.percentage }}% ({{ containerCapacity.current }}/{{ containerCapacity.total }})</span>
            </div>
            <div class="w-full bg-slate-200 rounded-full h-3">
              <div 
                class="bg-gradient-to-r from-orange-400 to-orange-500 h-3 rounded-full transition-all duration-1000 ease-out" 
                :style="{ width: containerCapacity.percentage + '%' }"
              ></div>
            </div>
            <p class="text-xs text-slate-600 mt-2">{{ containerCapacity.message }}</p>
          </div>
        </div>

        <!-- Analytics Button -->
        <button 
          @click="$emit('view-analytics')"
          class="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-3 px-4 rounded-lg transition-colors duration-200 flex items-center justify-center gap-2"
        >
          <TrendingUp :size="16" />
          View Analytics Dashboard
        </button>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Activity, CheckCircle, AlertCircle, TrendingUp } from 'lucide-vue-next';

interface PortStatusData {
  operationalStatus: string;
  systemStatus: string;
  statusMessage: string;
}

interface UtilizationData {
  percentage: number;
  occupied: number;
  total: number;
  available: number;
}

interface ContainerCapacityData {
  percentage: number;
  current: number;
  total: number;
  message: string;
}

interface Props {
  loading?: boolean;
  error?: string | null;
  statusData: PortStatusData;
  berthUtilization: UtilizationData;
  containerCapacity: ContainerCapacityData;
}

withDefaults(defineProps<Props>(), {
  loading: false,
  error: null
});

defineEmits<{
  'view-analytics': [];
  retry: [];
}>();
</script>