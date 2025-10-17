<template>
  <div class="berth-management">
    <!-- Page Header -->
    <div class="mb-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900">Berth Operations</h1>
          <p class="text-gray-600 mt-1">Manage berth assignments and operations</p>
        </div>
        <div class="flex space-x-3">
          <button
            @click="refreshData"
            :disabled="isLoading"
            class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
          >
            <Loader2Icon v-if="isLoading" class="w-4 h-4 mr-2 animate-spin" />
            Refresh
          </button>
          <button
            v-if="canManageBerths"
            @click="showCreateModal = true"
            class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
          >
            <Plus class="w-4 h-4 mr-2" />
            Create Berth
          </button>
        </div>
      </div>
    </div>

    <!-- Statistics Component -->
    <BerthStats :stats="displayStats" />

    <!-- Filters Component -->
    <BerthFilters
      v-model:filters="filters"
      :port-options="portOptions"
      :status-options="statusOptions"
      :type-options="typeOptions"
      @apply="applyFilters"
      @clear="clearFilters"
    />

    <!-- View Controls -->
    <div class="flex items-center justify-between mb-4 bg-white p-4 rounded-lg shadow-sm">
      <div class="flex items-center space-x-4">
        <div class="flex items-center space-x-2">
          <label class="text-sm font-medium text-gray-700">Show:</label>
          <select
            v-model.number="pageSize"
            @change="changePageSize"
            class="px-3 py-1.5 border border-gray-300 rounded-md text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          >
            <option :value="10">10</option>
            <option :value="25">25</option>
            <option :value="50">50</option>
            <option :value="100">100</option>
          </select>
          <span class="text-sm text-gray-600">entries</span>
        </div>
        <div class="text-sm text-gray-600">
          Showing {{ startIndex + 1 }} to {{ Math.min(endIndex, filteredBerths.length) }} of {{ filteredBerths.length }} berths
        </div>
      </div>
      
      <div class="flex items-center space-x-2">
        <label class="text-sm font-medium text-gray-700">View:</label>
        <div class="inline-flex rounded-md shadow-sm">
          <button
            @click="viewMode = 'grid'"
            :class="[
              'px-3 py-2 text-sm font-medium rounded-l-md border',
              viewMode === 'grid'
                ? 'bg-blue-600 text-white border-blue-600'
                : 'bg-white text-gray-700 border-gray-300 hover:bg-gray-50'
            ]"
          >
            <LayoutGrid class="w-4 h-4" />
          </button>
          <button
            @click="viewMode = 'table'"
            :class="[
              'px-3 py-2 text-sm font-medium rounded-r-md border-t border-r border-b',
              viewMode === 'table'
                ? 'bg-blue-600 text-white border-blue-600'
                : 'bg-white text-gray-700 border-gray-300 hover:bg-gray-50'
            ]"
          >
            <List class="w-4 h-4" />
          </button>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="flex justify-center items-center py-12">
      <Loader2Icon class="w-12 h-12 text-blue-600 animate-spin" />
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-md p-4 mb-6">
      <div class="flex">
        <div class="ml-3">
          <h3 class="text-sm font-medium text-red-800">
            Error Loading Berths
          </h3>
          <div class="mt-2 text-sm text-red-700">
            <p>{{ error }}</p>
          </div>
          <div class="mt-4">
            <button
              @click="loadBerths"
              class="bg-red-100 px-2 py-1.5 rounded-md text-sm font-medium text-red-800 hover:bg-red-200"
            >
              Try Again
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Berths Grid Display -->
    <div v-else-if="viewMode === 'grid'" class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
      <BerthCard 
        v-for="berth in paginatedBerths" 
        :key="berth.berthId" 
        :berth="berth"
        @edit="editBerth"
        @view="viewBerth"
        @assignments="assignBerth"
        @delete="confirmDeleteBerth"
      />
    </div>

    <!-- Berths Table Display -->
    <div v-else-if="viewMode === 'table'" class="bg-white shadow-md rounded-lg overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Berth</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Port</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Type</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Capacity</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Utilization</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="berth in paginatedBerths" :key="berth.berthId" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="flex items-center">
                <div>
                  <div class="text-sm font-medium text-gray-900">{{ berth.name }}</div>
                  <div class="text-sm text-gray-500">{{ berth.identifier || `B${berth.berthId}` }}</div>
                </div>
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ berth.portName || 'N/A' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ berth.type || 'N/A' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="getStatusBadgeClass(berth.status)" class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                {{ berth.status }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ berth.currentLoad || 0 }} / {{ berth.capacity }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="flex items-center">
                <div class="w-16 bg-gray-200 rounded-full h-2 mr-2">
                  <div :class="getUtilizationColor(getUtilization(berth))" class="h-2 rounded-full" :style="{ width: `${Math.min(getUtilization(berth), 100)}%` }"></div>
                </div>
                <span class="text-sm text-gray-900">{{ getUtilization(berth) }}%</span>
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <button @click="viewBerth(berth)" class="text-blue-600 hover:text-blue-900 mr-3">View</button>
              <button @click="editBerth(berth)" class="text-indigo-600 hover:text-indigo-900 mr-3">Edit</button>
              <button @click="assignBerth(berth)" class="text-green-600 hover:text-green-900 mr-3">Assign</button>
              <button @click="confirmDeleteBerth(berth)" class="text-red-600 hover:text-red-900">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Empty State -->
    <div v-if="!isLoading && !error && paginatedBerths.length === 0" class="text-center py-12">
      <Anchor class="mx-auto h-12 w-12 text-gray-400" />
      <h3 class="mt-2 text-sm font-medium text-gray-900">No berths found</h3>
      <p class="mt-1 text-sm text-gray-500">Try adjusting your filters or create a new berth.</p>
      <div class="mt-6">
        <button
          @click="showCreateModal = true"
          class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
        >
          <Plus class="w-4 h-4 mr-2" />
          Create berth
        </button>
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="totalPages > 1" class="flex justify-center mt-8">
      <nav class="relative z-0 inline-flex rounded-md shadow-sm -space-x-px">
        <button
          @click="previousPage"
          :disabled="!hasPreviousPage"
          class="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <ChevronLeft class="h-5 w-5" />
        </button>
        
        <span class="relative inline-flex items-center px-4 py-2 border border-gray-300 bg-white text-sm font-medium text-gray-700">
          Page {{ currentPage }} of {{ totalPages }}
        </span>
        
        <button
          @click="nextPage"
          :disabled="!hasNextPage"
          class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <ChevronRight class="h-5 w-5" />
        </button>
      </nav>
    </div>

    <!-- Modal Component -->
    <BerthModal
      v-if="showCreateModal || showEditModal"
      :is-editing="showEditModal"
      :berth="berthForm"
      :status-options="statusOptions"
      :port-options="portOptions"
      :is-submitting="isSubmitting"
      @submit="handleFormSubmit"
      @cancel="closeModal"
    />

    <!-- View Berth Details Modal -->
    <div v-if="showViewModal" class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity z-50" @click="closeModal">
      <div class="fixed inset-0 z-50 overflow-y-auto">
        <div class="flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0">
          <div @click.stop class="relative transform overflow-hidden rounded-lg bg-white text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-3xl">
            <div class="bg-white px-4 pb-4 pt-5 sm:p-6 sm:pb-4">
              <div class="sm:flex sm:items-start">
                <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left w-full">
                  <h3 class="text-lg font-semibold leading-6 text-gray-900 mb-4">Berth Details</h3>
                  
                  <div class="mt-4 grid grid-cols-2 gap-4">
                    <!-- Basic Information -->
                    <div class="col-span-2 border-b pb-2 mb-2">
                      <h4 class="text-sm font-semibold text-gray-700">Basic Information</h4>
                    </div>
                    
                    <div>
                      <label class="block text-sm font-medium text-gray-500">Name</label>
                      <p class="mt-1 text-sm text-gray-900">{{ berthForm.name }}</p>
                    </div>
                    
                    <div>
                      <label class="block text-sm font-medium text-gray-500">Identifier</label>
                      <p class="mt-1 text-sm text-gray-900">{{ berthForm.identifier || 'N/A' }}</p>
                    </div>
                    
                    <div>
                      <label class="block text-sm font-medium text-gray-500">Port</label>
                      <p class="mt-1 text-sm text-gray-900">{{ berthForm.portName || 'N/A' }}</p>
                    </div>
                    
                    <div>
                      <label class="block text-sm font-medium text-gray-500">Type</label>
                      <p class="mt-1 text-sm text-gray-900">{{ berthForm.type || 'N/A' }}</p>
                    </div>
                    
                    <div>
                      <label class="block text-sm font-medium text-gray-500">Status</label>
                      <span :class="getStatusBadgeClass(berthForm.status || '')" class="mt-1 px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                        {{ berthForm.status }}
                      </span>
                    </div>
                    
                    <div>
                      <label class="block text-sm font-medium text-gray-500">Priority</label>
                      <p class="mt-1 text-sm text-gray-900">{{ berthForm.priority || 'N/A' }}</p>
                    </div>
                    
                    <!-- Capacity Information -->
                    <div class="col-span-2 border-b pb-2 mb-2 mt-4">
                      <h4 class="text-sm font-semibold text-gray-700">Capacity Information</h4>
                    </div>
                    
                    <div>
                      <label class="block text-sm font-medium text-gray-500">Total Capacity</label>
                      <p class="mt-1 text-sm text-gray-900">{{ berthForm.capacity }} units</p>
                    </div>
                    
                    <div>
                      <label class="block text-sm font-medium text-gray-500">Current Load</label>
                      <p class="mt-1 text-sm text-gray-900">{{ berthForm.currentLoad || 0 }} units</p>
                    </div>
                    
                    <div class="col-span-2">
                      <label class="block text-sm font-medium text-gray-500">Utilization</label>
                      <div class="mt-2 relative">
                        <div class="overflow-hidden h-4 text-xs flex rounded bg-gray-200">
                          <div 
                            :class="getUtilizationColor(getUtilization(berthForm as Berth))" 
                            class="shadow-none flex flex-col text-center whitespace-nowrap text-white justify-center transition-all duration-500"
                            :style="{ width: `${Math.min(getUtilization(berthForm as Berth), 100)}%` }"
                          ></div>
                        </div>
                        <p class="mt-1 text-sm text-gray-600">{{ getUtilization(berthForm as Berth) }}%</p>
                      </div>
                    </div>
                    
                    <!-- Technical Specifications -->
                    <div class="col-span-2 border-b pb-2 mb-2 mt-4">
                      <h4 class="text-sm font-semibold text-gray-700">Technical Specifications</h4>
                    </div>
                    
                    <div>
                      <label class="block text-sm font-medium text-gray-500">Max Ship Length</label>
                      <p class="mt-1 text-sm text-gray-900">{{ berthForm.maxShipLength ? `${berthForm.maxShipLength} m` : 'N/A' }}</p>
                    </div>
                    
                    <div>
                      <label class="block text-sm font-medium text-gray-500">Max Draft</label>
                      <p class="mt-1 text-sm text-gray-900">{{ berthForm.maxDraft ? `${berthForm.maxDraft} m` : 'N/A' }}</p>
                    </div>
                    
                    <div>
                      <label class="block text-sm font-medium text-gray-500">Crane Count</label>
                      <p class="mt-1 text-sm text-gray-900">{{ berthForm.craneCount || 'N/A' }}</p>
                    </div>
                    
                    <div>
                      <label class="block text-sm font-medium text-gray-500">Hourly Rate</label>
                      <p class="mt-1 text-sm text-gray-900">{{ berthForm.hourlyRate ? `$${berthForm.hourlyRate}` : 'N/A' }}</p>
                    </div>
                    
                    <!-- Services -->
                    <div class="col-span-2 mt-4">
                      <label class="block text-sm font-medium text-gray-500">Available Services</label>
                      <p class="mt-1 text-sm text-gray-900">{{ berthForm.availableServices || 'N/A' }}</p>
                    </div>
                    
                    <!-- Notes -->
                    <div v-if="berthForm.notes" class="col-span-2 mt-4">
                      <label class="block text-sm font-medium text-gray-500">Notes</label>
                      <p class="mt-1 text-sm text-gray-900">{{ berthForm.notes }}</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="bg-gray-50 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6">
              <button
                @click="editBerth(berthForm as Berth)"
                type="button"
                class="inline-flex w-full justify-center rounded-md bg-blue-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-blue-500 sm:ml-3 sm:w-auto"
              >
                Edit Berth
              </button>
              <button
                @click="closeModal"
                type="button"
                class="mt-3 inline-flex w-full justify-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50 sm:mt-0 sm:w-auto"
              >
                Close
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Assignment Modal -->
    <BerthAssignmentModal
      v-if="showAssignmentModal"
      :berth="berthToAssign"
      :is-submitting="isSubmitting"
      @submit="handleAssignmentSubmit"
      @cancel="closeModal"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { Loader2Icon, Anchor, Plus, ChevronLeft, ChevronRight, LayoutGrid, List } from 'lucide-vue-next';

// Import child components from berths subfolder
import BerthStats from '../berths/BerthStats.vue';
import BerthFilters from '../berths/BerthFilters.vue';
import BerthCard from '../berths/BerthCard.vue';
import BerthModal from '../berths/BerthModal.vue';
import BerthAssignmentModal from '../berths/BerthAssignmentModal.vue';

// Import services and types
import { berthApi } from '../../services/berthApi';
import { berthAssignmentApi } from '../../services/berthAssignmentApi';
import { portService } from '../../services/portService';
import type { 
  Berth, 
  BerthCreateUpdate,
  BerthFilters as BerthFiltersType,
  BerthStats as BerthStatsType,
  PaginatedResponse
} from '../../types/berth';

// Reactive data
const paginatedData = ref<PaginatedResponse<Berth>>({
  data: [],
  totalCount: 0,
  page: 1,
  pageSize: 25,
  totalPages: 0,
  hasNextPage: false,
  hasPreviousPage: false
});

const stats = ref<BerthStatsType>({
  totalBerths: 0,
  activeBerths: 0,
  availableBerths: 0,
  totalCapacity: 0,
  currentOccupancy: 0,
  statusCounts: {},
  typeCounts: {},
  portCounts: {}
});

const portOptions = ref<Array<{ id: number; name: string }>>([]);

// Filters
const filters = ref<BerthFiltersType>({
  page: 1,
  pageSize: 25,
  sortBy: 'updatedAt',
  sortDirection: 'desc',
  searchTerm: '',
  status: '',
  type: '',
  portId: '',
  minCapacity: '',
  maxCapacity: ''
});

// Component state
const isLoading = ref(false);
const error = ref<string | null>(null);
const isSubmitting = ref(false);
const showCreateModal = ref(false);
const showEditModal = ref(false);
const showViewModal = ref(false);
const showDeleteModal = ref(false);
const showAssignmentModal = ref(false);
const berthForm = ref<Partial<Berth & BerthCreateUpdate>>({});
const viewMode = ref<'grid' | 'table'>('grid');
const pageSize = ref(25);
const currentPage = ref(1);
const allBerths = ref<Berth[]>([]);
const berthToDelete = ref<Berth | null>(null);
const berthToAssign = ref<Berth | null>(null);

// Filter options based on berth model
const statusOptions = [
  'Available', 'Occupied', 'Reserved', 'Under Maintenance', 'Out of Service'
];

const typeOptions = [
  'Container', 'Bulk', 'Tanker', 'RoRo', 'Multipurpose', 'General Cargo'
];

// Computed properties
const canManageBerths = computed(() => {
  const currentUser = localStorage.getItem('current_user');
  if (currentUser) {
    try {
      const user = JSON.parse(currentUser);
      return user.roles && (
        user.roles.includes('Admin') || 
        user.roles.includes('PortManager') || 
        user.roles.includes('Operator')
      );
    } catch (error) {
      return false;
    }
  }
  return false;
});

// Client-side filtering
const filteredBerths = computed(() => {
  let result = [...allBerths.value];
  
  // Apply status filter
  if (filters.value.status) {
    result = result.filter(b => b.status === filters.value.status);
  }
  
  // Apply type filter
  if (filters.value.type) {
    result = result.filter(b => b.type === filters.value.type);
  }
  
  // Apply port filter
  if (filters.value.portId) {
    result = result.filter(b => b.portId.toString() === filters.value.portId);
  }
  
  // Apply capacity filters
  if (filters.value.minCapacity) {
    const minCap = parseInt(filters.value.minCapacity);
    result = result.filter(b => b.capacity >= minCap);
  }
  
  if (filters.value.maxCapacity) {
    const maxCap = parseInt(filters.value.maxCapacity);
    result = result.filter(b => b.capacity <= maxCap);
  }
  
  // Apply search term
  if (filters.value.searchTerm) {
    const searchLower = filters.value.searchTerm.toLowerCase();
    result = result.filter(b => 
      b.name.toLowerCase().includes(searchLower) ||
      (b.identifier && b.identifier.toLowerCase().includes(searchLower))
    );
  }
  
  return result;
});

// Pagination
const paginatedBerths = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  const end = start + pageSize.value;
  return filteredBerths.value.slice(start, end);
});

