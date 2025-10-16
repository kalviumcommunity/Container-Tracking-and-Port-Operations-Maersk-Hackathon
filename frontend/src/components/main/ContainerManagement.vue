<template>
  <div class="container-management">
    <!-- Header Component -->
    <ContainerHeader 
      :is-loading="isLoading"
      :can-manage="canManageContainers"
      @refresh="refreshData"
      @export="exportContainers" 
      @create="showCreateModal = true"
    />

    <!-- Statistics Component -->
    <ContainerStats :stats="stats" />

    <!-- Filters Component -->
    <ContainerFilters
      v-model:filters="filters"
      :location-options="locationOptions"
      :ship-options="shipOptions"
      :status-options="statusOptions"
      :type-options="typeOptions"
      :cargo-type-options="cargoTypeOptions"
      @apply="applyFilters"
      @clear="clearFilters"
    />

    <!-- Bulk Actions Component -->
    <ContainerBulkActions
      v-if="selectedContainers.length > 0"
      :selected-count="selectedContainers.length"
      :status-options="statusOptions"
      @bulk-update="performBulkStatusUpdate"
      @clear-selection="clearSelection"
    />

    <!-- Loading State -->
    <div v-if="isLoading" class="flex justify-center items-center py-12">
      <Loader2Icon class="w-12 h-12 text-blue-600 animate-spin" />
    </div>

    <!-- Error State -->
    <ContainerError 
      v-else-if="error" 
      :error="error" 
      @retry="loadContainers" 
    />

    <!-- Main Table Component -->
    <ContainerTable
      v-else
      :containers="paginatedData.data"
      :pagination="paginatedData"
      :selected-containers="selectedContainers"
      :current-sort="currentSort"
      :can-manage="canManageContainers"
      :filters="filters"
      @toggle-select-all="toggleSelectAll"
      @toggle-select="toggleSelect"
      @sort="sortBy"
      @view="viewContainer"
      @edit="editContainer"
      @delete="deleteContainer"
      @update-page-size="updatePageSize"
      @next-page="nextPage"
      @previous-page="previousPage"
    />

    <!-- Modal Component -->
    <ContainerModal
      v-if="showCreateModal || showEditModal"
      :is-editing="showEditModal"
      :container="containerForm"
      :cargo-type-options="cargoTypeOptions"
      :type-options="typeOptions"
      :status-options="statusOptions"
      :ship-options="shipOptions"
      :is-submitting="isSubmitting"
      @submit="handleFormSubmit"
      @cancel="closeModal"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { Loader2Icon } from 'lucide-vue-next';

// Import child components from containers subfolder
import ContainerHeader from '../containers/ContainerHeader.vue';
import ContainerStats from '../containers/ContainerStats.vue';
import ContainerFilters from '../containers/ContainerFilters.vue';
import ContainerBulkActions from '../containers/ContainerBulkActions.vue';
import ContainerError from '../containers/ContainerError.vue';
import ContainerTable from '../containers/ContainerTable.vue';
import ContainerModal from '../containers/ContainerModal.vue';

// Import services and types
import { containerService } from '../../services/containerService';
import { portService } from '../../services/portService';
import { shipService } from '../../services/shipService';
import type { 
  Container, 
  ContainerCreateRequest,
  ContainerUpdateRequest,
  ContainerFilters as ContainerFiltersType,
  ContainerStats as ContainerStatsType,
  PaginatedResponse,
  BulkStatusUpdate 
} from '../../types/container';

// Reactive data
const paginatedData = ref<PaginatedResponse<Container>>({
  data: [],
  totalCount: 0,
  page: 1,
  pageSize: 25,
  totalPages: 0,
  hasNextPage: false,
  hasPreviousPage: false
});

const stats = ref<ContainerStatsType>({
  totalContainers: 0,
  availableContainers: 0,
  inTransitContainers: 0,
  atPortContainers: 0,
  loadingContainers: 0,
  unloadingContainers: 0,
  containersByType: {},
  containersByStatus: {},
  containersByLocation: {}
});

// Add missing reactive variables for options
const locationOptions = ref<string[]>([]);
const shipOptions = ref<Array<{ id: number; name: string }>>([]);

// Filters
const filters = ref<ContainerFiltersType>({
  page: 1,
  pageSize: 25,
  sortBy: 'updatedAt',
  sortDirection: 'desc',
  searchTerm: '',
  status: '',
  type: '',
  cargoType: '',
  currentLocation: '',
  destination: '',
  shipId: '',
  createdAfter: '',
  createdBefore: '',
  minWeight: '',
  maxWeight: ''
});

