import { apiClient } from './api'

export interface BerthAssignment {
  assignmentId: number
  berthId: number
  shipId: number
  assignmentType: string
  status: string
  scheduledArrival?: string
  scheduledDeparture?: string
  actualArrival?: string
  actualDeparture?: string
}

export const berthAssignmentApi = {
  async getAll(): Promise<{ data: BerthAssignment[] }> {
    try {
      const response = await apiClient.get('/berth-assignments')
      return { data: response.data.data || response.data || [] }
    } catch (error) {
      console.error('Error fetching berth assignments:', error)
      return { data: [] }
    }
  },

  async create(assignmentData: Partial<BerthAssignment>): Promise<BerthAssignment> {
    const response = await apiClient.post('/berth-assignments', assignmentData)
    return response.data.data || response.data
  },

  async update(id: number, assignmentData: Partial<BerthAssignment>): Promise<BerthAssignment> {
    const response = await apiClient.put(`/berth-assignments/${id}`, assignmentData)
    return response.data.data || response.data
  }
}
