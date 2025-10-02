<template>
  <div class="relative bg-white rounded-xl shadow-2xl w-full max-w-md mx-auto max-h-[95vh] overflow-hidden">
    <!-- Header with Back Button -->
    <div class="sticky top-0 bg-white border-b border-slate-200 px-4 sm:px-6 py-3 sm:py-4 flex items-center gap-3 sm:gap-4 z-10">
      <button
        @click="$emit('cancel')"
        class="p-2 hover:bg-slate-100 rounded-lg transition-colors flex-shrink-0"
        title="Go back"
      >
        <ArrowLeft :size="20" class="text-slate-600" />
      </button>
      <div class="flex items-center gap-2 sm:gap-3 min-w-0">
        <div class="p-2 bg-blue-600 rounded-lg shadow-lg flex-shrink-0">
          <UserPlus :size="18" class="text-white sm:w-5 sm:h-5" />
        </div>
        <div class="min-w-0">
          <h2 class="text-lg sm:text-xl font-bold text-slate-900">Create Account</h2>
          <p class="text-xs sm:text-sm text-slate-600 hidden sm:block">Join PortTrack operations team</p>
        </div>
      </div>
    </div>

    <!-- Form Content -->
    <div class="overflow-y-auto max-h-[calc(95vh-64px)] sm:max-h-[calc(95vh-80px)]">
      <form @submit.prevent="handleSubmit" class="p-4 sm:p-6 space-y-4 sm:space-y-6">
        <!-- Username Field -->
        <div>
          <label for="username" class="block text-sm font-medium text-slate-700 mb-2">
            Username *
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <User :size="20" class="text-slate-400" />
            </div>
            <input
              id="username"
              v-model="form.username"
              type="text"
              required
              class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="Choose a username"
              :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.username }"
            />
          </div>
          <p v-if="errors.username" class="mt-1 text-sm text-red-600">{{ errors.username }}</p>
        </div>

        <!-- Email Field -->
        <div>
          <label for="email" class="block text-sm font-medium text-slate-700 mb-2">
            Email Address *
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Mail :size="20" class="text-slate-400" />
            </div>
            <input
              id="email"
              v-model="form.email"
              type="email"
              required
              class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="Enter your email address"
              :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.email }"
            />
          </div>
          <p v-if="errors.email" class="mt-1 text-sm text-red-600">{{ errors.email }}</p>
        </div>

        <!-- Role Selection -->
        <div>
          <label class="block text-sm font-medium text-slate-700 mb-2">
            Role *
          </label>
          <div class="space-y-2">
            <label
              v-for="role in availableRoles"
              :key="role.id"
              class="flex items-center p-3 border rounded-lg cursor-pointer transition-all"
              :class="{
                'border-blue-500 bg-blue-50': form.roleIds.includes(role.id),
                'border-slate-300 hover:border-slate-400': !form.roleIds.includes(role.id)
              }"
            >
              <input
                type="checkbox"
                :value="role.id"
                v-model="form.roleIds"
                class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-slate-300 rounded"
              />
              <div class="ml-3">
                <div class="font-medium text-slate-900">{{ role.name }}</div>
                <div class="text-sm text-slate-600">{{ role.description }}</div>
              </div>
            </label>
          </div>
          <p v-if="errors.roleIds" class="mt-1 text-sm text-red-600">{{ errors.roleIds }}</p>
        </div>

        <!-- Password Field -->
        <div>
          <label for="password" class="block text-sm font-medium text-slate-700 mb-2">
            Password *
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Lock :size="20" class="text-slate-400" />
            </div>
            <input
              id="password"
              v-model="form.password"
              :type="showPassword ? 'text' : 'password'"
              required
              class="w-full pl-10 pr-12 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="Create a password"
              :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.password }"
            />
            <button
              type="button"
              @click="showPassword = !showPassword"
              class="absolute inset-y-0 right-0 pr-3 flex items-center"
            >
              <EyeOff v-if="showPassword" :size="20" class="text-slate-400 hover:text-slate-600" />
              <Eye v-else :size="20" class="text-slate-400 hover:text-slate-600" />
            </button>
          </div>
          <p v-if="errors.password" class="mt-1 text-sm text-red-600">{{ errors.password }}</p>
        </div>

        <!-- Confirm Password Field -->
        <div>
          <label for="confirmPassword" class="block text-sm font-medium text-slate-700 mb-2">
            Confirm Password *
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Lock :size="20" class="text-slate-400" />
            </div>
            <input
              id="confirmPassword"
              v-model="form.confirmPassword"
              :type="showConfirmPassword ? 'text' : 'password'"
              required
              class="w-full pl-10 pr-12 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="Confirm your password"
              :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.confirmPassword }"
            />
            <button
              type="button"
              @click="showConfirmPassword = !showConfirmPassword"
              class="absolute inset-y-0 right-0 pr-3 flex items-center"
            >
              <EyeOff v-if="showConfirmPassword" :size="20" class="text-slate-400 hover:text-slate-600" />
              <Eye v-else :size="20" class="text-slate-400 hover:text-slate-600" />
            </button>
          </div>
          <p v-if="errors.confirmPassword" class="mt-1 text-sm text-red-600">{{ errors.confirmPassword }}</p>
        </div>

        <!-- Submit Button -->
        <button
          type="submit"
          :disabled="isLoading"
          class="w-full flex justify-center items-center gap-2 py-3 px-4 border border-transparent rounded-lg shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          <Loader2 v-if="isLoading" :size="20" class="animate-spin" />
          <UserPlus v-else :size="20" />
          {{ isLoading ? 'Creating account...' : 'Create Account' }}
        </button>

        <!-- Login Link -->
        <div class="text-center">
          <p class="text-sm text-slate-600">
            Already have an account?
            <button type="button" @click="$emit('show-login')" class="text-blue-600 hover:text-blue-500 font-medium">
              Sign in here
            </button>
          </p>
        </div>
      </form>

      <!-- Messages -->
      <div v-if="errorMessage" class="bg-red-50 border border-red-200 rounded-lg p-4">
        <div class="flex items-center gap-2">
          <AlertTriangle :size="20" class="text-red-600" />
          <p class="text-red-800">{{ errorMessage }}</p>
        </div>
      </div>

      <div v-if="successMessage" class="bg-green-50 border border-green-200 rounded-lg p-4">
        <div class="flex items-center gap-2">
          <CheckCircle :size="20" class="text-green-600" />
          <p class="text-green-800">{{ successMessage }}</p>
        </div>
      </div>
      </div>
    </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { User, Mail, Lock, UserPlus, Building, Phone, Loader2, AlertTriangle, CheckCircle, ArrowLeft } from 'lucide-vue-next'

