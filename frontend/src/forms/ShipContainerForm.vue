<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center gap-4 mb-6">
      <div class="p-3 bg-blue-600 rounded-lg shadow-lg">
        <Link2 :size="24" class="text-white" />
      </div>
      <div>
        <h2 class="text-2xl font-bold text-slate-900">
          {{ isEditing ? 'Edit Ship-Container Assignment' : 'Assign Container to Ship' }}
        </h2>
        <p class="text-slate-600">Manage container loading on ships</p>
      </div>
    </div>

    <!-- Assignment Form -->
    <form @submit.prevent="handleSubmit" class="bg-white rounded-xl shadow-lg border border-slate-200 p-6 space-y-6">
      <!-- Ship Selection -->
      <div>
        <label for="shipId" class="block text-sm font-medium text-slate-700 mb-2">
          Ship *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Ship :size="20" class="text-slate-400" />
          </div>
          <select
            id="shipId"
            v-model="form.shipId"
            required
            @change="onShipChange"
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.shipId }"
          >
            <option value="">Select ship</option>
            <option v-for="ship in availableShips" :key="ship.id" :value="ship.id">
              {{ ship.name }} ({{ ship.currentContainers }}/{{ ship.capacity }} TEU)
            </option>
          </select>
        </div>
        <p v-if="errors.shipId" class="mt-1 text-sm text-red-600">{{ errors.shipId }}</p>
      </div>

      <!-- Container Selection -->
      <div>
        <label for="containerId" class="block text-sm font-medium text-slate-700 mb-2">
          Container *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Package :size="20" class="text-slate-400" />
          </div>
          <select
            id="containerId"
            v-model="form.containerId"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.containerId }"
          >
            <option value="">Select container</option>
            <option 
              v-for="container in availableContainers" 
              :key="container.id" 
              :value="container.id"
              :disabled="!!(container.shipId && container.shipId !== form.shipId)"
            >
              {{ container.containerNumber }} - {{ container.type }} ({{ container.weight }}kg)
              <span v-if="container.shipId && container.shipId !== form.shipId">(Already assigned)</span>
            </option>
          </select>
        </div>
        <p v-if="errors.containerId" class="mt-1 text-sm text-red-600">{{ errors.containerId }}</p>
      </div>

      <!-- Position on Ship -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <!-- Bay -->
        <div>
          <label for="bay" class="block text-sm font-medium text-slate-700 mb-2">
            Bay *
          </label>
          <input
            id="bay"
            v-model.number="form.bay"
            type="number"
            min="1"
            required
            class="w-full px-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., 12"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.bay }"
          />
          <p v-if="errors.bay" class="mt-1 text-sm text-red-600">{{ errors.bay }}</p>
        </div>

        <!-- Row -->
        <div>
          <label for="row" class="block text-sm font-medium text-slate-700 mb-2">
            Row *
          </label>
          <input
            id="row"
            v-model.number="form.row"
            type="number"
            min="1"
            required
            class="w-full px-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., 6"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.row }"
          />
          <p v-if="errors.row" class="mt-1 text-sm text-red-600">{{ errors.row }}</p>
        </div>

        <!-- Tier -->
        <div>
          <label for="tier" class="block text-sm font-medium text-slate-700 mb-2">
            Tier *
          </label>
          <input
            id="tier"
            v-model.number="form.tier"
            type="number"
            min="1"
            required
            class="w-full px-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., 3"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.tier }"
          />
          <p v-if="errors.tier" class="mt-1 text-sm text-red-600">{{ errors.tier }}</p>
        </div>

        <!-- Slot -->
        <div>
          <label for="slot" class="block text-sm font-medium text-slate-700 mb-2">
            Slot
          </label>
          <input
            id="slot"
            v-model="form.slot"
            type="text"
            class="w-full px-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., A"
          />
        </div>
      </div>

      <!-- Loading/Unloading Information -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <!-- Loading Port -->
        <div>
          <label for="loadingPortId" class="block text-sm font-medium text-slate-700 mb-2">
            Loading Port
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <MapPin :size="20" class="text-slate-400" />
            </div>
            <select
              id="loadingPortId"
              v-model="form.loadingPortId"
              class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            >
              <option value="">Select loading port</option>
              <option v-for="port in availablePorts" :key="port.id" :value="port.id">
                {{ port.name }} ({{ port.code }})
              </option>
            </select>
          </div>
        </div>

        <!-- Unloading Port -->
        <div>
          <label for="unloadingPortId" class="block text-sm font-medium text-slate-700 mb-2">
            Unloading Port
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <MapPin :size="20" class="text-slate-400" />
            </div>
            <select
              id="unloadingPortId"
              v-model="form.unloadingPortId"
              class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            >
              <option value="">Select unloading port</option>
              <option v-for="port in availablePorts" :key="port.id" :value="port.id">
                {{ port.name }} ({{ port.code }})
              </option>
            </select>
          </div>
        </div>
      </div>

      <!-- Dates -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <!-- Loading Date -->
        <div>
          <label for="loadingDate" class="block text-sm font-medium text-slate-700 mb-2">
            Loading Date
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Calendar :size="20" class="text-slate-400" />
            </div>
            <input
              id="loadingDate"
              v-model="form.loadingDate"
              type="datetime-local"
              class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            />
          </div>
        </div>

        <!-- Unloading Date -->
        <div>
          <label for="unloadingDate" class="block text-sm font-medium text-slate-700 mb-2">
            Expected Unloading Date
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Calendar :size="20" class="text-slate-400" />
            </div>
            <input
              id="unloadingDate"
              v-model="form.unloadingDate"
              type="datetime-local"
              class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            />
          </div>
        </div>
      </div>

      <!-- Status -->
      <div>
        <label for="status" class="block text-sm font-medium text-slate-700 mb-2">
          Assignment Status *
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
            <option value="Planned">Planned</option>
            <option value="Loading">Loading</option>
            <option value="Loaded">Loaded</option>
            <option value="InTransit">In Transit</option>
            <option value="Unloading">Unloading</option>
            <option value="Unloaded">Unloaded</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>
        <p v-if="errors.status" class="mt-1 text-sm text-red-600">{{ errors.status }}</p>
      </div>

      <!-- Special Handling -->
      <div>
        <label class="block text-sm font-medium text-slate-700 mb-2">
          Special Handling Requirements
        </label>
        <div class="grid grid-cols-2 gap-4">
          <label
            v-for="requirement in specialRequirements"
            :key="requirement.id"
            class="flex items-center p-3 border rounded-lg cursor-pointer transition-all"
            :class="{
              'border-blue-500 bg-blue-50': form.specialHandling.includes(requirement.id),
              'border-slate-300 hover:border-slate-400': !form.specialHandling.includes(requirement.id)
            }"
          >
            <input
              type="checkbox"
              :value="requirement.id"
              v-model="form.specialHandling"
              class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-slate-300 rounded"
            />
            <div class="ml-3">
              <div class="font-medium text-slate-900">{{ requirement.name }}</div>
              <div class="text-sm text-slate-600">{{ requirement.description }}</div>
            </div>
          </label>
        </div>
      </div>

      <!-- Ship Capacity Status -->
      <div v-if="selectedShip" class="bg-slate-50 rounded-lg p-4">
        <h3 class="font-medium text-slate-900 mb-3">Ship Capacity Status</h3>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm">
          <div>
            <span class="text-slate-600">Current Load:</span>
            <span class="font-medium text-slate-900 ml-2">
              {{ selectedShip.currentContainers }} containers
            </span>
          </div>
          <div>
            <span class="text-slate-600">Total Capacity:</span>
            <span class="font-medium text-slate-900 ml-2">
              {{ selectedShip.capacity }} TEU
            </span>
          </div>
          <div>
            <span class="text-slate-600">Utilization:</span>
            <span class="font-medium text-slate-900 ml-2">
              {{ Math.round((selectedShip.currentContainers / selectedShip.capacity) * 100) }}%
            </span>
          </div>
        </div>
        <div class="mt-3">
          <div class="w-full bg-slate-200 rounded-full h-2">
            <div 
              class="h-2 rounded-full transition-all"
              :class="{
                'bg-green-500': (selectedShip.currentContainers / selectedShip.capacity) < 0.8,
                'bg-yellow-500': (selectedShip.currentContainers / selectedShip.capacity) >= 0.8 && (selectedShip.currentContainers / selectedShip.capacity) < 0.95,
                'bg-red-500': (selectedShip.currentContainers / selectedShip.capacity) >= 0.95
              }"
              :style="{ width: `${Math.min(((selectedShip.currentContainers || 0) / (selectedShip.capacity || 1)) * 100, 100)}%` }"
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
          placeholder="Additional notes about this assignment..."
        ></textarea>
      </div>

      <!-- Actions -->
      <div class="flex gap-4 pt-4 border-t border-slate-200">
        <button
          type="submit"
          :disabled="isLoading || isShipAtCapacity"
          class="flex-1 flex justify-center items-center gap-2 py-3 px-4 border border-transparent rounded-lg shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          <Loader2 v-if="isLoading" :size="20" class="animate-spin" />
          <Save v-else :size="20" />
          {{ isLoading ? 'Saving...' : (isEditing ? 'Update Assignment' : 'Create Assignment') }}
        </button>
        
        <button
          type="button"
          @click="$emit('cancel')"
          class="px-6 py-3 border border-slate-300 rounded-lg text-slate-700 hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors"
        >
          Cancel
        </button>
      </div>

      <div v-if="isShipAtCapacity" class="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
        <div class="flex items-center gap-2">
          <AlertTriangle :size="20" class="text-yellow-600" />
          <p class="text-yellow-800">Warning: Selected ship is at or near capacity</p>
        </div>
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
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { 
  Link2, Ship, Package, MapPin, Calendar, Activity,
  Save, Loader2, AlertTriangle, CheckCircle 
} from 'lucide-vue-next'

