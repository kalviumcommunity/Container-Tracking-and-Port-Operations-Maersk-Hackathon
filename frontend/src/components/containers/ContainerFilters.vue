<template>
  <div class="mb-6 bg-white rounded-lg shadow-md p-5">
    <div class="flex justify-between items-center mb-4">
      <h2 class="text-lg font-semibold">Filters</h2>
      <div class="flex items-center gap-2">
        <button
          @click="showAdvanced = !showAdvanced"
          class="text-sm text-blue-600 hover:text-blue-800 flex items-center gap-1"
        >
          {{ showAdvanced ? 'Hide Advanced Filters' : 'Show Advanced Filters' }}
          <ChevronDown v-if="!showAdvanced" :size="16" />
          <ChevronUp v-else :size="16" />
        </button>
        <button
          @click="$emit('clear')"
          class="text-sm text-gray-500 hover:text-gray-700"
        >
          Clear All
        </button>
      </div>
    </div>

    <!-- Basic Filters -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Search</label>
        <input
          :value="filters.searchTerm"
          @input="updateFilter('searchTerm', $event.target.value)"
          type="text"
          placeholder="Container ID, cargo type..."
          class="w-full px-3 py-2 border border-gray-300 rounded-md"
        />
      </div>

      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Status</label>
        <select
          :value="filters.status"
          @change="updateFilter('status', $event.target.value)"
          class="w-full px-3 py-2 border border-gray-300 rounded-md"
        >
          <option value="">All Statuses</option>
          <option value="Available">Available</option>
          <option value="In Transit">In Transit</option>
          <option value="Loading">Loading</option>
          <option value="Loaded">Loaded</option>
          <option value="Unloading">Unloading</option>
          <option value="At Port">At Port</option>
          <option value="In Storage">In Storage</option>
          <option value="Maintenance">Maintenance</option>
          <option value="Customs Hold">Customs Hold</option>
          <option value="Empty">Empty</option>
        </select>
      </div>

      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Type</label>
        <select
          :value="filters.type"
          @change="updateFilter('type', $event.target.value)"
          class="w-full px-3 py-2 border border-gray-300 rounded-md"
        >
          <option value="">All Types</option>
          <option value="Dry">Dry</option>
          <option value="Refrigerated">Refrigerated</option>
          <option value="Tank">Tank</option>
          <option value="Open Top">Open Top</option>
          <option value="Flat Rack">Flat Rack</option>
          <option value="Bulk">Bulk</option>
          <option value="High Cube">High Cube</option>
          <option value="Platform">Platform</option>
        </select>
      </div>

      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Location</label>
        <select
          :value="filters.currentLocation"
          @change="updateFilter('currentLocation', $event.target.value)"
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
    <div v-if="showAdvanced" class="mt-4 pt-4 border-t border-gray-200 space-y-4">
      <!-- Second Row -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Cargo Type</label>
          <select
            :value="filters.cargoType"
            @change="updateFilter('cargoType', $event.target.value)"
            class="w-full px-3 py-2 border border-gray-300 rounded-md"
          >
            <option value="">All Cargo Types</option>
            <option value="Electronics">Electronics</option>
            <option value="Automotive Parts">Automotive Parts</option>
            <option value="Food Products">Food Products</option>
            <option value="Chemicals">Chemicals</option>
            <option value="Dairy">Dairy</option>
            <option value="Frozen Goods">Frozen Goods</option>
            <option value="Pharmaceuticals">Pharmaceuticals</option>
            <option value="Hazardous Materials">Hazardous Materials</option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Condition</label>
          <select
            :value="filters.condition"
            @change="updateFilter('condition', $event.target.value)"
            class="w-full px-3 py-2 border border-gray-300 rounded-md"
          >
            <option value="">All Conditions</option>
            <option value="Good">Good</option>
            <option value="Damaged">Damaged</option>
            <option value="Needs Repair">Needs Repair</option>
            <option value="Under Maintenance">Under Maintenance</option>
            <option value="Excellent">Excellent</option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Size</label>
          <select
            :value="filters.size"
            @change="updateFilter('size', $event.target.value)"
            class="w-full px-3 py-2 border border-gray-300 rounded-md"
          >
            <option value="">All Sizes</option>
            <option value="20ft">20ft</option>
            <option value="40ft">40ft</option>
            <option value="45ft">45ft</option>
            <option value="53ft">53ft</option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Ship</label>
          <select
            :value="filters.shipId"
            @change="updateFilter('shipId', $event.target.value)"
            class="w-full px-3 py-2 border border-gray-300 rounded-md"
          >
            <option value="">All Ships</option>
            <option v-for="ship in shipOptions" :key="ship.id" :value="ship.id">
              {{ ship.name }}
            </option>
          </select>
        </div>
      </div>

      <!-- Weight and Date Filters -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Min Weight (kg)</label>
          <input
            :value="filters.minWeight"
            @input="updateFilter('minWeight', $event.target.value)"
            type="number"
            min="0"
            placeholder="0"
            class="w-full px-3 py-2 border border-gray-300 rounded-md"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Max Weight (kg)</label>
          <input
            :value="filters.maxWeight"
            @input="updateFilter('maxWeight', $event.target.value)"
            type="number"
            min="0"
            placeholder="50000"
            class="w-full px-3 py-2 border border-gray-300 rounded-md"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Created After</label>
          <input
            :value="filters.createdAfter"
            @change="updateFilter('createdAfter', $event.target.value)"
            type="date"
            class="w-full px-3 py-2 border border-gray-300 rounded-md"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Created Before</label>
          <input
            :value="filters.createdBefore"
            @change="updateFilter('createdBefore', $event.target.value)"
            type="date"
            class="w-full px-3 py-2 border border-gray-300 rounded-md"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { ChevronDownIcon as ChevronDown, ChevronUpIcon as ChevronUp } from 'lucide-vue-next';

const props = defineProps<{
  filters: any;
  locationOptions: string[];
  shipOptions: Array<{ id: number; name: string }>;
}>();

const emit = defineEmits<{
  'update:filters': [filters: any];
  apply: [];
  clear: [];
}>();

const showAdvanced = ref(false);

const updateFilter = (key: string, value: any) => {
  const updatedFilters = { ...props.filters, [key]: value };
  emit('update:filters', updatedFilters);
  emit('apply');
};

// Development-only debug logging
if (import.meta.env.DEV) {

}
</script>
