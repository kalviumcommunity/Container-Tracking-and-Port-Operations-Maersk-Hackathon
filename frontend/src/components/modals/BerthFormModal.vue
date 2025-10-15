<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="isOpen" class="fixed inset-0 z-50 overflow-y-auto">
        <!-- Backdrop -->
        <div 
          class="fixed inset-0 bg-black/50 backdrop-blur-sm transition-opacity"
          @click="handleClose"
        ></div>

        <!-- Modal Container -->
        <div class="flex min-h-screen items-center justify-center p-4">
          <div 
            class="relative bg-white rounded-2xl shadow-2xl w-full max-w-3xl transform transition-all"
            @click.stop
          >
            <!-- Header -->
            <div class="bg-gradient-to-r from-blue-600 to-blue-700 px-6 py-4 rounded-t-2xl">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-white/20 rounded-lg flex items-center justify-center">
                    <Anchor :size="20" class="text-white" />
                  </div>
                  <h2 class="text-2xl font-bold text-white">
                    {{ isEditMode ? 'Edit Berth' : 'Create New Berth' }}
                  </h2>
                </div>
                <button 
                  @click="handleClose"
                  class="p-2 hover:bg-white/10 rounded-lg transition-colors"
                >
                  <X :size="24" class="text-white" />
                </button>
              </div>
            </div>

            <!-- Form Body -->
            <form @submit.prevent="handleSubmit" class="p-6">
              <!-- Error Message -->
              <div v-if="errorMessage" class="mb-4 p-4 bg-red-50 border border-red-200 rounded-lg flex items-start gap-3">
                <AlertCircle :size="20" class="text-red-600 flex-shrink-0 mt-0.5" />
                <div class="flex-1">
                  <p class="text-sm font-medium text-red-800">{{ errorMessage }}</p>
                </div>
                <button @click="errorMessage = ''" type="button">
                  <X :size="16" class="text-red-600" />
                </button>
              </div>

              <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <!-- Basic Information Section -->
                <div class="md:col-span-2">
                  <h3 class="text-lg font-semibold text-slate-900 mb-4 flex items-center gap-2">
                    <Info :size="20" class="text-blue-600" />
                    Basic Information
                  </h3>
                </div>

                <!-- Berth Name -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Berth Name <span class="text-red-500">*</span>
                  </label>
                  <input
                    v-model="formData.name"
                    type="text"
                    required
                    placeholder="e.g., North Container Berth 1"
                    class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>

                <!-- Identifier -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Identifier <span class="text-red-500">*</span>
                  </label>
                  <input
                    v-model="formData.identifier"
                    type="text"
                    required
                    placeholder="e.g., NCB1"
                    class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>

                <!-- Port Selection -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Port <span class="text-red-500">*</span>
                  </label>
                  <select
                    v-model="formData.portId"
                    required
                    class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  >
                    <option value="">Select a port</option>
                    <option v-for="port in ports" :key="port.portId" :value="port.portId">
                      {{ port.name }} {{ port.code ? `(${port.code})` : '' }}
                    </option>
                  </select>
                </div>

                <!-- Status -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Status <span class="text-red-500">*</span>
                  </label>
                  <select
                    v-model="formData.status"
                    required
                    class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  >
                    <option value="Available">Available</option>
                    <option value="Occupied">Occupied</option>
                    <option value="Maintenance">Maintenance</option>
                  </select>
                </div>

                <!-- Technical Specifications -->
                <div class="md:col-span-2">
                  <h3 class="text-lg font-semibold text-slate-900 mb-4 flex items-center gap-2">
                    <Settings :size="20" class="text-blue-600" />
                    Technical Specifications
                  </h3>
                </div>

                <!-- Berth Type -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Berth Type <span class="text-red-500">*</span>
                  </label>
                  <select
                    v-model="formData.type"
                    required
                    class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  >
                    <option value="">Select type</option>
                    <option value="Container">Container</option>
                    <option value="Bulk">Bulk</option>
                    <option value="RoRo">RoRo (Roll-on/Roll-off)</option>
                    <option value="General">General Cargo</option>
                  </select>
                </div>

                <!-- Priority -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Priority <span class="text-red-500">*</span>
                  </label>
                  <select
                    v-model="formData.priority"
                    required
                    class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  >
                    <option value="Low">Low</option>
                    <option value="Medium">Medium</option>
                    <option value="High">High</option>
                    <option value="Critical">Critical</option>
                  </select>
                </div>

                <!-- Capacity -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Capacity <span class="text-red-500">*</span>
                  </label>
                  <input
                    v-model.number="formData.capacity"
                    type="number"
                    required
                    min="1"
                    placeholder="Maximum capacity"
                    class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>

                <!-- Current Load -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Current Load
                  </label>
                  <input
                    v-model.number="formData.currentLoad"
                    type="number"
                    min="0"
                    :max="formData.capacity"
                    placeholder="Current load"
                    class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>

                <!-- Max Ship Length -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Max Ship Length (meters)
                  </label>
                  <input
                    v-model.number="formData.maxShipLength"
                    type="number"
                    min="0"
                    placeholder="Maximum ship length"
                    class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>

                <!-- Crane Count -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Crane Count
                  </label>
                  <input
                    v-model.number="formData.craneCount"
                    type="number"
                    min="0"
                    placeholder="Number of cranes"
                    class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>

                <!-- Max Draft -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Max Draft (meters)
                  </label>
                  <input
                    v-model.number="formData.maxDraft"
                    type="number"
                    step="0.1"
                    min="0"
                    placeholder="Maximum draft depth"
                    class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>

                <!-- Hourly Rate -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Hourly Rate
                  </label>
                  <input
                    v-model.number="formData.hourlyRate"
                    type="number"
                    step="0.01"
                    min="0"
                    placeholder="Hourly rate"
                    class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>

                <!-- Available Services -->
                <div class="md:col-span-2">
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Available Services
                  </label>
                  <div class="grid grid-cols-2 md:grid-cols-3 gap-3">
                    <label 
                      v-for="service in availableServiceOptions" 
                      :key="service"
                      class="flex items-center gap-2 p-3 border border-slate-200 rounded-lg hover:bg-slate-50 cursor-pointer transition-colors"
                    >
                      <input
                        type="checkbox"
                        :value="service"
                        v-model="selectedServices"
                        class="rounded border-slate-300 text-blue-600 focus:ring-blue-500"
                      />
                      <span class="text-sm text-slate-700">{{ service }}</span>
                    </label>
                  </div>
                </div>

                <!-- Additional Information -->
                <div class="md:col-span-2">
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Notes
                  </label>
                  <textarea
                    v-model="formData.notes"
                    rows="3"
                    placeholder="Additional notes or special requirements..."
                    class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 resize-none"
                  ></textarea>
                </div>
              </div>

              <!-- Form Actions -->
              <div class="mt-8 flex items-center justify-end gap-3 pt-6 border-t border-slate-200">
                <button
                  type="button"
                  @click="handleClose"
                  :disabled="submitting"
                  class="px-6 py-2.5 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 transition-colors disabled:opacity-50"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  :disabled="submitting"
                  class="px-6 py-2.5 bg-gradient-to-r from-blue-600 to-blue-700 text-white rounded-lg hover:from-blue-700 hover:to-blue-800 transition-all shadow-md disabled:opacity-50 flex items-center gap-2"
                >
                  <Loader v-if="submitting" :size="18" class="animate-spin" />
                  <Check v-else :size="18" />
                  {{ submitting ? 'Saving...' : (isEditMode ? 'Update Berth' : 'Create Berth') }}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { 
  Anchor, X, AlertCircle, Info, Settings, MapPin, 
  Check, Loader 
} from 'lucide-vue-next'
import type { Berth } from '../../types/berth'
import type { Port } from '../../types/port'

