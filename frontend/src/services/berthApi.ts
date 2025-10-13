import { apiClient } from './api'

export interface Berth {
  berthId: number
  name: string
  identifier: string
  type: string
  status: string
  capacity: number
  currentLoad: number
  portId: number
  portName?: string
}

export const berthApi = {
  async getAll(): Promise<{ data: Berth[] }> {
    try {
      const response = await apiClient.get('/berths')
      return { data: response.data.data || response.data || [] }
    } catch (error) {
      console.error('Error fetching berths:', error)
      return { data: [] }
    }
  },

  async getByPort(portId: number): Promise<{ data: Berth[] }> {
    try {
      const response = await apiClient.get(`/ports/${portId}/berths`)
      return { data: response.data.data || response.data || [] }
    } catch (error) {
      console.error('Error fetching port berths:', error)
      return { data: [] }
    }
  },

  async create(berthData: Partial<Berth>): Promise<Berth> {
    const response = await apiClient.post('/berths', berthData)
    return response.data.data || response.data
  },

  async update(id: number, berthData: Partial<Berth>): Promise<Berth> {
    const response = await apiClient.put(`/berths/${id}`, berthData)
    return response.data.data || response.data
  }
}
