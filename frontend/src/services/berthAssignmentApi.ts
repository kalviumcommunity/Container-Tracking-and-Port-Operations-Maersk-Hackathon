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
      const response = await apiClient.get<ApiResponse<BerthAssignment[]>>('/berthassignments')
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
      const response = await apiClient.get<ApiResponse<BerthAssignment>>(`/berthassignments/${id}`)
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
      const response = await apiClient.get<ApiResponse<BerthAssignment[]>>(`/berthassignments/berth/${berthId}`)
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
      const response = await apiClient.get<ApiResponse<BerthAssignment[]>>(`/berthassignments/ship/${shipId}`)
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
    priority?: number
    status?: string
    scheduledArrival?: string
    scheduledDeparture?: string
    containerCount?: number
    estimatedProcessingTime?: number
    notes?: string
  }): Promise<{ data: BerthAssignment }> {
    try {
      const response = await apiClient.post<ApiResponse<BerthAssignment>>('/berthassignments', assignmentData)
      return { data: response.data.data }
    } catch (error) {
      console.error('Error creating berth assignment:', error)
      throw error
    }
  },

  /**
   * Update berth assignment
   */
  async update(id: number, updateData: any): Promise<{ data: BerthAssignment }> {
    try {
      const response = await apiClient.put<ApiResponse<BerthAssignment>>(`/berthassignments/${id}`, updateData)
      return { data: response.data.data }
    } catch (error) {
      console.error(`Error updating berth assignment ${id}:`, error)
      throw error
    }
  },

  /**
   * Release container from berth
   */
  async release(id: number): Promise<{ data: BerthAssignment }> {
    try {
      const response = await apiClient.put<ApiResponse<BerthAssignment>>(`/berthassignments/${id}/release`)
      return { data: response.data.data }
    } catch (error) {
      console.error(`Error releasing assignment ${id}:`, error)
      throw error
    }
  },

  /**
   * Get assignments by date range
   */
  async getByDateRange(startDate: string, endDate: string): Promise<{ data: BerthAssignment[] }> {
    try {
      const response = await apiClient.get<ApiResponse<BerthAssignment[]>>(`/berthassignments/daterange?startDate=${startDate}&endDate=${endDate}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error('Error fetching assignments by date range:', error)
      return { data: [] }
    }
  },

  /**
   * Get active assignments
   */
  async getActive(): Promise<{ data: BerthAssignment[] }> {
    try {
      const response = await apiClient.get<ApiResponse<BerthAssignment[]>>('/berthassignments/active')
      return { data: response.data.data || [] }
    } catch (error) {
      console.error('Error fetching active assignments:', error)
      return { data: [] }
    }
  },

  /**
   * Delete berth assignment
   */
  async delete(id: number): Promise<void> {
    try {
      await apiClient.delete(`/berthassignments/${id}`)
    } catch (error) {
      console.error(`Error deleting berth assignment ${id}:`, error)
      throw error
    }
  }
}