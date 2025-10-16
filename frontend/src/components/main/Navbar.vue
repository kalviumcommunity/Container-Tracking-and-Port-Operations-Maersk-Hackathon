<template>
  <nav class="bg-white shadow-lg border-b border-slate-200 sticky top-0 z-50">
    <div class="max-w-7xl mx-auto px-6">
      <div class="flex items-center justify-between h-16">
        <!-- Logo/Brand -->
        <div class="flex items-center gap-4">
          <router-link to="/" class="flex items-center gap-3 hover:opacity-80 transition-opacity">
            <div class="p-2 bg-blue-600 rounded-xl shadow-lg">
              <Ship :size="24" class="text-white" />
            </div>
            <div>
              <h1 class="text-xl font-bold text-slate-900">PortTrack</h1>
              <p class="text-xs text-slate-600">Container Operations</p>
            </div>
          </router-link>
        </div>

        <!-- Desktop Navigation -->
        <div class="hidden md:flex items-center gap-4">
          <!-- Navigation Links -->
          <div class="flex items-center" v-if="isAuthenticated">
            <router-link
              v-for="item in filteredNavigationItems"
              :key="item.name"
              :to="item.path"
              class="flex items-center gap-2 px-3 py-2 mx-1 rounded-lg font-medium transition-all duration-200 hover:bg-slate-50"
              :class="[
                currentRoute.path === item.path
                  ? 'text-blue-600 bg-blue-50 border border-blue-200'
                  : 'text-slate-600 hover:text-slate-900'
              ]"
            >
              <component :is="item.icon" :size="18" />
              <span>{{ item.name }}</span>
            </router-link>
          </div>

          <!-- Authentication Section -->
          <div class="flex items-center gap-2">
            <div v-if="isAuthenticated" class="relative">
              <!-- User Dropdown -->
              <div class="relative">
                <button 
                  @click="showUserDropdown = !showUserDropdown"
                  class="flex items-center space-x-2 px-3 py-2 bg-slate-50 hover:bg-slate-100 rounded-lg transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
                >
                  <User class="w-5 h-5 text-slate-600" />
                  <span class="text-sm font-medium text-slate-700">{{ currentUser?.username || 'User' }}</span>
                  <ChevronDown class="w-4 h-4 text-slate-500 transition-transform duration-200" :class="{ 'rotate-180': showUserDropdown }" />
                </button>

                <!-- Dropdown Menu -->
                <div 
                  v-if="showUserDropdown"
                  class="absolute right-0 mt-2 w-72 bg-white rounded-lg shadow-xl border border-slate-200 py-2 z-50"
                  @click.stop
                >
                  <!-- User Info Header -->
                  <div class="px-4 py-3 border-b border-slate-100">
                    <div class="flex items-center space-x-3">
                      <div class="w-10 h-10 bg-blue-600 rounded-full flex items-center justify-center">
                        <User class="w-5 h-5 text-white" />
                      </div>
                      <div class="flex-1 min-w-0">
                        <p class="text-sm font-semibold text-slate-900 truncate">{{ currentUser?.fullName || currentUser?.username }}</p>
                        <p class="text-xs text-slate-500 truncate">{{ currentUser?.email }}</p>
                        <div class="flex flex-wrap gap-1 mt-1">
                          <span 
                            v-for="role in currentUser?.roles" 
                            :key="role"
                            class="inline-block bg-blue-100 text-blue-800 text-xs px-2 py-0.5 rounded-full"
                          >
                            {{ role }}
                          </span>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Menu Items -->
                  <div class="py-1">
                    <!-- Role Application -->
                    <button
                      @click="openRoleApplication"
                      class="w-full text-left px-4 py-2 text-sm text-slate-700 hover:bg-slate-50 flex items-center space-x-3"
                    >
                      <Shield class="w-4 h-4 text-slate-400" />
                      <span>Request Additional Access</span>
                      <span v-if="pendingApplicationsCount > 0" class="ml-auto bg-orange-100 text-orange-800 text-xs px-2 py-0.5 rounded-full">
                        {{ pendingApplicationsCount }}
                      </span>
                    </button>

                    <!-- Change Password -->
                    <button
                      @click="openChangePassword"
                      class="w-full text-left px-4 py-2 text-sm text-slate-700 hover:bg-slate-50 flex items-center space-x-3"
                    >
                      <Key class="w-4 h-4 text-slate-400" />
                      <span>Change Password</span>
                    </button>

                    <!-- Account Settings -->
                    <button
                      @click="openAccountSettings"
                      class="w-full text-left px-4 py-2 text-sm text-slate-700 hover:bg-slate-50 flex items-center space-x-3"
                    >
                      <Settings class="w-4 h-4 text-slate-400" />
                      <span>Account Settings</span>
                    </button>

                    <!-- Admin Dashboard (if admin) -->
                    <router-link
                      v-if="isAdmin"
                      to="/admin-dashboard"
                      @click="showUserDropdown = false"
                      class="w-full text-left px-4 py-2 text-sm text-slate-700 hover:bg-slate-50 flex items-center space-x-3 no-underline"
                    >
                      <UserCog class="w-4 h-4 text-slate-400" />
                      <span>Admin Dashboard</span>
                      <span class="ml-auto bg-red-100 text-red-800 text-xs px-2 py-0.5 rounded-full">Admin</span>
                    </router-link>

                    <hr class="my-1 border-slate-100" />

                    <!-- Logout -->
                    <button
                      @click="handleLogout"
                      class="w-full text-left px-4 py-2 text-sm text-red-600 hover:bg-red-50 flex items-center space-x-3"
                    >
                      <LogOut class="w-4 h-4 text-red-500" />
                      <span>Sign Out</span>
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Login/Register buttons for non-authenticated users -->
            <div v-else class="flex items-center space-x-2">
              <button
                @click="showLoginModal = true"
                class="flex items-center gap-2 px-3 py-2 text-slate-600 hover:text-slate-900 hover:bg-slate-50 rounded-lg transition-all duration-200"
              >
                <LogIn :size="16" />
                <span class="text-sm font-medium">Sign In</span>
              </button>
              <button
                @click="showRegisterModal = true"
                class="flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-all duration-200"
              >
                <UserPlus :size="16" />
                <span class="text-sm font-medium">Sign Up</span>
              </button>
            </div>
        </div>

        <!-- Mobile Menu Button -->
        <div class="md:hidden">
          <button
            @click="toggleMobileMenu"
            class="p-3 rounded-lg text-slate-600 hover:text-slate-900 hover:bg-slate-50 transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50"
            aria-label="Toggle mobile menu"
          >
            <div class="relative w-6 h-6">
              <span
                :class="[
                  'absolute top-0 left-0 w-6 h-0.5 bg-current transition-all duration-300 ease-in-out',
                  isMobileMenuOpen ? 'rotate-45 translate-y-2.5' : 'rotate-0 translate-y-0'
                ]"
              ></span>
              <span
                :class="[
                  'absolute top-2.5 left-0 w-6 h-0.5 bg-current transition-all duration-300 ease-in-out',
                  isMobileMenuOpen ? 'opacity-0' : 'opacity-100'
                ]"
              ></span>
              <span
                :class="[
                  'absolute top-5 left-0 w-6 h-0.5 bg-current transition-all duration-300 ease-in-out',
                  isMobileMenuOpen ? '-rotate-45 -translate-y-2.5' : 'rotate-0 translate-y-0'
                ]"
              ></span>
            </div>
          </button>
        </div>
        </div>
      </div>

      <!-- Mobile Navigation -->
      <div
        v-if="isMobileMenuOpen"
        class="md:hidden border-t border-slate-200 bg-white animate-slideDown"
      >
        <div class="py-4 px-2">
          <!-- Navigation Links (only if authenticated) -->
          <div v-if="isAuthenticated" class="flex flex-col gap-1 mb-4">
            <router-link
              v-for="item in filteredNavigationItems"
              :key="item.name"
              :to="item.path"
              @click="closeMobileMenu"
              class="flex items-center gap-3 px-4 py-3 mx-2 rounded-lg font-medium transition-all duration-200 hover:scale-105"
              :class="[
                currentRoute.path === item.path
                  ? 'text-blue-600 bg-blue-50 border border-blue-200 shadow-sm'
                  : 'text-slate-600 hover:text-slate-900 hover:bg-slate-50'
              ]"
            >
              <component :is="item.icon" :size="20" />
              <span>{{ item.name }}</span>
            </router-link>
          </div>

          <!-- Mobile Authentication -->
          <div class="border-t border-slate-200 pt-4">
            <div v-if="isAuthenticated" class="px-2">
              <!-- User Info -->
              <div class="flex items-center gap-3 px-4 py-3 bg-slate-50 rounded-lg mb-2">
                <User :size="20" class="text-slate-600" />
                <span class="font-medium text-slate-700">{{ currentUser?.username }}</span>
              </div>
              <!-- Logout Button -->
              <button
                @click="handleLogout"
                class="w-full flex items-center gap-3 px-4 py-3 text-red-600 hover:bg-red-50 rounded-lg transition-all duration-200"
              >
                <LogOut :size="20" />
                <span class="font-medium">Logout</span>
              </button>
            </div>
            <div v-else class="flex flex-col gap-2 px-2">
              <!-- Login Button -->
              <button
                @click="showLoginModal = true; closeMobileMenu()"
                class="w-full flex items-center justify-center gap-2 px-4 py-3 text-blue-600 border border-blue-600 hover:bg-blue-50 rounded-lg transition-all duration-200"
              >
                <LogIn :size="20" />
                <span class="font-medium">Sign In</span>
              </button>
              <!-- Register Button -->
              <button
                @click="showRegisterModal = true; closeMobileMenu()"
                class="w-full flex items-center justify-center gap-2 px-4 py-3 bg-blue-600 text-white hover:bg-blue-700 rounded-lg transition-all duration-200"
              >
                <UserPlus :size="20" />
                <span class="font-medium">Sign Up</span>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
    

    <!-- Authentication Modals -->
    <!-- Login Modal -->
    <div v-if="showLoginModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <LoginForm
        @login-success="handleLoginSuccess"
        @show-register="showRegisterModal = true; showLoginModal = false"
        @cancel="showLoginModal = false"
      />
    </div>

    <!-- Register Modal -->
    <div v-if="showRegisterModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <RegistrationForm
        @register-success="handleRegisterSuccess"
        @show-login="showLoginModal = true; showRegisterModal = false"
        @cancel="showRegisterModal = false"
      />
    </div>

    <!-- Role Application Modal -->
    <div 
      v-if="showRoleApplicationModal" 
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click="showRoleApplicationModal = false"
    >
      <RoleApplication 
        @close="showRoleApplicationModal = false"
        @application-submitted="onApplicationSubmitted"
      />
    </div>

    <!-- Change Password Modal -->
    <div 
      v-if="showChangePasswordModal" 
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click="showChangePasswordModal = false"
    >
      <ChangePassword 
        @close="showChangePasswordModal = false"
        @password-changed="onPasswordChanged"
      />
    </div>

    <!-- Account Settings Modal -->
    <div 
      v-if="showAccountSettingsModal" 
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
      @click="showAccountSettingsModal = false"
    >
      <AccountSettings 
        @close="showAccountSettingsModal = false"
        @profile-updated="onProfileUpdated"
      />
    </div>
  </nav>
