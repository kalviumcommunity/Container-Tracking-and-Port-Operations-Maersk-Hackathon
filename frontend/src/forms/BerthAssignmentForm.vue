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
        <div class="p-2 bg-green-600 rounded-lg shadow-lg flex-shrink-0">
          <Ship :size="18" class="text-white sm:w-5 sm:h-5" />
        </div>
        <div class="min-w-0">
          <h2 class="text-lg sm:text-xl font-bold text-slate-900 truncate">
            {{ isEditing ? 'Edit Berth Assignment' : 'New Berth Assignment' }}
          </h2>
          <p class="text-xs sm:text-sm text-slate-600 hidden sm:block">Assign ships to berths with scheduling</p>
        </div>
      </div>
    </div>

    <!-- Form Content -->
    <div class="overflow-y-auto max-h-[calc(95vh-64px)] sm:max-h-[calc(95vh-80px)]">
      <form @submit.prevent="handleSubmit" class="p-4 sm:p-6 space-y-4 sm:space-y-6">
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
            <option v-for="container in availableContainers" :key="container.id" :value="container.id">
              {{ container.containerNumber }} - {{ container.type }} ({{ container.weight }}kg)
            </option>
          </select>
        </div>
        <p v-if="errors.containerId" class="mt-1 text-sm text-red-600">{{ errors.containerId }}</p>
      </div>

      <!-- Berth Selection -->
      <div>
        <label for="berthId" class="block text-sm font-medium text-slate-700 mb-2">
          Berth *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Anchor :size="20" class="text-slate-400" />
          </div>
          <select
            id="berthId"
            v-model="form.berthId"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.berthId }"
          >
            <option value="">Select berth</option>
            <option 
              v-for="berth in availableBerths" 
              :key="berth.id" 
              :value="berth.id"
              :disabled="berth.status !== 'Available'"
            >
              {{ berth.berthNumber }} - {{ berth.port }} 
              <span v-if="berth.status !== 'Available'">({{ berth.status }})</span>
            </option>
          </select>
        </div>
        <p v-if="errors.berthId" class="mt-1 text-sm text-red-600">{{ errors.berthId }}</p>
      </div>

      <!-- Assignment Type -->
      <div>
        <label for="assignmentType" class="block text-sm font-medium text-slate-700 mb-2">
          Assignment Type *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Settings :size="20" class="text-slate-400" />
          </div>
          <select
            id="assignmentType"
            v-model="form.assignmentType"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.assignmentType }"
          >
            <option value="">Select assignment type</option>
            <option value="Loading">Loading</option>
            <option value="Unloading">Unloading</option>
            <option value="Storage">Storage</option>
            <option value="Transit">Transit</option>
          </select>
        </div>
        <p v-if="errors.assignmentType" class="mt-1 text-sm text-red-600">{{ errors.assignmentType }}</p>
      </div>

      <!-- Priority -->
      <div>
        <label for="priority" class="block text-sm font-medium text-slate-700 mb-2">
          Priority *
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Flag :size="20" class="text-slate-400" />
          </div>
          <select
            id="priority"
            v-model="form.priority"
            required
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.priority }"
          >
            <option value="">Select priority</option>
            <option value="Low">Low</option>
            <option value="Normal">Normal</option>
            <option value="High">High</option>
            <option value="Critical">Critical</option>
          </select>
        </div>
        <p v-if="errors.priority" class="mt-1 text-sm text-red-600">{{ errors.priority }}</p>
      </div>

      <!-- Scheduled Times -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <!-- Start Time -->
        <div>
          <label for="startTime" class="block text-sm font-medium text-slate-700 mb-2">
            Scheduled Start Time *
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Calendar :size="20" class="text-slate-400" />
            </div>
            <input
              id="startTime"
              v-model="form.startTime"
              type="datetime-local"
              required
              class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.startTime }"
            />
          </div>
          <p v-if="errors.startTime" class="mt-1 text-sm text-red-600">{{ errors.startTime }}</p>
        </div>

        <!-- End Time -->
        <div>
          <label for="endTime" class="block text-sm font-medium text-slate-700 mb-2">
            Scheduled End Time *
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Calendar :size="20" class="text-slate-400" />
            </div>
            <input
              id="endTime"
              v-model="form.endTime"
              type="datetime-local"
              required
              class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.endTime }"
            />
          </div>
          <p v-if="errors.endTime" class="mt-1 text-sm text-red-600">{{ errors.endTime }}</p>
        </div>
      </div>

      <!-- Position in Berth -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <!-- Row -->
        <div>
          <label for="positionRow" class="block text-sm font-medium text-slate-700 mb-2">
            Position Row
          </label>
          <input
            id="positionRow"
            v-model="form.positionRow"
            type="text"
            class="w-full px-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., A"
          />
        </div>

        <!-- Bay -->
        <div>
          <label for="positionBay" class="block text-sm font-medium text-slate-700 mb-2">
            Position Bay
          </label>
          <input
            id="positionBay"
            v-model.number="form.positionBay"
            type="number"
            min="1"
            class="w-full px-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., 12"
          />
        </div>

        <!-- Tier -->
        <div>
          <label for="positionTier" class="block text-sm font-medium text-slate-700 mb-2">
            Position Tier
          </label>
          <input
            id="positionTier"
            v-model.number="form.positionTier"
            type="number"
            min="1"
            class="w-full px-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            placeholder="e.g., 3"
          />
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
            <option value="Scheduled">Scheduled</option>
            <option value="InProgress">In Progress</option>
            <option value="Completed">Completed</option>
            <option value="Cancelled">Cancelled</option>
            <option value="OnHold">On Hold</option>
          </select>
        </div>
        <p v-if="errors.status" class="mt-1 text-sm text-red-600">{{ errors.status }}</p>
      </div>

      <!-- Assigned Crew -->
      <div>
        <label for="assignedCrew" class="block text-sm font-medium text-slate-700 mb-2">
          Assigned Crew
        </label>
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <Users :size="20" class="text-slate-400" />
          </div>
          <select
            id="assignedCrew"
            v-model="form.assignedCrew"
            multiple
            class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            size="4"
          >
            <option v-for="crew in availableCrew" :key="crew.id" :value="crew.id">
              {{ crew.name }} - {{ crew.role }}
            </option>
          </select>
        </div>
        <p class="mt-1 text-sm text-slate-500">Hold Ctrl/Cmd to select multiple crew members</p>
      </div>

      <!-- Special Instructions -->
      <div>
        <label for="specialInstructions" class="block text-sm font-medium text-slate-700 mb-2">
          Special Instructions
        </label>
        <textarea
          id="specialInstructions"
          v-model="form.specialInstructions"
          rows="3"
          class="w-full px-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors resize-none"
          placeholder="Any special handling instructions or notes..."
        ></textarea>
      </div>

      <!-- Assignment Summary -->
      <div v-if="form.containerId && form.berthId" class="bg-slate-50 rounded-lg p-4">
        <h3 class="font-medium text-slate-900 mb-3">Assignment Summary</h3>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
          <div>
            <span class="text-slate-600">Container:</span>
            <span class="font-medium text-slate-900 ml-2">
              {{ selectedContainer?.containerNumber }}
            </span>
          </div>
          <div>
            <span class="text-slate-600">Berth:</span>
            <span class="font-medium text-slate-900 ml-2">
              {{ selectedBerth?.berthNumber }}
            </span>
          </div>
          <div>
            <span class="text-slate-600">Duration:</span>
            <span class="font-medium text-slate-900 ml-2">
              {{ calculateDuration() }}
            </span>
          </div>
          <div>
            <span class="text-slate-600">Priority:</span>
            <span class="font-medium ml-2" :class="{
              'text-red-600': form.priority === 'Critical',
              'text-orange-600': form.priority === 'High',
              'text-blue-600': form.priority === 'Normal',
              'text-slate-600': form.priority === 'Low'
            }">
              {{ form.priority }}
            </span>
          </div>
        </div>
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
import { ref, reactive, computed, onMounted } from 'vue'
import { 
  Link, Package, Anchor, Settings, Flag, Calendar, Activity, Users,
  Save, Loader2, AlertTriangle, CheckCircle, ArrowLeft 
} from 'lucide-vue-next'
import { containerApi } from '../services/api'

