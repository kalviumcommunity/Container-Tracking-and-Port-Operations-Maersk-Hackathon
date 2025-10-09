import { httpClient } from './http';

export interface Ship {
  id: number;
  name: string;
  status?: string;
  capacity?: number;
}

export class ShipService {
  private readonly endpoint = '/ships';

  async getAll(): Promise<{ data: Ship[] }> {
    try {
      const response = await httpClient.get(this.endpoint);
      const data = response.data.data || response.data || [];
      return { data };
    } catch (error) {
      console.error('ShipService: Error fetching ships:', error);
      return { data: [] };
    }
  }

  async getById(id: number): Promise<Ship | null> {
    try {
      const response = await httpClient.get(`${this.endpoint}/${id}`);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`ShipService: Error fetching ship ${id}:`, error);
      return null;
    }
  }
}

export const shipService = new ShipService();
