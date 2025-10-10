<template>
  <div class="flex items-center justify-between bg-blue-50 border border-blue-200 rounded-lg p-4 mb-6">
    <div class="flex items-center space-x-4">
      <span class="font-medium">{{ selectedCount }} containers selected</span>
      <select
        v-model="bulkStatus"
        class="border border-gray-300 rounded px-2 py-1"
      >
        <option value="">Change Status To...</option>
        <option v-for="status in statusOptions" :key="status" :value="status">
          {{ status }}
        </option>
      </select>
      <button
        @click="handleBulkUpdate"
        :disabled="!bulkStatus"
        class="px-3 py-1 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
      >
        Apply
      </button>
    </div>
    <button
      @click="$emit('clear-selection')"
      class="text-sm text-gray-500 hover:text-gray-700"
    >
      Clear Selection
    </button>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const props = defineProps<{
  selectedCount: number;
  statusOptions: string[];
}>();

const emit = defineEmits<{
  'bulk-update': [status: string];
  'clear-selection': [];
}>();

const bulkStatus = ref('');

const handleBulkUpdate = () => {
  if (bulkStatus.value) {
    emit('bulk-update', bulkStatus.value);
    bulkStatus.value = '';
  }
};
</script>
