<!-- filepath: c:\Users\dhruv\Desktop\Company projects\Container-Tracking-and-Port-Operations-Maersk-Hackathon\frontend\src\components\main\PortOperationManagement.vue -->
<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 to-blue-50">
    <main class="mx-auto px-6 py-8" style="max-width: 1400px;">
      
      <!-- Header Section -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-slate-900 mb-2">Port Operations Management</h1>
        <p class="text-slate-600">Real-time berth management and operational control</p>
      </div>

      <!-- Toast Notification -->
      <Transition name="toast">
        <div 
          v-if="toast.show" 
          class="fixed top-4 right-4 z-50 max-w-md"
        >
          <div 
            :class="[
              'p-4 rounded-lg shadow-lg border',
              toast.type === 'success' ? 'bg-green-50 border-green-200' : 'bg-red-50 border-red-200'
            ]"
          >
            <div class="flex items-start gap-3">
              <CheckCircle2 v-if="toast.type === 'success'" :size="20" class="text-green-600 flex-shrink-0 mt-0.5" />
              <AlertCircle v-else :size="20" class="text-red-600 flex-shrink-0 mt-0.5" />
              <div class="flex-1">
                <p :class="[
                  'text-sm font-medium',
                  toast.type === 'success' ? 'text-green-800' : 'text-red-800'
                ]">
                  {{ toast.message }}
                </p>
              </div>
              <button @click="toast.show = false">
                <X :size="16" :class="toast.type === 'success' ? 'text-green-600' : 'text-red-600'" />
              </button>
            </div>
          </div>
        </div>
      </Transition>

      <!-- Statistics Cards -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div class="bg-white rounded-xl p-6 shadow-md border border-slate-200">
          <div class="flex items-center justify-between mb-2">
            <span class="text-slate-600 text-sm font-medium">Total Berths</span>
            <Anchor :size="20" class="text-blue-600" />
          </div>
          <div class="text-3xl font-bold text-slate-900">{{ berthStats.total }}</div>
        </div>

        <div class="bg-white rounded-xl p-6 shadow-md border border-slate-200">
          <div class="flex items-center justify-between mb-2">
            <span class="text-slate-600 text-sm font-medium">Available</span>
            <CheckCircle2 :size="20" class="text-green-600" />
          </div>
          <div class="text-3xl font-bold text-green-600">{{ berthStats.available }}</div>
        </div>

        <div class="bg-white rounded-xl p-6 shadow-md border border-slate-200">
          <div class="flex items-center justify-between mb-2">
            <span class="text-slate-600 text-sm font-medium">Occupied</span>
            <AlertCircle :size="20" class="text-amber-600" />
          </div>
          <div class="text-3xl font-bold text-amber-600">{{ berthStats.occupied }}</div>
        </div>

        <div class="bg-white rounded-xl p-6 shadow-md border border-slate-200">
          <div class="flex items-center justify-between mb-2">
            <span class="text-slate-600 text-sm font-medium">Utilization</span>
            <TrendingUp :size="20" class="text-blue-600" />
          </div>
          <div class="text-3xl font-bold text-slate-900">{{ berthStats.utilizationRate }}%</div>
        </div>
      </div>

      <!-- Filters Section -->
      <div class="bg-white rounded-xl border border-slate-200 shadow-md p-6 mb-8">
        <div class="flex flex-wrap gap-4 items-end">
          <!-- Port Filter -->
          <div class="flex-1 min-w-[200px]">
            <label class="block text-sm font-medium text-slate-700 mb-2">Filter by Port</label>
            <select 
              v-model="filters.portId" 
              @change="applyFilters"
              class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="">All Ports</option>
              <option v-for="port in ports" :key="port.portId" :value="port.portId">
                {{ port.name }} {{ port.code ? `(${port.code})` : '' }}
              </option>
            </select>
          </div>

          <!-- Status Filter -->
          <div class="flex-1 min-w-[200px]">
            <label class="block text-sm font-medium text-slate-700 mb-2">Filter by Status</label>
            <select 
              v-model="filters.status" 
              @change="applyFilters"
              class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="">All Status</option>
              <option value="Available">Available</option>
              <option value="Occupied">Occupied</option>
              <option value="Maintenance">Maintenance</option>
            </select>
          </div>

          <!-- Type Filter -->
          <div class="flex-1 min-w-[200px]">
            <label class="block text-sm font-medium text-slate-700 mb-2">Filter by Type</label>
            <select 
              v-model="filters.type" 
              @change="applyFilters"
              class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="">All Types</option>
              <option value="Container">Container</option>
              <option value="Bulk">Bulk</option>
              <option value="RoRo">RoRo</option>
              <option value="General">General</option>
            </select>
          </div>

          <!-- Search -->
          <div class="flex-1 min-w-[250px]">
            <label class="block text-sm font-medium text-slate-700 mb-2">Search Berth</label>
            <div class="relative">
              <Search :size="18" class="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
              <input 
                v-model="filters.search"
                @input="applyFilters"
                type="text" 
                placeholder="Search by name or identifier..."
                class="w-full pl-10 pr-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
          </div>

          <!-- Actions -->
          <div class="flex gap-2">
            <button 
              @click="loadBerths"
              class="px-4 py-2 bg-slate-100 text-slate-700 rounded-lg hover:bg-slate-200 transition-colors"
              :disabled="loading"
            >
              <RefreshCw :size="18" :class="{ 'animate-spin': loading }" />
            </button>
            <button 
              @click="openCreateBerthModal"
              class="px-6 py-2 bg-gradient-to-r from-blue-600 to-blue-700 text-white rounded-lg hover:from-blue-700 hover:to-blue-800 transition-all shadow-md flex items-center gap-2"
            >
              <Plus :size="18" />
              New Berth
            </button>
          </div>
        </div>
      </div>

      <!-- Berth Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
        <!-- Loading State -->
        <div v-if="loading" class="col-span-full flex justify-center items-center py-20">
          <div class="text-center">
            <RefreshCw :size="48" class="animate-spin text-blue-600 mx-auto mb-4" />
            <p class="text-slate-600">Loading berth data...</p>
          </div>
        </div>

        <!-- Empty State -->
        <div v-else-if="paginatedBerths.length === 0" class="col-span-full">
          <div class="bg-white rounded-xl border-2 border-dashed border-slate-300 p-12 text-center">
            <Anchor :size="48" class="text-slate-300 mx-auto mb-4" />
            <h3 class="text-lg font-semibold text-slate-900 mb-2">No Berths Found</h3>
            <p class="text-slate-600 mb-6">
              {{ hasActiveFilters 
                ? 'No berths match your filter criteria.' 
                : 'Get started by creating your first berth.' }}
            </p>
            <button 
              v-if="!hasActiveFilters"
              @click="openCreateBerthModal"
              class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors inline-flex items-center gap-2"
            >
              <Plus :size="20" />
              Create First Berth
            </button>
            <button 
              v-else
              @click="clearFilters"
              class="px-6 py-3 bg-slate-200 text-slate-700 rounded-lg hover:bg-slate-300 transition-colors"
            >
              Clear Filters
            </button>
          </div>
        </div>

        <!-- Berth Cards -->
        <div 
          v-for="berth in paginatedBerths" 
          :key="berth.berthId"
          class="bg-white rounded-xl border border-slate-200 shadow-md hover:shadow-xl transition-all duration-300 overflow-hidden"
        >
          <!-- Card Header -->
          <div class="bg-gradient-to-r from-blue-600 to-blue-700 p-4">
            <div class="flex items-start justify-between">
              <div class="flex items-center gap-3">
                <div class="w-12 h-12 bg-white/20 rounded-lg flex items-center justify-center">
                  <span class="text-white font-bold text-lg">{{ berth.identifier }}</span>
                </div>
                <div>
                  <h3 class="text-white font-bold text-lg">{{ berth.name }}</h3>
                  <p class="text-blue-100 text-sm">{{ getPortName(berth.portId) }}</p>
                </div>
              </div>
              <span 
                class="px-3 py-1 rounded-full text-xs font-semibold"
                :class="getStatusClass(berth.status)"
              >
                {{ berth.status }}
              </span>
            </div>
          </div>

          <!-- Card Body -->
          <div class="p-4">
            <!-- Capacity Info -->
            <div class="mb-4">
              <div class="flex justify-between items-center mb-2">
                <span class="text-sm text-slate-600">Capacity Utilization</span>
                <span class="text-sm font-semibold text-slate-900">
                  {{ berth.currentLoad }} / {{ berth.capacity }}
                </span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-2">
                <div 
                  class="h-2 rounded-full transition-all duration-300"
                  :class="getUtilizationColorClass(getUtilizationRatio(berth))"
                  :style="{ width: `${getUtilizationRatio(berth) * 100}%` }"
                ></div>
              </div>
            </div>

            <!-- Berth Details Grid -->
            <div class="grid grid-cols-2 gap-3 mb-4">
              <div class="bg-slate-50 rounded-lg p-3">
                <p class="text-xs text-slate-600 mb-1">Type</p>
                <p class="font-semibold text-slate-900">{{ berth.type }}</p>
              </div>
              <div class="bg-slate-50 rounded-lg p-3">
                <p class="text-xs text-slate-600 mb-1">Priority</p>
                <p class="font-semibold text-slate-900">{{ berth.priority }}</p>
              </div>
              <div class="bg-slate-50 rounded-lg p-3">
                <p class="text-xs text-slate-600 mb-1">Max Ship Length</p>
                <p class="font-semibold text-slate-900">{{ berth.maxShipLength || 'N/A' }}m</p>
              </div>
              <div class="bg-slate-50 rounded-lg p-3">
                <p class="text-xs text-slate-600 mb-1">Cranes</p>
                <p class="font-semibold text-slate-900">{{ berth.craneCount || 0 }}</p>
              </div>
            </div>

            <!-- Services -->
            <div v-if="berth.availableServices" class="mb-4">
              <p class="text-xs text-slate-600 mb-2">Available Services</p>
              <div class="flex flex-wrap gap-1">
                <span 
                  v-for="service in parseServices(berth.availableServices)" 
                  :key="service"
                  class="px-2 py-1 bg-blue-50 text-blue-700 text-xs rounded-md"
                >
                  {{ service }}
                </span>
              </div>
            </div>

            <!-- Active Assignments -->
            <div v-if="berth.activeAssignmentCount && berth.activeAssignmentCount > 0" class="mb-4 p-3 bg-amber-50 border border-amber-200 rounded-lg">
              <div class="flex items-center gap-2">
                <AlertCircle :size="16" class="text-amber-600" />
                <span class="text-sm text-amber-800">
                  {{ berth.activeAssignmentCount }} active assignment{{ berth.activeAssignmentCount > 1 ? 's' : '' }}
                </span>
              </div>
            </div>

            <!-- Action Buttons -->
            <div class="flex gap-2">
              <button 
                @click="viewBerthDetails(berth)"
                class="flex-1 px-4 py-2 bg-slate-100 text-slate-700 rounded-lg hover:bg-slate-200 transition-colors text-sm font-medium"
              >
                View
              </button>
              <button 
                @click="openEditBerthModal(berth)"
                class="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors text-sm font-medium"
              >
                Edit
              </button>
              <button 
                @click="openAssignmentModal(berth)"
                class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors text-sm font-medium flex items-center justify-center"
              >
                <Plus :size="16" />
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Pagination -->
      <div v-if="totalPages > 1" class="mt-8 flex justify-center">
        <div class="flex items-center gap-2">
          <button 
            @click="goToPage(pagination.currentPage - 1)" 
            :disabled="pagination.currentPage === 1"
            class="px-4 py-2 border border-slate-300 rounded-lg hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            Previous
          </button>
          
          <!-- Page Numbers -->
          <div class="flex gap-1">
            <button
              v-for="page in visiblePages"
              :key="page"
              @click="goToPage(page)"
              :class="[
                'px-4 py-2 rounded-lg transition-colors',
                page === pagination.currentPage
                  ? 'bg-blue-600 text-white'
                  : 'border border-slate-300 hover:bg-slate-50'
              ]"
            >
              {{ page }}
            </button>
          </div>

          <button 
            @click="goToPage(pagination.currentPage + 1)" 
            :disabled="pagination.currentPage === totalPages"
            class="px-4 py-2 border border-slate-300 rounded-lg hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            Next
          </button>
        </div>
      </div>

      <!-- Results Info -->
      <div v-if="filteredBerths.length > 0" class="mt-4 text-center text-sm text-slate-600">
        Showing {{ startIndex + 1 }} - {{ Math.min(endIndex, filteredBerths.length) }} of {{ filteredBerths.length }} berths
      </div>
    </main>

    <!-- Modals -->
    <BerthFormModal 
      :isOpen="modals.createBerth"
      :berth="null"
      :ports="ports"
      @close="modals.createBerth = false"
      @submit="handleCreateBerth"
    />

    <BerthFormModal 
      :isOpen="modals.editBerth"
      :berth="selectedBerth"
      :ports="ports"
      @close="modals.editBerth = false"
      @submit="handleUpdateBerth"
    />

    <BerthDetailsModal 
      :isOpen="modals.berthDetails"
      :berth="selectedBerth"
      :ports="ports"
      @close="modals.berthDetails = false"
      @edit="handleEditFromDetails"
      @assign="handleAssignFromDetails"
    />

    <BerthAssignmentModal 
      :isOpen="modals.assignment"
      :berth="selectedBerth"
      @close="modals.assignment = false"
      @submit="handleCreateAssignment"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { 
  Anchor, CheckCircle2, AlertCircle, TrendingUp, Search, 
  RefreshCw, Plus, X
} from 'lucide-vue-next'
import { berthApi, portApi, berthAssignmentApi } from '../../services/api'
import type { Berth } from '../../types/berth'
import type { Port } from '../../types/port'
import BerthFormModal from '../modals/BerthFormModal.vue'
import BerthDetailsModal from '../modals/BerthDetailsModal.vue'
import BerthAssignmentModal from '../modals/BerthAssignmentModal.vue'