// Props
const props = defineProps<{
  isOpen: boolean
  berth?: Berth | null
  ports: Port[]
}>()

// Emits
const emit = defineEmits<{
  close: []
  submit: [data: any]
}>()

// State
const submitting = ref(false)
const errorMessage = ref('')
const selectedServices = ref<string[]>([])

// Available service options
const availableServiceOptions = [
  'Crane Service',
  'Fuel Supply',
  'Water Supply',
  'Electricity',
  'Waste Disposal',
  'Security',
  'Customs',
  'Tugboat Service',
  'Pilotage',
  'Mooring',
  'Container Handling',
  'Cargo Storage'
]

// Form data
const formData = ref({
  name: '',
  identifier: '',
  portId: '',
  status: 'Available',
  type: '',
  priority: 'Medium',
  capacity: 100,
  currentLoad: 0,
  maxShipLength: null as number | null,
  maxDraft: null as number | null,
  craneCount: 0,
  hourlyRate: null as number | null,
  notes: ''
})

// Computed
const isEditMode = computed(() => !!props.berth)

// Methods
const resetForm = () => {
  formData.value = {
    name: '',
    identifier: '',
    portId: '',
    status: 'Available',
    type: '',
    priority: 'Medium',
    capacity: 100,
    currentLoad: 0,
    maxShipLength: null,
    maxDraft: null,
    craneCount: 0,
    hourlyRate: null,
    notes: ''
  }
  selectedServices.value = []
  errorMessage.value = ''
}

const loadBerthData = () => {
  if (props.berth) {
    formData.value = {
      name: props.berth.name,
      identifier: props.berth.identifier || '',
      portId: props.berth.portId.toString(),
      status: props.berth.status,
      type: props.berth.type || '',
      priority: props.berth.priority || 'Medium',
      capacity: props.berth.capacity,
      currentLoad: props.berth.currentLoad,
      maxShipLength: props.berth.maxShipLength || null,
      maxDraft: props.berth.maxDraft || null,
      craneCount: props.berth.craneCount || 0,
      hourlyRate: props.berth.hourlyRate || null,
      notes: props.berth.notes || ''
    }
    
    // Load services
    if (props.berth.availableServices) {
      selectedServices.value = props.berth.availableServices.split(',').map(s => s.trim()).filter(s => s)
    }
  }
}

const handleClose = () => {
  if (!submitting.value) {
    resetForm()
    emit('close')
  }
}

const handleSubmit = async () => {
  submitting.value = true
  errorMessage.value = ''

  try {
    // Prepare submission data
    const submitData = {
      ...formData.value,
      portId: parseInt(formData.value.portId),
      availableServices: selectedServices.value.join(', ')
    }

    emit('submit', submitData)
    resetForm()
  } catch (error: any) {
    errorMessage.value = error.message || 'An error occurred while saving the berth'
  } finally {
    submitting.value = false
  }
}

// Watch for berth changes
watch(() => props.berth, () => {
  if (props.isOpen) {
    if (props.berth) {
      loadBerthData()
    } else {
      resetForm()
    }
  }
}, { immediate: true })

// Watch for modal open
watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    if (props.berth) {
      loadBerthData()
    } else {
      resetForm()
    }
  }
})
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
