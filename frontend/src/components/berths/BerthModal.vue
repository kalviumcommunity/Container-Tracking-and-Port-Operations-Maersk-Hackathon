<template>
  <div class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
    <div class="relative top-10 mx-auto p-5 border w-full max-w-3xl shadow-lg rounded-md bg-white">
      <div class="mt-3">
        <h3 class="text-lg font-medium text-gray-900 mb-6 flex items-center">
          <MapPin class="w-5 h-5 mr-2 text-blue-600" />
          {{ isEditing ? 'Edit Berth' : 'Create New Berth' }}
        </h3>
        
        <form @submit.prevent="handleSubmit" class="space-y-6">
          <!-- Berth Name and Basic Info -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Berth Name *</label>
              <input
                v-model="form.name"
                type="text"
                required
                placeholder="e.g., Berth A1, Terminal 3"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
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

          <!-- Port Assignment and Capacity -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Port Assignment *</label>
              <select
                v-model="form.portId"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Select Port</option>
                <option v-for="port in portOptions" :key="port.id" :value="port.id">
                  {{ port.name }}
                </option>
              </select>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Container Capacity *</label>
              <input
                v-model.number="form.capacity"
                type="number"
                min="1"
                required
                placeholder="e.g., 500"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
          </div>

          <!-- Berth Identifier and Type -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Berth Identifier</label>
              <input
                v-model="form.identifier"
                type="text"
                placeholder="e.g., B-001, WHARF-A1"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Berth Type</label>
              <select
                v-model="form.type"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Select Type</option>
                <option value="Container">Container</option>
                <option value="Bulk">Bulk</option>
                <option value="Tanker">Tanker</option>
                <option value="RoRo">RoRo</option>
                <option value="Multipurpose">Multipurpose</option>
                <option value="General Cargo">General Cargo</option>
              </select>
            </div>
          </div>

          <!-- Ship Specifications -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Max Ship Length (meters)</label>
              <input
                v-model.number="form.maxShipLength"
                type="number"
                min="0"
                step="0.1"
                placeholder="e.g., 300"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Maximum Draft (meters)</label>
              <input
                v-model.number="form.maxDraft"
                type="number"
                min="0"
                step="0.1"
                placeholder="e.g., 14.0"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
          </div>

          <!-- Equipment and Financial -->
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Crane Count</label>
              <input
                v-model.number="form.craneCount"
                type="number"
                min="0"
                placeholder="e.g., 4"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Hourly Rate ($)</label>
              <input
                v-model.number="form.hourlyRate"
                type="number"
                min="0"
                step="0.01"
                placeholder="e.g., 250.00"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Priority</label>
              <input
                v-model="form.priority"
                type="text"
                placeholder="High, Medium, Low"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
          </div>

          <!-- Available Services -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Available Services</label>
            <input
              v-model="form.availableServices"
              type="text"
              placeholder="e.g., Crane, Refueling, Maintenance, Loading"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            />
            <p class="mt-1 text-sm text-gray-500">Enter services separated by commas</p>
          </div>

          <!-- Current Load (for editing) -->
          <div v-if="isEditing">
            <label class="block text-sm font-medium text-gray-700 mb-2">Current Load</label>
            <input
              v-model.number="form.currentLoad"
              type="number"
              min="0"
              :max="form.capacity"
              placeholder="Current container count"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            />
          </div>

          <!-- Notes and Additional Information -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Notes</label>
            <textarea
              v-model="form.notes"
              rows="3"
              placeholder="Additional information about the berth, special requirements, restrictions, etc."
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            ></textarea>
          </div>

          <!-- Current Utilization (for editing existing berths) -->
          <div v-if="isEditing" class="grid grid-cols-1 md:grid-cols-2 gap-4 bg-gray-50 p-4 rounded-lg">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Current Load</label>
              <div class="text-lg font-semibold text-blue-600">
                {{ form.currentLoad || 0 }}/{{ form.capacity }}
              </div>
              <div class="text-xs text-gray-500">Containers</div>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Utilization Rate</label>
              <div class="text-lg font-semibold text-green-600">
                {{ Math.round(((form.currentLoad || 0) / form.capacity) * 100) }}%
              </div>
              <div class="text-xs text-gray-500">Capacity Used</div>
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
              {{ isSubmitting ? 'Saving...' : (isEditing ? 'Update Berth' : 'Create Berth') }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';
import { MapPin, Loader2 } from 'lucide-vue-next';

const props = defineProps<{
  isEditing: boolean;
  berth?: any;
  statusOptions: string[];
  portOptions: Array<{ id: number; name: string }>;
  isSubmitting: boolean;
}>();

const emit = defineEmits<{
  submit: [data: any];
  cancel: [];
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

const handleSubmit = () => {
  const submissionData: any = { ...form.value };
  
  // Ensure proper data types
  if (submissionData.portId) {
    submissionData.portId = parseInt(String(submissionData.portId));
  }
  
  // Handle null/empty values properly - keep as null for optional fields
  if (!submissionData.maxShipLength) {
    submissionData.maxShipLength = null;
  }
  if (!submissionData.maxDraft) {
    submissionData.maxDraft = null;
  }
  if (!submissionData.craneCount) {
    submissionData.craneCount = null;
  }
  if (!submissionData.hourlyRate) {
    submissionData.hourlyRate = null;
  }
  
  emit('submit', submissionData);
};

// Initialize form when component mounts
onMounted(() => {
  if (props.berth) {
    Object.assign(form.value, props.berth);
  }
});
</script>