const startIndex = computed(() => (currentPage.value - 1) * pageSize.value);
const endIndex = computed(() => startIndex.value + pageSize.value);

const totalPages = computed(() => Math.ceil(filteredBerths.value.length / pageSize.value));

const hasNextPage = computed(() => currentPage.value < totalPages.value);
const hasPreviousPage = computed(() => currentPage.value > 1);

// Stats for filtered berths
const displayStats = computed(() => {
  const berths = filteredBerths.value;
  const statusCounts: Record<string, number> = {};
  const typeCounts: Record<string, number> = {};
  const portCounts: Record<string, number> = {};
  
  let totalCapacity = 0;
  let currentOccupancy = 0;
  let availableCount = 0;
  let activeCount = 0;

  berths.forEach(berth => {
    const status = berth.status || 'Unknown';
    statusCounts[status] = (statusCounts[status] || 0) + 1;
    
    const type = berth.type || 'Unknown';
    typeCounts[type] = (typeCounts[type] || 0) + 1;
    
    const port = berth.portName || 'Unknown';
    portCounts[port] = (portCounts[port] || 0) + 1;
    
    totalCapacity += berth.capacity || 0;
    currentOccupancy += berth.currentLoad || 0;
    
    if (berth.status === 'Available') availableCount++;
    if (berth.status !== 'Under Maintenance' && berth.status !== 'Out of Service') activeCount++;
  });

  return {
    totalBerths: berths.length,
    activeBerths: activeCount,
    availableBerths: availableCount,
    totalCapacity,
    currentOccupancy,
    statusCounts,
    typeCounts,
    portCounts
  };
});

