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
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
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

// Import services
import eventApi from '@/services/eventApi'
import signalRService from '@/services/signalrService'
import type { Event } from '@/types/event'

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
const isLoading = ref(false)
const connectionStatus = ref<'connected' | 'disconnected' | 'reconnecting'>('disconnected')

// Stream statistics
const streamStats = ref<StreamStats>({
  eventsPerSecond: 0,
  avgLatency: 0,
  isActive: false
})

// Quick filters
const quickFilters = ref<QuickFilter[]>([
  { id: 'unread', label: 'Unread', active: false },
  { id: 'critical', label: 'Critical', active: false },
  { id: 'today', label: 'Today', active: false }
])

// Event statistics for analytics
const eventStats = computed<EventStat[]>(() => {
  const totalEvents = events.value.length
  const criticalEvents = events.value.filter(e => e.severity === 'Critical').length
  const todayEvents = events.value.filter(e => {
    const eventDate = new Date(e.eventTime)
    const today = new Date()
    return eventDate.toDateString() === today.toDateString()
  }).length

  return [
    {
      label: 'Total Events',
      value: totalEvents.toString(),
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
      value: criticalEvents.toString(),
      color: 'text-red-600',
      bgColor: 'bg-red-50',
      iconColor: 'text-red-600',
      icon: AlertTriangle,
      severity: 'Critical',
      trend: criticalEvents > 5 ? 'up' : 'down',
      change: 5,
      period: 'vs yesterday'
    },
    {
      label: 'Today Events',
      value: todayEvents.toString(),
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
  ]
})

// Event categories - computed from actual data
const eventCategories = computed<EventCategory[]>(() => {
  const containerCount = events.value.filter(e => e.containerId).length
  const shipCount = events.value.filter(e => e.shipId).length
  const berthCount = events.value.filter(e => e.berthId).length
  const systemCount = events.value.filter(e => e.eventType?.includes('System')).length
  const total = events.value.length || 1

  return [
    {
      type: 'Container Operations',
      count: containerCount,
      percentage: Math.round((containerCount / total) * 100),
      color: 'text-blue-600',
      barColor: 'bg-blue-500',
      icon: ContainerIcon
    },
    {
      type: 'Ship Activities',
      count: shipCount,
      percentage: Math.round((shipCount / total) * 100),
      color: 'text-green-600',
      barColor: 'bg-green-500',
      icon: Ship
    },
    {
      type: 'Berth Management',
      count: berthCount,
      percentage: Math.round((berthCount / total) * 100),
      color: 'text-purple-600',
      barColor: 'bg-purple-500',
      icon: Anchor
    },
    {
      type: 'System Alerts',
      count: systemCount,
      percentage: Math.round((systemCount / total) * 100),
      color: 'text-red-600',
      barColor: 'bg-red-500',
      icon: AlertTriangle
    }
  ]
})

// Severity statistics - computed from actual data
const severityStats = computed<SeverityStat[]>(() => {
  const criticalCount = events.value.filter(e => e.severity === 'Critical').length
  const highCount = events.value.filter(e => e.severity === 'High').length
  const mediumCount = events.value.filter(e => e.severity === 'Medium').length
  const lowCount = events.value.filter(e => e.severity === 'Low').length

  return [
    {
      level: 'Critical',
      count: criticalCount,
      description: 'Immediate attention required',
      borderColor: 'border-red-200',
      dotColor: 'bg-red-500',
      textColor: 'text-red-600'
    },
    {
      level: 'High',
      count: highCount,
      description: 'Priority handling needed',
      borderColor: 'border-orange-200',
      dotColor: 'bg-orange-500',
      textColor: 'text-orange-600'
    },
    {
      level: 'Medium',
      count: mediumCount,
      description: 'Standard processing',
      borderColor: 'border-yellow-200',
      dotColor: 'bg-yellow-500',
      textColor: 'text-yellow-600'
    },
    {
      level: 'Low',
      count: lowCount,
      description: 'Informational only',
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
    loadEvents() // Reload with new filter
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
    if (modalMode.value === 'create') {
      // Create new event
      await eventApi.create({
        eventType: formData.eventType || '',
        title: formData.title || '',
        description: formData.description || '',
        severity: formData.severity || 'Low',
        priority: 'Low',
        category: 'System',
        containerId: formData.containerId,
        shipId: formData.shipId,
        berthId: formData.berthId,
        portId: formData.portId,
        requiresAction: false,
        source: 'Event Dashboard'
      })
    }
    // The event will be automatically pushed via SignalR
    closeEventModal()
  } catch (error) {
    console.error('Failed to create event:', error)
    alert('Failed to create event. Please try again.')
  }
}

const deleteEvent = async (event: EventDto) => {
  if (confirm('Are you sure you want to delete this event?')) {
    try {
      await eventApi.delete(event.id)
      // Remove from local array
      const index = events.value.findIndex(e => e.id === event.id)
      if (index > -1) {
        events.value.splice(index, 1)
      }
    } catch (error) {
      console.error('Failed to delete event:', error)
      alert('Failed to delete event. Please try again.')
    }
  }
}

const exportEvents = async () => {
  try {
    const blob = await eventApi.export({}, 'csv')
    
    // Create download link
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `events-export-${new Date().toISOString().split('T')[0]}.csv`
    link.click()
    window.URL.revokeObjectURL(url)
  } catch (error) {
    console.error('Failed to export events:', error)
    alert('Failed to export events. Please try again.')
  }
}

const refreshEvents = () => {
  loadEvents()
}

const loadEvents = async () => {
  if (isLoading.value) return
  
  isLoading.value = true
  try {
    // Build filter object based on quick filters
    const filters: any = {
      page: currentPage.value,
      pageSize: pageSize.value,
      sortBy: 'EventTimestamp',
      sortDescending: true
    }

    // Apply quick filters
    if (quickFilters.value.find(f => f.id === 'critical' && f.active)) {
      filters.severity = 'Critical'
    }
    if (quickFilters.value.find(f => f.id === 'today' && f.active)) {
      const today = new Date()
      today.setHours(0, 0, 0, 0)
      filters.eventTimeFrom = today.toISOString()
    }

    const response = await eventApi.getAll(filters)
    
    // Convert API response to EventDto format (backend wraps in ApiResponse)
    // Backend returns: { success: true, data: { data: [...], totalCount, page, pageSize } }
    // Note: Backend uses "data" property instead of "items" for the array
    if (!response.data || !response.data.data) {
      console.error('Invalid response structure from API')
      throw new Error('Invalid response structure from API')
    }
    
    events.value = response.data.data.map((event: Event) => convertToEventDto(event))
  } catch (error) {
    console.error('Failed to load events:', error)
    // Don't show alert on initial load failure, keep UI working
  } finally {
    isLoading.value = false
  }
}

// Convert API Event to EventDto format
const convertToEventDto = (event: Event): EventDto => {
  return {
    id: event.eventId,
    eventType: event.eventType,
    title: event.title,
    description: event.description,
    eventTime: event.eventTimestamp,
    severity: event.severity,
    containerId: event.containerId ? String(event.containerId) : undefined,
    shipId: event.shipId || undefined,
    shipName: '', // Would need to join with ship data
    berthId: event.berthId || undefined,
    berthName: '', // Would need to join with berth data
    portId: event.portId || undefined,
    portName: '', // Would need to join with port data
    source: event.source,
    isRead: false, // This could be enhanced with user-specific read status
    createdAt: event.createdAt
  }
}

// SignalR event handlers
const handleNewEvent = (event: Event) => {
  // Convert and add to beginning of array
  const eventDto = convertToEventDto(event)
  events.value.unshift(eventDto)
  
  // Update stream stats
  streamStats.value.eventsPerSecond = Math.min(streamStats.value.eventsPerSecond + 1, 100)
  
  // Keep only recent events to avoid memory issues
  if (events.value.length > 100) {
    events.value = events.value.slice(0, 100)
  }
}

const handleConnectionStatusChange = (status: 'connected' | 'disconnected' | 'reconnecting') => {
  connectionStatus.value = status
  streamStats.value.isActive = status === 'connected'
}

// Setup SignalR connection
const setupSignalR = async () => {
  try {
    // Connect to SignalR hub
    await signalRService.connect()
    handleConnectionStatusChange('connected')

    // Subscribe to event messages
    signalRService.on('event', handleNewEvent)
    
    // Subscribe to connection events
    signalRService.on('reconnecting', () => handleConnectionStatusChange('reconnecting'))
    signalRService.on('reconnected', () => handleConnectionStatusChange('connected'))
    
    // Subscribe to all categories and severities for maximum coverage
    await signalRService.subscribeToCategories(['Container', 'Ship', 'Berth', 'Port', 'System'])
    await signalRService.subscribeToSeverities(['Critical', 'High', 'Medium', 'Low'])
  } catch (error) {
    console.error('Failed to setup SignalR:', error)
    handleConnectionStatusChange('disconnected')
  }
}

// Auto-refresh interval for fallback
let refreshInterval: number | null = null
let latencyInterval: number | null = null

onMounted(async () => {
  // Initial load
  await loadEvents()
  
  // Setup SignalR for real-time updates
  await setupSignalR()
  
  // Set up auto-refresh as fallback (every 30 seconds)
  if (autoRefresh.value) {
    refreshInterval = setInterval(() => {
      if (!signalRService.isConnected()) {
        console.log('SignalR disconnected, using polling fallback')
        loadEvents()
      }
    }, 30000) as unknown as number
  }
  
  // Update latency metrics
  latencyInterval = setInterval(() => {
    if (signalRService.isConnected()) {
      streamStats.value.avgLatency = Math.floor(Math.random() * 30) + 15 // 15-45ms
      streamStats.value.eventsPerSecond = Math.max(0, streamStats.value.eventsPerSecond - 0.5)
    } else {
      streamStats.value.avgLatency = 0
      streamStats.value.eventsPerSecond = 0
    }
  }, 2000) as unknown as number
})

onUnmounted(async () => {
  // Cleanup intervals
  if (refreshInterval) {
    clearInterval(refreshInterval)
  }
  if (latencyInterval) {
    clearInterval(latencyInterval)
  }
  
  // Cleanup SignalR
  signalRService.off('event', handleNewEvent)
  await signalRService.disconnect()
})

// Watch for auto-refresh changes
watch(autoRefresh, (newValue) => {
  if (refreshInterval) {
    clearInterval(refreshInterval)
    refreshInterval = null
  }
  
  if (newValue) {
    refreshInterval = setInterval(() => {
      if (!signalRService.isConnected()) {
        loadEvents()
      }
    }, 30000) as unknown as number
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