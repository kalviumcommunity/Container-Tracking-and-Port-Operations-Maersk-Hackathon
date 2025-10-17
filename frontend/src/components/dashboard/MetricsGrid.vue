<template>
  <section class="mb-12">
    <div class="mb-8 flex justify-between items-center">
      <div>
        <h2 class="text-2xl font-bold text-slate-900 mb-2">Port Activity at a Glance</h2>
        <p class="text-slate-600">Live overview of cargo operations and port capacity • Updated every 5 minutes</p>
      </div>
      
      <!-- Port Selector Dropdown -->
      <div class="min-w-64">
        <label for="port-select" class="block text-sm font-medium text-slate-700 mb-2">Select Port</label>
        <select 
          id="port-select"
          :value="selectedPort?.portId || ''"
          @change="handlePortChange"
          class="w-full px-4 py-2 border border-slate-300 rounded-lg bg-white text-slate-900 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
        >
          <option value="">All Ports</option>
          <option 
            v-for="port in ports" 
            :key="port.portId" 
            :value="port.portId"
          >
            {{ port.name }}
          </option>
        </select>
      </div>
    </div>
    
    <!-- Loading State -->
    <div v-if="loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div
        v-for="i in 4"
        :key="i"
        class="bg-white rounded-xl border border-slate-200 p-6 animate-pulse"
      >
        <div class="flex items-start justify-between mb-4">
          <div class="p-3 bg-slate-200 rounded-lg w-12 h-12"></div>
          <div class="text-right">
            <div class="h-4 bg-slate-200 rounded w-20"></div>
          </div>
        </div>
        <div class="mb-3">
          <div class="h-8 bg-slate-200 rounded w-16 mb-2"></div>
          <div class="h-4 bg-slate-200 rounded w-24"></div>
        </div>
        <div class="w-full bg-slate-200 rounded-full h-2"></div>
      </div>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-xl p-6 text-center">
      <div class="text-red-600 mb-2">
        <AlertCircle :size="48" class="mx-auto" />
      </div>
      <h3 class="text-lg font-semibold text-red-900 mb-2">Failed to Load Metrics</h3>
      <p class="text-red-700 mb-4">{{ error }}</p>
      <button
        @click="$emit('retry')"
        class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
      >
        Retry
      </button>
    </div>

    <!-- Metrics Grid -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div
        v-for="(stat, index) in stats"
        :key="stat.title"
        class="bg-white rounded-xl border border-slate-200 p-6 hover:shadow-lg transition-all duration-300 hover:-translate-y-1 group animate-slideIn"
        :style="{ animationDelay: `${index * 100}ms` }"
      >
        <div class="flex items-start gap-4 mb-4">
          <div class="p-3 rounded-lg flex-shrink-0" :class="stat.bgColor">
            <component :is="stat.icon" :size="24" :class="stat.iconColor" />
          </div>
          <div class="flex-1 min-w-0">
            <p class="text-sm font-semibold text-slate-900 mb-1 leading-tight">{{ stat.title }}</p>
            <p v-if="stat.subtitle" class="text-xs text-slate-500 leading-tight">{{ stat.subtitle }}</p>
          </div>
        </div>
        <div class="mb-3">
          <p class="text-3xl font-bold text-slate-900">{{ stat.value }}</p>
          <div class="flex items-center gap-1 mt-2">
            <TrendingUp :size="14" class="text-emerald-600" />
            <span class="text-sm font-medium text-slate-600">{{ stat.change }}</span>
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
</template>

<script setup lang="ts">
import { TrendingUp, AlertCircle } from 'lucide-vue-next';

interface MetricStat {
  title: string;
  subtitle?: string;
  value: string;
  icon: any;
  change: string;
  bgColor: string;
  iconColor: string;
  progressColor: string;
  progress: string;
}

interface Port {
  portId: number;
  name: string;
  location: string;
  country: string;
  capacity: number;
}

interface Props {
  stats: MetricStat[];
  loading?: boolean;
  error?: string | null;
  ports: Port[];
  selectedPort: Port | null;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  retry: [];
  'port-changed': [port: Port | null];
}>();

const handlePortChange = (event: Event) => {
  const target = event.target as HTMLSelectElement;
  const portId = target.value;
  
  if (!portId) {
    emit('port-changed', null);
  } else {
    const selectedPort = props.ports.find(p => p.portId === parseInt(portId));
    emit('port-changed', selectedPort || null);
  }
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