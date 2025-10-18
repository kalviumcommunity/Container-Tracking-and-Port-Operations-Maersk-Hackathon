<!-- Simplified Page Header Component -->
<template>
  <header class="bg-white border-b border-gray-200 shadow-sm">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex items-center justify-between h-16">
        <!-- Title Section -->
        <div class="flex items-center space-x-4">
          <div class="flex items-center justify-center w-10 h-10 bg-blue-600 rounded-lg">
            <Anchor :size="24" class="text-white" />
          </div>
          <div>
            <h1 class="text-xl font-bold text-gray-900">Port Operations</h1>
            <p class="text-sm text-gray-600">Manage berths and operations</p>
          </div>
        </div>

        <!-- Status and Actions -->
        <div class="flex items-center space-x-4">
          <!-- Live Status Indicator -->
          <div class="flex items-center space-x-2 px-3 py-1 bg-green-50 rounded-lg border border-green-200">
            <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
            <span class="text-sm font-medium text-green-700">
              {{ availableBerths }} / {{ totalBerths }} Available
            </span>
          </div>

          <!-- Refresh Button -->
          <button
            @click="$emit('refresh')"
            :disabled="loading"
            class="p-2 text-gray-600 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
            title="Refresh Data"
          >
            <RefreshCw 
              :size="18" 
              :class="{ 'animate-spin': loading }"
            />
          </button>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { Anchor, RefreshCw } from 'lucide-vue-next';

interface Props {
  totalBerths: number;
  availableBerths: number;
  loading: boolean;
}

withDefaults(defineProps<Props>(), {
  totalBerths: 0,
  availableBerths: 0,
  loading: false
});

defineEmits<{
  refresh: [];
}>();
</script>

<style scoped>
.animate-pulse {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

.animate-spin {
  animation: spin 1s linear infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}
</style>
