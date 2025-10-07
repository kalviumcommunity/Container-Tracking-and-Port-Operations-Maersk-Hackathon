<template>
  <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
    <div class="flex items-center gap-3 mb-6">
      <div class="p-2 bg-slate-600 rounded-lg">
        <FileText :size="20" class="text-white" />
      </div>
      <div>
        <h2 class="text-xl font-semibold text-slate-900">My Role Applications</h2>
        <p class="text-sm text-slate-600">Track the status of your role requests</p>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center py-8">
      <Loader2 :size="32" class="animate-spin text-blue-600" />
    </div>

    <!-- Applications List -->
    <div v-else-if="applications.length > 0" class="space-y-4">
      <div
        v-for="application in applications"
        :key="application.applicationId"
        class="border border-slate-200 rounded-lg p-4 hover:border-slate-300 transition-colors"
      >
        <div class="flex items-start justify-between">
          <div class="flex-1">
            <div class="flex items-center gap-3 mb-2">
              <h3 class="font-medium text-slate-900">{{ application.requestedRole }}</h3>
              <span
                :class="{
                  'bg-yellow-100 text-yellow-800': application.status === 'Pending',
                  'bg-green-100 text-green-800': application.status === 'Approved',
                  'bg-red-100 text-red-800': application.status === 'Rejected',
                  'bg-gray-100 text-gray-800': application.status === 'Cancelled'
                }"
                class="px-2 py-1 rounded text-xs font-medium"
              >
                {{ application.status }}
              </span>
            </div>
            
            <p class="text-sm text-slate-600 mb-3">{{ application.justification }}</p>
            
            <div class="flex items-center gap-4 text-xs text-slate-500">
              <span class="flex items-center gap-1">
                <Calendar :size="14" />
                Submitted {{ formatDate(application.submittedAt) }}
              </span>
              <span v-if="application.reviewedAt" class="flex items-center gap-1">
                <Clock :size="14" />
                Reviewed {{ formatDate(application.reviewedAt) }}
              </span>
            </div>
            
            <div v-if="application.reviewNotes" class="mt-3 p-3 bg-slate-50 rounded border-l-4 border-slate-300">
              <p class="text-sm text-slate-700">
                <span class="font-medium">Review Notes:</span>
                {{ application.reviewNotes }}
              </p>
              <p v-if="application.reviewerName" class="text-xs text-slate-500 mt-1">
                - {{ application.reviewerName }}
              </p>
            </div>
          </div>
          
          <div class="ml-4">
            <button
              v-if="application.status === 'Pending'"
              @click="cancelApplication(application.applicationId)"
              :disabled="isCancelling"
              class="px-3 py-1 text-sm text-red-600 hover:bg-red-50 rounded border border-red-200 hover:border-red-300 transition-colors disabled:opacity-50"
            >
              <Loader2 v-if="isCancelling" :size="14" class="animate-spin" />
              <span v-else>Cancel</span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- No Applications -->
    <div v-else class="text-center py-8">
      <FileText :size="48" class="text-slate-400 mx-auto mb-4" />
      <h3 class="text-lg font-medium text-slate-900 mb-2">No Applications Yet</h3>
      <p class="text-slate-600">You haven't submitted any role applications yet.</p>
    </div>

    <!-- Success/Error Messages -->
    <div v-if="successMessage" class="mt-4 p-4 bg-green-50 border border-green-200 rounded-lg">
      <div class="flex items-center gap-2">
        <CheckCircle :size="20" class="text-green-600" />
        <p class="text-green-800">{{ successMessage }}</p>
      </div>
    </div>

    <div v-if="errorMessage" class="mt-4 p-4 bg-red-50 border border-red-200 rounded-lg">
      <div class="flex items-center gap-2">
        <AlertTriangle :size="20" class="text-red-600" />
        <p class="text-red-800">{{ errorMessage }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { FileText, Loader2, Calendar, Clock, CheckCircle, AlertTriangle } from 'lucide-vue-next'
import { roleApplicationApi, type RoleApplication } from '../services/api'

const applications = ref<RoleApplication[]>([])
const isLoading = ref(false)
const isCancelling = ref(false)
const successMessage = ref('')
const errorMessage = ref('')

const emit = defineEmits(['application-cancelled'])

const loadApplications = async () => {
  isLoading.value = true
  try {
    applications.value = await roleApplicationApi.getMyApplications()
  } catch (error: any) {
    console.error('Failed to load applications:', error)
    errorMessage.value = 'Failed to load your applications'
  } finally {
    isLoading.value = false
  }
}

const cancelApplication = async (applicationId: number) => {
  isCancelling.value = true
  errorMessage.value = ''
  successMessage.value = ''
  
  try {
    await roleApplicationApi.cancelApplication(applicationId)
    successMessage.value = 'Application cancelled successfully'
    
    // Refresh the list
    setTimeout(() => {
      loadApplications()
      emit('application-cancelled')
    }, 1500)
    
  } catch (error: any) {
    console.error('Failed to cancel application:', error)
    errorMessage.value = error.response?.data?.message || 'Failed to cancel application'
  } finally {
    isCancelling.value = false
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

// Expose method for parent component to refresh
defineExpose({
  refresh: loadApplications
})

onMounted(() => {
  loadApplications()
})
</script>