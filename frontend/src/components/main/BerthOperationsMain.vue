<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header Section -->
    <div class="bg-white shadow-sm border-b border-gray-200 sticky top-16 z-40">
      <div class="max-w-7xl mx-auto px-6 py-4">
        <div class="flex items-center justify-between">
          <div class="flex items-center space-x-4">
            <div class="p-2 bg-blue-600 rounded-lg">
              <Anchor :size="24" class="text-white" />
            </div>
            <div>
              <h1 class="text-2xl font-bold text-gray-900">Port Operations Management</h1>
              <p class="text-sm text-gray-600">Administrative Dashboard & Control Center</p>
            </div>
          </div>
          
          <!-- Real-time Status Indicator -->
          <div class="flex items-center space-x-4">
            <div class="flex items-center space-x-2">
              <div class="streaming-indicator"></div>
              <span class="text-sm font-medium text-gray-700">Live Data Stream</span>
            </div>
            <div class="flex items-center space-x-2 bg-green-50 px-3 py-1 rounded-full">
              <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
              <span class="text-sm font-medium text-green-700">System Online</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Tab Navigation -->
    <div class="bg-white border-b border-gray-200 sticky top-32 z-30">
      <div class="max-w-7xl mx-auto px-6">
        <nav class="flex space-x-0" role="tablist">
          <button
            v-for="tab in tabs"
            :key="tab.id"
            @click="activeTab = tab.id"
            :class="[
              'relative px-6 py-4 font-medium text-sm transition-all duration-200 border-b-2 focus:outline-none',
              activeTab === tab.id
                ? 'text-blue-600 border-blue-500 bg-blue-50'
                : 'text-gray-600 border-transparent hover:text-gray-900 hover:border-gray-300 hover:bg-gray-50'
            ]"
            :role="'tab'"
            :aria-selected="activeTab === tab.id"
          >
            <div class="flex items-center space-x-2">
              <component :is="tab.icon" :size="18" />
              <span>{{ tab.name }}</span>
              <span v-if="tab.badge" class="ml-2 bg-red-100 text-red-800 text-xs px-2 py-0.5 rounded-full">
                {{ tab.badge }}
              </span>
            </div>
            
            <!-- Active tab indicator -->
            <div 
              v-if="activeTab === tab.id" 
              class="absolute bottom-0 left-0 right-0 h-0.5 bg-blue-600 transform scale-x-100 transition-transform duration-200"
            ></div>
          </button>
        </nav>
      </div>
    </div>

    <!-- Tab Content -->
    <div class="max-w-7xl mx-auto px-6 py-6">
      <!-- Dashboard Tab -->
      <div v-if="activeTab === 'dashboard'" class="space-y-6" role="tabpanel">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <!-- Key Metrics Cards -->
          <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200 admin-card">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm font-medium text-gray-600">Active Berths</p>
                <p class="text-3xl font-bold text-gray-900">{{ stats.activeBerths }}</p>
              </div>
              <div class="p-3 bg-blue-100 rounded-lg">
                <Anchor class="text-blue-600" :size="24" />
              </div>
            </div>
            <div class="mt-2 flex items-center text-sm">
              <TrendingUp class="text-green-500 mr-1" :size="16" />
              <span class="text-green-600">+12%</span>
              <span class="text-gray-500 ml-1">from last month</span>
            </div>
          </div>

          <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200 admin-card">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm font-medium text-gray-600">Containers Processed</p>
                <p class="text-3xl font-bold text-gray-900">{{ stats.containersProcessed }}</p>
              </div>
              <div class="p-3 bg-green-100 rounded-lg">
                <Container class="text-green-600" :size="24" />
              </div>
            </div>
            <div class="mt-2 flex items-center text-sm">
              <TrendingUp class="text-green-500 mr-1" :size="16" />
              <span class="text-green-600">+8%</span>
              <span class="text-gray-500 ml-1">today</span>
            </div>
          </div>

          <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200 admin-card">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm font-medium text-gray-600">Ships in Port</p>
                <p class="text-3xl font-bold text-gray-900">{{ stats.shipsInPort }}</p>
              </div>
              <div class="p-3 bg-yellow-100 rounded-lg">
                <Ship class="text-yellow-600" :size="24" />
              </div>
            </div>
            <div class="mt-2 flex items-center text-sm">
              <TrendingDown class="text-red-500 mr-1" :size="16" />
              <span class="text-red-600">-3%</span>
              <span class="text-gray-500 ml-1">from yesterday</span>
            </div>
          </div>

          <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200 admin-card">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm font-medium text-gray-600">Efficiency Rate</p>
                <p class="text-3xl font-bold text-gray-900">{{ stats.efficiencyRate }}%</p>
              </div>
              <div class="p-3 bg-purple-100 rounded-lg">
                <Activity class="text-purple-600" :size="24" />
              </div>
            </div>
            <div class="mt-2 flex items-center text-sm">
              <TrendingUp class="text-green-500 mr-1" :size="16" />
              <span class="text-green-600">+5%</span>
              <span class="text-gray-500 ml-1">this week</span>
            </div>
          </div>
        </div>

        <!-- Real-time Operations Chart -->
        <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200 data-stream">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-gray-900">Real-time Operations</h3>
            <div class="flex items-center space-x-2">
              <div class="connection-indicator">
                <div class="connection-dot online"></div>
                <span class="text-sm text-gray-600">Live Updates</span>
              </div>
            </div>
          </div>
          <div class="h-64 bg-gray-50 rounded-lg flex items-center justify-center loading-shimmer">
            <div class="text-center">
              <BarChart3 class="mx-auto text-gray-400 mb-2" :size="48" />
              <p class="text-gray-500">Real-time charts will be integrated here</p>
              <p class="text-sm text-gray-400">WebSocket connection ready for streaming data</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Enhanced Berths Management Tab -->
      <div v-if="activeTab === 'berths'" class="space-y-6" role="tabpanel">
        <!-- Admin Control Header -->
        <div class="bg-gradient-to-r from-blue-50 to-indigo-50 rounded-xl p-6 border border-blue-200">
          <div class="flex flex-col lg:flex-row lg:items-center lg:justify-between space-y-4 lg:space-y-0">
            <div>
              <h2 class="text-2xl font-bold text-gray-900 mb-2">Berth Management Center</h2>
              <p class="text-gray-600">Real-time monitoring and administration of port berths</p>
              <div class="flex items-center space-x-4 mt-3">
                <div class="flex items-center space-x-2">
                  <div class="streaming-indicator"></div>
                  <span class="text-sm font-medium text-gray-700">Live Updates Active</span>
                </div>
                <div class="text-sm text-gray-600">
                  <span class="font-semibold text-blue-600">{{ berths.length }}</span> total berths
                </div>
                <div class="text-sm text-gray-600">
                  <span class="font-semibold text-green-600">{{ berths.filter(b => b.status === 'Available').length }}</span> available
                </div>
              </div>
            </div>
            
            <!-- Quick Actions -->
            <div class="flex flex-wrap gap-3">
              <button 
                @click="showBerthModal = true"
                class="bg-blue-600 text-white px-4 py-2.5 rounded-lg hover:bg-blue-700 transition-all duration-200 flex items-center space-x-2 shadow-sm hover:shadow-md"
              >
                <Plus :size="16" />
                <span>Create Berth</span>
              </button>
              <button class="bg-green-600 text-white px-4 py-2.5 rounded-lg hover:bg-green-700 transition-all duration-200 flex items-center space-x-2 shadow-sm hover:shadow-md">
                <RefreshCw :size="16" />
                <span>Sync Data</span>
              </button>
              <button class="bg-gray-100 text-gray-700 px-4 py-2.5 rounded-lg hover:bg-gray-200 transition-all duration-200 flex items-center space-x-2 border border-gray-300">
                <Download :size="16" />
                <span>Export</span>
              </button>
            </div>
          </div>
        </div>

        <!-- Advanced Filters & Search -->
        <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
          <div class="flex flex-col lg:flex-row lg:items-center lg:justify-between space-y-4 lg:space-y-0">
            <!-- Search and Filters -->
            <div class="flex flex-col sm:flex-row space-y-3 sm:space-y-0 sm:space-x-4 flex-1">
              <!-- Search Input -->
              <div class="relative flex-1 max-w-md">
                <Search class="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" :size="20" />
                <input
                  v-model="searchQuery"
                  type="text"
                  placeholder="Search berths by name, ID, or status..."
                  class="w-full pl-10 pr-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all duration-200"
                />
              </div>
              
              <!-- Status Filter -->
              <select 
                v-model="statusFilter" 
                class="px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-white min-w-[140px]"
              >
                <option value="">All Statuses</option>
                <option value="Available">Available</option>
                <option value="Occupied">Occupied</option>
                <option value="Under Maintenance">Maintenance</option>
                <option value="Reserved">Reserved</option>
              </select>
              
              <!-- Type Filter -->
              <select 
                v-model="typeFilter" 
                class="px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-white min-w-[120px]"
              >
                <option value="">All Types</option>
                <option value="Container">Container</option>
                <option value="Bulk">Bulk</option>
                <option value="Liquid">Liquid</option>
                <option value="General">General</option>
              </select>
            </div>
            
            <!-- View Controls -->
            <div class="flex items-center space-x-3">
              <!-- Sort Options -->
              <select 
                v-model="sortBy" 
                class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-white text-sm"
              >
                <option value="name">Sort by Name</option>
                <option value="status">Sort by Status</option>
                <option value="capacity">Sort by Capacity</option>
                <option value="priority">Sort by Priority</option>
                <option value="updated">Last Updated</option>
              </select>
              
              <!-- View Toggle -->
              <div class="flex items-center bg-gray-100 rounded-lg p-1">
                <button
                  @click="viewMode = 'grid'"
                  :class="[
                    'px-3 py-1.5 rounded-md text-sm font-medium transition-all duration-200',
                    viewMode === 'grid' ? 'bg-white shadow-sm text-gray-900' : 'text-gray-600 hover:text-gray-900'
                  ]"
                >
                  <Grid3X3 :size="16" class="inline" />
                  Grid
                </button>
                <button
                  @click="viewMode = 'list'"
                  :class="[
                    'px-3 py-1.5 rounded-md text-sm font-medium transition-all duration-200',
                    viewMode === 'list' ? 'bg-white shadow-sm text-gray-900' : 'text-gray-600 hover:text-gray-900'
                  ]"
                >
                  <List :size="16" class="inline" />
                  List
                </button>
              </div>
            </div>
          </div>
          
          <!-- Quick Filters -->
          <div class="mt-4 flex flex-wrap gap-2">
            <button
              @click="quickFilter = 'available'"
              :class="[
                'px-3 py-1.5 text-sm rounded-full transition-all duration-200',
                quickFilter === 'available' ? 'bg-green-100 text-green-800 border border-green-300' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              ]"
            >
              Available Only ({{ berths.filter(b => b.status === 'Available').length }})
            </button>
            <button
              @click="quickFilter = 'occupied'"
              :class="[
                'px-3 py-1.5 text-sm rounded-full transition-all duration-200',
                quickFilter === 'occupied' ? 'bg-blue-100 text-blue-800 border border-blue-300' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              ]"
            >
              Occupied ({{ berths.filter(b => b.status === 'Occupied').length }})
            </button>
            <button
              @click="quickFilter = 'high-priority'"
              :class="[
                'px-3 py-1.5 text-sm rounded-full transition-all duration-200',
                quickFilter === 'high-priority' ? 'bg-purple-100 text-purple-800 border border-purple-300' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              ]"
            >
              High Priority ({{ berths.filter(b => b.priority === 1).length }})
            </button>
            <button
              @click="quickFilter = 'maintenance'"
              :class="[
                'px-3 py-1.5 text-sm rounded-full transition-all duration-200',
                quickFilter === 'maintenance' ? 'bg-orange-100 text-orange-800 border border-orange-300' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              ]"
            >
              Maintenance ({{ berths.filter(b => b.status.includes('Maintenance')).length }})
            </button>
            <button
              v-if="quickFilter"
              @click="clearFilters"
              class="px-3 py-1.5 text-sm rounded-full bg-red-100 text-red-800 hover:bg-red-200 transition-all duration-200"
            >
              Clear Filters
            </button>
          </div>
        </div>

        <!-- Live Statistics Dashboard -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div class="bg-white rounded-lg p-4 border border-gray-200 shadow-sm">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-gray-600">Utilization Rate</p>
                <p class="text-2xl font-bold text-blue-600">{{ Math.round((berths.filter(b => b.status === 'Occupied').length / berths.length) * 100) }}%</p>
              </div>
              <div class="p-2 bg-blue-100 rounded-lg">
                <TrendingUp class="text-blue-600" :size="20" />
              </div>
            </div>
            <div class="mt-2">
              <div class="w-full bg-gray-200 rounded-full h-2">
                <div 
                  class="bg-blue-600 h-2 rounded-full transition-all duration-500" 
                  :style="{ width: Math.round((berths.filter(b => b.status === 'Occupied').length / berths.length) * 100) + '%' }"
                ></div>
              </div>
            </div>
          </div>
          
          <div class="bg-white rounded-lg p-4 border border-gray-200 shadow-sm">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-gray-600">Average Capacity</p>
                <p class="text-2xl font-bold text-green-600">{{ Math.round(berths.reduce((sum, b) => sum + b.capacity, 0) / berths.length) }}</p>
              </div>
              <div class="p-2 bg-green-100 rounded-lg">
                <Container class="text-green-600" :size="20" />
              </div>
            </div>
            <p class="text-xs text-gray-500 mt-1">TEU per berth</p>
          </div>
          
          <div class="bg-white rounded-lg p-4 border border-gray-200 shadow-sm">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-gray-600">Revenue/Hour</p>
                <p class="text-2xl font-bold text-purple-600">${{ berths.reduce((sum, b) => sum + (b.hourlyRate || 0), 0).toLocaleString() }}</p>
              </div>
              <div class="p-2 bg-purple-100 rounded-lg">
                <DollarSign class="text-purple-600" :size="20" />
              </div>
            </div>
            <p class="text-xs text-gray-500 mt-1">Total potential</p>
          </div>
          
          <div class="bg-white rounded-lg p-4 border border-gray-200 shadow-sm">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-gray-600">Live Updates</p>
                <p class="text-2xl font-bold text-orange-600">{{ liveUpdateCount }}</p>
              </div>
              <div class="p-2 bg-orange-100 rounded-lg">
                <Activity class="text-orange-600" :size="20" />
              </div>
            </div>
            <div class="flex items-center mt-1">
              <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse mr-2"></div>
              <p class="text-xs text-gray-500">Real-time sync</p>
            </div>
          </div>
        </div>

        <!-- Berths Display -->
        <div v-if="viewMode === 'grid'" class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-8">
          <BerthCard 
            v-for="berth in filteredBerths" 
            :key="berth.berthId" 
            :berth="berth"
            :class="['berth-card-animation', { 'new-update': isRecentlyUpdated(berth) }]"
            @edit="editBerth"
            @view="viewBerth"
            @assign="assignBerth"
          />
        </div>

        <!-- List View -->
        <div v-else class="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Berth</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Capacity</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Utilization</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Revenue</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Last Updated</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                <tr 
                  v-for="berth in filteredBerths" 
                  :key="berth.berthId"
                  :class="['streaming-table-row', { 'new-data': isRecentlyUpdated(berth) }]"
                >
                  <td class="px-6 py-4 whitespace-nowrap">
                    <div class="flex items-center">
                      <div class="flex-shrink-0 h-10 w-10">
                        <div class="h-10 w-10 bg-blue-100 rounded-lg flex items-center justify-center">
                          <span class="text-blue-600 font-bold text-sm">{{ berth.identifier }}</span>
                        </div>
                      </div>
                      <div class="ml-4">
                        <div class="text-sm font-medium text-gray-900">{{ berth.name }}</div>
                        <div class="text-sm text-gray-500">{{ berth.type }} • {{ berth.portName }}</div>
                      </div>
                    </div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    <span :class="getStatusBadgeClass(berth.status)" class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                      {{ berth.status }}
                    </span>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {{ berth.capacity }} TEU
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    <div class="flex items-center">
                      <div class="flex-1">
                        <div class="flex justify-between text-sm">
                          <span class="text-gray-600">{{ berth.currentLoad || 0 }}/{{ berth.capacity }}</span>
                          <span class="text-gray-500">{{ Math.round(((berth.currentLoad || 0) / berth.capacity) * 100) }}%</span>
                        </div>
                        <div class="w-full bg-gray-200 rounded-full h-2 mt-1">
                          <div 
                            class="bg-blue-600 h-2 rounded-full transition-all duration-500" 
                            :style="{ width: Math.round(((berth.currentLoad || 0) / berth.capacity) * 100) + '%' }"
                          ></div>
                        </div>
                      </div>
                    </div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    ${{ berth.hourlyRate || 0 }}/hr
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {{ formatLastUpdated(berth.updatedAt) }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <div class="flex space-x-2">
                      <button @click="viewBerth(berth)" class="text-blue-600 hover:text-blue-900">
                        <Eye :size="16" />
                      </button>
                      <button @click="editBerth(berth)" class="text-gray-600 hover:text-gray-900">
                        <Edit :size="16" />
                      </button>
                      <button @click="assignBerth(berth)" class="text-green-600 hover:text-green-900">
                        <Users :size="16" />
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Empty State -->
        <div v-if="filteredBerths.length === 0" class="text-center py-12">
          <Anchor class="mx-auto h-12 w-12 text-gray-400" />
          <h3 class="mt-2 text-sm font-medium text-gray-900">No berths found</h3>
          <p class="mt-1 text-sm text-gray-500">Try adjusting your filters or search criteria.</p>
          <div class="mt-6">
            <button
              @click="clearFilters"
              class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
            >
              Clear all filters
            </button>
          </div>
        </div>
      </div>

      <!-- Operations Tab -->
      <div v-if="activeTab === 'operations'" class="space-y-6" role="tabpanel">
        <div class="flex justify-between items-center">
          <h2 class="text-xl font-semibold text-gray-900">Live Operations Control Center</h2>
          <div class="flex items-center space-x-3">
            <div class="streaming-status">
              <div class="connection-dot online"></div>
              <span class="text-sm text-gray-600">Live Stream Active</span>
            </div>
            <button class="bg-orange-600 text-white px-4 py-2 rounded-lg hover:bg-orange-700 transition-colors flex items-center space-x-2">
              <AlertTriangle :size="16" />
              <span>Emergency Stop</span>
            </button>
            <button class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center space-x-2">
              <Play :size="16" />
              <span>Start Operation</span>
            </button>
          </div>
        </div>

        <!-- Real-time Operations Dashboard -->
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <!-- Active Operations Panel -->
          <div class="lg:col-span-2 bg-white rounded-lg shadow-sm p-6 border border-gray-200">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-semibold text-gray-900">Active Operations</h3>
              <div class="flex items-center space-x-2">
                <div class="streaming-indicator"></div>
                <span class="text-sm text-gray-600">{{ activeOperations.length }} Active</span>
              </div>
            </div>
            <div class="space-y-3 max-h-96 overflow-y-auto">
              <div v-for="operation in activeOperations" :key="operation.id" 
                   class="flex items-center space-x-4 p-4 bg-gray-50 rounded-lg operation-item hover:bg-gray-100 transition-colors">
                <div class="flex-shrink-0">
                  <div :class="getOperationIconClass(operation.type)" class="w-10 h-10 rounded-full flex items-center justify-center">
                    <Ship v-if="operation.type === 'docking'" class="text-blue-600" :size="20" />
                    <Container v-else-if="operation.type === 'loading'" class="text-green-600" :size="20" />
                    <Truck v-else-if="operation.type === 'transport'" class="text-yellow-600" :size="20" />
                    <AlertCircle v-else class="text-red-600" :size="20" />
                  </div>
                </div>
                <div class="flex-1 min-w-0">
                  <div class="flex items-center justify-between">
                    <p class="text-sm font-medium text-gray-900">{{ operation.description }}</p>
                    <span class="text-xs text-gray-500">{{ operation.berthId }}</span>
                  </div>
                  <div class="flex items-center space-x-2 mt-1">
                    <p class="text-sm text-gray-500">{{ operation.timestamp }}</p>
                    <div class="w-2 h-2 bg-green-400 rounded-full animate-pulse"></div>
                  </div>
                  <!-- Progress Bar -->
                  <div class="mt-2 w-full bg-gray-200 rounded-full h-2">
                    <div :class="getProgressBarClass(operation.status)" 
                         class="h-2 rounded-full transition-all duration-300"
                         :style="{ width: operation.progress + '%' }"></div>
                  </div>
                </div>
                <div class="flex-shrink-0 flex flex-col space-y-1">
                  <span :class="getStatusClass(operation.status)" class="px-2 py-1 text-xs font-medium rounded-full">
                    {{ operation.status }}
                  </span>
                  <button class="text-blue-600 hover:text-blue-800 text-xs">Monitor</button>
                </div>
              </div>
            </div>
          </div>

          <!-- Live Controls Panel -->
          <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
            <h3 class="text-lg font-semibold text-gray-900 mb-4">Operations Control</h3>
            <div class="space-y-4">
              <!-- Quick Actions -->
              <div class="grid grid-cols-2 gap-2">
                <button class="bg-blue-100 text-blue-700 p-3 rounded-lg hover:bg-blue-200 transition-colors text-sm">
                  <Ship :size="16" class="mx-auto mb-1" />
                  <div>Dock Ship</div>
                </button>
                <button class="bg-green-100 text-green-700 p-3 rounded-lg hover:bg-green-200 transition-colors text-sm">
                  <Container :size="16" class="mx-auto mb-1" />
                  <div>Load Container</div>
                </button>
                <button class="bg-yellow-100 text-yellow-700 p-3 rounded-lg hover:bg-yellow-200 transition-colors text-sm">
                  <Truck :size="16" class="mx-auto mb-1" />
                  <div>Transport</div>
                </button>
                <button class="bg-red-100 text-red-700 p-3 rounded-lg hover:bg-red-200 transition-colors text-sm">
                  <AlertTriangle :size="16" class="mx-auto mb-1" />
                  <div>Alert</div>
                </button>
              </div>

              <!-- System Status -->
              <div class="border-t pt-4">
                <h4 class="font-medium text-gray-900 mb-3">System Status</h4>
                <div class="space-y-2">
                  <div class="flex justify-between items-center">
                    <span class="text-sm text-gray-600">Data Stream</span>
                    <div class="flex items-center space-x-1">
                      <div class="w-2 h-2 bg-green-400 rounded-full"></div>
                      <span class="text-xs text-green-600">Online</span>
                    </div>
                  </div>
                  <div class="flex justify-between items-center">
                    <span class="text-sm text-gray-600">Operations</span>
                    <div class="flex items-center space-x-1">
                      <div class="w-2 h-2 bg-blue-400 rounded-full animate-pulse"></div>
                      <span class="text-xs text-blue-600">{{ activeOperations.length }} Active</span>
                    </div>
                  </div>
                  <div class="flex justify-between items-center">
                    <span class="text-sm text-gray-600">Alerts</span>
                    <div class="flex items-center space-x-1">
                      <div class="w-2 h-2 bg-yellow-400 rounded-full"></div>
                      <span class="text-xs text-yellow-600">{{ alerts.length }} Pending</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Performance Metrics -->
              <div class="border-t pt-4">
                <h4 class="font-medium text-gray-900 mb-3">Live Metrics</h4>
                <div class="space-y-3">
                  <div>
                    <div class="flex justify-between text-sm mb-1">
                      <span>Efficiency</span>
                      <span>{{ liveMetrics.efficiency }}%</span>
                    </div>
                    <div class="w-full bg-gray-200 rounded-full h-2">
                      <div class="bg-green-400 h-2 rounded-full transition-all duration-300" 
                           :style="{ width: liveMetrics.efficiency + '%' }"></div>
                    </div>
                  </div>
                  <div>
                    <div class="flex justify-between text-sm mb-1">
                      <span>Throughput</span>
                      <span>{{ liveMetrics.throughput }}%</span>
                    </div>
                    <div class="w-full bg-gray-200 rounded-full h-2">
                      <div class="bg-blue-400 h-2 rounded-full transition-all duration-300" 
                           :style="{ width: liveMetrics.throughput + '%' }"></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Real-time Activity Feed -->
        <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-gray-900">Live Activity Feed</h3>
            <div class="flex items-center space-x-3">
              <div class="streaming-indicator"></div>
              <span class="text-sm text-gray-600">Real-time Updates</span>
              <button class="text-blue-600 hover:text-blue-800 text-sm">Filter</button>
            </div>
          </div>
          <div class="h-64 overflow-y-auto">
            <div class="space-y-2">
              <div v-for="activity in liveActivityFeed" :key="activity.id" 
                   class="flex items-center space-x-3 p-3 rounded-lg transition-all duration-300"
                   :class="{ 'bg-blue-50': activity.isNew, 'bg-gray-50': !activity.isNew }">
                <div class="flex-shrink-0">
                  <div :class="getActivityIconClass(activity.type)" class="w-8 h-8 rounded-full flex items-center justify-center">
                    <Activity :size="14" />
                  </div>
                </div>
                <div class="flex-1 min-w-0">
                  <p class="text-sm text-gray-900">{{ activity.message }}</p>
                  <p class="text-xs text-gray-500">{{ activity.timestamp }}</p>
                </div>
                <div v-if="activity.isNew" class="flex-shrink-0">
                  <div class="w-2 h-2 bg-blue-400 rounded-full animate-pulse"></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Analytics Tab -->
      <div v-if="activeTab === 'analytics'" class="space-y-6" role="tabpanel">
        <div class="flex justify-between items-center">
          <h2 class="text-xl font-semibold text-gray-900">Analytics & Reports</h2>
          <div class="flex space-x-3">
            <select class="border border-gray-300 rounded-lg px-3 py-2 text-sm">
              <option>Last 7 days</option>
              <option>Last 30 days</option>
              <option>Last 90 days</option>
            </select>
            <button class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2">
              <BarChart3 :size="16" />
              <span>Generate Report</span>
            </button>
          </div>
        </div>

        <!-- Analytics Cards -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200 admin-card">
            <h3 class="text-lg font-semibold text-gray-900 mb-4">Performance Metrics</h3>
            <div class="h-48 bg-gray-50 rounded-lg flex items-center justify-center">
              <div class="text-center">
                <TrendingUp class="mx-auto text-gray-400 mb-2" :size="32" />
                <p class="text-gray-500">Performance charts integration point</p>
                <p class="text-sm text-gray-400">Real-time analytics ready</p>
              </div>
            </div>
          </div>

          <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200 admin-card">
            <h3 class="text-lg font-semibold text-gray-900 mb-4">Resource Utilization</h3>
            <div class="h-48 bg-gray-50 rounded-lg flex items-center justify-center">
              <div class="text-center">
                <PieChart class="mx-auto text-gray-400 mb-2" :size="32" />
                <p class="text-gray-500">Utilization charts integration point</p>
                <p class="text-sm text-gray-400">Live streaming data visualization</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Real-time Analytics Dashboard -->
        <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-gray-900">Real-time Analytics</h3>
            <div class="flex items-center space-x-2">
              <div class="streaming-indicator"></div>
              <span class="text-sm text-gray-600">Live Data</span>
            </div>
          </div>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
            <div class="bg-blue-50 p-4 rounded-lg">
              <div class="text-2xl font-bold text-blue-600">{{ stats.efficiencyRate }}%</div>
              <div class="text-sm text-blue-600">Current Efficiency</div>
            </div>
            <div class="bg-green-50 p-4 rounded-lg">
              <div class="text-2xl font-bold text-green-600">{{ stats.containersProcessed }}</div>
              <div class="text-sm text-green-600">Containers Today</div>
            </div>
            <div class="bg-purple-50 p-4 rounded-lg">
              <div class="text-2xl font-bold text-purple-600">{{ stats.shipsInPort }}</div>
              <div class="text-sm text-purple-600">Ships Docked</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Settings Tab -->
      <div v-if="activeTab === 'settings'" class="space-y-6" role="tabpanel">
        <div class="flex justify-between items-center">
          <h2 class="text-xl font-semibold text-gray-900">System Settings</h2>
          <button class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center space-x-2">
            <Save :size="16" />
            <span>Save Changes</span>
          </button>
        </div>

        <!-- Settings Grid -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
            <h3 class="text-lg font-semibold text-gray-900 mb-4">Port Configuration</h3>
            <div class="space-y-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Port Name</label>
                <input type="text" class="w-full border border-gray-300 rounded-lg px-3 py-2" value="Maersk Container Terminal">
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Operating Hours</label>
                <input type="text" class="w-full border border-gray-300 rounded-lg px-3 py-2" value="24/7">
              </div>
            </div>
          </div>

          <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
            <h3 class="text-lg font-semibold text-gray-900 mb-4">Real-time Settings</h3>
            <div class="space-y-4">
              <div class="flex items-center justify-between">
                <span class="text-sm font-medium text-gray-700">Live Data Streaming</span>
                <div class="streaming-toggle">
                  <input type="checkbox" checked class="toggle-input">
                  <div class="toggle-slider"></div>
                </div>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm font-medium text-gray-700">Auto-refresh Dashboard</span>
                <div class="streaming-toggle">
                  <input type="checkbox" checked class="toggle-input">
                  <div class="toggle-slider"></div>
                </div>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm font-medium text-gray-700">Real-time Notifications</span>
                <div class="streaming-toggle">
                  <input type="checkbox" checked class="toggle-input">
                  <div class="toggle-slider"></div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Streaming Configuration -->
        <div class="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
          <h3 class="text-lg font-semibold text-gray-900 mb-4">Streaming Configuration</h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Update Frequency (seconds)</label>
              <select class="w-full border border-gray-300 rounded-lg px-3 py-2">
                <option value="1">1 second</option>
                <option value="5" selected>5 seconds</option>
                <option value="10">10 seconds</option>
                <option value="30">30 seconds</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Data Buffer Size</label>
              <select class="w-full border border-gray-300 rounded-lg px-3 py-2">
                <option value="100">100 records</option>
                <option value="500" selected>500 records</option>
                <option value="1000">1000 records</option>
              </select>
            </div>
          </div>
        </div>
      </div>

      <!-- Users Tab -->
      <div v-if="activeTab === 'users'" class="space-y-6" role="tabpanel">
        <div class="flex justify-between items-center">
          <h2 class="text-xl font-semibold text-gray-900">User Management</h2>
          <div class="flex space-x-3">
            <button class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2">
              <UserPlus :size="16" />
              <span>Add User</span>
            </button>
            <button class="bg-gray-100 text-gray-700 px-4 py-2 rounded-lg hover:bg-gray-200 transition-colors flex items-center space-x-2">
              <Shield :size="16" />
              <span>Manage Roles</span>
            </button>
          </div>
        </div>

        <!-- Users Table -->
        <div class="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
          <div class="px-6 py-4 border-b border-gray-200">
            <h3 class="text-lg font-semibold text-gray-900">Active Users</h3>
          </div>
          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">User</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Role</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Last Active</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                <tr v-for="user in users" :key="user.id">
                  <td class="px-6 py-4 whitespace-nowrap">
                    <div class="flex items-center">
                      <div class="flex-shrink-0 h-10 w-10">
                        <div class="h-10 w-10 rounded-full bg-blue-100 flex items-center justify-center">
                          <User class="text-blue-600" :size="20" />
                        </div>
                      </div>
                      <div class="ml-4">
                        <div class="text-sm font-medium text-gray-900">{{ user.name }}</div>
                        <div class="text-sm text-gray-500">{{ user.email }}</div>
                      </div>
                    </div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-blue-100 text-blue-800">
                      {{ user.role }}
                    </span>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    <span :class="user.status === 'Online' ? 'text-green-800 bg-green-100' : 'text-gray-800 bg-gray-100'" 
                          class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                      {{ user.status }}
                    </span>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {{ user.lastActive }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    <button class="text-blue-600 hover:text-blue-900 mr-3">Edit</button>
                    <button class="text-red-600 hover:text-red-900">Remove</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { 
  Anchor, 
  BarChart3, 
  Container, 
  Ship, 
  Activity, 
  Users, 
  Settings, 
  TrendingUp, 
  TrendingDown,
  Plus,
  Download,
  Play,
  Save,
  UserPlus,
  Shield,
  User,
  PieChart,
  RefreshCw,
  Search,
  Grid3X3,
  List,
  Eye,
  Edit,
  DollarSign,
  AlertTriangle,
  Truck,
  AlertCircle,
  CheckCircle,
  Clock
} from 'lucide-vue-next'
import BerthCard from '../berths/BerthCard.vue'

export default {
  name: 'BerthOperationsMain',
  components: {
    Anchor,
    BarChart3,
    Container,
    Ship,
    Activity,
    Users,
    Settings,
    TrendingUp,
    TrendingDown,
    Plus,
    Download,
    Play,
    Save,
    UserPlus,
    Shield,
    User,
    PieChart,
    BerthCard,
    RefreshCw,
    Search,
    Grid3X3,
    List,
    Eye,
    Edit,
    DollarSign,
    AlertTriangle,
    Truck,
    AlertCircle,
    CheckCircle,
    Clock
  },
  data() {
    return {
      activeTab: 'dashboard',
      viewMode: 'grid',
      
      // Live streaming and admin controls
      showBerthModal: false,
      liveUpdateCount: 42,
      
      // Search and filtering
      searchQuery: '',
      statusFilter: '',
      typeFilter: '',
      sortBy: 'name',
      quickFilter: '',
      
      tabs: [
        { id: 'dashboard', name: 'Dashboard', icon: BarChart3, badge: null },
        { id: 'berths', name: 'Berths', icon: Anchor, badge: '12' },
        { id: 'operations', name: 'Operations', icon: Activity, badge: '3' },
        { id: 'analytics', name: 'Analytics', icon: TrendingUp, badge: null },
        { id: 'settings', name: 'Settings', icon: Settings, badge: null },
        { id: 'users', name: 'Users', icon: Users, badge: null }
      ],
      stats: {
        activeBerths: 24,
        containersProcessed: 1847,
        shipsInPort: 12,
        efficiencyRate: 94
      },
      berths: [
        {
          berthId: 1,
          name: 'Berth A-1',
          identifier: 'A1',
          type: 'Container',
          status: 'Occupied',
          capacity: 850,
          currentLoad: 720,
          maxShipLength: 300,
          maxDraft: 14.5,
          availableServices: 'Crane, Fuel, Water',
          craneCount: 4,
          hourlyRate: 150,
          priority: 1,
          notes: 'High priority berth for large container ships',
          portId: 1,
          portName: 'Port of Hamburg',
          activeAssignmentCount: 1,
          createdAt: '2024-01-01T00:00:00Z',
          updatedAt: '2024-10-15T10:30:00Z'
        },
        {
          berthId: 2,
          name: 'Berth A-2',
          identifier: 'A2',
          type: 'Container',
          status: 'Available',
          capacity: 650,
          currentLoad: 0,
          maxShipLength: 250,
          maxDraft: 12.0,
          availableServices: 'Crane, Water',
          craneCount: 3,
          hourlyRate: 120,
          priority: 2,
          notes: 'Medium capacity berth suitable for mid-size vessels',
          portId: 1,
          portName: 'Port of Hamburg',
          activeAssignmentCount: 0,
          createdAt: '2024-01-01T00:00:00Z',
          updatedAt: '2024-10-15T08:15:00Z'
        },
        {
          berthId: 3,
          name: 'Berth B-1',
          identifier: 'B1',
          type: 'Bulk',
          status: 'Under Maintenance',
          capacity: 900,
          currentLoad: 0,
          maxShipLength: 320,
          maxDraft: 16.0,
          availableServices: 'Conveyor, Crane, Fuel',
          craneCount: 2,
          hourlyRate: 180,
          priority: 1,
          notes: 'Currently undergoing scheduled maintenance',
          portId: 1,
          portName: 'Port of Hamburg',
          activeAssignmentCount: 0,
          createdAt: '2024-01-01T00:00:00Z',
          updatedAt: '2024-10-15T06:00:00Z'
        }
      ],
      recentOperations: [
        {
          id: 1,
          description: 'Container loaded onto MSC Gülsün',
          timestamp: '2 minutes ago',
          status: 'Completed'
        },
        {
          id: 2,
          description: 'Ship Maersk Sealand departed from Berth C-2',
          timestamp: '15 minutes ago',
          status: 'Completed'
        },
        {
          id: 3,
          description: 'Cargo inspection in progress',
          timestamp: '23 minutes ago',
          status: 'In Progress'
        }
      ],
      users: [
        {
          id: 1,
          name: 'John Doe',
          email: 'john@maersk.com',
          role: 'Port Manager',
          status: 'Online',
          lastActive: '2 min ago'
        },
        {
          id: 2,
          name: 'Sarah Wilson',
          email: 'sarah@maersk.com',
          role: 'Operations Supervisor',
          status: 'Online',
          lastActive: '5 min ago'
        }
      ],
      
      // Live Operations Data
      activeOperations: [
        {
          id: 1,
          description: 'Container ship MSC Gülsün docking procedure',
          timestamp: '2 min ago',
          status: 'In Progress',
          type: 'docking',
          berthId: 'A-1',
          progress: 75
        },
        {
          id: 2,
          description: 'Loading 240 containers onto Maersk Madrid',
          timestamp: '8 min ago',
          status: 'In Progress',
          type: 'loading',
          berthId: 'B-2',
          progress: 45
        },
        {
          id: 3,
          description: 'Truck convoy departing with 50 containers',
          timestamp: '12 min ago',
          status: 'Active',
          type: 'transport',
          berthId: 'C-1',
          progress: 90
        },
        {
          id: 4,
          description: 'Emergency inspection requested for Berth A-3',
          timestamp: '15 min ago',
          status: 'Urgent',
          type: 'inspection',
          berthId: 'A-3',
          progress: 20
        }
      ],
      
      alerts: [
        { id: 1, message: 'Weather alert: Strong winds expected', type: 'warning' },
        { id: 2, message: 'Maintenance scheduled for Berth B-1', type: 'info' }
      ],
      
      liveMetrics: {
        efficiency: 94,
        throughput: 78
      },
      
      liveActivityFeed: [
        {
          id: 1,
          message: 'Ship Ever Given completed docking at Berth A-1',
          timestamp: '1 min ago',
          type: 'success',
          isNew: true
        },
        {
          id: 2,
          message: 'Container loading operation started at Berth B-2',
          timestamp: '3 min ago',
          type: 'info',
          isNew: true
        },
        {
          id: 3,
          message: 'Fuel supply connected to vessel at Berth C-1',
          timestamp: '5 min ago',
          type: 'info',
          isNew: false
        },
        {
          id: 4,
          message: 'Security checkpoint cleared for incoming vessel',
          timestamp: '7 min ago',
          type: 'success',
          isNew: false
        },
        {
          id: 5,
          message: 'Weather conditions updated: Wind speed 15 knots',
          timestamp: '10 min ago',
          type: 'warning',
          isNew: false
        }
      ]
    }
  },
  computed: {
    filteredBerths() {
      let filtered = [...this.berths];
      
      // Apply search query
      if (this.searchQuery) {
        const query = this.searchQuery.toLowerCase();
        filtered = filtered.filter(berth => 
          berth.name.toLowerCase().includes(query) ||
          berth.identifier.toLowerCase().includes(query) ||
          berth.status.toLowerCase().includes(query) ||
          berth.type.toLowerCase().includes(query) ||
          berth.portName.toLowerCase().includes(query)
        );
      }
      
      // Apply status filter
      if (this.statusFilter) {
        filtered = filtered.filter(berth => berth.status === this.statusFilter);
      }
      
      // Apply type filter
      if (this.typeFilter) {
        filtered = filtered.filter(berth => berth.type === this.typeFilter);
      }
      
      // Apply quick filters
      if (this.quickFilter) {
        switch (this.quickFilter) {
          case 'available':
            filtered = filtered.filter(berth => berth.status === 'Available');
            break;
          case 'occupied':
            filtered = filtered.filter(berth => berth.status === 'Occupied');
            break;
          case 'high-priority':
            filtered = filtered.filter(berth => berth.priority === 1);
            break;
          case 'maintenance':
            filtered = filtered.filter(berth => berth.status.includes('Maintenance'));
            break;
        }
      }
      
      // Apply sorting
      filtered.sort((a, b) => {
        switch (this.sortBy) {
          case 'name':
            return a.name.localeCompare(b.name);
          case 'status':
            return a.status.localeCompare(b.status);
          case 'capacity':
            return b.capacity - a.capacity;
          case 'priority':
            return a.priority - b.priority;
          case 'updated':
            return new Date(b.updatedAt) - new Date(a.updatedAt);
          default:
            return 0;
        }
      });
      
      return filtered;
    }
  },
  mounted() {
    // Simulate live updates
    this.startLiveUpdates();
  },
  methods: {
    // Status badge styling
    getStatusBadgeClass(status) {
      const classes = {
        'Available': 'bg-green-100 text-green-800 border-green-300',
        'Occupied': 'bg-blue-100 text-blue-800 border-blue-300',
        'Under Maintenance': 'bg-orange-100 text-orange-800 border-orange-300',
        'Reserved': 'bg-purple-100 text-purple-800 border-purple-300',
        'Maintenance': 'bg-orange-100 text-orange-800 border-orange-300'
      };
      return classes[status] || 'bg-gray-100 text-gray-800 border-gray-300';
    },
    
    // Filter controls
    clearFilters() {
      this.searchQuery = '';
      this.statusFilter = '';
      this.typeFilter = '';
      this.quickFilter = '';
      this.sortBy = 'name';
    },
    
    // Berth actions
    viewBerth(berth) {
      console.log('Viewing berth:', berth);
      // Implement berth view modal
    },
    
    editBerth(berth) {
      console.log('Editing berth:', berth);
      // Implement berth edit modal
    },
    
    assignBerth(berth) {
      console.log('Assigning berth:', berth);
      // Implement berth assignment
    },
    
    // Live streaming functionality
    startLiveUpdates() {
      // Simulate live updates every 5 seconds
      setInterval(() => {
        this.liveUpdateCount += Math.floor(Math.random() * 3) + 1;
        // Simulate random berth updates
        if (this.berths.length > 0) {
          const randomBerth = this.berths[Math.floor(Math.random() * this.berths.length)];
          randomBerth.updatedAt = new Date().toISOString();
        }
      }, 5000);
    },
    
    isRecentlyUpdated(berth) {
      const now = new Date();
      const updated = new Date(berth.updatedAt);
      return (now - updated) < 30000; // Updated within last 30 seconds
    },
    
    formatLastUpdated(dateString) {
      const date = new Date(dateString);
      const now = new Date();
      const diffMs = now - date;
      const diffMins = Math.floor(diffMs / 60000);
      
      if (diffMins < 1) return 'Just now';
      if (diffMins < 60) return `${diffMins}m ago`;
      if (diffMins < 1440) return `${Math.floor(diffMins / 60)}h ago`;
      return date.toLocaleDateString();
    },
    
    getStatusClass(status) {
      const classes = {
        'Completed': 'bg-green-100 text-green-800',
        'In Progress': 'bg-yellow-100 text-yellow-800',
        'Active': 'bg-blue-100 text-blue-800',
        'Urgent': 'bg-red-100 text-red-800',
        'Failed': 'bg-red-100 text-red-800'
      }
      return classes[status] || 'bg-gray-100 text-gray-800'
    },
    
    // Operations-specific methods
    getOperationIconClass(type) {
      const classes = {
        'docking': 'bg-blue-100',
        'loading': 'bg-green-100',
        'transport': 'bg-yellow-100',
        'inspection': 'bg-red-100'
      };
      return classes[type] || 'bg-gray-100';
    },
    
    getProgressBarClass(status) {
      const classes = {
        'In Progress': 'bg-blue-400',
        'Active': 'bg-green-400',
        'Urgent': 'bg-red-400',
        'Completed': 'bg-green-500'
      };
      return classes[status] || 'bg-gray-400';
    },
    
    getActivityIconClass(type) {
      const classes = {
        'success': 'bg-green-100',
        'info': 'bg-blue-100',
        'warning': 'bg-yellow-100',
        'error': 'bg-red-100'
      };
      return classes[type] || 'bg-gray-100';
    }
  }
}
</script>

<style scoped>
/* Real-time streaming animations */
.streaming-indicator {
  width: 12px;
  height: 12px;
  background: linear-gradient(45deg, #3b82f6, #06b6d4);
  border-radius: 50%;
  position: relative;
  animation: pulse-stream 2s infinite;
}

.streaming-indicator::before {
  content: '';
  position: absolute;
  top: -2px;
  left: -2px;
  right: -2px;
  bottom: -2px;
  border-radius: 50%;
  border: 2px solid #3b82f6;
  animation: ripple 2s infinite;
}

@keyframes pulse-stream {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

@keyframes ripple {
  0% {
    transform: scale(0.8);
    opacity: 1;
  }
  100% {
    transform: scale(1.4);
    opacity: 0;
  }
}

.streaming-status {
  display: flex;
  align-items: center;
  gap: 8px;
}

/* Enhanced berth card animations with live updates */
.berth-card-animation {
  transition: all 0.3s ease;
}

.berth-card-animation:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
}

.berth-card-animation.new-update {
  animation: newUpdatePulse 2s ease-out;
  border: 2px solid #3b82f6;
}

@keyframes newUpdatePulse {
  0% { 
    transform: scale(1); 
    box-shadow: 0 0 0 0 rgba(59, 130, 246, 0.4);
  }
  50% { 
    transform: scale(1.02); 
    box-shadow: 0 0 0 10px rgba(59, 130, 246, 0);
  }
  100% { 
    transform: scale(1); 
    box-shadow: 0 0 0 0 rgba(59, 130, 246, 0);
  }
}

/* Enhanced streaming table effects */
.streaming-table-row {
  transition: all 0.2s ease;
}

.streaming-table-row:hover {
  background: #f8fafc;
  transform: scale(1.002);
}

.streaming-table-row.new-data {
  animation: highlight-new 3s ease-out;
}

@keyframes highlight-new {
  0% { 
    background: #dbeafe;
    transform: scale(1.005);
  }
  100% { 
    background: transparent;
    transform: scale(1);
  }
}

/* Advanced search and filter styling */
.search-input-glow:focus {
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
  border-color: #3b82f6;
}

/* Live statistics animations */
.stat-card {
  transition: all 0.3s ease;
}

.stat-card:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

/* Progress bar animations */
.progress-bar {
  transition: width 0.8s ease-in-out;
}

.progress-bar-glow {
  background: linear-gradient(90deg, #3b82f6, #06b6d4);
  position: relative;
  overflow: hidden;
}

.progress-bar-glow::after {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.3), transparent);
  animation: progress-shine 2s infinite;
}

@keyframes progress-shine {
  0% { left: -100%; }
  100% { left: 100%; }
}

/* Filter pills animation */
.filter-pill {
  transition: all 0.2s ease;
}

.filter-pill:hover {
  transform: scale(1.05);
}

.filter-pill.active {
  animation: filterSelect 0.3s ease-out;
}

@keyframes filterSelect {
  0% { transform: scale(1); }
  50% { transform: scale(1.1); }
  100% { transform: scale(1); }
}

/* Enhanced admin gradient backgrounds */
.admin-header-gradient {
  background: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 50%, #f0f9ff 100%);
  position: relative;
}

.admin-header-gradient::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(59, 130, 246, 0.1), transparent);
  animation: admin-sweep 8s infinite;
}

