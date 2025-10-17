<template>
  <div class="bg-white rounded-xl border border-slate-200 shadow-sm">
    <div class="border-b border-slate-200 p-6">
      <div class="flex items-center gap-3">
        <div class="p-2 bg-emerald-50 rounded-lg">
          <Building2 :size="20" class="text-emerald-600" />
        </div>
        <div>
          <h3 class="text-xl font-semibold text-slate-900">Available Berths</h3>
          <p class="text-sm text-slate-600">Free berths ready for ship docking</p>
        </div>
      </div>
    </div>
    
    <div class="p-6">
      <!-- Loading State -->
      <div v-if="loading" class="space-y-4">
        <div v-for="i in 4" :key="i" class="flex items-center justify-between p-4 bg-slate-50 rounded-lg animate-pulse">
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 rounded-lg bg-slate-200"></div>
            <div class="flex flex-col gap-2">
              <div class="h-4 bg-slate-200 rounded w-24"></div>
              <div class="h-3 bg-slate-200 rounded w-20"></div>
            </div>
            <div class="h-6 bg-slate-200 rounded-full w-16"></div>
          </div>
          <div class="text-right space-y-1">
            <div class="h-4 bg-slate-200 rounded w-16"></div>
            <div class="h-3 bg-slate-200 rounded w-12"></div>
          </div>
        </div>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="text-center py-8">
        <AlertCircle :size="48" class="mx-auto text-red-500 mb-4" />
        <h3 class="text-lg font-semibold text-red-900 mb-2">Failed to Load Available Berths</h3>
        <p class="text-red-700 mb-4">{{ error }}</p>
        <button
          @click="$emit('retry')"
          class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
        >
          Retry
        </button>
      </div>

      <!-- Empty State -->
      <div v-else-if="berths.length === 0" class="text-center py-8">
        <Building2 :size="48" class="mx-auto text-slate-400 mb-4" />
        <h3 class="text-lg font-semibold text-slate-700 mb-2">No Available Berths</h3>
        <p class="text-slate-500">All berths are currently occupied or under maintenance</p>
      </div>

      <!-- Berth List -->
      <div v-else class="space-y-4">
        <div 
          v-for="(berth, index) in paginatedBerths"
          :key="berth.berthId"
          class="flex items-center justify-between p-4 bg-slate-50 rounded-lg hover:bg-slate-100 transition-all duration-200 group animate-slideIn"
          :style="{ animationDelay: `${index * 50}ms` }"
        >
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 rounded-lg bg-white border-2 border-slate-200 flex items-center justify-center font-bold text-slate-700 group-hover:border-emerald-300 transition-colors">
              {{ getBerthDisplayNumber(berth) }}
            </div>
            <div class="flex flex-col">
              <span class="font-semibold text-slate-900">{{ berth.name }}</span>
              <span class="text-sm text-slate-600">{{ berth.type || 'Standard' }} Berth</span>
              <span v-if="berth.availableServices" class="text-xs text-slate-500">{{ formatServices(berth.availableServices) }}</span>
            </div>
            <span 
              class="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold"
              :class="getStatusColor(berth.status)"
            >
              {{ berth.status }}
            </span>
            <div v-if="berth.capacity || berth.craneCount" class="text-xs text-slate-500">
              <div v-if="berth.capacity">{{ berth.capacity }} TEU capacity</div>
              <div v-if="berth.craneCount">{{ berth.craneCount }} cranes</div>
            </div>
          </div>
          <div class="text-right">
            <div class="text-sm font-semibold text-slate-900">{{ berth.portName || 'Port Area' }}</div>
            <div v-if="berth.hourlyRate" class="text-xs text-slate-500">${{ berth.hourlyRate }}/hr</div>
            <div class="text-xs text-slate-500">Available Now</div>
          </div>
        </div>
      </div>
      
      <!-- Pagination Controls -->
      <div v-if="!loading && !error && berths.length > 0" class="mt-6 pt-4 border-t border-slate-200">
        <div class="flex items-center justify-between">
          <p class="text-sm text-slate-600">
            Showing {{ (currentPage - 1) * berthsPerPage + 1 }} to {{ Math.min(currentPage * berthsPerPage, berths.length) }} of {{ berths.length }} available berths
          </p>
          <div class="flex items-center gap-2">
            <button 
              @click="previousPage"
              :disabled="currentPage === 1"
              class="px-3 py-1 text-sm font-medium text-slate-600 hover:text-slate-900 disabled:text-slate-400 disabled:cursor-not-allowed transition-colors"
            >
              Previous
            </button>
            <div class="flex items-center gap-1">
              <button
                v-for="page in visiblePages"
                :key="page"
                @click="goToPage(page)"
                :class="[
                  'px-3 py-1 text-sm font-medium rounded transition-colors',
                  page === currentPage 
                    ? 'bg-emerald-600 text-white' 
                    : 'text-slate-600 hover:text-slate-900'
                ]"
              >
                {{ page }}
              </button>
            </div>
            <button 
              @click="nextPage"
              :disabled="currentPage === totalPages"
              class="px-3 py-1 text-sm font-medium text-slate-600 hover:text-slate-900 disabled:text-slate-400 disabled:cursor-not-allowed transition-colors"
            >
              Next
            </button>
          </div>
        </div>
      </div>
      

    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { Building2, AlertCircle } from 'lucide-vue-next';

