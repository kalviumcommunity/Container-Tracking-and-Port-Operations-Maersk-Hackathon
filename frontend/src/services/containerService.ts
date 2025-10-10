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

  // Use shared API client consistently
  private async getApiClient() {
    const { api } = await import('./api');
    return api;
  }

  async getContainers(filters: ContainerFilters = {}): Promise<PaginatedResponse<Container>> {
    const api = await this.getApiClient();
    
    const params = new URLSearchParams();
    Object.entries(filters).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        params.append(key, value.toString());
      }
    });
    
    const url = `${this.endpoint}?${params.toString()}`;
    
    try {
      const response = await api.get(url);
      return this.normalizeResponse(response.data, filters);
    } catch (error) {
      if (error.response?.status === 404) {
        return this.emptyPaginatedResponse();
      }
      throw error;
    }
  }

  async getById(id: string): Promise<Container | null> {
    try {
      const api = await this.getApiClient();
      const response = await api.get(`${this.endpoint}/${id}`);
      return response.data.data || response.data;
    } catch (error) {
      return null;
    }
  }

  async getStatistics(): Promise<ContainerStats> {
    try {
      const api = await this.getApiClient();
      const response = await api.get(`${this.endpoint}/statistics`);
      return response.data.data || response.data;
    } catch (error) {
      throw error;
    }
  }

  async create(data: ContainerCreateRequest): Promise<Container> {
    const api = await this.getApiClient();
    const payload = this.prepareCreatePayload(data);
    
    try {
      const response = await api.post(this.endpoint, payload);
      return response.data.data || response.data;
    } catch (error) {
      throw error;
    }
  }

  async update(id: string, data: ContainerUpdateRequest): Promise<Container> {
    const api = await this.getApiClient();
    const payload = this.prepareUpdatePayload(data);
    
    try {
      const response = await api.put(`${this.endpoint}/${id}`, payload);
      return response.data.data || response.data;
    } catch (error) {
      throw error;
    }
  }

  async delete(id: string): Promise<boolean> {
    try {
      const api = await this.getApiClient();
      await api.delete(`${this.endpoint}/${id}`);
      return true;
    } catch (error) {
      throw error;
    }
  }

  async bulkUpdateStatus(update: BulkStatusUpdate): Promise<BulkUpdateResult> {
    try {
      const api = await this.getApiClient();
      const response = await api.patch(`${this.endpoint}/bulk-status`, update);
      return response.data.data || response.data;
    } catch (error) {
      throw error;
    }
  }

  async exportContainers(filters: ContainerFilters = {}): Promise<Blob> {
    try {
      const api = await this.getApiClient();
      const params = new URLSearchParams();
      Object.entries(filters).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          params.append(key, String(value));
        }
      });

      const response = await api.get(`${this.endpoint}/export?${params.toString()}`, {
        responseType: 'blob'
      });
      return response.data;
    } catch (error) {
      throw error;
    }
  }

  // Legacy method for backward compatibility
  async getAll(): Promise<{ data: Container[] }> {
    try {
      const api = await this.getApiClient();
      const response = await api.get(`${this.endpoint}/all`);
      const raw = response.data?.data ?? response.data ?? [];
      const data: Container[] = Array.isArray(raw) ? raw : [];
      return { data };
    } catch (error) {
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
      maxWeight: data.maxWeight ? parseFloat(data.maxWeight.toString()) : null,
      size: data.size || '',
      temperature: data.temperature ? parseFloat(data.temperature.toString()) : null,
      coordinates: data.coordinates || '',
      estimatedArrival: data.estimatedArrival || null,
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
      weight: data.weight ? parseFloat(data.weight.toString()) : 0,
      maxWeight: data.maxWeight ? parseFloat(data.maxWeight.toString()) : null,
      size: data.size || '',
      temperature: data.temperature ? parseFloat(data.temperature.toString()) : null,
      coordinates: data.coordinates || '',
      estimatedArrival: data.estimatedArrival || null,
      shipId: data.shipId ? parseInt(data.shipId.toString()) : null
    };
  }
}

export const containerService = new ContainerService();


