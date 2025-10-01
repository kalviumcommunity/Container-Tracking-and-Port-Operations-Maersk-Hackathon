<template>
  <nav class="bg-white shadow-lg border-b border-slate-200 sticky top-0 z-50">
    <div class="max-w-7xl mx-auto px-6">
      <div class="flex items-center justify-between h-16">
        <!-- Logo/Brand -->
        <div class="flex items-center gap-4">
          <router-link to="/" class="flex items-center gap-3 hover:opacity-80 transition-opacity">
            <div class="p-2 bg-blue-600 rounded-xl shadow-lg">
              <Ship :size="24" class="text-white" />
            </div>
            <div>
              <h1 class="text-xl font-bold text-slate-900">PortTrack</h1>
              <p class="text-xs text-slate-600">Container Operations</p>
            </div>
          </router-link>
        </div>

        <!-- Desktop Navigation -->
        <div class="hidden md:flex items-center">
          <router-link
            v-for="item in navigationItems"
            :key="item.name"
            :to="item.path"
            class="flex items-center gap-2 px-3 py-2 mx-1 rounded-lg font-medium transition-all duration-200 hover:bg-slate-50"
            :class="[
              $route.path === item.path
                ? 'text-blue-600 bg-blue-50 border border-blue-200'
                : 'text-slate-600 hover:text-slate-900'
            ]"
          >
            <component :is="item.icon" :size="18" />
            <span>{{ item.name }}</span>
          </router-link>
        </div>

        <!-- Mobile Menu Button -->
        <div class="md:hidden">
          <button
            @click="toggleMobileMenu"
            class="p-3 rounded-lg text-slate-600 hover:text-slate-900 hover:bg-slate-50 transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50"
            aria-label="Toggle mobile menu"
          >
            <div class="relative w-6 h-6">
              <span
                :class="[
                  'absolute top-0 left-0 w-6 h-0.5 bg-current transition-all duration-300 ease-in-out',
                  isMobileMenuOpen ? 'rotate-45 translate-y-2.5' : 'rotate-0 translate-y-0'
                ]"
              ></span>
              <span
                :class="[
                  'absolute top-2.5 left-0 w-6 h-0.5 bg-current transition-all duration-300 ease-in-out',
                  isMobileMenuOpen ? 'opacity-0' : 'opacity-100'
                ]"
              ></span>
              <span
                :class="[
                  'absolute top-5 left-0 w-6 h-0.5 bg-current transition-all duration-300 ease-in-out',
                  isMobileMenuOpen ? '-rotate-45 -translate-y-2.5' : 'rotate-0 translate-y-0'
                ]"
              ></span>
            </div>
          </button>
        </div>
      </div>

      <!-- Mobile Navigation -->
      <div
        v-if="isMobileMenuOpen"
        class="md:hidden border-t border-slate-200 bg-white animate-slideDown"
      >
        <div class="py-4 px-2">
          <div class="flex flex-col gap-1">
            <router-link
              v-for="item in navigationItems"
              :key="item.name"
              :to="item.path"
              @click="closeMobileMenu"
              class="flex items-center gap-3 px-4 py-3 mx-2 rounded-lg font-medium transition-all duration-200 hover:scale-105"
              :class="[
                $route.path === item.path
                  ? 'text-blue-600 bg-blue-50 border border-blue-200 shadow-sm'
                  : 'text-slate-600 hover:text-slate-900 hover:bg-slate-50'
              ]"
            >
              <component :is="item.icon" :size="20" />
              <span>{{ item.name }}</span>
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute } from 'vue-router'
import { 
  Ship, 
  Home, 
  BarChart3, 
  Container, 
  Anchor, 
  Activity
} from 'lucide-vue-next'

const route = useRoute()
const isMobileMenuOpen = ref(false)

// Navigation items configuration
const navigationItems = [
  {
    name: 'Home',
    path: '/',
    icon: Home
  },
  {
    name: 'Dashboard',
    path: '/dashboard',
    icon: BarChart3
  },
  {
    name: 'Containers',
    path: '/container-management',
    icon: Container
  },
  {
    name: 'Port Operations',
    path: '/port-operation-management',
    icon: Anchor
  },
  {
    name: 'Event Stream',
    path: '/event-streaming',
    icon: Activity
  }
]

// Mobile menu methods
const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}

const closeMobileMenu = () => {
  isMobileMenuOpen.value = false
}
</script>

<style scoped>
@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-slideDown {
  animation: slideDown 0.2s ease-out;
}
</style>