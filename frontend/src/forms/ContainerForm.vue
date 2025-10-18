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
          <Package :size="18" class="text-white sm:w-5 sm:h-5" />
        </div>
        <div class="min-w-0">
          <h2 class="text-lg sm:text-xl font-bold text-slate-900 truncate">
            {{ isEditing ? 'Edit Container' : 'Add New Container' }}
          </h2>
          <p class="text-xs sm:text-sm text-slate-600 hidden sm:block">Manage container information and tracking</p>
        </div>
      </div>
    </div>

    <!-- Form Content -->
    <div class="overflow-y-auto max-h-[calc(95vh-64px)] sm:max-h-[calc(95vh-80px)]">
      <form @submit.prevent="handleSubmit" class="p-4 sm:p-6 space-y-4 sm:space-y-6">
      <!-- Container Number -->
      <div>
        <label for="containerNumber" class="block text-sm font-medium text-slate-700 mb-2">
          Container Number *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Hash :size="18" class="text-slate-400 sm:w-5 sm:h-5" />
          </div>
          <input
            id="containerNumber"
            v-model="form.containerNumber"
            type="text"
            required
            class="w-full pl-10 sm:pl-12 pr-4 py-2.5 sm:py-3 text-sm sm:text-base border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., MSCU1234567"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.containerNumber }"
          />
        </div>
        <p v-if="errors.containerNumber" class="mt-1 text-sm text-red-600">{{ errors.containerNumber }}</p>
      </div>

      <!-- Container Type -->
      <div>
        <label for="type" class="block text-sm font-medium text-slate-700 mb-2">
          Container Type *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Package :size="20" class="text-slate-400" />
          </div>
          <select
            id="type"
            v-model="form.type"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.type }"
          >
            <option value="">Select container type</option>
            <option value="Dry">Dry Container</option>
            <option value="Refrigerated">Refrigerated Container</option>
            <option value="Tank">Tank Container</option>
            <option value="OpenTop">Open Top Container</option>
            <option value="FlatRack">Flat Rack Container</option>
          </select>
        </div>
        <p v-if="errors.type" class="mt-1 text-sm text-red-600">{{ errors.type }}</p>
      </div>

      <!-- Weight -->
      <div>
        <label for="weight" class="block text-sm font-medium text-slate-700 mb-2">
          Weight (kg) *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Scale :size="20" class="text-slate-400" />
          </div>
          <input
            id="weight"
            v-model.number="form.weight"
            type="number"
            min="0"
            step="0.01"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="Enter weight in kg"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.weight }"
          />
        </div>
        <p v-if="errors.weight" class="mt-1 text-sm text-red-600">{{ errors.weight }}</p>
      </div>

      <!-- Origin -->
      <div>
        <label for="origin" class="block text-sm font-medium text-slate-700 mb-2">
          Origin *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <MapPin :size="20" class="text-slate-400" />
          </div>
          <input
            id="origin"
            v-model="form.origin"
            type="text"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., Shanghai, China"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.origin }"
          />
        </div>
        <p v-if="errors.origin" class="mt-1 text-sm text-red-600">{{ errors.origin }}</p>
      </div>

      <!-- Destination -->
      <div>
        <label for="destination" class="block text-sm font-medium text-slate-700 mb-2">
          Destination *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <MapPin :size="20" class="text-slate-400" />
          </div>
          <input
            id="destination"
            v-model="form.destination"
            type="text"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., Los Angeles, USA"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.destination }"
          />
        </div>
        <p v-if="errors.destination" class="mt-1 text-sm text-red-600">{{ errors.destination }}</p>
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
            <option value="InTransit">In Transit</option>
            <option value="AtPort">At Port</option>
            <option value="Loading">Loading</option>
            <option value="Unloading">Unloading</option>
            <option value="Delivered">Delivered</option>
          </select>
        </div>
        <p v-if="errors.status" class="mt-1 text-sm text-red-600">{{ errors.status }}</p>
      </div>

      <!-- Ship Assignment -->
      <div>
        <label for="shipId" class="block text-sm font-medium text-slate-700 mb-2">
          Assign to Ship
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Ship :size="20" class="text-slate-400" />
          </div>
          <select
            id="shipId"
            v-model="form.shipId"
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
          >
            <option value="">No ship assigned</option>
            <option v-for="ship in availableShips" :key="ship.id" :value="ship.id">
              {{ ship.name }} ({{ ship.capacity }} TEU)
            </option>
          </select>
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
          placeholder="Additional notes or special instructions..."
        ></textarea>
      </div>

      <!-- Actions -->
      <div class="flex gap-4 pt-4 border-t border-slate-200">
        <button
          type="submit"
          :disabled="isLoading"
          class="flex-1 flex justify-center items-center gap-2 py-2.5 sm:py-3 px-4 text-sm sm:text-base border border-transparent rounded-lg shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          <Loader2 v-if="isLoading" :size="18" class="animate-spin sm:w-5 sm:h-5" />
          <Save v-else :size="18" class="sm:w-5 sm:h-5" />
          <span class="hidden sm:inline">{{ isLoading ? 'Saving...' : (isEditing ? 'Update Container' : 'Create Container') }}</span>
          <span class="sm:hidden">{{ isLoading ? 'Saving...' : (isEditing ? 'Update' : 'Create') }}</span>
        </button>
        
        <button
          type="button"
          @click="$emit('cancel')"
          class="px-4 sm:px-6 py-2.5 sm:py-3 text-sm sm:text-base border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors"
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
import { ref, reactive, onMounted } from 'vue'
import { Package, Hash, Scale, MapPin, Activity, Ship, Save, Loader2, AlertTriangle, CheckCircle, ArrowLeft } from 'lucide-vue-next'