interface ShipContainerForm {
  id?: number
  shipId?: number
  containerId?: number
  bay: number
  row: number
  tier: number
  slot?: string
  loadingPortId?: number
  unloadingPortId?: number
  loadingDate?: string
  unloadingDate?: string
  status: string
  specialHandling: string[]
  notes?: string
}

interface ShipData {
  id: number
  name: string
  capacity: number
  currentContainers: number
}

interface Container {
  id: number
  containerNumber: string
  type: string
  weight: number
  shipId?: number
}

interface Port {
  id: number
  name: string
  code: string
}

interface SpecialRequirement {
  id: string
  name: string
  description: string
}

const props = defineProps<{
  assignment?: ShipContainerForm
  isEditing?: boolean
}>()

const emit = defineEmits(['submit', 'cancel'])

const form = reactive<ShipContainerForm>({
  shipId: undefined,
  containerId: undefined,
  bay: 0,
  row: 0,
  tier: 0,
  slot: '',
  loadingPortId: undefined,
  unloadingPortId: undefined,
  loadingDate: '',
  unloadingDate: '',
  status: '',
  specialHandling: [],
  notes: ''
})

const errors = reactive<Partial<Record<keyof ShipContainerForm, string>>>({})
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const availableShips = ref<ShipData[]>([])
const availableContainers = ref<Container[]>([])
const availablePorts = ref<Port[]>([])