// Methods
const loadBerths = async () => {
  isLoading.value = true;
  error.value = null;
  
  try {
    if (berthApi && typeof berthApi.getAll === 'function') {
      const response = await berthApi.getAll();
      allBerths.value = response.data || [];
      currentPage.value = 1; // Reset to first page
    } else {
      allBerths.value = [];
    }
  } catch (err: any) {
    error.value = 'Failed to load berths. Please try again.';
    allBerths.value = [];
  } finally {
    isLoading.value = false;
  }
};

const calculateStatistics = (berths: Berth[]) => {
  // Calculate real statistics from berth data
  const statusCounts: Record<string, number> = {};
  const typeCounts: Record<string, number> = {};
  const portCounts: Record<string, number> = {};
  
  let totalCapacity = 0;
  let currentOccupancy = 0;
  let availableCount = 0;
  let activeCount = 0;

  berths.forEach(berth => {
    // Count by status
    const status = berth.status || 'Unknown';
    statusCounts[status] = (statusCounts[status] || 0) + 1;
    
    // Count by type
    const type = berth.type || 'Unknown';
    typeCounts[type] = (typeCounts[type] || 0) + 1;
    
    // Count by port
    const port = berth.portName || 'Unknown';
    portCounts[port] = (portCounts[port] || 0) + 1;
    
    // Calculate capacity metrics
    totalCapacity += berth.capacity || 0;
    currentOccupancy += berth.currentLoad || 0;
    
    // Count available berths
    if (berth.status === 'Available') {
      availableCount++;
    }
    
    // Count active berths (not in maintenance or out of service)
    if (berth.status !== 'Under Maintenance' && berth.status !== 'Out of Service') {
      activeCount++;
    }
  });

  stats.value = {
    totalBerths: berths.length,
    activeBerths: activeCount,
    availableBerths: availableCount,
    totalCapacity,
    currentOccupancy,
    statusCounts,
    typeCounts,
    portCounts
  };
};

