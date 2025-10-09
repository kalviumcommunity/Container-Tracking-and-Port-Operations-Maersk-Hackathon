import { httpClient } from './http';

export interface Port {
  id: number;
  name: string;
  location: string;
  code?: string;
}

export class PortService {
  private readonly endpoint = '/ports';

  async getAll(): Promise<{ data: Port[] }> {
    try {
      const response = await httpClient.get(this.endpoint);
      const data = response.data.data || response.data || [];
      return { data };
    } catch (error) {
      console.error('PortService: Error fetching ports:', error);
      return { data: [] };
    }
  }

  async getById(id: number): Promise<Port | null> {
    try {
      const response = await httpClient.get(`${this.endpoint}/${id}`);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`PortService: Error fetching port ${id}:`, error);
      return null;
    }
  }
}

export const portService = new PortService();
