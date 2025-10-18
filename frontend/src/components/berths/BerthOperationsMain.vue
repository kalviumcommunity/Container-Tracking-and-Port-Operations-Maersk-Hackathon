<!-- Professional Admin Dashboard with Advanced Tab Navigation -->
<template>
  <div class="min-h-screen bg-slate-50">
    <!-- Admin Application Header -->
    <header class="bg-white border-b border-slate-200 shadow-lg sticky top-0 z-50">
      <div class="max-w-8xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex items-center justify-between h-20">
          <!-- Branding & System Info -->
          <div class="flex items-center space-x-6">
            <div class="flex items-center space-x-4">
              <div class="w-12 h-12 bg-gradient-to-br from-blue-600 via-blue-700 to-blue-800 rounded-xl flex items-center justify-center shadow-lg">
                <Anchor class="w-7 h-7 text-white" />
              </div>
              <div>
                <h1 class="text-2xl font-bold text-slate-900 tracking-tight">Port Operations Center</h1>
                <p class="text-sm text-slate-600 font-medium">Administrative Control Panel</p>
              </div>
            </div>
            
            <!-- System Status Indicator -->
            <div class="hidden lg:flex items-center space-x-2 px-3 py-1.5 bg-green-50 border border-green-200 rounded-lg">
              <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
              <span class="text-xs font-semibold text-green-700 uppercase tracking-wider">System Online</span>
            </div>
          </div>

          <!-- Admin Control Actions -->
          <div class="flex items-center space-x-4">
            <!-- Quick Stats -->
            <div class="hidden md:flex items-center space-x-6 px-4 py-2 bg-slate-50 rounded-lg border border-slate-200">
              <div class="text-center">
                <div class="text-lg font-bold text-slate-900">{{ stats.availableBerths }}</div>
                <div class="text-xs text-slate-500 uppercase tracking-wider">Available</div>
              </div>
              <div class="w-px h-8 bg-slate-300"></div>
              <div class="text-center">
                <div class="text-lg font-bold text-slate-900">{{ operations.filter(op => op.status === 'Active').length }}</div>
                <div class="text-xs text-slate-500 uppercase tracking-wider">Active</div>
              </div>
            </div>
            
            <!-- Action Buttons -->
            <div class="flex items-center space-x-3">
              <button
                @click="loadOperationData"
                :disabled="loading"
                class="inline-flex items-center px-4 py-2.5 border border-slate-300 shadow-sm text-sm font-semibold rounded-lg text-slate-700 bg-white hover:bg-slate-50 hover:border-slate-400 focus:outline-none focus:ring-2 focus:ring-offset-1 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200"
              >
                <RefreshCw :class="['w-4 h-4 mr-2', { 'animate-spin': loading }]" />
                {{ loading ? 'Refreshing...' : 'Refresh Data' }}
              </button>
              
              <button
                @click="openCreateBerthModal"
                class="inline-flex items-center px-4 py-2.5 border border-transparent text-sm font-semibold rounded-lg shadow-sm text-white bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 focus:outline-none focus:ring-2 focus:ring-offset-1 focus:ring-blue-500 transition-all duration-200"
              >
                <Plus class="w-4 h-4 mr-2" />
                Create New Berth
              </button>
            </div>
          </div>
        </div>
      </div>
    </header>

    <!-- Professional Tab Navigation System -->
    <nav class="bg-white border-b border-slate-200 shadow-sm sticky top-20 z-40">
      <div class="max-w-8xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex items-center justify-between">
          <!-- Main Navigation Tabs -->
          <div class="flex space-x-0" role="tablist" aria-label="Admin Navigation">
            <button 
              v-for="tab in navigationTabs"
              :key="tab.id"
              @click="switchTab(tab.id)"
              :class="[
                'relative flex items-center px-6 py-4 text-sm font-semibold transition-all duration-300 border-b-3 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 group',
                activeTab === tab.id 
                  ? 'text-blue-700 border-blue-600 bg-blue-50/50' 
                  : 'text-slate-600 border-transparent hover:text-slate-900 hover:border-slate-300 hover:bg-slate-50'
              ]"
              role="tab"
              :aria-selected="activeTab === tab.id"
              :aria-controls="`panel-${tab.id}`"
            >
              <!-- Tab Icon -->
              <component 
                :is="tab.icon" 
                :class="[
                  'w-5 h-5 mr-3 transition-all duration-200',
                  activeTab === tab.id ? 'text-blue-600' : 'text-slate-500 group-hover:text-slate-700'
                ]" 
              />
              
              <!-- Tab Label -->
              <span class="font-semibold">{{ tab.name }}</span>
              
              <!-- Count Badge -->
              <span 
                v-if="tab.count !== undefined" 
                :class="[
                  'ml-3 inline-flex items-center px-2.5 py-1 rounded-full text-xs font-bold transition-all duration-200',
                  activeTab === tab.id 
                    ? 'bg-blue-600 text-white shadow-sm' 
                    : 'bg-slate-200 text-slate-700 group-hover:bg-slate-300'
                ]"
              >
                {{ tab.count }}
              </span>
              
              <!-- Active Tab Indicator -->
              <div 
                v-if="activeTab === tab.id"
                class="absolute bottom-0 left-0 right-0 h-1 bg-gradient-to-r from-blue-500 to-blue-600 rounded-t-sm"
              ></div>
              
              <!-- Hover Indicator -->
              <div 
                v-else
                class="absolute bottom-0 left-0 right-0 h-1 bg-slate-300 opacity-0 group-hover:opacity-100 transition-opacity duration-200 rounded-t-sm"
              ></div>
            </button>
          </div>
          
          <!-- Tab Actions -->
          <div class="flex items-center space-x-4">
            <!-- Tab-specific Actions -->
            <div v-if="activeTab === 'berths'" class="flex items-center space-x-2">
              <button
                @click="openCreateBerthModal"
                class="inline-flex items-center px-3 py-2 text-xs font-medium text-blue-700 bg-blue-50 border border-blue-200 rounded-lg hover:bg-blue-100 focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <Plus class="w-3 h-3 mr-1" />
                Add Berth
              </button>
            </div>
            
            <div v-if="activeTab === 'operations'" class="flex items-center space-x-2">
              <button
                class="inline-flex items-center px-3 py-2 text-xs font-medium text-green-700 bg-green-50 border border-green-200 rounded-lg hover:bg-green-100 focus:outline-none focus:ring-2 focus:ring-green-500"
              >
                <Activity class="w-3 h-3 mr-1" />
                New Operation
              </button>
            </div>
            
            <!-- View Options -->
            <div class="flex items-center space-x-1 bg-slate-100 rounded-lg p-1">
              <button
                @click="viewMode = 'grid'"
                :class="[
                  'p-2 rounded-md text-xs font-medium transition-all duration-200',
                  viewMode === 'grid' ? 'bg-white shadow-sm text-slate-900' : 'text-slate-600 hover:text-slate-900'
                ]"
              >
                Grid
              </button>
              <button
                @click="viewMode = 'list'"
                :class="[
                  'p-2 rounded-md text-xs font-medium transition-all duration-200',
                  viewMode === 'list' ? 'bg-white shadow-sm text-slate-900' : 'text-slate-600 hover:text-slate-900'
                ]"
              >
                List
              </button>
            </div>
          </div>
        </div>
      </div>
    </nav>

    <!-- Enhanced Admin Stats Dashboard -->
    <section class="bg-gradient-to-br from-slate-800 via-slate-900 to-blue-900 text-white">
      <div class="max-w-8xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-6">
          <div class="bg-white/10 backdrop-blur-sm rounded-xl p-4 border border-white/20">
            <div class="flex items-center justify-between">
              <div>
                <div class="text-3xl font-bold">{{ stats.totalBerths }}</div>
                <div class="text-xs text-slate-300 uppercase tracking-wider font-medium">Total Berths</div>
              </div>
              <Anchor class="w-8 h-8 text-white/60" />
            </div>
          </div>
          
          <div class="bg-white/10 backdrop-blur-sm rounded-xl p-4 border border-white/20">
            <div class="flex items-center justify-between">
              <div>
                <div class="text-3xl font-bold text-emerald-400">{{ stats.availableBerths }}</div>
                <div class="text-xs text-slate-300 uppercase tracking-wider font-medium">Available</div>
              </div>
              <div class="w-8 h-8 bg-emerald-500/20 rounded-lg flex items-center justify-center">
                <div class="w-3 h-3 bg-emerald-400 rounded-full"></div>
              </div>
            </div>
          </div>
          
          <div class="bg-white/10 backdrop-blur-sm rounded-xl p-4 border border-white/20">
            <div class="flex items-center justify-between">
              <div>
                <div class="text-3xl font-bold text-amber-400">{{ stats.occupiedBerths }}</div>
                <div class="text-xs text-slate-300 uppercase tracking-wider font-medium">Occupied</div>
              </div>
              <div class="w-8 h-8 bg-amber-500/20 rounded-lg flex items-center justify-center">
                <div class="w-3 h-3 bg-amber-400 rounded-full"></div>
              </div>
            </div>
          </div>
          
          <div class="bg-white/10 backdrop-blur-sm rounded-xl p-4 border border-white/20">
            <div class="flex items-center justify-between">
              <div>
                <div class="text-3xl font-bold text-blue-400">{{ stats.occupancyPercentage }}%</div>
                <div class="text-xs text-slate-300 uppercase tracking-wider font-medium">Utilization</div>
              </div>
              <BarChart3 class="w-8 h-8 text-blue-400/60" />
            </div>
          </div>
          
          <div class="bg-white/10 backdrop-blur-sm rounded-xl p-4 border border-white/20">
            <div class="flex items-center justify-between">
              <div>
                <div class="text-3xl font-bold text-purple-400">{{ stats.totalShips }}</div>
                <div class="text-xs text-slate-300 uppercase tracking-wider font-medium">Total Ships</div>
              </div>
              <div class="w-8 h-8 bg-purple-500/20 rounded-lg flex items-center justify-center">
                <div class="w-4 h-2 bg-purple-400 rounded-sm"></div>
              </div>
            </div>
          </div>
          
          <div class="bg-white/10 backdrop-blur-sm rounded-xl p-4 border border-white/20">
            <div class="flex items-center justify-between">
              <div>
                <div class="text-3xl font-bold text-rose-400">{{ operations.filter(op => op.status === 'Active').length }}</div>
                <div class="text-xs text-slate-300 uppercase tracking-wider font-medium">Active Ops</div>
              </div>
              <Activity class="w-8 h-8 text-rose-400/60" />
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Main Content Area with Tab Panels -->
    <main class="max-w-8xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Tab Content Container -->
      <div class="tab-content">
        <!-- Dashboard Tab Panel -->
        <div 
          v-show="activeTab === 'dashboard'" 
          id="panel-dashboard"
          role="tabpanel"
          aria-labelledby="tab-dashboard"
          class="space-y-8"
        >
          <div class="bg-white rounded-2xl shadow-lg border border-slate-200 overflow-hidden">
            <div class="bg-gradient-to-r from-blue-50 to-indigo-50 px-8 py-6 border-b border-slate-200">
              <div class="flex items-center justify-between">
                <div>
                  <h2 class="text-2xl font-bold text-slate-900">Operations Overview</h2>
                  <p class="text-slate-600 mt-1">Real-time monitoring and management dashboard</p>
                </div>
                <div class="flex items-center space-x-3">
                  <div class="flex items-center space-x-2 px-3 py-2 bg-white rounded-lg border border-slate-200">
                    <div class="w-3 h-3 bg-green-500 rounded-full animate-pulse"></div>
                    <span class="text-sm font-medium text-slate-700">Live Data</span>
                  </div>
                </div>
              </div>
            </div>
            <div class="p-8">
              <DashboardOverview 
                :berths="berths"
                :operations="operations"
                :loading="loading"
                @create-berth="openCreateBerthModal"
                @assign-berth="openAssignmentModal"
                @view-berth="viewBerth"
              />
            </div>
          </div>
        </div>

        <!-- Berths Management Tab Panel -->
        <div 
          v-show="activeTab === 'berths'" 
          id="panel-berths"
          role="tabpanel"
          aria-labelledby="tab-berths"
          class="space-y-6"
        >
          <div class="bg-white rounded-2xl shadow-lg border border-slate-200 overflow-hidden">
            <div class="bg-gradient-to-r from-emerald-50 to-green-50 px-8 py-6 border-b border-slate-200">
              <div class="flex items-center justify-between">
                <div>
                  <h2 class="text-2xl font-bold text-slate-900">Berth Management</h2>
                  <p class="text-slate-600 mt-1">Comprehensive berth operations and assignments</p>
                </div>
                <div class="flex items-center space-x-4">
                  <div class="text-sm text-slate-600 bg-white px-4 py-2 rounded-lg border border-slate-200">
                    <span class="font-semibold">{{ filteredBerths.length }}</span> of <span class="font-semibold">{{ berths.length }}</span> berths displayed
                  </div>
                  <button
                    @click="openCreateBerthModal"
                    class="inline-flex items-center px-4 py-2 bg-emerald-600 text-white text-sm font-semibold rounded-lg hover:bg-emerald-700 focus:outline-none focus:ring-2 focus:ring-emerald-500 transition-all duration-200"
                  >
                    <Plus class="w-4 h-4 mr-2" />
                    Create Berth
                  </button>
                </div>
              </div>
            </div>
            <div class="p-8">
              <BerthManagement
                :berths="filteredBerths"
                :filters="berthFilters"
                :pagination="berthPagination"
                :status-options="berthStatusOptions"
                :port-options="portOptions"
                :loading="loading"
                @create="openCreateBerthModal"
                @edit="editBerth"
                @view="viewBerth"
                @delete="deleteBerth"
                @assign="manageBerthAssignments"
                @filter-change="updateBerthFilters"
                @page-change="updateBerthPage"
              />
            </div>
          </div>
        </div>

        <!-- Operations Management Tab Panel -->
        <div 
          v-show="activeTab === 'operations'" 
          id="panel-operations"
          role="tabpanel"
          aria-labelledby="tab-operations"
          class="space-y-6"
        >
          <div class="bg-white rounded-2xl shadow-lg border border-slate-200 overflow-hidden">
            <div class="bg-gradient-to-r from-amber-50 to-orange-50 px-8 py-6 border-b border-slate-200">
              <div class="flex items-center justify-between">
                <div>
                  <h2 class="text-2xl font-bold text-slate-900">Operations Control</h2>
                  <p class="text-slate-600 mt-1">Monitor and manage active port operations</p>
                </div>
                <div class="flex items-center space-x-3">
                  <div class="text-sm text-slate-600 bg-white px-4 py-2 rounded-lg border border-slate-200">
                    <span class="font-semibold text-amber-600">{{ operations.filter(op => op.status === 'Active').length }}</span> active operations
                  </div>
                </div>
              </div>
            </div>
            <div class="p-8">
              <OperationsManagement
                :operations="operations"
                :assignments="berthAssignments"
                :loading="loading"
                @create-operation="createOperation"
                @update-operation="updateOperation"
                @complete-operation="completeOperation"
              />
            </div>
          </div>
        </div>

        <!-- Analytics Dashboard Tab Panel -->
        <div 
          v-show="activeTab === 'analytics'" 
          id="panel-analytics"
          role="tabpanel"
          aria-labelledby="tab-analytics"
          class="space-y-6"
        >
          <div class="bg-white rounded-2xl shadow-lg border border-slate-200 overflow-hidden">
            <div class="bg-gradient-to-r from-purple-50 to-violet-50 px-8 py-6 border-b border-slate-200">
              <div class="flex items-center justify-between">
                <div>
                  <h2 class="text-2xl font-bold text-slate-900">Analytics & Insights</h2>
                  <p class="text-slate-600 mt-1">Performance metrics and operational intelligence</p>
                </div>
                <div class="flex items-center space-x-3">
                  <div class="text-sm text-slate-600 bg-white px-4 py-2 rounded-lg border border-slate-200">
                    Real-time analytics
                  </div>
                </div>
              </div>
            </div>
            <div class="p-8">
              <AnalyticsDashboard
                :berth-stats="berthStatistics"
                :berths="berths"
                :assignments="berthAssignments"
                :loading="loading"
              />
            </div>
          </div>
        </div>

        <!-- System Settings Tab Panel -->
        <div 
          v-show="activeTab === 'settings'" 
          id="panel-settings"
          role="tabpanel"
          aria-labelledby="tab-settings"
          class="space-y-6"
        >
          <div class="bg-white rounded-2xl shadow-lg border border-slate-200 overflow-hidden">
            <div class="bg-gradient-to-r from-slate-50 to-gray-50 px-8 py-6 border-b border-slate-200">
              <div class="flex items-center justify-between">
                <div>
                  <h2 class="text-2xl font-bold text-slate-900">System Configuration</h2>
                  <p class="text-slate-600 mt-1">Administrative settings and system preferences</p>
                </div>
              </div>
            </div>
            <div class="p-8">
              <div class="text-center py-16">
                <Settings class="w-16 h-16 text-slate-400 mx-auto mb-6" />
                <h3 class="text-xl font-semibold text-slate-900 mb-3">Configuration Panel</h3>
                <p class="text-slate-600 max-w-md mx-auto">System configuration and administrative settings will be available here. Contact your system administrator for access.</p>
              </div>
            </div>
          </div>
        </div>

        <!-- User Management Tab Panel -->
        <div 
          v-show="activeTab === 'users'" 
          id="panel-users"
          role="tabpanel"
          aria-labelledby="tab-users"
          class="space-y-6"
        >
          <div class="bg-white rounded-2xl shadow-lg border border-slate-200 overflow-hidden">
            <div class="bg-gradient-to-r from-rose-50 to-pink-50 px-8 py-6 border-b border-slate-200">
              <div class="flex items-center justify-between">
                <div>
                  <h2 class="text-2xl font-bold text-slate-900">User Management</h2>
                  <p class="text-slate-600 mt-1">Manage user access, roles, and permissions</p>
                </div>
              </div>
            </div>
            <div class="p-8">
              <div class="text-center py-16">
                <Users class="w-16 h-16 text-slate-400 mx-auto mb-6" />
                <h3 class="text-xl font-semibold text-slate-900 mb-3">Access Control</h3>
                <p class="text-slate-600 max-w-md mx-auto">User management functionality will be implemented here. This includes role-based access control and permission management.</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Modals -->
    <BerthFormModal
      :isOpen="showBerthModal"
      :berth="isEditingBerth ? selectedBerth : null"
      :ports="ports"
      @close="closeBerthModal"
      @submit="handleBerthSubmit"
    />

    <BerthDetailsModal
      :isOpen="showDetailsModal"
      :berth="selectedBerth"
      :ports="ports"
      @close="closeDetailsModal"
      @edit="handleEditFromDetails"
      @assign="handleAssignFromDetails"
    />

    <BerthAssignmentModal
      :isOpen="showAssignmentModal"
      :berth="selectedBerth"
      @close="closeAssignmentModal"
      @submit="handleAssignmentSubmit"
    />

    <!-- Loading Overlay -->
    <LoadingOverlay v-if="loading && isInitialLoad" />

    <!-- Notifications -->
    <NotificationCenter 
      v-if="currentNotification.type"
      :notification="currentNotification"
      @clear="clearNotification"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { 
  LayoutDashboard, 
  Anchor, 
  Activity, 
  BarChart3, 
  Settings,
  RefreshCw,
  Plus,
  Users
} from 'lucide-vue-next';