const loadStatistics = async () => {
  try {
    // Calculate statistics from loaded berth data
    calculateStatistics(paginatedData.value.data);
  } catch (err) {
    error.value = 'Failed to load berth statistics.';
  }
};

const loadPorts = async () => {
  try {
    if (portService && typeof portService.getAll === 'function') {
      const response = await portService.getAll();
      
      portOptions.value = response.data
        .map(port => ({
          id: port.portId,
          name: port.name || `Port ${port.portId}`
        }))
        .sort((a, b) => a.name.localeCompare(b.name));
    } else {
      portOptions.value = [];
    }
  } catch (err) {
    console.error('Error loading ports:', err);
    portOptions.value = [];
  }
};

const refreshData = async () => {
  try {
    await Promise.all([
      loadBerths(),
      loadStatistics(),
      loadPorts()
    ]);
  } catch (error) {
    console.error('Error refreshing data:', error);
  }
};

const applyFilters = () => {
  currentPage.value = 1; // Reset to first page when applying filters
};

const clearFilters = () => {
  filters.value = {
    page: 1,
    pageSize: 25,
    sortBy: 'updatedAt',
    sortDirection: 'desc',
    searchTerm: '',
    status: '',
    type: '',
    portId: '',
    minCapacity: '',
    maxCapacity: ''
  };
  currentPage.value = 1;
};

