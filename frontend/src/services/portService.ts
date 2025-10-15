import type { Port, PortCreateUpdate, PortDetail } from '../types/port'
import type { ApiResponse } from './api'

/**
 * Enhanced Port API Service aligned with backend PortsController
 */
export class PortService {
  private readonly endpoint = '/ports';

  async getAll(): Promise<{ data: Port[] }> {
    try {
      const { api } = await import('./api');
      const response = await api.get<ApiResponse<Port[]>>(this.endpoint);
      const data = response.data.data || [];
      return { data };
    } catch (error) {
      console.error('Error fetching ports:', error);
      return { data: [] };
    }
  }

  async getById(id: number): Promise<{ data: Port | null }> {
    try {
      const { api } = await import('./api');
      const response = await api.get<ApiResponse<Port>>(`${this.endpoint}/${id}`);
      return { data: response.data.data || null };
    } catch (error) {
      console.error(`Error fetching port ${id}:`, error);
      return { data: null };
    }
  }

  /**
   * Get detailed port information with berths
   */
  async getDetails(id: number): Promise<{ data: PortDetail | null }> {
    try {
      const { api } = await import('./api');
      const response = await api.get<ApiResponse<PortDetail>>(`${this.endpoint}/${id}/details`);
      return { data: response.data.data || null };
    } catch (error) {
      console.error(`Error fetching port details ${id}:`, error);
      return { data: null };
    }
  }

  /**
   * Get ports by location
   */
  async getByLocation(location: string): Promise<{ data: Port[] }> {
    try {
      const { api } = await import('./api');
      const response = await api.get<ApiResponse<Port[]>>(`${this.endpoint}/location/${encodeURIComponent(location)}`);
      return { data: response.data.data || [] };
    } catch (error) {
      console.error(`Error fetching ports by location ${location}:`, error);
      return { data: [] };
    }
  }

  /**
   * Create new port
   */
  async create(portData: PortCreateUpdate): Promise<{ data: Port }> {
    try {
      const { api } = await import('./api');
      const response = await api.post<ApiResponse<Port>>(this.endpoint, portData);
      return { data: response.data.data };
    } catch (error) {
      console.error('Error creating port:', error);
      throw error;
    }
  }

  /**
   * Update existing port
   */
  async update(id: number, portData: PortCreateUpdate): Promise<{ data: Port }> {
    try {
      const { api } = await import('./api');
      const response = await api.put<ApiResponse<Port>>(`${this.endpoint}/${id}`, portData);
      return { data: response.data.data };
    } catch (error) {
      console.error(`Error updating port ${id}:`, error);
      throw error;
    }
  }

  /**
   * Delete port
   */
  async delete(id: number): Promise<{ success: boolean; message?: string }> {
    try {
      const { api } = await import('./api');
      const response = await api.delete<ApiResponse<any>>(`${this.endpoint}/${id}`);
      return { 
        success: true, 
        message: response.data.message || 'Port deleted successfully' 
      };
    } catch (error) {
      console.error(`Error deleting port ${id}:`, error);
      throw error;
    }
  }
}

export const portService = new PortService();
