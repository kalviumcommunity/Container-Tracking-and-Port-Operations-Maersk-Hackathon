# API Integration Guide

This guide covers how to integrate with the **PortTrack Container Operations** backend API from the Vue 3 frontend application, including authentication, error handling, and best practices.

## 🔧 API Configuration

### Base Setup (api.ts)
```typescript
import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios'
import { useAuthStore } from '@/stores/authStore'
import { useErrorStore } from '@/stores/errorStore'

// API Base Configuration
const API_CONFIG = {
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api',
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
}

// Create Axios Instance
export const apiClient: AxiosInstance = axios.create(API_CONFIG)

// Request Interceptor
apiClient.interceptors.request.use(
  (config: AxiosRequestConfig) => {
    // Add authentication token
    const token = localStorage.getItem('access_token')
    if (token) {
      config.headers = {
        ...config.headers,
        Authorization: `Bearer ${token}`
      }
    }

    // Add request timestamp
    config.metadata = { startTime: new Date() }
    
    console.log(`🚀 API Request: ${config.method?.toUpperCase()} ${config.url}`)
    return config
  },
  (error) => {
    console.error('❌ Request Error:', error)
    return Promise.reject(error)
  }
)

// Response Interceptor
apiClient.interceptors.response.use(
  (response: AxiosResponse) => {
    // Log response time
    const endTime = new Date()
    const startTime = response.config.metadata?.startTime
    const duration = startTime ? endTime.getTime() - startTime.getTime() : 0
    
    console.log(`✅ API Response: ${response.config.url} (${duration}ms)`)
    return response
  },
  async (error) => {
    const { response, config } = error

    // Handle different error types
    if (response) {
      switch (response.status) {
        case 401:
          console.error('🔒 Unauthorized - Redirecting to login')
          const authStore = useAuthStore()
          await authStore.logout()
          break
          
        case 403:
          console.error('🚫 Forbidden - Insufficient permissions')
          const errorStore = useErrorStore()
          errorStore.addError({
            type: 'permission',
            message: 'You do not have permission to perform this action',
            statusCode: 403
          })
          break
          
        case 429:
          console.error('⏱️ Rate limit exceeded')
          break
          
        case 500:
          console.error('💥 Server error')
          break
      }
    } else if (error.code === 'ECONNABORTED') {
      console.error('⏰ Request timeout')
    } else {
      console.error('🌐 Network error')
    }

    return Promise.reject(error)
  }
)
```

## 🔐 Authentication Service

### Auth Service Implementation
```typescript
// services/authService.ts
import { apiClient } from './api'
import type { 
  LoginRequest, 
  LoginResponse, 
  RegisterRequest, 
  RefreshTokenRequest,
  UserProfile,
  ChangePasswordRequest
} from '@/types/auth'

export const authService = {
  // Login
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>('/auth/login', credentials)
    return response.data
  },

  // Register
  async register(userData: RegisterRequest): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>('/auth/register', userData)
    return response.data
  },

  // Refresh Token
  async refreshToken(tokenData: RefreshTokenRequest): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>('/auth/refresh', tokenData)
    return response.data
  },

  // Get Current User Profile
  async getProfile(): Promise<UserProfile> {
    const response = await apiClient.get<UserProfile>('/auth/profile')
    return response.data
  },

  // Change Password
  async changePassword(passwordData: ChangePasswordRequest): Promise<void> {
    await apiClient.post('/auth/change-password', passwordData)
  },

  // Logout
  async logout(): Promise<void> {
    await apiClient.post('/auth/logout')
  },

  // Validate Token
  async validateToken(): Promise<boolean> {
    try {
      await apiClient.get('/auth/validate')
      return true
    } catch {
      return false
    }
  }
}
```

## 📦 Container Service

