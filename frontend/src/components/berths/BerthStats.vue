<template>
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

    <!-- Port Distribution -->
    <div class="bg-white shadow rounded-lg p-6">
      <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">Port Distribution</h3>
      <div class="space-y-3">
        <div v-for="port in portDistribution" :key="port.name" class="flex items-center justify-between">
          <div class="flex items-center">
            <Building class="w-4 h-4 text-gray-400 mr-2" />
            <span class="text-sm font-medium text-gray-900">{{ port.name }}</span>
          </div>
          <div class="flex items-center space-x-2">
            <span class="text-sm text-gray-500">{{ port.count }}</span>
            <span class="text-xs text-gray-400">({{ port.percentage }}%)</span>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- Utilization Chart -->
  <div class="mt-6 bg-white shadow rounded-lg p-6">
    <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">Utilization Distribution</h3>
    <div class="space-y-4">
      <div v-for="range in utilizationRanges" :key="range.label" class="flex items-center">
        <div class="w-24 text-sm text-gray-600">{{ range.label }}</div>
        <div class="flex-1 mx-4">
          <div class="bg-gray-200 rounded-full h-4">
            <div 
              class="h-4 rounded-full transition-all duration-300"
              :class="range.color"
              :style="{ width: `${(range.count / stats.totalBerths) * 100}%` }"
            ></div>
          </div>
        </div>
        <div class="w-16 text-sm text-gray-500 text-right">
          {{ range.count }} berths
        </div>
      </div>
    </div>
  </div>

  <!-- Feature Usage -->
  <div class="mt-6 bg-white shadow rounded-lg p-6">
    <h3 class="text-lg leading-6 font-medium text-gray-900 mb-4">Feature Usage</h3>
    <div class="grid grid-cols-2 md:grid-cols-3 gap-4">
      <div v-for="feature in featureUsage" :key="feature.name" class="text-center">
        <div class="text-2xl font-bold" :class="feature.color">{{ feature.count }}</div>
        <div class="text-sm text-gray-500">{{ feature.name }}</div>
        <div class="text-xs text-gray-400">{{ feature.percentage }}% of berths</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { 
  MapPin, 
  CheckCircle, 
  Package, 
  TrendingUp,
  Building
} from 'lucide-vue-next';

interface BerthStats {
  totalBerths: number;
  activeBerths: number;
  availableBerths: number;
  totalCapacity: number;
  currentOccupancy: number;
  statusCounts: Record<string, number>;
  portCounts: Record<string, number>;
  utilizationRanges: Record<string, number>;
  featureCounts: Record<string, number>;
}

interface Props {
  stats: BerthStats;
  loading?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  loading: false
});

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
    'Full': 'bg-purple-500',
    'Partially Occupied': 'bg-orange-500',
    'Inactive': 'bg-gray-500'
  };

  return Object.entries(props.stats.statusCounts).map(([name, count]) => ({
    name,
    count,
    percentage: Math.round((count / props.stats.totalBerths) * 100),
    color: statusColors[name as keyof typeof statusColors] || 'bg-gray-500'
  }));
});

const portDistribution = computed(() => {
  return Object.entries(props.stats.portCounts).map(([name, count]) => ({
    name,
    count,
    percentage: Math.round((count / props.stats.totalBerths) * 100)
  }));
});

const utilizationRanges = computed(() => {
  return [
    {
      label: '0-25%',
      count: props.stats.utilizationRanges['0-25'] || 0,
      color: 'bg-green-500'
    },
    {
      label: '26-50%',
      count: props.stats.utilizationRanges['26-50'] || 0,
      color: 'bg-blue-500'
    },
    {
      label: '51-75%',
      count: props.stats.utilizationRanges['51-75'] || 0,
      color: 'bg-yellow-500'
    },
    {
      label: '76-90%',
      count: props.stats.utilizationRanges['76-90'] || 0,
      color: 'bg-orange-500'
    },
    {
      label: '91-100%',
      count: props.stats.utilizationRanges['91-100'] || 0,
      color: 'bg-red-500'
    }
  ];
});

const featureUsage = computed(() => {
  const features = [
    { key: 'refrigerated', name: 'Refrigerated', color: 'text-blue-600' },
    { key: 'dangerous', name: 'Dangerous Goods', color: 'text-red-600' },
    { key: 'oversized', name: 'Oversized', color: 'text-purple-600' },
    { key: 'heavyLift', name: 'Heavy Lift', color: 'text-orange-600' },
    { key: 'railConnection', name: 'Rail Connection', color: 'text-green-600' },
    { key: 'roadAccess', name: 'Road Access', color: 'text-indigo-600' }
  ];

  return features.map(feature => {
    const count = props.stats.featureCounts[feature.key] || 0;
    return {
      ...feature,
      count,
      percentage: Math.round((count / props.stats.totalBerths) * 100)
    };
  });
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