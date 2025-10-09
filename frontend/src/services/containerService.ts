import { httpClient } from './http';
import type {
  Container,
  ContainerFilters,
  ContainerCreateRequest,
  ContainerUpdateRequest,
  ContainerStats,
  PaginatedResponse,
  BulkStatusUpdate,
  BulkUpdateResult
} from '../types/container';

export class ContainerService {
  private readonly endpoint = '/containers';

  async getContainers(filters: ContainerFilters = {}): Promise<PaginatedResponse<Container>> {
    console.log('ContainerService: Getting containers with filters:', filters);
    
    const params = new URLSearchParams();
    Object.entries(filters).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        params.append(key, value.toString());
      }
    });
    
    const url = `${this.endpoint}?${params.toString()}`;
    console.log('ContainerService: Request URL:', url);
    
    try {
      const response = await httpClient.get(url);
      console.log('ContainerService: Raw response:', response.data);
      
      return this.normalizeResponse(response.data, filters);
    } catch (error) {
      console.error('ContainerService: Error fetching containers:', error);
      
      if (error.response?.status === 404) {
        return this.emptyPaginatedResponse();
      }
      
      throw error;
    }
  }

  async getContainer(id: string): Promise<Container | null> {
    try {
      const response = await httpClient.get(`${this.endpoint}/${id}`);
      return response.data.data || response.data;
    } catch (error) {
      console.error(`ContainerService: Error fetching container ${id}:`, error);
      return null;
    }
  }

  async getStatistics(): Promise<ContainerStats> {
    try {
      const response = await httpClient.get(`${this.endpoint}/statistics`);
      return response.data.data || response.data;
    } catch (error) {
      console.error('ContainerService: Error fetching statistics:', error);
      throw error;
    }
  }

  async create(data: ContainerCreateRequest): Promise<Container> {
    console.log('ContainerService: Creating container:', data);
    
    const payload = this.prepareCreatePayload(data);
    console.log('ContainerService: Sending payload:', payload);
    
    try {
      const response = await httpClient.post(this.endpoint, payload);
      console.log('ContainerService: Create response:', response.data);
      return response.data.data || response.data;
    } catch (error) {
      console.error('ContainerService: Error creating container:', error);
      throw error;
    }
  }

  async update(id: string, data: ContainerUpdateRequest): Promise<Container> {
    console.log('ContainerService: Updating container:', id, data);
    
    const payload = this.prepareUpdatePayload(data);
    console.log('ContainerService: Sending update payload:', payload);
    
    try {
      const response = await httpClient.put(`${this.endpoint}/${id}`, payload);
      console.log('ContainerService: Update response:', response.data);
      return response.data.data || response.data;
    } catch (error) {
      console.error('ContainerService: Error updating container:', error);
      throw error;
    }
  }

  async delete(id: string): Promise<boolean> {
    try {
      await httpClient.delete(`${this.endpoint}/${id}`);
      return true;
    } catch (error) {
      console.error(`ContainerService: Error deleting container ${id}:`, error);
      throw error;
    }
  }

  async bulkUpdateStatus(update: BulkStatusUpdate): Promise<BulkUpdateResult> {
    try {
      const response = await httpClient.patch(`${this.endpoint}/bulk-status`, update);
      return response.data.data || response.data;
    } catch (error) {
      console.error('ContainerService: Error bulk updating containers:', error);
      throw error;
    }
  }

  async exportContainers(filters: ContainerFilters = {}): Promise<Blob> {
    try {
      const params = new URLSearchParams();
      Object.entries(filters).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          params.append(key, String(value));
        }
      });

      const response = await httpClient.get(`${this.endpoint}/export?${params.toString()}`, {
        responseType: 'blob'
      });
      return response.data;
    } catch (error) {
      console.error('ContainerService: Error exporting containers:', error);
      throw error;
    }
  }

  // Legacy method for backward compatibility
  async getAll(): Promise<{ data: Container[] }> {
    try {
      const response = await httpClient.get(`${this.endpoint}/all`);
      const raw = response.data?.data ?? response.data ?? [];
      const data: Container[] = Array.isArray(raw) ? raw : [];
      return { data };
    } catch (error) {
      console.error('ContainerService: Error fetching all containers:', error);
      return { data: [] };
    }
  }

  // Private helper methods
  private normalizeResponse(responseData: any, filters: ContainerFilters): PaginatedResponse<Container> {
    if (responseData.success && responseData.data) {
      return responseData.data;
    } else if (responseData.data && Array.isArray(responseData.data)) {
      return this.convertToPaginated(responseData.data, filters);
    } else if (Array.isArray(responseData)) {
      return this.convertToPaginated(responseData, filters);
    } else if (responseData.data) {
      return responseData;
    } else {
      console.error('ContainerService: Unexpected response format:', responseData);
      throw new Error('Unexpected response format from server');
    }
  }

  private convertToPaginated(data: Container[], filters: ContainerFilters): PaginatedResponse<Container> {
    const page = filters.page || 1;
    const pageSize = filters.pageSize || 25;
    
    return {
      data,
      totalCount: data.length,
      page,
      pageSize,
      totalPages: Math.ceil(data.length / pageSize),
      hasNextPage: false,
      hasPreviousPage: false
    };
  }

  private emptyPaginatedResponse(): PaginatedResponse<Container> {
    return {
      data: [],
      totalCount: 0,
      page: 1,
      pageSize: 25,
      totalPages: 0,
      hasNextPage: false,
      hasPreviousPage: false
    };
  }

  private prepareCreatePayload(data: ContainerCreateRequest) {
    return {
      containerId: data.containerId,
      cargoType: data.cargoType || '',
      cargoDescription: data.cargoDescription || '',
      type: data.type,
      status: data.status,
      condition: data.condition || 'Good',
      currentLocation: data.currentLocation,
      destination: data.destination || '',
      weight: parseFloat(data.weight?.toString() || '0') || 0,
      size: data.size || '',
      temperature: data.temperature ? parseFloat(data.temperature.toString()) : null,
      shipId: data.shipId ? parseInt(data.shipId.toString()) : null
    };
  }

  private prepareUpdatePayload(data: ContainerUpdateRequest) {
    return {
      cargoType: data.cargoType || '',
      cargoDescription: data.cargoDescription || '',
      type: data.type,
      status: data.status,
      condition: data.condition || 'Good',
      currentLocation: data.currentLocation,
      destination: data.destination || '',
      weight: parseFloat(data.weight?.toString() || '0') || 0,
      size: data.size || '',
      temperature: data.temperature ? parseFloat(data.temperature.toString()) : null,
      shipId: data.shipId ? parseInt(data.shipId.toString()) : null
    };
  }
}

export const containerService = new ContainerService();
