<template>
  <div class="bg-white rounded-lg shadow-md p-6 mb-6">
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
            class="inline-flex items-center px-3 py-2 border border-gray-300 shadow-sm text-sm leading-4 font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors"
            :class="{ 'bg-blue-50 border-blue-300 text-blue-700': activeQuickFilter === quickFilter.key }"
          >
            <component :is="quickFilter.icon" class="w-4 h-4 mr-1" />
            {{ quickFilter.label }}
          </button>
        </div>
      </div>

      <!-- Status and Port Filter -->
      <div class="grid grid-cols-1 md:grid-cols-1 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Status</label>
          <select
            v-model="localFilters.status"
            @change="emitFilters"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="">All Statuses</option>
            <option v-for="status in statusOptions" :key="status" :value="status">
              {{ status }}
            </option>
          </select>
        </div>

        <!-- Port Filter Removed - Not needed for single-port demo -->
        <!-- <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Port</label>
          <select
            v-model="localFilters.portId"
            @change="emitFilters"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="">All Ports</option>
            <option v-for="port in portOptions" :key="port.id" :value="port.id.toString()">
              {{ port.name }}
            </option>
          </select>
        </div> -->
      </div>

      <!-- Berth Type and Min Capacity -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Berth Type</label>
          <select
            v-model="localFilters.type"
            @change="emitFilters"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="">All Types</option>
            <option v-for="type in typeOptions" :key="type" :value="type">
              {{ type }}
            </option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Minimum Capacity</label>
          <input
            v-model="localFilters.minCapacity"
            @input="emitFilters"
            type="number"
            min="0"
            placeholder="e.g., 100"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          />
        </div>
      </div>

      <!-- Max Capacity and Search -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Maximum Capacity</label>
          <input
            v-model="localFilters.maxCapacity"
            @input="emitFilters"
            type="number"
            min="0"
            placeholder="e.g., 1000"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Search</label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Search class="h-5 w-5 text-gray-400" />
            </div>
            <input
              v-model="localFilters.searchTerm"
              @input="emitFilters"
              type="text"
              placeholder="Search berth names..."
              class="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            />
          </div>
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="flex justify-between items-center pt-4 border-t border-gray-200">
        <button
          @click="clearFilters"
          class="inline-flex items-center px-4 py-2 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-all duration-200 hover:border-red-300 hover:text-red-600"
          :disabled="appliedFiltersCount === 0"
          :class="{ 'opacity-50 cursor-not-allowed': appliedFiltersCount === 0 }"
        >
          <Filter class="w-4 h-4 mr-2" />
          Clear all filters
        </button>
        <div class="text-sm text-gray-500">
          <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                :class="appliedFiltersCount > 0 ? 'bg-blue-100 text-blue-800' : 'bg-gray-100 text-gray-600'">
            {{ appliedFiltersCount }} filter{{ appliedFiltersCount === 1 ? '' : 's' }} applied
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { Filter, Search, CheckCircle, Package, AlertCircle, Zap, Users } from 'lucide-vue-next';
import type { BerthFilters } from '../../types/berth';

const props = defineProps<{
  statusOptions: string[];
  typeOptions: string[];
  portOptions: Array<{ id: number; name: string }>;
}>();

const emit = defineEmits<{
  'update:filters': [filters: BerthFilters];
  apply: [];
  clear: [];
}>();

const activeQuickFilter = ref('');
const localFilters = ref<BerthFilters>({
  status: '',
  type: '',
  portId: '',
  minCapacity: '',
  maxCapacity: '',
  searchTerm: '',
  page: 1,
  pageSize: 25,
  sortBy: 'updatedAt',
  sortDirection: 'desc'
});

const quickFilters = [
  { key: 'available', label: 'Available', icon: CheckCircle, filters: { status: 'Available' } },
  { key: 'occupied', label: 'Occupied', icon: Package, filters: { status: 'Occupied' } },
  { key: 'maintenance', label: 'Under Maintenance', icon: AlertCircle, filters: { status: 'Under Maintenance' } },
  { key: 'high-capacity', label: 'High Capacity (500+)', icon: Zap, filters: { minCapacity: '500' } },
  { key: 'reserved', label: 'Reserved', icon: Users, filters: { status: 'Reserved' } }
];

const appliedFiltersCount = computed(() => {
  let count = 0;
  if (localFilters.value.status) count++;
  if (localFilters.value.portId) count++;
  if (localFilters.value.type) count++;
  if (localFilters.value.minCapacity) count++;
  if (localFilters.value.maxCapacity) count++;
  if (localFilters.value.searchTerm) count++;
  return count;
});

const applyQuickFilter = (quickFilter: any) => {
  if (activeQuickFilter.value === quickFilter.key) {
    clearFilters();
    return;
  }
  activeQuickFilter.value = quickFilter.key;
  localFilters.value = {
    status: '', type: '', portId: '', minCapacity: '', maxCapacity: '', searchTerm: '',
    page: 1, pageSize: 25, sortBy: 'updatedAt', sortDirection: 'desc'
  };
  Object.assign(localFilters.value, quickFilter.filters);
  emitFilters();
};

const clearFilters = () => {
  activeQuickFilter.value = '';
  localFilters.value = {
    status: '', type: '', portId: '', minCapacity: '', maxCapacity: '', searchTerm: '',
    page: 1, pageSize: 25, sortBy: 'updatedAt', sortDirection: 'desc'
  };
  emit('clear');
  emitFilters();
};

const emitFilters = () => {
  emit('update:filters', { ...localFilters.value });
  emit('apply');
};
</script>