// Reactive state
const berths = ref<Berth[]>([])
const ports = ref<Port[]>([])
const loading = ref(false)
const selectedBerth = ref<Berth | null>(null)

// Toast notification
const toast = ref({
  show: false,
  message: '',
  type: 'success' as 'success' | 'error'
})

// Modal states
const modals = ref({
  createBerth: false,
  editBerth: false,
  berthDetails: false,
  assignment: false
})

// Filters
const filters = ref({
  portId: '',
  status: '',
  type: '',
  search: ''
})

// Pagination
const pagination = ref({
  currentPage: 1,
  itemsPerPage: 12
})

// Computed properties
const berthStats = computed(() => {
  const available = berths.value.filter(b => b.status === 'Available').length
  const occupied = berths.value.filter(b => b.status === 'Occupied').length
  const utilizationRate = berths.value.length > 0 
    ? Math.round((occupied / berths.value.length) * 100)
    : 0

  return {
    total: berths.value.length,
    available,
    occupied,
    maintenance: berths.value.filter(b => b.status === 'Maintenance').length,
    utilizationRate
  }
})

const hasActiveFilters = computed(() => {
  return !!(filters.value.portId || filters.value.status || filters.value.type || filters.value.search)
})

const filteredBerths = computed(() => {
  let filtered = [...berths.value]

  // Filter by port
  if (filters.value.portId) {
    filtered = filtered.filter(b => b.portId === parseInt(filters.value.portId))
  }

  // Filter by status
  if (filters.value.status) {
    filtered = filtered.filter(b => b.status === filters.value.status)
  }

  // Filter by type
  if (filters.value.type) {
    filtered = filtered.filter(b => b.type === filters.value.type)
  }

  // Search filter
  if (filters.value.search) {
    const query = filters.value.search.toLowerCase()
    filtered = filtered.filter(b => 
      b.name.toLowerCase().includes(query) ||
      (b.identifier && b.identifier.toLowerCase().includes(query))
    )
  }

  return filtered
})

