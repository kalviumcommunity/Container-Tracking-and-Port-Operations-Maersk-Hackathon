<template>
  <div class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
    <div class="relative top-10 mx-auto p-5 border w-full max-w-4xl shadow-lg rounded-md bg-white">
      <div class="mt-3">
        <h3 class="text-lg font-medium text-gray-900 mb-6 flex items-center">
          <Package class="w-5 h-5 mr-2 text-blue-600" />
          {{ isEditing ? 'Edit Container' : 'Create New Container' }}
        </h3>
        
        <form @submit.prevent="handleSubmit" class="space-y-6">
          <!-- Container ID and Basic Info -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Container ID *</label>
              <input
                v-model="form.containerId"
                :disabled="isEditing"
                type="text"
                required
                placeholder="e.g., MAEU1234567"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                :class="{ 'bg-gray-100': isEditing }"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Status *</label>
              <select
                v-model="form.status"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Select Status</option>
                <option v-for="status in statusOptions" :key="status" :value="status">
                  {{ status }}
                </option>
              </select>
            </div>
          </div>

          <!-- Type and Condition -->
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Container Type *</label>
              <select
                v-model="form.type"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Select Type</option>
                <option v-for="type in typeOptions" :key="type" :value="type">
                  {{ type }}
                </option>
              </select>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Size</label>
              <select
                v-model="form.size"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Select Size</option>
                <option value="20ft">20ft</option>
                <option value="40ft">40ft</option>
                <option value="45ft">45ft</option>
                <option value="53ft">53ft</option>
              </select>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Condition</label>
              <select
                v-model="form.condition"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Select Condition</option>
                <option value="Good">Good</option>
                <option value="Damaged">Damaged</option>
                <option value="Needs Repair">Needs Repair</option>
                <option value="Under Maintenance">Under Maintenance</option>
                <option value="Excellent">Excellent</option>
              </select>
            </div>
          </div>

          <!-- Cargo Information -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Cargo Type</label>
              <select
                v-model="form.cargoType"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Select Cargo Type</option>
                <option v-for="cargoType in cargoTypeOptions" :key="cargoType" :value="cargoType">
                  {{ cargoType }}
                </option>
              </select>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Ship Assignment</label>
              <select
                v-model="form.shipId"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">No Ship Assigned</option>
                <option v-for="ship in shipOptions" :key="ship.id" :value="ship.id">
                  {{ ship.name }}
                </option>
              </select>
            </div>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Cargo Description</label>
            <textarea
              v-model="form.cargoDescription"
              rows="3"
              placeholder="Detailed description of cargo contents..."
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            ></textarea>
          </div>

          <!-- Location Information -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Current Location</label>
              <input
                v-model="form.currentLocation"
                type="text"
                placeholder="e.g., Port of Copenhagen"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Destination</label>
              <input
                v-model="form.destination"
                type="text"
                placeholder="e.g., Port of Hamburg"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
          </div>

          <!-- Weight and Temperature -->
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Weight (kg)</label>
              <input
                v-model.number="form.weight"
                type="number"
                min="0"
                step="0.1"
                placeholder="25000"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Max Weight (kg)</label>
              <input
                v-model.number="form.maxWeight"
                type="number"
                min="0"
                step="0.1"
                placeholder="30000"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            
            <div v-if="form.type === 'Refrigerated'">
              <label class="block text-sm font-medium text-gray-700 mb-2">Temperature (°C)</label>
              <input
                v-model.number="form.temperature"
                type="number"
                step="0.1"
                placeholder="-18"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
          </div>

          <!-- Advanced Fields -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">GPS Coordinates</label>
              <input
                v-model="form.coordinates"
                type="text"
                placeholder="55.6761° N, 12.5683° E"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Estimated Arrival</label>
              <input
                v-model="form.estimatedArrival"
                type="datetime-local"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="flex justify-end space-x-4 pt-6 border-t border-gray-200">
            <button
              type="button"
              @click="$emit('cancel')"
              class="px-4 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-gray-500"
            >
              Cancel
            </button>
            <button
              type="submit"
              :disabled="isSubmitting"
              class="px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed flex items-center"
            >
              <Loader2 v-if="isSubmitting" class="w-4 h-4 mr-2 animate-spin" />
              {{ isSubmitting ? 'Saving...' : (isEditing ? 'Update Container' : 'Create Container') }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';
import { Package, Loader2 } from 'lucide-vue-next';

const props = defineProps<{
  isEditing: boolean;
  container?: any;
  cargoTypeOptions: string[];
  typeOptions: string[];
  statusOptions: string[];
  shipOptions: Array<{ id: number; name: string }>;
  isSubmitting: boolean;
}>();

const emit = defineEmits<{
  submit: [data: any];
  cancel: [];
}>();

const form = ref({
  containerId: '',
  cargoType: '',
  cargoDescription: '',
  type: '',
  status: '',
  condition: 'Good',
  currentLocation: '',
  destination: '',
  weight: 0,
  maxWeight: null,
  size: '',
  temperature: null,
  coordinates: '',
  estimatedArrival: '',
  shipId: null
});

// Watch for container prop changes
watch(() => props.container, (newContainer) => {
  console.log('Container prop changed:', newContainer);
  
  if (newContainer) {
    Object.assign(form.value, {
      containerId: newContainer.containerId || '',
      cargoType: newContainer.cargoType || '',
      cargoDescription: newContainer.cargoDescription || '',
      type: newContainer.type || '',
      status: newContainer.status || '',
      condition: newContainer.condition || 'Good',
      currentLocation: newContainer.currentLocation || '',
      destination: newContainer.destination || '',
      weight: newContainer.weight || 0,
      maxWeight: newContainer.maxWeight || null,
      size: newContainer.size || '',
      temperature: newContainer.temperature || null,
      coordinates: newContainer.coordinates || '',
      estimatedArrival: newContainer.estimatedArrival ? 
        new Date(newContainer.estimatedArrival).toISOString().slice(0, 16) : '',
      shipId: newContainer.shipId || null
    });
    
    console.log('Form populated with:', form.value);
  }
}, { immediate: true });

const handleSubmit = () => {
  const submissionData = { ...form.value };
  
  // Ensure proper data types
  if (submissionData.shipId) {
    submissionData.shipId = parseInt(submissionData.shipId.toString());
  }
  
  // Handle null/empty values properly
  if (!submissionData.maxWeight || submissionData.maxWeight === '') {
    submissionData.maxWeight = null;
  }
  if (!submissionData.temperature || submissionData.temperature === '') {
    submissionData.temperature = null;
  }
  if (!submissionData.shipId || submissionData.shipId === '') {
    submissionData.shipId = null;
  }
  if (!submissionData.estimatedArrival || submissionData.estimatedArrival === '') {
    submissionData.estimatedArrival = null;
  }
  
  console.log('Submitting form data:', submissionData);
  emit('submit', submissionData);
};

// Initialize form when component mounts
onMounted(() => {
  if (props.container) {
    Object.assign(form.value, props.container);
  }
});
</script>
