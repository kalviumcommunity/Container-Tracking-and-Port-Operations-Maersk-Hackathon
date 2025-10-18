<!-- Simple Assignment Modal Component -->
<template>
  <div class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
    <div class="bg-white rounded-xl shadow-xl max-w-lg w-full">
      <div class="p-6 border-b border-gray-200">
        <h3 class="text-lg font-semibold text-gray-900">Create Assignment</h3>
        <p class="text-sm text-gray-600">Assign a ship or container to a berth</p>
      </div>
      
      <div class="p-6">
        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Berth</label>
            <select v-model="formData.berthId" class="w-full px-3 py-2 border border-gray-300 rounded-md">
              <option value="">Select berth...</option>
              <option value="1">Berth A1</option>
              <option value="2">Berth A2</option>
            </select>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Ship</label>
            <select v-model="formData.shipId" class="w-full px-3 py-2 border border-gray-300 rounded-md">
              <option value="">Select ship...</option>
              <option v-for="ship in availableShips" :key="ship.id" :value="ship.id">
                {{ ship.name }}
              </option>
            </select>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Assignment Type</label>
            <select v-model="formData.assignmentType" class="w-full px-3 py-2 border border-gray-300 rounded-md">
              <option value="Loading">Loading</option>
              <option value="Unloading">Unloading</option>
              <option value="Storage">Storage</option>
              <option value="Maintenance">Maintenance</option>
            </select>
          </div>
        </div>
      </div>
      
      <div class="p-6 border-t border-gray-200 flex justify-end space-x-3">
        <button
          @click="$emit('cancel')"
          class="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-md hover:bg-gray-200"
        >
          Cancel
        </button>
        <button
          @click="handleSubmit"
          :disabled="isSubmitting"
          class="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-md hover:bg-blue-700 disabled:opacity-50"
        >
          {{ isSubmitting ? 'Creating...' : 'Create Assignment' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

interface Props {
  berth?: any;
  availableShips: any[];
  availableContainers: any[];
  isSubmitting: boolean;
}

withDefaults(defineProps<Props>(), {
  availableShips: () => [],
  availableContainers: () => [],
  isSubmitting: false
});

const emit = defineEmits<{
  submit: [data: any];
  cancel: [];
}>();

const formData = ref({
  berthId: '',
  shipId: '',
  assignmentType: 'Loading'
});

const handleSubmit = () => {
  emit('submit', formData.value);
};
</script>
