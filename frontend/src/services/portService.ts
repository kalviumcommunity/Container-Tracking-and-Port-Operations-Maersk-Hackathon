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
      const { api } = await import('./api');
      const response = await api.get(this.endpoint);
      const data = response.data.data || response.data || [];
      return { data };
    } catch (error) {
      return { data: [] };
    }
  }

  async getById(id: number): Promise<Port | null> {
    try {
      const { api } = await import('./api');
      const response = await api.get(`${this.endpoint}/${id}`);
      return response.data.data || response.data;
    } catch (error) {
      return null;
    }
  }
}

export const portService = new PortService();