// Component state
const isLoading = ref(false);
const error = ref<string | null>(null);
const isSubmitting = ref(false);
const showCreateModal = ref(false);
const showEditModal = ref(false);
const selectedContainers = ref<string[]>([]);
const containerForm = ref<Partial<Container & ContainerCreateRequest>>({});

// Current sort state
const currentSort = ref({ field: 'updatedAt', direction: 'desc' });

// Updated filter options based on backend model
const statusOptions = [
  'Available', 'In Transit', 'Loading', 'Loaded', 'Unloading', 
  'At Port', 'In Storage', 'Maintenance', 'Customs Hold', 'Empty'
];

const typeOptions = [
  'Dry', 'Refrigerated', 'Open Top', 'Flat Rack', 
  'Tank', 'Bulk', 'High Cube', 'Platform'
];

const cargoTypeOptions = [
  'Electronics', 'Textiles', 'Automotive Parts', 'Machinery', 
  'Food Products', 'Chemicals', 'Raw Materials', 'Consumer Goods',
  'Dairy', 'Frozen Goods', 'Pharmaceuticals', 'Hazardous Materials'
];

const conditionOptions = [
  'Good', 'Damaged', 'Needs Repair', 'Under Maintenance', 'Excellent'
];

const sizeOptions = [
  '20ft', '40ft', '45ft', '53ft'
];

