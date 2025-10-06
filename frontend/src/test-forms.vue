<template>
  <div class="min-h-screen bg-slate-100 p-8">
    <div class="max-w-6xl mx-auto">
      <h1 class="text-3xl font-bold text-slate-900 mb-8">Form Popup Tests</h1>
      
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
import ContainerForm from './forms/ContainerForm.vue'
import BerthForm from './forms/BerthForm.vue'
import BerthAssignmentForm from './forms/BerthAssignmentForm.vue'
import ShipForm from './forms/ShipForm.vue'
import LoginForm from './forms/LoginForm.vue'
import RegistrationForm from './forms/RegistrationForm.vue'

// Modal visibility states
const showContainerForm = ref(false)
const showBerthForm = ref(false)
const showBerthAssignmentForm = ref(false)
const showShipForm = ref(false)
const showLoginForm = ref(false)
const showRegistrationForm = ref(false)

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