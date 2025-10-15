<!-- Simple Analytics Dashboard Component -->
<template>
  <div class="space-y-6">
    <div class="bg-white rounded-lg border border-gray-200 p-6">
      <h3 class="text-lg font-semibold text-gray-900 mb-4">Analytics Dashboard</h3>
      <p class="text-gray-600">View comprehensive analytics and reports.</p>
      
      <div v-if="loading" class="mt-6 grid grid-cols-1 md:grid-cols-2 gap-4">
        <div v-for="i in 4" :key="i" class="animate-pulse h-32 bg-gray-200 rounded"></div>
      </div>
      
      <div v-else class="mt-6">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div class="bg-gray-50 rounded-lg p-4">
            <div class="text-sm text-gray-600">Total Berths</div>
            <div class="text-2xl font-bold text-gray-900">{{ berthStats.totalBerths }}</div>
          </div>
          <div class="bg-gray-50 rounded-lg p-4">
            <div class="text-sm text-gray-600">Active Berths</div>
            <div class="text-2xl font-bold text-gray-900">{{ berthStats.activeBerths }}</div>
          </div>
          <div class="bg-gray-50 rounded-lg p-4">
            <div class="text-sm text-gray-600">Available Berths</div>
            <div class="text-2xl font-bold text-gray-900">{{ berthStats.availableBerths }}</div>
          </div>
          <div class="bg-gray-50 rounded-lg p-4">
            <div class="text-sm text-gray-600">Total Capacity</div>
            <div class="text-2xl font-bold text-gray-900">{{ berthStats.totalCapacity }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface BerthStats {
  totalBerths: number;
  activeBerths: number;
  availableBerths: number;
  totalCapacity: number;
  currentOccupancy: number;
}

interface Props {
  berthStats: BerthStats;
  berths: any[];
  assignments: any[];
  loading: boolean;
}

withDefaults(defineProps<Props>(), {
  berths: () => [],
  assignments: () => [],
  loading: false
});
</script>