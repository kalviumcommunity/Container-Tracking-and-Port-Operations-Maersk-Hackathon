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
              v-for="item in navigationItems"
              :key="item.name"
              :to="item.path"
              class="flex items-center gap-2 px-3 py-2 mx-1 rounded-lg font-medium transition-all duration-200 hover:bg-slate-50"
              :class="[
                $route.path === item.path
                  ? 'text-blue-600 bg-blue-50 border border-blue-200'
                  : 'text-slate-600 hover:text-slate-900'
              ]"
            >
              <component :is="item.icon" :size="18" />
              <span>{{ item.name }}</span>
            </router-link>
          </div>

          <!-- Authentication Buttons -->
          <div class="flex items-center gap-2">
            <div v-if="isAuthenticated" class="flex items-center gap-2">
              <!-- User Info -->
              <div class="flex items-center gap-2 px-3 py-2 bg-slate-50 rounded-lg">
                <User :size="16" class="text-slate-600" />
                <span class="text-sm font-medium text-slate-700">{{ currentUser?.username }}</span>
              </div>
              <!-- Logout Button -->
              <button
                @click="handleLogout"
                class="flex items-center gap-2 px-3 py-2 text-slate-600 hover:text-red-600 hover:bg-red-50 rounded-lg transition-all duration-200"
              >
                <LogOut :size="16" />
                <span class="text-sm font-medium">Logout</span>
              </button>
            </div>
            <div v-else class="flex items-center gap-2">
              <!-- Login Button -->
              <button
                @click="showLoginModal = true"
                class="flex items-center gap-2 px-4 py-2 text-blue-600 hover:text-blue-700 hover:bg-blue-50 rounded-lg transition-all duration-200"
              >
                <LogIn :size="16" />
                <span class="text-sm font-medium">Sign In</span>
              </button>
              <!-- Register Button -->
              <button
                @click="showRegisterModal = true"
                class="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white hover:bg-blue-700 rounded-lg transition-all duration-200"
              >
                <UserPlus :size="16" />
                <span class="text-sm font-medium">Sign Up</span>
              </button>
            </div>
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

      <!-- Mobile Navigation -->
      <div
        v-if="isMobileMenuOpen"
        class="md:hidden border-t border-slate-200 bg-white animate-slideDown"
      >
        <div class="py-4 px-2">
          <!-- Navigation Links (only if authenticated) -->
          <div v-if="isAuthenticated" class="flex flex-col gap-1 mb-4">
            <router-link
              v-for="item in navigationItems"
              :key="item.name"
              :to="item.path"
              @click="closeMobileMenu"
              class="flex items-center gap-3 px-4 py-3 mx-2 rounded-lg font-medium transition-all duration-200 hover:scale-105"
              :class="[
                $route.path === item.path
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
  LogIn,
  LogOut,
  UserPlus
} from 'lucide-vue-next'
import LoginForm from '../forms/LoginForm.vue'
import RegistrationForm from '../forms/RegistrationForm.vue'
import { authApi } from '../services/api'

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
    LoginForm,
    RegistrationForm
  },
  setup() {
    const route = useRoute()
    return { $route: route }
  },
  data() {
    return {
      isMobileMenuOpen: false,
      showLoginModal: false,
      showRegisterModal: false,
      currentUser: null,
      isAuthenticated: false,
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
          name: 'Port Operations',
          path: '/port-operation-management',
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
  async mounted() {
    await this.checkAuthStatus()
  },
  methods: {
    // Mobile menu methods
    toggleMobileMenu() {
      this.isMobileMenuOpen = !this.isMobileMenuOpen
    },

    closeMobileMenu() {
      this.isMobileMenuOpen = false
    },

    // Authentication methods
    async checkAuthStatus() {
      // Check for current user in localStorage (our primary authentication method)
      const currentUser = localStorage.getItem('current_user')
      if (currentUser) {
        try {
          this.currentUser = JSON.parse(currentUser)
          this.isAuthenticated = true
          console.log('User found in localStorage:', this.currentUser)
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
            console.log('Legacy admin user found in localStorage:', this.currentUser)
            
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
      
      // Redirect to dashboard after login
      if (this.$route.path === '/') {
        this.$router.push('/dashboard')
      }
    },

    async handleRegisterSuccess(user) {
      this.showRegisterModal = false
      
      // Check if this is an admin auto-login
      if (user.autoLogin && user.isAdmin) {
        // Auto-login the admin user
        this.currentUser = user
        this.isAuthenticated = true
        
        // Show success message
        console.log('System Admin account created and logged in automatically!')
        
        // Redirect to dashboard to load data
        this.$router.push('/dashboard')
      } else {
        // Regular registration - show login modal
        this.showLoginModal = true
      }
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
        this.$router.push('/')
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