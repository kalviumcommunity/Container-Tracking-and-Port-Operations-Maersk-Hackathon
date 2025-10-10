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
          <!-- Add navigation to AdminPanel -->
          <div class="flex items-center space-x-4">
            <router-link 
              to="/admin/roles" 
              class="bg-blue-800 hover:bg-blue-700 text-white px-4 py-2 rounded-lg transition-colors"
            >
              Role Requests
              <span v-if="pendingApplicationsCount > 0" class="ml-2 bg-orange-500 text-xs px-2 py-1 rounded-full">
                {{ pendingApplicationsCount }}
              </span>
            </router-link>
            <div class="bg-blue-800 rounded-lg px-4 py-2">
              <div class="text-blue-100 text-sm">Total Users</div>
              <div class="text-white text-2xl font-bold">
                {{ systemStats?.totalUsers || users.length }}
              </div>
            </div>
            <div class="bg-blue-800 rounded-lg px-4 py-2">
              <div class="text-blue-100 text-sm">Active Users</div>
              <div class="text-white text-2xl font-bold">
                {{ systemStats?.activeUsers || users.filter(u => u.isActive && !u.isBlocked).length }}
              </div>
            </div>
            <div class="bg-blue-800 rounded-lg px-4 py-2">
              <div class="text-blue-100 text-sm">Blocked Users</div>
              <div class="text-white text-2xl font-bold">
                {{ systemStats?.blockedUsers || blockedUsersCount }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto px-6 py-6">
      <!-- Error Message -->
      <div v-if="errorMessage" class="bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
        <div class="flex">
          <svg class="w-5 h-5 text-red-400 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
          </svg>
          <div class="ml-3">
            <p class="text-sm text-red-800">{{ errorMessage }}</p>
            <button @click="errorMessage = ''" class="mt-2 text-xs text-red-600 underline hover:text-red-500">
              Dismiss
            </button>
          </div>
        </div>
      </div>

      <!-- Loading Spinner -->
      <div v-if="isLoading" class="bg-blue-50 border border-blue-200 rounded-lg p-8 mb-6">
        <div class="flex items-center justify-center">
          <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
          <span class="ml-3 text-blue-700">Loading users...</span>
        </div>
      </div>

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
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4"></path>
                </svg>
                <span>Role Requests</span>
                <span v-if="pendingRequests.length > 0" class="bg-orange-100 text-orange-800 text-xs px-2 py-1 rounded-full ml-2">
                  {{ pendingRequests.length }}
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
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 01-2 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
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
              <option value="Admin">Admin</option>
              <option value="PortManager">Port Manager</option>
              <option value="Operator">Operator</option>
              <option value="Viewer">Viewer</option>
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
            :key="user.userId || user.id"
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
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                </svg>
              </div>
              <div class="ml-4">
                <p class="text-sm font-medium text-gray-600">Deleted Users</p>
                <p class="text-2xl font-semibold text-gray-900">{{ deletedUsersCount }}</p>
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

      <!-- Role Requests Tab -->
      <div v-if="activeTab === 'requests'" class="space-y-6">
        <!-- Pending Requests -->
        <div class="bg-white rounded-lg shadow-md">
          <div class="px-6 py-4 border-b border-gray-200">
            <div class="flex items-center justify-between">
              <h3 class="text-lg font-medium text-gray-900">Pending Role Requests</h3>
              <span class="bg-orange-100 text-orange-800 text-sm px-3 py-1 rounded-full">
                {{ pendingRequests.length }} pending
              </span>
            </div>
          </div>

          <div v-if="pendingRequests.length === 0" class="p-12 text-center">
            <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4"></path>
            </svg>
            <h3 class="mt-2 text-sm font-medium text-gray-900">No pending requests</h3>
            <p class="mt-1 text-sm text-gray-500">All role requests have been processed.</p>
          </div>

          <div v-else class="divide-y divide-gray-200">
            <div v-for="request in pendingRequests" :key="request.applicationId" class="p-6">
              <div class="flex items-start justify-between">
                <div class="flex-1">
                  <div class="flex items-center space-x-3 mb-3">
                    <div class="h-10 w-10 bg-blue-100 rounded-full flex items-center justify-center">
                      <span class="text-sm font-medium text-blue-800">{{ request.fullName.charAt(0).toUpperCase() }}</span>
                    </div>
                    <div>
                      <h4 class="text-lg font-medium text-gray-900">{{ request.fullName }}</h4>
                      <p class="text-sm text-gray-600">{{ request.username }}</p>
                    </div>
                  </div>
                  
                  <div class="mb-4">
                    <div class="flex items-center space-x-2 mb-2">
                      <span class="text-sm text-gray-600">Requesting role:</span>
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                        {{ request.requestedRole }}
                      </span>
                    </div>
                    <div class="text-sm text-gray-600">
                      <span class="font-medium">Justification:</span>
                      <p class="mt-1 text-gray-700">{{ request.justification }}</p>
                    </div>
                  </div>

                  <div class="text-xs text-gray-500">
                    Requested on {{ formatDate(request.requestedAt) }}
                  </div>
                </div>

                <div class="flex space-x-2 ml-6">
                  <button
                    @click="approveRequest(request.applicationId)"
                    class="inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
                  >
                    <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
                    </svg>
                    Approve
                  </button>
                  <button
                    @click="rejectRequest(request.applicationId)"
                    class="inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-white bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
                  >
                    <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                    </svg>
                    Reject
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Recent Decisions -->
        <div class="bg-white rounded-lg shadow-md">
          <div class="px-6 py-4 border-b border-gray-200">
            <h3 class="text-lg font-medium text-gray-900">Recent Decisions</h3>
          </div>
          
          <div v-if="recentRequests.length === 0" class="p-12 text-center">
            <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"></path>
            </svg>
            <h3 class="mt-2 text-sm font-medium text-gray-900">No recent decisions</h3>
            <p class="mt-1 text-sm text-gray-500">Role request decisions will appear here.</p>
          </div>

          <div v-else class="divide-y divide-gray-200">
            <div v-for="request in recentRequests" :key="request.applicationId" class="p-6">
              <div class="flex items-start justify-between">
                <div class="flex-1">
                  <div class="flex items-center space-x-3 mb-2">
                    <div class="h-8 w-8 bg-gray-100 rounded-full flex items-center justify-center">
                      <span class="text-xs font-medium text-gray-600">{{ request.fullName.charAt(0).toUpperCase() }}</span>
                    </div>
                    <div>
                      <span class="text-sm font-medium text-gray-900">{{ request.fullName }}</span>
                      <span class="text-sm text-gray-600 ml-2">requested</span>
                      <span class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800 ml-1">
                        {{ request.requestedRole }}
                      </span>
                    </div>
                  </div>
                  <div class="text-xs text-gray-500">
                    {{ formatDate(request.reviewedAt) }}
                  </div>
                </div>
                <div class="flex items-center">
                  <span :class="[
                    'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                    request.status === 'Approved' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                  ]">
                    {{ request.status }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Role Management Modal -->
    <div v-if="showRoleModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Manage Roles - {{ selectedUser?.fullName }}</h3>
          <div class="space-y-3">
            <div v-for="role in backendRoles" :key="role" class="flex items-center justify-between">
              <span class="text-sm text-gray-700">{{ role }}</span>
              <label class="inline-flex items-center">
                <input
                  type="checkbox"
                  :checked="selectedUser?.roles.includes(role)"
                  @change="toggleRole(role, $event.target.checked)"
                  class="form-checkbox h-4 w-4 text-blue-600"
                />
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
import { ref, computed, onMounted, watch } from 'vue'
import { userManagementApi } from '@/services/userManagementApi'
import { roleApplicationApi } from '@/services/api'

// Reactive data
const activeTab = ref('users')
const searchQuery = ref('')
const selectedRole = ref('')
const statusFilter = ref('')
const showRoleModal = ref(false)
const selectedUser = ref(null)
const isLoading = ref(false)
const errorMessage = ref('')
const users = ref([])
const systemStats = ref(null)
const pendingRequests = ref([])
const recentRequests = ref([])
const pendingApplicationsCount = ref(0)

// Backend roles (matching our actual backend constants)
const backendRoles = ['Admin', 'PortManager', 'Operator', 'Viewer']

// API Methods
const loadUsers = async () => {
  try {
    isLoading.value = true
    errorMessage.value = ''
    const response = await userManagementApi.getUsers() // ✅ FETCHING FROM BACKEND
    users.value = response.users || []
  } catch (error) {
    errorMessage.value = 'Failed to load users: ' + error.message
    console.error('Error loading users:', error)
  } finally {
    isLoading.value = false
  }
}

const loadSystemStats = async () => {
  try {
    const stats = await userManagementApi.getSystemStats()
    systemStats.value = stats
  } catch (error) {
    console.error('Error loading system stats:', error)
  }
}

const performSearch = async () => {
  if (!searchQuery.value.trim()) {
    await loadUsers()
    return
  }
  
  try {
    isLoading.value = true
    const response = await userManagementApi.searchUsers(searchQuery.value.trim())
    users.value = response.users || []
  } catch (error) {
    errorMessage.value = 'Search failed: ' + error.message
  } finally {
    isLoading.value = false
  }
}

// Computed properties
const filteredUsers = computed(() => {
  let filtered = users.value

  // Apply search filter
  if (searchQuery.value.trim()) {
    const search = searchQuery.value.toLowerCase()
    filtered = filtered.filter(user => 
      user.fullName.toLowerCase().includes(search) ||
      user.email.toLowerCase().includes(search) ||
      (user.department && user.department.toLowerCase().includes(search))
    )
  }

  // Apply role filter
  if (selectedRole.value) {
    filtered = filtered.filter(user => 
      user.roles && user.roles.includes(selectedRole.value)
    )
  }

  // Apply status filter
  if (statusFilter.value) {
    switch (statusFilter.value) {
      case 'active':
        filtered = filtered.filter(user => user.isActive && !user.isBlocked && !user.isDeleted)
        break
      case 'inactive':
        filtered = filtered.filter(user => !user.isActive && !user.isBlocked && !user.isDeleted)
        break
      case 'blocked':
        filtered = filtered.filter(user => user.isBlocked)
        break
      case 'deleted':
        filtered = filtered.filter(user => user.isDeleted)
        break
    }
  } else {
    // By default, don't show deleted users unless specifically filtered
    filtered = filtered.filter(user => !user.isDeleted)
  }

  return filtered
})

const activeUsersCount = computed(() => 
  users.value.filter(user => user.isActive && !user.isBlocked && !user.isDeleted).length
)

const blockedUsersCount = computed(() => 
  users.value.filter(user => user.isBlocked).length
)

const deletedUsersCount = computed(() => 
  users.value.filter(user => user.isDeleted).length
)

const roleDistribution = computed(() => {
  const roleCount = {}
  const totalUsers = users.value.filter(user => !user.isDeleted).length
  
  users.value.forEach(user => {
    if (!user.isDeleted && user.roles) {
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

// Helper functions
const formatDate = (dateString) => {
  if (!dateString) return 'Never'
  const date = new Date(dateString)
  return isNaN(date.getTime()) ? 'Invalid Date' : date.toLocaleDateString()
}

const getRoleColor = (role) => {
  const colors = {
    'Admin': 'bg-red-100 text-red-800',
    'PortManager': 'bg-purple-100 text-purple-800',
    'Operator': 'bg-blue-100 text-blue-800',
    'Viewer': 'bg-gray-100 text-gray-800'
  }
  return colors[role] || 'bg-gray-100 text-gray-800'
}

const getUserStatusColor = (user) => {
  if (user.isDeleted) return 'bg-gray-100 text-gray-800'
  if (user.isBlocked) return 'bg-red-100 text-red-800'
  if (!user.isActive) return 'bg-yellow-100 text-yellow-800'
  return 'bg-green-100 text-green-800'
}

const getUserStatusText = (user) => {
  if (user.isDeleted) return 'Deleted'
  if (user.isBlocked) return 'Blocked'
  if (!user.isActive) return 'Inactive'
  return 'Active'
}

const getUserCardBorderColor = (user) => {
  if (user.isDeleted) return 'border-gray-400'
  if (user.isBlocked) return 'border-red-400'
  if (!user.isActive) return 'border-yellow-400'
  return 'border-green-400'
}

// User management actions
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

const saveUserRoles = async () => {
  if (!selectedUser.value) return
  
  try {
    isLoading.value = true
    const userId = selectedUser.value.userId || selectedUser.value.id
    
    await userManagementApi.updateUserRoles(userId, { roles: selectedUser.value.roles })
    
    // Update the user in our local array
    const userIndex = users.value.findIndex(u => (u.userId || u.id) === userId)
    if (userIndex !== -1) {
      users.value[userIndex].roles = [...selectedUser.value.roles]
    }
    
    showRoleModal.value = false
    selectedUser.value = null
  } catch (error) {
    errorMessage.value = 'Failed to update user roles: ' + error.message
  } finally {
    isLoading.value = false
  }
}

const deleteUser = async (user) => {
  if (!confirm(`Are you sure you want to delete ${user.fullName}? This action can be undone by restoring the user.`)) {
    return
  }
  
  try {
    isLoading.value = true
    const userId = user.userId || user.id
    
    await userManagementApi.deleteUser(userId)
    
    // Update the user in our local array
    const userIndex = users.value.findIndex(u => (u.userId || u.id) === userId)
    if (userIndex !== -1) {
      users.value[userIndex].isDeleted = true
    }
  } catch (error) {
    errorMessage.value = 'Failed to delete user: ' + error.message
  } finally {
    isLoading.value = false
  }
}

const restoreUser = async (user) => {
  if (!confirm(`Are you sure you want to restore ${user.fullName}?`)) {
    return
  }
  
  try {
    isLoading.value = true
    const userId = user.userId || user.id
    
    await userManagementApi.restoreUser(userId)
    
    // Update the user in our local array
    const userIndex = users.value.findIndex(u => (u.userId || u.id) === userId)
    if (userIndex !== -1) {
      users.value[userIndex].isDeleted = false
    }
  } catch (error) {
    errorMessage.value = 'Failed to restore user: ' + error.message
  } finally {
    isLoading.value = false
  }
}

const blockUser = async (user) => {
  const action = user.isBlocked ? 'unblock' : 'block'
  if (!confirm(`Are you sure you want to ${action} ${user.fullName}?`)) {
    return
  }
  
  try {
    isLoading.value = true
    const userId = user.userId || user.id
    
    await userManagementApi.updateUserStatus(userId, !user.isBlocked)
    
    // Update the user in our local array
    const userIndex = users.value.findIndex(u => (u.userId || u.id) === userId)
    if (userIndex !== -1) {
      users.value[userIndex].isBlocked = !user.isBlocked
    }
  } catch (error) {
    errorMessage.value = `Failed to ${action} user: ${error.message}`
  } finally {
    isLoading.value = false
  }
}

// Role Request Methods
const loadRoleRequests = async () => {
  try {
    const [pending, all] = await Promise.all([
      roleApplicationApi.getPendingApplications(),
      roleApplicationApi.getAllApplications()
    ])
    
    pendingRequests.value = pending
    pendingApplicationsCount.value = pending.length
    
    // Get recent approved/rejected requests (last 10)
    recentRequests.value = all
      .filter(request => request.status !== 'Pending')
      .sort((a, b) => new Date(b.reviewedAt) - new Date(a.reviewedAt))
      .slice(0, 10)
  } catch (error) {
    console.error('Error loading role requests:', error)
    // Use fallback data if API fails
    pendingRequests.value = []
    recentRequests.value = []
    pendingApplicationsCount.value = 0
  }
}

const approveRequest = async (applicationId) => {
  try {
    await roleApplicationApi.reviewApplication(applicationId, 'Approved', 'Request approved by administrator')
    await loadRoleRequests()
    await loadUsers() // Refresh users to show updated roles
  } catch (error) {
    errorMessage.value = 'Failed to approve request: ' + error.message
  }
}

const rejectRequest = async (applicationId) => {
  const reason = prompt('Please provide a reason for rejection (optional):')
  try {
    await roleApplicationApi.reviewApplication(applicationId, 'Rejected', reason || 'Request rejected by administrator')
    await loadRoleRequests()
  } catch (error) {
    errorMessage.value = 'Failed to reject request: ' + error.message
  }
}

// Initialize component
onMounted(async () => {
  console.log('Admin User Management component mounted')
  await loadUsers()
  await loadSystemStats()
  await loadRoleRequests()
})

// Watch for search query changes with debouncing
let searchTimeout = null
watch(searchQuery, (newValue) => {
  clearTimeout(searchTimeout)
  searchTimeout = setTimeout(() => {
    if (newValue.trim()) {
      performSearch()
    } else {
      loadUsers()
    }
  }, 300)
})

// Watch for active tab changes to load role requests when needed
watch(activeTab, (newTab) => {
  if (newTab === 'requests') {
    loadRoleRequests()
  }
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