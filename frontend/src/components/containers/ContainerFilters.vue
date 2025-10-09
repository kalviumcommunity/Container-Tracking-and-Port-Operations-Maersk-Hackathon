<template>
  <div class="bg-white rounded-lg shadow-md p-6">
    <div class="flex justify-between items-center mb-4">
      <h2 class="text-lg font-semibold text-gray-800">Filters & Search</h2>
      <button
        @click="$emit('clear-filters')"
        class="text-sm text-gray-500 hover:text-gray-700"
      >
        Clear All Filters
      </button>
    </div>
    
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-4">
      <!-- Search Term -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Search</label>
        <input
          :value="filters.searchTerm"
          @input="updateFilter('searchTerm', $event.target.value)"
          type="text"
          placeholder="Container ID, cargo type, location..."
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
        >
      </div>

      <!-- Status Filter -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Status</label>
        <select
          :value="filters.status"
          @change="updateFilter('status', $event.target.value)"
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
        >
          <option value="">All Statuses</option>
          <option v-for="status in statusOptions" :key="status" :value="status">
            {{ status }}
          </option>
        </select>
      </div>

      <!-- Type Filter -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Container Type</label>
        <select
          :value="filters.type"
          @change="updateFilter('type', $event.target.value)"
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
        >
          <option value="">All Types</option>
          <option v-for="type in typeOptions" :key="type" :value="type">
            {{ type }}
          </option>
        </select>
      </div>

      <!-- Cargo Type Filter -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Cargo Type</label>
        <select
          :value="filters.cargoType"
          @change="updateFilter('cargoType', $event.target.value)"
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
        >
          <option value="">All Cargo Types</option>
          <option v-for="cargoType in cargoTypeOptions" :key="cargoType" :value="cargoType">
            {{ cargoType }}
          </option>
        </select>
      </div>

      <!-- Location Filter -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Current Location</label>
        <select
          :value="filters.currentLocation"
          @change="updateFilter('currentLocation', $event.target.value)"
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
        >
          <option value="">All Locations</option>
          <option v-for="location in locationOptions" :key="location" :value="location">
            {{ location }}
          </option>
        </select>
      </div>

      <!-- Ship Filter -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Ship</label>
        <select
          :value="filters.shipId"
          @change="updateFilter('shipId', $event.target.value)"
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
        >
          <option value="">All Ships</option>
          <option v-for="ship in shipOptions" :key="ship.id" :value="ship.id">
            {{ ship.name }}
          </option>
        </select>
      </div>

      <!-- Weight Range -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Min Weight (kg)</label>
        <input
          :value="filters.minWeight"
          @input="updateFilter('minWeight', $event.target.value)"
          type="number"
          min="0"
          placeholder="0"
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
        >
      </div>

      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Max Weight (kg)</label>
        <input
          :value="filters.maxWeight"
          @input="updateFilter('maxWeight', $event.target.value)"
          type="number"
          min="0"
          placeholder="50000"
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
        >
      </div>
    </div>

    <!-- Date Range Filters -->
    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Created After</label>
        <input
          :value="filters.createdAfter"
          @change="updateFilter('createdAfter', $event.target.value)"
          type="date"
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
        >
      </div>
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Created Before</label>
        <input
          :value="filters.createdBefore"
          @change="updateFilter('createdBefore', $event.target.value)"
          type="date"
          class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
        >
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ContainerFilters } from '../../types/container';

interface Props {
  filters: ContainerFilters;
  statusOptions: string[];
  typeOptions: string[];
  cargoTypeOptions: string[];
  locationOptions: string[];
  shipOptions: Array<{ id: number; name: string }>;
}

defineProps<Props>();

const emit = defineEmits<{
  'update-filter': [key: string, value: any];
  'clear-filters': [];
}>();

const updateFilter = (key: string, value: any) => {
  emit('update-filter', key, value);
};
</script>
