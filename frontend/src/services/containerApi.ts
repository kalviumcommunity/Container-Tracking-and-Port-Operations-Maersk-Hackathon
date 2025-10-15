import { apiClient } from './api'
import type { ApiResponse } from './api'

export interface Container {
  containerId: string
  cargoType: string
  cargoDescription: string
  type: string
  status: string
  condition: string
  currentLocation: string
  destination: string
  weight: number
  maxWeight?: number
  size: string
  temperature?: number
  coordinates: string
  estimatedArrival?: string
  shipId?: number
  createdAt: string
  updatedAt: string
}

export interface ContainerCreateUpdate {
  containerId: string
  cargoType: string
  cargoDescription: string
  type: string
  status: string
  condition?: string
  currentLocation: string
  destination: string
  weight: number
  maxWeight?: number
  size: string
  temperature?: number
  coordinates?: string
  shipId?: number
}

/**
 * Container API Service
 */
export const containerApi = {
  /**
   * Get all containers
   */
  async getAll(): Promise<{ data: Container[] }> {
    try {
      const response = await apiClient.get<ApiResponse<Container[]>>('/containers')
      return { data: response.data.data || [] }
    } catch (error) {
      console.error('Error fetching containers:', error)
      return { 
        data: [
          {
            containerId: 'MAEU1234567',
            cargoType: 'Electronics',
            cargoDescription: 'Consumer electronics shipment',
            type: 'Dry',
            status: 'In Transit',
            condition: 'Good',
            currentLocation: 'Port of Hamburg',
            destination: 'Port of Copenhagen',
            weight: 25000,
            size: '40ft',
            coordinates: '53.5511,9.9937',
            estimatedArrival: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString(),
            shipId: 1,
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
          }
        ]
      }
    }
  },

  /**
   * Get container by ID
   */
  async getById(id: string): Promise<{ data: Container | null }> {
    try {
      const response = await apiClient.get<ApiResponse<Container>>(`/containers/${id}`)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error fetching container ${id}:`, error)
      return { data: null }
    }
  },

  /**
   * Get containers by status
   */
  async getByStatus(status: string): Promise<{ data: Container[] }> {
    try {
      const response = await apiClient.get<ApiResponse<Container[]>>(`/containers/status/${status}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching containers by status ${status}:`, error)
      return { data: [] }
    }
  },

  /**
   * Get containers by location
   */
  async getByLocation(location: string): Promise<{ data: Container[] }> {
    try {
      const response = await apiClient.get<ApiResponse<Container[]>>(`/containers/location/${encodeURIComponent(location)}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching containers by location ${location}:`, error)
      return { data: [] }
    }
  },

  /**
   * Create new container
   */
  async create(containerData: ContainerCreateUpdate): Promise<{ data: Container }> {
    try {
      const response = await apiClient.post<ApiResponse<Container>>('/containers', containerData)
      return { data: response.data.data }
    } catch (error) {
      console.error('Error creating container:', error)
      throw error
    }
  },

  /**
   * Update container
   */
  async update(id: string, updateData: Partial<ContainerCreateUpdate>): Promise<{ data: Container }> {
    try {
      const response = await apiClient.put<ApiResponse<Container>>(`/containers/${id}`, updateData)
      return { data: response.data.data }
    } catch (error) {
      console.error(`Error updating container ${id}:`, error)
      throw error
    }
  },

  /**
   * Delete container
   */
  async delete(id: string): Promise<void> {
    try {
      await apiClient.delete(`/containers/${id}`)
    } catch (error) {
      console.error(`Error deleting container ${id}:`, error)
      throw error
    }
  }
}
