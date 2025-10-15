<template>
  <div
    class="bg-white rounded-xl border border-slate-200 p-6 hover:shadow-lg transition-all duration-300 hover:-translate-y-1 animate-slideIn"
    :style="{ animationDelay: `${animationDelay}ms` }"
  >
    <div class="flex items-start justify-between mb-4">
      <div class="p-3 rounded-lg" :class="stat.bgColor">
        <component :is="stat.icon" :size="24" :class="stat.iconColor" />
      </div>
      <div class="text-right">
        <span class="text-xs font-medium px-2 py-1 rounded-full" :class="getSeverityColor(stat.severity)">
          {{ stat.severity }}
        </span>
      </div>
    </div>
    <div class="mb-3">
      <p class="text-3xl font-bold text-slate-900">{{ stat.value }}</p>
      <p class="text-sm font-medium text-slate-600">{{ stat.label }}</p>
      <div class="flex items-center gap-1 mt-2">
        <TrendingUp :size="14" :class="stat.trend === 'up' ? 'text-green-600' : 'text-red-600'" />
        <span class="text-sm font-medium" :class="stat.trend === 'up' ? 'text-green-600' : 'text-red-600'">
          {{ stat.trend === 'up' ? '+' : '-' }}{{ stat.change }}%
        </span>
        <span class="text-sm text-slate-500">{{ stat.period }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { TrendingUp } from 'lucide-vue-next'
import type { EventStat } from './types'

interface Props {
  stat: EventStat
  animationDelay: number
}

defineProps<Props>()

const getSeverityColor = (severity: string) => {
  switch (severity?.toLowerCase()) {
    case 'critical':
      return 'bg-red-100 text-red-700'
    case 'high':
      return 'bg-orange-100 text-orange-700'
    case 'medium':
      return 'bg-yellow-100 text-yellow-700'
    case 'low':
      return 'bg-green-100 text-green-700'
    default:
      return 'bg-slate-100 text-slate-700'
  }
}
</script>

<style scoped>
@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-slideIn {
  animation: slideIn 0.6s ease-out forwards;
}
</style>