interface BerthAssignmentForm {
  id?: number
  containerId?: number
  berthId?: number
  assignmentType: string
  priority: string
  startTime: string
  endTime: string
  positionRow?: string
  positionBay?: number
  positionTier?: number
  status: string
  assignedCrew: number[]
  specialInstructions?: string
}

interface Container {
  id: number
  containerNumber: string
  type: string
  weight: number
}

interface Berth {
  id: number
  berthNumber: string
  port: string
  status: string
}

interface Crew {
  id: number
  name: string
  role: string
}

const props = defineProps<{
  assignment?: BerthAssignmentForm
  isEditing?: boolean
}>()

const emit = defineEmits(['submit', 'cancel'])

const form = reactive<BerthAssignmentForm>({
  containerId: undefined,
  berthId: undefined,
  assignmentType: '',
  priority: '',
  startTime: '',
  endTime: '',
  positionRow: '',
  positionBay: undefined,
  positionTier: undefined,
  status: '',
  assignedCrew: [],
  specialInstructions: ''
})

const errors = reactive<Partial<Record<keyof BerthAssignmentForm, string>>>({})
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const availableContainers = ref<Container[]>([])
const availableBerths = ref<Berth[]>([])
const availableCrew = ref<Crew[]>([])

const selectedContainer = computed(() => 
  availableContainers.value.find(c => c.id === form.containerId)
)

