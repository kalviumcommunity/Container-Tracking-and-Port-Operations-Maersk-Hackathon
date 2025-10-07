import axios, { AxiosResponse } from 'axios'

// API Configuration
const API_BASE_URL = process.env.NODE_ENV === 'production' 
  ? 'https://your-api-domain.com/api' 
  : 'http://localhost:5221/api'

// Create axios instance with default config
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Add token to requests
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('auth_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// TypeScript interfaces for User Management
export interface UserListDto {
  userId: number
  username: string
  email: string
  fullName: string
  phoneNumber?: string
  department?: string
  portId?: number
  roles: string[]
  isActive: boolean
  isBlocked: boolean
  isDeleted: boolean
  lastLoginAt?: string
  createdAt: string
  updatedAt: string
}

export interface UpdateUserRolesDto {
  roles: string[]
}

export interface UpdateUserStatusDto {
  isBlocked: boolean
  reason?: string
}

export interface SystemStatsDto {
  totalUsers: number
  activeUsers: number
  blockedUsers: number
  deletedUsers: number
  totalRoles: number
  pendingRoleRequests: number
  recentRegistrations: number
  lastWeekLogins: number
}

export interface UsersPagedResponse {
  users: UserListDto[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

export interface ApiResponse<T> {
  data: T
  success: boolean
  message?: string
}

// User Management API Service
export class UserManagementApiService {
  
  // Get all users with pagination and filtering
  async getUsers(params?: {
    pageNumber?: number
    pageSize?: number
    searchTerm?: string
    role?: string
    isActive?: boolean
    isBlocked?: boolean
  }): Promise<UsersPagedResponse> {
    try {
      const response: AxiosResponse<UsersPagedResponse> = await apiClient.get('/usermanagement', { params })
      return response.data
    } catch (error) {
      console.error('Error fetching users:', error)
      throw error
    }
  }

  // Get specific user by ID
  async getUserById(userId: number): Promise<UserListDto> {
    try {
      const response: AxiosResponse<UserListDto> = await apiClient.get(`/usermanagement/${userId}`)
      return response.data
    } catch (error) {
      console.error('Error fetching user:', error)
      throw error
    }
  }

  // Update user roles
  async updateUserRoles(userId: number, roles: UpdateUserRolesDto): Promise<ApiResponse<string>> {
    try {
      const response: AxiosResponse<ApiResponse<string>> = await apiClient.put(
        `/usermanagement/${userId}/roles`, 
        roles
      )
      return response.data
    } catch (error) {
      console.error('Error updating user roles:', error)
      throw error
    }
  }

  // Block or unblock user
  async updateUserStatus(userId: number, isBlocked: boolean, reason?: string): Promise<ApiResponse<string>> {
    try {
      const response: AxiosResponse<ApiResponse<string>> = await apiClient.post(
        `/usermanagement/${userId}/block`, 
        { isBlocked, reason }
      )
      return response.data
    } catch (error) {
      console.error('Error updating user status:', error)
      throw error
    }
  }

  // Soft delete user
  async deleteUser(userId: number, reason?: string): Promise<ApiResponse<string>> {
    try {
      const response: AxiosResponse<ApiResponse<string>> = await apiClient.delete(
        `/usermanagement/${userId}`
      )
      return response.data
    } catch (error) {
      console.error('Error deleting user:', error)
      throw error
    }
  }

  // Restore user
  async restoreUser(userId: number): Promise<ApiResponse<string>> {
    try {
      const response: AxiosResponse<ApiResponse<string>> = await apiClient.delete(
        `/usermanagement/${userId}?restore=true`
      )
      return response.data
    } catch (error) {
      console.error('Error restoring user:', error)
      throw error
    }
  }

  // Get system statistics
  async getSystemStats(): Promise<SystemStatsDto> {
    try {
      const response: AxiosResponse<SystemStatsDto> = await apiClient.get('/usermanagement/stats')
      return response.data
    } catch (error) {
      console.error('Error fetching system stats:', error)
      throw error
    }
  }

  // Search users by term
  async searchUsers(searchTerm: string, pageNumber = 1, pageSize = 10): Promise<UsersPagedResponse> {
    try {
      const response: AxiosResponse<UsersPagedResponse> = await apiClient.get('/usermanagement', {
        params: { searchTerm, pageNumber, pageSize }
      })
      return response.data
    } catch (error) {
      console.error('Error searching users:', error)
      throw error
    }
  }

  // Get users by role
  async getUsersByRole(role: string, pageNumber = 1, pageSize = 10): Promise<UsersPagedResponse> {
    try {
      const response: AxiosResponse<UsersPagedResponse> = await apiClient.get('/usermanagement', {
        params: { role, pageNumber, pageSize }
      })
      return response.data
    } catch (error) {
      console.error('Error fetching users by role:', error)
      throw error
    }
  }

  // Get blocked users
  async getBlockedUsers(pageNumber = 1, pageSize = 10): Promise<UsersPagedResponse> {
    try {
      const response: AxiosResponse<UsersPagedResponse> = await apiClient.get('/usermanagement', {
        params: { isBlocked: true, pageNumber, pageSize }
      })
      return response.data
    } catch (error) {
      console.error('Error fetching blocked users:', error)
      throw error
    }
  }

  // Get deleted users
  async getDeletedUsers(pageNumber = 1, pageSize = 10): Promise<UsersPagedResponse> {
    try {
      const response: AxiosResponse<UsersPagedResponse> = await apiClient.get('/usermanagement', {
        params: { isActive: false, pageNumber, pageSize }
      })
      return response.data
    } catch (error) {
      console.error('Error fetching deleted users:', error)
      throw error
    }
  }
}

// Export singleton instance
export const userManagementApi = new UserManagementApiService()

// Helper functions for user management
export const userManagementHelpers = {
  // Format user status
  getUserStatus(user: UserListDto): string {
    if (user.isDeleted) return 'Deleted'
    if (user.isBlocked) return 'Blocked'
    if (!user.isActive) return 'Inactive'
    return 'Active'
  },

  // Get status color class
  getUserStatusColor(user: UserListDto): string {
    if (user.isDeleted) return 'bg-gray-100 text-gray-800'
    if (user.isBlocked) return 'bg-red-100 text-red-800'
    if (!user.isActive) return 'bg-yellow-100 text-yellow-800'
    return 'bg-green-100 text-green-800'
  },

  // Check if user is admin
  isAdmin(user: UserListDto): boolean {
    return user.roles.some(role => 
      role.toLowerCase().includes('admin') || 
      role.toLowerCase().includes('superadmin')
    )
  },

  // Format date
  formatDate(dateString?: string): string {
    if (!dateString) return 'Never'
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    })
  },

  // Get role color
  getRoleColor(role: string): string {
    const colors: Record<string, string> = {
      'System Administrator': 'bg-purple-100 text-purple-800',
      'SuperAdmin': 'bg-purple-100 text-purple-800',
      'Admin': 'bg-red-100 text-red-800',
      'Port Manager': 'bg-blue-100 text-blue-800',
      'Container Manager': 'bg-green-100 text-green-800',
      'Supervisor': 'bg-yellow-100 text-yellow-800',
      'Container Operator': 'bg-indigo-100 text-indigo-800',
      'Logistics Coordinator': 'bg-pink-100 text-pink-800',
      'Security Officer': 'bg-red-100 text-red-800',
      'IT Support': 'bg-gray-100 text-gray-800',
      'Viewer': 'bg-gray-100 text-gray-600'
    }
    return colors[role] || 'bg-gray-100 text-gray-800'
  }
}

export default userManagementApi