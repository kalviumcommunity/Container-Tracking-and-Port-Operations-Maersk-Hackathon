<!-- Simple Loading Overlay Component -->
<template>
  <div class="fixed inset-0 bg-white bg-opacity-75 flex items-center justify-center z-50">
    <div class="text-center">
      <div class="inline-block animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600"></div>
      <p class="mt-4 text-lg font-medium text-gray-900">Loading Port Operations...</p>
      <p class="text-sm text-gray-600">Please wait while we fetch the latest data</p>
    </div>
  </div>
</template>

<script setup lang="ts">
// No props needed for simple loading overlay
</script>

<style scoped>
.animate-spin {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}
</style>