const selectedBerth = computed(() => 
  availableBerths.value.find(b => b.id === form.berthId)
)

const calculateDuration = (): string => {
  if (!form.startTime || !form.endTime) return 'N/A'
  
  const start = new Date(form.startTime)
  const end = new Date(form.endTime)
  const diffMs = end.getTime() - start.getTime()
  const diffHours = Math.round(diffMs / (1000 * 60 * 60))
  
  if (diffHours < 24) {
    return `${diffHours} hour${diffHours !== 1 ? 's' : ''}`
  } else {
    const days = Math.floor(diffHours / 24)
    const hours = diffHours % 24
    return `${days} day${days !== 1 ? 's' : ''} ${hours > 0 ? `${hours} hour${hours !== 1 ? 's' : ''}` : ''}`
  }
}

const validate = (): boolean => {
  Object.keys(errors).forEach(key => delete errors[key as keyof BerthAssignmentForm])
  
  if (!form.containerId) {
    errors.containerId = 'Container selection is required'
    return false
  }
  
  if (!form.berthId) {
    errors.berthId = 'Berth selection is required'
    return false
  }
  
  if (!form.assignmentType) {
    errors.assignmentType = 'Assignment type is required'
    return false
  }
  
  if (!form.priority) {
    errors.priority = 'Priority is required'
    return false
  }
  
  if (!form.startTime) {
    errors.startTime = 'Start time is required'
    return false
  }
  
  if (!form.endTime) {
    errors.endTime = 'End time is required'
    return false
  }
  
  if (form.startTime && form.endTime) {
    const start = new Date(form.startTime)
    const end = new Date(form.endTime)
    if (end <= start) {
      errors.endTime = 'End time must be after start time'
      return false
    }
  }
  
  if (!form.status) {
    errors.status = 'Status is required'
    return false
  }
  
  return true
}

const loadAvailableContainers = async () => {
  try {
    // Load containers from the backend API
    const containers = await containerApi.getContainers()
    
    // Map the API response to the expected format
    availableContainers.value = containers.map(container => ({
      id: container.id,
      containerNumber: container.containerNumber,
      type: container.type,
      weight: container.weight || 0
    }))
    
    
  } catch (error) {
    console.error('Failed to load containers from API:', error)
    
    // Fallback to mock data if API fails
    availableContainers.value = [
      { id: 1, containerNumber: 'MSCU1234567', type: 'Dry', weight: 25000 },
      { id: 2, containerNumber: 'MSCU2345678', type: 'Refrigerated', weight: 28000 },
      { id: 3, containerNumber: 'MSCU3456789', type: 'Tank', weight: 30000 },
      { id: 4, containerNumber: 'MSCU4567890', type: 'OpenTop', weight: 22000 }
    ]
    
    errorMessage.value = 'Could not load containers from server, using sample data'
  }
}

const loadAvailableBerths = async () => {
  try {
    // Mock API call
    availableBerths.value = [
      { id: 1, berthNumber: 'B-001', port: 'Port of Shanghai', status: 'Available' },
      { id: 2, berthNumber: 'B-002', port: 'Port of Shanghai', status: 'Occupied' },
      { id: 3, berthNumber: 'B-003', port: 'Port of Singapore', status: 'Available' },
      { id: 4, berthNumber: 'B-004', port: 'Port of Singapore', status: 'Maintenance' }
    ]
  } catch (error) {
    console.error('Failed to load berths:', error)
  }
}

const loadAvailableCrew = async () => {
  try {
    // Mock API call
    availableCrew.value = [
      { id: 1, name: 'John Smith', role: 'Crane Operator' },
      { id: 2, name: 'Maria Garcia', role: 'Stevedore' },
      { id: 3, name: 'David Chen', role: 'Supervisor' },
      { id: 4, name: 'Sarah Johnson', role: 'Quality Inspector' },
      { id: 5, name: 'Ahmed Ali', role: 'Safety Officer' }
    ]
  } catch (error) {
    console.error('Failed to load crew:', error)
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
    
    const assignmentData = {
      ...form,
      id: props.assignment?.id || Date.now()
    }
    
    successMessage.value = props.isEditing 
      ? 'Assignment updated successfully!' 
      : 'Assignment created successfully!'
    
    setTimeout(() => {
      emit('submit', assignmentData)
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
  loadAvailableContainers()
  loadAvailableBerths()
  loadAvailableCrew()
})
</script>