### Container API Operations
```typescript
// services/containerService.ts
import { apiClient } from './api'
import type { 
  Container, 
  ContainerFilters, 
  CreateContainerRequest,
  UpdateContainerRequest,
  ContainerListResponse 
} from '@/types/container'

export const containerService = {
  // Get All Containers with Filtering
  async getContainers(filters?: ContainerFilters): Promise<ContainerListResponse> {
    const params = new URLSearchParams()
    
    if (filters) {
      Object.entries(filters).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          params.append(key, String(value))
        }
      })
    }

    const response = await apiClient.get<ContainerListResponse>(
      `/containers?${params.toString()}`
    )
    return response.data
  },

  // Get Container by ID
  async getContainer(id: string): Promise<Container> {
    const response = await apiClient.get<Container>(`/containers/${id}`)
    return response.data
  },

  // Create New Container
  async createContainer(containerData: CreateContainerRequest): Promise<Container> {
    const response = await apiClient.post<Container>('/containers', containerData)
    return response.data
  },

  // Update Container
  async updateContainer(id: string, updateData: UpdateContainerRequest): Promise<Container> {
    const response = await apiClient.put<Container>(`/containers/${id}`, updateData)
    return response.data
  },

  // Delete Container
  async deleteContainer(id: string): Promise<void> {
    await apiClient.delete(`/containers/${id}`)
  },

  // Get Container History
  async getContainerHistory(id: string): Promise<ContainerEvent[]> {
    const response = await apiClient.get<ContainerEvent[]>(`/containers/${id}/history`)
    return response.data
  },

  // Update Container Status
  async updateContainerStatus(id: string, status: string, location?: string): Promise<void> {
    await apiClient.patch(`/containers/${id}/status`, { status, location })
  },

  // Bulk Operations
  async bulkUpdateContainers(containerIds: string[], updateData: Partial<Container>): Promise<void> {
    await apiClient.patch('/containers/bulk-update', {
      containerIds,
      updateData
    })
  }
}
```

## 🚢 Ship Service

### Ship Management API
```typescript
// services/shipService.ts
import { apiClient } from './api'
import type { 
  Ship, 
  CreateShipRequest, 
  UpdateShipRequest,
  ShipContainer,
  BerthAssignment 
} from '@/types/ship'

export const shipService = {
  // Get All Ships
  async getShips(): Promise<Ship[]> {
    const response = await apiClient.get<Ship[]>('/ships')
    return response.data
  },

  // Get Ship Details
  async getShip(id: string): Promise<Ship> {
    const response = await apiClient.get<Ship>(`/ships/${id}`)
    return response.data
  },

  // Create Ship
  async createShip(shipData: CreateShipRequest): Promise<Ship> {
    const response = await apiClient.post<Ship>('/ships', shipData)
    return response.data
  },

  // Update Ship
  async updateShip(id: string, updateData: UpdateShipRequest): Promise<Ship> {
    const response = await apiClient.put<Ship>(`/ships/${id}`, updateData)
    return response.data
  },

  // Get Ship Containers
  async getShipContainers(shipId: string): Promise<ShipContainer[]> {
    const response = await apiClient.get<ShipContainer[]>(`/ships/${shipId}/containers`)
    return response.data
  },

  // Assign Container to Ship
  async assignContainer(shipId: string, containerId: string): Promise<void> {
    await apiClient.post(`/ships/${shipId}/containers/${containerId}`)
  },

  // Remove Container from Ship
  async removeContainer(shipId: string, containerId: string): Promise<void> {
    await apiClient.delete(`/ships/${shipId}/containers/${containerId}`)
  },

  // Get Ship Berth Assignment
  async getBerthAssignment(shipId: string): Promise<BerthAssignment | null> {
    try {
      const response = await apiClient.get<BerthAssignment>(`/ships/${shipId}/berth`)
      return response.data
    } catch (error) {
      if (error.response?.status === 404) return null
      throw error
    }
  }
}
```

## 🏗️ Port Service

