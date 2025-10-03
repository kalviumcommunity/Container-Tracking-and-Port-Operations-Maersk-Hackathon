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
        <div class="p-2 bg-purple-600 rounded-lg shadow-lg flex-shrink-0">
          <Anchor :size="18" class="text-white sm:w-5 sm:h-5" />
        </div>
        <div class="min-w-0">
          <h2 class="text-lg sm:text-xl font-bold text-slate-900 truncate">
            {{ isEditing ? 'Edit Berth' : 'Add New Berth' }}
          </h2>
          <p class="text-xs sm:text-sm text-slate-600 hidden sm:block">Manage berth information and capacity</p>
        </div>
      </div>
    </div>

    <!-- Form Content -->
    <div class="overflow-y-auto max-h-[calc(95vh-64px)] sm:max-h-[calc(95vh-80px)]">
      <form @submit.prevent="handleSubmit" class="p-4 sm:p-6 space-y-4 sm:space-y-6">
        <!-- Berth Number -->
        <div>
          <label for="berthNumber" class="block text-sm font-medium text-slate-700 mb-2">
            Berth Number *
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Hash :size="20" class="text-slate-400" />
          </div>
          <input
            id="berthNumber"
            v-model="form.berthNumber"
            type="text"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., B-001"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.berthNumber }"
          />
        </div>
        <p v-if="errors.berthNumber" class="mt-1 text-sm text-red-600">{{ errors.berthNumber }}</p>
      </div>

      <!-- Port Assignment -->
      <div>
        <label for="portId" class="block text-sm font-medium text-slate-700 mb-2">
          Port *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <MapPin :size="20" class="text-slate-400" />
          </div>
          <select
            id="portId"
            v-model="form.portId"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.portId }"
          >
            <option value="">Select port</option>
            <option v-for="port in availablePorts" :key="port.id" :value="port.id">
              {{ port.name }} ({{ port.code }})
            </option>
          </select>
        </div>
        <p v-if="errors.portId" class="mt-1 text-sm text-red-600">{{ errors.portId }}</p>
      </div>

      <!-- Berth Type -->
      <div>
        <label for="type" class="block text-sm font-medium text-slate-700 mb-2">
          Berth Type *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Settings :size="20" class="text-slate-400" />
          </div>
          <select
            id="type"
            v-model="form.type"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.type }"
          >
            <option value="">Select berth type</option>
            <option value="Container">Container Berth</option>
            <option value="Bulk">Bulk Cargo Berth</option>
            <option value="Tanker">Tanker Berth</option>
            <option value="RoRo">Roll-on/Roll-off Berth</option>
            <option value="Multipurpose">Multipurpose Berth</option>
          </select>
        </div>
        <p v-if="errors.type" class="mt-1 text-sm text-red-600">{{ errors.type }}</p>
      </div>

      <!-- Dimensions -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <!-- Length -->
        <div>
          <label for="length" class="block text-sm font-medium text-slate-700 mb-2">
            Length (m) *
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Ruler :size="20" class="text-slate-400" />
            </div>
            <input
              id="length"
              v-model.number="form.length"
              type="number"
              min="1"
              step="0.1"
              required
              class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="e.g., 400"
              :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.length }"
            />
          </div>
          <p v-if="errors.length" class="mt-1 text-sm text-red-600">{{ errors.length }}</p>
        </div>

        <!-- Width -->
        <div>
          <label for="width" class="block text-sm font-medium text-slate-700 mb-2">
            Width (m) *
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Ruler :size="20" class="text-slate-400" />
            </div>
            <input
              id="width"
              v-model.number="form.width"
              type="number"
              min="1"
              step="0.1"
              required
              class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="e.g., 50"
              :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.width }"
            />
          </div>
          <p v-if="errors.width" class="mt-1 text-sm text-red-600">{{ errors.width }}</p>
        </div>
      </div>

      <!-- Depth -->
      <div>
        <label for="depth" class="block text-sm font-medium text-slate-700 mb-2">
          Water Depth (m) *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Waves :size="20" class="text-slate-400" />
          </div>
          <input
            id="depth"
            v-model.number="form.depth"
            type="number"
            min="1"
            step="0.1"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., 16.5"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.depth }"
          />
        </div>
        <p v-if="errors.depth" class="mt-1 text-sm text-red-600">{{ errors.depth }}</p>
      </div>

      <!-- Max Ship Size -->
      <div>
        <label for="maxShipSize" class="block text-sm font-medium text-slate-700 mb-2">
          Maximum Ship Size (TEU)
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Ship :size="20" class="text-slate-400" />
          </div>
          <input
            id="maxShipSize"
            v-model.number="form.maxShipSize"
            type="number"
            min="1"
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., 24000"
          />
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
            <option value="Available">Available</option>
            <option value="Occupied">Occupied</option>
            <option value="Reserved">Reserved</option>
            <option value="Maintenance">Under Maintenance</option>
            <option value="OutOfService">Out of Service</option>
          </select>
        </div>
        <p v-if="errors.status" class="mt-1 text-sm text-red-600">{{ errors.status }}</p>
      </div>

      <!-- Equipment -->
      <div>
        <label class="block text-sm font-medium text-slate-700 mb-2">
          Available Equipment
        </label>
        <div class="grid grid-cols-2 gap-4">
          <label
            v-for="equipment in availableEquipment"
            :key="equipment.id"
            class="flex items-center p-3 border rounded-lg cursor-pointer transition-all"
            :class="{
              'border-blue-500 bg-blue-50': form.equipment.includes(equipment.id),
              'border-slate-300 hover:border-slate-400': !form.equipment.includes(equipment.id)
            }"
          >
            <input
              type="checkbox"
              :value="equipment.id"
              v-model="form.equipment"
              class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-slate-300 rounded"
            />
            <div class="ml-3">
              <div class="font-medium text-slate-900">{{ equipment.name }}</div>
              <div class="text-sm text-slate-600">{{ equipment.description }}</div>
            </div>
          </label>
        </div>
      </div>

      <!-- Operating Hours -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label for="operatingHoursStart" class="block text-sm font-medium text-slate-700 mb-2">
            Operating Hours Start
          </label>
          <input
            id="operatingHoursStart"
            v-model="form.operatingHoursStart"
            type="time"
            class="w-full px-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
          />
        </div>

        <div>
          <label for="operatingHoursEnd" class="block text-sm font-medium text-slate-700 mb-2">
            Operating Hours End
          </label>
          <input
            id="operatingHoursEnd"
            v-model="form.operatingHoursEnd"
            type="time"
            class="w-full px-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
          />
        </div>
      </div>

      <!-- Current Occupancy -->
      <div>
        <label class="block text-sm font-medium text-slate-700 mb-2">
          Current Occupancy
        </label>
        <div class="bg-slate-50 rounded-lg p-4">
          <div class="flex items-center justify-between mb-2">
            <span class="text-sm text-slate-600">Ship at berth:</span>
            <span class="font-semibold text-slate-900">
              {{ form.currentShip ? form.currentShip : 'None' }}
            </span>
          </div>
          <div class="flex items-center gap-2">
            <div class="w-3 h-3 rounded-full" :class="{
              'bg-green-500': form.status === 'Available',
              'bg-red-500': form.status === 'Occupied',
              'bg-yellow-500': form.status === 'Reserved',
              'bg-gray-500': form.status === 'Maintenance' || form.status === 'OutOfService'
            }"></div>
            <span class="text-sm text-slate-600">{{ form.status || 'Unknown' }}</span>
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
          placeholder="Additional notes about the berth..."
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
          {{ isLoading ? 'Saving...' : (isEditing ? 'Update Berth' : 'Create Berth') }}
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
import { ref, reactive, onMounted } from 'vue'
import { 
  Anchor, Hash, MapPin, Settings, Ruler, Waves, Ship, Activity, 
  Save, Loader2, AlertTriangle, CheckCircle, ArrowLeft 
} from 'lucide-vue-next'

