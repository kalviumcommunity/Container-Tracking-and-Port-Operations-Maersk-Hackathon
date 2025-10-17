<template>
  <div class="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow p-6 border border-gray-200">
    <!-- Header with Container ID and Status -->
    <div class="flex justify-between items-start mb-4">
      <div class="flex-1">
        <h3 class="text-lg font-bold text-gray-900 mb-1">
          {{ container.containerId }}
        </h3>
        <p class="text-sm text-gray-600">
          {{ container.cargoType }}
        </p>
      </div>
      <span 
        :class="getStatusClass(container.status)"
        class="px-3 py-1 rounded-full text-xs font-semibold"
      >
        {{ container.status }}
      </span>
    </div>

    <!-- Container Type Badge -->
    <div class="mb-4">
      <span 
        :class="getTypeClass(container.type)"
        class="inline-flex items-center px-2.5 py-0.5 rounded text-xs font-medium"
      >
        {{ container.type }}
      </span>
    </div>

    <!-- Container Details Grid -->
    <div class="space-y-3 mb-4">
      <!-- Location -->
      <div class="flex items-start">
        <MapPinIcon class="w-4 h-4 text-gray-400 mt-0.5 mr-2 flex-shrink-0" />
        <div class="flex-1 min-w-0">
          <p class="text-xs text-gray-500">Current Location</p>
          <p class="text-sm font-medium text-gray-900 truncate">
            {{ container.currentLocation || 'Unknown' }}
          </p>
        </div>
      </div>

      <!-- Destination -->
      <div v-if="container.destination" class="flex items-start">
        <NavigationIcon class="w-4 h-4 text-gray-400 mt-0.5 mr-2 flex-shrink-0" />
        <div class="flex-1 min-w-0">
          <p class="text-xs text-gray-500">Destination</p>
          <p class="text-sm font-medium text-gray-900 truncate">
            {{ container.destination }}
          </p>
        </div>
      </div>

      <!-- Weight -->
      <div v-if="container.weight" class="flex items-start">
        <ScaleIcon class="w-4 h-4 text-gray-400 mt-0.5 mr-2 flex-shrink-0" />
        <div class="flex-1 min-w-0">
          <p class="text-xs text-gray-500">Weight</p>
          <p class="text-sm font-medium text-gray-900">
            {{ container.weight.toLocaleString() }} kg
            <span v-if="container.maxWeight" class="text-gray-500">
              / {{ container.maxWeight.toLocaleString() }} kg
            </span>
          </p>
        </div>
      </div>

      <!-- Ship Assignment -->
      <div v-if="container.shipName" class="flex items-start">
        <ShipIcon class="w-4 h-4 text-gray-400 mt-0.5 mr-2 flex-shrink-0" />
        <div class="flex-1 min-w-0">
          <p class="text-xs text-gray-500">Assigned Ship</p>
          <p class="text-sm font-medium text-gray-900 truncate">
            {{ container.shipName }}
          </p>
        </div>
      </div>

      <!-- Temperature (for refrigerated) -->
      <div v-if="container.temperature !== null && container.temperature !== undefined" class="flex items-start">
        <ThermometerIcon class="w-4 h-4 text-gray-400 mt-0.5 mr-2 flex-shrink-0" />
        <div class="flex-1 min-w-0">
          <p class="text-xs text-gray-500">Temperature</p>
          <p class="text-sm font-medium text-gray-900">
            {{ container.temperature }}°C
          </p>
        </div>
      </div>
    </div>

    <!-- Action Buttons -->
    <div class="flex items-center justify-between pt-4 border-t border-gray-200">
      <button
        @click="$emit('view', container)"
        class="flex items-center text-sm text-blue-600 hover:text-blue-700 font-medium"
      >
        <EyeIcon class="w-4 h-4 mr-1" />
        View Details
      </button>
      
      <div v-if="canManage" class="flex items-center space-x-2">
        <button
          @click="$emit('edit', container)"
          class="p-2 text-gray-600 hover:text-blue-600 hover:bg-blue-50 rounded transition-colors"
          title="Edit container"
        >
          <PencilIcon class="w-4 h-4" />
        </button>
        <button
          @click="$emit('delete', container)"
          class="p-2 text-gray-600 hover:text-red-600 hover:bg-red-50 rounded transition-colors"
          title="Delete container"
        >
          <TrashIcon class="w-4 h-4" />
        </button>
      </div>
    </div>

    <!-- Last Updated -->
    <div class="mt-3 text-xs text-gray-500 text-center">
      Updated {{ formatDate(container.updatedAt) }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { 
  MapPinIcon, 
  NavigationIcon, 
  ScaleIcon, 
  ShipIcon, 
  ThermometerIcon,
  EyeIcon, 
  PencilIcon, 
  TrashIcon 
} from 'lucide-vue-next';

interface Container {
  containerId: string;
  cargoType: string;
  type: string;
  status: string;
  currentLocation: string;
  destination?: string;
  weight?: number;
  maxWeight?: number;
  temperature?: number;
  shipName?: string;
  updatedAt: string;
}

interface Props {
  container: Container;
  canManage?: boolean;
}

defineProps<Props>();
defineEmits<{
  view: [container: Container];
  edit: [container: Container];
  delete: [container: Container];
}>();

const getStatusClass = (status: string): string => {
  const statusClasses: Record<string, string> = {
    'Available': 'bg-green-100 text-green-800',
    'In Transit': 'bg-blue-100 text-blue-800',
    'At Port': 'bg-yellow-100 text-yellow-800',
    'Loading': 'bg-orange-100 text-orange-800',
    'Unloading': 'bg-purple-100 text-purple-800',
    'Maintenance': 'bg-red-100 text-red-800',
  };
  return statusClasses[status] || 'bg-gray-100 text-gray-800';
};

const getTypeClass = (type: string): string => {
  const typeClasses: Record<string, string> = {
    'Dry': 'bg-gray-100 text-gray-700',
    'Refrigerated': 'bg-blue-50 text-blue-700',
    'Tank': 'bg-yellow-50 text-yellow-700',
    'OpenTop': 'bg-green-50 text-green-700',
    'FlatRack': 'bg-purple-50 text-purple-700',
  };
  return typeClasses[type] || 'bg-gray-100 text-gray-700';
};

const formatDate = (dateString: string): string => {
  const date = new Date(dateString);
  const now = new Date();
  const diffInHours = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60));
  
  if (diffInHours < 1) return 'just now';
  if (diffInHours < 24) return `${diffInHours}h ago`;
  if (diffInHours < 48) return 'yesterday';
  
  return date.toLocaleDateString();
};
</script>
