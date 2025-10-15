<template>
  <div class="flex items-center gap-3 mt-4 flex-wrap">
    <div class="flex items-center gap-2">
      <span class="text-sm font-medium text-slate-700">Quick Filter:</span>
      <button 
        v-for="filter in quickFilters" 
        :key="filter.id"
        @click="$emit('toggle-filter', filter.id)"
        class="px-3 py-1 text-xs font-medium rounded-full transition-colors"
        :class="filter.active 
          ? 'bg-blue-100 text-blue-700 border border-blue-200' 
          : 'bg-slate-100 text-slate-600 hover:bg-slate-200'"
      >
        {{ filter.label }}
      </button>
    </div>
    <div class="ml-auto flex items-center gap-2">
      <span class="text-xs text-slate-500">Auto-refresh:</span>
      <button 
        @click="$emit('toggle-auto-refresh')"
        class="flex items-center gap-1 px-2 py-1 text-xs rounded-full transition-colors"
        :class="autoRefresh 
          ? 'bg-green-100 text-green-700' 
          : 'bg-slate-100 text-slate-600'"
      >
        <div class="w-2 h-2 rounded-full" 
             :class="autoRefresh ? 'bg-green-500 animate-pulse' : 'bg-slate-400'"></div>
        {{ autoRefresh ? 'On' : 'Off' }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { QuickFilter } from './types'

interface Props {
  quickFilters: QuickFilter[]
  autoRefresh: boolean
}

interface Emits {
  (e: 'toggle-filter', filterId: string): void
  (e: 'toggle-auto-refresh'): void
}

defineProps<Props>()
defineEmits<Emits>()
</script>