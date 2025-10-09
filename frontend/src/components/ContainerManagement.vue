<!-- filepath: c:\Users\dhruv\Desktop\Company projects\Container-Tracking-and-Port-Operations-Maersk-Hackathon\frontend\src\components\ContainerManagement.vue -->
<template>
  <div class="container-management">
    <!-- Header with Actions -->
    <div class="flex justify-between items-center mb-6">
      <div>
        <h1 class="text-3xl font-bold text-gray-900">Container Management</h1>
        <p class="mt-1 text-sm text-gray-600">
          Manage and track containers across global ports
        </p>
      </div>
      <div class="flex space-x-3">
        <button
          @click="refreshData"
          :disabled="isLoading"
          class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
        >
          <RefreshCwIcon class="w-4 h-4 mr-2" />
          Refresh
        </button>
        <button
          @click="exportContainers"
          :disabled="isLoading"
          class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
        >
          <DownloadIcon class="w-4 h-4 mr-2" />
          Export CSV
        </button>
        <button
          v-if="canManageContainers"
          @click="showCreateModal = true"
          class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
        >
          <PlusIcon class="w-4 h-4 mr-2" />
          Create Container
        </button>
      </div>
    </div>

    <!-- Analytics Dashboard -->
    <div class="mb-6">
      <ContainerStats :stats="stats" />
    </div>

    <!-- Filters Section -->
    <div class="mb-6 bg-white rounded-lg shadow-md p-5">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-lg font-semibold">Filters</h2>
        <div class="flex items-center gap-2">
          <button 
            @click="showAdvancedFilters = !showAdvancedFilters"
            class="text-sm text-blue-600 hover:text-blue-800 flex items-center gap-1"
          >
            {{ showAdvancedFilters ? 'Hide Advanced Filters' : 'Show Advanced Filters' }}
            <ChevronDown v-if="!showAdvancedFilters" :size="16" />
            <ChevronUp v-else :size="16" />
          </button>
          <button 
            @click="clearFilters"
            class="text-sm text-gray-500 hover:text-gray-700"
          >
            Clear All
          </button>
        </div>
      </div>
      
      <!-- Basic Filters -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Search</label>
          <input
            v-model="filters.searchTerm"
            @input="debouncedSearch"
            type="text"
            placeholder="Container ID, cargo type..."
            class="w-full px-3 py-2 border border-gray-300 rounded-md"
          >
        </div>
        
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Status</label>
          <select
            v-model="filters.status"
            @change="applyFilters"
            class="w-full px-3 py-2 border border-gray-300 rounded-md"
          >
            <option value="">All Statuses</option>
            <option v-for="status in statusOptions" :key="status" :value="status">
              {{ status }}
            </option>
          </select>
        </div>
        
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Location</label>
          <select
            v-model="filters.currentLocation"
            @change="applyFilters"
            class="w-full px-3 py-2 border border-gray-300 rounded-md"
          >
            <option value="">All Locations</option>
            <option v-for="location in locationOptions" :key="location" :value="location">
              {{ location }}
            </option>
          </select>
        </div>
      </div>
      
      <!-- Advanced Filters -->
      <div v-if="showAdvancedFilters" class="mt-4 pt-4 border-t border-gray-200">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <!-- Container Type -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Container Type</label>
            <select
              v-model="filters.type"
              @change="applyFilters"
              class="w-full px-3 py-2 border border-gray-300 rounded-md"
            >
              <option value="">All Types</option>
              <option v-for="type in typeOptions" :key="type" :value="type">
                {{ type }}
              </option>
            </select>
          </div>
          
          <!-- Cargo Type -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Cargo Type</label>
            <select
              v-model="filters.cargoType"
              @change="applyFilters"
              class="w-full px-3 py-2 border border-gray-300 rounded-md"
            >
              <option value="">All Cargo Types</option>
              <option v-for="cargoType in cargoTypeOptions" :key="cargoType" :value="cargoType">
                {{ cargoType }}
              </option>
            </select>
          </div>
          
          <!-- Destination -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Destination</label>
            <input
              v-model="filters.destination"
              @change="applyFilters"
              type="text"
              placeholder="Enter destination"
              class="w-full px-3 py-2 border border-gray-300 rounded-md"
            >
          </div>
          
          <!-- Ship Assignment -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Ship</label>
            <select
              v-model="filters.shipId"
              @change="applyFilters"
              class="w-full px-3 py-2 border border-gray-300 rounded-md"
            >
              <option value="">All Ships</option>
              <option v-for="ship in shipOptions" :key="ship.id" :value="ship.id">
                {{ ship.name }}
              </option>
            </select>
          </div>
          
          <!-- Weight Range -->
          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Min Weight (kg)</label>
              <input
                v-model="filters.minWeight"
                @change="applyFilters"
                type="number"
                min="0"
                placeholder="Min"
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Max Weight (kg)</label>
              <input
                v-model="filters.maxWeight"
                @change="applyFilters"
                type="number"
                min="0"
                placeholder="Max"
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
            </div>
          </div>
          
          <!-- Date Range -->
          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Created After</label>
              <input
                v-model="filters.createdAfter"
                @change="applyFilters"
                type="date"
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Created Before</label>
              <input
                v-model="filters.createdBefore"
                @change="applyFilters"
                type="date"
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
            </div>
          </div>
        </div>
        
        <!-- Active Filters Display -->
        <div class="mt-4 flex flex-wrap gap-2" v-if="hasActiveFilters">
          <div class="text-sm font-medium text-gray-700">Active filters:</div>
          <div 
            v-for="(value, key) in activeFilters" 
            :key="key"
            class="bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded-full flex items-center gap-1"
          >
            {{ formatFilterKey(key) }}: {{ formatFilterValue(key, value) }}
            <button @click="removeFilter(key)" class="hover:text-blue-600">
              <XIcon :size="12" />
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Bulk Actions (when items selected) -->
    <div 
      v-if="selectedContainers.length > 0"
      class="flex items-center justify-between bg-blue-50 border border-blue-200 rounded-lg p-4 mb-6"
    >
      <div class="flex items-center space-x-4">
        <span class="font-medium">{{ selectedContainers.length }} containers selected</span>
        <select
          v-model="bulkStatusUpdate"
          class="border border-gray-300 rounded px-2 py-1"
        >
          <option value="">Change Status To...</option>
          <option v-for="status in statusOptions" :key="status" :value="status">
            {{ status }}
          </option>
        </select>
        <button
          @click="performBulkStatusUpdate"
          :disabled="!bulkStatusUpdate"
          class="px-3 py-1 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
        >
          Apply
        </button>
      </div>
      <button
        @click="clearSelection"
        class="text-sm text-gray-500 hover:text-gray-700"
      >
        Clear Selection
      </button>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="flex justify-center items-center py-12">
      <Loader2Icon class="w-12 h-12 text-blue-600 animate-spin" />
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
      <div class="flex items-center">
        <AlertTriangleIcon class="w-5 h-5 text-red-400 mr-3" />
        <div>
          <h3 class="text-sm font-medium text-red-800">Error loading containers</h3>
          <div class="mt-1 text-sm text-red-700">{{ error }}</div>
          <button
            @click="loadContainers"
            class="mt-2 bg-red-100 px-2 py-1 rounded text-sm text-red-800 hover:bg-red-200"
          >
            Try Again
          </button>
        </div>
      </div>
    </div>

    <!-- Container Table -->
    <div v-else class="bg-white rounded-lg shadow-md overflow-hidden">
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
              v-model="filters.pageSize"
              @change="updatePageSize"
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
                  @change="toggleSelectAll"
                  class="rounded border-gray-300 text-blue-600"
                >
              </th>
              <th 
                @click="sortBy('containerId')"
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
                @click="sortBy('status')"
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
                @click="sortBy('updatedAt')"
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer"
              >
                Last Updated
                <span v-if="currentSort.field === 'updatedAt'">
                  {{ currentSort.direction === 'asc' ? '↑' : '↓' }}
                </span>
              </th>
              <th v-if="canManageContainers" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="container in paginatedData.data" :key="container.containerId" class="hover:bg-gray-50">
              <td class="px-6 py-4 whitespace-nowrap">
                <input
                  type="checkbox"
                  :value="container.containerId"
                  :checked="selectedContainers.includes(container.containerId)"
                  @change="toggleSelect(container.containerId)"
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
              <td v-if="canManageContainers" class="px-6 py-4 whitespace-nowrap text-sm font-medium text-right">
                <button
                  @click="viewContainer(container)"
                  class="text-blue-600 hover:text-blue-900 mr-2"
                >
                  View
                </button>
                <button
                  @click="editContainer(container)"
                  class="text-indigo-600 hover:text-indigo-900 mr-2"
                >
                  Edit
                </button>
                <button
                  @click="deleteContainer(container)"
                  class="text-red-600 hover:text-red-900"
                >
                  Delete
                </button>
              </td>
            </tr>
            <!-- Empty State -->
            <tr v-if="paginatedData.data.length === 0">
              <td colspan="8" class="px-6 py-10 text-center text-gray-500">
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
              Showing page <span class="font-medium">{{ paginatedData.page }}</span> of 
              <span class="font-medium">{{ paginatedData.totalPages || 1 }}</span>
            </p>
          </div>
          <div>
            <nav class="relative z-0 inline-flex rounded-md shadow-sm -space-x-px">
              <button
                @click="previousPage"
                :disabled="!paginatedData.hasPreviousPage"
                class="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
              >
                <ChevronLeftIcon class="h-5 w-5" />
              </button>
              <button
                @click="nextPage"
                :disabled="!paginatedData.hasNextPage"
                class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50"
              >
                <ChevronRightIcon class="h-5 w-5" />
              </button>
            </nav>
          </div>
        </div>
      </div>
    </div>

    <!-- Create/Edit Container Modal -->
    <div v-if="showCreateModal || showEditModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50 flex items-center justify-center">
      <div class="relative mx-auto p-6 border w-full max-w-2xl shadow-lg rounded-md bg-white">
        <div class="flex justify-between items-center border-b pb-3 mb-4">
          <h3 class="text-lg font-medium text-gray-900">
            {{ showCreateModal ? 'Create New Container' : 'Edit Container' }}
          </h3>
          <button @click="closeModal" class="text-gray-400 hover:text-gray-600">
            <XIcon class="h-5 w-5" />
          </button>
        </div>
          
        <form @submit.prevent="handleFormSubmit">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <!-- Container ID -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Container ID *</label>
              <input
                v-model="containerForm.containerId"
                :disabled="showEditModal"
                type="text"
                required
                placeholder="e.g., MAEU1234567"
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
            </div>

            <!-- Cargo Type -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Cargo Type *</label>
              <select
                v-model="containerForm.cargoType"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
                <option value="">Select cargo type...</option>
                <option v-for="cargoType in cargoTypeOptions" :key="cargoType" :value="cargoType">{{ cargoType }}</option>
              </select>
            </div>

            <!-- Container Type -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Container Type *</label>
              <select
                v-model="containerForm.type"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
                <option value="">Select container type...</option>
                <option v-for="type in typeOptions" :key="type" :value="type">{{ type }}</option>
              </select>
            </div>

            <!-- Status -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Status *</label>
              <select
                v-model="containerForm.status"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
                <option value="">Select status...</option>
                <option v-for="status in statusOptions" :key="status" :value="status">{{ status }}</option>
              </select>
            </div>

            <!-- Current Location -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Current Location *</label>
              <input
                v-model="containerForm.currentLocation"
                type="text"
                required
                placeholder="Enter current location"
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
            </div>

            <!-- Destination -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Destination</label>
              <input
                v-model="containerForm.destination"
                type="text"
                placeholder="Enter destination"
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
            </div>

            <!-- Weight -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Weight (kg)</label>
              <input
                v-model.number="containerForm.weight"
                type="number"
                min="0"
                step="0.01"
                placeholder="25000"
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
            </div>

            <!-- Size -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Size</label>
              <select
                v-model="containerForm.size"
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
                <option value="">Select size...</option>
                <option value="20ft">20ft</option>
                <option value="40ft">40ft</option>
                <option value="45ft">45ft</option>
              </select>
            </div>

            <!-- Ship -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Ship</label>
              <select
                v-model="containerForm.shipId"
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
                <option :value="null">No ship assigned</option>
                <option v-for="ship in shipOptions" :key="ship.id" :value="ship.id">{{ ship.name }}</option>
              </select>
            </div>

            <!-- Temperature (for refrigerated containers) -->
            <div v-if="containerForm.type === 'Refrigerated'">
              <label class="block text-sm font-medium text-gray-700 mb-1">Temperature (°C)</label>
              <input
                v-model.number="containerForm.temperature"
                type="number"
                min="-30"
                max="30"
                step="0.1"
                placeholder="-18"
                class="w-full px-3 py-2 border border-gray-300 rounded-md"
              >
            </div>
          </div>

          <!-- Cargo Description -->
          <div class="mt-4">
            <label class="block text-sm font-medium text-gray-700 mb-1">Cargo Description</label>
            <textarea
              v-model="containerForm.cargoDescription"
              rows="3"
              placeholder="Detailed description of cargo contents..."
              class="w-full px-3 py-2 border border-gray-300 rounded-md"
            ></textarea>
          </div>

          <!-- Form Actions -->
          <div class="flex justify-end space-x-3 mt-6">
            <button
              type="button"
              @click="closeModal"
              class="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              :disabled="isSubmitting"
              class="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 disabled:opacity-50"
            >
              {{ isSubmitting ? 'Saving...' : (showCreateModal ? 'Create Container' : 'Update Container') }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { 
  RefreshCwIcon, 
  DownloadIcon, 
  PlusIcon, 
  Loader2Icon, 
  AlertTriangleIcon,
  ChevronLeftIcon,
  ChevronRightIcon,
  ChevronDownIcon as ChevronDown, 
  ChevronUpIcon as ChevronUp, 
  XIcon
} from 'lucide-vue-next';

// Import components
import ContainerStats from './containers/ContainerStats.vue';

// Import services
import { containerService } from '../services/containerService';
import { portService } from '../services/portService';
import { shipService } from '../services/shipService';

// Import types
import type { 
  Container, 
  ContainerFilters, 
  ContainerStats as ContainerStatsType, 
  PaginatedResponse,
  ContainerCreateRequest,
  ContainerUpdateRequest,
  BulkStatusUpdate,
  BulkUpdateResult
} from '../types/container';

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

// Filters
const filters = ref<ContainerFilters>({
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

// Current sort
const currentSort = ref({
  field: 'updatedAt',
  direction: 'desc'
});

// Component state
const isLoading = ref(false);
const error = ref<string | null>(null);
const isSubmitting = ref(false);
const showCreateModal = ref(false);
const showEditModal = ref(false);
const selectedContainers = ref<string[]>([]);
const bulkStatusUpdate = ref('');
const searchDebounceTimer = ref<number | null>(null);
const showAdvancedFilters = ref(false);

// Form data
const containerForm = ref<Partial<Container>>({
  containerId: '',
  cargoType: '',
  cargoDescription: '',
  type: '',
  status: '',
  condition: 'Good',
  currentLocation: '',
  destination: '',
  weight: undefined,
  size: '',
  temperature: undefined,
  shipId: undefined
});

// Filter options
const statusOptions = [
  'Available', 
  'In Transit', 
  'Loading', 
  'Loaded', 
  'Unloading', 
  'At Port', 
  'In Storage', 
  'Maintenance', 
  'Customs Hold'
];
const typeOptions = [
  'Dry', 
  'Refrigerated', 
  'Open Top', 
  'Flat Rack', 
  'Tank', 
  'Bulk', 
  'High Cube', 
  'Platform'
];
const cargoTypeOptions = [
  'Electronics', 
  'Textiles', 
  'Automotive Parts', 
  'Machinery', 
  'Food Products', 
  'Chemicals', 
  'Raw Materials', 
  'Consumer Goods', 
  'Pharmaceuticals', 
  'Oil', 
  'Grain', 
  'Coal', 
  'Steel', 
  'Furniture', 
  'Toys', 
  'Clothing'
];
const locationOptions = ref<string[]>([]);
const shipOptions = ref<Array<{ id: number; name: string }>>([]);

// Computed properties
const canManageContainers = computed(() => {
  const currentUser = localStorage.getItem('current_user');
  if (currentUser) {
    try {
      const user = JSON.parse(currentUser);
      return user.roles && (user.roles.includes('Admin') || user.roles.includes('PortManager') || user.roles.includes('Operator'));
    } catch (error) {
      return false;
    }
  }
  return false;
});

const allSelected = computed(() => 
  paginatedData.value.data.length > 0 && selectedContainers.value.length === paginatedData.value.data.length
);

const someSelected = computed(() => 
  selectedContainers.value.length > 0 && selectedContainers.value.length < paginatedData.value.data.length
);

const paginationInfo = computed(() => {
  const start = (paginatedData.value.page - 1) * paginatedData.value.pageSize + 1;
  const end = Math.min(start + paginatedData.value.pageSize - 1, paginatedData.value.totalCount || 0);
  return { 
    start: paginatedData.value.totalCount ? start : 0, 
    end: paginatedData.value.totalCount ? end : 0, 
    total: paginatedData.value.totalCount || 0 
  };
});

// Computed properties to handle filters display
const hasActiveFilters = computed(() => {
  return Object.keys(activeFilters.value).length > 0;
});

const activeFilters = computed(() => {
  const result: Record<string, any> = {};
  
  for (const [key, value] of Object.entries(filters.value)) {
    // Exclude pagination, sorting, and empty values
    if (
      key !== 'page' && 
      key !== 'pageSize' && 
      key !== 'sortBy' && 
      key !== 'sortDirection' &&
      value !== '' && 
      value !== null && 
      value !== undefined
    ) {
      result[key] = value;
    }
  }
  
  return result;
});

// Methods
const loadContainers = async () => {
  isLoading.value = true;
  error.value = null;
  
  try {
    paginatedData.value = await containerService.getContainers(filters.value);
  } catch (err: any) {
    error.value = 'Failed to load containers. Please try again.';
  } finally {
    isLoading.value = false;
  }
};

const loadStatistics = async () => {
  try {
    stats.value = await containerService.getStatistics();
  } catch (err) {
    error.value = 'Failed to load container statistics.';
  }
};

const loadPorts = async () => {
  try {
    const response = await portService.getAll();
    locationOptions.value = response.data
      .map(port => port.name)
      .filter((name): name is string => !!name)
      .sort();
  } catch (err) {
    error.value = 'Failed to load port locations.';
  }
};

const loadShips = async () => {
  try {
    const response = await shipService.getAll();
    shipOptions.value = response.data
      .map(ship => ({ id: ship.id, name: ship.name }))
      .sort((a, b) => a.name.localeCompare(b.name));
  } catch (err) {
    error.value = 'Failed to load ships.';
  }
};

const refreshData = async () => {
  await Promise.all([
    loadContainers(),
    loadStatistics(),
    loadPorts(),
    loadShips()
  ]);
};

const debouncedSearch = () => {
  if (searchDebounceTimer.value) {
    clearTimeout(searchDebounceTimer.value);
  }
  
  searchDebounceTimer.value = window.setTimeout(() => {
    applyFilters();
  }, 500);
};

const applyFilters = () => {
  filters.value.page = 1; // Reset to first page when changing filters
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

// Format filter keys and values for display
const formatFilterKey = (key: string): string => {
  // Convert camelCase to words with spaces and capitalization
  return key
    .replace(/([A-Z])/g, ' $1')
    .replace(/^./, (str) => str.toUpperCase());
};

const formatFilterValue = (key: string, value: any): string => {
  if (key === 'shipId' && value) {
    const ship = shipOptions.value.find(s => s.id === parseInt(value));
    return ship ? ship.name : value;
  }
  return value.toString();
};

const removeFilter = (key: string) => {
  if (key in filters.value) {
    // @ts-ignore - we know the key exists
    filters.value[key] = '';
    applyFilters();
  }
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

const updatePageSize = () => {
  filters.value.page = 1; // Reset to first page when changing page size
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
  bulkStatusUpdate.value = '';
};

const performBulkStatusUpdate = async () => {
  if (!bulkStatusUpdate.value || selectedContainers.value.length === 0) return;
  
  try {
    const update: BulkStatusUpdate = {
      containerIds: selectedContainers.value,
      newStatus: bulkStatusUpdate.value
    };
    
    const result = await containerService.bulkUpdateStatus(update);
    
    if (result.successCount > 0) {
      await loadContainers();
      await loadStatistics();
      clearSelection();
    }
    
    if (result.failedCount > 0) {
      alert(`${result.successCount} containers updated successfully. ${result.failedCount} failed to update.`);
    }
  } catch (err) {
    alert('Failed to update containers. Please try again later.');
  }
};

const exportContainers = async () => {
  try {
    const blob = await containerService.exportContainers(filters.value);
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `containers_export_${new Date().toISOString().split('T')[0]}.csv`;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
  } catch (err) {
    alert('Failed to export containers. Please try again later.');
  }
};

const getStatusBadgeClass = (status: string): string => {
  const statusClasses: Record<string, string> = {
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
  
  return statusClasses[status] || 'bg-gray-100 text-gray-800';
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

const formatWeight = (weight: number | undefined): string => {
  if (!weight) return '-';
  
  // Format weight with thousands separator
  return new Intl.NumberFormat('en-US').format(weight);
};

const viewContainer = (container: Container) => {
  // TODO: Implement detailed view modal
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
    await containerService.delete(container.containerId);
    await loadContainers();
    await loadStatistics();
  } catch (err) {
    alert('Failed to delete container. Please try again later.');
  }
};

const handleFormSubmit = async () => {
  isSubmitting.value = true;
  
  try {
    if (showCreateModal.value) {
      await containerService.create(containerForm.value as ContainerCreateRequest);
    } else {
      await containerService.update(
        containerForm.value.containerId as string, 
        containerForm.value as ContainerUpdateRequest
      );
    }
    
    await loadContainers();
    await loadStatistics();
    closeModal();
  } catch (err) {
    alert(`Failed to save container. Please try again later.`);
  } finally {
    isSubmitting.value = false;
  }
};

const closeModal = () => {
  showCreateModal.value = false;
  showEditModal.value = false;
  containerForm.value = {
    containerId: '',
    cargoType: '',
    cargoDescription: '',
    type: '',
    status: '',
    condition: 'Good',
    currentLocation: '',
    destination: '',
    weight: undefined,
    size: '',
    temperature: undefined,
    shipId: undefined
  };
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

/* Custom checkbox styles for indeterminate state */
input[type="checkbox"]:indeterminate {
  background-color: #3b82f6;
  border-color: #3b82f6;
}

input[type="checkbox"]:indeterminate::before {
  content: '';
  position: absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
  width: 8px;
  height: 2px;
  background-color: white;
}

/* Modal backdrop blur effect */
.fixed.inset-0.bg-gray-600.bg-opacity-50 {
  backdrop-filter: blur(4px);
}

/* Responsive table improvements */
@media (max-width: 768px) {
  .overflow-x-auto table {
    font-size: 0.875rem;
  }
  
  .overflow-x-auto th,
  .overflow-x-auto td {
    padding: 0.5rem 0.75rem;
  }
}

/* Status badge animations */
.inline-flex.px-2.py-1.text-xs.font-semibold.rounded-full {
  transition: all 0.2s ease-in-out;
}

.inline-flex.px-2.py-1.text-xs.font-semibold.rounded-full:hover {
  transform: scale(1.05);
}

/* Button hover effects */
button:hover {
  transition: all 0.2s ease-in-out;
}

/* Form input focus styles */
.focus\:ring-blue-500:focus {
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

/* Loading spinner colors */
.border-blue-600 {
  border-color: #2563eb;
}

/* Analytics cards hover effect */
.bg-gradient-to-r {
  transition: transform 0.2s ease-in-out;
}

.bg-gradient-to-r:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

/* Table row hover effect */
.hover\:bg-gray-50:hover {
  background-color: #f9fafb;
  transition: background-color 0.2s ease-in-out;
}

/* Pagination button styles */
.disabled\:opacity-50:disabled {
  cursor: not-allowed;
}

/* Modal animation */
.fixed.inset-0 {
  animation: fadeIn 0.3s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Filter section styling */
.grid.grid-cols-1.md\:grid-cols-2.lg\:grid-cols-4.gap-4 {
  margin-bottom: 1rem;
}

/* Bulk actions bar styling */
.bg-blue-50.border.border-blue-200 {
  animation: slideDown 0.3s ease-out;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Error message styling */
.bg-red-50.border.border-red-200 {
  animation: shake 0.5s ease-in-out;
}

@keyframes shake {
  0%, 20%, 40%, 60%, 80%, 100% {
    transform: translateX(0);
  }
  10%, 30%, 50%, 70%, 90% {
    transform: translateX(-5px);
  }
}

/* Success notification (for future use) */
.success-notification {
  background-color: #10b981;
  max-height: 200px;
  color: white;
  padding: 1rem;
  border-radius: 0.5rem;
  margin-bottom: 1rem;
  animation: slideIn 0.3s ease-out;
}

/* Container type badges */
.bg-gray-100.text-gray-800 {
  background-color: #f3f4f6;
  color: #1f2937;
}

/* Weight formatting for large numbers */
.font-mono {
  font-family: ui-monospace, SFMono-Regular, "SF Mono", Consolas, "Liberation Mono", Menlo, monospace;
}

/* Custom scrollbar for modal content */
.overflow-y-auto::-webkit-scrollbar {
  width: 6px;
}

.overflow-y-auto::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 3px;
}

.overflow-y-auto::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 3px;
}

.overflow-y-auto::-webkit-scrollbar-thumb:hover {
  background: #a1a1a1;
}

/* Temperature input styling for refrigerated containers */
input[type="number"][placeholder*="°C"] {
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 24 24' stroke='currentColor'%3E%3Cpath stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 8px center;
  background-size: 16px;
}

/* Cargo description textarea styling */
textarea {
  resize: vertical;
  min-height: 80px;
}

/* Sort indicator styling */
.cursor-pointer:hover {
  background-color: #f9fafb;
}

th span {
  margin-left: 4px;
  font-weight: bold;
  color: #3b82f6;
}

/* Print styles */
@media print {
  .container-management {
    padding: 0;
  }
  
  button,
  .fixed, 
  .sticky {
    display: none !important;
  }
  
  .bg-gradient-to-r {
    background: #f3f4f6 !important;
    color: #000 !important;
    -webkit-print-color-adjust: exact;
  }
}
</style>
