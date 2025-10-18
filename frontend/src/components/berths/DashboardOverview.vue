<!-- Dashboard Overview Component -->
<template>
  <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
    <!-- Berth Status Overview -->
    <div class="bg-white rounded-xl border border-gray-200 shadow-sm">
      <div class="p-6 border-b border-gray-200">
        <div class="flex items-center justify-between">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-blue-50 rounded-lg">
              <Anchor :size="20" class="text-blue-600" />
            </div>
            <div>
              <h3 class="text-lg font-semibold text-gray-900">Berth Status</h3>
              <p class="text-sm text-gray-600">Real-time berth occupancy</p>
            </div>
          </div>
          <button
            @click="$emit('create-berth')"
            class="px-4 py-2 text-sm font-medium text-blue-600 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors"
          >
            <Plus :size="16" class="inline mr-1" />
            Add Berth
          </button>
        </div>
      </div>
      
      <div class="p-6">
        <div v-if="loading" class="grid grid-cols-3 gap-4">
          <div v-for="i in 6" :key="i" class="animate-pulse">
            <div class="h-24 bg-gray-200 rounded-lg"></div>
          </div>
        </div>
        
        <div v-else class="grid grid-cols-2 md:grid-cols-3 gap-4">
          <div
            v-for="berth in displayBerths"
            :key="berth.berthId || berth.id"
            @click="$emit('view-berth', berth)"
            class="p-4 border border-gray-200 rounded-lg hover:shadow-md transition-all duration-200 cursor-pointer"
            :class="getBerthStatusClass(berth.status)"
          >
            <div class="text-center space-y-2">
              <div class="text-sm font-bold text-gray-900">{{ berth.name }}</div>
              <span 
                class="inline-flex items-center px-2 py-1 text-xs font-semibold rounded-full"
                :class="getStatusBadgeClass(berth.status)"
              >
                {{ berth.status }}
              </span>
              <div v-if="berth.capacity" class="text-xs text-gray-600">
                {{ berth.currentLoad || 0 }} / {{ berth.capacity }}
              </div>
            </div>
          </div>
        </div>
        
        <div v-if="berths.length > 6" class="mt-4 text-center">
          <button
            @click="$emit('view-all-berths')"
            class="text-sm text-blue-600 hover:text-blue-800 font-medium"
          >
            View All {{ berths.length }} Berths →
          </button>
        </div>
      </div>
    </div>

    <!-- Active Operations -->
    <div class="bg-white rounded-xl border border-gray-200 shadow-sm">
      <div class="p-6 border-b border-gray-200">
        <div class="flex items-center justify-between">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-purple-50 rounded-lg">
              <Activity :size="20" class="text-purple-600" />
            </div>
            <div>
              <h3 class="text-lg font-semibold text-gray-900">Active Operations</h3>
              <p class="text-sm text-gray-600">Current activities</p>
            </div>
          </div>
          <button
            @click="$emit('assign-berth')"
            class="px-4 py-2 text-sm font-medium text-purple-600 bg-purple-50 rounded-lg hover:bg-purple-100 transition-colors"
          >
            <Settings :size="16" class="inline mr-1" />
            Assign
          </button>
        </div>
      </div>
      
      <div class="p-6">
        <div v-if="loading" class="space-y-4">
          <div v-for="i in 4" :key="i" class="animate-pulse">
            <div class="h-16 bg-gray-200 rounded-lg"></div>
          </div>
        </div>
        
        <div v-else-if="operations.length === 0" class="text-center py-8">
          <Activity :size="48" class="mx-auto text-gray-300 mb-4" />
          <p class="text-gray-500">No active operations</p>
          <button
            @click="$emit('assign-berth')"
            class="mt-2 text-sm text-blue-600 hover:text-blue-800"
          >
            Create Assignment
          </button>
        </div>
        
        <div v-else class="space-y-4">
          <div
            v-for="operation in operations.slice(0, 4)"
            :key="operation.id"
            class="p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"
          >
            <div class="flex items-center justify-between mb-3">
              <div class="flex items-center space-x-3">
                <div class="w-8 h-8 rounded-lg bg-white border border-gray-200 flex items-center justify-center font-bold text-gray-700 text-sm">
                  {{ operation.id.slice(-2) }}
                </div>
                <div>
                  <div class="font-medium text-gray-900 text-sm">{{ operation.vessel }}</div>
                  <div class="text-xs text-gray-600">{{ operation.berth || 'Berth TBD' }}</div>
                </div>
              </div>
              <div class="text-right">
                <span 
                  class="inline-flex items-center px-2 py-1 text-xs font-semibold rounded-full"
                  :class="getPriorityBadgeClass(operation.priority)"
                >
                  {{ operation.priority }}
                </span>
                <div class="text-xs text-gray-600 mt-1">{{ operation.type }}</div>
              </div>
            </div>
            
            <div class="space-y-2">
              <div class="flex justify-between text-sm">
                <span class="text-gray-600">Progress</span>
                <span class="font-medium text-gray-900">{{ operation.progress }}%</span>
              </div>
              <div class="w-full bg-gray-200 rounded-full h-2">
                <div
                  class="bg-gradient-to-r from-blue-500 to-blue-600 h-2 rounded-full transition-all duration-1000"
                  :style="{ width: `${operation.progress}%` }"
                ></div>
              </div>
              <div class="flex items-center gap-2 text-sm text-gray-600">
                <Clock :size="12" />
                <span>ETA: {{ operation.eta }}</span>
              </div>
            </div>
          </div>
        </div>
        
        <div v-if="operations.length > 4" class="mt-4 text-center">
          <button
            @click="$emit('view-all-operations')"
            class="text-sm text-purple-600 hover:text-purple-800 font-medium"
          >
            View All {{ operations.length }} Operations →
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { 
  Anchor, 
  Activity, 
  Plus, 
  Settings, 
  Clock 
} from 'lucide-vue-next';

