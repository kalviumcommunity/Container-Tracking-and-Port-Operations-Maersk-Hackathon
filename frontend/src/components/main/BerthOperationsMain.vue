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
    <BerthStats :stats="stats" />

    <!-- Filters Component -->
    <BerthFilters
      v-model:filters="filters"
      :port-options="portOptions"
      :status-options="statusOptions"
      :type-options="typeOptions"
      @apply="applyFilters"
      @clear="clearFilters"
    />

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
    <div v-else class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
      <BerthCard 
        v-for="berth in paginatedData.data" 
        :key="berth.berthId" 
        :berth="berth"
        @edit="editBerth"
        @view="viewBerth"
        @assign="assignBerth"
      />
    </div>

    <!-- Empty State -->
    <div v-if="!isLoading && !error && paginatedData.data.length === 0" class="text-center py-12">
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
    <div v-if="paginatedData.totalPages > 1" class="flex justify-center mt-8">
      <nav class="relative z-0 inline-flex rounded-md shadow-sm -space-x-px">
        <button
          @click="previousPage"
          :disabled="!paginatedData.hasPreviousPage"
          class="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
        >
          <ChevronLeft class="h-5 w-5" />
        </button>
        
        <span class="relative inline-flex items-center px-4 py-2 border border-gray-300 bg-white text-sm font-medium text-gray-700">
          Page {{ paginatedData.page }} of {{ paginatedData.totalPages }}
        </span>
        
        <button
          @click="nextPage"
          :disabled="!paginatedData.hasNextPage"
          class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
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
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { Loader2Icon, Anchor, Plus, ChevronLeft, ChevronRight } from 'lucide-vue-next';

// Import child components from berths subfolder
import BerthStats from '../berths/BerthStats.vue';
import BerthFilters from '../berths/BerthFilters.vue';
import BerthCard from '../berths/BerthCard.vue';
import BerthModal from '../berths/BerthModal.vue';

// Import services and types
import { berthApi } from '../../services/berthApi';
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
  availableBerths: 0,
  occupiedBerths: 0,
  maintenanceBerths: 0,
  averageOccupancyRate: 0,
  totalRevenue: 0,
  activeBerths: 0,
  totalCapacity: 0,
  currentOccupancy: 0,
  statusCounts: {},
  averageUtilization: 0,
  berthsByStatus: {},
  berthsByType: {},
  berthsByPort: {},
  portCounts: {},
  utilizationRanges: {},
  featureCounts: {}
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
const berthForm = ref<Partial<Berth & BerthCreateUpdate>>({});

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

// Methods
const loadBerths = async () => {
  isLoading.value = true;
  error.value = null;
  
  try {
    if (berthApi && typeof berthApi.getAll === 'function') {
      const response = await berthApi.getAll();
      paginatedData.value = {
        data: response.data || [],
        totalCount: response.data?.length || 0,
        page: 1,
        pageSize: 25,
        totalPages: Math.ceil((response.data?.length || 0) / 25),
        hasNextPage: false,
        hasPreviousPage: false
      };
    } else {
      // Show empty state instead of mock data
      paginatedData.value = {
        data: [],
        totalCount: 0,
        page: 1,
        pageSize: 25,
        totalPages: 0,
        hasNextPage: false,
        hasPreviousPage: false
      };
    }
  } catch (err: any) {
    error.value = 'Failed to load berths. Please try again.';
  } finally {
    isLoading.value = false;
  }
};

const loadStatistics = async () => {
  try {
    // Mock statistics for now since we don't have a dedicated statistics endpoint
    stats.value = {
      totalBerths: 0,
      availableBerths: 0,
      occupiedBerths: 0,
      maintenanceBerths: 0,
      averageOccupancyRate: 0,
      totalRevenue: 0,
      activeBerths: 0,
      totalCapacity: 0,
      currentOccupancy: 0,
      statusCounts: {},
      averageUtilization: 0,
      berthsByStatus: {},
      berthsByType: {},
      berthsByPort: {},
      portCounts: {},
      utilizationRanges: {},
      featureCounts: {}
    };
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
  filters.value.page = 1;
  loadBerths();
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
  loadBerths();
};

const nextPage = async () => {
  if (paginatedData.value.hasNextPage) {
    filters.value.page = (filters.value.page || 1) + 1;
    await loadBerths();
  }
};

const previousPage = async () => {
  if (paginatedData.value.hasPreviousPage) {
    filters.value.page = Math.max(1, (filters.value.page || 1) - 1);
    await loadBerths();
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
  // TODO: Implement detailed view modal
  alert(`Viewing berth: ${berth.name}`);
};

const editBerth = (berth: Berth) => {
  berthForm.value = { ...berth };
  showEditModal.value = true;
};

const assignBerth = (berth: Berth) => {
  // TODO: Implement berth assignment functionality
  alert(`Assigning berth: ${berth.name}`);
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
  berthForm.value = {};
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