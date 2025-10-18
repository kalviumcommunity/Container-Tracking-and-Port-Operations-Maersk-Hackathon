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
      <!-- Messages at the top -->
      <div v-if="errorMessage" class="m-4 sm:m-6 mb-0 bg-red-50 border border-red-200 rounded-lg p-4">
        <div class="flex items-center gap-2">
          <AlertTriangle :size="20" class="text-red-600" />
          <p class="text-red-800">{{ errorMessage }}</p>
        </div>
      </div>

      <div v-if="successMessage" class="m-4 sm:m-6 mb-0 bg-green-50 border border-green-200 rounded-lg p-4">
        <div class="flex items-center gap-2">
          <CheckCircle :size="20" class="text-green-600" />
          <p class="text-green-800">{{ successMessage }}</p>
        </div>
      </div>

      <form @submit.prevent="handleSubmit" class="p-4 sm:p-6 space-y-4 sm:space-y-6" :class="{ 'pt-4': errorMessage || successMessage }">
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

        <!-- Full Name Field -->
        <div>
          <label for="fullName" class="block text-sm font-medium text-slate-700 mb-2">
            Full Name *
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <User :size="20" class="text-slate-400" />
            </div>
            <input
              id="fullName"
              v-model="form.fullName"
              type="text"
              required
              class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="Enter your full name"
              :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.fullName }"
            />
          </div>
          <p v-if="errors.fullName" class="mt-1 text-sm text-red-600">{{ errors.fullName }}</p>
        </div>

        <!-- Phone Number Field -->
        <div>
          <label for="phoneNumber" class="block text-sm font-medium text-slate-700 mb-2">
            Phone Number
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Phone :size="20" class="text-slate-400" />
            </div>
            <input
              id="phoneNumber"
              v-model="form.phoneNumber"
              type="tel"
              class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="Enter your phone number (optional)"
              :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.phoneNumber }"
            />
          </div>
          <p v-if="errors.phoneNumber" class="mt-1 text-sm text-red-600">{{ errors.phoneNumber }}</p>
        </div>

        <!-- Department Field -->
        <div>
          <label for="department" class="block text-sm font-medium text-slate-700 mb-2">
            Department
          </label>
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <Building :size="20" class="text-slate-400" />
            </div>
            <input
              id="department"
              v-model="form.department"
              type="text"
              class="w-full pl-10 pr-4 py-3 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="Enter your department (optional)"
              :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.department }"
            />
          </div>
          <p v-if="errors.department" class="mt-1 text-sm text-red-600">{{ errors.department }}</p>
        </div>

        <!-- Security Notice -->
        <div class="bg-blue-50 border border-blue-200 rounded-lg p-4">
          <div class="flex items-center gap-2">
            <div class="p-1 bg-blue-600 rounded">
              <UserPlus :size="16" class="text-white" />
            </div>
            <div>
              <h4 class="font-medium text-blue-900">Account Setup</h4>
              <p class="text-sm text-blue-700">New accounts start with Viewer access. You can request additional permissions after registration.</p>
            </div>
          </div>
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
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { User, Mail, Lock, UserPlus, Building, Phone, Loader2, AlertTriangle, CheckCircle, ArrowLeft, Eye, EyeOff } from 'lucide-vue-next'
import { authApi } from '../services/api'

const emit = defineEmits(['register-success', 'show-login', 'cancel'])

interface RegistrationForm {
  username: string
  email: string
  password: string
  confirmPassword: string
  fullName: string
  phoneNumber: string
  department: string
}

const form = reactive<RegistrationForm>({
  username: '',
  email: '',
  password: '',
  confirmPassword: '',
  fullName: '',
  phoneNumber: '',
  department: ''
})

const errors = reactive<Partial<Record<keyof RegistrationForm, string>>>({})
const showPassword = ref(false)
const showConfirmPassword = ref(false)
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')



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

  if (!form.fullName.trim()) {
    errors.fullName = 'Full name is required'
    return false
  } else if (form.fullName.length < 2 || form.fullName.length > 100) {
    errors.fullName = 'Full name must be between 2 and 100 characters'
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
  
  return true
}

const handleSubmit = async () => {
  if (!validate()) return
  
  isLoading.value = true
  errorMessage.value = ''
  successMessage.value = ''
  
  try {
    // Create registration request - backend will automatically assign Viewer role
    const registerRequest = {
      username: form.username,
      email: form.email,
      password: form.password,
      fullName: form.fullName,
      phoneNumber: form.phoneNumber || undefined,
      department: form.department || undefined
    }
    
    // Use JWT registration API
    const response = await authApi.register(registerRequest)
    
    // Successful registration and auto-login
    successMessage.value = 'Account created successfully! Logging you in...'
    
    // Save user session
    const currentUser = {
      id: response.user.userId,
      username: response.user.username,
      email: response.user.email,
      fullName: response.user.fullName,
      roles: response.user.roles,
      permissions: response.user.permissions,
      loginTime: new Date().toISOString(),
      token: response.token
    }
    
    localStorage.setItem('current_user', JSON.stringify(currentUser))
    localStorage.setItem('auth_token', response.token)
    
    setTimeout(() => {
      emit('register-success', currentUser)
    }, 1500)
    
  } catch (error: any) {
    console.error('Registration error:', error)
    if (error.response?.data?.message) {
      errorMessage.value = error.response.data.message
    } else if (error.response?.status === 400) {
      errorMessage.value = 'Invalid registration data. Please check your information.'
    } else {
      errorMessage.value = 'Registration failed. Please try again.'
    }
  } finally {
    isLoading.value = false
  }
}
</script>
