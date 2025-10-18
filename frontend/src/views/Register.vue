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
import { useRouter, useRoute } from 'vue-router'
import RegistrationForm from '@/forms/RegistrationForm.vue'

const router = useRouter()
const route = useRoute()

const handleRegisterSuccess = (user: any) => {
  // Check if there's a redirect parameter
  const redirectTo = route.query.redirect as string
  
  if (redirectTo) {
    // Redirect to the intended destination
    router.push(redirectTo)
  } else {
    // Default redirect to dashboard
    router.push('/dashboard')
  }
}
</script>