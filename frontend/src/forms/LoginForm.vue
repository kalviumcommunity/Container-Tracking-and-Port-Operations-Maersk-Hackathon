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
          <Ship :size="18" class="text-white sm:w-5 sm:h-5" />
        </div>
        <div class="min-w-0">
          <h2 class="text-lg sm:text-xl font-bold text-slate-900">Sign In</h2>
          <p class="text-xs sm:text-sm text-slate-600 hidden sm:block">Access your PortTrack dashboard</p>
        </div>
      </div>
    </div>

    <!-- Form Content -->
    <div class="overflow-y-auto max-h-[calc(95vh-64px)] sm:max-h-[calc(95vh-80px)]">
      <form @submit.prevent="handleSubmit" class="p-4 sm:p-6 space-y-4 sm:space-y-6">
        <!-- Username Field -->
        <div>
          <label for="username" class="block text-sm font-medium text-slate-700 mb-2">
            Username
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
              placeholder="Enter your username"
              :class="{ 'border-red-300 focus:ring-red-500 focus:border-red-500': errors.username }"
            />
          </div>
          <p v-if="errors.username" class="mt-1 text-sm text-red-600">{{ errors.username }}</p>
        </div>

        <!-- Password Field -->
        <div>
          <label for="password" class="block text-sm font-medium text-slate-700 mb-2">
            Password
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
              placeholder="Enter your password"
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

        <!-- Remember Me -->
        <div class="flex items-center justify-between">
          <div class="flex items-center">
            <input
              id="remember"
              v-model="form.rememberMe"
              type="checkbox"
              class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-slate-300 rounded"
            />
            <label for="remember" class="ml-2 block text-sm text-slate-700">
              Remember me
            </label>
          </div>
          <button type="button" class="text-sm text-blue-600 hover:text-blue-500">
            Forgot password?
          </button>
        </div>

        <!-- Submit Button -->
        <button
          type="submit"
          :disabled="isLoading"
          class="w-full flex justify-center items-center gap-2 py-3 px-4 border border-transparent rounded-lg shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          <Loader2 v-if="isLoading" :size="20" class="animate-spin" />
          <LogIn v-else :size="20" />
          {{ isLoading ? 'Signing in...' : 'Sign in' }}
        </button>

        <!-- Register Link -->
        <div class="text-center">
          <p class="text-sm text-slate-600">
            Don't have an account?
            <button type="button" @click="$emit('show-register')" class="text-blue-600 hover:text-blue-500 font-medium">
              Sign up here
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
import { Ship, User, Lock, LogIn, Loader2, AlertTriangle, CheckCircle, ArrowLeft } from 'lucide-vue-next'
import { authApi } from '../services/api'

const emit = defineEmits(['login-success', 'show-register', 'cancel'])

interface LoginForm {
  username: string
  password: string
  rememberMe: boolean
}

const form = reactive<LoginForm>({
  username: '',
  password: '',
  rememberMe: false
})

const errors = reactive<Partial<Record<keyof LoginForm, string>>>({})
const showPassword = ref(false)
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

const validate = (): boolean => {
  Object.keys(errors).forEach(key => delete errors[key as keyof LoginForm])
  
  if (!form.username.trim()) {
    errors.username = 'Username is required'
    return false
  }
  
  if (!form.password.trim()) {
    errors.password = 'Password is required'
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
    // Get registered users from localStorage
    const registeredUsers = JSON.parse(localStorage.getItem('registered_users') || '[]')
    
    // Find user with matching username
    const user = registeredUsers.find(u => u.username === form.username)
    
    if (!user) {
      errorMessage.value = 'Username not found. Please check your username or sign up first.'
      return
    }
    
    // Check password
    if (user.password !== form.password) {
      errorMessage.value = 'Invalid password. Please try again.'
      return
    }
    
    // Successful authentication
    successMessage.value = 'Login successful! Redirecting...'
    
    // Save current user session to localStorage
    const currentUser = {
      id: user.id,
      username: user.username,
      email: user.email,
      roles: user.roles,
      isAdmin: user.isAdmin || false,
      loginTime: new Date().toISOString()
    }
    
    localStorage.setItem('current_user', JSON.stringify(currentUser))
    
    // Update last login time in registered users
    const updatedUsers = registeredUsers.map(u => 
      u.id === user.id 
        ? { ...u, lastLoginAt: new Date().toISOString() }
        : u
    )
    localStorage.setItem('registered_users', JSON.stringify(updatedUsers))
    
    setTimeout(() => {
      emit('login-success', currentUser)
    }, 1000)
    
  } catch (error) {
    console.error('Login error:', error)
    errorMessage.value = 'Login failed. Please try again.'
  } finally {
    isLoading.value = false
  }
}
</script>