@keyframes admin-sweep {
  0% { left: -100%; }
  100% { left: 100%; }
}

/* Enhanced view toggle */
.view-toggle {
  background: #f1f5f9;
  border-radius: 8px;
  padding: 4px;
}

.view-toggle button {
  transition: all 0.2s ease;
}

.view-toggle button.active {
  background: white;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  transform: scale(1.05);
}

/* Live update indicators */
.live-indicator {
  position: relative;
}

.live-indicator::after {
  content: '';
  position: absolute;
  top: -2px;
  right: -2px;
  width: 8px;
  height: 8px;
  background: #10b981;
  border-radius: 50%;
  animation: live-pulse 2s infinite;
}

@keyframes live-pulse {
  0%, 100% { 
    opacity: 1; 
    transform: scale(1);
  }
  50% { 
    opacity: 0.5; 
    transform: scale(1.2);
  }
}

/* Enhanced table styling */
.admin-table {
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.admin-table th {
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.admin-table tbody tr:hover {
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
}

/* Action buttons styling */
.action-button {
  transition: all 0.2s ease;
  padding: 8px;
  border-radius: 6px;
}

.action-button:hover {
  background: rgba(59, 130, 246, 0.1);
  transform: scale(1.1);
}

/* Empty state styling */
.empty-state {
  padding: 48px 24px;
  text-align: center;
}

.empty-state-icon {
  margin: 0 auto 16px;
  width: 48px;
  height: 48px;
  color: #9ca3af;
}

/* Responsive enhancements */
@media (max-width: 768px) {
  .berth-card-animation.new-update {
    animation: newUpdatePulseMobile 1.5s ease-out;
  }
  
  .admin-header-gradient {
    padding: 16px;
  }
  
  .streaming-table-row {
    font-size: 14px;
  }
  
  .filter-pill {
    font-size: 12px;
    padding: 6px 12px;
  }
}

@keyframes newUpdatePulseMobile {
  0% { 
    transform: scale(1); 
    box-shadow: 0 0 0 0 rgba(59, 130, 246, 0.4);
  }
  50% { 
    transform: scale(1.01); 
    box-shadow: 0 0 0 5px rgba(59, 130, 246, 0);
  }
  100% { 
    transform: scale(1); 
    box-shadow: 0 0 0 0 rgba(59, 130, 246, 0);
  }
}

/* Advanced loading states for admin interface */
.admin-loading {
  background: linear-gradient(90deg, #f1f5f9 25%, #e2e8f0 50%, #f1f5f9 75%);
  background-size: 200% 100%;
  animation: admin-shimmer 1.8s infinite;
}

@keyframes admin-shimmer {
  0% { background-position: -200% 0; }
  100% { background-position: 200% 0; }
}

/* Enhanced scrollbar for better admin experience */
.admin-scroll::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

.admin-scroll::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 4px;
}

.admin-scroll::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #cbd5e1, #94a3b8);
  border-radius: 4px;
}

.admin-scroll::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(135deg, #94a3b8, #64748b);
}

/* Status badge enhancements */
.status-badge {
  border: 1px solid;
  font-weight: 600;
  position: relative;
  overflow: hidden;
}

.status-badge::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  animation: badge-shine 3s infinite;
}

