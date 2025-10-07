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
  
  // Legacy users data (kept for compatibility with existing components)
  legacyUsers: [
    {
      id: 1,
      fullName: 'John Smith',
      email: 'john.smith@example.com',
      username: 'jsmith',
      department: 'Port Operations',
      phoneNumber: '+1-555-0123',
      roles: ['Port Manager', 'Supervisor'],
      isActive: true,
      isDeleted: false,
      isBlocked: false,
      lastLoginAt: '2025-10-07T09:30:00Z',
      createdAt: '2025-01-15T10:00:00Z'
    },
    {
      id: 2,
      fullName: 'Sarah Johnson',
      email: 'sarah.johnson@example.com',
      username: 'sjohnson',
      department: 'Container Operations',
      phoneNumber: '+1-555-0124',
      roles: ['Container Operator', 'Viewer'],
      isActive: true,
      isDeleted: false,
      isBlocked: false,
      lastLoginAt: '2025-10-07T08:15:00Z',
      createdAt: '2025-02-20T14:30:00Z'
    },
    {
      id: 3,
      fullName: 'Michael Chen',
      email: 'michael.chen@example.com',
      username: 'mchen',
      department: 'Logistics',
      phoneNumber: '+1-555-0125',
      roles: ['Logistics Coordinator'],
      isActive: true,
      isDeleted: false,
      isBlocked: false,
      lastLoginAt: '2025-10-06T16:45:00Z',
      createdAt: '2025-03-10T09:20:00Z'
    },
    {
      id: 4,
      fullName: 'Emily Davis',
      email: 'emily.davis@example.com',
      username: 'edavis',
      department: 'Security',
      phoneNumber: '+1-555-0126',
      roles: ['Security Officer'],
      isActive: false,
      isDeleted: false,
      isBlocked: true,
      lastLoginAt: '2025-10-05T12:30:00Z',
      createdAt: '2025-04-05T11:10:00Z'
    },
    {
      id: 5,
      fullName: 'David Wilson',
      email: 'david.wilson@example.com',
      username: 'dwilson',
      department: 'IT Support',
      phoneNumber: '+1-555-0127',
      roles: ['System Administrator', 'IT Support'],
      isActive: true,
      isDeleted: false,
      isBlocked: false,
      lastLoginAt: '2025-10-07T07:20:00Z',
      createdAt: '2025-01-30T15:45:00Z'
    },
    {
      id: 6,
      fullName: 'Admin User',
      email: 'admin@example.com',
      username: 'admin',
      department: 'Administration',
      phoneNumber: '+1-555-0100',
      roles: ['System Administrator', 'Port Manager'],
      isActive: true,
      isAdmin: true,
      isDeleted: false,
      isBlocked: false,
      lastLoginAt: '2025-10-07T10:00:00Z',
      createdAt: '2025-01-01T00:00:00Z'
    }
  ],

  // Role requests data
  roleRequests: [
    {
      id: 1,
      userId: 3,
      userName: 'Michael Chen',
      userEmail: 'michael.chen@example.com',
      requestedRole: 'Supervisor',
      reason: 'I have been promoted to team lead and need supervisor privileges to manage my team effectively.',
      status: 'pending',
      requestedAt: '2025-10-06T14:30:00Z'
    },
    {
      id: 2,
      userId: 4,
      userName: 'Emily Davis',
      userEmail: 'emily.davis@example.com',
      requestedRole: 'Port Manager',
      reason: 'Need access to port management features for the new security protocols implementation.',
      status: 'pending',
      requestedAt: '2025-10-05T10:15:00Z'
    },
    {
      id: 3,
      userId: 2,
      userName: 'Sarah Johnson',
      userEmail: 'sarah.johnson@example.com',
      requestedRole: 'Container Manager',
      reason: 'Requesting container manager role to handle advanced container operations and reporting.',
      status: 'approved',
      requestedAt: '2025-10-04T16:20:00Z',
      processedAt: '2025-10-05T09:10:00Z'
    },
    {
      id: 4,
      userId: 2,
      userName: 'Sarah Johnson',
      userEmail: 'sarah.johnson@example.com',
      requestedRole: 'Supervisor',
      reason: 'Need supervisor access to manage the new container operations team.',
      status: 'pending',
      requestedAt: '2025-10-06T10:30:00Z'
    }
  ],

  // Available roles
  availableRoles: [
    {
      id: 1,
      name: 'System Administrator',
      level: 'Admin',
      description: 'Full system access with all administrative privileges',
      permissions: ['user_management', 'system_config', 'data_backup', 'security_settings']
    },
    {
      id: 2,
      name: 'Port Manager',
      level: 'Manager',
      description: 'Manage port operations, berths, and ship assignments',
      permissions: ['port_management', 'berth_assignment', 'ship_operations', 'reports']
    },
    {
      id: 3,
      name: 'Container Manager',
      level: 'Manager',
      description: 'Oversee container operations and tracking',
      permissions: ['container_management', 'tracking', 'inventory', 'reports']
    },
    {
      id: 4,
      name: 'Supervisor',
      level: 'Supervisor',
      description: 'Supervise operations and manage team members',
      permissions: ['team_management', 'operations_oversight', 'basic_reports']
    },
    {
      id: 5,
      name: 'Container Operator',
      level: 'Operator',
      description: 'Handle container operations and movements',
      permissions: ['container_operations', 'movement_tracking', 'basic_reports']
    },
    {
      id: 6,
      name: 'Logistics Coordinator',
      level: 'Operator',
      description: 'Coordinate logistics and transportation',
      permissions: ['logistics_coordination', 'transport_planning', 'schedule_management']
    },
    {
      id: 7,
      name: 'Security Officer',
      level: 'Operator',
      description: 'Monitor security and access control',
      permissions: ['security_monitoring', 'access_control', 'incident_reporting']
    },
    {
      id: 8,
      name: 'IT Support',
      level: 'Support',
      description: 'Provide technical support and maintenance',
      permissions: ['technical_support', 'system_maintenance', 'user_assistance']
    },
    {
      id: 9,
      name: 'Viewer',
      level: 'Basic',
      description: 'Read-only access to basic information',
      permissions: ['view_dashboard', 'view_reports']
    }
  ],

  // API Methods
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
      console.error('Error fetching users:', error)
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
      console.error('Error fetching system stats:', error)
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

  async searchUsers(searchTerm, pageNumber = 1) {
    return await this.fetchUsers({ 
      searchTerm, 
      pageNumber,
      pageSize: this.pageSize 
    })
  },

  async getUsersByRole(role, pageNumber = 1) {
    return await this.fetchUsers({ 
      role, 
      pageNumber,
      pageSize: this.pageSize 
    })
  },

  async getBlockedUsers(pageNumber = 1) {
    return await this.fetchUsers({ 
      isBlocked: true, 
      pageNumber,
      pageSize: this.pageSize 
    })
  },

  // Legacy Actions (kept for compatibility)
  addUser(user) {
    const newUser = {
      ...user,
      id: Math.max(...this.legacyUsers.map(u => u.id)) + 1,
      createdAt: new Date().toISOString(),
      isActive: true,
      lastLoginAt: null
    }
    this.legacyUsers.push(newUser)
    return newUser
  },

  updateUser(userId, updates) {
    // Try API first for current users, fallback to legacy
    const userIndex = this.users.findIndex(u => u.userId === userId || u.id === userId)
    if (userIndex !== -1) {
      this.users[userIndex] = { ...this.users[userIndex], ...updates }
      return this.users[userIndex]
    }
    
    // Fallback to legacy users
    const legacyUserIndex = this.legacyUsers.findIndex(u => u.id === userId)
    if (legacyUserIndex !== -1) {
      this.legacyUsers[legacyUserIndex] = { ...this.legacyUsers[legacyUserIndex], ...updates }
      return this.legacyUsers[legacyUserIndex]
    }
    return null
  },

  toggleUserStatus(userId) {
    // Use API method if possible
    if (this.users.some(u => u.userId === userId)) {
      return this.toggleUserBlock(userId)
    }
    
    // Fallback to legacy
    const user = this.legacyUsers.find(u => u.id === userId)
    if (user) {
      user.isActive = !user.isActive
    }
    return user
  },

  // Legacy method kept for compatibility
  legacyUpdateUserRoles(userId, roles) {
    const user = this.legacyUsers.find(u => u.id === userId)
    if (user) {
      user.roles = [...roles]
    }
    return user
  },

  // Legacy delete methods
  legacyDeleteUser(userId) {
    const userIndex = this.legacyUsers.findIndex(u => u.id === userId)
    if (userIndex !== -1) {
      this.legacyUsers[userIndex].isDeleted = true
      this.legacyUsers[userIndex].isActive = false
      return true
    }
    return false
  },

  permanentDeleteUser(userId) {
    const userIndex = this.legacyUsers.findIndex(u => u.id === userId)
    if (userIndex !== -1) {
      this.legacyUsers.splice(userIndex, 1)
      return true
    }
    return false
  },

  blockUser(userId) {
    // Use API method if possible
    if (this.users.some(u => u.userId === userId)) {
      return this.toggleUserBlock(userId)
    }
    
    // Fallback to legacy
    const user = this.legacyUsers.find(u => u.id === userId)
    if (user) {
      user.isBlocked = !user.isBlocked
      if (user.isBlocked) {
        user.isActive = false
      }
      console.log(`Legacy user ${userId} block status changed to:`, user.isBlocked ? 'Blocked' : 'Unblocked')
      return true
    }
    return false
  },

  restoreUser(userId) {
    const user = this.legacyUsers.find(u => u.id === userId)
    if (user && user.isDeleted) {
      user.isDeleted = false
      user.isActive = true
      console.log(`Legacy user ${userId} has been restored`)
      return true
    }
    return false
  },

  addRoleRequest(request) {
    const newRequest = {
      ...request,
      id: Math.max(...this.roleRequests.map(r => r.id)) + 1,
      status: 'pending',
      requestedAt: new Date().toISOString()
    }
    this.roleRequests.push(newRequest)
    return newRequest
  },

  approveRoleRequest(requestId) {
    const request = this.roleRequests.find(r => r.id === requestId)
    if (request) {
      request.status = 'approved'
      request.processedAt = new Date().toISOString()
      
      // Add role to user
      const user = this.users.find(u => u.id === request.userId)
      if (user && !user.roles.includes(request.requestedRole)) {
        user.roles.push(request.requestedRole)
      }
    }
    return request
  },

  rejectRoleRequest(requestId) {
    const request = this.roleRequests.find(r => r.id === requestId)
    if (request) {
      request.status = 'rejected'
      request.processedAt = new Date().toISOString()
    }
    return request
  },

  cancelRoleRequest(requestId) {
    const index = this.roleRequests.findIndex(r => r.id === requestId)
    if (index !== -1) {
      this.roleRequests.splice(index, 1)
      return true
    }
    return false
  },

  // Helper methods
  getUserById(userId) {
    // Check API users first
    let user = this.users.find(u => u.userId === userId || u.id === userId)
    if (user) return user
    
    // Fallback to legacy users
    return this.legacyUsers.find(u => u.id === userId)
  },

  getAllUsers() {
    // Return API users if available, otherwise legacy users
    if (this.users.length > 0) {
      return this.users
    }
    return this.legacyUsers.filter(u => !u.isDeleted)
  },

  getActiveUsers() {
    if (this.users.length > 0) {
      return this.users.filter(u => u.isActive && !u.isDeleted)
    }
    return this.legacyUsers.filter(u => u.isActive && !u.isDeleted)
  },

  getFilteredUsers(filters = {}) {
    const allUsers = this.getAllUsers()
    
    return allUsers.filter(user => {
      if (filters.searchTerm) {
        const searchTerm = filters.searchTerm.toLowerCase()
        const matchesSearch = 
          user.fullName?.toLowerCase().includes(searchTerm) ||
          user.email?.toLowerCase().includes(searchTerm) ||
          user.username?.toLowerCase().includes(searchTerm) ||
          user.department?.toLowerCase().includes(searchTerm)
        if (!matchesSearch) return false
      }
      
      if (filters.role) {
        if (!user.roles.includes(filters.role)) return false
      }
      
      if (filters.isActive !== undefined) {
        if (user.isActive !== filters.isActive) return false
      }
      
      if (filters.isBlocked !== undefined) {
        if (user.isBlocked !== filters.isBlocked) return false
      }
      
      if (filters.isDeleted !== undefined) {
        if (user.isDeleted !== filters.isDeleted) return false
      }
      
      return true
    })
  },

  // Initialize store with API data
  async initialize() {
    try {
      await this.fetchUsers()
      await this.fetchSystemStats()
    } catch (error) {
      console.warn('Failed to initialize user store with API data, using legacy data:', error)
    }
  },

  getRoleByName(roleName) {
    return this.availableRoles.find(r => r.name === roleName)
  },

  getUserCountForRole(roleName) {
    return this.users.filter(user => user.roles.includes(roleName)).length
  },

  getPendingRequests() {
    return this.roleRequests.filter(request => request.status === 'pending')
  },

  getRequestsForUser(userId) {
    return this.roleRequests
      .filter(request => request.userId === userId)
      .sort((a, b) => new Date(b.requestedAt) - new Date(a.requestedAt))
  }
})

// Helper functions
export const roleHelpers = {
  getRoleColor(role) {
    const colors = {
      'System Administrator': 'bg-purple-100 text-purple-800',
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
  },

  getRequestStatusColor(status) {
    const colors = {
      'pending': 'bg-yellow-100 text-yellow-800',
      'approved': 'bg-green-100 text-green-800',
      'rejected': 'bg-red-100 text-red-800'
    }
    return colors[status] || 'bg-gray-100 text-gray-800'
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
  },

  formatPermission(permission) {
    return permission.replace(/_/g, ' ').replace(/\b\w/g, l => l.toUpperCase())
  }
}