// Import types
import type { Port } from '../../types/port';
import type { Berth, BerthCreateUpdate, BerthAssignment } from '../../types/berth';
import type { Ship as ShipType } from '../../types/ship';
import type { Container } from '../../types/container';

// Import components
import DashboardOverview from './DashboardOverview.vue';
import BerthManagement from './BerthManagement.vue';
import OperationsManagement from './OperationsManagement.vue';
import AnalyticsDashboard from './AnalyticsDashboard.vue';
import BerthFormModal from '../modals/BerthFormModal.vue';
import BerthDetailsModal from '../modals/BerthDetailsModal.vue';
import BerthAssignmentModal from '../modals/BerthAssignmentModal.vue';
import LoadingOverlay from './LoadingOverlay.vue';
import NotificationCenter from './NotificationCenter.vue';

// Import services
import { portApi, berthApi, berthAssignmentApi, shipApi, containerApi } from '../../services/api';

// Component state
const loading = ref(true);
const isInitialLoad = ref(true);
const activeTab = ref('dashboard');
const viewMode = ref('grid'); // New view mode state

// Data state
const berths = ref<Berth[]>([]);
const ships = ref<ShipType[]>([]);
const containers = ref<Container[]>([]);
const berthAssignments = ref<BerthAssignment[]>([]);
const ports = ref<Port[]>([]);
const operations = ref<any[]>([]);
const notifications = ref<Array<{id: number, type: 'success' | 'error' | 'info', message: string}>>([]);
const currentNotification = ref<{type: 'success' | 'error' | 'info' | null, message: string}>({ type: null, message: '' });

