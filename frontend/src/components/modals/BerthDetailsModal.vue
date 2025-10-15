<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="isOpen && berth" class="fixed inset-0 z-50 overflow-y-auto">
        <!-- Backdrop -->
        <div 
          class="fixed inset-0 bg-black/50 backdrop-blur-sm transition-opacity"
          @click="emit('close')"
        ></div>

        <!-- Modal Container -->
        <div class="flex min-h-screen items-center justify-center p-4">
          <div 
            class="relative bg-white rounded-2xl shadow-2xl w-full max-w-5xl transform transition-all max-h-[90vh] overflow-y-auto"
            @click.stop
          >
            <!-- Header -->
            <div class="sticky top-0 bg-gradient-to-r from-blue-600 to-blue-700 px-6 py-4 rounded-t-2xl z-10">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="w-12 h-12 bg-white/20 rounded-lg flex items-center justify-center">
                    <Anchor :size="24" class="text-white" />
                  </div>
                  <div>
                    <h2 class="text-2xl font-bold text-white">{{ berth.name }}</h2>
                    <p class="text-blue-100 text-sm">{{ berth.identifier }} - {{ getPortName(berth.portId) }}</p>
                  </div>
                </div>
                <button 
                  @click="emit('close')"
                  class="p-2 hover:bg-white/10 rounded-lg transition-colors"
                >
                  <X :size="24" class="text-white" />
                </button>
              </div>
            </div>

            <!-- Content -->
            <div class="p-6">
              <!-- Status Banner -->
              <div class="mb-6 p-4 rounded-xl border-2" :class="getStatusBannerClass(berth.status)">
                <div class="flex items-center justify-between">
                  <div class="flex items-center gap-3">
                    <component :is="getStatusIcon(berth.status)" :size="24" />
                    <div>
                      <p class="font-semibold text-lg">Current Status: {{ berth.status }}</p>
                      <p class="text-sm opacity-90">{{ getStatusDescription(berth.status) }}</p>
                    </div>
                  </div>
                  <span 
                    class="px-4 py-2 rounded-full text-sm font-semibold"
                    :class="getStatusClass(berth.status)"
                  >
                    {{ berth.status }}
                  </span>
                </div>
              </div>

              <!-- Key Metrics Grid -->
              <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
                <div class="bg-gradient-to-br from-blue-50 to-blue-100 rounded-xl p-4 border border-blue-200">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-sm font-medium text-blue-800">Capacity</span>
                    <Container :size="20" class="text-blue-600" />
                  </div>
                  <p class="text-2xl font-bold text-blue-900">{{ berth.capacity }}</p>
                  <p class="text-xs text-blue-700 mt-1">Maximum capacity</p>
                </div>

                <div class="bg-gradient-to-br from-green-50 to-green-100 rounded-xl p-4 border border-green-200">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-sm font-medium text-green-800">Current Load</span>
                    <TrendingUp :size="20" class="text-green-600" />
                  </div>
                  <p class="text-2xl font-bold text-green-900">{{ berth.currentLoad }}</p>
                  <p class="text-xs text-green-700 mt-1">{{ utilizationPercentage }}% utilized</p>
                </div>

                <div class="bg-gradient-to-br from-purple-50 to-purple-100 rounded-xl p-4 border border-purple-200">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-sm font-medium text-purple-800">Priority</span>
                    <Zap :size="20" class="text-purple-600" />
                  </div>
                  <p class="text-2xl font-bold text-purple-900">{{ berth.priority }}</p>
                  <p class="text-xs text-purple-700 mt-1">Berth priority level</p>
                </div>

                <div class="bg-gradient-to-br from-amber-50 to-amber-100 rounded-xl p-4 border border-amber-200">
                  <div class="flex items-center justify-between mb-2">
                    <span class="text-sm font-medium text-amber-800">Assignments</span>
                    <Clipboard :size="20" class="text-amber-600" />
                  </div>
                  <p class="text-2xl font-bold text-amber-900">{{ berth.activeAssignmentCount || 0 }}</p>
                  <p class="text-xs text-amber-700 mt-1">Active assignments</p>
                </div>
              </div>

              <!-- Utilization Progress Bar -->
              <div class="mb-6 p-4 bg-slate-50 rounded-xl border border-slate-200">
                <div class="flex justify-between items-center mb-2">
                  <span class="text-sm font-medium text-slate-700">Capacity Utilization</span>
                  <span class="text-sm font-semibold text-slate-900">
                    {{ berth.currentLoad }} / {{ berth.capacity }} ({{ utilizationPercentage }}%)
                  </span>
                </div>
                <div class="w-full bg-slate-200 rounded-full h-3">
                  <div 
                    class="h-3 rounded-full transition-all duration-300 flex items-center justify-end pr-2"
                    :class="getUtilizationColorClass(utilizationRatio)"
                    :style="{ width: `${utilizationPercentage}%` }"
                  >
                    <span v-if="utilizationPercentage > 10" class="text-white text-xs font-medium">
                      {{ utilizationPercentage }}%
                    </span>
                  </div>
                </div>
              </div>

              <!-- Tabs -->
              <div class="mb-6 border-b border-slate-200">
                <div class="flex gap-4">
                  <button
                    v-for="tab in tabs"
                    :key="tab.id"
                    @click="activeTab = tab.id"
                    :class="[
                      'px-4 py-3 font-medium text-sm transition-colors flex items-center gap-2',
                      activeTab === tab.id
                        ? 'text-blue-600 border-b-2 border-blue-600'
                        : 'text-slate-600 hover:text-slate-900'
                    ]"
                  >
                    <component :is="tab.icon" :size="18" />
                    {{ tab.label }}
                  </button>
                </div>
              </div>

              <!-- Tab Content -->
              <div class="min-h-[300px]">
                <!-- Technical Specifications Tab -->
                <div v-if="activeTab === 'specs'" class="space-y-6">
                  <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div class="bg-white border border-slate-200 rounded-lg p-4">
                      <p class="text-sm text-slate-600 mb-1">Berth Type</p>
                      <p class="text-lg font-semibold text-slate-900">{{ berth.type }}</p>
                    </div>
                    
                    <div class="bg-white border border-slate-200 rounded-lg p-4">
                      <p class="text-sm text-slate-600 mb-1">Priority Level</p>
                      <p class="text-lg font-semibold text-slate-900">{{ berth.priority || 'Medium' }}</p>
                    </div>
                    
                    <div class="bg-white border border-slate-200 rounded-lg p-4">
                      <p class="text-sm text-slate-600 mb-1">Max Ship Length</p>
                      <p class="text-lg font-semibold text-slate-900">
                        {{ berth.maxShipLength ? `${berth.maxShipLength}m` : 'Not specified' }}
                      </p>
                    </div>
                    
                    <div class="bg-white border border-slate-200 rounded-lg p-4">
                      <p class="text-sm text-slate-600 mb-1">Max Draft</p>
                      <p class="text-lg font-semibold text-slate-900">
                        {{ berth.maxDraft ? `${berth.maxDraft}m` : 'Not specified' }}
                      </p>
                    </div>
                    
                    <div class="bg-white border border-slate-200 rounded-lg p-4">
                      <p class="text-sm text-slate-600 mb-1">Crane Count</p>
                      <p class="text-lg font-semibold text-slate-900">{{ berth.craneCount || 0 }}</p>
                    </div>
                    
                    <div class="bg-white border border-slate-200 rounded-lg p-4">
                      <p class="text-sm text-slate-600 mb-1">Hourly Rate</p>
                      <p class="text-lg font-semibold text-slate-900">
                        {{ berth.hourlyRate ? `$${berth.hourlyRate}` : 'Not specified' }}
                      </p>
                    </div>
                    
                    <div class="bg-white border border-slate-200 rounded-lg p-4">
                      <p class="text-sm text-slate-600 mb-1">Port</p>
                      <p class="text-lg font-semibold text-slate-900">{{ getPortName(berth.portId) }}</p>
                    </div>
                  </div>

                  <!-- Available Services -->
                  <div v-if="berth.availableServices">
                    <h4 class="text-lg font-semibold text-slate-900 mb-3">Available Services</h4>
                    <div class="flex flex-wrap gap-2">
                      <span 
                        v-for="service in parseServices(berth.availableServices)" 
                        :key="service"
                        class="px-3 py-1.5 bg-blue-100 text-blue-800 text-sm font-medium rounded-lg flex items-center gap-2"
                      >
                        <Check :size="14" />
                        {{ service }}
                      </span>
                    </div>
                  </div>
                </div>

                <!-- Location Tab -->
                <div v-if="activeTab === 'location'" class="space-y-6">
                  <div class="bg-slate-50 border border-slate-200 rounded-xl p-6">
                    <h4 class="text-lg font-semibold text-slate-900 mb-4 flex items-center gap-2">
                      <MapPin :size="20" class="text-blue-600" />
                      Location Information
                    </h4>
                    
                    <div class="space-y-4">
                      <div>
                        <p class="text-sm text-slate-600 mb-1">Port Location</p>
                        <p class="text-base font-medium text-slate-900">
                          {{ getPortName(berth.portId) }}
                        </p>
                      </div>

                      <div>
                        <p class="text-sm text-slate-600 mb-1">Berth Identifier</p>
                        <p class="text-base font-medium text-slate-900">
                          {{ berth.identifier || 'Not specified' }}
                        </p>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Activity Tab -->
                <div v-if="activeTab === 'activity'" class="space-y-4">
                  <div class="bg-slate-50 border border-slate-200 rounded-xl p-6">
                    <h4 class="text-lg font-semibold text-slate-900 mb-4 flex items-center gap-2">
                      <Activity :size="20" class="text-blue-600" />
                      Recent Activity
                    </h4>
                    
                    <div class="text-center py-12 text-slate-500">
                      <Clock :size="48" class="mx-auto mb-4 opacity-50" />
                      <p>Activity tracking coming soon</p>
                    </div>
                  </div>
                </div>

                <!-- Notes Tab -->
                <div v-if="activeTab === 'notes'" class="space-y-4">
                  <div class="bg-slate-50 border border-slate-200 rounded-xl p-6">
                    <h4 class="text-lg font-semibold text-slate-900 mb-4 flex items-center gap-2">
                      <FileText :size="20" class="text-blue-600" />
                      Additional Notes
                    </h4>
                    
                    <div v-if="berth.notes" class="prose prose-sm max-w-none">
                      <p class="text-slate-700 whitespace-pre-wrap">{{ berth.notes }}</p>
                    </div>
                    <div v-else class="text-center py-12 text-slate-500">
                      <FileText :size="48" class="mx-auto mb-4 opacity-50" />
                      <p>No notes available</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Footer Actions -->
            <div class="sticky bottom-0 bg-slate-50 px-6 py-4 border-t border-slate-200 rounded-b-2xl flex items-center justify-between">
              <button
                @click="emit('close')"
                class="px-6 py-2.5 border border-slate-300 text-slate-700 rounded-lg hover:bg-white transition-colors"
              >
                Close
              </button>
              <div class="flex gap-3">
                <button
                  @click="emit('edit', berth)"
                  class="px-6 py-2.5 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors flex items-center gap-2"
                >
                  <Edit2 :size="18" />
                  Edit Berth
                </button>
                <button
                  @click="emit('assign', berth)"
                  class="px-6 py-2.5 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors flex items-center gap-2"
                >
                  <Plus :size="18" />
                  New Assignment
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { 
  Anchor, X, Container, TrendingUp, Zap, Clipboard, Check, MapPin,
  ExternalLink, Activity, Clock, FileText, Edit2, Plus, Info, Settings,
  CheckCircle2, AlertCircle, Wrench
} from 'lucide-vue-next'
import type { Berth } from '../../types/berth'
import type { Port } from '../../types/port'

