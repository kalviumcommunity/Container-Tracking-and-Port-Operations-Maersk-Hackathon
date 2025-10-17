import { createRouter, createWebHistory } from 'vue-router'
import Home from '@/components/main/Home.vue'
import Dashboard from '@/components/main/Dashboard.vue'
import ContainerManagement from '@/components/main/ContainerManagement.vue'
import PortOperationManagement from '@/components/main/PortOperationManagement.vue'
import EventStreaming from '@/components/main/EventStreaming.vue'
import AdminDashboard from '@/components/AdminDashboard.vue'
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
      component: PortOperationManagement,
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
  
  // Redirect authenticated users from landing page to dashboard
  if (to.name === 'landing' && isAuthenticated) {
    next('/dashboard')
    return
  }
  
  if (to.meta.requiresAuth && !isAuthenticated) {
    // Redirect to landing page if trying to access protected route without auth
    next('/')
  } else if (to.meta.requiresAdmin) {
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
