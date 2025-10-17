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

export interface ContainerFilterOptions {
  page?: number
  pageSize?: number
  sortBy?: string
  sortDirection?: 'asc' | 'desc'
  status?: string
  type?: string
  cargoType?: string
  currentLocation?: string
  destination?: string
  shipId?: number
  searchTerm?: string
}

export interface PaginatedContainerResponse {
  data: Container[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

/**
 * Container API Service
 */
export const containerApi = {
  /**
   * Get all containers with optional filtering and sorting
   */
  async getAll(options?: ContainerFilterOptions): Promise<{ data: Container[] }> {
    try {
      // Build query parameters
      const params = new URLSearchParams()
      
      if (options?.page) params.append('page', options.page.toString())
      if (options?.pageSize) params.append('pageSize', options.pageSize.toString())
      if (options?.sortBy) params.append('sortBy', options.sortBy)
      if (options?.sortDirection) params.append('sortDirection', options.sortDirection)
      if (options?.status) params.append('status', options.status)
      if (options?.type) params.append('type', options.type)
      if (options?.cargoType) params.append('cargoType', options.cargoType)
      if (options?.currentLocation) params.append('currentLocation', options.currentLocation)
      if (options?.destination) params.append('destination', options.destination)
      if (options?.shipId) params.append('shipId', options.shipId.toString())
      if (options?.searchTerm) params.append('searchTerm', options.searchTerm)

      const url = params.toString() ? `/containers?${params.toString()}` : '/containers'
      console.log('Container API: Fetching from', url, 'with options:', options)
      
      const response = await apiClient.get<ApiResponse<PaginatedContainerResponse>>(url)
      console.log('Container API: Response received:', response.data)
      
      // Handle both paginated and direct response formats
      if (response.data.data?.data) {
        console.log('Container API: Using paginated format, found', response.data.data.data.length, 'containers')
        return { data: response.data.data.data }
      } else if (Array.isArray(response.data.data)) {
        console.log('Container API: Using direct format, found', response.data.data.length, 'containers')
        return { data: response.data.data }
      } else {
        console.warn('Container API: Unexpected response format:', response.data)
        return { data: [] }
      }
    } catch (error) {
      console.error('Container API: Error fetching containers:', error)
      console.log('Container API: Falling back to mock data')
      // Return enhanced mock data for development
      return { 
        data: [
          {
            containerId: 'MAEU2024001',
            cargoType: 'Electronics',
            cargoDescription: 'Consumer electronics shipment',
            type: 'Dry',
            status: 'Arrived',
            condition: 'Good',
            currentLocation: 'Port Terminal A',
            destination: 'Port of Rotterdam',
            weight: 25000,
            maxWeight: 30000,
            size: '40ft',
            coordinates: '53.5511,9.9937',
            estimatedArrival: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString(),
            shipId: 1,
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
          },
          {
            containerId: 'COSCO2024001',
            cargoType: 'Automotive',
            cargoDescription: 'Vehicle parts and accessories',
            type: 'Refrigerated',
            status: 'Loading',
            condition: 'Good',
            currentLocation: 'Port Terminal B',
            destination: 'Port of Hamburg',
            weight: 28000,
            maxWeight: 32000,
            size: '40ft',
            temperature: -18,
            coordinates: '53.5411,9.9837',
            estimatedArrival: new Date(Date.now() + 48 * 60 * 60 * 1000).toISOString(),
            shipId: 2,
            createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000).toISOString(),
            updatedAt: new Date(Date.now() - 1 * 60 * 60 * 1000).toISOString()
          },
          {
            containerId: 'HAPAG2024001',
            cargoType: 'Chemicals',
            cargoDescription: 'Industrial chemicals and solvents',
            type: 'Tank',
            status: 'Inspection',
            condition: 'Good',
            currentLocation: 'Inspection Zone',
            destination: 'Port of Antwerp',
            weight: 30000,
            maxWeight: 35000,
            size: '20ft',
            coordinates: '53.5311,9.9737',
            estimatedArrival: new Date(Date.now() + 72 * 60 * 60 * 1000).toISOString(),
            shipId: 1,
            createdAt: new Date(Date.now() - 4 * 60 * 60 * 1000).toISOString(),
            updatedAt: new Date(Date.now() - 3 * 60 * 60 * 1000).toISOString()
          },
          {
            containerId: 'MSC2024001',
            cargoType: 'Textiles',
            cargoDescription: 'Cotton and synthetic fabrics',
            type: 'OpenTop',
            status: 'Departed',
            condition: 'Good',
            currentLocation: 'En Route',
            destination: 'Port of Le Havre',
            weight: 22000,
            maxWeight: 28000,
            size: '40ft',
            coordinates: '53.5211,9.9637',
            estimatedArrival: new Date(Date.now() + 96 * 60 * 60 * 1000).toISOString(),
            shipId: 3,
            createdAt: new Date(Date.now() - 6 * 60 * 60 * 1000).toISOString(),
            updatedAt: new Date(Date.now() - 5 * 60 * 60 * 1000).toISOString()
          }
        ]
      }
    }
  },

  /**
   * Get containers with filtering and pagination (for dashboard use)
   */
  async getFiltered(options?: ContainerFilterOptions): Promise<{ data: PaginatedContainerResponse }> {
    try {
      // Build query parameters
      const params = new URLSearchParams()
      
      if (options?.page) params.append('page', options.page.toString())
      if (options?.pageSize) params.append('pageSize', options.pageSize.toString())
      if (options?.sortBy) params.append('sortBy', options.sortBy)
      if (options?.sortDirection) params.append('sortDirection', options.sortDirection)
      if (options?.status) params.append('status', options.status)
      if (options?.type) params.append('type', options.type)
      if (options?.cargoType) params.append('cargoType', options.cargoType)
      if (options?.currentLocation) params.append('currentLocation', options.currentLocation)
      if (options?.destination) params.append('destination', options.destination)
      if (options?.shipId) params.append('shipId', options.shipId.toString())
      if (options?.searchTerm) params.append('searchTerm', options.searchTerm)

      const url = params.toString() ? `/containers?${params.toString()}` : '/containers'
      const response = await apiClient.get<ApiResponse<PaginatedContainerResponse>>(url)
      
      return { data: response.data.data || { data: [], totalCount: 0, page: 1, pageSize: 10, totalPages: 0, hasNextPage: false, hasPreviousPage: false } }
    } catch (error) {
      console.error('Error fetching filtered containers:', error)
      throw error
    }
  },

  /**
   * Get latest containers (sorted by creation date descending)
   */
  async getLatest(limit: number = 50): Promise<{ data: Container[] }> {
    return this.getAll({
      pageSize: limit,
      sortBy: 'CreatedAt',
      sortDirection: 'desc'
    })
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
