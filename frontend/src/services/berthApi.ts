import { apiClient } from './api'
import type { Berth, BerthCreateUpdate, BerthAssignment } from '../types/berth'
import type { ApiResponse } from './api'

/**
 * Enhanced Berth API Service aligned with backend BerthsController
 */
export const berthApi = {
  /**
   * Get all berths
   */
  async getAll(): Promise<{ data: Berth[] }> {
    try {
      const response = await apiClient.get<ApiResponse<Berth[]>>('/berths')
      return { data: response.data.data || [] }
    } catch (error) {
      console.error('Error fetching berths:', error)
      return { data: [] }
    }
  },

  /**
   * Get berth by ID
   */
  async getById(id: number): Promise<{ data: Berth | null }> {
    try {
      const response = await apiClient.get<ApiResponse<Berth>>(`/berths/${id}`)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error fetching berth ${id}:`, error)
      return { data: null }
    }
  },

  /**
   * Get detailed berth information with assignments
   */
  async getDetails(id: number): Promise<{ data: Berth & { berthAssignments: BerthAssignment[] } | null }> {
    try {
      const response = await apiClient.get<ApiResponse<any>>(`/berths/${id}/details`)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error fetching berth details ${id}:`, error)
      return { data: null }
    }
  },

  /**
   * Get berths by port
   */
  async getByPort(portId: number): Promise<{ data: Berth[] }> {
    try {
      const response = await apiClient.get<ApiResponse<Berth[]>>(`/berths/port/${portId}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching berths for port ${portId}:`, error)
      return { data: [] }
    }
  },

  /**
   * Get berths by status
   */
  async getByStatus(status: string): Promise<{ data: Berth[] }> {
    try {
      const response = await apiClient.get<ApiResponse<Berth[]>>(`/berths/status/${status}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching berths by status ${status}:`, error)
      return { data: [] }
    }
  },

  /**
   * Create new berth
   */
  async create(berthData: BerthCreateUpdate): Promise<{ data: Berth }> {
    try {
      const response = await apiClient.post<ApiResponse<Berth>>('/berths', berthData)
      return { data: response.data.data }
    } catch (error) {
      console.error('Error creating berth:', error)
      throw error
    }
  },

  /**
   * Update berth
   */
  async update(id: number, updateData: BerthCreateUpdate): Promise<{ data: Berth }> {
    try {
      const response = await apiClient.put<ApiResponse<Berth>>(`/berths/${id}`, updateData)
      return { data: response.data.data }
    } catch (error) {
      console.error(`Error updating berth ${id}:`, error)
      throw error
    }
  },

  /**
   * Delete berth
   */
  async delete(id: number): Promise<void> {
    try {
      await apiClient.delete(`/berths/${id}`)
    } catch (error) {
      console.error(`Error deleting berth ${id}:`, error)
      throw error
    }
  }
}
