<template>
  <div class="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200 border border-gray-200">
    <!-- Card Header -->
    <div class="px-6 py-4 border-b border-gray-200">
      <div class="flex items-center justify-between">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center">
              <MapPin class="w-5 h-5 text-blue-600" />
            </div>
          </div>
          <div class="ml-4">
            <h3 class="text-lg font-semibold text-gray-900">{{ berth.name }}</h3>
            <div class="flex items-center text-sm text-gray-500">
              <Building class="w-4 h-4 mr-1" />
              {{ berth.portName }}
            </div>
          </div>
        </div>
        <div class="flex items-center space-x-2">
          <span 
            class="inline-flex px-3 py-1 text-xs font-semibold rounded-full"
            :class="getStatusBadgeClass(berth.status)"
          >
            {{ berth.status }}
          </span>
          <div v-if="showActions" class="relative">
            <button
              @click="showMenu = !showMenu"
              class="p-1 text-gray-400 hover:text-gray-600 focus:outline-none"
            >
              <MoreVertical class="w-5 h-5" />
            </button>
            <div
              v-if="showMenu"
              v-click-outside="() => showMenu = false"
              class="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg py-1 z-10 border border-gray-200"
            >
              <button
                @click="handleAction('view')"
                class="flex items-center w-full px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
              >
                <Eye class="w-4 h-4 mr-2" />
                View Details
              </button>
              <button
                @click="handleAction('edit')"
                class="flex items-center w-full px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
              >
                <Edit class="w-4 h-4 mr-2" />
                Edit Berth
              </button>
              <button
                @click="handleAction('assignments')"
                class="flex items-center w-full px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
              >
                <Users class="w-4 h-4 mr-2" />
                Manage Assignments
              </button>
              <hr class="my-1">
              <button
                @click="handleAction('delete')"
                class="flex items-center w-full px-4 py-2 text-sm text-red-600 hover:bg-red-50"
              >
                <Trash2 class="w-4 h-4 mr-2" />
                Delete Berth
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Card Body -->
    <div class="px-6 py-4">
      <!-- Capacity and Utilization -->
      <div class="grid grid-cols-2 gap-4 mb-4">
        <div class="bg-gray-50 rounded-lg p-3">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs font-medium text-gray-600 uppercase tracking-wider">Capacity</p>
              <p class="text-lg font-semibold text-gray-900">{{ formatNumber(berth.capacity) }}</p>
              <p class="text-xs text-gray-500">Containers</p>
            </div>
            <Package class="w-8 h-8 text-gray-400" />
          </div>
        </div>
        
        <div class="bg-gray-50 rounded-lg p-3">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs font-medium text-gray-600 uppercase tracking-wider">Current</p>
              <p class="text-lg font-semibold text-gray-900">{{ berth.currentOccupancy || 0 }}</p>
              <p class="text-xs text-gray-500">In Use</p>
            </div>
            <div class="text-right">
              <div class="text-sm font-medium" :class="getUtilizationTextClass(utilizationPercentage)">
                {{ utilizationPercentage }}%
              </div>
              <div class="w-16 bg-gray-200 rounded-full h-2 mt-1">
                <div 
                  class="h-2 rounded-full transition-all duration-300"
                  :class="getUtilizationBarClass(utilizationPercentage)"
                  :style="{ width: `${Math.min(utilizationPercentage, 100)}%` }"
                ></div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Physical Specifications -->
      <div class="grid grid-cols-3 gap-4 mb-4">
        <div class="text-center">
          <div class="text-xs font-medium text-gray-600 uppercase tracking-wider mb-1">Length</div>
          <div class="text-sm font-semibold text-gray-900">
            {{ berth.length ? berth.length + 'm' : '-' }}
          </div>
        </div>
        <div class="text-center">
          <div class="text-xs font-medium text-gray-600 uppercase tracking-wider mb-1">Water Depth</div>
          <div class="text-sm font-semibold text-gray-900">
            {{ berth.waterDepth ? berth.waterDepth + 'm' : '-' }}
          </div>
        </div>
        <div class="text-center">
          <div class="text-xs font-medium text-gray-600 uppercase tracking-wider mb-1">Max Draft</div>
          <div class="text-sm font-semibold text-gray-900">
            {{ berth.maxDraft ? berth.maxDraft + 'm' : '-' }}
          </div>
        </div>
      </div>

      <!-- Berth Type and Equipment -->
      <div class="mb-4">
        <div class="flex items-center justify-between text-sm">
          <span class="text-gray-600">Type:</span>
          <span v-if="berth.berthType" class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
            {{ berth.berthType }}
          </span>
          <span v-else class="text-gray-400">Not specified</span>
        </div>
        
        <div v-if="berth.craneCount || berth.craneCapacity" class="flex items-center justify-between text-sm mt-2">
          <span class="text-gray-600">Equipment:</span>
          <div class="flex items-center space-x-2">
            <span v-if="berth.craneCount" class="inline-flex items-center px-2 py-1 rounded text-xs bg-gray-100 text-gray-700">
              <Zap class="w-3 h-3 mr-1" />
              {{ berth.craneCount }} crane{{ berth.craneCount === 1 ? '' : 's' }}
            </span>
            <span v-if="berth.craneCapacity" class="inline-flex items-center px-2 py-1 rounded text-xs bg-gray-100 text-gray-700">
              {{ berth.craneCapacity }}t capacity
            </span>
          </div>
        </div>
      </div>

      <!-- Features -->
      <div v-if="hasFeatures" class="mb-4">
        <div class="text-xs font-medium text-gray-600 uppercase tracking-wider mb-2">Features</div>
        <div class="flex flex-wrap gap-1">
          <span v-if="berth.features?.refrigerated" class="inline-flex items-center px-2 py-1 rounded text-xs bg-blue-100 text-blue-700">
            <Snowflake class="w-3 h-3 mr-1" />
            Refrigerated
          </span>
          <span v-if="berth.features?.dangerous" class="inline-flex items-center px-2 py-1 rounded text-xs bg-red-100 text-red-700">
            <AlertTriangle class="w-3 h-3 mr-1" />
            Dangerous Goods
          </span>
          <span v-if="berth.features?.oversized" class="inline-flex items-center px-2 py-1 rounded text-xs bg-purple-100 text-purple-700">
            <Maximize class="w-3 h-3 mr-1" />
            Oversized
          </span>
          <span v-if="berth.features?.heavyLift" class="inline-flex items-center px-2 py-1 rounded text-xs bg-orange-100 text-orange-700">
            <Weight class="w-3 h-3 mr-1" />
            Heavy Lift
          </span>
          <span v-if="berth.features?.railConnection" class="inline-flex items-center px-2 py-1 rounded text-xs bg-green-100 text-green-700">
            <Train class="w-3 h-3 mr-1" />
            Rail
          </span>
          <span v-if="berth.features?.roadAccess" class="inline-flex items-center px-2 py-1 rounded text-xs bg-indigo-100 text-indigo-700">
            <Truck class="w-3 h-3 mr-1" />
            Road
          </span>
        </div>
      </div>

      <!-- Operating Hours -->
      <div class="flex items-center justify-between text-sm mb-4">
        <span class="text-gray-600">Operating Hours:</span>
        <div class="flex items-center text-gray-900">
          <Clock class="w-4 h-4 mr-1 text-gray-400" />
          {{ berth.operatingHours || '24/7' }}
        </div>
      </div>

      <!-- Active Assignments -->
      <div v-if="berth.activeAssignmentCount !== undefined" class="flex items-center justify-between text-sm">
        <span class="text-gray-600">Active Assignments:</span>
        <div class="flex items-center">
          <Users class="w-4 h-4 mr-1 text-gray-400" />
          <span class="font-medium text-gray-900">{{ berth.activeAssignmentCount }}</span>
        </div>
      </div>

      <!-- Notes -->
      <div v-if="berth.notes" class="mt-4 pt-4 border-t border-gray-200">
        <div class="text-xs font-medium text-gray-600 uppercase tracking-wider mb-1">Notes</div>
        <p class="text-sm text-gray-700 line-clamp-2">{{ berth.notes }}</p>
      </div>
    </div>

    <!-- Card Footer (if compact mode) -->
    <div v-if="compact" class="px-6 py-3 bg-gray-50 border-t border-gray-200">
      <div class="flex items-center justify-between">
        <button
          @click="$emit('view', berth)"
          class="text-xs font-medium text-blue-600 hover:text-blue-800"
        >
          View Details
        </button>
        <div class="flex space-x-2">
          <button
            @click="$emit('edit', berth)"
            class="text-xs font-medium text-indigo-600 hover:text-indigo-800"
          >
            Edit
          </button>
          <button
            @click="$emit('assignments', berth)"
            class="text-xs font-medium text-green-600 hover:text-green-800"
          >
            Assignments
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { 
  MapPin, 
  Building, 
  Package, 
  Eye, 
  Edit, 
  Users, 
  Trash2, 
  MoreVertical,
  Clock,
  Zap,
  Snowflake,
  AlertTriangle,
  Maximize,
  Weight,
  Train,
  Truck
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
  maxDraft?: number;
  craneCount?: number;
  craneCapacity?: number;
  features?: {
    refrigerated?: boolean;
    dangerous?: boolean;
    oversized?: boolean;
    heavyLift?: boolean;
    railConnection?: boolean;
    roadAccess?: boolean;
  };
  operatingHours?: string;
  notes?: string;
  activeAssignmentCount?: number;
}

