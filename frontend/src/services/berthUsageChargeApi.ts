import { apiClient } from './api'
import type { BerthUsageCharge, BerthUsageChargeCreateUpdate } from '../types/berthUsageCharge'
import type { ApiResponse } from './api'

/**
 * BerthUsageCharge API Service aligned with backend BerthUsageChargesController
 * Handles billing and payment tracking for berth usage
 */
export const berthUsageChargeApi = {
  /**
   * Get all berth usage charges
   */
  async getAll(): Promise<{ data: BerthUsageCharge[] }> {
    try {
      const response = await apiClient.get<ApiResponse<BerthUsageCharge[]>>('/berth-usage-charges')
      return { data: response.data.data || [] }
    } catch (error) {
      console.error('Error fetching berth usage charges:', error)
      return { data: [] }
    }
  },

  /**
   * Get berth usage charge by ID
   */
  async getById(id: number): Promise<{ data: BerthUsageCharge | null }> {
    try {
      const response = await apiClient.get<ApiResponse<BerthUsageCharge>>(`/berth-usage-charges/${id}`)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error fetching berth usage charge ${id}:`, error)
      return { data: null }
    }
  },

  /**
   * Get charges by berth assignment ID
   */
  async getByAssignment(assignmentId: number): Promise<{ data: BerthUsageCharge[] }> {
    try {
      const response = await apiClient.get<ApiResponse<BerthUsageCharge[]>>(`/berth-usage-charges/assignment/${assignmentId}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching charges for assignment ${assignmentId}:`, error)
      return { data: [] }
    }
  },

  /**
   * Get charges by payment status
   */
  async getByPaymentStatus(status: string): Promise<{ data: BerthUsageCharge[] }> {
    try {
      const response = await apiClient.get<ApiResponse<BerthUsageCharge[]>>(`/berth-usage-charges/payment-status/${status}`)
      return { data: response.data.data || [] }
    } catch (error) {
      console.error(`Error fetching charges by status ${status}:`, error)
      return { data: [] }
    }
  },

  /**
   * Create a new berth usage charge
   */
  async create(data: BerthUsageChargeCreateUpdate): Promise<{ data: BerthUsageCharge | null }> {
    try {
      const response = await apiClient.post<ApiResponse<BerthUsageCharge>>('/berth-usage-charges', data)
      return { data: response.data.data || null }
    } catch (error) {
      console.error('Error creating berth usage charge:', error)
      throw error
    }
  },

  /**
   * Update an existing berth usage charge
   */
  async update(id: number, data: BerthUsageChargeCreateUpdate): Promise<{ data: BerthUsageCharge | null }> {
    try {
      const response = await apiClient.put<ApiResponse<BerthUsageCharge>>(`/berth-usage-charges/${id}`, data)
      return { data: response.data.data || null }
    } catch (error) {
      console.error(`Error updating berth usage charge ${id}:`, error)
      throw error
    }
  },

  /**
   * Delete a berth usage charge
   */
  async delete(id: number): Promise<boolean> {
    try {
      await apiClient.delete(`/berth-usage-charges/${id}`)
      return true
    } catch (error) {
      console.error(`Error deleting berth usage charge ${id}:`, error)
      return false
    }
  },

  /**
   * Calculate total revenue from charges
   */
  async calculateRevenue(startDate?: string, endDate?: string): Promise<{ data: number }> {
    try {
      const params = new URLSearchParams()
      if (startDate) params.append('startDate', startDate)
      if (endDate) params.append('endDate', endDate)
      
      const response = await apiClient.get<ApiResponse<number>>(`/berth-usage-charges/revenue?${params}`)
      return { data: response.data.data || 0 }
    } catch (error) {
      console.error('Error calculating revenue:', error)
      return { data: 0 }
    }
  }
}
