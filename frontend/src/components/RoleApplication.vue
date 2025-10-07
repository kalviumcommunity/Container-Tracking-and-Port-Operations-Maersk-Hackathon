<template>
  <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6 max-w-2xl mx-auto">
    <div class="flex items-cent      // Roles loaded successfullyr gap-3 mb-6">
      <div class="p-2 bg-blue-600 rounded-lg">
        <UserPlus :size="20" class="text-white" />
      </div>
      <div>
        <h2 class="text-xl font-semibold text-slate-900">Request Additional Role</h2>
        <p class="text-sm text-slate-600">Apply for additional permissions to expand your access</p>
      </div>
    </div>

    <!-- Application Form -->
    <div v-if="!showRoleSelection && selectedRole" class="space-y-4">
      <div class="border border-slate-200 rounded-lg p-4 bg-slate-50">
        <h3 class="font-medium text-slate-900 mb-2">Requesting: {{ selectedRole.roleName }}</h3>
        <p class="text-sm text-slate-600">{{ selectedRole.description }}</p>
      </div>

      <div>
        <label for="justification" class="block text-sm font-medium text-slate-700 mb-2">
          Justification <span class="text-red-500">*</span>
        </label>
        <textarea
          id="justification"
          v-model="justification"
          rows="4"
          class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 resize-none"
          placeholder="Please explain why you need this role and how you plan to use the additional permissions..."
          maxlength="1000"
        ></textarea>
        <div class="text-xs text-slate-500 mt-1">{{ justification.length }}/1000 characters</div>
      </div>

      <div class="flex gap-3 pt-4">
        <button
          @click="submitApplication"
          :disabled="isLoading || !justification.trim()"
          class="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center justify-center gap-2"
        >
          <Loader2 v-if="isLoading" :size="16" class="animate-spin" />
          <span>Submit Application</span>
        </button>
        <button
          @click="cancelApplication"
          :disabled="isLoading"
          class="px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 transition-colors"
        >
          Cancel
        </button>
      </div>
    </div>

    <!-- Available Roles Selection -->
    <div v-else-if="availableRoles.length > 0" class="space-y-4">
      <div class="text-sm text-slate-600 mb-4">
        Select a role to request additional access permissions:
      </div>
      
      <div v-for="role in availableRoles" :key="role.roleName" 
           :class="[
             'border rounded-lg p-4 transition-colors',
             role.canApply ? 'border-slate-200 hover:border-blue-300 cursor-pointer' : 'border-gray-300 bg-gray-50 cursor-not-allowed opacity-60'
           ]"
           @click="role.canApply && selectRole(role)">
        <div class="flex items-start justify-between">
          <div class="flex-1">
            <h3 class="font-medium text-slate-900">{{ role.roleName }}</h3>
            <p class="text-sm text-slate-600 mt-1">{{ role.description }}</p>
            <div class="flex items-center gap-4 mt-3">
              <span v-if="!role.canApply" class="text-xs px-2 py-1 bg-gray-100 rounded text-gray-700">
                {{ role.reasonCannotApply }}
              </span>
              <span v-else class="text-xs px-2 py-1 bg-green-100 text-green-800 rounded">
                Available to request
              </span>
            </div>
          </div>
          <div v-if="role.canApply" class="flex items-center text-blue-600">
            <ChevronRight :size="20" />
          </div>
        </div>
      </div>
    </div>

    <!-- No Available Roles -->
    <div v-else class="text-center py-8">
      <Shield :size="48" class="text-slate-400 mx-auto mb-4" />
      <h3 class="text-lg font-medium text-slate-900 mb-2">All Roles Assigned</h3>
      <p class="text-slate-600">You have access to all available roles or have pending applications.</p>
    </div>

    <!-- Success Message -->
    <div v-if="successMessage" class="mt-4 p-4 bg-green-50 border border-green-200 rounded-lg">
      <div class="flex items-center gap-2">
        <CheckCircle :size="20" class="text-green-600" />
        <p class="text-green-800">{{ successMessage }}</p>
      </div>
    </div>

    <!-- Error Message -->
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
import { UserPlus, Shield, CheckCircle, AlertTriangle, Loader2, ChevronRight } from 'lucide-vue-next'
import { roleApplicationApi } from '../services/api'

interface AvailableRole {
  roleName: string
  description: string
  canApply: boolean
  reasonCannotApply?: string
}

const availableRoles = ref<AvailableRole[]>([])
const selectedRole = ref<AvailableRole | null>(null)
const showRoleSelection = ref(true)
const justification = ref('')
const isLoading = ref(false)
const successMessage = ref('')
const errorMessage = ref('')

const emit = defineEmits(['application-submitted', 'close'])

const loadAvailableRoles = async () => {
  try {
    // Load available roles
    const roles = await roleApplicationApi.getAvailableRoles()
    console.log('Loaded roles:', roles)
    availableRoles.value = roles
  } catch (error: any) {
    console.error('Failed to load available roles:', error)
    console.error('Error details:', error.response?.data || error.message)
    errorMessage.value = 'Failed to load available roles. Please make sure you are logged in.'
  }
}

const selectRole = (role: AvailableRole) => {
  if (!role.canApply) return
  
  selectedRole.value = role
  showRoleSelection.value = false
  justification.value = `I am requesting the ${role.roleName} role to expand my operational capabilities. This role will allow me to contribute more effectively to our team's objectives.`
}

const cancelApplication = () => {
  selectedRole.value = null
  showRoleSelection.value = true
  justification.value = ''
  errorMessage.value = ''
  successMessage.value = ''
}

const submitApplication = async () => {
  if (!selectedRole.value || !justification.value.trim()) {
    errorMessage.value = 'Please provide a justification for your role request'
    return
  }

  isLoading.value = true
  errorMessage.value = ''
  successMessage.value = ''
  
  try {
    await roleApplicationApi.submitApplication({
      requestedRole: selectedRole.value.roleName,
      justification: justification.value.trim()
    })
    
    successMessage.value = `Application for ${selectedRole.value.roleName} role submitted successfully! It will be reviewed by an administrator.`
    
    // Reset form and emit success
    setTimeout(() => {
      selectedRole.value = null
      showRoleSelection.value = true
      justification.value = ''
      loadAvailableRoles()
      emit('application-submitted')
    }, 2000)
    
  } catch (error: any) {
    console.error('Failed to submit application:', error)
    errorMessage.value = error.response?.data?.message || 'Failed to submit application'
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  loadAvailableRoles()
})
</script>