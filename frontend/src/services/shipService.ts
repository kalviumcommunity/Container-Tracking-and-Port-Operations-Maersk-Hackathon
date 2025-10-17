// Ship interface defined below in this file (removed incorrect import)

export interface Ship {
  id: number;
  shipId?: number; // Backend might use different field names
  name: string;
  status?: string;
  capacity?: number;
}

export class ShipService {
  private readonly endpoint = '/ships';

  async getAll(): Promise<{ data: Ship[] }> {
    try {
      const { api } = await import('./api');
      const response = await api.get(this.endpoint);

      // Handle different response formats from backend
      let ships = response.data?.data || response.data || [];

      // Normalize ship data to ensure consistent structure
      if (Array.isArray(ships)) {
        ships = ships.map(ship => ({
          id: ship.shipId || ship.id,
          shipId: ship.shipId || ship.id,
          name: ship.name || `Ship ${ship.shipId || ship.id}`,
          status: ship.status || 'Unknown',
          capacity: ship.capacity || 0
        }));
      }

      return { data: ships };
    } catch (error) {
      console.error('Error loading ships from API:', error);

      // Fallback to mock data if API fails
      const mockShips = [
        { id: 1, shipId: 1, name: 'Maersk Edinburgh', status: 'Docked', capacity: 13092 },
        { id: 2, shipId: 2, name: 'MSC Oscar', status: 'At Sea', capacity: 19224 },
        { id: 3, shipId: 3, name: 'CMA CGM Bougainville', status: 'Loading', capacity: 18000 },
        { id: 4, shipId: 4, name: 'Ever Given', status: 'Docked', capacity: 20124 }
      ];

      return { data: mockShips };
    }
  }

  async getById(id: number): Promise<Ship | null> {
    try {
      const { api } = await import('./api');
      const response = await api.get(`${this.endpoint}/${id}`);

      const ship = response.data?.data || response.data;
      if (ship) {
        return {
          id: ship.shipId || ship.id,
          shipId: ship.shipId || ship.id,
          name: ship.name,
          status: ship.status,
          capacity: ship.capacity
        };
      }
      return null;
    } catch (error) {
      console.error(`Error loading ship ${id}:`, error);
      return null;
    }
  }

  async create(shipData: any): Promise<Ship> {
    try {
      const { api } = await import('./api');
      const response = await api.post(this.endpoint, shipData);
      return response.data?.data || response.data;
    } catch (error) {
      console.error('Error creating ship:', error);
      throw error;
    }
  }

  async update(id: number, shipData: any): Promise<Ship> {
    try {
      const { api } = await import('./api');
      const response = await api.put(`${this.endpoint}/${id}`, shipData);
      return response.data?.data || response.data;
// duplicate trailing code removed
    } catch (error) {
      console.error('Error updating ship:', error);
      throw error;
    }
  }
}

export const shipService = new ShipService();
