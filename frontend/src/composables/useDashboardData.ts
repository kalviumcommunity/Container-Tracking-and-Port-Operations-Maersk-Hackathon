import { ref, computed } from 'vue'
import { containerApi, shipApi, portApi, berthApi, berthAssignmentApi } from '../services/api'
import type { Container } from '../services/containerApi'
import type { Ship } from '../services/shipApi'
import type { Port } from '../types/port'
import type { Berth, BerthAssignment } from '../types/berth'

export function useDashboardData() {
  // Reactive data
  const containers = ref<Container[]>([])
  const ships = ref<Ship[]>([])
  const ports = ref<Port[]>([])
  const berths = ref<Berth[]>([])
  const assignments = ref<BerthAssignment[]>([])
  const loading = ref(false)

  // Computed statistics
  const containerStats = computed(() => ({
    total: containers.value.length,
    inTransit: containers.value.filter(c => c.status === 'In Transit').length,
    atPort: containers.value.filter(c => c.status === 'At Port').length,
    available: containers.value.filter(c => c.status === 'Available').length
  }))

  const shipStats = computed(() => ({
    total: ships.value.length,
    docked: ships.value.filter(s => s.status === 'Docked').length,
    atSea: ships.value.filter(s => s.status === 'At Sea').length,
    inbound: ships.value.filter(s => s.status === 'Inbound').length
  }))

  const berthStats = computed(() => ({
    total: berths.value.length,
    available: berths.value.filter(b => b.status === 'Available').length,
    occupied: berths.value.filter(b => b.status === 'Occupied').length,
    maintenance: berths.value.filter(b => b.status === 'Maintenance').length,
    utilizationRate: berths.value.length > 0 
      ? Math.round((berths.value.filter(b => b.status === 'Occupied').length / berths.value.length) * 100)
      : 0
  }))

  const portStats = computed(() => ({
    total: ports.value.length,
    active: ports.value.filter(p => p.status === 'Active').length,
    totalCapacity: ports.value.reduce((sum, p) => sum + p.totalContainerCapacity, 0),
    currentUtilization: ports.value.reduce((sum, p) => sum + (p.currentContainerCount || 0), 0)
  }))

  // Load all dashboard data
  const loadDashboardData = async () => {
    loading.value = true
    try {
      // Load all data in parallel
      const [
        containersResponse,
        shipsResponse,
        portsResponse,
        berthsResponse,
        assignmentsResponse
      ] = await Promise.all([
        containerApi.getAll(),
        shipApi.getAll(),
        portApi.getAll(),
        berthApi.getAll(),
        berthAssignmentApi.getAll()
      ])

      containers.value = containersResponse.data
      ships.value = shipsResponse.data
      ports.value = portsResponse.data
      berths.value = berthsResponse.data
      assignments.value = assignmentsResponse.data

    } catch (error) {
      console.error('Error loading dashboard data:', error)
    } finally {
      loading.value = false
    }
  }

  return {
    // Data
    containers,
    ships,
    ports,
    berths,
    assignments,
    loading,
    
    // Statistics
    containerStats,
    shipStats,
    berthStats,
    portStats,
    
    // Methods
    loadDashboardData
  }
}
