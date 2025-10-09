<template>
  <div class="min-h-screen bg-slate-100 p-8">
    <div class="max-w-6xl mx-auto">
      <h1 class="text-3xl font-bold text-slate-900 mb-8">Form Popup Tests & Toast Notifications</h1>
      
      <!-- Toast Notification Tests -->
      <div class="bg-white p-6 rounded-lg shadow mb-8">
        <h2 class="text-2xl font-semibold mb-4 flex items-center">
          <Activity class="w-6 h-6 mr-2 text-blue-600" />
          Toast Notification System
        </h2>
        
        <!-- Basic Toast Types -->
        <div class="mb-6">
          <h3 class="text-lg font-medium mb-3">Basic Toast Types</h3>
          <div class="grid grid-cols-2 md:grid-cols-4 gap-3">
            <button
              @click="showSuccessToast"
              class="flex items-center justify-center gap-2 px-3 py-2 bg-green-600 text-white rounded hover:bg-green-700 text-sm"
            >
              <CheckCircle :size="16" />
              Success
            </button>
            <button
              @click="showErrorToast"
              class="flex items-center justify-center gap-2 px-3 py-2 bg-red-600 text-white rounded hover:bg-red-700 text-sm"
            >
              <AlertCircle :size="16" />
              Error
            </button>
            <button
              @click="showWarningToast"
              class="flex items-center justify-center gap-2 px-3 py-2 bg-orange-600 text-white rounded hover:bg-orange-700 text-sm"
            >
              <AlertTriangle :size="16" />
              Warning
            </button>
            <button
              @click="showInfoToast"
              class="flex items-center justify-center gap-2 px-3 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 text-sm"
            >
              <Info :size="16" />
              Info
            </button>
          </div>
        </div>

        <!-- Advanced Features -->
        <div class="mb-6">
          <h3 class="text-lg font-medium mb-3">Advanced Features</h3>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-3">
            <button
              @click="showPersistentToast"
              class="px-3 py-2 bg-purple-600 text-white rounded hover:bg-purple-700 text-sm"
            >
              Persistent Toast
            </button>
            <button
              @click="showConfirmToast"
              class="px-3 py-2 bg-yellow-600 text-white rounded hover:bg-yellow-700 text-sm"
            >
              Confirmation Toast
            </button>
            <button
              @click="clearAllToasts"
              class="px-3 py-2 bg-slate-600 text-white rounded hover:bg-slate-700 text-sm"
            >
              Clear All Toasts
            </button>
          </div>
        </div>

        <!-- Real-world Examples -->
        <div>
          <h3 class="text-lg font-medium mb-3">Real-world Examples</h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-3">
            <button
              @click="simulateLoginSuccess"
              class="px-3 py-2 bg-green-100 text-green-800 rounded hover:bg-green-200 border border-green-300 text-sm"
            >
              Login Success
            </button>
            <button
              @click="simulateApiError"
              class="px-3 py-2 bg-red-100 text-red-800 rounded hover:bg-red-200 border border-red-300 text-sm"
            >
              API Error
            </button>
          </div>
        </div>
      </div>
      
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <!-- Container Form Test -->
        <div class="bg-white p-6 rounded-lg shadow">
          <h3 class="text-lg font-semibold mb-4">Container Form</h3>
          <button
            @click="showContainerForm = true"
            class="w-full px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          >
            Open Container Form
          </button>
        </div>

        <!-- Berth Form Test -->
        <div class="bg-white p-6 rounded-lg shadow">
          <h3 class="text-lg font-semibold mb-4">Berth Form</h3>
          <button
            @click="showBerthForm = true"
            class="w-full px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700"
          >
            Open Berth Form
          </button>
        </div>

        <!-- Berth Assignment Form Test -->
        <div class="bg-white p-6 rounded-lg shadow">
          <h3 class="text-lg font-semibold mb-4">Berth Assignment Form</h3>
          <button
            @click="showBerthAssignmentForm = true"
            class="w-full px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700"
          >
            Open Assignment Form
          </button>
        </div>

        <!-- Ship Form Test -->
        <div class="bg-white p-6 rounded-lg shadow">
          <h3 class="text-lg font-semibold mb-4">Ship Form</h3>
          <button
            @click="showShipForm = true"
            class="w-full px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          >
            Open Ship Form
          </button>
        </div>

        <!-- Login Form Test -->
        <div class="bg-white p-6 rounded-lg shadow">
          <h3 class="text-lg font-semibold mb-4">Login Form</h3>
          <button
            @click="showLoginForm = true"
            class="w-full px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700"
          >
            Open Login Form
          </button>
        </div>

        <!-- Registration Form Test -->
        <div class="bg-white p-6 rounded-lg shadow">
          <h3 class="text-lg font-semibold mb-4">Registration Form</h3>
          <button
            @click="showRegistrationForm = true"
            class="w-full px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          >
            Open Registration Form
          </button>
        </div>
      </div>
    </div>

    <!-- Modal Overlays -->
    <!-- Container Form Modal -->
    <div v-if="showContainerForm" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <ContainerForm
        @submit="handleContainerSubmit"
        @cancel="showContainerForm = false"
      />
    </div>

    <!-- Berth Form Modal -->
    <div v-if="showBerthForm" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <BerthForm
        @submit="handleBerthSubmit"
        @cancel="showBerthForm = false"
      />
    </div>

    <!-- Berth Assignment Form Modal -->
    <div v-if="showBerthAssignmentForm" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <BerthAssignmentForm
        @submit="handleBerthAssignmentSubmit"
        @cancel="showBerthAssignmentForm = false"
      />
    </div>

    <!-- Ship Form Modal -->
    <div v-if="showShipForm" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <ShipForm
        @submit="handleShipSubmit"
        @cancel="showShipForm = false"
      />
    </div>

    <!-- Login Form Modal -->
    <div v-if="showLoginForm" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <LoginForm
        @login-success="handleLoginSuccess"
        @show-register="switchToRegister"
        @cancel="showLoginForm = false"
      />
    </div>

    <!-- Registration Form Modal -->
    <div v-if="showRegistrationForm" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <RegistrationForm
        @register-success="handleRegisterSuccess"
        @show-login="switchToLogin"
        @cancel="showRegistrationForm = false"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { 
  Activity, CheckCircle, AlertCircle, AlertTriangle, Info
} from 'lucide-vue-next'
import ContainerForm from './forms/ContainerForm.vue'
import BerthForm from './forms/BerthForm.vue'
import BerthAssignmentForm from './forms/BerthAssignmentForm.vue'
import ShipForm from './forms/ShipForm.vue'
import LoginForm from './forms/LoginForm.vue'
import RegistrationForm from './forms/RegistrationForm.vue'
import { useToast } from './composables/useToast.js'

