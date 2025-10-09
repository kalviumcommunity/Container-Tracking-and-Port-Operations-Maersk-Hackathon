<template>
  <div class="bg-white rounded-lg p-6 w-full max-w-md mx-4" @click.stop>
    <div class="flex justify-between items-center mb-4">
      <h3 class="text-lg font-semibold text-slate-900">Account Settings</h3>
      <button 
        @click="$emit('close')" 
        class="text-slate-400 hover:text-slate-600 transition-colors"
      >
        <X class="w-6 h-6" />
      </button>
    </div>

    <form @submit.prevent="updateProfile" class="space-y-4">
      <!-- Full Name -->
      <div>
        <label for="fullName" class="block text-sm font-medium text-slate-700 mb-1">
          Full Name
        </label>
        <input
          id="fullName"
          v-model="form.fullName"
          type="text"
          required
          class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          placeholder="Enter your full name"
        />
      </div>

      <!-- Email -->
      <div>
        <label for="email" class="block text-sm font-medium text-slate-700 mb-1">
          Email Address
        </label>
        <input
          id="email"
          v-model="form.email"
          type="email"
          required
          class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          placeholder="Enter your email address"
        />
      </div>

      <!-- Phone Number -->
      <div>
        <label for="phoneNumber" class="block text-sm font-medium text-slate-700 mb-1">
          Phone Number
        </label>
        <input
          id="phoneNumber"
          v-model="form.phoneNumber"
          type="tel"
          class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          placeholder="Enter your phone number"
        />
      </div>

      <!-- Department -->
      <div>
        <label for="department" class="block text-sm font-medium text-slate-700 mb-1">
          Department
        </label>
        <input
          id="department"
          v-model="form.department"
          type="text"
          class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          placeholder="Enter your department"
        />
      </div>

      <!-- Current Roles (Read-only) -->
      <div>
        <label class="block text-sm font-medium text-slate-700 mb-2">
          Current Roles
        </label>
        <div class="flex flex-wrap gap-2">
          <span 
            v-for="role in currentUser?.roles" 
            :key="role"
            class="inline-block bg-blue-100 text-blue-800 text-sm px-3 py-1 rounded-full"
          >
            {{ role }}
          </span>
        </div>
        <p class="text-xs text-slate-500 mt-1">
          To request additional roles, use "Request Additional Access" from the user menu.
        </p>
      </div>

      <!-- Action Buttons -->
      <div class="flex space-x-3 pt-4">
        <button
          type="button"
          @click="$emit('close')"
          class="flex-1 px-4 py-2 border border-slate-300 text-slate-700 rounded-lg hover:bg-slate-50 transition-colors"
        >
          Cancel
        </button>
        <button
          type="submit"
          :disabled="!isFormValid || loading"
          class="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center justify-center"
        >
          <Loader2 v-if="loading" class="w-4 h-4 animate-spin mr-2" />
          <span v-if="!loading">Update Profile</span>
          <span v-else>Updating...</span>
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { X, AlertTriangle, CheckCircle, Loader2 } from 'lucide-vue-next'
import { authApi } from '../services/api'
import { useToast } from '../composables/useToast.js'

const emit = defineEmits(['close', 'profile-updated'])
const { success: showSuccess, error: showError, warning } = useToast()

const form = ref({
  fullName: '',
  email: '',
  phoneNumber: '',
  department: ''
})

const currentUser = ref(null)
const loading = ref(false)

const isFormValid = computed(() => {
  return form.value.fullName.trim() && 
         form.value.email.trim() &&
         /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.value.email)
})

const loadCurrentUser = async () => {
  try {
    // Get user from localStorage or API
    const userFromStorage = localStorage.getItem('current_user')
    if (userFromStorage) {
      currentUser.value = JSON.parse(userFromStorage)
    } else {
      currentUser.value = await authApi.getCurrentUser()
    }

    if (currentUser.value) {
      // Pre-fill the form
      form.value.fullName = currentUser.value.fullName || ''
      form.value.email = currentUser.value.email || ''
      form.value.phoneNumber = currentUser.value.phoneNumber || ''
      form.value.department = currentUser.value.department || ''
    }
  } catch (err) {
    console.error('Error loading user:', err)
  }
}

const updateProfile = async () => {
  if (!isFormValid.value) {
    warning('Please fill in all required fields with valid information.')
    return
  }

  loading.value = true

  try {
    await authApi.updateProfile({
      fullName: form.value.fullName,
      email: form.value.email,
      phoneNumber: form.value.phoneNumber || undefined,
      department: form.value.department || undefined
    })

    showSuccess('Profile updated successfully! 🎉')
    
    // Update localStorage
    if (currentUser.value) {
      const updatedUser = {
        ...currentUser.value,
        fullName: form.value.fullName,
        email: form.value.email,
        phoneNumber: form.value.phoneNumber,
        department: form.value.department
      }
      localStorage.setItem('current_user', JSON.stringify(updatedUser))
    }

    setTimeout(() => {
      emit('profile-updated')
      emit('close')
    }, 1500)

  } catch (err: any) {
    console.error('Update profile error:', err)
    
    if (err.response?.status === 400) {
      showError('Invalid information provided. Please check your input and try again.')
    } else if (err.response?.status === 409) {
      showError('Email address is already in use by another account.')
    } else if (err.response?.data?.message) {
      showError(err.response.data.message)
    } else {
      showError('Failed to update profile. Please try again.')
    }
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadCurrentUser()
})
</script>