// Modal state
const showBerthModal = ref(false);
const showDetailsModal = ref(false);
const showAssignmentModal = ref(false);
const selectedBerth = ref<Berth | null>(null);
const isEditingBerth = ref(false);
const isSubmittingBerth = ref(false);
const isSubmittingAssignment = ref(false);

// Filter and pagination state
const berthFilters = ref({
  status: '',
  portId: '',
  berthType: '',
  minCapacity: null,
  search: '',
  pageSize: 12,
  page: 1
});

const berthPagination = ref({
  page: 1,
  pageSize: 12,
  totalCount: 0,
  totalPages: 1,
  hasPreviousPage: false,
  hasNextPage: false
});

// Navigation configuration
const navigationTabs = computed(() => [
  {
    id: 'dashboard',
    name: 'Dashboard',
    icon: LayoutDashboard,
    count: undefined
  },
  {
    id: 'berths',
    name: 'Berths',
    icon: Anchor,
    count: berths.value.length
  },
  {
    id: 'operations',
    name: 'Operations',
    icon: Activity,
    count: operations.value.filter(op => op.status === 'Active').length
  },
  {
    id: 'analytics',
    name: 'Analytics',
    icon: BarChart3,
    count: undefined
  },
  {
    id: 'settings',
    name: 'Settings',
    icon: Settings,
    count: undefined
  },
  {
    id: 'users',
    name: 'Users',
    icon: Users,
    count: undefined
  }
]);

