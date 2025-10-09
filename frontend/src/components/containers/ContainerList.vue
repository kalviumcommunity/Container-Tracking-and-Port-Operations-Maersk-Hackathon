<template>
  <div class="container-list">
    <!-- Container Table -->
    <div class="bg-white rounded-lg shadow-md overflow-hidden">
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
                  class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                >
              </th>
              <th 
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer"
                @click="$emit('sort', 'containerId')"
              >
                Container ID
                <span v-if="currentSort.field === 'containerId'">
                  {{ currentSort.direction === 'asc' ? '↑' : '↓' }}
                </span>
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Cargo Type
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Container Type
              </th>
              <th 
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer"
                @click="$emit('sort', 'status')"
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
                Destination
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Weight (kg)
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Ship
              </th>
              <th 
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer"
                @click="$emit('sort', 'updatedAt')"
              >
                Last Updated
                <span v-if="currentSort.field === 'updatedAt'">
                  {{ currentSort.direction === 'asc' ? '↑' : '↓' }}
                </span>
              </th>
              <th v-if="canManageContainers" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
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
                  class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                >
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900">{{ container.containerId }}</div>
                <div v-if="container.cargoDescription" class="text-xs text-gray-500">
                  {{ container.cargoDescription }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ container.cargoType }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
                  {{ container.type }}
                </span>
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
                {{ container.destination || '-' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ container.weight ? formatWeight(container.weight) : '-' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ container.shipName || '-' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatDate(container.updatedAt) }}
              </td>
              <td v-if="canManageContainers" class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <div class="flex space-x-2">
                  <button
                    @click="$emit('view', container)"
                    class="text-blue-600 hover:text-blue-900"
                  >
                    View
                  </button>
                  <button
                    @click="$emit('edit', container)"
                    class="text-indigo-600 hover:text-indigo-900"
                  >
                    Edit
                  </button>
                  <button
                    @click="$emit('delete', container)"
                    class="text-red-600 hover:text-red-900"
                  >
                    Delete
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200">
        <div class="flex-1 flex justify-between sm:hidden">
          <button
            @click="$emit('previous-page')"
            :disabled="!pagination.hasPreviousPage"
            class="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
          >
            Previous
          </button>
          <button
            @click="$emit('next-page')"
            :disabled="!pagination.hasNextPage"
            class="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
          >
            Next
          </button>
        </div>
        <div class="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
          <div>
            <p class="text-sm text-gray-700">
              Showing page <span class="font-medium">{{ pagination.page }}</span> of 
              <span class="font-medium">{{ pagination.totalPages }}</span>
            </p>
          </div>
          <div>
            <nav class="relative z-0 inline-flex rounded-md shadow-sm -space-x-px">
              <button
                @click="$emit('previous-page')"
                :disabled="!pagination.hasPreviousPage"
                class="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
              >
                Previous
              </button>
              <button
                @click="$emit('next-page')"
                :disabled="!pagination.hasNextPage"
                class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
              >
                Next
              </button>
            </nav>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { Container, ContainerFilters, PaginatedResponse } from '../../types/container';

interface Props {
  containers: Container[];
  pagination: PaginatedResponse<Container>;
  filters: ContainerFilters;
  selectedContainers: string[];
  currentSort: { field: string; direction: string };
  canManageContainers: boolean;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  'toggle-select-all': [];
  'toggle-select': [containerId: string];
  'sort': [field: string];
  'view': [container: Container];
  'edit': [container: Container];
  'delete': [container: Container];
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
  const end = Math.min(start + props.pagination.pageSize - 1, props.pagination.totalCount);
  return { start, end, total: props.pagination.totalCount };
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

const formatWeight = (weight: number): string => {
  if (!weight) return '-';
  return new Intl.NumberFormat('en-US').format(weight);
};
</script>
