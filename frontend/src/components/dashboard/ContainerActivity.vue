<template>
  <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
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
        <button 
          @click="$emit('view-all')"
          class="px-4 py-2 text-sm font-medium text-blue-600 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors"
        >
          View All
        </button>
      </div>
    </div>
    
    <div class="p-6">
      <!-- Loading State -->
      <div v-if="loading" class="space-y-4">
        <div v-for="i in 4" :key="i" class="flex items-center justify-between p-4 bg-slate-50 rounded-lg animate-pulse">
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 rounded-lg bg-slate-200"></div>
            <div class="flex flex-col gap-2">
              <div class="h-4 bg-slate-200 rounded w-24"></div>
              <div class="h-3 bg-slate-200 rounded w-20"></div>
            </div>
            <div class="h-6 bg-slate-200 rounded-full w-16"></div>
          </div>
          <div class="text-right space-y-1">
            <div class="h-4 bg-slate-200 rounded w-16"></div>
            <div class="h-3 bg-slate-200 rounded w-12"></div>
          </div>
        </div>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="text-center py-8">
        <AlertCircle :size="48" class="mx-auto text-red-500 mb-4" />
        <h3 class="text-lg font-semibold text-red-900 mb-2">Failed to Load Container Activity</h3>
        <p class="text-red-700 mb-4">{{ error }}</p>
        <button
          @click="$emit('retry')"
          class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
        >
          Retry
        </button>
      </div>

      <!-- Empty State -->
      <div v-else-if="containers.length === 0" class="text-center py-8">
        <Container :size="48" class="mx-auto text-slate-400 mb-4" />
        <h3 class="text-lg font-semibold text-slate-700 mb-2">No Container Activity</h3>
        <p class="text-slate-500">No recent container movements to display</p>
      </div>

      <!-- Container List -->
      <div v-else class="space-y-4">
        <div 
          v-for="(container, index) in containers"
          :key="container.id"
          class="flex items-center justify-between p-4 bg-slate-50 rounded-lg hover:bg-slate-100 transition-all duration-200 group animate-slideIn"
          :style="{ animationDelay: `${index * 100}ms` }"
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
      
      <!-- Footer -->
      <div v-if="!loading && !error && containers.length > 0" class="mt-6 pt-4 border-t border-slate-200 flex items-center justify-between">
        <p class="text-sm text-slate-600">
          Showing latest {{ containers.length }} activities • <span class="font-semibold">{{ totalOperations }} total operations today</span>
        </p>
        <button 
          @click="$emit('view-detailed-log')"
          class="text-sm font-medium text-blue-600 hover:text-blue-700 transition-colors"
        >
          View detailed log →
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Container, AlertCircle } from 'lucide-vue-next';

interface ContainerActivity {
  id: string;
  status: string;
  berth: string;
  time: string;
  type: string;
}

interface Props {
  containers: ContainerActivity[];
  loading?: boolean;
  error?: string | null;
  totalOperations?: number;
}

withDefaults(defineProps<Props>(), {
  loading: false,
  error: null,
  totalOperations: 0
});

defineEmits<{
  'view-all': [];
  'view-detailed-log': [];
  retry: [];
}>();

const getStatusColor = (status: string): string => {
  const statusColors: Record<string, string> = {
    "Arrived": "bg-blue-100 text-blue-800 border-blue-200",
    "Loading": "bg-orange-100 text-orange-800 border-orange-200",
    "Inspection": "bg-purple-100 text-purple-800 border-purple-200",
    "Departed": "bg-green-100 text-green-800 border-green-200",
  };
  return statusColors[status] || "bg-slate-100 text-slate-800 border-slate-200";
};
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
</style>