// Computed properties
const stats = computed(() => {
  const totalBerths = berths.value.length;
  const occupiedBerths = berthAssignments.value.length;
  const availableBerths = totalBerths - occupiedBerths;
  const availabilityPercentage = totalBerths > 0 ? Math.round((availableBerths / totalBerths) * 100) : 0;
  const occupancyPercentage = totalBerths > 0 ? Math.round((occupiedBerths / totalBerths) * 100) : 0;
  const totalShips = ships.value.length;
  const dockedShips = berthAssignments.value.length;

  return {
    totalBerths,
    occupiedBerths,
    availableBerths,
    availabilityPercentage,
    occupancyPercentage,
    totalShips,
    dockedShips
  };
});

const berthStatistics = computed(() => {
  const totalBerths = berths.value.length;
  const totalCapacity = berths.value.reduce((sum, berth) => sum + (berth.capacity || 0), 0);
  const currentOccupancy = berths.value.reduce((sum, berth) => sum + (berth.currentLoad || 0), 0);
  
  return {
    totalBerths,
    activeBerths: berths.value.filter(b => b.status !== 'Inactive').length,
    availableBerths: berths.value.filter(b => b.status === 'Available').length,
    totalCapacity,
    currentOccupancy
  };
});

const berthStatusOptions = computed(() => [
  'Available', 'Occupied', 'Under Maintenance', 'Reserved', 'Full', 'Partially Occupied', 'Inactive'
]);

