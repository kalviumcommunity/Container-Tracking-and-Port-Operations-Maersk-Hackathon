<template>
  <div class="relative bg-white rounded-xl shadow-2xl w-full max-w-4xl mx-auto max-h-[95vh] overflow-hidden">
    <!-- Header with Back Button -->
    <div class="sticky top-0 bg-white border-b border-slate-200 px-4 sm:px-6 py-3 sm:py-4 flex items-center gap-3 sm:gap-4 z-10">
      <button
        @click="$emit('cancel')"
        class="p-2 hover:bg-slate-100 rounded-lg transition-colors flex-shrink-0"
        title="Go back"
      >
        <ArrowLeft :size="20" class="text-slate-600" />
      </button>
      <div class="flex items-center gap-2 sm:gap-3 min-w-0">
        <div class="p-2 bg-blue-600 rounded-lg shadow-lg flex-shrink-0">
          <Ship :size="18" class="text-white sm:w-5 sm:h-5" />
        </div>
        <div class="min-w-0">
          <h2 class="text-lg sm:text-xl font-bold text-slate-900 truncate">
            {{ isEditing ? 'Edit Ship' : 'Add New Ship' }}
          </h2>
          <p class="text-xs sm:text-sm text-slate-600 hidden sm:block">Manage vessel information and cargo capacity</p>
        </div>
      </div>
    </div>

    <!-- Form Content -->
    <div class="overflow-y-auto max-h-[calc(95vh-64px)] sm:max-h-[calc(95vh-80px)]">
      <form @submit.prevent="handleSubmit" class="p-4 sm:p-6 space-y-4 sm:space-y-6">
        <!-- Ship Name -->
        <div>
        <label for="name" class="block text-sm font-medium text-slate-700 mb-2">
          Ship Name *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Ship :size="20" class="text-slate-400" />
          </div>
          <input
            id="name"
            v-model="form.name"
            type="text"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., MSC Gülsün"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.name }"
          />
        </div>
        <p v-if="errors.name" class="mt-1 text-sm text-red-600">{{ errors.name }}</p>
      </div>

      <!-- IMO Number -->
      <div>
        <label for="imoNumber" class="block text-sm font-medium text-slate-700 mb-2">
          IMO Number *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Hash :size="20" class="text-slate-400" />
          </div>
          <input
            id="imoNumber"
            v-model="form.imoNumber"
            type="text"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., IMO 9863043"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.imoNumber }"
          />
        </div>
        <p v-if="errors.imoNumber" class="mt-1 text-sm text-red-600">{{ errors.imoNumber }}</p>
      </div>

      <!-- Capacity -->
      <div>
        <label for="capacity" class="block text-sm font-medium text-slate-700 mb-2">
          Capacity (TEU) *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Package :size="20" class="text-slate-400" />
          </div>
          <input
            id="capacity"
            v-model.number="form.capacity"
            type="number"
            min="1"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., 23756"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.capacity }"
          />
        </div>
        <p v-if="errors.capacity" class="mt-1 text-sm text-red-600">{{ errors.capacity }}</p>
      </div>

      <!-- Current Port -->
      <div>
        <label for="currentPortId" class="block text-sm font-medium text-slate-700 mb-2">
          Current Port
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <MapPin :size="20" class="text-slate-400" />
          </div>
          <select
            id="currentPortId"
            v-model="form.currentPortId"
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
          >
            <option value="">Select current port</option>
            <option v-for="port in availablePorts" :key="port.id" :value="port.id">
              {{ port.name }} ({{ port.code }})
            </option>
          </select>
        </div>
      </div>

      <!-- Status -->
      <div>
        <label for="status" class="block text-sm font-medium text-slate-700 mb-2">
          Status *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Activity :size="20" class="text-slate-400" />
          </div>
          <select
            id="status"
            v-model="form.status"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.status }"
          >
            <option value="">Select status</option>
            <option value="AtSea">At Sea</option>
            <option value="Docked">Docked</option>
            <option value="Loading">Loading</option>
            <option value="Unloading">Unloading</option>
            <option value="Maintenance">Maintenance</option>
          </select>
        </div>
        <p v-if="errors.status" class="mt-1 text-sm text-red-600">{{ errors.status }}</p>
      </div>

      <!-- Arrival Date -->
      <div>
        <label for="arrivalDate" class="block text-sm font-medium text-slate-700 mb-2">
          Expected/Actual Arrival Date
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Calendar :size="20" class="text-slate-400" />
          </div>
          <input
            id="arrivalDate"
            v-model="form.arrivalDate"
            type="datetime-local"
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.arrivalDate }"
          />
        </div>
        <p v-if="errors.arrivalDate" class="mt-1 text-sm text-red-600">{{ errors.arrivalDate }}</p>
      </div>

      <!-- Departure Date -->
      <div>
        <label for="departureDate" class="block text-sm font-medium text-slate-700 mb-2">
          Expected/Actual Departure Date
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Calendar :size="20" class="text-slate-400" />
          </div>
          <input
            id="departureDate"
            v-model="form.departureDate"
            type="datetime-local"
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.departureDate }"
          />
        </div>
        <p v-if="errors.departureDate" class="mt-1 text-sm text-red-600">{{ errors.departureDate }}</p>
      </div>

      <!-- Captain -->
      <div>
        <label for="captain" class="block text-sm font-medium text-slate-700 mb-2">
          Captain Name
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <User :size="20" class="text-slate-400" />
          </div>
          <input
            id="captain"
            v-model="form.captain"
            type="text"
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., Captain John Smith"
          />
        </div>
      </div>

      <!-- Flag State -->
      <div>
        <label for="flagState" class="block text-sm font-medium text-slate-700 mb-2">
          Flag State
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Flag :size="20" class="text-slate-400" />
          </div>
          <input
            id="flagState"
            v-model="form.flagState"
            type="text"
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., Panama"
          />
        </div>
      </div>

      <!-- Current Container Count -->
      <div>
        <label class="block text-sm font-medium text-slate-700 mb-2">
          Current Container Load
        </label>
        <div class="bg-slate-50 rounded-lg p-4">
          <div class="flex items-center justify-between mb-2">
            <span class="text-sm text-slate-600">Containers on board:</span>
            <span class="font-semibold text-slate-900">{{ form.currentContainerCount || 0 }} / {{ form.capacity || 0 }}</span>
          </div>
          <div class="w-full bg-slate-200 rounded-full h-2">
            <div 
              class="bg-blue-600 h-2 rounded-full transition-all"
              :style="{ width: `${Math.min(((form.currentContainerCount || 0) / (form.capacity || 1)) * 100, 100)}%` }"
            ></div>
          </div>
        </div>
      </div>

      <!-- Notes -->
      <div>
        <label for="notes" class="block text-sm font-medium text-slate-700 mb-2">
          Notes
        </label>
        <textarea
          id="notes"
          v-model="form.notes"
          rows="3"
          class="w-full px-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors resize-none"
          placeholder="Additional notes about the ship..."
        ></textarea>
      </div>

      <!-- Actions -->
      <div class="flex gap-4 pt-4 border-t border-slate-200">
        <button
          type="submit"
          :disabled="isLoading"
          class="flex-1 flex justify-center items-center gap-2 py-3 px-4 border border-transparent rounded-lg shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          <Loader2 v-if="isLoading" :size="20" class="animate-spin" />
          <Save v-else :size="20" />
          {{ isLoading ? 'Saving...' : (isEditing ? 'Update Ship' : 'Create Ship') }}
        </button>
        
        <button
          type="button"
          @click="$emit('cancel')"
          class="px-6 py-3 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors"
        >
          Cancel
        </button>
      </div>
    </form>

    <!-- Messages -->
    <div v-if="errorMessage" class="bg-red-50 border border-red-200 rounded-lg p-4">
      <div class="flex items-center gap-2">
        <AlertTriangle :size="20" class="text-red-600" />
        <p class="text-red-800">{{ errorMessage }}</p>
      </div>
    </div>

    <div v-if="successMessage" class="bg-green-50 border border-green-200 rounded-lg p-4">
      <div class="flex items-center gap-2">
        <CheckCircle :size="20" class="text-green-600" />
        <p class="text-green-800">{{ successMessage }}</p>
      </div>
    </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, watch } from 'vue'
