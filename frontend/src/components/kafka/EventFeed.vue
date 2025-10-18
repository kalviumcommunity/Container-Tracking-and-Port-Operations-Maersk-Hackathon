<template>
  <div 
    ref="eventContainerRef"
    class="lg:col-span-2 bg-white rounded-xl border border-slate-200 shadow-sm"
    :style="isScrollableMode ? `height: ${initialHeight}; display: flex; flex-direction: column;` : ''"
  >
    <div class="border-b border-slate-200 p-6">
      <div class="flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="p-2 bg-blue-50 rounded-lg">
            <Radio :size="20" class="text-blue-600" />
          </div>
          <div>
            <h3 class="text-xl font-semibold text-slate-900">Live Event Stream</h3>
            <p class="text-sm text-slate-600">Real-time events • {{ filteredEvents.length }} events loaded</p>
          </div>
        </div>
        <div class="flex items-center gap-2">
          <button 
            @click="toggleEventView"
            class="px-3 py-2 text-sm font-medium text-slate-600 bg-slate-50 rounded-lg hover:bg-slate-100 transition-colors flex items-center gap-2"
          >
            <component :is="viewMode === 'list' ? Grid : List" :size="16" />
            {{ viewMode === 'list' ? 'Grid' : 'List' }}
          </button>
          <button 
            @click="$emit('create-event')"
            class="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition-colors flex items-center gap-2"
          >
            <Plus :size="16" />
            Create Event
          </button>
          <button class="px-4 py-2 text-sm font-medium text-blue-600 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors flex items-center gap-2">
            <Eye :size="16" />
            View All
          </button>
        </div>
      </div>
      
      <!-- Quick Filters -->
      <EventFilters
        :quick-filters="quickFilters"
        :auto-refresh="autoRefresh"
        @toggle-filter="toggleQuickFilter"
        @toggle-auto-refresh="toggleAutoRefresh"
      />
    </div>
    
    <div 
      class="p-6"
      :style="isScrollableMode ? 'flex: 1 1 auto; overflow-y: auto;' : ''"
    >
      <!-- Event List View -->
      <div v-if="viewMode === 'list'" class="space-y-4">
        <EventListItem
          v-for="event in paginatedEvents"
          :key="event.id"
          :event="event"
          @view-event="$emit('view-event', $event)"
          @delete-event="$emit('delete-event', $event)"
        />
      </div>

      <!-- Event Grid View -->
      <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <EventGridItem
          v-for="event in paginatedEvents"
          :key="event.id"
          :event="event"
          @view-event="$emit('view-event', $event)"
          @delete-event="$emit('delete-event', $event)"
        />
      </div>

      <!-- Pagination -->
      <div v-if="totalPages > 1" class="flex items-center justify-between mt-6 pt-6 border-t border-slate-200">
        <div class="text-sm text-slate-600">
          Showing {{ ((currentPage - 1) * pageSize) + 1 }} to {{ Math.min(currentPage * pageSize, filteredEvents.length) }} of {{ filteredEvents.length }} events
        </div>
        <div class="flex items-center gap-2">
          <button 
            @click="previousPage"
            :disabled="currentPage === 1"
            class="px-3 py-2 text-sm font-medium text-slate-600 bg-white border border-slate-300 rounded-lg hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Previous
          </button>
          <div class="flex items-center gap-1">
            <button
              v-for="page in visiblePages"
              :key="page"
              @click="goToPage(page)"
              class="px-3 py-2 text-sm font-medium rounded-lg"
              :class="page === currentPage 
                ? 'bg-blue-600 text-white' 
                : 'text-slate-600 hover:bg-slate-100'"
            >
              {{ page }}
            </button>
          </div>
          <button 
            @click="nextPage"
            :disabled="currentPage === totalPages"
            class="px-3 py-2 text-sm font-medium text-slate-600 bg-white border border-slate-300 rounded-lg hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Next
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { Radio, Grid, List, Plus, Eye } from 'lucide-vue-next'
import EventFilters from './EventFilters.vue'
import EventListItem from './EventListItem.vue'
import EventGridItem from './EventGridItem.vue'
import type { EventDto, QuickFilter } from './types'

interface Props {
  events: EventDto[]
  viewMode: 'list' | 'grid'
  autoRefresh: boolean
  quickFilters: QuickFilter[]
  currentPage: number
  pageSize: number
  isScrollableMode: boolean
  initialHeight: string
}

interface Emits {
  (e: 'toggle-view'): void
  (e: 'create-event'): void
  (e: 'view-event', event: EventDto): void
  (e: 'delete-event', event: EventDto): void
  (e: 'toggle-filter', filterId: string): void
  (e: 'toggle-auto-refresh'): void
  (e: 'page-change', page: number): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const eventContainerRef = ref<HTMLElement>()

const filteredEvents = computed(() => {
  let filtered = props.events
  
  // Apply quick filters
  const activeFilters = props.quickFilters.filter(f => f.active)
  if (activeFilters.length > 0) {
    filtered = filtered.filter(event => {
      return activeFilters.some(filter => {
        switch (filter.id) {
          case 'unread':
            return !event.isRead
          case 'critical':
            return event.severity === 'Critical'
          case 'today':
            return new Date(event.eventTime).toDateString() === new Date().toDateString()
          default:
            return true
        }
      })
    })
  }
  
  return filtered
})

const totalPages = computed(() => Math.ceil(filteredEvents.value.length / props.pageSize))

const paginatedEvents = computed(() => {
  const start = (props.currentPage - 1) * props.pageSize
  const end = start + props.pageSize
  return filteredEvents.value.slice(start, end)
})

const visiblePages = computed(() => {
  const pages = []
  const total = totalPages.value
  const current = props.currentPage
  
  // Show up to 5 pages
  let start = Math.max(1, current - 2)
  let end = Math.min(total, start + 4)
  
  // Adjust start if we're near the end
  if (end - start < 4) {
    start = Math.max(1, end - 4)
  }
  
  for (let i = start; i <= end; i++) {
    pages.push(i)
  }
  
  return pages
})

const toggleEventView = () => {
  emit('toggle-view')
}

const toggleQuickFilter = (filterId: string) => {
  emit('toggle-filter', filterId)
}

const toggleAutoRefresh = () => {
  emit('toggle-auto-refresh')
}

const previousPage = () => {
  if (props.currentPage > 1) {
    emit('page-change', props.currentPage - 1)
  }
}

const nextPage = () => {
  if (props.currentPage < totalPages.value) {
    emit('page-change', props.currentPage + 1)
  }
}

const goToPage = (page: number) => {
  emit('page-change', page)
}
</script>
