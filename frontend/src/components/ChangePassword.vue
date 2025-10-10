<template>
  <div class="bg-white rounded-lg p-6 w-full max-w-md mx-4" @click.stop>
    <div class="flex justify-between items-center mb-4">
      <h3 class="text-lg font-semibold text-slate-900">Change Password</h3>
      <button 
        @click="$emit('close')" 
        class="text-slate-400 hover:text-slate-600 transition-colors"
      >
        <X class="w-6 h-6" />
      </button>
    </div>

    <form @submit.prevent="changePassword" class="space-y-4">
      <!-- Current Password -->
      <div>
        <label for="currentPassword" class="block text-sm font-medium text-slate-700 mb-1">
          Current Password
        </label>
        <div class="relative">
          <input
            id="currentPassword"
            v-model="form.currentPassword"
            :type="showCurrentPassword ? 'text' : 'password'"
            required
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 pr-10"
            placeholder="Enter current password"
          />
          <button
            type="button"
            @click="showCurrentPassword = !showCurrentPassword"
            class="absolute right-3 top-2.5 text-slate-400 hover:text-slate-600"
          >
            <Eye v-if="!showCurrentPassword" class="w-5 h-5" />
            <EyeOff v-else class="w-5 h-5" />
          </button>
        </div>
      </div>

      <!-- New Password -->
      <div>
        <label for="newPassword" class="block text-sm font-medium text-slate-700 mb-1">
          New Password
        </label>
        <div class="relative">
          <input
            id="newPassword"
            v-model="form.newPassword"
            :type="showNewPassword ? 'text' : 'password'"
            required
            minlength="6"
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 pr-10"
            placeholder="Enter new password"
          />
          <button
            type="button"
            @click="showNewPassword = !showNewPassword"
            class="absolute right-3 top-2.5 text-slate-400 hover:text-slate-600"
          >
            <Eye v-if="!showNewPassword" class="w-5 h-5" />
            <EyeOff v-else class="w-5 h-5" />
          </button>
        </div>
        <p class="text-xs text-slate-500 mt-1">Password must be at least 6 characters long</p>
      </div>

      <!-- Confirm New Password -->
      <div>
        <label for="confirmPassword" class="block text-sm font-medium text-slate-700 mb-1">
          Confirm New Password
        </label>
        <div class="relative">
          <input
            id="confirmPassword"
            v-model="form.confirmPassword"
            :type="showConfirmPassword ? 'text' : 'password'"
            required
            class="w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 pr-10"
            placeholder="Confirm new password"
          />
          <button
            type="button"
            @click="showConfirmPassword = !showConfirmPassword"
            class="absolute right-3 top-2.5 text-slate-400 hover:text-slate-600"
          >
            <Eye v-if="!showConfirmPassword" class="w-5 h-5" />
            <EyeOff v-else class="w-5 h-5" />
          </button>
        </div>
        <p v-if="form.newPassword && form.confirmPassword && form.newPassword !== form.confirmPassword" 
           class="text-xs text-red-500 mt-1">
          Passwords do not match
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
          <span v-if="!loading">Change Password</span>
          <span v-else>Changing...</span>
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { Eye, EyeOff, X, Loader2 } from 'lucide-vue-next'
import { authApi } from '../services/api'
import { useToast } from '../composables/useToast.js'

const emit = defineEmits(['close', 'password-changed'])
const { success: showSuccess, error: showError, warning } = useToast()

const form = ref({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const showCurrentPassword = ref(false)
const showNewPassword = ref(false) 
const showConfirmPassword = ref(false)
const loading = ref(false)

const isFormValid = computed(() => {
  return form.value.currentPassword &&
         form.value.newPassword &&
         form.value.confirmPassword &&
         form.value.newPassword === form.value.confirmPassword &&
         form.value.newPassword.length >= 6
})

const changePassword = async () => {
  if (!isFormValid.value) {
    warning('Please fill in all fields correctly before submitting.')
    return
  }

  loading.value = true

  try {
    await authApi.changePassword({
      currentPassword: form.value.currentPassword,
      newPassword: form.value.newPassword
    })

    showSuccess('Password changed successfully! 🎉')
    
    // Reset form
    form.value = {
      currentPassword: '',
      newPassword: '',
      confirmPassword: ''
    }
    
    setTimeout(() => {
      emit('password-changed')
      emit('close')
    }, 1500)

  } catch (err: any) {
    if (err.response?.status === 401) {
      showError('Current password is incorrect. Please try again.')
    } else if (err.response?.status === 400) {
      showError('Invalid password format. Password must be at least 6 characters long.')
    } else if (err.response?.data?.message) {
      showError(err.response.data.message)
    } else {
      showError('Failed to change password. Please try again.')
    }
  } finally {
    loading.value = false
  }
}
</script>