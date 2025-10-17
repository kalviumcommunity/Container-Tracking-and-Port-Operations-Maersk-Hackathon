<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-slate-100 flex items-center justify-center p-4">
    <div class="w-full max-w-md">
      <LoginForm
        @login-success="handleLoginSuccess"
        @show-register="$router.push('/register')"
        @cancel="$router.push('/')"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import LoginForm from '@/forms/LoginForm.vue'

const router = useRouter()

const handleLoginSuccess = (user: any) => {
  // Redirect to dashboard after successful login
  router.push('/dashboard')
}
</script>