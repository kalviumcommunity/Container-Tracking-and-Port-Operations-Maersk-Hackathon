<template>
  <div class="berth-list">
    <!-- Berth Table -->
    <div class="bg-white rounded-2xl shadow-lg overflow-hidden border border-slate-200">
      <div class="px-8 py-6 border-b border-slate-200">
        <div class="flex justify-between items-center">
          <div class="flex items-center space-x-5">
            <h2 class="text-2xl font-bold text-slate-900">Berths</h2>
            <span class="text-sm text-slate-600 bg-slate-100 px-3 py-1 rounded-full">
              {{ paginationInfo.start }}-{{ paginationInfo.end }} of {{ paginationInfo.total }}
            </span>
          </div>
          <div class="flex items-center space-x-3">
            <label class="text-sm font-medium text-slate-700">Show:</label>
            <select
              :value="filters.pageSize"
              @change="$emit('update-page-size', Number(($event.target as HTMLSelectElement).value))"
              class="px-3 py-2 border border-slate-300 rounded-lg text-sm font-medium bg-white hover:border-slate-400 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all"
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
        <table class="min-w-full divide-y divide-slate-200">
          <thead class="bg-slate-50">
            <tr>
              <th class="px-6 py-4 text-left">
                <input
                  type="checkbox"
                  :checked="allSelected"
                  :indeterminate="someSelected"
                  @change="$emit('toggle-select-all')"
                  class="rounded border-slate-300 text-blue-600 focus:ring-blue-500 w-4 h-4"
                >
              </th>
              <th 
                class="px-6 py-4 text-left text-xs font-bold text-slate-700 uppercase tracking-wider cursor-pointer hover:text-slate-900 transition-colors"
                @click="$emit('sort', 'name')"
              >
                Berth Name
                <span v-if="currentSort.field === 'name'" class="ml-1">
                  {{ currentSort.direction === 'asc' ? '↑' : '↓' }}
                </span>
              </th>
              <th class="px-6 py-4 text-left text-xs font-bold text-slate-700 uppercase tracking-wider">
                Port
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
                Type
              </th>
              <th 
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer"
                @click="$emit('sort', 'capacity')"
              >
                Capacity
                <span v-if="currentSort.field === 'capacity'">
                  {{ currentSort.direction === 'asc' ? '↑' : '↓' }}
                </span>
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Utilization
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Length (m)
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Depth (m)
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Operating Hours
              </th>
              <th v-if="canManageBerths" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="berth in berths" :key="berth.berthId" class="hover:bg-gray-50">
              <td class="px-6 py-4 whitespace-nowrap">
                <input
                  type="checkbox"
                  :value="berth.berthId"
                  :checked="selectedBerths.includes(berth.berthId)"
                  @change="$emit('toggle-select', berth.berthId)"
                  class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                >
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900">{{ berth.name }}</div>
                <div v-if="berth.notes" class="text-xs text-gray-500 truncate max-w-xs">
                  {{ berth.notes }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                <div class="flex items-center">
                  <MapPin class="w-4 h-4 mr-1 text-gray-400" />
                  {{ berth.portName }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span 
                  class="inline-flex px-2 py-1 text-xs font-semibold rounded-full"
                  :class="getStatusBadgeClass(berth.status)"
                >
                  {{ berth.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                <span v-if="berth.berthType" class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
                  {{ berth.berthType }}
                </span>
                <span v-else class="text-gray-400">-</span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                <div class="flex items-center">
                  <Package class="w-4 h-4 mr-1 text-gray-400" />
                  {{ formatCapacity(berth.capacity) }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="flex-1">
                    <div class="text-sm font-medium text-gray-900">
                      {{ berth.currentOccupancy || 0 }}/{{ berth.capacity }}
                    </div>
                    <div class="w-full bg-gray-200 rounded-full h-2">
                      <div 
                        class="h-2 rounded-full"
                        :class="getUtilizationBarClass(berth.currentOccupancy || 0, berth.capacity)"
                        :style="{ width: `${Math.min(((berth.currentOccupancy || 0) / berth.capacity) * 100, 100)}%` }"
                      ></div>
                    </div>
                  </div>
                  <div class="ml-2 text-xs text-gray-500">
                    {{ Math.round(((berth.currentOccupancy || 0) / berth.capacity) * 100) }}%
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ berth.length ? berth.length + 'm' : '-' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ berth.waterDepth ? berth.waterDepth + 'm' : '-' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                <div class="flex items-center">
                  <Clock class="w-4 h-4 mr-1 text-gray-400" />
                  {{ berth.operatingHours || '24/7' }}
                </div>
              </td>
              <td v-if="canManageBerths" class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <div class="flex space-x-2">
                  <button
                    @click="$emit('view', berth)"
                    class="text-blue-600 hover:text-blue-900"
                    title="View Details"
                  >
                    <Eye class="w-4 h-4" />
                  </button>
                  <button
                    @click="$emit('edit', berth)"
                    class="text-indigo-600 hover:text-indigo-900"
                    title="Edit Berth"
                  >
                    <Edit class="w-4 h-4" />
                  </button>
                  <button
                    @click="$emit('assignments', berth)"
                    class="text-green-600 hover:text-green-900"
                    title="Manage Assignments"
                  >
                    <Users class="w-4 h-4" />
                  </button>
                  <button
                    @click="$emit('delete', berth)"
                    class="text-red-600 hover:text-red-900"
                    title="Delete Berth"
                  >
                    <Trash2 class="w-4 h-4" />
                  </button>
                </div>
              </td>
            </tr>
            <!-- Empty State -->
            <tr v-if="berths.length === 0">
              <td :colspan="canManageBerths ? 11 : 10" class="px-6 py-10 text-center text-gray-500">
                <div class="flex flex-col items-center">
                  <MapPin class="w-12 h-12 text-gray-300 mb-3" />
                  <p class="text-lg font-medium">No berths found</p>
                  <p class="text-sm">Try adjusting your filters or create a new berth.</p>
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
                <ChevronLeft class="h-5 w-5" />
              </button>
              <button
                @click="$emit('next-page')"
                :disabled="!pagination.hasNextPage"
                class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
              >
                <ChevronRight class="h-5 w-5" />
              </button>
            </nav>
          </div>
        </div>
      </div>
    </div>

    <!-- Bulk Actions Bar (when berths are selected) -->
    <div v-if="selectedBerths.length > 0" class="mt-4 bg-blue-50 border border-blue-200 rounded-lg p-4">
      <div class="flex items-center justify-between">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <Package class="h-5 w-5 text-blue-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm font-medium text-blue-900">
              {{ selectedBerths.length }} berth{{ selectedBerths.length === 1 ? '' : 's' }} selected
            </p>
          </div>
        </div>
        <div class="flex space-x-2">
          <button
            @click="$emit('bulk-status-change')"
            class="inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-blue-700 bg-blue-100 hover:bg-blue-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            Change Status
          </button>
          <button
            @click="$emit('bulk-export')"
            class="inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-blue-700 bg-blue-100 hover:bg-blue-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            Export Selected
          </button>
          <button
            @click="$emit('clear-selection')"
            class="inline-flex items-center px-3 py-2 border border-gray-300 text-sm leading-4 font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            Clear
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { 
  MapPin, 
  Package, 
  Clock, 
  Eye, 
  Edit, 
  Users, 
  Trash2, 
  ChevronLeft, 
  ChevronRight 
} from 'lucide-vue-next';

interface Berth {
  berthId: number;
  name: string;
  portName: string;
  status: string;
  berthType?: string;
  capacity: number;
  currentOccupancy?: number;
  length?: number;
  waterDepth?: number;
  operatingHours?: string;
  notes?: string;
  activeAssignmentCount?: number;
}

interface BerthFilters {
  pageSize: number;
  [key: string]: any;
}

interface PaginatedResponse {
  page: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  totalCount: number;
  pageSize: number;
}

interface Props {
  berths: Berth[];
  pagination: PaginatedResponse;
  filters: BerthFilters;
  selectedBerths: number[];
  currentSort: { field: string; direction: string };
  canManageBerths: boolean;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  'toggle-select-all': [];
  'toggle-select': [berthId: number];
  'sort': [field: string];
  'view': [berth: Berth];
  'edit': [berth: Berth];
  'assignments': [berth: Berth];
  'delete': [berth: Berth];
  'update-page-size': [size: number];
  'next-page': [];
  'previous-page': [];
  'bulk-status-change': [];
  'bulk-export': [];
  'clear-selection': [];
}>();

const allSelected = computed(() => 
  props.berths.length > 0 && props.selectedBerths.length === props.berths.length
);

const someSelected = computed(() => 
  props.selectedBerths.length > 0 && props.selectedBerths.length < props.berths.length
);

const paginationInfo = computed(() => {
  const start = (props.pagination.page - 1) * props.pagination.pageSize + 1;
  const end = Math.min(start + props.pagination.pageSize - 1, props.pagination.totalCount);
  return { start, end, total: props.pagination.totalCount };
});

const getStatusBadgeClass = (status: string): string => {
  const statusClasses = {
    'Available': 'bg-green-100 text-green-800',
    'Occupied': 'bg-yellow-100 text-yellow-800',
    'Under Maintenance': 'bg-red-100 text-red-800',
    'Reserved': 'bg-blue-100 text-blue-800',
    'Full': 'bg-purple-100 text-purple-800',
    'Partially Occupied': 'bg-orange-100 text-orange-800',
    'Inactive': 'bg-gray-100 text-gray-800',
    'Emergency': 'bg-red-200 text-red-900'
  };
  
  return statusClasses[status as keyof typeof statusClasses] || 'bg-gray-100 text-gray-800';
};

const getUtilizationBarClass = (current: number, capacity: number): string => {
  const percentage = (current / capacity) * 100;
  
  if (percentage >= 90) return 'bg-red-500';
  if (percentage >= 75) return 'bg-yellow-500';
  if (percentage >= 50) return 'bg-blue-500';
  return 'bg-green-500';
};

const formatCapacity = (capacity: number): string => {
  if (!capacity) return '-';
  return new Intl.NumberFormat('en-US').format(capacity);
};
</script>