import { Ship, Hash, Package, MapPin, Activity, Calendar, User, Flag, Save, Loader2, AlertTriangle, CheckCircle, ArrowLeft } from 'lucide-vue-next'

interface ShipForm {
  id?: number
  shipId?: number
  name: string
  imoNumber: string
  capacity: number
  currentPortId?: number
  status: string
  arrivalDate?: string
  departureDate?: string
  captain?: string
  flagState?: string
  currentContainerCount?: number
  notes?: string
  // Additional fields for backend compatibility
  flag?: string
  type?: string
  length?: number
  beam?: number
  draft?: number
  grossTonnage?: number
  yearBuilt?: number
  coordinates?: string
  speed?: number
  heading?: number
  nextPort?: string
}

interface Port {
  id: number
  name: string
  code: string
}

const props = defineProps<{
  ship?: ShipForm
  isEditing?: boolean
}>()

const emit = defineEmits(['submit', 'cancel'])

const form = reactive<ShipForm>({
  name: '',
  imoNumber: '',
  capacity: 0,
  currentPortId: undefined,
  status: '',
  arrivalDate: '',
  departureDate: '',
  captain: '',
  flagState: '',
  currentContainerCount: 0,
  notes: ''
})

const errors = reactive<Partial<Record<keyof ShipForm, string>>>({})
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const availablePorts = ref<Port[]>([])

