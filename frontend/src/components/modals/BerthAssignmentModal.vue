<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="isOpen && berth" class="fixed inset-0 z-50 overflow-y-auto">
        <!-- Backdrop -->
        <div 
          class="fixed inset-0 bg-black/50 backdrop-blur-sm transition-opacity"
          @click="handleClose"
        ></div>

        <!-- Modal Container -->
        <div class="flex min-h-screen items-center justify-center p-4">
          <div 
            class="relative bg-white rounded-2xl shadow-2xl w-full max-w-2xl transform transition-all"
            @click.stop
          >
            <!-- Header -->
            <div class="bg-gradient-to-r from-green-600 to-green-700 px-6 py-4 rounded-t-2xl">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-white/20 rounded-lg flex items-center justify-center">
                    <Clipboard :size="20" class="text-white" />
                  </div>
                  <div>
                    <h2 class="text-2xl font-bold text-white">New Berth Assignment</h2>
                    <p class="text-green-100 text-sm">Assign container to {{ berth.name }}</p>
                  </div>
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

              <!-- Berth Information Card -->
              <div class="mb-6 p-4 bg-gradient-to-r from-green-50 to-green-100 border border-green-200 rounded-xl">
                <div class="flex items-start justify-between">
                  <div>
                    <h3 class="text-lg font-semibold text-green-900 mb-1">{{ berth.name }}</h3>
                    <p class="text-sm text-green-700">{{ berth.identifier }} • Type: {{ berth.type }}</p>
                  </div>
                  <span 
                    class="px-3 py-1 rounded-full text-xs font-semibold"
                    :class="getStatusClass(berth.status)"
                  >
                    {{ berth.status }}
                  </span>
                </div>
                <div class="mt-3 grid grid-cols-3 gap-3 text-sm">
                  <div class="bg-white/50 rounded-lg p-2">
                    <p class="text-green-700 text-xs">Capacity</p>
                    <p class="font-semibold text-green-900">{{ berth.capacity }}</p>
                  </div>
                  <div class="bg-white/50 rounded-lg p-2">
                    <p class="text-green-700 text-xs">Current Load</p>
                    <p class="font-semibold text-green-900">{{ berth.currentLoad }}</p>
                  </div>
                  <div class="bg-white/50 rounded-lg p-2">
                    <p class="text-green-700 text-xs">Available</p>
                    <p class="font-semibold text-green-900">{{ berth.capacity - berth.currentLoad }}</p>
                  </div>
                </div>
              </div>

              <div class="space-y-6">
                <!-- Container Selection -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Container <span class="text-red-500">*</span>
                  </label>
                  <div class="relative">
                    <input
                      v-model="containerSearch"
                      @input="searchContainers"
                      @focus="showContainerDropdown = true"
                      type="text"
                      required
                      placeholder="Search for container by ID..."
                      class="w-full px-4 py-2 pr-10 border border-slate-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-green-500"
                    />
                    <Search :size="18" class="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400" />
                  </div>

                  <!-- Container Dropdown -->
                  <div 
                    v-if="showContainerDropdown && filteredContainers.length > 0"
                    class="mt-2 max-h-60 overflow-y-auto border border-slate-200 rounded-lg shadow-lg bg-white"
                  >
                    <button
                      v-for="container in filteredContainers"
                      :key="container.containerId"
                      type="button"
                      @click="selectContainer(container)"
                      class="w-full px-4 py-3 text-left hover:bg-slate-50 transition-colors border-b border-slate-100 last:border-b-0"
                    >
                      <div class="flex items-center justify-between">
                        <div>
                          <p class="font-semibold text-slate-900">{{ container.containerId }}</p>
                          <p class="text-sm text-slate-600">{{ container.type }} • {{ container.status }}</p>
                        </div>
                        <Check v-if="formData.containerId === container.containerId" :size="18" class="text-green-600" />
                      </div>
                    </button>
                  </div>

                  <!-- Selected Container Info -->
                  <div v-if="selectedContainer" class="mt-3 p-4 bg-slate-50 border border-slate-200 rounded-lg">
                    <div class="flex items-start justify-between">
                      <div class="flex-1">
                        <p class="font-semibold text-slate-900">{{ selectedContainer.containerId }}</p>
                        <div class="mt-2 grid grid-cols-2 gap-2 text-sm">
                          <div>
                            <span class="text-slate-600">Type:</span>
                            <span class="ml-1 font-medium text-slate-900">{{ selectedContainer.type }}</span>
                          </div>
                          <div>
                            <span class="text-slate-600">Status:</span>
                            <span class="ml-1 font-medium text-slate-900">{{ selectedContainer.status }}</span>
                          </div>
                          <div v-if="selectedContainer.weight">
                            <span class="text-slate-600">Weight:</span>
                            <span class="ml-1 font-medium text-slate-900">{{ selectedContainer.weight }} kg</span>
                          </div>
                          <div v-if="selectedContainer.currentLocation">
                            <span class="text-slate-600">Location:</span>
                            <span class="ml-1 font-medium text-slate-900">{{ selectedContainer.currentLocation }}</span>
                          </div>
                        </div>
                      </div>
                      <button
                        type="button"
                        @click="clearContainer"
                        class="ml-3 p-1 hover:bg-slate-200 rounded transition-colors"
                      >
                        <X :size="18" class="text-slate-600" />
                      </button>
                    </div>
                  </div>
                </div>

                <!-- Assignment Date & Time -->
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-slate-700 mb-2">
                      Assigned Date <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.assignedDate"
                      type="date"
                      required
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-green-500"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-slate-700 mb-2">
                      Assigned Time <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.assignedTime"
                      type="time"
                      required
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-green-500"
                    />
                  </div>
                </div>

                <!-- Expected Release (Optional) -->
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-slate-700 mb-2">
                      Expected Release Date (Optional)
                    </label>
                    <input
                      v-model="formData.expectedReleaseDate"
                      type="date"
                      :min="formData.assignedDate"
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-green-500"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-slate-700 mb-2">
                      Expected Release Time
                    </label>
                    <input
                      v-model="formData.expectedReleaseTime"
                      type="time"
                      :disabled="!formData.expectedReleaseDate"
                      class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-green-500 disabled:bg-slate-100"
                    />
                  </div>
                </div>

                <!-- Priority -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Assignment Priority
                  </label>
                  <select
                    v-model="formData.priority"
                    class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-green-500"
                  >
                    <option value="Low">Low</option>
                    <option value="Medium">Medium</option>
                    <option value="High">High</option>
                    <option value="Urgent">Urgent</option>
                  </select>
                </div>

                <!-- Assignment Notes -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">
                    Assignment Notes
                  </label>
                  <textarea
                    v-model="formData.notes"
                    rows="3"
                    placeholder="Enter any special instructions or notes for this assignment..."
                    class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-green-500 resize-none"
                  ></textarea>
                </div>

                <!-- Notify Option -->
                <div class="flex items-center gap-3 p-4 bg-blue-50 border border-blue-200 rounded-lg">
                  <input
                    v-model="formData.notifyStaff"
                    type="checkbox"
                    id="notifyStaff"
                    class="w-4 h-4 rounded border-slate-300 text-green-600 focus:ring-green-500"
                  />
                  <label for="notifyStaff" class="text-sm text-slate-700 cursor-pointer">
                    <span class="font-medium">Notify relevant staff members</span>
                    <p class="text-slate-600 text-xs mt-0.5">Send notifications to port operators and managers about this assignment</p>
                  </label>
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
                  :disabled="submitting || !formData.containerId"
                  class="px-6 py-2.5 bg-gradient-to-r from-green-600 to-green-700 text-white rounded-lg hover:from-green-700 hover:to-green-800 transition-all shadow-md disabled:opacity-50 flex items-center gap-2"
                >
                  <Loader v-if="submitting" :size="18" class="animate-spin" />
                  <Check v-else :size="18" />
                  {{ submitting ? 'Creating Assignment...' : 'Create Assignment' }}
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
import { ref, computed, watch, onMounted } from 'vue'
import { 
  Clipboard, X, AlertCircle, Search, Check, Loader
} from 'lucide-vue-next'
import { containerApi } from '../../services/api'
import type { Berth } from '../../types/berth'
import type { Container } from '../../types/container'

