import { createRouter, createWebHistory } from 'vue-router'
import Home from '@/components/Home.vue'
import Dashboard from '../components/Dashboard.vue'
import ContainerManagement from '@/components/ContainerManagement.vue'
import PortOperationManagement from '@/components/PortOperationManagement.vue'
import EventStreaming from '@/components/EventStreaming.vue'
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
  ]
})

// Navigation guard for authentication
router.beforeEach((to, from, next) => {
  // Check for current user in localStorage (primary authentication method)
  const currentUser = localStorage.getItem('current_user')
  
  // Check for JWT authentication (fallback)
  const isAuthenticatedJWT = authApi.isAuthenticated()
  
  // Check for legacy admin user (migration support)
  const adminUser = localStorage.getItem('admin_user')
  
  const isAuthenticated = !!currentUser || isAuthenticatedJWT || !!adminUser
  
  if (to.meta.requiresAuth && !isAuthenticated) {
    // Redirect to home if trying to access protected route without auth
    next('/')
  } else {
    next()
  }
})

export default router
