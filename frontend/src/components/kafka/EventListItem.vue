<template>
  <div class="group bg-white rounded-lg border border-slate-200 p-4 hover:shadow-md transition-all duration-200 hover:border-blue-300">
    <div class="flex items-start gap-4">
      <!-- Event Icon and Status -->
      <div class="flex-shrink-0">
        <div class="p-2 rounded-lg" :class="getEventTypeColors(event.eventType).bgColor">
          <component :is="getEventIcon(event.eventType)" :size="20" :class="getEventTypeColors(event.eventType).iconColor" />
        </div>
      </div>

      <!-- Event Content -->
      <div class="flex-1 min-w-0">
        <div class="flex items-start justify-between mb-2">
          <div class="flex-1">
            <h4 class="text-sm font-semibold text-slate-900 group-hover:text-blue-900 transition-colors">
              {{ event.title }}
            </h4>
            <p class="text-xs text-slate-600 mt-1">{{ event.description }}</p>
          </div>
          <div class="flex items-center gap-2 ml-4">
            <span class="text-xs font-medium px-2 py-1 rounded-full" :class="getSeverityColor(event.severity)">
              {{ event.severity }}
            </span>
            <div v-if="!event.isRead" class="w-2 h-2 bg-blue-500 rounded-full"></div>
          </div>
        </div>

        <!-- Event Metadata -->
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-4 text-xs text-slate-500">
            <span class="flex items-center gap-1">
              <Clock :size="12" />
              {{ formatTime(event.eventTime) }}
            </span>
            <span v-if="event.source" class="flex items-center gap-1">
              <Database :size="12" />
              {{ event.source }}
            </span>
            <span v-if="event.shipName" class="flex items-center gap-1">
              <Ship :size="12" />
              {{ event.shipName }}
            </span>
            <span v-if="event.containerId" class="flex items-center gap-1">
              <Package :size="12" />
              {{ event.containerId }}
            </span>
          </div>

          <!-- Action Buttons -->
          <div class="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
            <button 
              @click="$emit('view-event', event)"
              class="p-1 text-slate-400 hover:text-blue-600 transition-colors"
              title="View Details"
            >
              <Eye :size="14" />
            </button>
            <button 
              @click="$emit('delete-event', event)"
              class="p-1 text-slate-400 hover:text-red-600 transition-colors"
              title="Delete Event"
            >
              <Trash2 :size="14" />
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { 
  Clock, 
  Database, 
  Ship, 
  Package, 
  Eye, 
  Trash2,
  Container as ContainerIcon,
  Anchor,
  AlertTriangle,
  CheckCircle,
  User,
  Settings,
  Bell
} from 'lucide-vue-next'
import type { EventDto } from './types'

interface Props {
  event: EventDto
}

interface Emits {
  (e: 'view-event', event: EventDto): void
  (e: 'delete-event', event: EventDto): void
}

defineProps<Props>()
defineEmits<Emits>()

const getEventIcon = (eventType: string) => {
  switch (eventType) {
    case 'Container Arrival':
    case 'Container Loading':
    case 'Container Unloading':
      return ContainerIcon
    case 'Ship Arrival':
    case 'Ship Departure':
      return Ship
    case 'Berth Assignment':
    case 'Berth Release':
      return Anchor
    case 'System Alert':
    case 'Security Alert':
      return AlertTriangle
    case 'Operation Completed':
      return CheckCircle
    case 'User Action':
      return User
    case 'System Maintenance':
      return Settings
    default:
      return Bell
  }
}

const getEventTypeColors = (eventType: string) => {
  switch (eventType) {
    case 'Container Arrival':
    case 'Container Loading':
    case 'Container Unloading':
      return { bgColor: 'bg-blue-50', iconColor: 'text-blue-600' }
    case 'Ship Arrival':
    case 'Ship Departure':
      return { bgColor: 'bg-green-50', iconColor: 'text-green-600' }
    case 'Berth Assignment':
    case 'Berth Release':
      return { bgColor: 'bg-purple-50', iconColor: 'text-purple-600' }
    case 'System Alert':
    case 'Security Alert':
      return { bgColor: 'bg-red-50', iconColor: 'text-red-600' }
    case 'Operation Completed':
      return { bgColor: 'bg-emerald-50', iconColor: 'text-emerald-600' }
    case 'User Action':
      return { bgColor: 'bg-orange-50', iconColor: 'text-orange-600' }
    case 'System Maintenance':
      return { bgColor: 'bg-gray-50', iconColor: 'text-gray-600' }
    default:
      return { bgColor: 'bg-slate-50', iconColor: 'text-slate-600' }
  }
}

const getSeverityColor = (severity: string) => {
  switch (severity?.toLowerCase()) {
    case 'critical':
      return 'bg-red-100 text-red-700 border border-red-200'
    case 'high':
      return 'bg-orange-100 text-orange-700 border border-orange-200'
    case 'medium':
      return 'bg-yellow-100 text-yellow-700 border border-yellow-200'
    case 'low':
      return 'bg-green-100 text-green-700 border border-green-200'
    default:
      return 'bg-slate-100 text-slate-700 border border-slate-200'
  }
}

const formatTime = (timestamp: string) => {
  const date = new Date(timestamp)
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  
  if (diff < 60000) { // Less than 1 minute
    return 'Just now'
  } else if (diff < 3600000) { // Less than 1 hour
    return `${Math.floor(diff / 60000)}m ago`
  } else if (diff < 86400000) { // Less than 1 day
    return `${Math.floor(diff / 3600000)}h ago`
  } else {
    return date.toLocaleDateString()
  }
}
</script>
