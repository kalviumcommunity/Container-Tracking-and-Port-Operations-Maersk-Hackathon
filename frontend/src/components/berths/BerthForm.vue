<template>
  <div class="bg-white rounded-lg shadow-md p-6">
    <div class="mb-6">
      <h3 class="text-lg font-semibold text-gray-900 flex items-center">
        <MapPin class="w-5 h-5 mr-2 text-blue-600" />
        {{ title || (isEditing ? 'Edit Berth Information' : 'Create New Berth') }}
      </h3>
      <p v-if="subtitle" class="text-sm text-gray-600 mt-1">{{ subtitle }}</p>
    </div>

    <form @submit.prevent="handleSubmit" class="space-y-6">
      <!-- Basic Information Section -->
      <div class="border-b border-gray-200 pb-6">
        <h4 class="text-md font-medium text-gray-900 mb-4">Basic Information</h4>
        
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Berth Name *
              <span class="text-xs text-gray-500">(e.g., Berth A1, Terminal 3)</span>
            </label>
            <input
              v-model="form.name"
              type="text"
              required
              placeholder="Enter berth name"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              :class="{ 'border-red-300': errors.name }"
            />
            <p v-if="errors.name" class="mt-1 text-xs text-red-600">{{ errors.name }}</p>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Status *</label>
            <select
              v-model="form.status"
              required
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              :class="{ 'border-red-300': errors.status }"
            >
              <option value="">Select Status</option>
              <option v-for="status in statusOptions" :key="status" :value="status">
                {{ status }}
              </option>
            </select>
            <p v-if="errors.status" class="mt-1 text-xs text-red-600">{{ errors.status }}</p>
          </div>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Port Assignment *</label>
            <select
              v-model="form.portId"
              required
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              :class="{ 'border-red-300': errors.portId }"
            >
              <option value="">Select Port</option>
              <option v-for="port in portOptions" :key="port.id" :value="port.id">
                {{ port.name }}
              </option>
            </select>
            <p v-if="errors.portId" class="mt-1 text-xs text-red-600">{{ errors.portId }}</p>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Berth Identifier *
              <span class="text-xs text-gray-500">(e.g., B-001, A-12)</span>
            </label>
            <input
              v-model="form.identifier"
              type="text"
              required
              placeholder="e.g., B-001"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              :class="{ 'border-red-300': errors.identifier }"
            />
            <p v-if="errors.identifier" class="mt-1 text-xs text-red-600">{{ errors.identifier }}</p>
          </div>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Container Capacity *
              <span class="text-xs text-gray-500">(Number of containers)</span>
            </label>
            <input
              v-model.number="form.capacity"
              type="number"
              min="1"
              required
              placeholder="e.g., 500"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              :class="{ 'border-red-300': errors.capacity }"
            />
            <p v-if="errors.capacity" class="mt-1 text-xs text-red-600">{{ errors.capacity }}</p>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Current Load
              <span class="text-xs text-gray-500">(Current containers)</span>
            </label>
            <input
              v-model.number="form.currentLoad"
              type="number"
              min="0"
              placeholder="e.g., 250"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            />
          </div>
        </div>
      </div>

      <!-- Physical Specifications Section -->
      <div class="border-b border-gray-200 pb-6">
        <h4 class="text-md font-medium text-gray-900 mb-4">Physical Specifications</h4>
        
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Berth Type *</label>
            <select
              v-model="form.type"
              required
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              :class="{ 'border-red-300': errors.type }"
            >
              <option value="">Select Type</option>
              <option value="Container">Container</option>
              <option value="Bulk">Bulk</option>
              <option value="Tanker">Tanker</option>
              <option value="RoRo">RoRo</option>
              <option value="Multipurpose">Multipurpose</option>
            </select>
            <p v-if="errors.type" class="mt-1 text-xs text-red-600">{{ errors.type }}</p>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Max Ship Length
              <span class="text-xs text-gray-500">(meters)</span>
            </label>
            <input
              v-model.number="form.maxShipLength"
              type="number"
              min="1"
              step="0.1"
              placeholder="e.g., 300"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            />
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Maximum Draft
              <span class="text-xs text-gray-500">(meters)</span>
            </label>
            <input
              v-model.number="form.maxDraft"
              type="number"
              min="0"
              step="0.1"
              placeholder="e.g., 14.0"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            />
          </div>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mt-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Crane Count</label>
            <input
              v-model.number="form.craneCount"
              type="number"
              min="0"
              placeholder="e.g., 4"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            />
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Hourly Rate
              <span class="text-xs text-gray-500">(currency per hour)</span>
            </label>
            <input
              v-model.number="form.hourlyRate"
              type="number"
              min="0"
              step="0.01"
              placeholder="e.g., 150.00"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            />
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Priority Level</label>
            <select
              v-model="form.priority"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            >
              <option value="">Select Priority</option>
              <option value="High">High</option>
              <option value="Medium">Medium</option>
              <option value="Low">Low</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Available Services Section -->
      <div class="border-b border-gray-200 pb-6">
        <h4 class="text-md font-medium text-gray-900 mb-4">Available Services</h4>
        
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">
            Services Offered
            <span class="text-xs text-gray-500">(Comma-separated list)</span>
          </label>
          <textarea
            v-model="form.availableServices"
            placeholder="e.g., Crane, Refueling, Maintenance, Refrigerated Storage, Dangerous Goods Handling"
            rows="3"
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors resize-none"
          ></textarea>
          <p class="mt-1 text-xs text-gray-500">
            List all available services at this berth (e.g., Crane, Refueling, Maintenance, Refrigerated Storage, Dangerous Goods Handling, Rail Connection, etc.)
          </p>
        </div>
      </div>

      <!-- Additional Information Section -->
      <div class="pb-6">
        <h4 class="text-md font-medium text-gray-900 mb-4">Additional Information</h4>
        
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Notes</label>
          <textarea
            v-model="form.notes"
            rows="4"
            placeholder="Additional information about the berth, special requirements, restrictions, operational notes, etc."
            class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors resize-none"
          ></textarea>
          <div class="text-xs text-gray-500 mt-1">
            {{ form.notes ? form.notes.length : 0 }}/500 characters
          </div>
        </div>
      </div>

      <!-- Current Status (for editing existing berths) -->
      <div v-if="isEditing && showCurrentStatus" class="border-t border-gray-200 pt-6">
        <h4 class="text-md font-medium text-gray-900 mb-4">Current Status</h4>
        
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4 bg-gray-50 p-4 rounded-lg">
          <div class="text-center">
            <div class="text-2xl font-bold text-blue-600">
              {{ form.currentLoad || 0 }}/{{ form.capacity }}
            </div>
            <div class="text-sm text-gray-600">Current Occupancy</div>
            <div class="text-xs text-gray-500">Containers</div>
          </div>
          
          <div class="text-center">
            <div class="text-2xl font-bold text-green-600">
              {{ Math.round(((form.currentLoad || 0) / form.capacity) * 100) }}%
            </div>
            <div class="text-sm text-gray-600">Utilization Rate</div>
            <div class="text-xs text-gray-500">Capacity Used</div>
          </div>
          
          <div class="text-center">
            <div class="text-2xl font-bold text-purple-600">
              {{ form.craneCount || 0 }}
            </div>
            <div class="text-sm text-gray-600">Available Cranes</div>
            <div class="text-xs text-gray-500">Equipment Count</div>
          </div>
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="flex justify-end space-x-4 pt-6">
        <button
          v-if="showCancelButton"
          type="button"
          @click="handleCancel"
          class="px-6 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-gray-500 transition-colors"
        >
          Cancel
        </button>
        <button
          v-if="showResetButton"
          type="button"
          @click="handleReset"
          class="px-6 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-gray-500 transition-colors"
        >
          Reset
        </button>
        <button
          type="submit"
          :disabled="isSubmitting || !isFormValid"
          class="px-8 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed flex items-center transition-colors"
        >
          <Loader2 v-if="isSubmitting" class="w-4 h-4 mr-2 animate-spin" />
          {{ isSubmitting ? 'Saving...' : (isEditing ? 'Update Berth' : 'Create Berth') }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue';
import { 
  MapPin, 
  Loader2,
  Snowflake,
  AlertTriangle,
  Maximize,
  Weight,
  Train,
  Truck
} from 'lucide-vue-next';

interface Props {
  isEditing?: boolean;
  berth?: any;
  statusOptions?: string[];
  portOptions?: Array<{ id: number; name: string }>;
  isSubmitting?: boolean;
  title?: string;
  subtitle?: string;
  showCancelButton?: boolean;
  showResetButton?: boolean;
  showCurrentStatus?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: false,
  isSubmitting: false,
  statusOptions: () => ['Available', 'Occupied', 'Under Maintenance', 'Reserved', 'Full', 'Partially Occupied', 'Inactive'],
  portOptions: () => [],
  showCancelButton: true,
  showResetButton: false,
  showCurrentStatus: true
});