### Port Operations API
```typescript
// services/portService.ts
import { apiClient } from './api'
import type { 
  Port, 
  Berth, 
  BerthAssignment, 
  CreateBerthRequest,
  UpdateBerthRequest 
} from '@/types/port'

export const portService = {
  // Get All Ports
  async getPorts(): Promise<Port[]> {
    const response = await apiClient.get<Port[]>('/ports')
    return response.data
  },

  // Get Port Details
  async getPort(id: string): Promise<Port> {
    const response = await apiClient.get<Port>(`/ports/${id}`)
    return response.data
  },

  // Get Port Berths
  async getPortBerths(portId: string): Promise<Berth[]> {
    const response = await apiClient.get<Berth[]>(`/ports/${portId}/berths`)
    return response.data
  },

  // Create Berth
  async createBerth(portId: string, berthData: CreateBerthRequest): Promise<Berth> {
    const response = await apiClient.post<Berth>(`/ports/${portId}/berths`, berthData)
    return response.data
  },

  // Update Berth
  async updateBerth(berthId: string, updateData: UpdateBerthRequest): Promise<Berth> {
    const response = await apiClient.put<Berth>(`/berths/${berthId}`, updateData)
    return response.data
  },

  // Get Berth Assignments
  async getBerthAssignments(): Promise<BerthAssignment[]> {
    const response = await apiClient.get<BerthAssignment[]>('/berth-assignments')
    return response.data
  },

  // Assign Ship to Berth
  async assignShipToBerth(
    shipId: string, 
    berthId: string, 
    estimatedArrival: string,
    estimatedDeparture: string
  ): Promise<BerthAssignment> {
    const response = await apiClient.post<BerthAssignment>('/berth-assignments', {
      shipId,
      berthId,
      estimatedArrival,
      estimatedDeparture
    })
    return response.data
  }
}
```

## 👥 User Management Service

### User Administration API
```typescript
// services/userService.ts
import { apiClient } from './api'
import type { 
  User, 
  Role, 
  Permission,
  RoleApplication,
  CreateUserRequest,
  UpdateUserRequest 
} from '@/types/user'

export const userService = {
  // Get All Users (Admin only)
  async getUsers(): Promise<User[]> {
    const response = await apiClient.get<User[]>('/users')
    return response.data
  },

  // Get User by ID
  async getUser(id: string): Promise<User> {
    const response = await apiClient.get<User>(`/users/${id}`)
    return response.data
  },

  // Create User (Admin only)
  async createUser(userData: CreateUserRequest): Promise<User> {
    const response = await apiClient.post<User>('/users', userData)
    return response.data
  },

  // Update User
  async updateUser(id: string, updateData: UpdateUserRequest): Promise<User> {
    const response = await apiClient.put<User>(`/users/${id}`, updateData)
    return response.data
  },

  // Get Available Roles
  async getRoles(): Promise<Role[]> {
    const response = await apiClient.get<Role[]>('/roles')
    return response.data
  },

  // Get Role Permissions
  async getRolePermissions(roleId: string): Promise<Permission[]> {
    const response = await apiClient.get<Permission[]>(`/roles/${roleId}/permissions`)
    return response.data
  },

  // Apply for Role
  async applyForRole(roleId: string, justification: string): Promise<RoleApplication> {
    const response = await apiClient.post<RoleApplication>('/role-applications', {
      roleId,
      justification
    })
    return response.data
  },

  // Get My Role Applications
  async getMyRoleApplications(): Promise<RoleApplication[]> {
    const response = await apiClient.get<RoleApplication[]>('/role-applications/my-applications')
    return response.data
  },

  // Get Pending Role Applications (Admin/Manager)
  async getPendingRoleApplications(): Promise<RoleApplication[]> {
    const response = await apiClient.get<RoleApplication[]>('/role-applications/pending')
    return response.data
  },

  // Approve/Reject Role Application
  async processRoleApplication(
    applicationId: string, 
    action: 'approve' | 'reject',
    comments?: string
  ): Promise<void> {
    await apiClient.patch(`/role-applications/${applicationId}`, {
      action,
      comments
    })
  }
}
```

## 🎯 Service Integration in Components

