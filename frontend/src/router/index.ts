import { createRouter, createWebHistory } from 'vue-router'
import Home from '@/components/Home.vue'
import Dashboard from '../components/Dashboard.vue'
import ContainerManagement from '@/components/ContainerManagement.vue'
import PortOperationManagement from '@/components/PortOperationManagement.vue'
import EventStreaming from '@/components/EventStreaming.vue'

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
      component: Dashboard
    },
    {
      path: '/container-management',
      name: 'container-management',
      component: ContainerManagement
    },
    {
      path: '/containers',
      redirect: '/container-management'
    },
    {
      path: '/port-operation-management',
      name: 'port-operation-management',
      component: PortOperationManagement
    },
    {
      path: '/event-streaming',
      name: 'event-streaming',
      component: EventStreaming
    },
  ]
})

export default router
