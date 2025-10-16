import { apiClient } from './api'
import type { ApiResponse } from './api'

export interface Ship {
  shipId: number
  name: string
  imoNumber?: string
  flag?: string
  type?: string
  capacity?: number
  status: string
  length?: number
  beam?: number
  draft?: number
  grossTonnage?: number
  yearBuilt?: number
  coordinates?: string
  speed?: number
  heading?: number
  nextPort?: string
  estimatedArrival?: string
  currentPortId?: number
  containerCount?: number
  createdAt?: string
  updatedAt?: string
}

export interface ShipCreateUpdate {
  name: string
  imoNumber?: string
  flag?: string
  type?: string
  capacity?: number
  status: string
  length?: number
  beam?: number
  draft?: number
  grossTonnage?: number
  yearBuilt?: number
  coordinates?: string
  speed?: number
  heading?: number
  nextPort?: string
  estimatedArrival?: string
  currentPortId?: number
}

/**
 * Ship API Service
 */
export const shipApi = {
  /**
   * Get all ships
   */
  async getAll(): Promise<{ data: Ship[] }> {
    try {
      const response = await apiClient.get<ApiResponse<Ship[]>>('/ships')
      return { data: response.data.data || [] }
    } catch (error) {
      console.error('Error fetching ships:', error)
      return { data: [] }
    }
  },

  /**
   * Get ship by ID
   */
  async getById(id: number): Promise<{ data: Ship | null }> {
    try {
      const response = await apiClient.get<ApiResponse<Ship>>(`/ships/${id}`)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error fetching ship ${id}:`, error)
      return { data: null }
    }
  },

  /**
   * Get ships by status
   */
  async getByStatus(status: string): Promise<{ data: Ship[] }> {
    try {
      const response = await apiClient.get<ApiResponse<Ship[]>>(`/ships/status/${status}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching ships by status ${status}:`, error)
      return { data: [] }
    }
  },

  /**
   * Create new ship
   */
  async create(shipData: ShipCreateUpdate): Promise<{ data: Ship }> {
    try {
      const response = await apiClient.post<ApiResponse<Ship>>('/ships', shipData)
      return { data: response.data.data }
    } catch (error) {
      console.error('Error creating ship:', error)
      throw error
    }
  },

  /**
   * Update ship
   */
  async update(id: number, updateData: Partial<ShipCreateUpdate>): Promise<{ data: Ship }> {
    try {
      const response = await apiClient.put<ApiResponse<Ship>>(`/ships/${id}`, updateData)
      return { data: response.data.data }
    } catch (error) {
      console.error(`Error updating ship ${id}:`, error)
      throw error
    }
  },

  /**
   * Delete ship
   */
  async delete(id: number): Promise<void> {
    try {
      await apiClient.delete(`/ships/${id}`)
    } catch (error) {
      console.error(`Error deleting ship ${id}:`, error)
      throw error
    }
  }
}
