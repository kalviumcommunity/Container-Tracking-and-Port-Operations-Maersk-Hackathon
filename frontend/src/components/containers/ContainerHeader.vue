<template>
  <div class="flex justify-between items-center mb-6">
    <div>
      <h1 class="text-3xl font-bold text-gray-900">Container Management</h1>
      <p class="mt-1 text-sm text-gray-600">
        Manage and track containers across global ports
      </p>
    </div>
    <div class="flex space-x-3">
      <button
        @click="$emit('refresh')"
        :disabled="isLoading"
        class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
      >
        <RefreshCwIcon class="w-4 h-4 mr-2" />
        Refresh
      </button>
      <button
        @click="$emit('export')"
        :disabled="isLoading"
        class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50"
      >
        <DownloadIcon class="w-4 h-4 mr-2" />
        Export CSV
      </button>
      <button
        v-if="canManage"
        @click="$emit('create')"
        class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
      >
        <PlusIcon class="w-4 h-4 mr-2" />
        Create Container
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { RefreshCwIcon, DownloadIcon, PlusIcon } from 'lucide-vue-next';

defineProps<{
  isLoading: boolean;
  canManage: boolean;
}>();

defineEmits<{
  refresh: [];
  export: [];
  create: [];
}>();
</script>
