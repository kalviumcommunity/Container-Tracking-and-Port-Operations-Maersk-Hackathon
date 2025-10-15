<template>
  <div class="bg-white rounded-lg shadow-md overflow-hidden">
    <!-- Table Header -->
    <div class="px-6 py-4 border-b border-gray-200">
      <div class="flex justify-between items-center">
        <div class="flex items-center space-x-4">
          <h2 class="text-lg font-semibold text-gray-800">Containers</h2>
          <span class="text-sm text-gray-500">
            {{ paginationInfo.start }}-{{ paginationInfo.end }} of {{ paginationInfo.total }}
          </span>
        </div>
        <div class="flex items-center space-x-2">
          <label class="text-sm text-gray-700">Show:</label>
          <select
            :value="filters.pageSize"
            @change="$emit('update-page-size', Number($event.target.value))"
            class="px-2 py-1 border border-gray-300 rounded text-sm"
          >
            <option :value="10">10</option>
            <option :value="25">25</option>
            <option :value="50">50</option>
            <option :value="100">100</option>
          </select>
        </div>
      </div>
    </div>
    
    <!-- Table Content -->
    <div class="overflow-x-auto">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left">
              <input
                type="checkbox"
                :checked="allSelected"
                :indeterminate="someSelected"
                @change="$emit('toggle-select-all')"
                class="rounded border-gray-300 text-blue-600"
              >
            </th>
            <th 
              @click="$emit('sort', 'containerId')"
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer"
            >
              Container ID
              <span v-if="currentSort.field === 'containerId'">
                {{ currentSort.direction === 'asc' ? '↑' : '↓' }}
              </span>
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Cargo Type
            </th>
            <th 
              @click="$emit('sort', 'status')"
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer"
            >
              Status
              <span v-if="currentSort.field === 'status'">
                {{ currentSort.direction === 'asc' ? '↑' : '↓' }}
              </span>
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Location
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Ship
            </th>
            <th 
              @click="$emit('sort', 'updatedAt')"
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer"
            >
              Last Updated
              <span v-if="currentSort.field === 'updatedAt'">
                {{ currentSort.direction === 'asc' ? '↑' : '↓' }}
              </span>
            </th>
            <th v-if="canManage" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              Actions
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="container in containers" :key="container.containerId" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap">
              <input
                type="checkbox"
                :value="container.containerId"
                :checked="selectedContainers.includes(container.containerId)"
                @change="$emit('toggle-select', container.containerId)"
                class="rounded border-gray-300 text-blue-600"
              >
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm font-medium text-gray-900">{{ container.containerId }}</div>
              <div v-if="container.cargoDescription" class="text-xs text-gray-500 truncate max-w-xs">
                {{ container.cargoDescription }}
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ container.cargoType }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span 
                class="inline-flex px-2 py-1 text-xs font-semibold rounded-full"
                :class="getStatusBadgeClass(container.status)"
              >
                {{ container.status }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ container.currentLocation }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ container.shipName || '-' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ formatDate(container.updatedAt) }}
            </td>
            <td v-if="canManage" class="px-6 py-4 whitespace-nowrap text-sm font-medium text-right">
              <div class="flex justify-end gap-2">
                <button
                  @click="$emit('view', container)"
                  class="action-btn view-btn"
                  title="View Details"
                >
                  <i class="fas fa-eye"></i>
                </button>
                <button
                  @click="$emit('edit', container)"
                  class="action-btn edit-btn"
                  title="Edit Container"
                >
                  <i class="fas fa-edit"></i>
                </button>
                <button
                  @click="$emit('delete', container)"
                  class="action-btn delete-btn"
                  title="Delete Container"
                >
                  <i class="fas fa-trash-alt"></i>
                </button>
              </div>
            </td>
          </tr>
          <!-- Empty State -->
          <tr v-if="containers.length === 0">
            <td :colspan="canManage ? 8 : 7" class="px-6 py-10 text-center text-gray-500">
              No containers found. Try adjusting your filters.
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    
    <!-- Pagination -->
    <div class="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200">
      <div class="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
        <div>
          <p class="text-sm text-gray-700">
            Showing page <span class="font-medium">{{ pagination.page }}</span> of 
            <span class="font-medium">{{ pagination.totalPages || 1 }}</span>
          </p>
        </div>
        <div>
          <nav class="relative z-0 inline-flex rounded-md shadow-sm -space-x-px">
            <button
              @click="$emit('previous-page')"
              :disabled="!pagination.hasPreviousPage"
              class="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
            >
              <ChevronLeftIcon class="h-5 w-5" />
            </button>
            <button
              @click="$emit('next-page')"
              :disabled="!pagination.hasNextPage"
              class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
            >
              <ChevronRightIcon class="h-5 w-5" />
            </button>
          </nav>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { ChevronLeftIcon, ChevronRightIcon } from 'lucide-vue-next';

const props = defineProps<{
  containers: any[];
  pagination: any;
  selectedContainers: string[];
  currentSort: { field: string; direction: string };
  canManage: boolean;
  filters: any;
}>();

const emit = defineEmits<{
  'toggle-select-all': [];
  'toggle-select': [containerId: string];
  'sort': [field: string];
  'view': [container: any];
  'edit': [container: any];
  'delete': [container: any];
  'update-page-size': [size: number];
  'next-page': [];
  'previous-page': [];
}>();

const allSelected = computed(() => 
  props.containers.length > 0 && props.selectedContainers.length === props.containers.length
);

const someSelected = computed(() => 
  props.selectedContainers.length > 0 && props.selectedContainers.length < props.containers.length
);

const paginationInfo = computed(() => {
  const start = (props.pagination.page - 1) * props.pagination.pageSize + 1;
  const end = Math.min(start + props.pagination.pageSize - 1, props.pagination.totalCount || 0);
  return { 
    start: props.pagination.totalCount ? start : 0, 
    end: props.pagination.totalCount ? end : 0, 
    total: props.pagination.totalCount || 0 
  };
});

const getStatusBadgeClass = (status: string): string => {
  const statusClasses = {
    'Available': 'bg-green-100 text-green-800',
    'In Transit': 'bg-yellow-100 text-yellow-800',
    'At Port': 'bg-blue-100 text-blue-800',
    'Loading': 'bg-purple-100 text-purple-800',
    'Loaded': 'bg-indigo-100 text-indigo-800',
    'Unloading': 'bg-orange-100 text-orange-800',
    'In Storage': 'bg-gray-100 text-gray-800',
    'Maintenance': 'bg-red-100 text-red-800',
    'Customs Hold': 'bg-pink-100 text-pink-800'
  };
  
  return statusClasses[status as keyof typeof statusClasses] || 'bg-gray-100 text-gray-800';
};

const formatDate = (dateString: string): string => {
  if (!dateString) return '-';
  
  try {
    const date = new Date(dateString);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { 
      hour: '2-digit', 
      minute: '2-digit' 
    });
  } catch (error) {
    return dateString;
  }
};
</script>

<style scoped>
.action-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
  font-size: 14px;
}

.view-btn {
  background: #e0f2fe;
  color: #0284c7;
}

.view-btn:hover {
  background: #bae6fd;
  color: #0369a1;
  transform: translateY(-1px);
}

.edit-btn {
  background: #dbeafe;
  color: #1d4ed8;
}

.edit-btn:hover {
  background: #bfdbfe;
  color: #1e40af;
  transform: translateY(-1px);
}

.delete-btn {
  background: #fee2e2;
  color: #dc2626;
}

.delete-btn:hover {
  background: #fecaca;
  color: #b91c1c;
  transform: translateY(-1px);
}
</style>
