<template>
  <!-- Toast Container - Fixed positioned overlay -->
  <Teleport to="body">
    <div
      v-if="toasts.length > 0"
      class="fixed inset-0 pointer-events-none z-[9999] overflow-hidden"
      aria-label="Notifications"
    >
      <!-- Toast stack positioned at top-right by default -->
      <div 
        :class="containerClasses"
        class="flex flex-col gap-3 p-4 max-h-screen overflow-y-auto pointer-events-none"
      >
        <TransitionGroup
          name="toast-list"
          tag="div"
          class="flex flex-col gap-3"
        >
          <div
            v-for="toast in visibleToasts"
            :key="toast.id"
            class="pointer-events-auto"
          >
            <Toast
              :toast="toast"
              :show-progress="showProgress"
              @dismiss="removeToast(toast.id)"
            />
          </div>
        </TransitionGroup>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import { computed } from 'vue'
import Toast from './Toast.vue'
import { useToast } from '../composables/useToast.js'

const props = defineProps({
  position: {
    type: String,
    default: 'top-right', // 'top-right', 'top-left', 'bottom-right', 'bottom-left', 'top-center', 'bottom-center'
    validator: (value) => [
      'top-right', 'top-left', 'bottom-right', 'bottom-left', 'top-center', 'bottom-center'
    ].includes(value)
  },
  maxToasts: {
    type: Number,
    default: 5
  },
  showProgress: {
    type: Boolean,
    default: true
  }
})

const { toasts, remove: removeToast } = useToast()

// Limit the number of visible toasts
const visibleToasts = computed(() => {
  return toasts.value.slice(-props.maxToasts)
})

// Container positioning classes
const containerClasses = computed(() => {
  const baseClasses = 'fixed'
  
  switch (props.position) {
    case 'top-right':
      return `${baseClasses} top-0 right-0`
    case 'top-left':
      return `${baseClasses} top-0 left-0`
    case 'bottom-right':
      return `${baseClasses} bottom-0 right-0`
    case 'bottom-left':
      return `${baseClasses} bottom-0 left-0`
    case 'top-center':
      return `${baseClasses} top-0 left-1/2 transform -translate-x-1/2`
    case 'bottom-center':
      return `${baseClasses} bottom-0 left-1/2 transform -translate-x-1/2`
    default:
      return `${baseClasses} top-0 right-0`
  }
})
</script>

<style scoped>
/* List transition animations */
.toast-list-enter-active,
.toast-list-leave-active {
  transition: all 0.3s ease;
}

.toast-list-enter-from,
.toast-list-leave-to {
  opacity: 0;
  transform: translateX(100%);
}

.toast-list-move {
  transition: transform 0.3s ease;
}

/* Custom scrollbar for toast container */
.max-h-screen::-webkit-scrollbar {
  width: 4px;
}

.max-h-screen::-webkit-scrollbar-track {
  background: transparent;
}

.max-h-screen::-webkit-scrollbar-thumb {
  background: rgba(0, 0, 0, 0.2);
  border-radius: 2px;
}

.max-h-screen::-webkit-scrollbar-thumb:hover {
  background: rgba(0, 0, 0, 0.3);
}
</style>