<template>
  <div class="container-management">
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div>
        <h1 class="text-3xl font-bold text-gray-900">Container Management</h1>
        <div v-if="isAdminUser" class="mt-2">
          <span class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-blue-100 text-blue-800">
            <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-6-3a2 2 0 11-4 0 2 2 0 014 0zm-2 4a5 5 0 00-4.546 2.916A5.986 5.986 0 0010 16a5.986 5.986 0 004.546-2.084A5 5 0 0010 11z" clip-rule="evenodd"></path>
            </svg>
            System Administrator Access
          </span>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-lg shadow-md p-6 mb-6">
      <h2 class="text-lg font-semibold text-gray-800 mb-4">Filters</h2>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label for="statusFilter" class="block text-sm font-medium text-gray-700">Status</label>
          <select
            id="statusFilter"
            v-model="filters.status"
            @change="applyFilters"
            class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="">All Statuses</option>
            <option v-for="status in statusOptions" :key="status" :value="status">{{ status }}</option>
          </select>
        </div>
        <div>
          <label for="locationFilter" class="block text-sm font-medium text-gray-700">Location</label>
          <select
            id="locationFilter"
            v-model="filters.location"
            @change="applyFilters"
            class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="">All Locations</option>
            <option v-for="location in locationOptions" :key="location" :value="location">{{ location }}</option>
          </select>
        </div>
        <div>
          <label for="typeFilter" class="block text-sm font-medium text-gray-700">Container Type</label>
          <select
            id="typeFilter"
            v-model="filters.type"
            @change="applyFilters"
            class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="">All Types</option>
            <option v-for="type in typeOptions" :key="type" :value="type">{{ type }}</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-blue-50 rounded-lg p-4">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center">
              <span class="text-blue-600 font-semibold">T</span>
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500">Total Containers</p>
            <p class="text-2xl font-semibold text-gray-900">{{ stats.total }}</p>
          </div>
        </div>
      </div>
      <div class="bg-green-50 rounded-lg p-4">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-green-100 rounded-full flex items-center justify-center">
              <span class="text-green-600 font-semibold">A</span>
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500">Available</p>
            <p class="text-2xl font-semibold text-gray-900">{{ stats.available }}</p>
          </div>
        </div>
      </div>
      <div class="bg-yellow-50 rounded-lg p-4">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-yellow-100 rounded-full flex items-center justify-center">
              <span class="text-yellow-600 font-semibold">T</span>
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500">In Transit</p>
            <p class="text-2xl font-semibold text-gray-900">{{ stats.inTransit }}</p>
          </div>
        </div>
      </div>
      <div class="bg-purple-50 rounded-lg p-4">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-purple-100 rounded-full flex items-center justify-center">
              <span class="text-purple-600 font-semibold">P</span>
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500">At Port</p>
            <p class="text-2xl font-semibold text-gray-900">{{ stats.atPort }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="flex justify-center items-center py-8">
      <div class="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600"></div>
    </div>

    <!-- Error State -->
    <div v-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
      <div class="flex">
        <div class="ml-3">
          <h3 class="text-sm font-medium text-red-800">Error loading containers</h3>
          <div class="mt-2 text-sm text-red-700">
            <p>{{ error }}</p>
          </div>
          <div class="mt-4">
            <button
              @click="loadContainers"
              class="bg-red-100 px-2 py-1 rounded text-sm text-red-800 hover:bg-red-200"
            >
              Try Again
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Container Table -->
    <div v-if="!isLoading && !error" class="bg-white rounded-lg shadow-md overflow-hidden">
      <div class="px-6 py-4 border-b border-gray-200">
        <div class="flex justify-between items-center">
          <h2 class="text-lg font-semibold text-gray-800">
            Containers ({{ filteredContainers.length }} of {{ containers.length }})
          </h2>
          <div v-if="isAdminUser" class="flex items-center gap-2 text-sm text-gray-600">
            <svg class="w-4 h-4 text-green-500" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path>
            </svg>
            <span class="font-medium">Data from ContainerTrackingDB</span>
          </div>
        </div>
      </div>
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Container ID
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Name
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Type
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Status
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Location
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Ship
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Last Updated
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="container in filteredContainers" :key="container.containerId" class="hover:bg-gray-50">
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                {{ container.containerId }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ container.name }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ container.type }}
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
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import { containerApi } from '../services/api';

export default {
  name: 'ContainerManagement',
  data() {
    return {
      // Data
      containers: [],
      filteredContainers: [],
      
      // Filters
      filters: {
        status: '',
        location: '',
        type: ''
      },
      
      // UI State
      isLoading: false,
      error: null,
      
      // Options for filters
      statusOptions: [],
      locationOptions: [],
      typeOptions: []
    };
  },
  
  computed: {
    stats() {
      const total = this.containers.length;
      const available = this.containers.filter(c => c.status === 'Available').length;
      const inTransit = this.containers.filter(c => c.status === 'In Transit').length;
      const atPort = this.containers.filter(c => c.status === 'At Port').length;
      
      return {
        total,
        available,
        inTransit,
        atPort
      };
    },
    
    isAdminUser() {
      const adminUser = localStorage.getItem('admin_user');
      return !!adminUser;
    }
  },
  
  async mounted() {
    await this.loadContainers();
  },
  
  methods: {
    async loadContainers() {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await containerApi.getAll();
        
        if (response.data) {
          this.containers = response.data;
          this.filteredContainers = [...this.containers];
          this.updateFilterOptions();
        } else {
          this.error = response.message || 'Failed to load containers';
        }
      } catch (error) {
        console.error('Error loading containers:', error);
        this.error = error.message || 'Failed to load containers. Please try again.';
      } finally {
        this.isLoading = false;
      }
    },
    
    updateFilterOptions() {
      // Get unique values for filter options
      this.statusOptions = [...new Set(this.containers.map(c => c.status))].sort();
      this.locationOptions = [...new Set(this.containers.map(c => c.currentLocation))].sort();
      this.typeOptions = [...new Set(this.containers.map(c => c.type))].sort();
    },
    
    applyFilters() {
      this.filteredContainers = this.containers.filter(container => {
        return (
          (!this.filters.status || container.status === this.filters.status) &&
          (!this.filters.location || container.currentLocation === this.filters.location) &&
          (!this.filters.type || container.type === this.filters.type)
        );
      });
    },
    
    getStatusBadgeClass(status) {
      const statusClasses = {
        'Available': 'bg-green-100 text-green-800',
        'In Transit': 'bg-yellow-100 text-yellow-800',
        'At Port': 'bg-blue-100 text-blue-800',
        'Loading': 'bg-purple-100 text-purple-800',
        'Unloading': 'bg-orange-100 text-orange-800',
        'Maintenance': 'bg-red-100 text-red-800'
      };
      
      return statusClasses[status] || 'bg-gray-100 text-gray-800';
    },
    
    formatDate(dateString) {
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
    }
  }
};
</script>

<style scoped>
.container-management {
  max-width: 1200px;
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