const validate = (): boolean => {
  Object.keys(errors).forEach(key => delete errors[key as keyof ShipForm])
  
  if (!form.name.trim()) {
    errors.name = 'Ship name is required'
    return false
  }
  
  if (!form.imoNumber.trim()) {
    errors.imoNumber = 'IMO number is required'
    return false
  } else if (!/^IMO\s?\d{7}$/.test(form.imoNumber.toUpperCase().replace(/\s+/g, ' '))) {
    errors.imoNumber = 'Invalid IMO number format (e.g., IMO 9863043)'
    return false
  }
  
  if (!form.capacity || form.capacity <= 0) {
    errors.capacity = 'Capacity must be greater than 0'
    return false
  }
  
  if (!form.status) {
    errors.status = 'Status is required'
    return false
  }
  
  if (form.arrivalDate && form.departureDate) {
    const arrival = new Date(form.arrivalDate)
    const departure = new Date(form.departureDate)
    if (departure <= arrival) {
      errors.departureDate = 'Departure date must be after arrival date'
      return false
    }
  }
  
  if (form.currentContainerCount && form.currentContainerCount > form.capacity) {
    errors.currentContainerCount = 'Container count cannot exceed ship capacity'
    return false
  }
  
  return true
}

const loadAvailablePorts = async () => {
  try {
    // Mock API call to load ports
    availablePorts.value = [
      { id: 1, name: 'Port of Shanghai', code: 'CNSHA' },
      { id: 2, name: 'Port of Singapore', code: 'SGSIN' },
      { id: 3, name: 'Port of Ningbo-Zhoushan', code: 'CNNGB' },
      { id: 4, name: 'Port of Shenzhen', code: 'CNSZN' },
      { id: 5, name: 'Port of Guangzhou', code: 'CNGZH' },
      { id: 6, name: 'Port of Busan', code: 'KRPUS' },
      { id: 7, name: 'Port of Hong Kong', code: 'HKHKG' },
      { id: 8, name: 'Port of Qingdao', code: 'CNTAO' }
    ]
  } catch (error) {
    console.error('Failed to load ports:', error)
  }
}

const handleSubmit = async () => {
  if (!validate()) return
  
  isLoading.value = true
  errorMessage.value = ''
  successMessage.value = ''
  
  try {
    // Import shipApi dynamically
    const { shipApi } = await import('../services/shipApi')
    
    const shipData = {
      name: form.name,
      imoNumber: form.imoNumber.toUpperCase().replace(/\s+/g, ' '),
      flag: form.flag || '',
      type: form.type || 'Container Ship',
      capacity: Number(form.capacity),
      status: form.status,
      length: form.length ? Number(form.length) : undefined,
      beam: form.beam ? Number(form.beam) : undefined,
      draft: form.draft ? Number(form.draft) : undefined,
      grossTonnage: form.grossTonnage ? Number(form.grossTonnage) : undefined,
      yearBuilt: form.yearBuilt ? Number(form.yearBuilt) : undefined,
      coordinates: form.coordinates || '',
      speed: form.speed ? Number(form.speed) : undefined,
      heading: form.heading ? Number(form.heading) : undefined,
      nextPort: form.nextPort || '',
      estimatedArrival: form.arrivalDate || undefined,
      currentPortId: form.currentPortId ? Number(form.currentPortId) : undefined
    }
    
    let result
    if (props.isEditing && props.ship?.shipId) {
      // Update existing ship
      result = await shipApi.update(props.ship.shipId, shipData)
    } else {
      // Create new ship
      result = await shipApi.create(shipData)
    }
    
    successMessage.value = props.isEditing 
      ? 'Ship updated successfully!' 
      : 'Ship created successfully!'
    
    setTimeout(() => {
      emit('submit', result.data)
    }, 1000)
    
  } catch (error) {
    errorMessage.value = props.isEditing 
      ? 'Failed to update ship. Please try again.' 
      : 'Failed to create ship. Please try again.'
  } finally {
    isLoading.value = false
  }
}

// Watch for capacity changes to validate container count
watch(() => form.capacity, () => {
  if (form.currentContainerCount && form.currentContainerCount > form.capacity) {
    form.currentContainerCount = form.capacity
  }
})

onMounted(() => {
  if (props.ship) {
    Object.assign(form, props.ship)
  }
  loadAvailablePorts()
})
</script>