const portOptions = computed(() => 
  ports.value.map(port => ({ id: port.portId, name: port.name }))
);

const filteredBerths = computed(() => {
  return berths.value.filter(berth => {
    const matchesStatus = !berthFilters.value.status || berth.status === berthFilters.value.status;
    const matchesPort = !berthFilters.value.portId || berth.portId === Number(berthFilters.value.portId);
    const matchesType = !berthFilters.value.berthType || berth.type === berthFilters.value.berthType;
    const matchesCapacity = !berthFilters.value.minCapacity || (berth.capacity || 0) >= berthFilters.value.minCapacity;
    const matchesSearch = !berthFilters.value.search || 
      berth.name?.toLowerCase().includes(berthFilters.value.search.toLowerCase()) ||
      berth.notes?.toLowerCase().includes(berthFilters.value.search.toLowerCase());
    
    return matchesStatus && matchesPort && matchesType && matchesCapacity && matchesSearch;
  });
});

const userPermissions = computed(() => {
  // This would be fetched from your auth system
  return {
    canManageBerths: true,
    canCreateOperations: true,
    canViewAnalytics: true,
    canManageSettings: false
  };
});

// Methods
const switchTab = (tabId: string) => {
  activeTab.value = tabId;
  // Add any tab-specific logic here

};

