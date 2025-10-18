<template>
  <section class="mb-8">
    <div class="mb-6">
      <h2 class="text-2xl font-bold text-slate-900 mb-2">Live Event Analytics</h2>
      <p class="text-slate-600">Real-time streaming analytics and operational insights</p>
    </div>
    
    <!-- Primary Stats Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
      <EventStats
        v-for="(stat, index) in eventStats"
        :key="index"
        :stat="stat"
        :animation-delay="index * 100"
      />
    </div>

    <!-- Event Category Breakdown -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
      <!-- Event Types Distribution -->
      <div class="bg-white rounded-xl border border-slate-200 p-6 shadow-sm">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-slate-900">Event Categories</h3>
          <button class="text-blue-600 hover:text-blue-700 text-sm font-medium">View Details</button>
        </div>
        <div class="space-y-3">
          <div v-for="category in eventCategories" :key="category.type" class="flex items-center justify-between">
            <div class="flex items-center gap-3">
              <component :is="category.icon" :size="20" :class="category.color" />
              <span class="font-medium text-slate-700">{{ category.type }}</span>
            </div>
            <div class="flex items-center gap-2">
              <div class="w-16 bg-slate-200 rounded-full h-2">
                <div 
                  class="h-2 rounded-full transition-all duration-500" 
                  :class="category.barColor"
                  :style="{ width: `${category.percentage}%` }"
                ></div>
              </div>
              <span class="text-sm font-medium text-slate-600 w-12">{{ category.count }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Severity Level Analysis -->
      <SeverityDistribution :severity-stats="severityStats" />
    </div>
  </section>
</template>

<script setup lang="ts">
import EventStats from './EventStats.vue'
import SeverityDistribution from './SeverityDistribution.vue'
import type { EventStat, EventCategory, SeverityStat } from './types'

interface Props {
  eventStats: EventStat[]
  eventCategories: EventCategory[]
  severityStats: SeverityStat[]
}

defineProps<Props>()
</script>
