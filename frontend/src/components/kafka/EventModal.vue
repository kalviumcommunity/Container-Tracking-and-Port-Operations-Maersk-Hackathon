<template>
  <div v-if="isOpen" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
    <div class="bg-white rounded-xl shadow-xl max-w-2xl w-full max-h-[90vh] overflow-hidden">
      <!-- Modal Header -->
      <div class="flex items-center justify-between p-6 border-b border-slate-200">
        <div class="flex items-center gap-3">
          <div class="p-2 bg-blue-50 rounded-lg">
            <component :is="getEventIcon(formData.eventType)" :size="20" class="text-blue-600" />
          </div>
          <div>
            <h3 class="text-lg font-semibold text-slate-900">
              {{ mode === 'create' ? 'Create New Event' : mode === 'edit' ? 'Edit Event' : 'Event Details' }}
            </h3>
            <p class="text-sm text-slate-600">
              {{ mode === 'view' ? 'View event information' : 'Fill in the event details below' }}
            </p>
          </div>
        </div>
        <button 
          @click="$emit('close')"
          class="p-2 text-slate-400 hover:text-slate-600 transition-colors"
        >
          <X :size="20" />
        </button>
      </div>

      <!-- Modal Content -->
      <div class="p-6 overflow-y-auto max-h-[calc(90vh-140px)]">
        <form @submit.prevent="handleSubmit" class="space-y-6">
          <!-- Event Type -->
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-2">Event Type</label>
            <select 
              v-model="formData.eventType"
              :disabled="mode === 'view'"
              class="w-full p-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-slate-50"
              required
            >
              <option value="">Select event type</option>
              <option v-for="type in eventTypes" :key="type" :value="type">{{ type }}</option>
            </select>
          </div>

          <!-- Title -->
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-2">Title</label>
            <input 
              v-model="formData.title"
              :disabled="mode === 'view'"
              type="text" 
              class="w-full p-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-slate-50"
              placeholder="Enter event title"
              required
            />
          </div>

          <!-- Description -->
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-2">Description</label>
            <textarea 
              v-model="formData.description"
              :disabled="mode === 'view'"
              class="w-full p-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-slate-50 resize-none"
              rows="3"
              placeholder="Enter event description"
              required
            ></textarea>
          </div>

          <!-- Severity -->
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-2">Severity</label>
            <select 
              v-model="formData.severity"
              :disabled="mode === 'view'"
              class="w-full p-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-slate-50"
              required
            >
              <option value="">Select severity</option>
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
              <option value="Critical">Critical</option>
            </select>
          </div>

          <!-- Additional Fields -->
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Container ID</label>
              <input 
                v-model="formData.containerId"
                :disabled="mode === 'view'"
                type="text" 
                class="w-full p-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-slate-50"
                placeholder="Container ID (optional)"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Ship ID</label>
              <input 
                v-model="formData.shipId"
                :disabled="mode === 'view'"
                type="number" 
                class="w-full p-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-slate-50"
                placeholder="Ship ID (optional)"
              />
            </div>
          </div>

          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Berth ID</label>
              <input 
                v-model="formData.berthId"
                :disabled="mode === 'view'"
                type="number" 
                class="w-full p-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-slate-50"
                placeholder="Berth ID (optional)"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Port ID</label>
              <input 
                v-model="formData.portId"
                :disabled="mode === 'view'"
                type="number" 
                class="w-full p-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-slate-50"
                placeholder="Port ID (optional)"
              />
            </div>
          </div>

          <!-- Source -->
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-2">Source</label>
            <input 
              v-model="formData.source"
              :disabled="mode === 'view'"
              type="text" 
              class="w-full p-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 disabled:bg-slate-50"
              placeholder="Event source"
              required
            />
          </div>
        </form>
      </div>

      <!-- Modal Footer -->
      <div v-if="mode !== 'view'" class="flex items-center justify-end gap-3 p-6 border-t border-slate-200">
        <button 
          @click="$emit('close')"
          type="button"
          class="px-4 py-2 text-sm font-medium text-slate-600 bg-slate-100 rounded-lg hover:bg-slate-200 transition-colors"
        >
          Cancel
        </button>
        <button 
          @click="handleSubmit"
          type="submit"
          class="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition-colors"
        >
          {{ mode === 'create' ? 'Create Event' : 'Update Event' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { X, Bell, Container as ContainerIcon, Ship, Anchor, AlertTriangle, CheckCircle, User, Settings } from 'lucide-vue-next'
import type { EventDto, EventFormData } from './types'

interface Props {
  isOpen: boolean
  event: EventDto | null
  mode: 'create' | 'edit' | 'view'
}

interface Emits {
  (e: 'close'): void
  (e: 'submit', data: EventFormData): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const eventTypes = [
  'Container Arrival',
  'Container Loading',
  'Container Unloading',
  'Ship Arrival',
  'Ship Departure',
  'Berth Assignment',
  'Berth Release',
  'System Alert',
  'Security Alert',
  'Operation Completed',
  'User Action',
  'System Maintenance'
]

const formData = ref<EventFormData>({
  eventType: '',
  title: '',
  description: '',
  severity: '',
  containerId: '',
  shipId: undefined,
  berthId: undefined,
  portId: undefined,
  source: ''
})

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

const handleSubmit = () => {
  emit('submit', formData.value)
}

// Watch for event changes to populate form
watch(() => props.event, (newEvent) => {
  if (newEvent) {
    formData.value = {
      eventType: newEvent.eventType,
      title: newEvent.title,
      description: newEvent.description,
      severity: newEvent.severity,
      containerId: newEvent.containerId || '',
      shipId: newEvent.shipId,
      berthId: newEvent.berthId,
      portId: newEvent.portId,
      source: newEvent.source
    }
  } else {
    // Reset form for create mode
    formData.value = {
      eventType: '',
      title: '',
      description: '',
      severity: '',
      containerId: '',
      shipId: undefined,
      berthId: undefined,
      portId: undefined,
      source: ''
    }
  }
}, { immediate: true })
</script>