// Toast composable
const { success, error, warning, info, confirmAction, clear } = useToast()

// Modal visibility states
const showContainerForm = ref(false)
const showBerthForm = ref(false)
const showBerthAssignmentForm = ref(false)
const showShipForm = ref(false)
const showLoginForm = ref(false)
const showRegistrationForm = ref(false)

// Toast methods
const showSuccessToast = () => {
  success('Operation completed successfully! 🎉')
}

const showErrorToast = () => {
  error('Something went wrong. Please try again.')
}

const showWarningToast = () => {
  warning('Please check your input before proceeding.')
}

const showInfoToast = () => {
  info('New update available. Click to learn more.')
}

const showPersistentToast = () => {
  error('Critical system error detected. Please contact support immediately.', { 
    persistent: true 
  })
}

const showConfirmToast = () => {
  confirmAction(
    'Are you sure you want to delete this container?',
    () => success('Container deleted successfully'),
    () => info('Delete operation cancelled')
  )
}

const clearAllToasts = () => {
  clear()
  info('All toasts cleared')
}

const simulateLoginSuccess = () => {
  success('Welcome back! You have been logged in successfully.')
}

const simulateApiError = () => {
  error('Failed to connect to server. Please check your internet connection and try again.', {
    duration: 6000
  })
}

// Form handlers
const handleContainerSubmit = (data: any) => {
  // Container submitted successfully
  showContainerForm.value = false
}

const handleBerthSubmit = (data: any) => {
  // Berth submitted successfully
  showBerthForm.value = false
}

const handleBerthAssignmentSubmit = (data: any) => {
  // Berth assignment submitted successfully
  showBerthAssignmentForm.value = false
}

const handleShipSubmit = (data: any) => {
  // Ship submitted successfully
  showShipForm.value = false
}

const handleLoginSuccess = (data: any) => {
  // Login successful
  showLoginForm.value = false
}

const handleRegisterSuccess = (data: any) => {
  // Registration successful
  showRegistrationForm.value = false
}

const switchToRegister = () => {
  showLoginForm.value = false
  showRegistrationForm.value = true
}

const switchToLogin = () => {
  showRegistrationForm.value = false
  showLoginForm.value = true
}
</script>