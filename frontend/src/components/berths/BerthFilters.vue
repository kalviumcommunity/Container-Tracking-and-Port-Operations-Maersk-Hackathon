<template>
  <div class="bg-white rounded-lg shadow-md p-6">
    <h3 class="text-lg font-semibold text-gray-900 mb-4 flex items-center">
      <Filter class="w-5 h-5 mr-2 text-blue-600" />
      Filter Berths
    </h3>

    <div class="space-y-4">
      <!-- Quick Filters -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">Quick Filters</label>
        <div class="flex flex-wrap gap-2">
          <button
            v-for="quickFilter in quickFilters"
            :key="quickFilter.key"
            @click="applyQuickFilter(quickFilter)"
            class="inline-flex items-center px-3 py-2 border border-gray-300 shadow-sm text-sm leading-4 font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
            :class="{ 'bg-blue-50 border-blue-300 text-blue-700': activeQuickFilter === quickFilter.key }"
          >
            <component :is="quickFilter.icon" class="w-4 h-4 mr-1" />
            {{ quickFilter.label }}
          </button>
        </div>
      </div>

      <!-- Status Filter -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Status</label>
          <select
            v-model="filters.status"
            @change="emitFilters"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="">All Statuses</option>
            <option v-for="status in statusOptions" :key="status" :value="status">
              {{ status }}
            </option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Port</label>
          <select
            v-model="filters.portId"
            @change="emitFilters"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="">All Ports</option>
            <option v-for="port in portOptions" :key="port.id" :value="port.id">
              {{ port.name }}
            </option>
          </select>
        </div>
      </div>

      <!-- Berth Type and Capacity -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Berth Type</label>
          <select
            v-model="filters.berthType"
            @change="emitFilters"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="">All Types</option>
            <option value="Container">Container Terminal</option>
            <option value="Bulk">Bulk Cargo</option>
            <option value="RoRo">RoRo (Roll-on/Roll-off)</option>
            <option value="Passenger">Passenger Terminal</option>
            <option value="General">General Cargo</option>
            <option value="Specialized">Specialized</option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Minimum Capacity</label>
          <input
            v-model.number="filters.minCapacity"
            @input="emitFilters"
            type="number"
            min="0"
            placeholder="e.g., 100"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          />
        </div>
      </div>

      <!-- Utilization Range -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">
          Utilization Range: {{ filters.utilizationMin }}% - {{ filters.utilizationMax }}%
        </label>
        <div class="flex items-center space-x-4">
          <input
            v-model.number="filters.utilizationMin"
            @input="emitFilters"
            type="range"
            min="0"
            max="100"
            class="flex-1"
          />
          <span class="text-sm text-gray-500 w-12">{{ filters.utilizationMin }}%</span>
          <input
            v-model.number="filters.utilizationMax"
            @input="emitFilters"
            type="range"
            min="0"
            max="100"
            class="flex-1"
          />
          <span class="text-sm text-gray-500 w-12">{{ filters.utilizationMax }}%</span>
        </div>
      </div>

      <!-- Features Filter -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">Features</label>
        <div class="grid grid-cols-2 md:grid-cols-3 gap-2">
          <label v-for="feature in featureOptions" :key="feature.key" class="flex items-center">
            <input
              type="checkbox"
              :checked="filters.features.includes(feature.key)"
              @change="toggleFeature(feature.key)"
              class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
            />
            <span class="ml-2 text-sm text-gray-700">{{ feature.label }}</span>
          </label>
        </div>
      </div>

      <!-- Search -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">Search</label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Search class="h-5 w-5 text-gray-400" />
          </div>
          <input
            v-model="filters.search"
            @input="emitFilters"
            type="text"
            placeholder="Search berth names, notes..."
            class="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          />
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="flex justify-between items-center pt-4 border-t border-gray-200">
        <button
          @click="clearFilters"
          class="text-sm text-gray-600 hover:text-gray-800"
        >
          Clear all filters
        </button>
        <div class="text-sm text-gray-500">
          {{ appliedFiltersCount }} filter{{ appliedFiltersCount === 1 ? '' : 's' }} applied
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { 
  Filter, 
  Search,
  Package,
  AlertCircle,
  CheckCircle,
  Clock,
  Zap,
  Users
} from 'lucide-vue-next';

