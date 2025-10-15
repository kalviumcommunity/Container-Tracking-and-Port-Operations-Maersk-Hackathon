<script setup>
import { RouterView, useRoute } from 'vue-router'
import { computed } from 'vue'
import Navbar from './components/main/Navbar.vue'
import ToastContainer from './components/ToastContainer.vue'

const route = useRoute()

// Hide navbar on landing page
const showNavbar = computed(() => {
  return route.name !== 'landing'
})
</script>

<template>
  <div id="app">
    <Navbar v-if="showNavbar" />
    <RouterView />
    <!-- Global Toast Container -->
    <ToastContainer position="top-right" :max-toasts="5" />
  </div>
</template>

