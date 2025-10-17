import { ref, reactive } from 'vue'

// Toast types
export const TOAST_TYPES = {
  SUCCESS: 'success',
  ERROR: 'error',
  WARNING: 'warning',
  INFO: 'info'
} as const

export type ToastType = typeof TOAST_TYPES[keyof typeof TOAST_TYPES]

export interface ToastAction {
  text: string
  handler: () => void
}

export interface ToastOptions {
  duration?: number
  dismissible?: boolean
  persistent?: boolean
  action?: {
    confirm?: ToastAction
    cancel?: ToastAction
  }
}

export interface Toast {
  id: number
  message: string
  type: ToastType
  duration: number
  dismissible: boolean
  action: ToastOptions['action'] | null
  persistent: boolean
  createdAt: number
}

// Global toast state
const toasts = ref<Toast[]>([])
let toastIdCounter = 0
const activeTimeouts = new Map<number, number>()

// Toast interface
const createToast = (message: string, type: ToastType = TOAST_TYPES.INFO, options: ToastOptions = {}): number => {
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
    const timeoutId = setTimeout(() => {
      removeToast(id)
    }, toast.duration)
    
    activeTimeouts.set(id, timeoutId)
  }

  return id
}

const removeToast = (id: number): void => {
  // Clear any active timeout
  if (activeTimeouts.has(id)) {
    clearTimeout(activeTimeouts.get(id))
    activeTimeouts.delete(id)
  }
  
  const index = toasts.value.findIndex(toast => toast.id === id)
  if (index > -1) {
    toasts.value.splice(index, 1)
  }
}

const clearAllToasts = (): void => {
  // Clear all active timeouts
  activeTimeouts.forEach(timeoutId => clearTimeout(timeoutId))
  activeTimeouts.clear()
  
  toasts.value = []
}

// Main composable
export const useToast = () => {
  return {
    // State
    toasts: toasts,
    
    // Methods
    toast: (message: string, type: ToastType = TOAST_TYPES.INFO, options: ToastOptions = {}) => 
      createToast(message, type, options),
    
    success: (message: string, options: ToastOptions = {}) => 
      createToast(message, TOAST_TYPES.SUCCESS, options),
    
    error: (message: string, options: ToastOptions = {}) => 
      createToast(message, TOAST_TYPES.ERROR, options),
    
    warning: (message: string, options: ToastOptions = {}) => 
      createToast(message, TOAST_TYPES.WARNING, options),
    
    info: (message: string, options: ToastOptions = {}) => 
      createToast(message, TOAST_TYPES.INFO, options),
    
    remove: removeToast,
    clear: clearAllToasts,
    
    // Utility methods
    showSuccess: (message: string) => createToast(message, TOAST_TYPES.SUCCESS),
    showError: (message: string) => createToast(message, TOAST_TYPES.ERROR),
    showWarning: (message: string) => createToast(message, TOAST_TYPES.WARNING),
    showInfo: (message: string) => createToast(message, TOAST_TYPES.INFO),
    
    // Advanced methods
    confirmAction: (message: string, onConfirm: () => void, onCancel: (() => void) | null = null) => {
      const toastId = createToast(message, TOAST_TYPES.WARNING, {
        persistent: true,
        action: {
          confirm: {
            text: 'Confirm',
            handler: () => {
              onConfirm()
              removeToast(toastId) // Remove the toast after action
            }
          },
          cancel: {
            text: 'Cancel',
            handler: () => {
              if (onCancel) onCancel()
              removeToast(toastId) // Remove the toast after action
            }
          }
        }
      })
      return toastId
    },
    
    // Loading toast
    loading: (message: string = 'Loading...') => {
      return createToast(message, TOAST_TYPES.INFO, {
        persistent: true,
        dismissible: false
      })
    },
    
    // Remove loading toast
    removeLoading: (toastId: number) => {
      removeToast(toastId)
    }
  }
}

// Export for use in other files without composable
export { toasts, removeToast, clearAllToasts, createToast }