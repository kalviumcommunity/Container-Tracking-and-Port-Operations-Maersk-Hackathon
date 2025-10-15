import { apiClient } from './api'
import type { ContainerStorageFee, ContainerStorageFeeCreateUpdate } from '../types/containerStorageFee'
import type { ApiResponse } from './api'

/**
 * ContainerStorageFee API Service aligned with backend ContainerStorageFeesController
 * Handles container storage billing and fee management
 */
export const containerStorageFeeApi = {
  /**
   * Get all container storage fees
   */
  async getAll(): Promise<{ data: ContainerStorageFee[] }> {
    try {
      const response = await apiClient.get<ApiResponse<ContainerStorageFee[]>>('/container-storage-fees')
      return { data: response.data.data || [] }
    } catch (error) {
      console.error('Error fetching container storage fees:', error)
      return { data: [] }
    }
  },

  /**
   * Get container storage fee by ID
   */
  async getById(id: number): Promise<{ data: ContainerStorageFee | null }> {
    try {
      const response = await apiClient.get<ApiResponse<ContainerStorageFee>>(`/container-storage-fees/${id}`)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error fetching container storage fee ${id}:`, error)
      return { data: null }
    }
  },

  /**
   * Get fees by container ID
   */
  async getByContainer(containerId: string): Promise<{ data: ContainerStorageFee[] }> {
    try {
      const response = await apiClient.get<ApiResponse<ContainerStorageFee[]>>(`/container-storage-fees/container/${containerId}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching fees for container ${containerId}:`, error)
      return { data: [] }
    }
  },

  /**
   * Get fees by port ID
   */
  async getByPort(portId: number): Promise<{ data: ContainerStorageFee[] }> {
    try {
      const response = await apiClient.get<ApiResponse<ContainerStorageFee[]>>(`/container-storage-fees/port/${portId}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching fees for port ${portId}:`, error)
      return { data: [] }
    }
  },

  /**
   * Get fees by status
   */
  async getByStatus(status: string): Promise<{ data: ContainerStorageFee[] }> {
    try {
      const response = await apiClient.get<ApiResponse<ContainerStorageFee[]>>(`/container-storage-fees/status/${status}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching fees by status ${status}:`, error)
      return { data: [] }
    }
  },

  /**
   * Create a new container storage fee
   */
  async create(data: ContainerStorageFeeCreateUpdate): Promise<{ data: ContainerStorageFee | null }> {
    try {
      const response = await apiClient.post<ApiResponse<ContainerStorageFee>>('/container-storage-fees', data)
      return { data: response.data.data || null }
    } catch (error) {
      console.error('Error creating container storage fee:', error)
      throw error
    }
  },

  /**
   * Update an existing container storage fee
   */
  async update(id: number, data: ContainerStorageFeeCreateUpdate): Promise<{ data: ContainerStorageFee | null }> {
    try {
      const response = await apiClient.put<ApiResponse<ContainerStorageFee>>(`/container-storage-fees/${id}`, data)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error updating container storage fee ${id}:`, error)
      throw error
    }
  },

  /**
   * Delete a container storage fee
   */
  async delete(id: number): Promise<boolean> {
    try {
      await apiClient.delete(`/container-storage-fees/${id}`)
      return true
    } catch (error) {
      console.error(`Error deleting container storage fee ${id}:`, error)
      return false
    }
  },

  /**
   * Calculate total storage revenue
   */
  async calculateRevenue(startDate?: string, endDate?: string): Promise<{ data: number }> {
    try {
      const params = new URLSearchParams()
      if (startDate) params.append('startDate', startDate)
      if (endDate) params.append('endDate', endDate)
      
      const response = await apiClient.get<ApiResponse<number>>(`/container-storage-fees/revenue?${params}`)
      return { data: response.data.data || 0 }
    } catch (error) {
      console.error('Error calculating storage revenue:', error)
      return { data: 0 }
    }
  }
}