interface Props {
  statusOptions?: string[];
  portOptions?: Array<{ id: number; name: string }>;
  initialFilters?: any;
}

const props = withDefaults(defineProps<Props>(), {
  statusOptions: () => ['Available', 'Occupied', 'Under Maintenance', 'Reserved', 'Full', 'Partially Occupied', 'Inactive'],
  portOptions: () => [],
  initialFilters: () => ({})
});

const emit = defineEmits<{
  'filters-changed': [filters: any];
}>();

const activeQuickFilter = ref('');

const filters = ref({
  status: '',
  portId: '',
  berthType: '',
  minCapacity: null,
  utilizationMin: 0,
  utilizationMax: 100,
  features: [],
  search: '',
  ...props.initialFilters
});

const quickFilters = [
  {
    key: 'available',
    label: 'Available',
    icon: CheckCircle,
    filters: { status: 'Available' }
  },
  {
    key: 'full',
    label: 'At Capacity',
    icon: Package,
    filters: { utilizationMin: 90, utilizationMax: 100 }
  },
  {
    key: 'maintenance',
    label: 'Under Maintenance',
    icon: AlertCircle,
    filters: { status: 'Under Maintenance' }
  },
  {
    key: 'high-capacity',
    label: 'High Capacity',
    icon: Zap,
    filters: { minCapacity: 500 }
  },
  {
    key: 'active',
    label: 'Active',
    icon: Users,
    filters: { status: 'Available,Occupied,Partially Occupied'.split(',') }
  }
];

const featureOptions = [
  { key: 'refrigerated', label: 'Refrigerated' },
  { key: 'dangerous', label: 'Dangerous Goods' },
  { key: 'oversized', label: 'Oversized Cargo' },
  { key: 'heavyLift', label: 'Heavy Lift' },
  { key: 'railConnection', label: 'Rail Connection' },
  { key: 'roadAccess', label: 'Road Access' }
];

const appliedFiltersCount = computed(() => {
  let count = 0;
  
  if (filters.value.status) count++;
  if (filters.value.portId) count++;
  if (filters.value.berthType) count++;
  if (filters.value.minCapacity) count++;
  if (filters.value.utilizationMin > 0 || filters.value.utilizationMax < 100) count++;
  if (filters.value.features.length > 0) count++;
  if (filters.value.search) count++;
  
  return count;
});

const applyQuickFilter = (quickFilter: any) => {
  if (activeQuickFilter.value === quickFilter.key) {
    // Deactivate if already active
    clearFilters();
    return;
  }
  
  activeQuickFilter.value = quickFilter.key;
  
  // Clear previous filters first
  clearFilters();
  
  // Apply the quick filter
  Object.assign(filters.value, quickFilter.filters);
  
  emitFilters();
};

const toggleFeature = (featureKey: string) => {
  const index = filters.value.features.indexOf(featureKey);
  if (index > -1) {
    filters.value.features.splice(index, 1);
  } else {
    filters.value.features.push(featureKey);
  }
  emitFilters();
};

const clearFilters = () => {
  activeQuickFilter.value = '';
  filters.value = {
    status: '',
    portId: '',
    berthType: '',
    minCapacity: null,
    utilizationMin: 0,
    utilizationMax: 100,
    features: [],
    search: ''
  };
  emitFilters();
};

const emitFilters = () => {
  emit('filters-changed', { ...filters.value });
};

// Watch for external filter changes
watch(() => props.initialFilters, (newFilters) => {
  if (newFilters) {
    Object.assign(filters.value, newFilters);
  }
}, { deep: true });
</script>