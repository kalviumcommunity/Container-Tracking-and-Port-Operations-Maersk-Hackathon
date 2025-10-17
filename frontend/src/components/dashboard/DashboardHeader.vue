<template>
  <div class="mb-8">
    <div class="flex items-center justify-between mb-4">
      <!-- Left side: Title, subtitle, and admin badge -->
      <div class="flex items-center gap-4">
        <div class="p-3 bg-blue-600 rounded-xl shadow-lg">
          <Ship :size="28" class="text-white" />
        </div>
        <div>
          <h1 class="text-3xl font-bold text-slate-900">Port Operations Dashboard</h1>
          <p class="text-slate-600 mt-1">Port Terminal - Real-time Operations</p>
          <div v-if="isAdminUser" class="mt-2">
            <span class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-emerald-100 text-emerald-800">
              <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
              </svg>
              System Admin - Full Database Access
            </span>
          </div>
        </div>
      </div>
      
      <!-- Right side: Live status and Time (without port name) -->
      <div class="flex items-center gap-6 text-sm text-slate-600">
        <div class="flex items-center gap-2 bg-green-50 px-4 py-2 rounded-lg border border-green-200">
          <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
          <span class="text-green-700 font-medium">Live</span>
        </div>
        <div class="flex items-center gap-2">
          <Clock :size="16" />
          <span>{{ currentTime }}</span>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { Ship, Globe, Clock } from 'lucide-vue-next';

interface Props {
  currentTime: string;
  portName?: string;
}

const props = withDefaults(defineProps<Props>(), {
  portName: 'Chennai Port'
});

const isAdminUser = computed(() => {
  // Check JWT authentication and user roles
  const currentUser = localStorage.getItem('current_user');
  if (currentUser) {
    try {
      const user = JSON.parse(currentUser);
      return user.roles && (user.roles.includes('Admin') || user.roles.includes('PortManager'));
    } catch (error) {
      console.error('Error parsing user data:', error);
      return false;
    }
  }
  
  // Fallback to legacy admin check
  const adminUser = localStorage.getItem('admin_user');
  return !!adminUser;
});
</script>