const specialRequirements = ref<SpecialRequirement[]>([
  { id: 'fragile', name: 'Fragile Goods', description: 'Handle with extra care' },
  { id: 'hazmat', name: 'Hazardous Materials', description: 'Special safety protocols required' },
  { id: 'refrigerated', name: 'Temperature Controlled', description: 'Maintain cold chain' },
  { id: 'oversized', name: 'Oversized Cargo', description: 'Requires special positioning' },
  { id: 'heavy', name: 'Heavy Cargo', description: 'Weight distribution considerations' },
  { id: 'priority', name: 'Priority Handling', description: 'Expedited processing required' }
])

const selectedShip = computed(() => 
  availableShips.value.find(s => s.id === form.shipId)
)

const isShipAtCapacity = computed(() => {
  if (!selectedShip.value) return false
  return (selectedShip.value.currentContainers / selectedShip.value.capacity) >= 0.95
})

const validate = (): boolean => {
  Object.keys(errors).forEach(key => delete errors[key as keyof ShipContainerForm])
  
  if (!form.shipId) {
    errors.shipId = 'Ship selection is required'
    return false
  }
  
  if (!form.containerId) {
    errors.containerId = 'Container selection is required'
    return false
  }
  
  if (!form.bay || form.bay <= 0) {
    errors.bay = 'Bay number is required and must be positive'
    return false
  }
  
  if (!form.row || form.row <= 0) {
    errors.row = 'Row number is required and must be positive'
    return false
  }
  
  if (!form.tier || form.tier <= 0) {
    errors.tier = 'Tier number is required and must be positive'
    return false
  }
  
  if (!form.status) {
    errors.status = 'Status is required'
    return false
  }
  
  return true
}

