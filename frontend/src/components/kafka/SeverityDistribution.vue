<template>
  <div class="bg-white rounded-xl border border-slate-200 p-6 shadow-sm">
    <div class="flex items-center justify-between mb-4">
      <h3 class="text-lg font-semibold text-slate-900">Severity Distribution</h3>
      <button class="text-blue-600 hover:text-blue-700 text-sm font-medium">Configure Alerts</button>
    </div>
    <div class="grid grid-cols-2 gap-4">
      <div 
        v-for="severity in severityStats" 
        :key="severity.level" 
        class="p-4 rounded-lg border-2 transition-all hover:shadow-md cursor-pointer" 
        :class="severity.borderColor"
        @click="$emit('severity-click', severity.level)"
      >
        <div class="flex items-center gap-2 mb-2">
          <div class="w-3 h-3 rounded-full" :class="severity.dotColor"></div>
          <span class="font-medium text-slate-700">{{ severity.level }}</span>
        </div>
        <div class="text-2xl font-bold" :class="severity.textColor">{{ severity.count }}</div>
        <div class="text-xs text-slate-500">{{ severity.description }}</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { SeverityStat } from './types'

interface Props {
  severityStats: SeverityStat[]
}

interface Emits {
  (e: 'severity-click', level: string): void
}

defineProps<Props>()
defineEmits<Emits>()
</script>