### Using Services in Vue Components
```vue
<template>
  <div class="container-management">
    <div v-if="isLoading" class="loading-spinner">
      Loading containers...
    </div>
    
    <div v-else-if="error" class="error-message">
      {{ error.message }}
    </div>
    
    <div v-else>
      <ContainerList :containers="containers" @update="handleUpdate" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { containerService } from '@/services/containerService'
import { useAuthStore } from '@/stores/authStore'
import type { Container, ContainerFilters } from '@/types/container'

// Reactive state
const containers = ref<Container[]>([])
const isLoading = ref(false)
const error = ref<Error | null>(null)

// Store access
const authStore = useAuthStore()

// Computed properties
const canEditContainers = computed(() => 
  authStore.hasPermission('edit_containers')
)

// Methods
const loadContainers = async (filters?: ContainerFilters) => {
  isLoading.value = true
  error.value = null
  
  try {
    const response = await containerService.getContainers(filters)
    containers.value = response.containers
  } catch (err) {
    error.value = err as Error
    console.error('Failed to load containers:', err)
  } finally {
    isLoading.value = false
  }
}

const handleUpdate = async (container: Container) => {
  if (!canEditContainers.value) {
    error.value = new Error('Insufficient permissions')
    return
  }

  try {
    await containerService.updateContainer(container.id, container)
    await loadContainers() // Refresh list
  } catch (err) {
    error.value = err as Error
  }
}

// Lifecycle
onMounted(() => {
  loadContainers()
})
</script>
```

## 🎣 Custom Composables for API Integration

### useApi Composable
```typescript
// composables/useApi.ts
import { ref, reactive } from 'vue'
import type { Ref } from 'vue'

interface ApiState<T> {
  data: Ref<T | null>
  isLoading: Ref<boolean>
  error: Ref<Error | null>
}

interface ApiOptions {
  immediate?: boolean
  onError?: (error: Error) => void
  onSuccess?: (data: any) => void
}

export function useApi<T>(
  apiCall: () => Promise<T>,
  options: ApiOptions = {}
): ApiState<T> & {
  execute: () => Promise<void>
  reset: () => void
} {
  const data = ref<T | null>(null) as Ref<T | null>
  const isLoading = ref(false)
  const error = ref<Error | null>(null)

  const execute = async () => {
    isLoading.value = true
    error.value = null

    try {
      const result = await apiCall()
      data.value = result
      options.onSuccess?.(result)
    } catch (err) {
      error.value = err as Error
      options.onError?.(err as Error)
    } finally {
      isLoading.value = false
    }
  }

  const reset = () => {
    data.value = null
    error.value = null
    isLoading.value = false
  }

  if (options.immediate !== false) {
    execute()
  }

  return {
    data,
    isLoading,
    error,
    execute,
    reset
  }
}
```

### Usage Example
```vue
<script setup lang="ts">
import { useApi } from '@/composables/useApi'
import { containerService } from '@/services/containerService'

const {
  data: containers,
  isLoading,
  error,
  execute: refetch
} = useApi(
  () => containerService.getContainers(),
  {
    onError: (error) => {
      console.error('Failed to fetch containers:', error)
    }
  }
)
</script>
```

## 🚨 Error Handling Best Practices

### Centralized Error Handling
```typescript
// stores/errorStore.ts
import { defineStore } from 'pinia'

interface AppError {
  id: string
  type: 'network' | 'validation' | 'permission' | 'server'
  message: string
  statusCode?: number
  timestamp: Date
  context?: any
}

export const useErrorStore = defineStore('error', {
  state: () => ({
    errors: [] as AppError[]
  }),

  actions: {
    addError(error: Omit<AppError, 'id' | 'timestamp'>) {
      const appError: AppError = {
        ...error,
        id: Date.now().toString(),
        timestamp: new Date()
      }
      
      this.errors.push(appError)
      
      // Auto-remove after 5 seconds
      setTimeout(() => {
        this.removeError(appError.id)
      }, 5000)
    },

    removeError(id: string) {
      const index = this.errors.findIndex(error => error.id === id)
      if (index > -1) {
        this.errors.splice(index, 1)
      }
    },

    clearErrors() {
      this.errors = []
    }
  }
})
```

## 📊 API Response Types

### TypeScript Interfaces
```typescript
// types/api.ts
export interface ApiResponse<T> {
  data: T
  message: string
  success: boolean
  timestamp: string
}

export interface PaginatedResponse<T> {
  data: T[]
  pagination: {
    page: number
    limit: number
    total: number
    pages: number
  }
}

export interface ApiError {
  message: string
  statusCode: number
  error: string
  timestamp: string
  path: string
}
```

This comprehensive API integration guide provides everything needed to effectively communicate with the PortTrack Container Operations backend API, including proper error handling, authentication, and type safety.