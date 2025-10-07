import { reactive } from 'vue'

// Shared store for user management
export const userStore = reactive({
  // Users data
  users: [
    {
      id: 1,
      fullName: 'John Smith',
      email: 'john.smith@maersk.com',
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
      email: 'sarah.johnson@maersk.com',
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
      email: 'michael.chen@maersk.com',
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
      email: 'emily.davis@maersk.com',
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
      email: 'david.wilson@maersk.com',
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
      email: 'admin@maersk.com',
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
      userEmail: 'michael.chen@maersk.com',
      requestedRole: 'Supervisor',
      reason: 'I have been promoted to team lead and need supervisor privileges to manage my team effectively.',
      status: 'pending',
      requestedAt: '2025-10-06T14:30:00Z'
    },
    {
      id: 2,
      userId: 4,
      userName: 'Emily Davis',
      userEmail: 'emily.davis@maersk.com',
      requestedRole: 'Port Manager',
      reason: 'Need access to port management features for the new security protocols implementation.',
      status: 'pending',
      requestedAt: '2025-10-05T10:15:00Z'
    },
    {
      id: 3,
      userId: 2,
      userName: 'Sarah Johnson',
      userEmail: 'sarah.johnson@maersk.com',
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
      userEmail: 'sarah.johnson@maersk.com',
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

  // Actions
  addUser(user) {
    const newUser = {
      ...user,
      id: Math.max(...this.users.map(u => u.id)) + 1,
      createdAt: new Date().toISOString(),
      isActive: true,
      lastLoginAt: null
    }
    this.users.push(newUser)
    return newUser
  },

  updateUser(userId, updates) {
    const userIndex = this.users.findIndex(u => u.id === userId)
    if (userIndex !== -1) {
      this.users[userIndex] = { ...this.users[userIndex], ...updates }
      return this.users[userIndex]
    }
    return null
  },

  toggleUserStatus(userId) {
    const user = this.users.find(u => u.id === userId)
    if (user) {
      user.isActive = !user.isActive
    }
    return user
  },

  updateUserRoles(userId, roles) {
    const user = this.users.find(u => u.id === userId)
    if (user) {
      user.roles = [...roles]
    }
    return user
  },

  deleteUser(userId) {
    const userIndex = this.users.findIndex(u => u.id === userId)
    if (userIndex !== -1) {
      // Mark as deleted instead of removing
      this.users[userIndex].isDeleted = true
      this.users[userIndex].isActive = false
      console.log(`User ${userId} has been deleted`)
      return true
    }
    return false
  },

  permanentDeleteUser(userId) {
    const userIndex = this.users.findIndex(u => u.id === userId)
    if (userIndex !== -1) {
      this.users.splice(userIndex, 1)
      console.log(`User ${userId} has been permanently deleted`)
      return true
    }
    return false
  },

  blockUser(userId) {
    const user = this.users.find(u => u.id === userId)
    if (user) {
      user.isBlocked = !user.isBlocked
      if (user.isBlocked) {
        user.isActive = false
      }
      console.log(`User ${userId} block status changed to:`, user.isBlocked ? 'Blocked' : 'Unblocked')
      return true
    }
    return false
  },

  restoreUser(userId) {
    const user = this.users.find(u => u.id === userId)
    if (user && user.isDeleted) {
      user.isDeleted = false
      user.isActive = true
      console.log(`User ${userId} has been restored`)
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

  getUserById(userId) {
    return this.users.find(u => u.id === userId)
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