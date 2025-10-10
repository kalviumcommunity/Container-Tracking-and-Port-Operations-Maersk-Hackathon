import { reactive } from 'vue'
import { userManagementApi, userManagementHelpers } from '@/services/userManagementApi'

// Shared store for user management
export const userStore = reactive({
  // State
  users: [],
  loading: false,
  error: null,
  currentPage: 1,
  pageSize: 10,
  totalUsers: 0,
  totalPages: 0,
  systemStats: null,

  // ✅ KEEP ONLY BACKEND API METHODS
  async fetchUsers(params = {}) {
    this.loading = true
    this.error = null
    try {
      const response = await userManagementApi.getUsers({
        pageNumber: params.pageNumber || this.currentPage,
        pageSize: params.pageSize || this.pageSize,
        searchTerm: params.searchTerm,
        role: params.role,
        isActive: params.isActive,
        isBlocked: params.isBlocked
      })
      
      this.users = response.users
      this.totalUsers = response.totalCount
      this.currentPage = response.pageNumber
      this.pageSize = response.pageSize
      this.totalPages = response.totalPages
      
      return response
    } catch (error) {
      this.error = error.message || 'Failed to fetch users'
      throw error
    } finally {
      this.loading = false
    }
  },

  async fetchSystemStats() {
    try {
      this.systemStats = await userManagementApi.getSystemStats()
      return this.systemStats
    } catch (error) {
      throw error
    }
  },

  async updateUserRoles(userId, roles) {
    this.loading = true
    this.error = null
    try {
      const response = await userManagementApi.updateUserRoles(userId, roles)
      if (response.success) {
        // Update local user data
        const userIndex = this.users.findIndex(u => u.userId === userId)
        if (userIndex !== -1) {
          this.users[userIndex] = { ...this.users[userIndex], ...response.data }
        }
      }
      return response
    } catch (error) {
      this.error = error.message || 'Failed to update user roles'
      throw error
    } finally {
      this.loading = false
    }
  },

  async toggleUserBlock(userId, reason = '') {
    this.loading = true
    this.error = null
    try {
      const user = this.users.find(u => u.userId === userId)
      if (!user) throw new Error('User not found')
      
      const response = await userManagementApi.updateUserStatus(userId, !user.isBlocked, reason)
      if (response.success) {
        // Update local user data
        const userIndex = this.users.findIndex(u => u.userId === userId)
        if (userIndex !== -1) {
          this.users[userIndex] = { ...this.users[userIndex], ...response.data }
        }
      }
      return response
    } catch (error) {
      this.error = error.message || 'Failed to update user status'
      throw error
    } finally {
      this.loading = false
    }
  },

  async deleteUser(userId, reason = '') {
    this.loading = true
    this.error = null
    try {
      const response = await userManagementApi.deleteUser(userId, reason)
      if (response.success) {
        // Remove user from local data or mark as deleted
        const userIndex = this.users.findIndex(u => u.userId === userId)
        if (userIndex !== -1) {
          this.users[userIndex].isDeleted = true
          this.users[userIndex].isActive = false
        }
      }
      return response
    } catch (error) {
      this.error = error.message || 'Failed to delete user'
      throw error
    } finally {
      this.loading = false
    }
  },

  // Helper methods for frontend components
  getUserById(userId) {
    return this.users.find(u => u.userId === userId || u.id === userId)
  },

  getAllUsers() {
    return this.users.filter(u => !u.isDeleted)
  },

  getActiveUsers() {
    return this.users.filter(u => u.isActive && !u.isDeleted)
  },

  getFilteredUsers(filters = {}) {
    let filtered = this.users
    
    if (filters.searchTerm) {
      const searchTerm = filters.searchTerm.toLowerCase()
      filtered = filtered.filter(user => 
        user.fullName?.toLowerCase().includes(searchTerm) ||
        user.email?.toLowerCase().includes(searchTerm) ||
        user.username?.toLowerCase().includes(searchTerm) ||
        user.department?.toLowerCase().includes(searchTerm)
      )
    }
    
    if (filters.role) {
      filtered = filtered.filter(user => 
        user.roles && user.roles.includes(filters.role)
      )
    }
    
    if (filters.isActive !== undefined) {
      filtered = filtered.filter(user => user.isActive === filters.isActive)
    }
    
    if (filters.isBlocked !== undefined) {
      filtered = filtered.filter(user => user.isBlocked === filters.isBlocked)
    }
    
    return filtered
  },

  // Initialize store with API data
  async initialize() {
    try {
      await this.fetchUsers()
      await this.fetchSystemStats()
    } catch (error) {
      console.warn('Failed to initialize user store - using fallback data')
    }
  }
})

// Helper functions (keep these as they're useful for UI)
export const roleHelpers = {
  getRoleColor(role) {
    const colors = {
      'Admin': 'bg-red-100 text-red-800',
      'PortManager': 'bg-purple-100 text-purple-800',
      'Operator': 'bg-blue-100 text-blue-800',
      'Viewer': 'bg-gray-100 text-gray-600'
    }
    return colors[role] || 'bg-gray-100 text-gray-800'
  },

  getUserStatusColor(user) {
    if (user.isDeleted) return 'bg-gray-100 text-gray-800'
    if (user.isBlocked) return 'bg-red-100 text-red-800'
    if (!user.isActive) return 'bg-yellow-100 text-yellow-800'
    return 'bg-green-100 text-green-800'
  },

  getUserStatusText(user) {
    if (user.isDeleted) return 'Deleted'
    if (user.isBlocked) return 'Blocked'
    if (!user.isActive) return 'Inactive'
    return 'Active'
  },

  formatDate(dateString) {
    if (!dateString) return 'Never'
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    })
  }
}