const loadOperationData = async () => {
  try {
    loading.value = true;

    const [portsResponse, berthsResponse, shipsResponse, containersResponse, assignmentsResponse] = 
      await Promise.all([
        portApi.getAll(),
        berthApi.getAll(),
        shipApi.getAll(),
        containerApi.getAll(),
        berthAssignmentApi.getAll()
      ]);

    ports.value = portsResponse.data || [];
    berths.value = berthsResponse.data || [];
    ships.value = (shipsResponse.data || []).map(ship => ({
      ...ship,
      shipId: ship.shipId || (ship as any).id || 0,
      containerCount: 0,
      status: ship.status || 'Unknown'
    }));
    containers.value = containersResponse.data || [];
    berthAssignments.value = assignmentsResponse.data || [];

    updateOperations();
    updateBerthPagination();

    addNotification({
      type: 'success',
      message: 'Port operations data updated successfully'
    });

  } catch (error) {
    console.error('Error loading operation data:', error);
    loadMockData();
    addNotification({
      type: 'info',
      message: 'Could not connect to the server. Using sample data for demonstration.'
    });
  } finally {
    loading.value = false;
    isInitialLoad.value = false;
  }
};

const loadMockData = () => {
  // Same mock data as before but organized better
  ports.value = [
    { portId: 1, name: 'Port of Hamburg', country: 'Germany', location: 'Hamburg, Germany', totalContainerCapacity: 10000 },
    { portId: 2, name: 'Port of Rotterdam', country: 'Netherlands', location: 'Rotterdam, Netherlands', totalContainerCapacity: 12000 }
  ];

  berths.value = [
    { 
      berthId: 1, name: 'Berth A1', status: 'Available', 
      portId: 1, portName: 'Port of Hamburg', capacity: 500, currentLoad: 0,
      type: 'Container', maxShipLength: 300, maxDraft: 14.0, craneCount: 4,
      activeAssignmentCount: 0
    },
    { 
      berthId: 2, name: 'Berth A2', status: 'Occupied', 
      portId: 1, portName: 'Port of Hamburg', capacity: 400, currentLoad: 320,
      type: 'Container', maxShipLength: 250, maxDraft: 13.0, craneCount: 3,
      activeAssignmentCount: 1
    },
    { 
      berthId: 3, name: 'Berth B1', status: 'Available', 
      portId: 1, portName: 'Port of Hamburg', capacity: 300, currentLoad: 0,
      type: 'Bulk', maxShipLength: 200, maxDraft: 11.0, craneCount: 2,
      activeAssignmentCount: 0
    },
    { 
      berthId: 4, name: 'Berth B2', status: 'Under Maintenance', 
      portId: 1, portName: 'Port of Hamburg', capacity: 350, currentLoad: 0,
      type: 'Container', maxShipLength: 280, maxDraft: 12.5, craneCount: 3,
      activeAssignmentCount: 0
    }
  ];

  ships.value = [
    { shipId: 1, id: 1, name: 'MV Ocean Explorer', status: 'Docked', containerCount: 150 },
    { shipId: 2, id: 2, name: 'MV Cargo Master', status: 'Docked', containerCount: 120 },
    { shipId: 3, id: 3, name: 'MV Sea Voyager', status: 'In Transit', containerCount: 80 }
  ];

  containers.value = [
    { containerId: 'CONT-001', cargoType: 'Electronics', type: 'Dry', status: 'Loading', currentLocation: 'Hamburg Port', createdAt: '2024-01-01', updatedAt: '2024-01-01', shipId: 1 },
    { containerId: 'CONT-002', cargoType: 'Food', type: 'Refrigerated', status: 'Loaded', currentLocation: 'Hamburg Port', createdAt: '2024-01-01', updatedAt: '2024-01-01', shipId: 1 },
    { containerId: 'CONT-003', cargoType: 'Machinery', type: 'Dry', status: 'Unloading', currentLocation: 'Hamburg Port', createdAt: '2024-01-01', updatedAt: '2024-01-01', shipId: 2 }
  ];

  berthAssignments.value = [
    { id: 1, berthId: 2, berthName: 'Berth A2', shipId: 1, status: 'Active', assignedAt: new Date().toISOString(), assignmentType: 'Ship' },
    { id: 2, berthId: 6, berthName: 'Berth C1', shipId: 2, status: 'Active', assignedAt: new Date().toISOString(), assignmentType: 'Ship' }
  ];

  updateOperations();
  updateBerthPagination();
};