@keyframes badge-shine {
  0% { left: -100%; }
  100% { left: 100%; }
}

/* Operation item animations */
.operation-item {
  transition: all 0.2s ease;
  border-left: 4px solid transparent;
}

.operation-item:hover {
  border-left-color: #3b82f6;
  background-color: #f8fafc;
}

/* Custom toggle switch for streaming settings */
.streaming-toggle {
  position: relative;
  display: inline-block;
  width: 48px;
  height: 24px;
}

.toggle-input {
  opacity: 0;
  width: 0;
  height: 0;
}

.toggle-slider {
  position: absolute;
  cursor: pointer;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: #cbd5e1;
  transition: 0.3s;
  border-radius: 24px;
}

.toggle-slider:before {
  position: absolute;
  content: "";
  height: 18px;
  width: 18px;
  left: 3px;
  top: 3px;
  background-color: white;
  transition: 0.3s;
  border-radius: 50%;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.toggle-input:checked + .toggle-slider {
  background: linear-gradient(45deg, #10b981, #059669);
}

.toggle-input:checked + .toggle-slider:before {
  transform: translateX(24px);
}

/* Live data connection indicators */
@keyframes data-flow {
  0% { opacity: 0.3; }
  50% { opacity: 1; }
  100% { opacity: 0.3; }
}

.live-data-indicator {
  animation: data-flow 1.5s infinite;
}

/* Tab transition effects */
div[role="tabpanel"] {
  animation: fadeInUp 0.3s ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Enhanced gradient backgrounds for admin interface */
.admin-gradient {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.status-gradient {
  background: linear-gradient(45deg, #12d8fa 0%, #1fa2ff 100%);
}

/* Real-time data stream effect */
.data-stream {
  position: relative;
  overflow: hidden;
}

.data-stream::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(59, 130, 246, 0.2), transparent);
  animation: data-stream-flow 3s infinite;
}

@keyframes data-stream-flow {
  0% { left: -100%; }
  100% { left: 100%; }
}

/* Enhanced hover effects for admin cards */
.admin-card {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.admin-card:hover {
  transform: translateY(-4px) scale(1.02);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
}

/* Professional loading states */
.loading-shimmer {
  background: linear-gradient(90deg, #f1f5f9 25%, #e2e8f0 50%, #f1f5f9 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
}

@keyframes shimmer {
  0% { background-position: -200% 0; }
  100% { background-position: 200% 0; }
}

/* Connection status indicators */
.connection-indicator {
  display: flex;
  align-items: center;
  gap: 8px;
}

.connection-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  animation: pulse 2s infinite;
}

.connection-dot.online {
  background-color: #10b981;
}

.connection-dot.offline {
  background-color: #ef4444;
}

.connection-dot.connecting {
  background-color: #f59e0b;
}

/* WebSocket streaming visualization */
.websocket-stream {
  position: relative;
  overflow: hidden;
}

.websocket-stream::after {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: repeating-linear-gradient(
    90deg,
    transparent,
    transparent 10px,
    rgba(59, 130, 246, 0.1) 10px,
    rgba(59, 130, 246, 0.1) 20px
  );
  animation: stream-flow 2s linear infinite;
}

@keyframes stream-flow {
  0% { transform: translateX(-20px); }
  100% { transform: translateX(20px); }
}

/* Real-time chart containers */
.chart-container {
  position: relative;
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
  border-radius: 8px;
  padding: 16px;
}

.chart-container::before {
  content: '';
  position: absolute;
  top: 8px;
  right: 8px;
  width: 8px;
  height: 8px;
  background: #10b981;
  border-radius: 50%;
  animation: pulse 2s infinite;
}

/* Live metrics styling */
.live-metric {
  position: relative;
  padding: 12px;
  background: white;
  border-radius: 8px;
  border-left: 4px solid #3b82f6;
  transition: all 0.3s ease;
}

.live-metric:hover {
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.15);
}

.live-metric::after {
  content: 'LIVE';
  position: absolute;
  top: 4px;
  right: 8px;
  font-size: 8px;
  font-weight: bold;
  color: #10b981;
  background: #dcfce7;
  padding: 2px 4px;
  border-radius: 4px;
}

/* Admin interface enhancements */
.admin-panel {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 24px;
  border-radius: 12px;
}

.admin-header {
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
  padding-bottom: 16px;
  margin-bottom: 24px;
}

.admin-section {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 8px;
  padding: 16px;
  margin-bottom: 16px;
}

/* Streaming data visualization effects */
.data-point {
  position: relative;
  display: inline-block;
  padding: 8px 12px;
  background: #f0f9ff;
  border-radius: 6px;
  margin: 4px;
  border-left: 3px solid #0284c7;
}

.data-point::before {
  content: '';
  position: absolute;
  top: -2px;
  left: -2px;
  right: -2px;
  bottom: -2px;
  background: linear-gradient(45deg, #0284c7, #0ea5e9);
  border-radius: 8px;
  z-index: -1;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.data-point:hover::before {
  opacity: 0.2;
}

/* Real-time status badges */
.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 8px;
  border-radius: 16px;
  font-size: 12px;
  font-weight: 600;
}

.status-badge.streaming {
  background: #dcfce7;
  color: #166534;
}

.status-badge.paused {
  background: #fef3c7;
  color: #92400e;
}

.status-badge.error {
  background: #fecaca;
  color: #991b1b;
}

/* Enhanced focus states for accessibility */
button:focus-visible {
  outline: 2px solid #3b82f6;
  outline-offset: 2px;
}

/* Loading states for streaming components */
.stream-loading {
  background: linear-gradient(90deg, #f1f5f9 25%, #e2e8f0 50%, #f1f5f9 75%);
  background-size: 200% 100%;
  animation: loading-stream 1.5s infinite;
}

@keyframes loading-stream {
  0% { background-position: -200% 0; }
  100% { background-position: 200% 0; }
}

/* Responsive streaming indicators */
@media (max-width: 768px) {
  .streaming-indicator {
    width: 10px;
    height: 10px;
  }
  
  .connection-dot {
    width: 6px;
    height: 6px;
  }
  
  .admin-card {
    transform: none;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  }
  
  .admin-card:hover {
    transform: translateY(-1px);
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  }
}

/* Tab enhancements */
nav[role="tablist"] button {
  position: relative;
  overflow: hidden;
}

nav[role="tablist"] button::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(59, 130, 246, 0.1), transparent);
  transition: left 0.5s;
}

nav[role="tablist"] button:hover::before {
  left: 100%;
}

/* Professional card shadows */
.shadow-sm {
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06);
}

/* Streaming data table effects */
.streaming-table-row {
  transition: all 0.2s ease;
}

.streaming-table-row:hover {
  background: #f8fafc;
  transform: scale(1.005);
}

.streaming-table-row.new-data {
  animation: highlight-new 2s ease-out;
}

@keyframes highlight-new {
  0% { background: #dbeafe; }
  100% { background: transparent; }
}

/* WebSocket connection status */
.ws-status {
  position: fixed;
  bottom: 20px;
  right: 20px;
  padding: 8px 12px;
  border-radius: 8px;
  background: white;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  border-left: 4px solid #10b981;
  font-size: 12px;
  z-index: 1000;
}

.ws-status.connected::before {
  content: '●';
  color: #10b981;
  margin-right: 6px;
}

.ws-status.disconnected::before {
  content: '●';
  color: #ef4444;
  margin-right: 6px;
}

.ws-status.connecting::before {
  content: '●';
  color: #f59e0b;
  margin-right: 6px;
  animation: pulse 1s infinite;
}
</style>