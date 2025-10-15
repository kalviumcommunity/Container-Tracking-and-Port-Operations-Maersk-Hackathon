import { apiClient } from './api'

export interface ContainerMovement {
  movementId: number
  containerId: string
  movementType: string
  fromLocation: string
  toLocation: string
  movementTimestamp: string
  status: string
  coordinates?: string
}

export const containerMovementApi = {
  async getByContainer(containerId: string): Promise<{ data: ContainerMovement[] }> {
    try {
      const response = await apiClient.get(`/container-movements/container/${containerId}`)
      return { data: response.data.data || response.data || [] }
    } catch (error) {
      console.error('Error fetching container movements:', error)
      return { data: [] }
    }
  },

  async create(movementData: Partial<ContainerMovement>): Promise<ContainerMovement> {
    const response = await apiClient.post('/container-movements', movementData)
    return response.data.data || response.data
  }
}
