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
import { eventApi } from '../../services/eventApi'
import type { EventCreate } from '../../types/event'

// Reactive data
const events = ref<EventDto[]>([])
const viewMode = ref<'list' | 'grid'>('list')
const autoRefresh = ref(true)
const currentPage = ref(1)
const pageSize = ref(50) // Increased from 10 to show more events
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

// Event categories - computed from real events
const eventCategories = computed<EventCategory[]>(() => {
  const categories: Record<string, { count: number; icon: any }> = {
    'Container Operations': { count: 0, icon: ContainerIcon },
    'Ship Activities': { count: 0, icon: Ship },
    'Berth Management': { count: 0, icon: Anchor },
    'System Alerts': { count: 0, icon: AlertTriangle }
  }
  
  // Count events by category based on eventType
  events.value.forEach(event => {
    const type = event.eventType?.toLowerCase() || ''
    if (type.includes('container')) {
      categories['Container Operations'].count++
    } else if (type.includes('ship')) {
      categories['Ship Activities'].count++
    } else if (type.includes('berth')) {
      categories['Berth Management'].count++
    } else {
      categories['System Alerts'].count++
    }
  })
  
  const total = events.value.length || 1
  
  return Object.entries(categories).map(([type, data], index) => ({
    type,
    count: data.count,
    percentage: Math.round((data.count / total) * 100),
    color: ['text-blue-600', 'text-green-600', 'text-purple-600', 'text-red-600'][index],
    barColor: ['bg-blue-500', 'bg-green-500', 'bg-purple-500', 'bg-red-500'][index],
    icon: data.icon
  }))
})

// Severity statistics - computed from real events
const severityStats = computed<SeverityStat[]>(() => {
  const severityCounts: Record<string, number> = {
    'Critical': 0,
    'High': 0,
    'Medium': 0,
    'Low': 0
  }
  
  events.value.forEach(event => {
    const severity = event.severity || 'Low'
    if (severityCounts[severity] !== undefined) {
      severityCounts[severity]++
    }
  })
  
  return [
    {
      level: 'Critical',
      count: severityCounts['Critical'],
      description: 'Immediate attention required',
      borderColor: 'border-red-200',
      dotColor: 'bg-red-500',
      textColor: 'text-red-600'
    },
    {
      level: 'High',
      count: severityCounts['High'],
      description: 'Priority handling needed',
      borderColor: 'border-orange-200',
      dotColor: 'bg-orange-500',
      textColor: 'text-orange-600'
    },
    {
      level: 'Medium',
      count: severityCounts['Medium'],
      description: 'Standard processing',
      borderColor: 'border-yellow-200',
      dotColor: 'bg-yellow-500',
      textColor: 'text-yellow-600'
    },
    {
      level: 'Low',
      count: severityCounts['Low'],
      description: 'Routine monitoring',
      borderColor: 'border-green-200',
      dotColor: 'bg-green-500',
      textColor: 'text-green-600'
    }
  ]
})

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

const handleEventSubmit = async (formData: EventFormData) => {
  try {
    // Map form data to backend EventCreate DTO
    const eventPayload: EventCreate = {
      eventType: formData.eventType,
      title: formData.title,
      description: formData.description || '',
      severity: formData.severity,
      source: formData.source || 'User',
      containerId: formData.containerId || undefined,
      shipId: formData.shipId || undefined,
      berthId: formData.berthId || undefined,
      portId: formData.portId || undefined,
      eventTime: new Date().toISOString()
    }

    // Call real backend API
    const createdEvent = await eventApi.create(eventPayload)
    
    // Refresh events list to show new event
    await loadEvents()
    
    closeEventModal()
  } catch (error) {
    console.error('Error creating event:', error)
    alert('Failed to create event. Please try again.')
  }
}

const deleteEvent = async (event: EventDto) => {
  if (confirm('Are you sure you want to delete this event?')) {
    try {
      // Call real backend API
      await eventApi.delete(event.id)
      
      // Remove from local list
      const index = events.value.findIndex(e => e.id === event.id)
      if (index > -1) {
        events.value.splice(index, 1)
      }
      
    } catch (error) {
      console.error('Error deleting event:', error)
      alert('Failed to delete event. Please try again.')
    }
  }
}

const exportEvents = () => {
  // TODO: Implement event export functionality
}

const refreshEvents = async () => {
  await loadEvents()
}

const loadEvents = async () => {
  try {
    // Call real backend API with filters
    const filter: any = {
      page: currentPage.value,
      pageSize: pageSize.value,
      sortBy: 'EventTimestamp',
      sortDescending: true
    }
    
    // Apply quick filters
    if (quickFilters.value.find(f => f.id === 'unread' && f.active)) {
      filter.isRead = false
    }
    if (quickFilters.value.find(f => f.id === 'critical' && f.active)) {
      filter.severity = 'Critical'
    }
    if (quickFilters.value.find(f => f.id === 'today' && f.active)) {
      const today = new Date().toISOString().split('T')[0]
      filter.startDate = today
    }
    
    const response = await eventApi.getAll(filter)
    
    // Map response to EventDto format expected by components
    events.value = (response.data.data || []).map((event: any) => ({
      id: event.id || event.eventId,
      eventType: event.eventType,
      title: event.title,
      description: event.description,
      eventTime: event.eventTimestamp || event.eventTime,
      severity: event.severity,
      containerId: event.containerId,
      shipId: event.shipId,
      shipName: event.shipName,
      berthId: event.berthId,
      berthName: event.berthName,
      portId: event.portId,
      portName: event.portName,
      userId: event.assignedToUserId,
      userName: event.assignedToUserName,
      source: event.source,
      isRead: event.isRead,
      createdAt: event.createdAt
    }))
  } catch (error) {
    console.error('Error loading events:', error)
    events.value = []
  }
}

// Auto-refresh interval
let refreshInterval: number | null = null

const startAutoRefresh = () => {
  if (refreshInterval) {
    clearInterval(refreshInterval)
  }
  if (autoRefresh.value) {
    refreshInterval = setInterval(async () => {
      await loadEvents()
      // Update stream stats based on loaded events
      streamStats.value.eventsPerSecond = events.value.length > 0 ? Math.min(events.value.length / 60, 50) : 0
    }, 5000) // Refresh every 5 seconds
  }
}

const stopAutoRefresh = () => {
  if (refreshInterval) {
    clearInterval(refreshInterval)
    refreshInterval = null
  }
}

onMounted(async () => {
  await loadEvents()
  startAutoRefresh()
})

onUnmounted(() => {
  stopAutoRefresh()
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
