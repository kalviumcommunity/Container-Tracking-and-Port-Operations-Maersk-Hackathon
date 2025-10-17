<script setup>
import { RouterView } from 'vue-router'
import Navbar from './components/main/Navbar.vue'
import ToastContainer from './components/ToastContainer.vue'
</script>

<template>
  <div id="app">
    <Navbar />
    <RouterView />
    <!-- Global Toast Container -->
    <ToastContainer position="top-right" :max-toasts="5" />
  </div>
</template>

