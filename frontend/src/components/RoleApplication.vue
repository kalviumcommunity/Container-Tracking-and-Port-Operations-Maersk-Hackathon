<template>
  <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
    <div class="flex items-center gap-3 mb-6">
      <div class="p-2 bg-blue-600 rounded-lg">
        <UserPlus :size="20" class="text-white" />
      </div>
      <div>
        <h2 class="text-xl font-semibold text-slate-900">Request Additional Role</h2>
        <p class="text-sm text-slate-600">Apply for additional permissions to expand your access</p>
      </div>
    </div>

    <!-- Available Roles -->
    <div v-if="availableRoles.length > 0" class="space-y-4">
      <div v-for="role in availableRoles" :key="role.name" class="border border-slate-200 rounded-lg p-4 hover:border-blue-300 transition-colors">
        <div class="flex items-start justify-between">
          <div class="flex-1">
            <h3 class="font-medium text-slate-900">{{ role.name }}</h3>
            <p class="text-sm text-slate-600 mt-1">{{ role.description }}</p>
            <div class="flex items-center gap-4 mt-3">
              <span class="text-xs px-2 py-1 bg-slate-100 rounded text-slate-700">
                {{ role.permissions?.length || 0 }} permissions
              </span>
              <span v-if="role.requiresApproval" class="text-xs px-2 py-1 bg-amber-100 text-amber-700 rounded">
                Requires approval
              </span>
            </div>
          </div>
          <button
            @click="applyForRole(role.name)"
            :disabled="isLoading"
            class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            <Loader2 v-if="isLoading" :size="16" class="animate-spin" />
            <span v-else>Apply</span>
          </button>
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
import { UserPlus, Shield, CheckCircle, AlertTriangle, Loader2 } from 'lucide-vue-next'
import { roleApplicationApi } from '../services/api'

interface AvailableRole {
  name: string
  description: string
  permissions: string[]
  requiresApproval: boolean
}

const availableRoles = ref<AvailableRole[]>([])
const isLoading = ref(false)
const successMessage = ref('')
const errorMessage = ref('')

const emit = defineEmits(['application-submitted'])

const loadAvailableRoles = async () => {
  try {
    const roles = await roleApplicationApi.getAvailableRoles()
    availableRoles.value = roles
  } catch (error: any) {
    console.error('Failed to load available roles:', error)
    errorMessage.value = 'Failed to load available roles'
  }
}

const applyForRole = async (roleName: string) => {
  isLoading.value = true
  errorMessage.value = ''
  successMessage.value = ''
  
  try {
    await roleApplicationApi.submitApplication({
      requestedRole: roleName,
      justification: `Requesting ${roleName} role for expanded operational access.`
    })
    
    successMessage.value = `Application for ${roleName} role submitted successfully! It will be reviewed by an administrator.`
    
    // Refresh available roles
    setTimeout(() => {
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