<template>
  <div class="min-h-screen bg-slate-50 py-8">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-slate-900 flex items-center">
          <Shield class="w-8 h-8 mr-3 text-red-600" />
          Admin Panel
        </h1>
        <p class="mt-2 text-slate-600">Manage users, roles, and system settings</p>
      </div>

      <!-- Pending Role Applications -->
      <div class="mb-8">
        <div class="bg-white shadow-sm rounded-xl border border-slate-200 p-6">
          <h2 class="text-xl font-semibold mb-4 flex items-center">
            <UserCheck class="w-5 h-5 mr-2 text-orange-500" />
            Pending Role Applications
            <span v-if="pendingApplications.length > 0" 
                  class="ml-2 bg-orange-100 text-orange-800 text-sm px-2 py-1 rounded-full">
              {{ pendingApplications.length }}
            </span>
          </h2>
          
          <div v-if="loading" class="text-center py-8">
            <Loader2 class="w-8 h-8 animate-spin text-blue-600 mx-auto" />
            <p class="mt-2 text-slate-600">Loading applications...</p>
          </div>
          
          <div v-else-if="pendingApplications.length === 0" class="text-center py-8">
            <FileCheck class="w-12 h-12 text-slate-400 mx-auto mb-3" />
            <p class="text-slate-500">No pending role applications</p>
          </div>
          
          <div v-else class="space-y-4">
            <div v-for="application in pendingApplications" :key="application.applicationId"
                 class="border border-slate-200 rounded-lg p-4 hover:bg-slate-50 transition-colors">
              <div class="flex justify-between items-start">
                <div class="flex-1">
                  <div class="flex items-center space-x-3 mb-2">
                    <div class="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center">
                      <User class="w-4 h-4 text-white" />
                    </div>
                    <div>
                      <h3 class="font-medium text-slate-900">{{ application.applicantName }}</h3>
                      <p class="text-sm text-slate-600">Requesting: <span class="font-semibold text-blue-600">{{ application.requestedRole }}</span></p>
                    </div>
                  </div>
                  <p class="text-sm text-slate-700 mb-3 ml-11">{{ application.justification }}</p>
                  <div class="flex items-center space-x-4 text-xs text-slate-500 ml-11">
                    <span class="flex items-center">
                      <Calendar class="w-3 h-3 mr-1" />
                      {{ formatDate(application.submittedAt) }}
                    </span>
                  </div>
                </div>
                <div class="flex space-x-2 ml-4">
                  <button 
                    @click="approveApplication(application.applicationId)"
                    :disabled="processing"
                    class="bg-green-600 text-white px-3 py-1.5 rounded-lg text-sm hover:bg-green-700 disabled:opacity-50 flex items-center space-x-1 transition-colors"
                  >
                    <Check class="w-4 h-4" />
                    <span>Approve</span>
                  </button>
                  <button 
                    @click="rejectApplication(application.applicationId)"
                    :disabled="processing"
                    class="bg-red-600 text-white px-3 py-1.5 rounded-lg text-sm hover:bg-red-700 disabled:opacity-50 flex items-center space-x-1 transition-colors"
                  >
                    <X class="w-4 h-4" />
                    <span>Reject</span>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- System Statistics -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-200">
          <div class="flex items-center">
            <div class="p-3 bg-blue-100 rounded-lg">
              <Users class="w-6 h-6 text-blue-600" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-slate-600">Total Users</p>
              <p class="text-2xl font-semibold text-slate-900">{{ stats.users || '...' }}</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-200">
          <div class="flex items-center">
            <div class="p-3 bg-green-100 rounded-lg">
              <Container class="w-6 h-6 text-green-600" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-slate-600">Containers</p>
              <p class="text-2xl font-semibold text-slate-900">{{ stats.containers || '...' }}</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-200">
          <div class="flex items-center">
            <div class="p-3 bg-purple-100 rounded-lg">
              <Ship class="w-6 h-6 text-purple-600" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-slate-600">Ships</p>
              <p class="text-2xl font-semibold text-slate-900">{{ stats.ships || '...' }}</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white p-6 rounded-xl shadow-sm border border-slate-200">
          <div class="flex items-center">
            <div class="p-3 bg-orange-100 rounded-lg">
              <Anchor class="w-6 h-6 text-orange-600" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-slate-600">Berths</p>
              <p class="text-2xl font-semibold text-slate-900">{{ stats.berths || '...' }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Activity -->
      <div class="bg-white shadow-sm rounded-xl border border-slate-200 p-6">
        <h2 class="text-xl font-semibold mb-4 flex items-center">
          <Activity class="w-5 h-5 mr-2 text-blue-500" />
          Recent Role Applications
        </h2>
        
        <div v-if="allApplications.length === 0" class="text-center py-8">
          <FileText class="w-12 h-12 text-slate-400 mx-auto mb-3" />
          <p class="text-slate-500">No role applications yet</p>
        </div>
        
        <div v-else class="space-y-3">
          <div v-for="application in allApplications.slice(0, 10)" :key="application.applicationId"
               class="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
            <div class="flex items-center space-x-3">
              <div class="w-6 h-6 bg-slate-600 rounded-full flex items-center justify-center">
                <User class="w-3 h-3 text-white" />
              </div>
              <div>
                <p class="text-sm font-medium text-slate-900">{{ application.applicantName }}</p>
                <p class="text-xs text-slate-600">{{ application.requestedRole }}</p>
              </div>
            </div>
            <div class="flex items-center space-x-2">
              <span 
                :class="{
                  'bg-yellow-100 text-yellow-800': application.status === 'Pending',
                  'bg-green-100 text-green-800': application.status === 'Approved', 
                  'bg-red-100 text-red-800': application.status === 'Rejected'
                }"
                class="px-2 py-1 rounded-full text-xs font-medium"
              >
                {{ application.status }}
              </span>
              <span class="text-xs text-slate-500">{{ formatDate(application.submittedAt) }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Success/Error Messages -->
      <div v-if="successMessage" class="fixed bottom-4 right-4 bg-green-600 text-white px-4 py-2 rounded-lg shadow-lg">
        <div class="flex items-center space-x-2">
          <CheckCircle class="w-4 h-4" />
          <span>{{ successMessage }}</span>
        </div>
      </div>
      
      <div v-if="errorMessage" class="fixed bottom-4 right-4 bg-red-600 text-white px-4 py-2 rounded-lg shadow-lg">
        <div class="flex items-center space-x-2">
          <AlertTriangle class="w-4 h-4" />
          <span>{{ errorMessage }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { 
  Shield, 
  UserCheck, 
  Users, 
  Container, 
  Ship, 
  Anchor, 
  User,
  Calendar,
  Check,
  X,
  Activity,
  FileText,
  FileCheck,
  Loader2,
  CheckCircle,
  AlertTriangle
} from 'lucide-vue-next'
import { roleApplicationApi } from '../services/api'

interface SystemStats {
  users: number
  containers: number
  ships: number
  berths: number
}

const pendingApplications = ref([])
const allApplications = ref([])
const stats = ref<SystemStats>({ users: 0, containers: 0, ships: 0, berths: 0 })
const loading = ref(true)
const processing = ref(false)
const successMessage = ref('')
const errorMessage = ref('')

const loadData = async () => {
  loading.value = true
  try {
    // Load pending applications
    const pending = await roleApplicationApi.getPendingApplications()
    pendingApplications.value = pending

    // Load all applications
    const all = await roleApplicationApi.getAllApplications()
    allApplications.value = all

    // Load system stats (mock for now - you can implement this API later)
    stats.value = {
      users: Math.floor(Math.random() * 50) + 10,
      containers: Math.floor(Math.random() * 500) + 100,
      ships: Math.floor(Math.random() * 20) + 5,
      berths: Math.floor(Math.random() * 30) + 10
    }
  } catch (error) {
    console.error('Error loading admin data:', error)
    showError('Failed to load admin data')
  } finally {
    loading.value = false
  }
}

const approveApplication = async (applicationId: number) => {
  processing.value = true
  try {
    await roleApplicationApi.reviewApplication(applicationId, 'Approved', 'Application approved by admin')
    await loadData() // Refresh
    showSuccess('Application approved successfully!')
  } catch (error) {
    console.error('Error approving application:', error)
    showError('Failed to approve application')
  } finally {
    processing.value = false
  }
}

const rejectApplication = async (applicationId: number) => {
  processing.value = true
  try {
    await roleApplicationApi.reviewApplication(applicationId, 'Rejected', 'Application rejected by admin')
    await loadData() // Refresh
    showSuccess('Application rejected')
  } catch (error) {
    console.error('Error rejecting application:', error)
    showError('Failed to reject application')
  } finally {
    processing.value = false
  }
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const showSuccess = (message: string) => {
  successMessage.value = message
  setTimeout(() => {
    successMessage.value = ''
  }, 3000)
}

const showError = (message: string) => {
  errorMessage.value = message
  setTimeout(() => {
    errorMessage.value = ''
  }, 3000)
}

onMounted(() => {
  loadData()
})
</script>