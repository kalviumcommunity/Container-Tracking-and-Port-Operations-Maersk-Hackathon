import { apiClient } from './api'
import type { ApiResponse } from './api'

export interface CrewMember {
  id: number
  name: string
  role: string
  department: string
  portId?: number
  isActive: boolean
  skills?: string[]
  contactInfo?: string
}

export interface CrewCreateUpdate {
  name: string
  role: string
  department: string
  portId?: number
  skills?: string[]
  contactInfo?: string
}

/**
 * Crew API Service - Mock implementation for crew management
 */
export const crewApi = {
  /**
   * Get all crew members
   */
  async getAll(): Promise<{ data: CrewMember[] }> {
    try {
      const response = await apiClient.get<ApiResponse<CrewMember[]>>('/crew')
      return { data: response.data.data || [] }
    } catch (error) {
      console.error('Error fetching crew members:', error)
      // Return empty data - let components show 0 state instead of mock data
      return { data: [] }
    }
  },

  /**
   * Get crew member by ID
   */
  async getById(id: number): Promise<{ data: CrewMember | null }> {
    try {
      const response = await apiClient.get<ApiResponse<CrewMember>>(`/crew/${id}`)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error fetching crew member ${id}:`, error)
      return { data: null }
    }
  },

  /**
   * Get crew by department
   */
  async getByDepartment(department: string): Promise<{ data: CrewMember[] }> {
    try {
      const response = await apiClient.get<ApiResponse<CrewMember[]>>(`/crew/department/${department}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching crew by department ${department}:`, error)
      return { data: [] }
    }
  },

  /**
   * Get crew by port
   */
  async getByPort(portId: number): Promise<{ data: CrewMember[] }> {
    try {
      const response = await apiClient.get<ApiResponse<CrewMember[]>>(`/crew/port/${portId}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching crew by port ${portId}:`, error)
      return { data: [] }
    }
  },

  /**
   * Create new crew member
   */
  async create(crewData: CrewCreateUpdate): Promise<{ data: CrewMember }> {
    try {
      const response = await apiClient.post<ApiResponse<CrewMember>>('/crew', crewData)
      return { data: response.data.data }
    } catch (error) {
      console.error('Error creating crew member:', error)
      throw error
    }
  },

  /**
   * Update crew member
   */
  async update(id: number, updateData: Partial<CrewCreateUpdate>): Promise<{ data: CrewMember }> {
    try {
      const response = await apiClient.put<ApiResponse<CrewMember>>(`/crew/${id}`, updateData)
      return { data: response.data.data }
    } catch (error) {
      console.error(`Error updating crew member ${id}:`, error)
      throw error
    }
  },

  /**
   * Delete crew member
   */
  async delete(id: number): Promise<void> {
    try {
      await apiClient.delete(`/crew/${id}`)
    } catch (error) {
      console.error(`Error deleting crew member ${id}:`, error)
      throw error
    }
  }
}
