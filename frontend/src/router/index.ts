import { createRouter, createWebHistory } from 'vue-router'
import Home from '@/components/main/Home.vue'
import Dashboard from '@/components/main/Dashboard.vue'
import ContainerManagement from '@/components/main/ContainerManagement.vue'
import BerthOperationsMain from '@/components/main/BerthOperationsMain.vue'
import EventStreaming from '@/components/main/EventStreaming.vue'
import AdminDashboard from '@/components/AdminDashboard.vue'
import Login from '@/views/Login.vue'
import Register from '@/views/Register.vue'
import { authApi } from '../services/api'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/login',
      name: 'login',
      component: Login,
      meta: { requiresGuest: true }
    },
    {
      path: '/register',
      name: 'register',
      component: Register,
      meta: { requiresGuest: true }
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: Dashboard,
      meta: { requiresAuth: true }
    },
    {
      path: '/container-management',
      name: 'container-management',
      component: ContainerManagement,
      meta: { requiresAuth: true }
    },
    {
      path: '/containers',
      redirect: '/container-management'
    },
    {
      path: '/berth-operation-management',
      name: 'berth-operation-management',
      component: BerthOperationsMain,
      meta: { requiresAuth: true }
    },
    {
      path: '/event-streaming',
      name: 'event-streaming',
      component: EventStreaming,
      meta: { requiresAuth: true }
    },
    {
      path: '/admin-dashboard',
      name: 'admin-dashboard',
      component: AdminDashboard,
      meta: { requiresAuth: true, requiresAdmin: true }
    },
    // Add aliases for berth operations
    {
      path: '/berths',
      redirect: '/berth-operation-management'
    },
    {
      path: '/operations',
      redirect: '/berth-operation-management'
    },
    // Catch all route for 404 handling
    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      redirect: '/'
    }
  ]
})

// Navigation guard for authentication
router.beforeEach((to, from, next) => {
  // Check for JWT authentication (primary method)
  const isAuthenticatedJWT = authApi.isAuthenticated()
  
  // Check for current user in localStorage (backup)
  const currentUser = localStorage.getItem('current_user')
  
  // Check for legacy admin user (migration support)
  const adminUser = localStorage.getItem('admin_user')
  
  const isAuthenticated = isAuthenticatedJWT || !!currentUser || !!adminUser
  
  // Redirect authenticated users from home page to dashboard
  if (to.name === 'home' && isAuthenticated) {
    next('/dashboard')
    return
  }
  
  // Redirect authenticated users from login/register pages to dashboard
  if (to.meta.requiresGuest && isAuthenticated) {
    next('/dashboard')
    return
  }
  
  // Redirect unauthenticated users from protected routes to login
  if (to.meta.requiresAuth && !isAuthenticated) {
    next('/login')
    return
  }
  
  if (to.meta.requiresAdmin) {
    // Check if user has admin role
    let userRoles = []
    
    if (currentUser) {
      try {
        const user = JSON.parse(currentUser)
        userRoles = user.roles || []
      } catch (e) {
        console.error('Error parsing user:', e)
      }
    }
    
    const isAdmin = userRoles.includes('Admin') || userRoles.includes('SuperAdmin') || !!adminUser
    
    if (!isAdmin) {
      // Redirect non-admin users to dashboard
      next('/dashboard')
    } else {
      next()
    }
  } else {
    next()
  }
})

export default router