const emit = defineEmits<{
  submit: [data: any];
  cancel: [];
  reset: [];
}>();

const form = ref({
  // Backend BerthCreateUpdateDto fields
  name: '',
  status: '',
  portId: null,
  capacity: 0,
  
  // Backend Berth model additional fields
  identifier: '',
  type: '',
  currentLoad: 0,
  maxShipLength: null,
  maxDraft: null,
  availableServices: '',
  craneCount: null,
  hourlyRate: null,
  priority: '',
  notes: ''
});

const errors = ref({
  name: '',
  status: '',
  portId: '',
  capacity: '',
  identifier: '',
  type: ''
});

const isFormValid = computed(() => {
  return form.value.name && 
         form.value.status && 
         form.value.portId && 
         form.value.capacity > 0 &&
         form.value.identifier &&
         form.value.type &&
         !Object.values(errors.value).some(error => error);
});

// Watch for berth prop changes
watch(() => props.berth, (newBerth) => {
  if (newBerth) {
    Object.assign(form.value, {
      name: newBerth.name || '',
      status: newBerth.status || '',
      portId: newBerth.portId || null,
      capacity: newBerth.capacity || 0,
      identifier: newBerth.identifier || '',
      type: newBerth.type || '',
      currentLoad: newBerth.currentLoad || 0,
      maxShipLength: newBerth.maxShipLength || null,
      maxDraft: newBerth.maxDraft || null,
      availableServices: newBerth.availableServices || '',
      craneCount: newBerth.craneCount || null,
      hourlyRate: newBerth.hourlyRate || null,
      priority: newBerth.priority || '',
      notes: newBerth.notes || ''
    });
  }
}, { immediate: true });

