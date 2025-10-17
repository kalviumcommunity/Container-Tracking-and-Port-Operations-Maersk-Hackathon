<template>
  <div class="min-h-screen bg-slate-50">
    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-6 py-8">
      <!-- Event Stream Header -->
      <EventStreamHeader :stream-stats="streamStats" />

      <!-- Event Analytics -->
      <EventAnalytics 
        :event-stats="eventStats"
        :event-categories="eventCategories"
        :severity-stats="severityStats"
      />

      <!-- Main Dashboard Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Enhanced Live Event Feed -->
        <EventFeed
          :events="events"
          :view-mode="viewMode"
          :auto-refresh="autoRefresh"
          :quick-filters="quickFilters"
          :current-page="currentPage"
          :page-size="pageSize"
          :is-scrollable-mode="isScrollableMode"
          :initial-height="initialHeight"
          @toggle-view="toggleEventView"
          @create-event="openEventModal(null)"
          @view-event="openEventModal($event)"
          @delete-event="deleteEvent"
          @toggle-filter="toggleQuickFilter"
          @toggle-auto-refresh="toggleAutoRefresh"
          @page-change="currentPage = $event"
        />

        <!-- Event Statistics Sidebar -->
        <div class="space-y-6">
          <!-- Live Stream Metrics -->
          <div class="bg-white rounded-xl border border-slate-200 p-6 shadow-sm">
            <div class="flex items-center gap-3 mb-4">
              <div class="p-2 bg-green-50 rounded-lg">
                <Activity :size="20" class="text-green-600" />
              </div>
              <h3 class="text-lg font-semibold text-slate-900">Stream Metrics</h3>
            </div>
            <div class="space-y-4">
              <div class="flex justify-between items-center">
                <span class="text-sm text-slate-600">Events/sec</span>
                <span class="font-semibold text-green-600">{{ streamStats.eventsPerSecond }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-sm text-slate-600">Avg Latency</span>
                <span class="font-semibold text-blue-600">{{ streamStats.avgLatency }}ms</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-sm text-slate-600">Total Events</span>
                <span class="font-semibold text-slate-900">{{ events.length }}</span>
              </div>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="bg-white rounded-xl border border-slate-200 p-6 shadow-sm">
            <h3 class="text-lg font-semibold text-slate-900 mb-4">Quick Actions</h3>
            <div class="space-y-3">
              <button 
                @click="openEventModal(null)"
                class="w-full flex items-center gap-3 p-3 text-left bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors"
              >
                <Plus :size="20" class="text-blue-600" />
                <span class="font-medium text-blue-900">Create Event</span>
              </button>
              <button 
                @click="exportEvents"
                class="w-full flex items-center gap-3 p-3 text-left bg-green-50 rounded-lg hover:bg-green-100 transition-colors"
              >
                <Download :size="20" class="text-green-600" />
                <span class="font-medium text-green-900">Export Events</span>
              </button>
              <button 
                @click="refreshEvents"
                class="w-full flex items-center gap-3 p-3 text-left bg-purple-50 rounded-lg hover:bg-purple-100 transition-colors"
              >
                <RefreshCw :size="20" class="text-purple-600" />
                <span class="font-medium text-purple-900">Refresh Data</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Event Modal -->
      <EventModal
        :is-open="showEventModal"
        :event="selectedEvent"
        :mode="modalMode"
        @close="closeEventModal"
        @submit="handleEventSubmit"
      />
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { 
  Activity,
  Plus,
  Download,
  RefreshCw,
  Container as ContainerIcon,
  Ship,
  Anchor,
  AlertTriangle,
  CheckCircle,
  User,
  Settings,
  Bell,
  TrendingUp,
  Package,
  MapPin,
  Truck,
  Database,
  Users,
  Shield,
  Wrench
} from 'lucide-vue-next'

// Import decomposed components
import EventStreamHeader from '../kafka/EventStreamHeader.vue'
import EventAnalytics from '../kafka/EventAnalytics.vue'
import EventFeed from '../kafka/EventFeed.vue'
import EventModal from '../kafka/EventModal.vue'

// Import types
import type { 
  EventDto, 
  EventFormData, 
  StreamStats, 
  EventStat, 
  EventCategory, 
  SeverityStat, 
  QuickFilter 
} from '../kafka/types'

// Reactive data
const events = ref<EventDto[]>([])
const viewMode = ref<'list' | 'grid'>('list')
const autoRefresh = ref(true)
const currentPage = ref(1)
const pageSize = ref(10)
const isScrollableMode = ref(true)
const initialHeight = ref('600px')
const showEventModal = ref(false)
const selectedEvent = ref<EventDto | null>(null)
const modalMode = ref<'create' | 'edit' | 'view'>('create')

// Stream statistics
const streamStats = ref<StreamStats>({
  eventsPerSecond: 12,
  avgLatency: 45,
  isActive: true
})

// Quick filters
const quickFilters = ref<QuickFilter[]>([
  { id: 'unread', label: 'Unread', active: false },
  { id: 'critical', label: 'Critical', active: false },
  { id: 'today', label: 'Today', active: false }
])

// Event statistics for analytics
const eventStats = computed<EventStat[]>(() => [
  {
    label: 'Total Events',
    value: events.value.length.toString(),
    color: 'text-blue-600',
    bgColor: 'bg-blue-50',
    iconColor: 'text-blue-600',
    icon: Bell,
    severity: 'Low',
    trend: 'up',
    change: 12,
    period: 'vs yesterday'
  },
  {
    label: 'Critical Events',
    value: events.value.filter(e => e.severity === 'Critical').length.toString(),
    color: 'text-red-600',
    bgColor: 'bg-red-50',
    iconColor: 'text-red-600',
    icon: AlertTriangle,
    severity: 'Critical',
    trend: 'down',
    change: 5,
    period: 'vs yesterday'
  },
  {
    label: 'Active Streams',
    value: '4',
    color: 'text-green-600',
    bgColor: 'bg-green-50',
    iconColor: 'text-green-600',
    icon: Activity,
    severity: 'Low',
    trend: 'up',
    change: 8,
    period: 'vs yesterday'
  },
  {
    label: 'Avg Response Time',
    value: `${streamStats.value.avgLatency}ms`,
    color: 'text-purple-600',
    bgColor: 'bg-purple-50',
    iconColor: 'text-purple-600',
    icon: TrendingUp,
    severity: 'Medium',
    trend: 'down',
    change: 3,
    period: 'vs yesterday'
  }
])

// Event categories
const eventCategories = ref<EventCategory[]>([
  {
    type: 'Container Operations',
    count: 45,
    percentage: 35,
    color: 'text-blue-600',
    barColor: 'bg-blue-500',
    icon: ContainerIcon
  },
  {
    type: 'Ship Activities',
    count: 32,
    percentage: 25,
    color: 'text-green-600',
    barColor: 'bg-green-500',
    icon: Ship
  },
  {
    type: 'Berth Management',
    count: 28,
    percentage: 22,
    color: 'text-purple-600',
    barColor: 'bg-purple-500',
    icon: Anchor
  },
  {
    type: 'System Alerts',
    count: 23,
    percentage: 18,
    color: 'text-red-600',
    barColor: 'bg-red-500',
    icon: AlertTriangle
  }
])

// Severity statistics
const severityStats = ref<SeverityStat[]>([
  {
    level: 'Critical',
    count: 5,
    description: 'Immediate attention required',
    borderColor: 'border-red-200',
    dotColor: 'bg-red-500',
    textColor: 'text-red-600'
  },
  {
    level: 'High',
    count: 12,
    description: 'Priority handling needed',
    borderColor: 'border-orange-200',
    dotColor: 'bg-orange-500',
    textColor: 'text-orange-600'
  },
  {
    level: 'Medium',
    count: 23,
    description: 'Standard processing',
    borderColor: 'border-yellow-200',
    dotColor: 'bg-yellow-500',
    textColor: 'text-yellow-600'
  },
  {
    level: 'Low',
    count: 45,
    description: 'Informational only',
    borderColor: 'border-green-200',
    dotColor: 'bg-green-500',
    textColor: 'text-green-600'
  }
])

// Methods
const toggleEventView = () => {
  viewMode.value = viewMode.value === 'list' ? 'grid' : 'list'
}

const toggleQuickFilter = (filterId: string) => {
  const filter = quickFilters.value.find(f => f.id === filterId)
  if (filter) {
    filter.active = !filter.active
  }
}

const toggleAutoRefresh = () => {
  autoRefresh.value = !autoRefresh.value
}

const openEventModal = (event: EventDto | null) => {
  selectedEvent.value = event
  modalMode.value = event ? 'view' : 'create'
  showEventModal.value = true
}

const closeEventModal = () => {
  showEventModal.value = false
  selectedEvent.value = null
}

const handleEventSubmit = (formData: EventFormData) => {
  // Handle event creation/update
  console.log('Event submitted:', formData)
  closeEventModal()
  // TODO: Implement actual event creation/update logic
}

const deleteEvent = (event: EventDto) => {
  if (confirm('Are you sure you want to delete this event?')) {
    const index = events.value.findIndex(e => e.id === event.id)
    if (index > -1) {
      events.value.splice(index, 1)
    }
  }
}

const exportEvents = () => {
  // TODO: Implement event export functionality
  console.log('Exporting events...')
}

const refreshEvents = () => {
  // TODO: Implement event refresh functionality
  console.log('Refreshing events...')
  loadEvents()
}

const loadEvents = async () => {
  try {
    // TODO: Replace with actual API call to fetch events from backend
    // const response = await eventApi.getAll()
    // events.value = response.data || []
    
    // For now, show empty state until API integration is complete
    events.value = []
  } catch (error) {
    console.error('Error loading events:', error)
    events.value = []
  }
}

// Auto-refresh interval
let refreshInterval: number | null = null

onMounted(() => {
  loadEvents()
  
  // Set up auto-refresh if enabled
  if (autoRefresh.value) {
    refreshInterval = setInterval(() => {
      // Only update if we have real data, otherwise keep zeros
      if (events.value.length > 0) {
        streamStats.value.eventsPerSecond = Math.min(events.value.length, 25)
        streamStats.value.avgLatency = 30 // Fixed reasonable latency
      } else {
        streamStats.value.eventsPerSecond = 0
        streamStats.value.avgLatency = 0
      }
    }, 2000)
  }
})

onUnmounted(() => {
  if (refreshInterval) {
    clearInterval(refreshInterval)
  }
})
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

/* Custom scrollbar */
::-webkit-scrollbar {
  width: 6px;
}

::-webkit-scrollbar-track {
  background: #f1f5f9;
}

::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 3px;
}

::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}
</style>