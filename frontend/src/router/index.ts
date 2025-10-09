import { createRouter, createWebHistory } from 'vue-router'
import Home from '@/components/Home.vue'
import Dashboard from '../components/Dashboard.vue'
import ContainerManagement from '@/components/ContainerManagement.vue'
import PortOperationManagement from '@/components/PortOperationManagement.vue'
import EventStreaming from '@/components/EventStreaming.vue'
import AdminDashboard from '@/components/AdminDashboard.vue'
import TestForms from '@/test-forms.vue'
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
      path: '/port-operation-management',
      name: 'port-operation-management',
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
      path: '/test-forms',
      name: 'test-forms',
      component: TestForms
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
  
  if (to.meta.requiresAuth && !isAuthenticated) {
    // Redirect to home if trying to access protected route without auth
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