interface Container {
  containerId?: string
  containerNumber: string
  cargoType?: string
  cargoDescription?: string
  type: string
  status: string
  condition?: string
  currentLocation?: string
  origin?: string
  destination: string
  weight: number
  maxWeight?: number
  size?: string
  temperature?: number
  coordinates?: string
  shipId?: number
  notes?: string
}

interface Ship {
  id: number
  name: string
  capacity: number
}

const props = defineProps<{
  container?: Container
  isEditing?: boolean
}>()

const emit = defineEmits(['submit', 'cancel'])

const form = reactive<Container>({
  containerNumber: '',
  cargoType: '',
  cargoDescription: '',
  type: '',
  status: 'Available',
  condition: 'Good',
  currentLocation: '',
  origin: '',
  destination: '',
  weight: 0,
  maxWeight: undefined,
  size: '20ft',
  temperature: undefined,
  coordinates: '',
  shipId: undefined,
  notes: ''
})

const errors = reactive<Partial<Record<keyof Container, string>>>({})
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const availableShips = ref<Ship[]>([])

const validate = (): boolean => {
  Object.keys(errors).forEach(key => delete errors[key as keyof Container])
  
  if (!form.containerNumber.trim()) {
    errors.containerNumber = 'Container number is required'
    return false
  } else if (!/^[A-Z]{4}[0-9]{7}$/.test(form.containerNumber.toUpperCase())) {
    errors.containerNumber = 'Invalid container number format (e.g., MSCU1234567)'
    return false
  }
  
  if (!form.type) {
    errors.type = 'Container type is required'
    return false
  }
  
  if (!form.weight || form.weight <= 0) {
    errors.weight = 'Weight must be greater than 0'
    return false
  } else if (form.weight > 100000) {
    errors.weight = 'Weight cannot exceed 100,000 kg'
    return false
  }
  
  if (!form.origin?.trim()) {
    errors.origin = 'Origin is required'
    return false
  }
  
  if (!form.destination?.trim()) {
    errors.destination = 'Destination is required'
    return false
  }
  
  if (!form.status) {
    errors.status = 'Status is required'
    return false
  }
  
  return true
}

const loadAvailableShips = async () => {
  try {
    // TODO: Replace with actual API call to load ships
    // const response = await shipApi.getAll()
    // availableShips.value = response.data || []
    
    // For now, show empty state until API integration is complete
    availableShips.value = []
  } catch (error) {
    console.error('Failed to load ships:', error)
    availableShips.value = []
  }
}

const handleSubmit = async () => {
  if (!validate()) return
  
  isLoading.value = true
  errorMessage.value = ''
  successMessage.value = ''
  
  try {
    // Import containerApi dynamically
    const { containerApi } = await import('../services/containerApi')
    
    const containerData = {
      containerId: form.containerNumber.toUpperCase(),
      cargoType: form.cargoType || '',
      cargoDescription: form.cargoDescription || '',
      type: form.type,
      status: form.status || 'Available',
      condition: form.condition || 'Good',
      currentLocation: form.origin || '',
      destination: form.destination || '',
      weight: Number(form.weight),
      maxWeight: form.maxWeight ? Number(form.maxWeight) : undefined,
      size: form.size || '20ft',
      temperature: form.temperature ? Number(form.temperature) : undefined,
      coordinates: form.coordinates || '',
      shipId: form.shipId ? Number(form.shipId) : undefined
    }
    
    let result
    if (props.isEditing && props.container?.containerId) {
      // Update existing container
      result = await containerApi.update(props.container.containerId, containerData)
    } else {
      // Create new container  
      result = await containerApi.create(containerData)
    }
    
    successMessage.value = props.isEditing 
      ? 'Container updated successfully!' 
      : 'Container created successfully!'
    
    setTimeout(() => {
      emit('submit', result.data)
    }, 1000)
    
  } catch (error: any) {
    console.error('Container form submission error:', error)
    errorMessage.value = error.response?.data?.message || 
      (props.isEditing 
        ? 'Failed to update container. Please try again.' 
        : 'Failed to create container. Please try again.')
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  if (props.container) {
    Object.assign(form, props.container)
  }
  loadAvailableShips()
})
</script>
