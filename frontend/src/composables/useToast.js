import { ref, reactive } from 'vue'

// Toast types
const TOAST_TYPES = {
  SUCCESS: 'success',
  ERROR: 'error',
  WARNING: 'warning',
  INFO: 'info'
}

// Global toast state
const toasts = ref([])
let toastIdCounter = 0

// Toast interface
const createToast = (message, type = TOAST_TYPES.INFO, options = {}) => {
  const id = ++toastIdCounter
  const toast = {
    id,
    message,
    type,
    duration: options.duration || (type === TOAST_TYPES.ERROR ? 5000 : 3000),
    dismissible: options.dismissible !== false,
    action: options.action || null,
    persistent: options.persistent || false,
    createdAt: Date.now()
  }

  toasts.value.push(toast)

  // Auto-dismiss unless persistent
  if (!toast.persistent) {
    setTimeout(() => {
      removeToast(id)
    }, toast.duration)
  }

  return id
}

const removeToast = (id) => {
  const index = toasts.value.findIndex(toast => toast.id === id)
  if (index > -1) {
    toasts.value.splice(index, 1)
  }
}

const clearAllToasts = () => {
  toasts.value = []
}

// Main composable
export const useToast = () => {
  return {
    // State
    toasts: toasts,
    
    // Methods
    toast: (message, type = TOAST_TYPES.INFO, options = {}) => 
      createToast(message, type, options),
    
    success: (message, options = {}) => 
      createToast(message, TOAST_TYPES.SUCCESS, options),
    
    error: (message, options = {}) => 
      createToast(message, TOAST_TYPES.ERROR, options),
    
    warning: (message, options = {}) => 
      createToast(message, TOAST_TYPES.WARNING, options),
    
    info: (message, options = {}) => 
      createToast(message, TOAST_TYPES.INFO, options),
    
    remove: removeToast,
    clear: clearAllToasts,
    
    // Utility methods
    showSuccess: (message) => createToast(message, TOAST_TYPES.SUCCESS),
    showError: (message) => createToast(message, TOAST_TYPES.ERROR),
    showWarning: (message) => createToast(message, TOAST_TYPES.WARNING),
    showInfo: (message) => createToast(message, TOAST_TYPES.INFO),
    
    // Advanced methods
    confirmAction: (message, onConfirm, onCancel = null) => {
      return createToast(message, TOAST_TYPES.WARNING, {
        persistent: true,
        action: {
          confirm: {
            text: 'Confirm',
            handler: () => {
              onConfirm()
              // Toast will be removed automatically after action
            }
          },
          cancel: {
            text: 'Cancel',
            handler: () => {
              if (onCancel) onCancel()
              // Toast will be removed automatically after action
            }
          }
        }
      })
    },
    
    // Loading toast
    loading: (message = 'Loading...') => {
      return createToast(message, TOAST_TYPES.INFO, {
        persistent: true,
        dismissible: false
      })
    }
  }
}

// Export for use in other files without composable
export { toasts, removeToast, clearAllToasts, createToast, TOAST_TYPES }