const nextPage = () => {
  if (hasNextPage.value) {
    currentPage.value++;
  }
};

const previousPage = () => {
  if (hasPreviousPage.value) {
    currentPage.value--;
  }
};

const changePageSize = () => {
  currentPage.value = 1; // Reset to first page when changing page size
};

// Table view helper functions
const getUtilization = (berth: Berth): number => {
  if (!berth.capacity) return 0;
  return Math.round(((berth.currentLoad || 0) / berth.capacity) * 100);
};

const getUtilizationColor = (utilization: number): string => {
  if (utilization >= 90) return 'bg-red-500';
  if (utilization >= 75) return 'bg-yellow-500';
  if (utilization >= 50) return 'bg-blue-500';
  return 'bg-green-500';
};

const getStatusBadgeClass = (status: string): string => {
  const statusMap: Record<string, string> = {
    'Available': 'bg-green-100 text-green-800',
    'Occupied': 'bg-yellow-100 text-yellow-800',
    'Under Maintenance': 'bg-red-100 text-red-800',
    'Reserved': 'bg-blue-100 text-blue-800',
    'Out of Service': 'bg-gray-100 text-gray-800'
  };
  return statusMap[status] || 'bg-gray-100 text-gray-800';
};

// Delete confirmation
const confirmDeleteBerth = (berth: Berth) => {
  if (confirm(`Are you sure you want to delete berth "${berth.name}"?\n\nThis action cannot be undone.`)) {
    deleteBerth(berth);
  }
};

