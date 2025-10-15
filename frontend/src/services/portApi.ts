import { apiClient } from './api'
import type { Port, PortCreateUpdate, PortDetail } from '../types/port'
import type { ApiResponse } from './api'

/**
 * Enhanced Port API Service aligned with backend PortsController
 */
export const portApi = {
  /**
   * Get all ports
   */
  async getAll(): Promise<{ data: Port[] }> {
    try {
      const response = await apiClient.get<ApiResponse<Port[]>>('/ports')
      return { data: response.data.data || [] }
    } catch (error) {
      console.error('Error fetching ports:', error)
      return { data: [] }
    }
  },

  /**
   * Get port by ID
   */
  async getById(id: number): Promise<{ data: Port | null }> {
    try {
      const response = await apiClient.get<ApiResponse<Port>>(`/ports/${id}`)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error fetching port ${id}:`, error)
      return { data: null }
    }
  },

  /**
   * Get detailed port information with berths
   */
  async getDetails(id: number): Promise<{ data: PortDetail | null }> {
    try {
      const response = await apiClient.get<ApiResponse<PortDetail>>(`/ports/${id}/details`)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error fetching port details ${id}:`, error)
      return { data: null }
    }
  },

  /**
   * Get ports by location
   */
  async getByLocation(location: string): Promise<{ data: Port[] }> {
    try {
      const response = await apiClient.get<ApiResponse<Port[]>>(`/ports/location/${encodeURIComponent(location)}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching ports by location ${location}:`, error)
      return { data: [] }
    }
  },

  /**
   * Create new port
   */
  async create(portData: PortCreateUpdate): Promise<{ data: Port }> {
    try {
      const response = await apiClient.post<ApiResponse<Port>>('/ports', portData)
      return { data: response.data.data }
    } catch (error) {
      console.error('Error creating port:', error)
      throw error
    }
  },

  /**
   * Update port
   */
  async update(id: number, updateData: PortCreateUpdate): Promise<{ data: Port }> {
    try {
      const response = await apiClient.put<ApiResponse<Port>>(`/ports/${id}`, updateData)
      return { data: response.data.data }
    } catch (error) {
      console.error(`Error updating port ${id}:`, error)
      throw error
    }
  },

  /**
   * Delete port
   */
  async delete(id: number): Promise<void> {
    try {
      await apiClient.delete(`/ports/${id}`)
    } catch (error) {
      console.error(`Error deleting port ${id}:`, error)
      throw error
    }
  }
}
