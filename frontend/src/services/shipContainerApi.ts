import { apiClient } from './api'
import type { ShipContainer, ShipContainerCreateUpdate } from '../types/shipContainer'
import type { ApiResponse } from './api'

/**
 * ShipContainer API Service aligned with backend ShipContainersController
 * Handles cargo management - loading/unloading containers on ships
 */
export const shipContainerApi = {
  /**
   * Get all ship containers
   */
  async getAll(): Promise<{ data: ShipContainer[] }> {
    try {
      const response = await apiClient.get<ApiResponse<ShipContainer[]>>('/ship-containers')
      return { data: response.data.data || [] }
    } catch (error) {
      console.error('Error fetching ship containers:', error)
      return { data: [] }
    }
  },

  /**
   * Get ship container by ID
   */
  async getById(id: number): Promise<{ data: ShipContainer | null }> {
    try {
      const response = await apiClient.get<ApiResponse<ShipContainer>>(`/ship-containers/${id}`)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error fetching ship container ${id}:`, error)
      return { data: null }
    }
  },

  /**
   * Get containers by ship ID
   */
  async getByShip(shipId: number): Promise<{ data: ShipContainer[] }> {
    try {
      const response = await apiClient.get<ApiResponse<ShipContainer[]>>(`/ship-containers/ship/${shipId}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching containers for ship ${shipId}:`, error)
      return { data: [] }
    }
  },

  /**
   * Get ship assignments by container ID
   */
  async getByContainer(containerId: string): Promise<{ data: ShipContainer[] }> {
    try {
      const response = await apiClient.get<ApiResponse<ShipContainer[]>>(`/ship-containers/container/${containerId}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching ships for container ${containerId}:`, error)
      return { data: [] }
    }
  },

  /**
   * Load a container onto a ship
   */
  async create(data: ShipContainerCreateUpdate): Promise<{ data: ShipContainer | null }> {
    try {
      const response = await apiClient.post<ApiResponse<ShipContainer>>('/ship-containers', data)
      return { data: response.data.data || null }
    } catch (error) {
      console.error('Error loading container onto ship:', error)
      throw error
    }
  },

  /**
   * Update ship container assignment
   */
  async update(id: number, data: ShipContainerCreateUpdate): Promise<{ data: ShipContainer | null }> {
    try {
      const response = await apiClient.put<ApiResponse<ShipContainer>>(`/ship-containers/${id}`, data)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error updating ship container ${id}:`, error)
      throw error
    }
  },

  /**
   * Unload a container from a ship (delete assignment)
   */
  async delete(id: number): Promise<boolean> {
    try {
      await apiClient.delete(`/ship-containers/${id}`)
      return true
    } catch (error) {
      console.error(`Error unloading container ${id}:`, error)
      return false
    }
  },

  /**
   * Get ship cargo manifest (all containers on a ship)
   */
  async getCargoManifest(shipId: number): Promise<{ data: ShipContainer[] }> {
    return this.getByShip(shipId)
  },

  /**
   * Get container history (all ships it's been on)
   */
  async getContainerHistory(containerId: string): Promise<{ data: ShipContainer[] }> {
    return this.getByContainer(containerId)
  }
}
