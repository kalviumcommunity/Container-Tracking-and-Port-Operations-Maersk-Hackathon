<template>
  <div class="bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
    <div class="flex items-center">
      <AlertTriangleIcon class="w-5 h-5 text-red-400 mr-3" />
      <div>
        <h3 class="text-sm font-medium text-red-800">Error loading containers</h3>
        <div class="mt-1 text-sm text-red-700">{{ error }}</div>
        <button
          @click="$emit('retry')"
          class="mt-2 bg-red-100 px-2 py-1 rounded text-sm text-red-800 hover:bg-red-200"
        >
          Try Again
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { AlertTriangleIcon } from 'lucide-vue-next';

defineProps<{
  error: string;
}>();

defineEmits<{
  retry: [];
}>();
</script>