</template>

<script>
import { useRoute } from 'vue-router'
import { 
  Ship, 
  Home, 
  BarChart3, 
  Container, 
  Anchor, 
  Activity,
  User,
  Users,
  LogIn,
  LogOut,
  UserPlus,
  ChevronDown,
  Shield,
  Key,
  Settings,
  UserCog
} from 'lucide-vue-next'
import LoginForm from '../../forms/LoginForm.vue'
import RegistrationForm from '../../forms/RegistrationForm.vue'
import RoleApplication from '../RoleApplication.vue'
import ChangePassword from '../ChangePassword.vue'
import AccountSettings from '../AccountSettings.vue'
import { authApi, roleApplicationApi } from '../../services/api.js'
import { useToast } from '../../composables/useToast'

export default {
  name: 'Navbar',
  components: {
    Ship,
    Home,
    BarChart3,
    Container,
    Anchor,
    Activity,
    User,
    LogIn,
    LogOut,
    UserPlus,
    ChevronDown,
    Shield,
    Key,
    Settings,
    UserCog,

    LoginForm,
    RegistrationForm,
    RoleApplication,
    ChangePassword,
    AccountSettings
  },
  setup() {
    const route = useRoute()
    const { success, info } = useToast()
    return { currentRoute: route, toast: { success, info } }
  },
  data() {
    return {
      isMobileMenuOpen: false,
      showLoginModal: false,
      showRegisterModal: false,
      showUserDropdown: false,
      showRoleApplicationModal: false,
      showChangePasswordModal: false,
      showAccountSettingsModal: false,
      currentUser: null,
      isAuthenticated: false,
      pendingApplicationsCount: 0,
      // Navigation items configuration
      navigationItems: [
        {
          name: 'Home',
          path: '/',
          icon: Home
        },
        {
          name: 'Dashboard',
          path: '/dashboard',
          icon: BarChart3
        },
        {
          name: 'Containers',
          path: '/container-management',
          icon: Container
        },
        {
          name: 'Berth Operations',
          path: '/berth-operation-management',
          icon: Anchor
        },
        {
          name: 'Event Stream',
          path: '/event-streaming',
          icon: Activity
        }
      ]
    }
  },
  computed: {
    isAdmin() {
      return this.currentUser?.roles?.includes('Admin') || 
             this.currentUser?.roles?.includes('SuperAdmin')

    },
    filteredNavigationItems() {
      // Return all navigation items for authenticated users
      // No role-based filtering needed for main navigation
      return this.navigationItems
    }
  },
  async mounted() {
    await this.checkAuthStatus()
    if (this.isAuthenticated) {
      await this.loadPendingApplicationsCount()
    }
    document.addEventListener('click', this.handleClickOutside)
  },
  beforeUnmount() {
    document.removeEventListener('click', this.handleClickOutside)
  },
  methods: {
    // Mobile menu methods
    toggleMobileMenu() {
      this.isMobileMenuOpen = !this.isMobileMenuOpen
    },

    closeMobileMenu() {
      this.isMobileMenuOpen = false
    },

    // Navigation helper methods
    isActivePage(path) {
      // Check if current route matches the path
      if (this.currentRoute.path === path) return true
      
      // Special handling for exact matches and sub-routes
      if (path === '/home' && this.currentRoute.path === '/home') return true
      if (path !== '/home' && path !== '/' && this.currentRoute.path.startsWith(path)) return true
      
      return false
    },

    async navigateToPage(path) {
      // Programmatic navigation with loading state
      if (this.currentRoute.path !== path) {
        this.isNavigating = true
        try {
          await this.$router.push(path)
        } catch (error) {
          console.error('Navigation error:', error)
        } finally {
          this.isNavigating = false
        }
      }
      this.closeMobileMenu()
    },

    // Authentication methods
    async checkAuthStatus() {
      // Check for current user in localStorage (our primary authentication method)
      const currentUser = localStorage.getItem('current_user')
      if (currentUser) {
        try {
          this.currentUser = JSON.parse(currentUser)
          this.isAuthenticated = true
          return
        } catch (error) {
          console.error('Error parsing current user from localStorage:', error)
          localStorage.removeItem('current_user')
        }
      }
      
      // Fallback: Check for regular JWT authentication (if using API)
      this.isAuthenticated = authApi.isAuthenticated()
      
      if (this.isAuthenticated) {
        try {
          this.currentUser = await authApi.getCurrentUser()
          if (!this.currentUser) {
            this.isAuthenticated = false
          }
        } catch (error) {
          console.error('Error checking auth status:', error)
          this.isAuthenticated = false
          this.currentUser = null
        }
      } else {
        // Final fallback: Check for legacy admin user in localStorage
        const adminUser = localStorage.getItem('admin_user')
        if (adminUser) {
          try {
            this.currentUser = JSON.parse(adminUser)
            this.isAuthenticated = true
            
            // Migrate to new format
            localStorage.setItem('current_user', JSON.stringify({
              id: this.currentUser.id,
              username: this.currentUser.username,
              email: this.currentUser.email,
              roles: this.currentUser.roles,
              isAdmin: this.currentUser.isAdmin,
              loginTime: new Date().toISOString()
            }))
            localStorage.removeItem('admin_user')
          } catch (error) {
            console.error('Error parsing admin user from localStorage:', error)
            localStorage.removeItem('admin_user')
          }
        }
      }
    },

    async handleLoginSuccess(user) {
      this.currentUser = user
      this.isAuthenticated = true
      this.showLoginModal = false
      
      // Show success toast
      this.toast.success('Welcome back! You have been logged in successfully.')
      
      // Redirect to dashboard after login
      if (this.$route.path === '/') {
        this.$router.push('/dashboard')
      }
    },

    async handleRegisterSuccess(user) {
      this.showRegisterModal = false
      
      // Set user as logged in
      this.currentUser = user
      this.isAuthenticated = true
      
      // Show success toast
      this.toast.success('Account created successfully! Welcome to PortTrack!')
      
      // Redirect to dashboard after registration
      this.$router.push('/dashboard')
    },

    async handleLogout() {
      try {
        // Try to logout from API if authenticated via JWT
        if (authApi.isAuthenticated()) {
          await authApi.logout()
        }
      } catch (error) {
        console.error('API logout error:', error)
      } finally {
        // Clear all authentication data from localStorage
        localStorage.removeItem('current_user')
        localStorage.removeItem('admin_user')
        localStorage.removeItem('auth_token')
        
        this.currentUser = null
        this.isAuthenticated = false
        
        // Show success toast
        this.toast.info('You have been logged out successfully.')
        
        this.$router.push('/')
      }
    },

    // New dropdown methods
    async loadPendingApplicationsCount() {
      try {
        const applications = await roleApplicationApi.getMyApplications()
        this.pendingApplicationsCount = applications.filter(app => app.status === 'Pending').length
      } catch (error) {
        console.error('Error loading applications count:', error)
        // Don't throw error - just set count to 0
        this.pendingApplicationsCount = 0
      }
    },

    openRoleApplication() {
      this.showRoleApplicationModal = true
      this.showUserDropdown = false
    },

    openChangePassword() {
      this.showChangePasswordModal = true
      this.showUserDropdown = false
    },

    openAccountSettings() {
      this.showAccountSettingsModal = true
      this.showUserDropdown = false
    },

    openAdminPanel() {
      this.$router.push('/admin')
      this.showUserDropdown = false
    },

    onApplicationSubmitted() {
      this.loadPendingApplicationsCount()
      // Show success message could be added here
    },

    onPasswordChanged() {
      // Show success message could be added here
    },

    onProfileUpdated() {
      this.checkAuthStatus()
      // Show success message could be added here
    },

    // Close dropdown when clicking outside
    handleClickOutside(event) {
      if (!this.$el.contains(event.target)) {
        this.showUserDropdown = false
      }
    }
  }
}
</script>

<style scoped>
@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-slideDown {
  animation: slideDown 0.2s ease-out;
}
</style>