// Props
const props = defineProps<{
  isOpen: boolean
  berth: Berth | null
}>()

// Emits
const emit = defineEmits<{
  close: []
  submit: [data: any]
}>()

// State
const submitting = ref(false)
const errorMessage = ref('')
const containerSearch = ref('')
const showContainerDropdown = ref(false)
const containers = ref<Container[]>([])
const selectedContainer = ref<Container | null>(null)

// Form data
const formData = ref({
  berthId: 0,
  containerId: '',
  assignedDate: new Date().toISOString().split('T')[0],
  assignedTime: new Date().toTimeString().split(' ')[0].slice(0, 5),
  expectedReleaseDate: '',
  expectedReleaseTime: '',
  priority: 'Medium',
  notes: '',
  notifyStaff: true
})

// Computed
const filteredContainers = computed(() => {
  if (!Array.isArray(containers.value)) return []
  if (!containerSearch.value) return containers.value.slice(0, 10)
  
  const query = containerSearch.value.toLowerCase()
  return containers.value
    .filter(c => 
      c.containerId.toLowerCase().includes(query) ||
      (c.type && c.type.toLowerCase().includes(query)) ||
      (c.status && c.status.toLowerCase().includes(query))
    )
    .slice(0, 10)
})

// Methods
const loadContainers = async () => {
  try {
    const response = await containerApi.getAll()
    containers.value = response.data || []
  } catch (error) {
    console.error('Error loading containers:', error)
    containers.value = []
  }
}