const updateOperations = () => {
  operations.value = berthAssignments.value.slice(0, 4).map((assignment, index) => {
    const ship = ships.value.find(s => s.shipId === assignment.shipId);
    const berth = berths.value.find(b => b.berthId === assignment.berthId);
    
    const operationTypes = ['Loading', 'Unloading', 'Inspection', 'Refueling'];
    const priorities = ['High', 'Medium', 'Low'];
    const etas = ['2 hours', '4 hours', '30 min', '1.5 hours'];
    
    return {
      id: `OP-${assignment.id}`,
      type: operationTypes[index % operationTypes.length],
      vessel: ship ? ship.name : `Ship ${assignment.shipId}`,
      berth: berth ? berth.name : `Berth ${assignment.berthId}`,
      progress: 0, // Show 0 instead of random data when backend not connected
      eta: etas[index % etas.length],
      priority: priorities[index % priorities.length],
      status: 'Active'
    };
  });
};

const updateBerthPagination = () => {
  const filtered = filteredBerths.value;
  const totalCount = filtered.length;
  const totalPages = Math.ceil(totalCount / berthFilters.value.pageSize);
  const page = Math.min(berthFilters.value.page, totalPages || 1);
  
  berthPagination.value = {
    page,
    pageSize: berthFilters.value.pageSize,
    totalCount,
    totalPages,
    hasPreviousPage: page > 1,
    hasNextPage: page < totalPages
  };
};

// Modal handlers
const openCreateBerthModal = () => {
  selectedBerth.value = null;
  isEditingBerth.value = false;
  showBerthModal.value = true;
};

const editBerth = (berth: any) => {
  selectedBerth.value = berth as Berth;
  isEditingBerth.value = true;
  showBerthModal.value = true;
};

const viewBerth = (berth: any) => {
  selectedBerth.value = berth as Berth;
  isEditingBerth.value = false;
  showBerthModal.value = true;
};

const deleteBerth = async (berth: any) => {
  const berthObj = berth as Berth;
  if (confirm(`Are you sure you want to delete berth "${berthObj.name}"?`)) {
    try {
      await berthApi.delete(berthObj.berthId || (berthObj as any).id);
      await loadOperationData();
      addNotification({
        type: 'success',
        message: `Berth ${berthObj.name} has been deleted successfully`
      });
    } catch (error) {
      console.error('Error deleting berth:', error);
      addNotification({
        type: 'error',
        message: 'Failed to delete the berth. Please try again.'
      });
    }
  }
};

const manageBerthAssignments = (berth: any) => {
  selectedBerth.value = berth;
  showAssignmentModal.value = true;
};

const openAssignmentModal = () => {
  showAssignmentModal.value = true;
};

const closeBerthModal = () => {
  showBerthModal.value = false;
  selectedBerth.value = null;
  isEditingBerth.value = false;
};

const closeAssignmentModal = () => {
  showAssignmentModal.value = false;
  selectedBerth.value = null;
};

const closeDetailsModal = () => {
  showDetailsModal.value = false;
  selectedBerth.value = null;
};

const handleEditFromDetails = () => {
  showDetailsModal.value = false;
  isEditingBerth.value = true;
  showBerthModal.value = true;
};

const handleAssignFromDetails = () => {
  showDetailsModal.value = false;
  showAssignmentModal.value = true;
};

const handleBerthSubmit = async (berthData: BerthCreateUpdate) => {
  try {
    isSubmittingBerth.value = true;
    
    if (isEditingBerth.value && selectedBerth.value) {
      await berthApi.update(selectedBerth.value.berthId, berthData);
      addNotification({
        type: 'success',
        message: `Berth "${berthData.name}" has been updated successfully.`
      });
    } else {
      await berthApi.create(berthData);
      addNotification({
        type: 'success',
        message: `Berth "${berthData.name}" has been created successfully.`
      });
    }
    
    await loadOperationData();
    closeBerthModal();
  } catch (error) {
    console.error('Error saving berth:', error);
    addNotification({
      type: 'error',
      message: 'Failed to save the berth. Please try again.'
    });
  } finally {
    isSubmittingBerth.value = false;
  }
};