const emit = defineEmits(['register-success', 'show-login', 'cancel'])

interface RegistrationForm {
  username: string
  email: string
  password: string
  confirmPassword: string
  roleIds: number[]
}

const form = reactive<RegistrationForm>({
  username: '',
  email: '',
  password: '',
  confirmPassword: '',
  roleIds: []
})

const errors = reactive<Partial<Record<keyof RegistrationForm, string>>>({})
const showPassword = ref(false)
const showConfirmPassword = ref(false)
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

const availableRoles = ref([
  { id: 1, name: 'Port Operator', description: 'Basic port operations access' },
  { id: 2, name: 'Port Supervisor', description: 'Supervise port operations and staff' },
  { id: 3, name: 'Port Manager', description: 'Full port management capabilities' },
  { id: 4, name: 'System Admin', description: 'Complete system administration' }
])

const validate = (): boolean => {
  Object.keys(errors).forEach(key => delete errors[key as keyof RegistrationForm])
  
  if (!form.username.trim()) {
    errors.username = 'Username is required'
    return false
  } else if (form.username.length < 3 || form.username.length > 50) {
    errors.username = 'Username must be between 3 and 50 characters'
    return false
  }
  
  if (!form.email.trim()) {
    errors.email = 'Email is required'
    return false
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
    errors.email = 'Invalid email format'
    return false
  }
  
  if (!form.password.trim()) {
    errors.password = 'Password is required'
    return false
  } else if (form.password.length < 8) {
    errors.password = 'Password must be at least 8 characters'
    return false
  } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(form.password)) {
    errors.password = 'Password must contain uppercase, lowercase, and number'
    return false
  }
  
  if (form.password !== form.confirmPassword) {
    errors.confirmPassword = 'Passwords do not match'
    return false
  }
  
  if (form.roleIds.length === 0) {
    errors.roleIds = 'Please select at least one role'
    return false
  }
  
  return true
}

const handleSubmit = async () => {
  if (!validate()) return
  
  isLoading.value = true
  errorMessage.value = ''
  successMessage.value = ''
  
  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 2000))
    
    // Mock successful registration
    const mockUser = {
      id: Date.now(),
      username: form.username,
      email: form.email,
      roleIds: form.roleIds
    }
    
    successMessage.value = 'Account created successfully! You can now sign in.'
    
    setTimeout(() => {
      emit('register-success', mockUser)
    }, 1500)
    
  } catch (error) {
    errorMessage.value = 'Registration failed. Please try again.'
  } finally {
    isLoading.value = false
  }
}
</script>