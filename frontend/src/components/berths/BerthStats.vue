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

  <!-- Detailed Stats -->
  <div class="mt-6 grid grid-cols-1 lg:grid-cols-2 gap-6">
    <!-- Status Breakdown -->
    <div class="bg-white shadow rounded-lg p-6">
      <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">Status Breakdown</h3>
      <div class="space-y-3">
        <div v-for="status in statusBreakdown" :key="status.name" class="flex items-center justify-between">
          <div class="flex items-center">
            <div class="w-3 h-3 rounded-full mr-3" :class="status.color"></div>
            <span class="text-sm font-medium text-gray-900">{{ status.name }}</span>
          </div>
          <div class="flex items-center space-x-2">
            <span class="text-sm text-gray-500">{{ status.count }}</span>
            <span class="text-xs text-gray-400">({{ status.percentage }}%)</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Berth Type Distribution -->
    <div class="bg-white shadow rounded-lg p-6">
      <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">Type Distribution</h3>
      <div class="space-y-3">
        <div v-for="type in typeDistribution" :key="type.name" class="flex items-center justify-between">
          <div class="flex items-center">
            <Package class="w-4 h-4 text-gray-400 mr-2" />
            <span class="text-sm font-medium text-gray-900">{{ type.name }}</span>
          </div>
          <div class="flex items-center space-x-2">
            <span class="text-sm text-gray-500">{{ type.count }}</span>
            <span class="text-xs text-gray-400">({{ type.percentage }}%)</span>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- Port Distribution -->
  <div class="mt-6 bg-white shadow rounded-lg p-6">
    <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">Port Distribution</h3>
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div v-for="port in portDistribution" :key="port.name" class="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
        <div class="flex items-center">
          <Building class="w-5 h-5 text-blue-600 mr-3" />
          <span class="text-sm font-medium text-gray-900">{{ port.name }}</span>
        </div>
        <div class="text-right">
          <div class="text-lg font-bold text-gray-900">{{ port.count }}</div>
          <div class="text-xs text-gray-500">{{ port.percentage }}%</div>
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
  TrendingUp,
  Building
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
  statusCounts: Record<string, number>;
  typeCounts: Record<string, number>;
  portCounts: Record<string, number>;
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

const availabilityPercentage = computed(() => {
  if (props.stats.totalBerths === 0) return 0;
  return Math.round((props.stats.availableBerths / props.stats.totalBerths) * 100);
});

const averageUtilization = computed(() => {
  if (props.stats.totalCapacity === 0) return 0;
  return Math.round((props.stats.currentOccupancy / props.stats.totalCapacity) * 100);
});

const statusBreakdown = computed(() => {
  const statusColors = {
    'Available': 'bg-green-500',
    'Occupied': 'bg-yellow-500',
    'Under Maintenance': 'bg-red-500',
    'Reserved': 'bg-blue-500',
    'Out of Service': 'bg-gray-500'
  };

  return Object.entries(props.stats.statusCounts).map(([name, count]) => ({
    name,
    count,
    percentage: Math.round((count / props.stats.totalBerths) * 100),
    color: statusColors[name as keyof typeof statusColors] || 'bg-gray-500'
  }));
});

const typeDistribution = computed(() => {
  return Object.entries(props.stats.typeCounts).map(([name, count]) => ({
    name,
    count,
    percentage: Math.round((count / props.stats.totalBerths) * 100)
  }));
});

const portDistribution = computed(() => {
  return Object.entries(props.stats.portCounts).map(([name, count]) => ({
    name,
    count,
    percentage: Math.round((count / props.stats.totalBerths) * 100)
  }));
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
</script>