/**
 * Event API Service
 * Handles all HTTP requests for event operations
 * Connects frontend to EventsController backend endpoints
 */

import { apiClient, type ApiResponse } from './api'
import type {
  Event,
  EventCreate,
  EventUpdate,
  EventFilter,
  EventStats,
  EventPagedResponse,
  EventAcknowledgeRequest,
  EventResolveRequest
} from '../types/event'

const BASE_URL = '/events'  // Fixed: removed '/api' prefix since baseURL already includes it

/**
 * Event API service for managing port operation events
 */
export const eventApi = {
  /**
   * Get all events with optional filtering and pagination
   * @param filter - Filter criteria including pagination, sorting, search
   * @returns Paginated list of events wrapped in ApiResponse
   */
  getAll: async (filter?: EventFilter): Promise<ApiResponse<EventPagedResponse>> => {
    try {
      const params = new URLSearchParams()
      
      if (filter) {
        // Search and filtering
        if (filter.searchTerm) params.append('searchTerm', filter.searchTerm)
        if (filter.eventType) params.append('eventType', filter.eventType)
        if (filter.category) params.append('category', filter.category)
        if (filter.priority) params.append('priority', filter.priority)
        if (filter.severity) params.append('severity', filter.severity)
        if (filter.status) params.append('status', filter.status)
        if (filter.isResolved !== undefined) params.append('isResolved', String(filter.isResolved))
        if (filter.requiresAction !== undefined) params.append('requiresAction', String(filter.requiresAction))
        if (filter.isRead !== undefined) params.append('isRead', String(filter.isRead))
        
        // Related entities
        if (filter.containerId) params.append('containerId', String(filter.containerId))
        if (filter.shipId) params.append('shipId', String(filter.shipId))
        if (filter.berthId) params.append('berthId', String(filter.berthId))
        if (filter.portId) params.append('portId', String(filter.portId))
        if (filter.assignedToUserId) params.append('assignedToUserId', String(filter.assignedToUserId))
        
        // Date range
        if (filter.startDate) params.append('startDate', filter.startDate)
        if (filter.endDate) params.append('endDate', filter.endDate)
        if (filter.eventTimeFrom) params.append('eventTimeFrom', filter.eventTimeFrom)
        if (filter.eventTimeTo) params.append('eventTimeTo', filter.eventTimeTo)
        
        // Pagination
        if (filter.page) params.append('page', String(filter.page))
        if (filter.pageSize) params.append('pageSize', String(filter.pageSize))
        
        // Sorting
        if (filter.sortBy) params.append('sortBy', filter.sortBy)
        if (filter.sortDescending !== undefined) params.append('sortDescending', String(filter.sortDescending))
      }
      
      const queryString = params.toString()
      const url = queryString ? `${BASE_URL}?${queryString}` : BASE_URL
      
      const response = await apiClient.get<EventPagedResponse>(url)
      return response.data
    } catch (error) {
      console.error('Error fetching events:', error)
      throw error
    }
  },

  /**
   * Get a single event by ID
   * @param id - Event ID
   * @returns Single event with full details
   */
  getById: async (id: number): Promise<Event> => {
    try {
      const response = await apiClient.get<Event>(`${BASE_URL}/${id}`)
      return response.data
    } catch (error) {
      console.error(`Error fetching event ${id}:`, error)
      throw error
    }
  },

  /**
   * Create a new event
   * @param data - Event creation data
   * @returns Created event
   */
  create: async (data: EventCreate): Promise<Event> => {
    try {
      const response = await apiClient.post<Event>(BASE_URL, data)
      return response.data
    } catch (error) {
      console.error('Error creating event:', error)
      throw error
    }
  },

  /**
   * Update an existing event
   * @param id - Event ID
   * @param data - Event update data
   * @returns Updated event
   */
  update: async (id: number, data: EventUpdate): Promise<Event> => {
    try {
      const response = await apiClient.put<Event>(`${BASE_URL}/${id}`, data)
      return response.data
    } catch (error) {
      console.error(`Error updating event ${id}:`, error)
      throw error
    }
  },

  /**
   * Delete an event
   * @param id - Event ID
   * @returns Success status
   */
  delete: async (id: number): Promise<void> => {
    try {
      await apiClient.delete(`${BASE_URL}/${id}`)
    } catch (error) {
      console.error(`Error deleting event ${id}:`, error)
      throw error
    }
  },

  /**
   * Acknowledge an event
   * @param id - Event ID
   * @param request - Acknowledgement request with optional notes
   * @returns Updated event
   */
  acknowledge: async (id: number, request?: EventAcknowledgeRequest): Promise<Event> => {
    try {
      const response = await apiClient.post<Event>(
        `${BASE_URL}/${id}/acknowledge`,
        request || {}
      )
      return response.data
    } catch (error) {
      console.error(`Error acknowledging event ${id}:`, error)
      throw error
    }
  },

  /**
   * Resolve an event
   * @param id - Event ID
   * @param request - Resolution request with resolution text and optional notes
   * @returns Updated event
   */
  resolve: async (id: number, request: EventResolveRequest): Promise<Event> => {
    try {
      const response = await apiClient.post<Event>(
        `${BASE_URL}/${id}/resolve`,
        request
      )
      return response.data
    } catch (error) {
      console.error(`Error resolving event ${id}:`, error)
      throw error
    }
  },

  /**
   * Get events assigned to the current user
   * @param filter - Optional filter criteria
   * @returns Paginated list of assigned events
   */
  getMyAssignments: async (filter?: EventFilter): Promise<EventPagedResponse> => {
    try {
      const params = new URLSearchParams()
      
      if (filter) {
        if (filter.page) params.append('page', String(filter.page))
        if (filter.pageSize) params.append('pageSize', String(filter.pageSize))
        if (filter.status) params.append('status', filter.status)
        if (filter.priority) params.append('priority', filter.priority)
      }
      
      const queryString = params.toString()
      const url = queryString 
        ? `${BASE_URL}/my-assignments?${queryString}` 
        : `${BASE_URL}/my-assignments`
      
      const response = await apiClient.get<EventPagedResponse>(url)
      return response.data
    } catch (error) {
      console.error('Error fetching my assignments:', error)
      throw error
    }
  },

  /**
   * Get event statistics
   * @param filter - Optional filter to scope statistics
   * @returns Event statistics summary
   */
  getStatistics: async (filter?: EventFilter): Promise<EventStats> => {
    try {
      const params = new URLSearchParams()
      
      if (filter) {
        if (filter.startDate) params.append('startDate', filter.startDate)
        if (filter.endDate) params.append('endDate', filter.endDate)
        if (filter.portId) params.append('portId', String(filter.portId))
      }
      
      const queryString = params.toString()
      const url = queryString 
        ? `${BASE_URL}/statistics?${queryString}` 
        : `${BASE_URL}/statistics`
      
      const response = await apiClient.get<EventStats>(url)
      return response.data
    } catch (error) {
      console.error('Error fetching event statistics:', error)
      throw error
    }
  },

  /**
   * Mark events as read
   * @param eventIds - Array of event IDs to mark as read
   * @returns Success status
   */
  markAsRead: async (eventIds: number[]): Promise<void> => {
    try {
      await apiClient.post(`${BASE_URL}/mark-read`, { eventIds })
    } catch (error) {
      console.error('Error marking events as read:', error)
      throw error
    }
  },

  /**
   * Get unresolved events count
   * @returns Count of unresolved events
   */
  getUnresolvedCount: async (): Promise<number> => {
    try {
      const response = await apiClient.get<{ count: number }>(`${BASE_URL}/unresolved-count`)
      return response.data.count
    } catch (error) {
      console.error('Error fetching unresolved count:', error)
      throw error
    }
  },

  /**
   * Get events by type
   * @param eventType - Event type filter
   * @param filter - Additional filter criteria
   * @returns Filtered events
   */
  getByType: async (eventType: string, filter?: EventFilter): Promise<EventPagedResponse> => {
    try {
      const updatedFilter = { ...filter, eventType }
      return await eventApi.getAll(updatedFilter)
    } catch (error) {
      console.error(`Error fetching events by type ${eventType}:`, error)
      throw error
    }
  },

  /**
   * Get events by severity
   * @param severity - Event severity filter
   * @param filter - Additional filter criteria
   * @returns Filtered events
   */
  getBySeverity: async (severity: string, filter?: EventFilter): Promise<EventPagedResponse> => {
    try {
      const updatedFilter = { ...filter, severity }
      return await eventApi.getAll(updatedFilter)
    } catch (error) {
      console.error(`Error fetching events by severity ${severity}:`, error)
      throw error
    }
  },

  /**
   * Get events requiring action
   * @param filter - Optional filter criteria
   * @returns Events requiring action
   */
  getRequiringAction: async (filter?: EventFilter): Promise<EventPagedResponse> => {
    try {
      const updatedFilter = { ...filter, requiresAction: true }
      return await eventApi.getAll(updatedFilter)
    } catch (error) {
      console.error('Error fetching events requiring action:', error)
      throw error
    }
  },

  /**
   * Get unresolved events
   * @param filter - Optional filter criteria
   * @returns Unresolved events
   */
  getUnresolved: async (filter?: EventFilter): Promise<EventPagedResponse> => {
    try {
      const updatedFilter = { ...filter, isResolved: false }
      return await eventApi.getAll(updatedFilter)
    } catch (error) {
      console.error('Error fetching unresolved events:', error)
      throw error
    }
  },

  /**
   * Export events to CSV/JSON
   * @param filter - Filter criteria for export
   * @param format - Export format ('csv' or 'json')
   * @returns Blob data for download
   */
  export: async (filter?: EventFilter, format: 'csv' | 'json' = 'csv'): Promise<Blob> => {
    try {
      const params = new URLSearchParams()
      params.append('format', format)
      
      if (filter) {
        if (filter.startDate) params.append('startDate', filter.startDate)
        if (filter.endDate) params.append('endDate', filter.endDate)
        if (filter.eventType) params.append('eventType', filter.eventType)
      }
      
      const response = await apiClient.get(`${BASE_URL}/export?${params.toString()}`, {
        responseType: 'blob'
      })
      
      return response.data
    } catch (error) {
      console.error('Error exporting events:', error)
      throw error
    }
  }
}

export default eventApi
