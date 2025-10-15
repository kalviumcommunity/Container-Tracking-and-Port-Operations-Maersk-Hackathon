export interface Container {
  containerId: string;
  cargoType: string;
  cargoDescription?: string;
  type: string;
  status: string;
  condition?: string;
  currentLocation: string;
  origin?: string;
  destination?: string;
  weight?: number;
  maxWeight?: number;
  declaredValue?: number;
  size?: string;
  temperature?: number;
  coordinates?: string;
  estimatedArrival?: string;
  createdAt: string;
  updatedAt: string;
  shipId?: number;
  shipName?: string;
  portId?: number;
  portName?: string;
}

export interface ContainerFilters {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDirection?: string;
  status?: string;
  type?: string;
  cargoType?: string;
  currentLocation?: string;
  destination?: string;
  shipId?: number | string;
  createdAfter?: string;
  createdBefore?: string;
  minWeight?: number | string;
  maxWeight?: number | string;
  searchTerm?: string;
  condition?: string;
  size?: string;
  coordinates?: string;
}

export interface ContainerCreateRequest {
  containerId: string;
  cargoType: string;
  cargoDescription?: string;
  type: string;
  status: string;
  condition?: string;
  currentLocation: string;
  destination?: string;
  weight?: number | string;
  maxWeight?: number | string;
  size?: string;
  temperature?: number | string;
  coordinates?: string;
  estimatedArrival?: string;
  shipId?: number | string;
}

export interface ContainerUpdateRequest {
  cargoType?: string;
  cargoDescription?: string;
  type?: string;
  status?: string;
  condition?: string;
  currentLocation?: string;
  destination?: string;
  weight?: number | string | null; // Allow null to support explicit 0 values
  maxWeight?: number | string | null;
  size?: string;
  temperature?: number | string | null;
  coordinates?: string;
  estimatedArrival?: string;
  shipId?: number | string | null;
}

export interface ContainerStats {
  totalContainers: number;
  availableContainers: number;
  inTransitContainers: number;
  atPortContainers: number;
  loadingContainers: number;
  unloadingContainers: number;
  containersByType: Record<string, number>;
  containersByStatus: Record<string, number>;
  containersByLocation: Record<string, number>;
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface BulkStatusUpdate {
  containerIds: string[];
  newStatus: string;
  reason?: string;
}

export interface BulkUpdateResult {
  successCount: number;
  failedCount: number;
  failedContainerIds: string[];
  errorMessages: string[];
}
