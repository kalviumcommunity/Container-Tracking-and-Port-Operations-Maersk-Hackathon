<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Admin Header -->
    <div class="bg-gradient-to-r from-blue-900 to-blue-700 shadow-lg">
      <div class="max-w-7xl mx-auto px-6 py-8">
        <div class="flex justify-between items-center">
          <div>
            <h1 class="text-3xl font-bold text-white">Administration Dashboard</h1>
            <p class="text-blue-100 mt-2">User Management & Role Administration</p>
          </div>
          <div class="flex items-center space-x-4">
            <div class="bg-blue-800 rounded-lg px-4 py-2">
              <div class="text-blue-100 text-sm">Total Users</div>
              <div class="text-white text-2xl font-bold">{{ filteredUsers.length }}</div>
            </div>
            <div class="bg-blue-800 rounded-lg px-4 py-2">
              <div class="text-blue-100 text-sm">Pending Requests</div>
              <div class="text-white text-2xl font-bold">{{ allPendingRequests.length }}</div>
            </div>
            <button
              @click="showAddUserModal = true"
              class="bg-green-600 hover:bg-green-700 text-white px-6 py-3 rounded-lg flex items-center space-x-2 transition-colors"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
              </svg>
              <span>Add New User</span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto px-6 py-6">
      <!-- Tabs Navigation -->
      <div class="bg-white rounded-lg shadow-md overflow-hidden mb-6">
        <div class="border-b border-gray-200">
          <nav class="flex space-x-8 px-6">
            <button
              @click="activeTab = 'users'"
              :class="[
                'py-4 px-2 border-b-2 font-medium text-sm transition-colors',
                activeTab === 'users' 
                  ? 'border-blue-500 text-blue-600' 
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              ]"
            >
              <div class="flex items-center space-x-2">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z"></path>
                </svg>
                <span>User Management</span>
                <span class="bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded-full ml-2">
                  {{ users.length }}
                </span>
              </div>
            </button>
            <button
              @click="activeTab = 'requests'"
              :class="[
                'py-4 px-2 border-b-2 font-medium text-sm transition-colors',
                activeTab === 'requests' 
                  ? 'border-blue-500 text-blue-600' 
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              ]"
            >
              <div class="flex items-center space-x-2">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01"></path>
                </svg>
                <span>Role Requests</span>
                <span v-if="allPendingRequests.length > 0" class="bg-orange-100 text-orange-800 text-xs px-2 py-1 rounded-full ml-2">
                  {{ allPendingRequests.length }}
                </span>
              </div>
            </button>
            <button
              @click="activeTab = 'analytics'"
              :class="[
                'py-4 px-2 border-b-2 font-medium text-sm transition-colors',
                activeTab === 'analytics' 
                  ? 'border-blue-500 text-blue-600' 
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              ]"
            >
              <div class="flex items-center space-x-2">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
                </svg>
                <span>Analytics</span>
              </div>
            </button>
          </nav>
        </div>
      </div>

      <!-- User Management Tab -->
      <div v-if="activeTab === 'users'">
        <!-- Search and Filters -->
        <div class="bg-white rounded-lg shadow-md p-6 mb-6">
          <div class="flex flex-col lg:flex-row gap-4">
            <div class="flex-1">
              <div class="relative">
                <svg class="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
                </svg>
                <input
                  v-model="searchQuery"
                  type="text"
                  placeholder="Search users by name, email, or department..."
                  class="w-full pl-10 pr-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                />
              </div>
            </div>
            <select
              v-model="selectedRole"
              class="px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="">All Roles</option>
              <option v-for="role in availableRoles" :key="role.id" :value="role.name">
                {{ role.name }}
              </option>
            </select>
            <select
              v-model="statusFilter"
              class="px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="">All Status</option>
              <option value="active">Active</option>
              <option value="inactive">Inactive</option>
              <option value="blocked">Blocked</option>
              <option value="deleted">Deleted</option>
            </select>
          </div>
        </div>

        <!-- Users Grid -->
        <div class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
          <div
            v-for="user in filteredUsers"
            :key="user.id"
            class="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow border-l-4"
            :class="getUserCardBorderColor(user)"
          >
            <div class="p-6">
              <!-- User Header -->
              <div class="flex items-start justify-between mb-4">
                <div class="flex items-center space-x-3">
                  <div class="flex-shrink-0 h-12 w-12">
                    <div class="h-12 w-12 rounded-full bg-gradient-to-r from-blue-500 to-blue-600 flex items-center justify-center text-white font-semibold text-lg">
                      {{ user.fullName.charAt(0).toUpperCase() }}
                    </div>
                  </div>
                  <div>
                    <h3 class="text-lg font-semibold text-gray-900">{{ user.fullName }}</h3>
                    <p class="text-sm text-gray-600">{{ user.email }}</p>
                  </div>
                </div>
                <span 
                  :class="getUserStatusColor(user)"
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                >
                  {{ getUserStatusText(user) }}
                </span>
              </div>

              <!-- User Details -->
              <div class="space-y-2 mb-4">
                <div class="flex items-center text-sm">
                  <svg class="w-4 h-4 text-gray-400 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-2m-2 0H7m5 0H7m0 0H5m2 0h.01M7 8h6m-6 4h6m-6 4h6"></path>
                  </svg>
                  <span class="text-gray-700">{{ user.department || 'No Department' }}</span>
                </div>
                <div class="flex items-center text-sm">
                  <svg class="w-4 h-4 text-gray-400 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21L6.5 11.5a10.99 10.99 0 002.25 2.25l2.613-1.226a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"></path>
                  </svg>
                  <span class="text-gray-700">{{ user.phoneNumber || 'No Phone' }}</span>
                </div>
                <div class="flex items-center text-sm">
                  <svg class="w-4 h-4 text-gray-400 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                  </svg>
                  <span class="text-gray-700">{{ formatDate(user.lastLoginAt) }}</span>
                </div>
              </div>

              <!-- User Roles -->
              <div class="mb-4">
                <h4 class="text-sm font-medium text-gray-700 mb-2">Roles:</h4>
                <div class="flex flex-wrap gap-1">
                  <span
                    v-for="role in user.roles"
                    :key="role"
                    :class="getRoleColor(role)"
                    class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                  >
                    {{ role }}
                  </span>
                </div>
              </div>

              <!-- Action Buttons -->
              <div class="flex flex-wrap gap-2">
                <button
                  @click="manageUserRoles(user)"
                  class="flex-1 bg-blue-600 hover:bg-blue-700 text-white px-3 py-2 rounded-md text-sm font-medium transition-colors"
                >
                  Manage Roles
                </button>
                <button
                  v-if="!user.isBlocked"
                  @click="blockUser(user)"
                  class="bg-yellow-600 hover:bg-yellow-700 text-white px-3 py-2 rounded-md text-sm font-medium transition-colors"
                >
                  Block
                </button>
                <button
                  v-else
                  @click="blockUser(user)"
                  class="bg-green-600 hover:bg-green-700 text-white px-3 py-2 rounded-md text-sm font-medium transition-colors"
                >
                  Unblock
                </button>
                <button
                  v-if="!user.isDeleted"
                  @click="deleteUser(user)"
                  class="bg-red-600 hover:bg-red-700 text-white px-3 py-2 rounded-md text-sm font-medium transition-colors"
                >
                  Delete
                </button>
                <button
                  v-else
                  @click="restoreUser(user)"
                  class="bg-green-600 hover:bg-green-700 text-white px-3 py-2 rounded-md text-sm font-medium transition-colors"
                >
                  Restore
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Role Requests Tab -->
      <div v-if="activeTab === 'requests'" class="space-y-6">
        <div v-if="allPendingRequests.length === 0" class="text-center py-12">
          <div class="bg-white rounded-lg shadow-md p-8">
            <svg class="mx-auto h-16 w-16 text-gray-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"></path>
            </svg>
            <h3 class="text-xl font-medium text-gray-900 mb-2">No Pending Requests</h3>
            <p class="text-gray-500">All role requests have been processed.</p>
          </div>
        </div>

        <div v-for="request in allPendingRequests" :key="request.id" class="bg-white rounded-lg shadow-md border-l-4 border-orange-400">
          <div class="p-6">
            <div class="flex items-center justify-between mb-4">
              <div class="flex items-center space-x-4">
                <div class="flex-shrink-0 h-12 w-12">
                  <div class="h-12 w-12 rounded-full bg-gradient-to-r from-blue-500 to-blue-600 flex items-center justify-center text-white font-semibold">
                    {{ request.userName.charAt(0).toUpperCase() }}
                  </div>
                </div>
                <div>
                  <h3 class="text-lg font-medium text-gray-900">{{ request.userName }}</h3>
                  <p class="text-sm text-gray-500">{{ request.userEmail }}</p>
                </div>
              </div>
              <div class="text-right">
                <div class="text-sm text-gray-500">{{ formatDate(request.requestedAt) }}</div>
                <span :class="getRequestStatusColor(request.status)" 
                      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium mt-1">
                  {{ request.status.toUpperCase() }}
                </span>
              </div>
            </div>

            <div class="bg-gray-50 rounded-lg p-4 mb-4">
              <div class="flex items-center space-x-2 mb-2">
                <span class="text-sm font-medium text-gray-700">Requested Role:</span>
                <span :class="getRoleColor(request.requestedRole)" 
                      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium">
                  {{ request.requestedRole }}
                </span>
              </div>
              <div class="text-sm text-gray-700">
                <span class="font-medium">Reason:</span>
                <p class="mt-1">{{ request.reason }}</p>
              </div>
            </div>

            <div class="flex space-x-3">
              <button
                @click="approveRequest(request)"
                class="flex-1 bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-md text-sm font-medium transition-colors flex items-center justify-center space-x-2"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
                </svg>
                <span>Approve Request</span>
              </button>
              <button
                @click="rejectRequest(request)"
                class="flex-1 bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-md text-sm font-medium transition-colors flex items-center justify-center space-x-2"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                </svg>
                <span>Reject Request</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Analytics Tab -->
      <div v-if="activeTab === 'analytics'" class="space-y-6">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <!-- Stats Cards -->
          <div class="bg-white rounded-lg shadow-md p-6">
            <div class="flex items-center">
              <div class="p-3 rounded-full bg-blue-100">
                <svg class="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z"></path>
                </svg>
              </div>
              <div class="ml-4">
                <p class="text-sm font-medium text-gray-600">Total Users</p>
                <p class="text-2xl font-semibold text-gray-900">{{ users.length }}</p>
              </div>
            </div>
          </div>

          <div class="bg-white rounded-lg shadow-md p-6">
            <div class="flex items-center">
              <div class="p-3 rounded-full bg-green-100">
                <svg class="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
              </div>
              <div class="ml-4">
                <p class="text-sm font-medium text-gray-600">Active Users</p>
                <p class="text-2xl font-semibold text-gray-900">{{ activeUsersCount }}</p>
              </div>
            </div>
          </div>

          <div class="bg-white rounded-lg shadow-md p-6">
            <div class="flex items-center">
              <div class="p-3 rounded-full bg-yellow-100">
                <svg class="w-6 h-6 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.864-.833-2.634 0L3.34 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
                </svg>
              </div>
              <div class="ml-4">
                <p class="text-sm font-medium text-gray-600">Blocked Users</p>
                <p class="text-2xl font-semibold text-gray-900">{{ blockedUsersCount }}</p>
              </div>
            </div>
          </div>

          <div class="bg-white rounded-lg shadow-md p-6">
            <div class="flex items-center">
              <div class="p-3 rounded-full bg-orange-100">
                <svg class="w-6 h-6 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"></path>
                </svg>
              </div>
              <div class="ml-4">
                <p class="text-sm font-medium text-gray-600">Pending Requests</p>
                <p class="text-2xl font-semibold text-gray-900">{{ allPendingRequests.length }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Role Distribution Chart -->
        <div class="bg-white rounded-lg shadow-md p-6">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Role Distribution</h3>
          <div class="space-y-3">
            <div v-for="role in roleDistribution" :key="role.name" class="flex items-center justify-between">
              <div class="flex items-center space-x-3">
                <span :class="getRoleColor(role.name)" class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium">
                  {{ role.name }}
                </span>
              </div>
              <div class="flex items-center space-x-2">
                <div class="w-32 bg-gray-200 rounded-full h-2">
                  <div 
                    class="bg-blue-600 h-2 rounded-full" 
                    :style="{ width: role.percentage + '%' }"
                  ></div>
                </div>
                <span class="text-sm font-medium text-gray-900 w-8">{{ role.count }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Add User Modal -->
    <div v-if="showAddUserModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Add New User</h3>
          <form @submit.prevent="addUser" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700">Full Name</label>
              <input v-model="newUser.fullName" type="text" required 
                     class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500">
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Email</label>
              <input v-model="newUser.email" type="email" required 
                     class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500">
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Department</label>
              <input v-model="newUser.department" type="text" 
                     class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500">
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Phone Number</label>
              <input v-model="newUser.phoneNumber" type="tel" 
                     class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500">
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Initial Role</label>
              <select v-model="newUser.initialRole" 
                      class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500">
                <option value="Viewer">Viewer</option>
                <option value="Container Operator">Container Operator</option>
                <option value="Logistics Coordinator">Logistics Coordinator</option>
                <option value="Security Officer">Security Officer</option>
              </select>
            </div>
            <div class="flex justify-end space-x-2 pt-4">
              <button type="button" @click="showAddUserModal = false" 
                      class="px-4 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50">
                Cancel
              </button>
              <button type="submit" 
                      class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700">
                Add User
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Role Management Modal -->
    <div v-if="showRoleModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Manage Roles - {{ selectedUser?.fullName }}</h3>
          <div class="space-y-3">
            <div v-for="role in availableRoles" :key="role.id" class="flex items-center justify-between">
              <span class="text-sm text-gray-700">{{ role.name }}</span>
              <label class="inline-flex items-center">
                <input
                  type="checkbox"
                  :checked="selectedUser?.roles.includes(role.name)"
                  @change="toggleRole(role.name, $event.target.checked)"
                  class="form-checkbox h-4 w-4 text-blue-600"
                />
                <span class="ml-2 text-sm text-gray-600">{{ role.level }}</span>
              </label>
            </div>
          </div>
          <div class="flex justify-end space-x-2 pt-6">
            <button @click="showRoleModal = false" 
                    class="px-4 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50">
              Cancel
            </button>
            <button @click="saveUserRoles" 
                    class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700">
              Save Changes
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { userStore, roleHelpers } from '@/stores/userStore'

// Reactive data
const activeTab = ref('users')
const searchQuery = ref('')
const selectedRole = ref('')
const statusFilter = ref('')
const showAddUserModal = ref(false)
const showRoleModal = ref(false)
const selectedUser = ref(null)

// Form data
const newUser = ref({
  fullName: '',
  email: '',
  department: '',
  phoneNumber: '',
  initialRole: 'Viewer'
})

// Computed properties using the store
const users = computed(() => userStore.users.filter(user => !user.isDeleted || statusFilter.value === 'deleted'))
const roleRequests = computed(() => userStore.roleRequests)
const availableRoles = computed(() => userStore.availableRoles)

const allPendingRequests = computed(() => {
  return userStore.getPendingRequests()
})

const filteredUsers = computed(() => {
  let filtered = users.value

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(user => 
      user.fullName.toLowerCase().includes(query) ||
      user.email.toLowerCase().includes(query) ||
      user.department?.toLowerCase().includes(query)
    )
  }

  if (selectedRole.value) {
    filtered = filtered.filter(user => user.roles.includes(selectedRole.value))
  }

  if (statusFilter.value) {
    filtered = filtered.filter(user => {
      if (statusFilter.value === 'active') return user.isActive && !user.isBlocked && !user.isDeleted
      if (statusFilter.value === 'inactive') return !user.isActive && !user.isBlocked && !user.isDeleted
      if (statusFilter.value === 'blocked') return user.isBlocked
      if (statusFilter.value === 'deleted') return user.isDeleted
      return true
    })
  }

  return filtered
})

const activeUsersCount = computed(() => 
  users.value.filter(user => user.isActive && !user.isBlocked && !user.isDeleted).length
)

const blockedUsersCount = computed(() => 
  users.value.filter(user => user.isBlocked).length
)

const roleDistribution = computed(() => {
  const roleCount = {}
  const totalUsers = users.value.filter(user => !user.isDeleted).length
  
  users.value.forEach(user => {
    if (!user.isDeleted) {
      user.roles.forEach(role => {
        roleCount[role] = (roleCount[role] || 0) + 1
      })
    }
  })
  
  return Object.entries(roleCount).map(([name, count]) => ({
    name,
    count,
    percentage: totalUsers > 0 ? Math.round((count / totalUsers) * 100) : 0
  })).sort((a, b) => b.count - a.count)
})

// Methods using store functions and role helpers
const getRoleColor = roleHelpers.getRoleColor
const getRequestStatusColor = roleHelpers.getRequestStatusColor
const formatDate = roleHelpers.formatDate
const getUserStatusColor = roleHelpers.getUserStatusColor
const getUserStatusText = roleHelpers.getUserStatusText

const getUserCardBorderColor = (user) => {
  if (user.isDeleted) return 'border-gray-400'
  if (user.isBlocked) return 'border-red-400'
  if (!user.isActive) return 'border-yellow-400'
  return 'border-green-400'
}

const addUser = () => {
  const userData = {
    fullName: newUser.value.fullName,
    email: newUser.value.email,
    username: newUser.value.email.split('@')[0],
    department: newUser.value.department,
    phoneNumber: newUser.value.phoneNumber,
    roles: [newUser.value.initialRole]
  }
  
  userStore.addUser(userData)
  showAddUserModal.value = false
  
  // Reset form
  newUser.value = {
    fullName: '',
    email: '',
    department: '',
    phoneNumber: '',
    initialRole: 'Viewer'
  }
}

const manageUserRoles = (user) => {
  selectedUser.value = { ...user }
  showRoleModal.value = true
}

const toggleRole = (roleName, isChecked) => {
  if (!selectedUser.value) return
  
  if (isChecked) {
    if (!selectedUser.value.roles.includes(roleName)) {
      selectedUser.value.roles.push(roleName)
    }
  } else {
    selectedUser.value.roles = selectedUser.value.roles.filter(role => role !== roleName)
  }
}

const saveUserRoles = () => {
  if (!selectedUser.value) return
  
  userStore.updateUserRoles(selectedUser.value.id, selectedUser.value.roles)
  showRoleModal.value = false
  selectedUser.value = null
}

const deleteUser = (user) => {
  if (confirm(`Are you sure you want to delete ${user.fullName}? This action can be undone by restoring the user.`)) {
    userStore.deleteUser(user.id)
  }
}

const restoreUser = (user) => {
  if (confirm(`Are you sure you want to restore ${user.fullName}?`)) {
    userStore.restoreUser(user.id)
  }
}

const blockUser = (user) => {
  const action = user.isBlocked ? 'unblock' : 'block'
  if (confirm(`Are you sure you want to ${action} ${user.fullName}?`)) {
    userStore.blockUser(user.id)
  }
}

const approveRequest = (request) => {
  if (confirm(`Approve role request for ${request.requestedRole} by ${request.userName}?`)) {
    userStore.approveRoleRequest(request.id)
  }
}

const rejectRequest = (request) => {
  if (confirm(`Reject role request for ${request.requestedRole} by ${request.userName}?`)) {
    userStore.rejectRoleRequest(request.id)
  }
}

onMounted(() => {
  console.log('Admin User Management component mounted')
})
</script>

<style scoped>
/* Custom scrollbar for tables */
.overflow-x-auto::-webkit-scrollbar {
  height: 6px;
}

.overflow-x-auto::-webkit-scrollbar-track {
  background: #f1f1f1;
}

.overflow-x-auto::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 3px;
}

.overflow-x-auto::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}

/* Smooth transitions */
.transition-colors {
  transition: background-color 0.2s ease-in-out, color 0.2s ease-in-out;
}

.transition-shadow {
  transition: box-shadow 0.2s ease-in-out;
}
</style>