const deleteBerth = async (berth: Berth) => {
  try {
    if (berthApi && typeof berthApi.delete === 'function') {
      await berthApi.delete(berth.berthId);
      await loadBerths();
      alert(`Berth "${berth.name}" has been successfully deleted.`);
    }
  } catch (err: any) {
    console.error('Delete error:', err);
    alert(`Failed to delete berth: ${err.message || 'Please try again later.'}`);
  }
};

const exportBerths = async () => {
  try {
    // Export functionality not implemented yet
    alert('Export functionality will be available soon.');
  } catch (err) {
    alert('Export failed. Please try again later.');
  }
};

const viewBerth = (berth: Berth) => {
  berthForm.value = { ...berth };
  showViewModal.value = true;
};

const editBerth = (berth: Berth) => {
  berthForm.value = { ...berth };
  showEditModal.value = true;
};

const assignBerth = (berth: Berth) => {
  berthToAssign.value = berth;
  showAssignmentModal.value = true;
};

const handleAssignmentSubmit = async (assignmentData: any) => {
  isSubmitting.value = true;
  
  try {
    // Convert priority number to string: 1 -> High, 2 -> Medium, 3 -> Low
    const priorityMap: { [key: number]: number } = {
      1: 1, // High
      2: 2, // Medium
      3: 3  // Low
    };
    
    // Prepare the payload matching backend BerthAssignmentCreateUpdateDto
    const payload = {
      berthId: assignmentData.berthId,
      assignmentType: assignmentData.assignmentType,
      status: 'Scheduled', // Default status for new assignments
      priority: assignmentData.priority ? priorityMap[assignmentData.priority] : undefined,
      scheduledArrival: assignmentData.startDate,
      scheduledDeparture: assignmentData.endDate,
      containerCount: assignmentData.capacityRequired,
      notes: assignmentData.notes || null,
      // Set shipId or containerId based on assignment type
      ...(assignmentData.assignmentType === 'Ship' 
        ? { shipId: assignmentData.shipId } 
        : { containerId: assignmentData.containerNumber }
      )
    };
    
    // Call the actual backend API
    const result = await berthAssignmentApi.create(payload);
    
    // Show success message with assignment ID
    alert(`Berth "${berthToAssign.value?.name}" has been assigned successfully! Assignment ID: ${result.data.id}`);
    
    // Close modal and refresh data
    closeModal();
    await loadBerths();
  } catch (err: any) {
    console.error('Assignment error:', err);
    alert(`Failed to assign berth: ${err.message || 'Please try again later.'}`);
  } finally {
    isSubmitting.value = false;
  }
};

