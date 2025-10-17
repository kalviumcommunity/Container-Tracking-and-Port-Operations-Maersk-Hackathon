<template>
  <!-- Port Selection -->
  <div class="mb-6 bg-white shadow rounded-lg p-4">
    <div class="flex items-center justify-between">
      <h3 class="text-lg font-medium text-gray-900">Berth Statistics</h3>
      <div class="flex items-center space-x-2">
        <label for="port-select" class="text-sm font-medium text-gray-700">Port:</label>
        <select
          id="port-select"
          v-model="selectedPort"
          @change="onPortChange"
          class="border border-gray-300 rounded-md px-3 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        >
          <option value="all">All Ports</option>
          <option v-for="port in ports" :key="port.id" :value="port.id">
            {{ port.name }}
          </option>
        </select>
      </div>
    </div>
  </div>

  <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
    <!-- Total Berths -->
    <div class="bg-white overflow-hidden shadow rounded-lg">
      <div class="p-5">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-blue-100 rounded-md flex items-center justify-center">
              <MapPin class="w-5 h-5 text-blue-600" />
            </div>
          </div>
          <div class="ml-5 w-0 flex-1">
            <dl>
              <dt class="text-sm font-medium text-gray-500 truncate">Total Berths</dt>
              <dd class="text-lg font-medium text-gray-900">
                {{ formatNumber(stats.totalBerths) }}
              </dd>
            </dl>
          </div>
        </div>
      </div>
      <div class="bg-gray-50 px-5 py-3">
        <div class="text-sm">
          <span class="text-green-600 font-medium">{{ stats.activeBerths }}</span>
          <span class="text-gray-500"> active</span>
        </div>
      </div>
    </div>

    <!-- Available Berths -->
    <div class="bg-white overflow-hidden shadow rounded-lg">
      <div class="p-5">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-green-100 rounded-md flex items-center justify-center">
              <CheckCircle class="w-5 h-5 text-green-600" />
            </div>
          </div>
          <div class="ml-5 w-0 flex-1">
            <dl>
              <dt class="text-sm font-medium text-gray-500 truncate">Available</dt>
              <dd class="text-lg font-medium text-gray-900">
                {{ formatNumber(stats.availableBerths) }}
              </dd>
            </dl>
          </div>
        </div>
      </div>
      <div class="bg-gray-50 px-5 py-3">
        <div class="text-sm">
          <span :class="getPercentageClass(availabilityPercentage)" class="font-medium">
            {{ availabilityPercentage }}%
          </span>
          <span class="text-gray-500"> of total</span>
        </div>
      </div>
    </div>

    <!-- Total Capacity -->
    <div class="bg-white overflow-hidden shadow rounded-lg">
      <div class="p-5">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-purple-100 rounded-md flex items-center justify-center">
              <Package class="w-5 h-5 text-purple-600" />
            </div>
          </div>
          <div class="ml-5 w-0 flex-1">
            <dl>
              <dt class="text-sm font-medium text-gray-500 truncate">Total Capacity</dt>
              <dd class="text-lg font-medium text-gray-900">
                {{ formatNumber(stats.totalCapacity) }}
              </dd>
            </dl>
          </div>
        </div>
      </div>
      <div class="bg-gray-50 px-5 py-3">
        <div class="text-sm">
          <span class="text-blue-600 font-medium">{{ formatNumber(stats.currentOccupancy) }}</span>
          <span class="text-gray-500"> in use</span>
        </div>
      </div>
    </div>

    <!-- Average Utilization -->
    <div class="bg-white overflow-hidden shadow rounded-lg">
      <div class="p-5">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-yellow-100 rounded-md flex items-center justify-center">
              <TrendingUp class="w-5 h-5 text-yellow-600" />
            </div>
          </div>
          <div class="ml-5 w-0 flex-1">
            <dl>
              <dt class="text-sm font-medium text-gray-500 truncate">Avg. Utilization</dt>
              <dd class="text-lg font-medium text-gray-900">
                {{ averageUtilization }}%
              </dd>
            </dl>
          </div>
        </div>
      </div>
      <div class="bg-gray-50 px-5 py-3">
        <div class="text-sm">
          <span :class="getUtilizationClass(averageUtilization)" class="font-medium">
            {{ getUtilizationStatus(averageUtilization) }}
          </span>
          <span class="text-gray-500"> capacity</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted } from 'vue';
import { 
  MapPin, 
  CheckCircle, 
  Package, 
  TrendingUp
} from 'lucide-vue-next';

interface Port {
  id: string;
  name: string;
}

interface BerthStats {
  totalBerths: number;
  activeBerths: number;
  availableBerths: number;
  totalCapacity: number;
  currentOccupancy: number;
}

interface Props {
  stats: BerthStats;
  loading?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  loading: false
});

const emit = defineEmits<{
  (e: 'port-changed', portId: string): void;
}>();

// Reactive data
const selectedPort = ref<string>('all');
const ports = ref<Port[]>([]);

// Computed properties
const availabilityPercentage = computed(() => {
  if (props.stats.totalBerths === 0) return 0;
  return Math.round((props.stats.availableBerths / props.stats.totalBerths) * 100);
});

const averageUtilization = computed(() => {
  if (props.stats.totalCapacity === 0) return 0;
  return Math.round((props.stats.currentOccupancy / props.stats.totalCapacity) * 100);
});

// Methods
const onPortChange = () => {
  emit('port-changed', selectedPort.value);
};

const fetchPorts = async () => {
  try {
    const response = await fetch('/api/ports');
    if (response.ok) {
      ports.value = await response.json();
    }
  } catch (error) {
    console.error('Failed to fetch ports:', error);
  }
};

// Lifecycle
onMounted(() => {
  fetchPorts();
});

const getPercentageClass = (percentage: number): string => {
  if (percentage >= 75) return 'text-green-600';
  if (percentage >= 50) return 'text-yellow-600';
  if (percentage >= 25) return 'text-orange-600';
  return 'text-red-600';
};

const getUtilizationClass = (utilization: number): string => {
  if (utilization >= 90) return 'text-red-600';
  if (utilization >= 75) return 'text-yellow-600';
  if (utilization >= 50) return 'text-blue-600';
  return 'text-green-600';
};

const getUtilizationStatus = (utilization: number): string => {
  if (utilization >= 90) return 'Critical';
  if (utilization >= 75) return 'High';
  if (utilization >= 50) return 'Moderate';
  return 'Low';
};

const formatNumber = (num: number): string => {
  if (!num) return '0';
  return new Intl.NumberFormat('en-US').format(num);
};
</script>