interface Berth {
  berthId?: number;
  id?: number;
  name: string;
  status: string;
  capacity?: number;
  currentLoad?: number;
}

interface Operation {
  id: string;
  vessel: string;
  berth?: string;
  type: string;
  progress: number;
  eta: string;
  priority: string;
}

interface Props {
  berths: Berth[];
  operations: Operation[];
  loading: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  berths: () => [],
  operations: () => [],
  loading: false
});

defineEmits<{
  'create-berth': [];
  'assign-berth': [];
  'view-berth': [berth: Berth];
  'view-all-berths': [];
  'view-all-operations': [];
}>();

const displayBerths = computed(() => {
  return props.berths.slice(0, 6);
});

const getBerthStatusClass = (status: string): string => {
  const statusClasses = {
    'Available': 'border-green-300 bg-green-50',
    'Occupied': 'border-orange-300 bg-orange-50',
    'Under Maintenance': 'border-red-300 bg-red-50',
    'Reserved': 'border-blue-300 bg-blue-50'
  };
  return statusClasses[status as keyof typeof statusClasses] || 'border-gray-300 bg-gray-50';
};

const getStatusBadgeClass = (status: string): string => {
  const statusClasses = {
    'Available': 'bg-green-100 text-green-800',
    'Occupied': 'bg-orange-100 text-orange-800',
    'Under Maintenance': 'bg-red-100 text-red-800',
    'Reserved': 'bg-blue-100 text-blue-800'
  };
  return statusClasses[status as keyof typeof statusClasses] || 'bg-gray-100 text-gray-800';
};

const getPriorityBadgeClass = (priority: string): string => {
  const priorityClasses = {
    'High': 'bg-red-100 text-red-800',
    'Medium': 'bg-yellow-100 text-yellow-800',
    'Low': 'bg-green-100 text-green-800'
  };
  return priorityClasses[priority as keyof typeof priorityClasses] || 'bg-gray-100 text-gray-800';
};
</script>

<style scoped>
.animate-pulse {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}
</style>