const onShipChange = () => {
  // Reset position when ship changes
  form.bay = 0
  form.row = 0
  form.tier = 0
  form.slot = ''
}

const loadAvailableShips = async () => {
  try {
    // Mock API call
    availableShips.value = [
      { id: 1, name: 'MSC Gülsün', capacity: 23756, currentContainers: 18500 },
      { id: 2, name: 'Ever Ace', capacity: 23992, currentContainers: 15200 },
      { id: 3, name: 'OOCL Hong Kong', capacity: 21413, currentContainers: 12800 },
      { id: 4, name: 'Madrid Maersk', capacity: 20568, currentContainers: 19800 }
    ]
  } catch (error) {
    console.error('Failed to load ships:', error)
  }
}

const loadAvailableContainers = async () => {
  try {
    // Mock API call
    availableContainers.value = [
      { id: 1, containerNumber: 'MSCU1234567', type: 'Dry', weight: 25000 },
      { id: 2, containerNumber: 'MSCU2345678', type: 'Refrigerated', weight: 28000, shipId: 2 },
      { id: 3, containerNumber: 'MSCU3456789', type: 'Tank', weight: 30000 },
      { id: 4, containerNumber: 'MSCU4567890', type: 'OpenTop', weight: 22000 },
      { id: 5, containerNumber: 'MSCU5678901', type: 'Dry', weight: 24500, shipId: 1 }
    ]
  } catch (error) {
    console.error('Failed to load containers:', error)
  }
}

const loadAvailablePorts = async () => {
  try {
    // Mock API call
    availablePorts.value = [
      { id: 1, name: 'Port of Shanghai', code: 'CNSHA' },
      { id: 2, name: 'Port of Singapore', code: 'SGSIN' },
      { id: 3, name: 'Port of Ningbo-Zhoushan', code: 'CNNGB' },
      { id: 4, name: 'Port of Shenzhen', code: 'CNSZN' },
      { id: 5, name: 'Port of Guangzhou', code: 'CNGZH' },
      { id: 6, name: 'Port of Busan', code: 'KRPUS' }
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
    // Import shipContainerApi dynamically
    const { shipContainerApi } = await import('../services/shipContainerApi')
    
    const assignmentData = {
      shipId: Number(form.shipId),
      containerId: form.containerId?.toString() || '',
      loadedAt: form.loadingDate || null
    }
    
    let result
    if (props.isEditing && props.assignment?.id) {
      // Update existing assignment
      result = await shipContainerApi.update(props.assignment.id, assignmentData)
    } else {
      // Create new assignment
      result = await shipContainerApi.create(assignmentData)
    }
    
    successMessage.value = props.isEditing 
      ? 'Ship-container assignment updated successfully!' 
      : 'Container assigned to ship successfully!'
    
    setTimeout(() => {
      emit('submit', result.data)
    }, 1000)
    
  } catch (error) {
    errorMessage.value = props.isEditing 
      ? 'Failed to update assignment. Please try again.' 
      : 'Failed to create assignment. Please try again.'
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  if (props.assignment) {
    Object.assign(form, props.assignment)
  }
  loadAvailableShips()
  loadAvailableContainers()
  loadAvailablePorts()
})
</script>