interface Props {
  berth: Berth;
  compact?: boolean;
  showActions?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  compact: false,
  showActions: true
});

const emit = defineEmits<{
  view: [berth: Berth];
  edit: [berth: Berth];
  assignments: [berth: Berth];
  delete: [berth: Berth];
}>();

const showMenu = ref(false);

const utilizationPercentage = computed(() => {
  if (!props.berth.capacity) return 0;
  return Math.round(((props.berth.currentOccupancy || 0) / props.berth.capacity) * 100);
});

const hasFeatures = computed(() => {
  return props.berth.features && Object.values(props.berth.features).some(feature => feature);
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

const getUtilizationBarClass = (percentage: number): string => {
  if (percentage >= 90) return 'bg-red-500';
  if (percentage >= 75) return 'bg-yellow-500';
  if (percentage >= 50) return 'bg-blue-500';
  return 'bg-green-500';
};

const getUtilizationTextClass = (percentage: number): string => {
  if (percentage >= 90) return 'text-red-600';
  if (percentage >= 75) return 'text-yellow-600';
  if (percentage >= 50) return 'text-blue-600';
  return 'text-green-600';
};

const formatNumber = (num: number): string => {
  if (!num) return '0';
  return new Intl.NumberFormat('en-US').format(num);
};

const handleAction = (action: string) => {
  showMenu.value = false;
  
  switch (action) {
    case 'view':
      emit('view', props.berth);
      break;
    case 'edit':
      emit('edit', props.berth);
      break;
    case 'assignments':
      emit('assignments', props.berth);
      break;
    case 'delete':
      emit('delete', props.berth);
      break;
  }
};

// Simple click outside directive implementation
const vClickOutside = {
  beforeMount(el: any, binding: any) {
    el.clickOutsideEvent = (event: Event) => {
      if (!(el === event.target || el.contains(event.target))) {
        binding.value();
      }
    };
    document.addEventListener('click', el.clickOutsideEvent);
  },
  unmounted(el: any) {
    document.removeEventListener('click', el.clickOutsideEvent);
  }
};
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>