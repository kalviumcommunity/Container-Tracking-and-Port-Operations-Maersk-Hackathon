<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-slate-100 flex items-center justify-center p-4">
    <div class="w-full max-w-md">
      <RegistrationForm
        @register-success="handleRegisterSuccess"
        @show-login="$router.push('/login')"
        @cancel="$router.push('/')"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import RegistrationForm from '@/forms/RegistrationForm.vue'

const router = useRouter()

const handleRegisterSuccess = (user: any) => {
  // Redirect to dashboard after successful registration
  router.push('/dashboard')
}
</script>