const searchContainers = () => {
  showContainerDropdown.value = true
}

const selectContainer = (container: Container) => {
  selectedContainer.value = container
  formData.value.containerId = container.containerId
  containerSearch.value = container.containerId
  showContainerDropdown.value = false
}

const clearContainer = () => {
  selectedContainer.value = null
  formData.value.containerId = ''
  containerSearch.value = ''
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

const resetForm = () => {
  formData.value = {
    berthId: 0,
    containerId: '',
    assignedDate: new Date().toISOString().split('T')[0],
    assignedTime: new Date().toTimeString().split(' ')[0].slice(0, 5),
    expectedReleaseDate: '',
    expectedReleaseTime: '',
    priority: 'Medium',
    notes: '',
    notifyStaff: true
  }
  containerSearch.value = ''
  selectedContainer.value = null
  errorMessage.value = ''
}

const handleClose = () => {
  if (!submitting.value) {
    resetForm()
    emit('close')
  }
}

const handleSubmit = async () => {
  if (!props.berth) return

  submitting.value = true
  errorMessage.value = ''

  try {
    // Combine date and time into ISO datetime
    const assignedAt = new Date(`${formData.value.assignedDate}T${formData.value.assignedTime}`).toISOString()
    
    let expectedReleasedAt = null
    if (formData.value.expectedReleaseDate && formData.value.expectedReleaseTime) {
      expectedReleasedAt = new Date(`${formData.value.expectedReleaseDate}T${formData.value.expectedReleaseTime}`).toISOString()
    }

    const submitData = {
      berthId: props.berth.berthId,
      containerId: formData.value.containerId,
      assignedAt,
      expectedReleasedAt,
      notes: formData.value.notes || null,
      priority: formData.value.priority
    }

    emit('submit', submitData)
    resetForm()
  } catch (error: any) {
    errorMessage.value = error.message || 'An error occurred while creating the assignment'
  } finally {
    submitting.value = false
  }
}

// Watch for berth changes
watch(() => props.berth, (newBerth) => {
  if (newBerth) {
    formData.value.berthId = newBerth.berthId
  }
}, { immediate: true })

// Watch for modal open
watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    loadContainers()
  }
})

// Close dropdown when clicking outside
onMounted(() => {
  document.addEventListener('click', (e) => {
    const target = e.target as HTMLElement
    if (!target.closest('.container-search')) {
      showContainerDropdown.value = false
    }
  })
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