interface Berth {
  berthId: number;
  name: string;
  identifier?: string;
  type?: string;
  capacity: number;
  currentLoad: number;
  maxShipLength?: number;
  maxDraft?: number;
  status: string;
  availableServices?: string;
  craneCount?: number;
  hourlyRate?: number;
  priority?: string;
  notes?: string;
  portId: number;
  portName: string;
  activeAssignmentCount: number;
}

interface Props {
  berths: Berth[];
  loading?: boolean;
  error?: string | null;
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  error: null
});

defineEmits<{
  retry: [];
}>();

// Pagination state
const currentPage = ref(1)
const berthsPerPage = 10

// Computed properties for pagination
const totalPages = computed(() => Math.ceil(props.berths.length / berthsPerPage))

const paginatedBerths = computed(() => {
  const start = (currentPage.value - 1) * berthsPerPage
  const end = start + berthsPerPage
  return props.berths.slice(start, end)
})

const visiblePages = computed(() => {
  const maxVisiblePages = 5
  const pages = []
  const start = Math.max(1, currentPage.value - Math.floor(maxVisiblePages / 2))
  const end = Math.min(totalPages.value, start + maxVisiblePages - 1)
  
  for (let i = start; i <= end; i++) {
    pages.push(i)
  }
  
  return pages
})

// Pagination methods
const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
  }
}

const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
  }
}

const previousPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

// Utility methods
const getBerthDisplayNumber = (berth: Berth): string => {
  if (berth.identifier) {
    return berth.identifier.slice(-3)
  }
  return berth.berthId.toString().padStart(3, '0')
}

const formatServices = (services?: string): string => {
  if (!services) return ''
  
  const serviceList = services.split(',').map(s => s.trim())
  if (serviceList.length <= 2) {
    return serviceList.join(', ')
  }
  return `${serviceList.slice(0, 2).join(', ')} +${serviceList.length - 2} more`
}

const getStatusColor = (status: string): string => {
  const statusColors: Record<string, string> = {
    "Available": "bg-emerald-100 text-emerald-800 border-emerald-200",
    "Free": "bg-green-100 text-green-800 border-green-200",
    "Ready": "bg-blue-100 text-blue-800 border-blue-200",
    "Occupied": "bg-orange-100 text-orange-800 border-orange-200",
    "Maintenance": "bg-red-100 text-red-800 border-red-200",
    "Reserved": "bg-purple-100 text-purple-800 border-purple-200",
    "Out of Service": "bg-gray-100 text-gray-800 border-gray-200"
  }
  return statusColors[status] || "bg-gray-100 text-gray-800"
}
</script>

<style scoped>
@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-slideIn {
  animation: slideIn 0.6s ease-out forwards;
  opacity: 0;
}
</style>