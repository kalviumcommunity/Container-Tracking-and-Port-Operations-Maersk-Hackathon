import { apiClient } from './api'
import type { ShipRoute, ShipRouteCreateUpdate } from '../types/shipRoute'
import type { ApiResponse } from './api'

/**
 * ShipRoute API Service aligned with backend ShipRoutesController
 * Handles ship route planning, tracking, and management
 */
export const shipRouteApi = {
  /**
   * Get all ship routes
   */
  async getAll(): Promise<{ data: ShipRoute[] }> {
    try {
      const response = await apiClient.get<ApiResponse<ShipRoute[]>>('/ship-routes')
      return { data: response.data.data || [] }
    } catch (error) {
      console.error('Error fetching ship routes:', error)
      return { data: [] }
    }
  },

  /**
   * Get ship route by ID
   */
  async getById(id: number): Promise<{ data: ShipRoute | null }> {
    try {
      const response = await apiClient.get<ApiResponse<ShipRoute>>(`/ship-routes/${id}`)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error fetching ship route ${id}:`, error)
      return { data: null }
    }
  },

  /**
   * Get routes by ship ID
   */
  async getByShip(shipId: number): Promise<{ data: ShipRoute[] }> {
    try {
      const response = await apiClient.get<ApiResponse<ShipRoute[]>>(`/ship-routes/ship/${shipId}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching routes for ship ${shipId}:`, error)
      return { data: [] }
    }
  },

  /**
   * Get routes by port ID (origin or destination)
   */
  async getByPort(portId: number): Promise<{ data: ShipRoute[] }> {
    try {
      const response = await apiClient.get<ApiResponse<ShipRoute[]>>(`/ship-routes/port/${portId}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching routes for port ${portId}:`, error)
      return { data: [] }
    }
  },

  /**
   * Get routes by status
   */
  async getByStatus(status: string): Promise<{ data: ShipRoute[] }> {
    try {
      const response = await apiClient.get<ApiResponse<ShipRoute[]>>(`/ship-routes/status/${status}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching routes by status ${status}:`, error)
      return { data: [] }
    }
  },

  /**
   * Get active routes (In Transit)
   */
  async getActive(): Promise<{ data: ShipRoute[] }> {
    try {
      const response = await apiClient.get<ApiResponse<ShipRoute[]>>('/ship-routes/active')
      return { data: response.data.data || [] }
    } catch (error) {
      console.error('Error fetching active routes:', error)
      return { data: [] }
    }
  },

  /**
   * Create a new ship route
   */
  async create(data: ShipRouteCreateUpdate): Promise<{ data: ShipRoute | null }> {
    try {
      const response = await apiClient.post<ApiResponse<ShipRoute>>('/ship-routes', data)
      return { data: response.data.data || null }
    } catch (error) {
      console.error('Error creating ship route:', error)
      throw error
    }
  },

  /**
   * Update an existing ship route
   */
  async update(id: number, data: ShipRouteCreateUpdate): Promise<{ data: ShipRoute | null }> {
    try {
      const response = await apiClient.put<ApiResponse<ShipRoute>>(`/ship-routes/${id}`, data)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error updating ship route ${id}:`, error)
      throw error
    }
  },

  /**
   * Delete a ship route
   */
  async delete(id: number): Promise<boolean> {
    try {
      await apiClient.delete(`/ship-routes/${id}`)
      return true
    } catch (error) {
      console.error(`Error deleting ship route ${id}:`, error)
      return false
    }
  },

  /**
   * Update route status
   */
  async updateStatus(id: number, status: string): Promise<{ data: ShipRoute | null }> {
    try {
      const response = await apiClient.patch<ApiResponse<ShipRoute>>(`/ship-routes/${id}/status`, { status })
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error updating route status for ${id}:`, error)
      throw error
    }
  },

  /**
   * Record actual departure
   */
  async recordDeparture(id: number, actualDeparture: string): Promise<{ data: ShipRoute | null }> {
    try {
      const response = await apiClient.patch<ApiResponse<ShipRoute>>(`/ship-routes/${id}/departure`, { actualDeparture })
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error recording departure for route ${id}:`, error)
      throw error
    }
  },

  /**
   * Record actual arrival
   */
  async recordArrival(id: number, actualArrival: string): Promise<{ data: ShipRoute | null }> {
    try {
      const response = await apiClient.patch<ApiResponse<ShipRoute>>(`/ship-routes/${id}/arrival`, { actualArrival })
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error recording arrival for route ${id}:`, error)
      throw error
    }
  }
}