interface BerthForm {
  id?: number
  berthNumber: string
  portId?: number
  type: string
  length: number
  width: number
  depth: number
  maxShipSize?: number
  status: string
  equipment: string[]
  operatingHoursStart?: string
  operatingHoursEnd?: string
  currentShip?: string
  notes?: string
}

interface Port {
  id: number
  name: string
  code: string
}

interface Equipment {
  id: string
  name: string
  description: string
}

const props = defineProps<{
  berth?: BerthForm
  isEditing?: boolean
}>()

const emit = defineEmits(['submit', 'cancel'])

const form = reactive<BerthForm>({
  berthNumber: '',
  portId: undefined,
  type: '',
  length: 0,
  width: 0,
  depth: 0,
  maxShipSize: undefined,
  status: '',
  equipment: [],
  operatingHoursStart: '00:00',
  operatingHoursEnd: '23:59',
  currentShip: '',
  notes: ''
})

const errors = reactive<Partial<Record<keyof BerthForm, string>>>({})
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const availablePorts = ref<Port[]>([])
const availableEquipment = ref<Equipment[]>([])

const validate = (): boolean => {
  Object.keys(errors).forEach(key => delete errors[key as keyof BerthForm])
  
  if (!form.berthNumber.trim()) {
    errors.berthNumber = 'Berth number is required'
    return false
  }
  
  if (!form.portId) {
    errors.portId = 'Port assignment is required'
    return false
  }
  
  if (!form.type) {
    errors.type = 'Berth type is required'
    return false
  }
  
  if (!form.length || form.length <= 0) {
    errors.length = 'Length must be greater than 0'
    return false
  }
  
  if (!form.width || form.width <= 0) {
    errors.width = 'Width must be greater than 0'
    return false
  }
  
  if (!form.depth || form.depth <= 0) {
    errors.depth = 'Depth must be greater than 0'
    return false
  }
  
  if (!form.status) {
    errors.status = 'Status is required'
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

const loadAvailableEquipment = async () => {
  try {
    // Mock API call to load equipment
    availableEquipment.value = [
      { id: 'crane', name: 'Container Cranes', description: 'Ship-to-shore container cranes' },
      { id: 'rtg', name: 'RTG Cranes', description: 'Rubber-tired gantry cranes' },
      { id: 'reach', name: 'Reach Stackers', description: 'Mobile container handling equipment' },
      { id: 'forklift', name: 'Forklifts', description: 'Various capacity forklifts' },
      { id: 'mooring', name: 'Mooring Equipment', description: 'Bollards and mooring systems' },
      { id: 'lighting', name: 'Floodlighting', description: '24/7 operational lighting' },
      { id: 'power', name: 'Shore Power', description: 'Electrical power connection' },
      { id: 'water', name: 'Fresh Water', description: 'Fresh water supply' }
    ]
  } catch (error) {
    console.error('Failed to load equipment:', error)
  }
}

const handleSubmit = async () => {
  if (!validate()) return
  
  isLoading.value = true
  errorMessage.value = ''
  successMessage.value = ''
  
  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1500))
    
    const berthData = {
      ...form,
      id: props.berth?.id || Date.now()
    }
    
    successMessage.value = props.isEditing 
      ? 'Berth updated successfully!' 
      : 'Berth created successfully!'
    
    setTimeout(() => {
      emit('submit', berthData)
    }, 1000)
    
  } catch (error) {
    errorMessage.value = props.isEditing 
      ? 'Failed to update berth. Please try again.' 
      : 'Failed to create berth. Please try again.'
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  if (props.berth) {
    Object.assign(form, props.berth)
  }
  loadAvailablePorts()
  loadAvailableEquipment()
})
</script>