const totalPages = computed(() => {
  return Math.ceil(filteredBerths.value.length / pagination.value.itemsPerPage)
})

const startIndex = computed(() => {
  return (pagination.value.currentPage - 1) * pagination.value.itemsPerPage
})

const endIndex = computed(() => {
  return startIndex.value + pagination.value.itemsPerPage
})

const paginatedBerths = computed(() => {
  return filteredBerths.value.slice(startIndex.value, endIndex.value)
})

const visiblePages = computed(() => {
  const current = pagination.value.currentPage
  const total = totalPages.value
  const pages: number[] = []

  if (total <= 7) {
    for (let i = 1; i <= total; i++) {
      pages.push(i)
    }
  } else {
    if (current <= 3) {
      pages.push(1, 2, 3, 4, 5)
    } else if (current >= total - 2) {
      pages.push(total - 4, total - 3, total - 2, total - 1, total)
    } else {
      pages.push(current - 2, current - 1, current, current + 1, current + 2)
    }
  }

  return pages
})

// Helper functions
const getPortName = (portId: number) => {
  const port = ports.value.find(p => p.portId === portId)
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

const getUtilizationRatio = (berth: Berth) => {
  return berth.capacity > 0 ? berth.currentLoad / berth.capacity : 0
}

const getUtilizationColorClass = (ratio: number) => {
  if (ratio >= 0.9) return 'bg-red-500'
  if (ratio >= 0.7) return 'bg-amber-500'
  return 'bg-green-500'
}

const parseServices = (services: string) => {
  return services ? services.split(',').map(s => s.trim()).filter(s => s) : []
}

// Toast notification helper
const showToast = (message: string, type: 'success' | 'error' = 'success') => {
  toast.value = { show: true, message, type }
  setTimeout(() => {
    toast.value.show = false
  }, 5000)
}

// Data loading
const loadBerths = async () => {
  loading.value = true
  try {
    const [berthsResponse, portsResponse] = await Promise.all([
      berthApi.getAll(),
      portApi.getAll()
    ])
    berths.value = berthsResponse.data
    ports.value = portsResponse.data
  } catch (error) {
    console.error('Error loading berth data:', error)
    showToast('Failed to load berth data', 'error')
  } finally {
    loading.value = false
  }
}

const applyFilters = () => {
  pagination.value.currentPage = 1 // Reset to first page when filtering
}

const clearFilters = () => {
  filters.value = {
    portId: '',
    status: '',
    type: '',
    search: ''
  }
  pagination.value.currentPage = 1
}

const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    pagination.value.currentPage = page
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

// Modal functions
const openCreateBerthModal = () => {
  selectedBerth.value = null
  modals.value.createBerth = true
}

const openEditBerthModal = (berth: Berth) => {
  selectedBerth.value = berth
  modals.value.editBerth = true
}

const viewBerthDetails = (berth: Berth) => {
  selectedBerth.value = berth
  modals.value.berthDetails = true
}

const openAssignmentModal = (berth: Berth) => {
  selectedBerth.value = berth
  modals.value.assignment = true
}

// Handle modal actions
const handleCreateBerth = async (data: any) => {
  try {
    await berthApi.create(data)
    showToast('Berth created successfully!')
    modals.value.createBerth = false
    await loadBerths()
  } catch (error: any) {
    showToast(error.message || 'Failed to create berth', 'error')
  }
}

const handleUpdateBerth = async (data: any) => {
  if (!selectedBerth.value) return
  
  try {
    await berthApi.update(selectedBerth.value.berthId, data)
    showToast('Berth updated successfully!')
    modals.value.editBerth = false
    await loadBerths()
  } catch (error: any) {
    showToast(error.message || 'Failed to update berth', 'error')
  }
}

const handleCreateAssignment = async (data: any) => {
  try {
    await berthAssignmentApi.create(data)
    showToast('Assignment created successfully!')
    modals.value.assignment = false
    await loadBerths()
  } catch (error: any) {
    showToast(error.message || 'Failed to create assignment', 'error')
  }
}

const handleEditFromDetails = () => {
  modals.value.berthDetails = false
  modals.value.editBerth = true
}

const handleAssignFromDetails = () => {
  modals.value.berthDetails = false
  modals.value.assignment = true
}

// Watch for filter changes to update URL params (optional)
watch(() => filters.value, () => {
  // Could add URL parameter updates here for bookmarking filter state
}, { deep: true })

// Lifecycle
onMounted(() => {
  loadBerths()
})
</script>

<style scoped>
.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease;
}

.toast-enter-from {
  transform: translateX(100%);
  opacity: 0;
}

.toast-leave-to {
  transform: translateY(-20px);
  opacity: 0;
}
</style>
