<template>
  <Transition
    name="toast"
    appear
    @enter="onEnter"
    @leave="onLeave"
  >
    <div
      :class="toastClasses"
      class="relative flex items-start gap-3 p-4 rounded-lg shadow-lg border max-w-sm w-full bg-white transform transition-all duration-300 ease-in-out"
      role="alert"
      :aria-live="toast.type === 'error' ? 'assertive' : 'polite'"
    >
      <!-- Icon -->
      <div class="flex-shrink-0 mt-0.5">
        <component :is="iconComponent" :size="20" :class="iconClasses" />
      </div>

      <!-- Content -->
      <div class="flex-grow min-w-0">
        <p class="text-sm font-medium text-slate-900 break-words">
          {{ toast.message }}
        </p>
        
        <!-- Action buttons for confirmations -->
        <div v-if="toast.action" class="flex gap-2 mt-3">
          <button
            v-if="toast.action.confirm"
            @click="handleAction('confirm')"
            class="px-3 py-1 text-xs font-medium bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors"
          >
            {{ toast.action.confirm.text }}
          </button>
          <button
            v-if="toast.action.cancel"
            @click="handleAction('cancel')"
            class="px-3 py-1 text-xs font-medium bg-slate-200 text-slate-700 rounded hover:bg-slate-300 transition-colors"
          >
            {{ toast.action.cancel.text }}
          </button>
        </div>
      </div>

      <!-- Close button -->
      <button
        v-if="toast.dismissible"
        @click="$emit('dismiss')"
        class="flex-shrink-0 p-1 text-slate-400 hover:text-slate-600 transition-colors rounded focus:outline-none focus:ring-2 focus:ring-slate-300"
        aria-label="Dismiss notification"
      >
        <X :size="16" />
      </button>

      <!-- Progress bar for timed toasts -->
      <div
        v-if="!toast.persistent && showProgress"
        class="absolute bottom-0 left-0 h-1 bg-current opacity-20 rounded-b-lg transition-all duration-100 linear"
        :style="{ width: progressWidth + '%' }"
      ></div>
    </div>
  </Transition>
</template>

<script setup>
import { computed, ref, onMounted, onUnmounted } from 'vue'
import { CheckCircle, AlertTriangle, Info, X, AlertCircle } from 'lucide-vue-next'
import { TOAST_TYPES } from '../composables/useToast.js'

const props = defineProps({
  toast: {
    type: Object,
    required: true
  },
  showProgress: {
    type: Boolean,
    default: true
  }
})

const emit = defineEmits(['dismiss'])

// Progress tracking for auto-dismiss
const progressWidth = ref(100)
let progressInterval = null

// Toast styling based on type
const toastClasses = computed(() => {
  const baseClasses = 'toast-item'
  
  switch (props.toast.type) {
    case TOAST_TYPES.SUCCESS:
      return `${baseClasses} border-green-200 bg-green-50`
    case TOAST_TYPES.ERROR:
      return `${baseClasses} border-red-200 bg-red-50`
    case TOAST_TYPES.WARNING:
      return `${baseClasses} border-orange-200 bg-orange-50`
    case TOAST_TYPES.INFO:
    default:
      return `${baseClasses} border-blue-200 bg-blue-50`
  }
})

// Icon based on type
const iconComponent = computed(() => {
  switch (props.toast.type) {
    case TOAST_TYPES.SUCCESS:
      return CheckCircle
    case TOAST_TYPES.ERROR:
      return AlertCircle
    case TOAST_TYPES.WARNING:
      return AlertTriangle
    case TOAST_TYPES.INFO:
    default:
      return Info
  }
})

const iconClasses = computed(() => {
  switch (props.toast.type) {
    case TOAST_TYPES.SUCCESS:
      return 'text-green-600'
    case TOAST_TYPES.ERROR:
      return 'text-red-600'
    case TOAST_TYPES.WARNING:
      return 'text-orange-600'
    case TOAST_TYPES.INFO:
    default:
      return 'text-blue-600'
  }
})

// Handle action buttons
const handleAction = (action) => {
  if (props.toast.action && props.toast.action[action]) {
    props.toast.action[action].handler()
  }
  emit('dismiss')
}

// Animation callbacks
const onEnter = (el) => {
  el.style.transform = 'translateX(100%)'
  el.style.opacity = '0'
  
  // Force reflow
  el.offsetHeight
  
  el.style.transform = 'translateX(0)'
  el.style.opacity = '1'
}

const onLeave = (el) => {
  el.style.transform = 'translateX(100%)'
  el.style.opacity = '0'
}

// Setup progress bar for non-persistent toasts
onMounted(() => {
  if (!props.toast.persistent && props.showProgress) {
    const startTime = Date.now()
    const duration = props.toast.duration
    
    progressInterval = setInterval(() => {
      const elapsed = Date.now() - startTime
      const remaining = Math.max(0, duration - elapsed)
      progressWidth.value = (remaining / duration) * 100
      
      if (remaining <= 0) {
        clearInterval(progressInterval)
      }
    }, 50)
  }
})

onUnmounted(() => {
  if (progressInterval) {
    clearInterval(progressInterval)
  }
})
</script>

<style scoped>
/* Toast animations */
.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease-in-out;
}

.toast-enter-from {
  opacity: 0;
  transform: translateX(100%);
}

.toast-leave-to {
  opacity: 0;
  transform: translateX(100%);
}

.toast-item {
  transform-origin: right center;
}

/* Hover effects */
.toast-item:hover {
  transform: translateX(-4px);
  box-shadow: 0 10px 25px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
}
</style>