// Props
const props = defineProps<{
  isOpen: boolean
  berth: Berth | null
  ports: Port[]
}>()

// Emits
const emit = defineEmits<{
  close: []
  edit: [berth: Berth]
  assign: [berth: Berth]
}>()

// State
const activeTab = ref('specs')

// Tabs configuration
const tabs = [
  { id: 'specs', label: 'Specifications', icon: Settings },
  { id: 'location', label: 'Location', icon: MapPin },
  { id: 'activity', label: 'Activity', icon: Activity },
  { id: 'notes', label: 'Notes', icon: FileText }
]

// Computed
const utilizationRatio = computed(() => {
  if (!props.berth) return 0
  return props.berth.capacity > 0 ? props.berth.currentLoad / props.berth.capacity : 0
})

const utilizationPercentage = computed(() => {
  return Math.round(utilizationRatio.value * 100)
})

// Helper functions
const getPortName = (portId: number) => {
  const port = props.ports.find(p => p.portId === portId)
  return port ? port.name : 'Unknown Port'
}

const getStatusClass = (status: string) => {
  switch (status) {
    case 'Available':
      return 'bg-green-100 text-green-800'
    case 'Occupied':
      return 'bg-amber-100 text-amber-800'
    case 'Maintenance':
      return 'bg-red-100 text-red-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}

const getStatusBannerClass = (status: string) => {
  switch (status) {
    case 'Available':
      return 'bg-green-50 border-green-200 text-green-800'
    case 'Occupied':
      return 'bg-amber-50 border-amber-200 text-amber-800'
    case 'Maintenance':
      return 'bg-red-50 border-red-200 text-red-800'
    default:
      return 'bg-gray-50 border-gray-200 text-gray-800'
  }
}

const getStatusIcon = (status: string) => {
  switch (status) {
    case 'Available':
      return CheckCircle2
    case 'Occupied':
      return AlertCircle
    case 'Maintenance':
      return Wrench
    default:
      return Info
  }
}

const getStatusDescription = (status: string) => {
  switch (status) {
    case 'Available':
      return 'This berth is available for new assignments'
    case 'Occupied':
      return 'This berth is currently occupied'
    case 'Maintenance':
      return 'This berth is under maintenance and unavailable'
    default:
      return 'Status information not available'
  }
}

const getUtilizationColorClass = (ratio: number) => {
  if (ratio >= 0.9) return 'bg-red-500'
  if (ratio >= 0.7) return 'bg-amber-500'
  return 'bg-green-500'
}

const parseServices = (services: string) => {
  return services ? services.split(',').map(s => s.trim()).filter(s => s) : []
}

const formatDate = (date: string | Date) => {
  if (!date) return 'N/A'
  return new Date(date).toLocaleString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active .relative,
.modal-leave-active .relative {
  transition: transform 0.3s ease;
}

.modal-enter-from .relative,
.modal-leave-to .relative {
  transform: scale(0.95);
}
</style>