const handleAssignmentSubmit = async (assignmentData: any) => {
  try {
    isSubmittingAssignment.value = true;
    await berthAssignmentApi.create(assignmentData);
    await loadOperationData();
    closeAssignmentModal();
    addNotification({
      type: 'success',
      message: 'Berth assignment has been created successfully.'
    });
  } catch (error) {
    console.error('Error creating assignment:', error);
    addNotification({
      type: 'error',
      message: 'Failed to create the assignment. Please try again.'
    });
  } finally {
    isSubmittingAssignment.value = false;
  }
};

// Filter and pagination handlers
const updateBerthFilters = (filters: any) => {
  berthFilters.value = { ...berthFilters.value, ...filters };
  updateBerthPagination();
};

const updateBerthPage = (page: number) => {
  berthFilters.value.page = page;
  updateBerthPagination();
};

// Operations handlers
const createOperation = (operationData: any) => {

  // Implement operation creation
};

const updateOperation = (operation: any) => {

  // Implement operation update
};

const completeOperation = (operation: any) => {

  // Implement operation completion
};

// Settings handlers
const updateSettings = (settings: any) => {

  // Implement settings update
};

// Notification system
const addNotification = (notification: {type: 'success' | 'error' | 'info', message: string}) => {
  currentNotification.value = notification;
  
  // Auto-clear notifications after 5 seconds
  if (notification.type === 'success' || notification.type === 'info') {
    setTimeout(() => {
      clearNotification();
    }, 5000);
  }
};

const dismissNotification = (id: number) => {
  const index = notifications.value.findIndex(n => n.id === id);
  if (index > -1) {
    notifications.value.splice(index, 1);
  }
};

const clearNotification = () => {
  currentNotification.value = { type: null, message: '' };
};

// Lifecycle
onMounted(() => {
  loadOperationData();
});

// Watch for tab changes to update URL or perform tab-specific actions
watch(activeTab, (newTab) => {

  // You could update the URL here if using Vue Router
});
</script>

<style scoped>
/* Enhanced Professional Styling */
.tab-content {
  min-height: 60vh;
}

/* Smooth tab transitions */
.tab-content > div {
  animation: fadeIn 0.3s ease-in-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Enhanced gradient backgrounds */
.bg-gradient-to-br {
  background-image: linear-gradient(to bottom right, var(--tw-gradient-stops));
}

.bg-gradient-to-r {
  background-image: linear-gradient(to right, var(--tw-gradient-stops));
}

/* Professional button hover effects */
button {
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

button:hover {
  transform: translateY(-1px);
}

button:active {
  transform: translateY(0);
}

/* Enhanced shadow utilities */
.shadow-lg {
  box-shadow: 0 10px 25px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
}

/* Tab navigation enhancements */
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

/* Professional card styling */
.rounded-2xl {
  border-radius: 1rem;
}

/* Backdrop blur for stats cards */
.backdrop-blur-sm {
  backdrop-filter: blur(4px);
}

/* Custom scrollbar for overflow content */
.tab-content::-webkit-scrollbar {
  width: 6px;
}

.tab-content::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 3px;
}

.tab-content::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 3px;
}

.tab-content::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}

/* Focus states for accessibility */
button:focus-visible {
  outline: 2px solid #3b82f6;
  outline-offset: 2px;
}

/* Enhanced loading states */
.animate-spin {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

.animate-pulse {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

/* Professional glass-morphism effects */
.glass {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

/* Responsive design enhancements */
@media (max-width: 768px) {
  .max-w-8xl {
    max-width: 100%;
  }
  
  nav[role="tablist"] {
    overflow-x: auto;
    scrollbar-width: none;
    -ms-overflow-style: none;
  }
  
  nav[role="tablist"]::-webkit-scrollbar {
    display: none;
  }
  
  .grid-cols-6 {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 640px) {
  .grid-cols-6 {
    grid-template-columns: repeat(1, minmax(0, 1fr));
  }
}

/* Enhanced text styling */
.tracking-tight {
  letter-spacing: -0.025em;
}

.tracking-wider {
  letter-spacing: 0.05em;
}

/* Professional spacing */
.space-x-0 > :not([hidden]) ~ :not([hidden]) {
  margin-left: 0;
}

/* Border utilities */
.border-b-3 {
  border-bottom-width: 3px;
}

/* Professional color transitions */
.transition-all {
  transition-property: all;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 300ms;
}

/* Enhanced container max-width */
.max-w-8xl {
  max-width: 88rem;
}

/* Z-index utilities for proper stacking */
.z-40 {
  z-index: 40;
}

.z-50 {
  z-index: 50;
}

/* Sticky positioning */
.sticky {
  position: sticky;
}

.top-0 {
  top: 0;
}

.top-20 {
  top: 5rem;
}
</style>