// Computed properties
const canManageContainers = computed(() => {
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
const loadContainers = async () => {
  isLoading.value = true;
  error.value = null;
  
  try {
    // Try to call the actual containerService
    if (containerService && typeof containerService.getContainers === 'function') {
      paginatedData.value = await containerService.getContainers(filters.value);
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
    error.value = 'Failed to load containers. Please try again.';
  } finally {
    isLoading.value = false;
  }
};

const loadStatistics = async () => {
  try {
    if (containerService && typeof containerService.getStatistics === 'function') {
      stats.value = await containerService.getStatistics();
    } else {
      // Show zero state instead of mock statistics
      stats.value = {
        totalContainers: 0,
        availableContainers: 0,
        inTransitContainers: 0,
        atPortContainers: 0,
        loadingContainers: 0,
        unloadingContainers: 0,
        containersByType: {},
        containersByStatus: {},
        containersByLocation: {}
      };
    }
  } catch (err) {
    error.value = 'Failed to load container statistics.';
  }
};

const loadPorts = async () => {
  try {
    if (portService && typeof portService.getAll === 'function') {
      const response = await portService.getAll();
      
      locationOptions.value = response.data
        .map(port => port.name)
        .filter((name): name is string => !!name)
        .sort();
    } else {
      // Show empty ports list until API data loads
      locationOptions.value = [];
    }
  } catch (err) {
    console.error('Error loading ports:', err);
    // Show empty list instead of fallback mock data
    locationOptions.value = [];
  }
};

const loadShips = async () => {
  try {
    if (shipService && typeof shipService.getAll === 'function') {
      const response = await shipService.getAll();
      
      // Ensure we have the right structure
      const ships = response.data || response || [];
      shipOptions.value = ships.map(ship => ({
        id: ship.shipId || ship.id,
        name: ship.name || `Ship ${ship.id}`
      })).sort((a, b) => a.name.localeCompare(b.name));
    } else {
      // Show empty ships list until API data loads
      shipOptions.value = [];
    }
  } catch (err) {
    console.error('Error loading ships:', err);
    // Show empty list instead of fallback mock data
    shipOptions.value = [];
  }
};

const refreshData = async () => {
  try {
    await Promise.all([
      loadContainers(),
      loadStatistics(),
      loadPorts(),
      loadShips()
    ]);
  } catch (error) {
    console.error('Error refreshing data:', error);
  }
};

const applyFilters = () => {
  filters.value.page = 1;
  loadContainers();
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
    cargoType: '',
    currentLocation: '',
    destination: '',
    shipId: '',
    createdAfter: '',
    createdBefore: '',
    minWeight: '',
    maxWeight: ''
  };
  loadContainers();
};

const sortBy = (field: string) => {
  if (currentSort.value.field === field) {
    currentSort.value.direction = currentSort.value.direction === 'asc' ? 'desc' : 'asc';
  } else {
    currentSort.value.field = field;
    currentSort.value.direction = 'asc';
  }
  
  filters.value.sortBy = field;
  filters.value.sortDirection = currentSort.value.direction;
  loadContainers();
};

const updatePageSize = (size: number) => {
  filters.value.pageSize = size;
  filters.value.page = 1;
  loadContainers();
};

const nextPage = async () => {
  if (paginatedData.value.hasNextPage) {
    filters.value.page = (filters.value.page || 1) + 1;
    await loadContainers();
  }
};

const previousPage = async () => {
  if (paginatedData.value.hasPreviousPage) {
    filters.value.page = Math.max(1, (filters.value.page || 1) - 1);
    await loadContainers();
  }
};

const toggleSelectAll = () => {
  if (selectedContainers.value.length === paginatedData.value.data.length) {
    selectedContainers.value = [];
  } else {
    selectedContainers.value = paginatedData.value.data.map(c => c.containerId);
  }
};

const toggleSelect = (containerId: string) => {
  const index = selectedContainers.value.indexOf(containerId);
  if (index === -1) {
    selectedContainers.value.push(containerId);
  } else {
    selectedContainers.value.splice(index, 1);
  }
};

const clearSelection = () => {
  selectedContainers.value = [];
};

const performBulkStatusUpdate = async (status: string) => {
  try {
    if (containerService && typeof containerService.bulkUpdateStatus === 'function') {
      const update: BulkStatusUpdate = {
        containerIds: selectedContainers.value,
        newStatus: status
      };
      
      await containerService.bulkUpdateStatus(update);
    }
    
    await loadContainers();
    await loadStatistics();
    clearSelection();
  } catch (err) {
    alert('Failed to update containers. Please try again later.');
  }
};

const exportContainers = async () => {
  try {
    if (containerService && typeof containerService.exportContainers === 'function') {
      const blob = await containerService.exportContainers(filters.value);
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `containers_export_${new Date().toISOString().split('T')[0]}.csv`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    } else {
      alert('Export functionality is not available at the moment.');
    }
  } catch (err) {
    alert('Export failed. Please try again later.');
  }
};

const viewContainer = (container: Container) => {
  // TODO: Implement detailed view modal
  alert(`Viewing container: ${container.containerId}`);
};

const editContainer = (container: Container) => {
  containerForm.value = { ...container };
  showEditModal.value = true;
};

const deleteContainer = async (container: Container) => {
  if (!confirm(`Are you sure you want to delete container ${container.containerId}?`)) {
    return;
  }
  
  try {
    if (containerService && typeof containerService.delete === 'function') {
      await containerService.delete(container.containerId);
    }
    
    await loadContainers();
    await loadStatistics();
  } catch (err) {
    alert('Failed to delete container. Please try again later.');
  }
};

const handleFormSubmit = async (containerData: any) => {
  isSubmitting.value = true;
  
  try {
    // Enhanced payload with ALL fields for better updates
    const payload = {
      containerId: containerData.containerId,
      cargoType: containerData.cargoType || '',
      cargoDescription: containerData.cargoDescription || '',
      type: containerData.type || '',
      status: containerData.status || 'Available',
      condition: containerData.condition || 'Good',
      currentLocation: containerData.currentLocation || '',
      destination: containerData.destination || '',
      weight: containerData.weight ? parseFloat(containerData.weight.toString()) : 0,
      maxWeight: containerData.maxWeight ? parseFloat(containerData.maxWeight.toString()) : undefined,
      size: containerData.size || '',
      temperature: containerData.temperature ? parseFloat(containerData.temperature.toString()) : undefined,
      coordinates: containerData.coordinates || '',
      estimatedArrival: containerData.estimatedArrival || null,
      shipId: containerData.shipId ? parseInt(containerData.shipId.toString()) : undefined
    };

    if (showCreateModal.value) {
      if (containerService && typeof containerService.create === 'function') {
        await containerService.create(payload);
      }
    } else {
      if (containerService && typeof containerService.update === 'function') {
        await containerService.update(containerForm.value.containerId as string, payload);
      }
    }
    
    await loadContainers();
    await loadStatistics();
    closeModal();
  } catch (err: any) {
    console.error('Form submission error:', err);
    alert(`Failed to save container: ${err.message || 'Please try again later.'}`);
  } finally {
    isSubmitting.value = false;
  }
};

const closeModal = () => {
  showCreateModal.value = false;
  showEditModal.value = false;
  containerForm.value = {};
};

// Initialize data on component mount
onMounted(() => {
  refreshData();
});
</script>

<style scoped>
.container-management {
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