// Form validation
const validateForm = () => {
  errors.value = {
    name: '',
    status: '',
    portId: '',
    capacity: '',
    identifier: '',
    type: ''
  };

  if (!form.value.name.trim()) {
    errors.value.name = 'Berth name is required';
  } else if (form.value.name.length < 2) {
    errors.value.name = 'Berth name must be at least 2 characters';
  }

  if (!form.value.status) {
    errors.value.status = 'Status is required';
  }

  if (!form.value.portId) {
    errors.value.portId = 'Port assignment is required';
  }

  if (!form.value.capacity || form.value.capacity <= 0) {
    errors.value.capacity = 'Capacity must be greater than 0';
  }
};

const handleSubmit = () => {
  validateForm();
  
  if (!isFormValid.value) {
    return;
  }

  const submissionData = { ...form.value };
  
  // Ensure proper data types
  if (submissionData.portId) {
    submissionData.portId = parseInt(submissionData.portId.toString());
  }
  
  // Handle null/empty values properly
  ['portId', 'length', 'waterDepth', 'maxDraft', 'craneCount', 'craneCapacity'].forEach(field => {
    if (!submissionData[field] || submissionData[field] === '') {
      submissionData[field] = null;
    }
  });

  // Limit notes length
  if (submissionData.notes && submissionData.notes.length > 500) {
    submissionData.notes = submissionData.notes.substring(0, 500);
  }
  
  emit('submit', submissionData);
};

const handleCancel = () => {
  emit('cancel');
};

const handleReset = () => {
  // Reset form to initial state
  Object.assign(form.value, {
    name: '',
    status: '',
    portId: null,
    capacity: 0,
    identifier: '',
    type: '',
    currentLoad: 0,
    maxShipLength: null,
    maxDraft: null,
    availableServices: '',
    craneCount: null,
    hourlyRate: null,
    priority: '',
    notes: ''
  });
  
  // Clear errors
  errors.value = {
    name: '',
    status: '',
    portId: '',
    capacity: '',
    identifier: '',
    type: ''
  };
  
  emit('reset');
};

// Initialize form when component mounts
onMounted(() => {
  if (props.berth) {
    Object.assign(form.value, props.berth);
  }
});
</script>
