import axios, { AxiosInstance, AxiosResponse } from 'axios'

// Configuration from environment variables
const API_CONFIG = {
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5221/api',
  timeout: parseInt(import.meta.env.VITE_API_TIMEOUT) || 10000,
  headers: {
    'Content-Type': 'application/json',
  }
}

// Create singleton axios instance
export const httpClient: AxiosInstance = axios.create(API_CONFIG)

// Token management utilities
const TOKEN_KEY = import.meta.env.VITE_JWT_STORAGE_KEY || 'auth_token'
const USER_KEY = import.meta.env.VITE_USER_STORAGE_KEY || 'current_user'

export const tokenManager = {
  getToken(): string | null {
    // Check for real JWT token first
    const jwtToken = localStorage.getItem(TOKEN_KEY)
    if (jwtToken && jwtToken !== 'null' && jwtToken !== 'undefined') {
      return jwtToken
    }
    
    // Fallback to user-based token (for development)
    const currentUser = localStorage.getItem(USER_KEY)
    if (currentUser) {
      try {
        const userData = JSON.parse(currentUser)
        return `mock-jwt-${userData.id}-${userData.username}`
      } catch (error) {
        console.warn('Invalid user data in localStorage:', error)
        localStorage.removeItem(USER_KEY)
      }
    }
    
    return null
  },

  setToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token)
    httpClient.defaults.headers.common['Authorization'] = `Bearer ${token}`
  },

  removeToken(): void {
    localStorage.removeItem(TOKEN_KEY)
    localStorage.removeItem(USER_KEY)
    delete httpClient.defaults.headers.common['Authorization']
  },

  isAuthenticated(): boolean {
    return this.getToken() !== null
  }
}

// Initialize authentication on startup
const initializeAuth = (): void => {
  const token = tokenManager.getToken()
  if (token) {
    httpClient.defaults.headers.common['Authorization'] = `Bearer ${token}`
  }
}

// Request interceptor
httpClient.interceptors.request.use(
  (config) => {
    // Always get fresh token in case it was updated
    const token = tokenManager.getToken()
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }

    // Add timestamp for debugging
    if (import.meta.env.VITE_DEBUG_MODE === 'true') {
      console.log(`🚀 API Request: ${config.method?.toUpperCase()} ${config.url}`)
    }

    return config
  },
  (error) => {
    console.error('❌ Request Error:', error)
    return Promise.reject(error)
  }
)

// Response interceptor
httpClient.interceptors.response.use(
  (response: AxiosResponse) => {
    if (import.meta.env.VITE_DEBUG_MODE === 'true') {
      console.log(`✅ API Response: ${response.config.url} (${response.status})`)
    }
    return response
  },
  (error) => {
    const { response, config } = error

    // Enhanced error logging
    if (response) {
      const status = response.status
      const url = config?.url || 'unknown'
      
      switch (status) {
        case 400:
          console.error(`❌ Bad Request (400): ${url}`, response.data)
          break
        case 401:
          console.error(`🔒 Unauthorized (401): ${url}`)
          console.warn('Authentication failed - clearing tokens and redirecting to login')
          tokenManager.removeToken()
          
          // Only redirect if not already on login page
          if (window.location.pathname !== '/login' && window.location.pathname !== '/') {
            window.location.href = '/login'
          }
          break
        case 403:
          console.error(`🚫 Forbidden (403): ${url}`)
          break
        case 404:
          console.warn(`📭 Not Found (404): ${url}`)
          break
        case 422:
          console.error(`❌ Validation Error (422): ${url}`, response.data)
          break
        case 500:
          console.error(`💥 Server Error (500): ${url}`)
          break
        default:
          console.error(`❌ API Error (${status}): ${url}`, response.data)
      }
    } else if (error.code === 'ECONNABORTED') {
      console.error(`⏰ Request Timeout: ${config?.url}`)
    } else if (error.code === 'ERR_NETWORK') {
      console.error(`🌐 Network Error: ${config?.url} - Check if backend is running`)
    } else {
      console.error('❌ Unknown Error:', error.message)
    }

    return Promise.reject(error)
  }
)

// Initialize authentication
initializeAuth()

// Export commonly used types
export interface ApiResponse<T> {
  data: T
  message?: string
  success?: boolean
  timestamp?: string
}

export interface ApiError {
  message: string
  statusCode?: number
  errors?: Record<string, string[]>
}

export default httpClient