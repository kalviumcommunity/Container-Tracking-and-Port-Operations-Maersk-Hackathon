<!-- Simplified Berth Management Component -->
<template>
  <div class="space-y-6">
    <!-- Filters and Search -->
    <div class="bg-white rounded-lg border border-gray-200 p-4">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Search</label>
          <input
            v-model="localFilters.search"
            type="text"
            placeholder="Search berths..."
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Status</label>
          <select
            v-model="localFilters.status"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">All Statuses</option>
            <option v-for="status in statusOptions" :key="status" :value="status">
              {{ status }}
            </option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Port</label>
          <select
            v-model="localFilters.portId"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">All Ports</option>
            <option v-for="port in portOptions" :key="port.id" :value="port.id">
              {{ port.name }}
            </option>
          </select>
        </div>
        <div class="flex items-end space-x-2">
          <button
            @click="$emit('create')"
            class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors"
          >
            <Plus :size="16" class="inline mr-1" />
            New Berth
          </button>
        </div>
      </div>
    </div>

    <!-- Berth Grid -->
    <div v-if="loading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="i in 6" :key="i" class="animate-pulse">
        <div class="h-64 bg-gray-200 rounded-lg"></div>
      </div>
    </div>

    <div v-else-if="berths.length === 0" class="text-center py-12">
      <Anchor :size="48" class="mx-auto text-gray-300 mb-4" />
      <h3 class="text-lg font-medium text-gray-900 mb-2">No berths found</h3>
      <p class="text-gray-600 mb-4">Get started by creating your first berth.</p>
      <button
        @click="$emit('create')"
        class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors"
      >
        Create Berth
      </button>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <BerthCard
        v-for="berth in berths"
        :key="berth.berthId || berth.id"
        :berth="berth"
        @view="$emit('view', $event)"
        @edit="$emit('edit', $event)"
        @delete="$emit('delete', $event)"
        @assignments="$emit('assign', $event)"
      />
    </div>

    <!-- Pagination -->
    <div v-if="pagination.totalPages > 1" class="flex items-center justify-between">
      <div class="text-sm text-gray-700">
        Showing {{ (pagination.page - 1) * pagination.pageSize + 1 }} to 
        {{ Math.min(pagination.page * pagination.pageSize, pagination.totalCount) }} of 
        {{ pagination.totalCount }} berths
      </div>
      <div class="flex space-x-2">
        <button
          @click="$emit('page-change', pagination.page - 1)"
          :disabled="!pagination.hasPreviousPage"
          class="px-3 py-2 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Previous
        </button>
        <button
          @click="$emit('page-change', pagination.page + 1)"
          :disabled="!pagination.hasNextPage"
          class="px-3 py-2 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Next
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { Plus, Anchor } from 'lucide-vue-next';
import BerthCard from './BerthCard.vue';

interface Berth {
  berthId?: number;
  id?: number;
  name: string;
  status: string;
  portId: number;
  capacity: number;
  currentLoad?: number;
}

interface Filters {
  search: string;
  status: string;
  portId: string;
  pageSize: number;
  page: number;
}

interface Pagination {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

interface Props {
  berths: Berth[];
  filters: Filters;
  pagination: Pagination;
  statusOptions: string[];
  portOptions: Array<{ id: number; name: string }>;
  loading: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  berths: () => [],
  statusOptions: () => [],
  portOptions: () => [],
  loading: false
});

const emit = defineEmits<{
  create: [];
  edit: [berth: Berth];
  view: [berth: Berth];
  delete: [berth: Berth];
  assign: [berth: Berth];
  'filter-change': [filters: Partial<Filters>];
  'page-change': [page: number];
}>();

const localFilters = ref({ ...props.filters });

watch(localFilters, (newFilters) => {
  emit('filter-change', newFilters);
}, { deep: true });
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
