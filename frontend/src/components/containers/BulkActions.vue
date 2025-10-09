<template>
  <div class="flex items-center justify-between bg-blue-50 border border-blue-200 rounded-lg p-4 mb-6">
    <div class="flex items-center space-x-4">
      <span class="font-medium">{{ selectedCount }} containers selected</span>
      <select
        :value="bulkStatusUpdate"
        @change="$emit('update-bulk-status', $event.target.value)"
        class="border border-gray-300 rounded px-2 py-1"
      >
        <option value="">Change Status To...</option>
        <option v-for="status in statusOptions" :key="status" :value="status">
          {{ status }}
        </option>
      </select>
      <button
        @click="$emit('perform-bulk-update')"
        :disabled="!bulkStatusUpdate"
        class="px-3 py-1 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
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
interface Props {
  selectedCount: number;
  statusOptions: string[];
  bulkStatusUpdate: string;
}

defineProps<Props>();

defineEmits<{
  'update-bulk-status': [value: string];
  'perform-bulk-update': [];
  'clear-selection': [];
}>();
</script>