const handleFormSubmit = async (berthData: any) => {
  isSubmitting.value = true;
  
  try {
    const payload = {
      name: berthData.name,
      identifier: berthData.identifier || '',
      type: berthData.type || '',
      capacity: Number(berthData.capacity),
      status: berthData.status,
      portId: Number(berthData.portId),
      maxShipLength: berthData.maxShipLength ? Number(berthData.maxShipLength) : undefined,
      maxDraft: berthData.maxDraft ? Number(berthData.maxDraft) : undefined,
      availableServices: berthData.availableServices || '',
      craneCount: berthData.craneCount ? Number(berthData.craneCount) : undefined,
      hourlyRate: berthData.hourlyRate ? Number(berthData.hourlyRate) : undefined,
      priority: berthData.priority ? Number(berthData.priority) : undefined,
      notes: berthData.notes || ''
    };

    if (showCreateModal.value) {
      if (berthApi && typeof berthApi.create === 'function') {
        await berthApi.create(payload);
      }
    } else {
      if (berthApi && typeof berthApi.update === 'function') {
        await berthApi.update(berthForm.value.berthId as number, payload);
      }
    }
    
    await loadBerths();
    await loadStatistics();
    closeModal();
  } catch (err: any) {
    console.error('Form submission error:', err);
    alert(`Failed to save berth: ${err.message || 'Please try again later.'}`);
  } finally {
    isSubmitting.value = false;
  }
};

const closeModal = () => {
  showCreateModal.value = false;
  showEditModal.value = false;
  showViewModal.value = false;
  showDeleteModal.value = false;
  showAssignmentModal.value = false;
  berthForm.value = {};
  berthToAssign.value = null;
};

// Initialize data on component mount
onMounted(() => {
  refreshData();
});
</script>

<style scoped>
.berth-management {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px;
}

.animate-spin {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}
</style>