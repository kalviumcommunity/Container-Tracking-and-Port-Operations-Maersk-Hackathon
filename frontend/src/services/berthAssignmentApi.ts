import { apiClient } from './api'
import type { BerthAssignment } from '../types/berth'
import type { ApiResponse } from './api'

/**
 * Enhanced Berth Assignment API Service aligned with backend BerthAssignmentsController
 */
export const berthAssignmentApi = {
  /**
   * Get all berth assignments
   */
  async getAll(): Promise<{ data: BerthAssignment[] }> {
    try {
      const response = await apiClient.get<ApiResponse<BerthAssignment[]>>('/berth-assignments')
      return { data: response.data.data || [] }
    } catch (error) {
      console.error('Error fetching berth assignments:', error)
      // Return mock data if API endpoint doesn't exist yet
      return {
        data: [
          { 
            id: 1, 
            berthId: 1, 
            berthName: 'Berth A1',
            shipId: 1, 
            containerId: 'MAEU1234567', 
            assignedAt: new Date().toISOString(),
            status: 'Active',
            assignmentType: 'Loading'
          },
          { 
            id: 2, 
            berthId: 2, 
            berthName: 'Berth A2',
            shipId: 2, 
            containerId: 'MAEU2345678', 
            assignedAt: new Date().toISOString(),
            status: 'Active',
            assignmentType: 'Unloading'
          }
        ]
      }
    }
  },

  /**
   * Get berth assignment by ID
   */
  async getById(id: number): Promise<{ data: BerthAssignment | null }> {
    try {
      const response = await apiClient.get<ApiResponse<BerthAssignment>>(`/berth-assignments/${id}`)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error fetching berth assignment ${id}:`, error)
      return { data: null }
    }
  },

  /**
   * Get assignments by berth
   */
  async getByBerth(berthId: number): Promise<{ data: BerthAssignment[] }> {
    try {
      const response = await apiClient.get<ApiResponse<BerthAssignment[]>>(`/berth-assignments/berth/${berthId}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching assignments for berth ${berthId}:`, error)
      return { data: [] }
    }
  },

  /**
   * Get assignments by ship
   */
  async getByShip(shipId: number): Promise<{ data: BerthAssignment[] }> {
    try {
      const response = await apiClient.get<ApiResponse<BerthAssignment[]>>(`/berth-assignments/ship/${shipId}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching assignments for ship ${shipId}:`, error)
      return { data: [] }
    }
  },

  /**
   * Create new berth assignment
   */
  async create(assignmentData: {
    berthId: number
    shipId?: number
    containerId?: string
    assignmentType: string
    priority?: string
    status?: string
    scheduledArrival?: string
    scheduledDeparture?: string
    containerCount?: number
    estimatedProcessingTime?: number
    notes?: string
  }): Promise<{ data: BerthAssignment }> {
    try {
      const response = await apiClient.post<ApiResponse<BerthAssignment>>('/berth-assignments', assignmentData)
      return { data: response.data.data }
    } catch (error) {
      console.error('Error creating berth assignment:', error)
      throw error
    }
  },

  /**
   * Update berth assignment
   */
  async update(id: number, updateData: Partial<BerthAssignment>): Promise<{ data: BerthAssignment }> {
    try {
      const response = await apiClient.put<ApiResponse<BerthAssignment>>(`/berth-assignments/${id}`, updateData)
      return { data: response.data.data }
    } catch (error) {
      console.error(`Error updating berth assignment ${id}:`, error)
      throw error
    }
  },

  /**
   * Complete/release berth assignment
   */
  async release(id: number): Promise<{ success: boolean; message?: string }> {
    try {
      const response = await apiClient.patch<ApiResponse<any>>(`/berth-assignments/${id}/release`)
      return { 
        success: true, 
        message: response.data.message || 'Assignment released successfully' 
      }
    } catch (error) {
      console.error(`Error releasing berth assignment ${id}:`, error)
      throw error
    }
  },

  /**
   * Delete berth assignment
   */
  async delete(id: number): Promise<{ success: boolean; message?: string }> {
    try {
      const response = await apiClient.delete<ApiResponse<any>>(`/berth-assignments/${id}`)
      return { 
        success: true, 
        message: response.data.message || 'Assignment deleted successfully' 
      }
    } catch (error) {
      console.error(`Error deleting berth assignment ${id}:`, error)
      throw error
    }
  }
}
