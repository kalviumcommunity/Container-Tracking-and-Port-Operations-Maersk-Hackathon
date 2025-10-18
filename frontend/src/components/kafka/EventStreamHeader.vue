<template>
  <div class="mb-8">
    <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
      <div class="flex items-center gap-4">
        <div class="p-3 bg-blue-600 rounded-xl shadow-lg">
          <Activity :size="28" class="text-white" />
        </div>
        <div>
          <h1 class="text-3xl font-bold text-slate-900">Advanced Event Streaming Dashboard</h1>
          <p class="text-slate-600 mt-1">Real-time port operations monitoring • {{ currentTime }}</p>
        </div>
      </div>
      <div class="flex items-center gap-4">
        <div class="flex items-center gap-2 bg-green-50 px-4 py-2 rounded-lg border border-green-200">
          <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
          <span class="text-green-700 font-medium">Stream Active</span>
        </div>
        <div class="flex items-center gap-2 bg-blue-50 px-4 py-2 rounded-lg border border-blue-200">
          <Zap :size="16" class="text-blue-600" />
          <span class="font-medium text-blue-700">{{ streamStats.eventsPerSecond }}/sec</span>
        </div>
        <div class="flex items-center gap-2 bg-purple-50 px-4 py-2 rounded-lg border border-purple-200">
          <Clock :size="16" class="text-purple-600" />
          <span class="font-medium text-purple-700">{{ streamStats.avgLatency }}ms</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { Activity, Zap, Clock } from 'lucide-vue-next'

interface StreamStats {
  eventsPerSecond: number
  avgLatency: number
  isActive: boolean
}

interface Props {
  streamStats: StreamStats
}

const props = defineProps<Props>()

const currentTime = computed(() => {
  return new Date().toLocaleTimeString()
})
</script>
