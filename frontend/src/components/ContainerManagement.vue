<template>
  <div class="container-management">
    <!-- Login Section -->
    <div v-if="!isAuthenticated" class="login-section bg-white rounded-lg shadow-md p-6 mb-6">
      <h2 class="text-2xl font-bold text-gray-800 mb-4">Login to View Containers</h2>
      <form @submit.prevent="handleLogin" class="space-y-4">
        <div>
          <label for="username" class="block text-sm font-medium text-gray-700">Username</label>
          <input
            id="username"
            v-model="loginForm.username"
            type="text"
            required
            class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            placeholder="Enter username"
          />
        </div>
        <div>
          <label for="password" class="block text-sm font-medium text-gray-700">Password</label>
          <input
            id="password"
            v-model="loginForm.password"
            type="password"
            required
            class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500"
            placeholder="Enter password"
          />
        </div>
        <button
          type="submit"
          :disabled="isLoading"
          class="w-full bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-50"
        >
          {{ isLoading ? 'Logging in...' : 'Login' }}
        </button>
      </form>
      <div class="mt-4 p-3 bg-blue-50 rounded-md">
        <p class="text-sm text-blue-800">
          <strong>Demo Credentials:</strong><br>
          Username: admin<br>
          Password: Admin123!
        </p>
      </div>
      <div v-if="error" class="mt-4 p-3 bg-red-50 rounded-md">
        <p class="text-sm text-red-800">{{ error }}</p>
      </div>
    </div>

    <!-- Container Management Section -->
    <div v-if="isAuthenticated" class="container-data">
      <!-- Header with Logout -->
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-3xl font-bold text-gray-900">Container Management</h1>
        <button
          @click="handleLogout"
          class="bg-red-600 text-white px-4 py-2 rounded-md hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2"
        >
          Logout
        </button>
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
          <h2 class="text-lg font-semibold text-gray-800">
            Containers ({{ filteredContainers.length }} of {{ containers.length }})
          </h2>
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
  </div>
</template>

<script>
import { authApi, containerApi } from '../services/api.js';

export default {
  name: 'ContainerManagement',
  data() {
    return {
      // Authentication
      isAuthenticated: false,
      loginForm: {
        username: '',
        password: ''
      },
      
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
    }
  },
  
  async mounted() {
    this.isAuthenticated = authApi.isAuthenticated();
    if (this.isAuthenticated) {
      await this.loadContainers();
    }
  },
  
  methods: {
    async handleLogin() {
      this.isLoading = true;
      this.error = null;
      
      try {
        const result = await authApi.login(this.loginForm.username, this.loginForm.password);
        
        if (result.success) {
          this.isAuthenticated = true;
          this.loginForm = { username: '', password: '' };
          await this.loadContainers();
        } else {
          this.error = result.message || 'Login failed';
        }
      } catch (error) {
        console.error('Login error:', error);
        this.error = error.message || 'Login failed. Please check your credentials.';
      } finally {
        this.isLoading = false;
      }
    },
    
    handleLogout() {
      authApi.logout();
      this.isAuthenticated = false;
      this.containers = [];
      this.filteredContainers = [];
      this.error = null;
    },
    
    async loadContainers() {
      this.isLoading = true;
      this.error = null;
      
      try {
        const response = await containerApi.getAllContainers();
        
        if (response.success) {
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